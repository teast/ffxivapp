// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellView.xaml.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ShellView.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FFXIVAPP.Client
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Timers;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media;
    using Avalonia.Threading;
    using FFXIVAPP.Client.Helpers;
    using FFXIVAPP.Client.Models;
    using FFXIVAPP.Client.Views;
    using FFXIVAPP.Common.Helpers;
    using FFXIVAPP.Common.Models;
    using FFXIVAPP.Common.Utilities;
    using FFXIVAPP.ResourceFiles;
    using NLog;

    /// <summary>
    ///     Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window {
        public static ShellView View;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Timer _spinner;

        public ComboBox LanguageSelect { get; }
        public Button save { get; }
        public Button screenshot { get; }
        public TabControl ShellViewTC { get; }
        public TabItem MainTI { get; }
        public MainView MainV { get; }
        public TabItem PluginsTI { get; }
        public TabControl PluginsTC { get; }
        public TabItem SettingsTI { get; }
        public SettingsView SettingsV { get; }
        public TabItem UpdateTI { get; }
        public Image PluginUpdateSpinner { get; }
        public UpdateView UpdateV { get; }
        public TabItem AboutTI { get; }
        public AboutView AboutV { get; }
        public Button BtnMinimize { get; }
        public Button BtnMaximize { get; }
        public Button BtnClose { get; }
        public Image ImageMaximize { get; }

        public ObservableCollection<TabItem> PluginsTCItems { get; }
        
        public ShellView() {
            View = this;
            View.Topmost = true;
            this.InitializeComponent();
            LanguageSelect = this.FindControl<ComboBox>("LanguageSelect");
            save = this.FindControl<Button>("save");
            screenshot = this.FindControl<Button>("screenshot");
            ShellViewTC = this.FindControl<TabControl>("ShellViewTC");
            MainTI = this.FindControl<TabItem>("MainTI");
            MainV = this.FindControl<MainView>("MainV");
            PluginsTI = this.FindControl<TabItem>("PluginsTI");
            PluginsTC = this.FindControl<TabControl>("PluginsTC");
            SettingsTI = this.FindControl<TabItem>("SettingsTI");
            SettingsV = this.FindControl<SettingsView>("SettingsV");
            UpdateTI = this.FindControl<TabItem>("UpdateTI");
            PluginUpdateSpinner = this.FindControl<Image>("PluginUpdateSpinner");
            UpdateV = this.FindControl<UpdateView>("UpdateV");
            AboutTI = this.FindControl<TabItem>("AboutTI");
            AboutV = this.FindControl<AboutView>("AboutV");
            BtnMinimize = this.FindControl<Button>("BtnMinimize");
            BtnMaximize = this.FindControl<Button>("BtnMaximize");
            BtnClose = this.FindControl<Button>("BtnClose");
            ImageMaximize = this.FindControl<Image>("ImageMaximize");
            
            ImageMaximize.Source = this.WindowState == WindowState.Maximized ? Theme.RestoreIcon20 : Theme.MaximizeIcon20;

            // To spin the refresh icon
            var rotate = (RotateTransform)PluginUpdateSpinner.RenderTransform;
            _spinner = new Timer(25);
            _spinner.Elapsed += (s, a) => {
                DispatcherHelper.Invoke(() => rotate.Angle = rotate.Angle + 10);
            };

            // Ask user to update game language on language change
            LanguageSelect.GetObservable(ComboBox.SelectedItemProperty).Subscribe(value => {
                ShellViewModel.Instance.SetLocaleCommand?.Execute(null);
            });

            BtnClose.Click += delegate { this.Close(); };
            BtnMinimize.Click += delegate { this.WindowState = WindowState.Minimized; };
            BtnMaximize.Click += delegate { this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized; };

            this.FindControl<DockPanel>("CustChrome").PointerPressed += (s, e) => {
                if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                    BeginMoveDrag(e);
            };
        }

        protected override void HandleWindowStateChanged(WindowState state)
        {
            if (state == WindowState.Maximized)
                ImageMaximize.Source = Theme.RestoreIcon20;
            else if (state == WindowState.Normal)
                ImageMaximize.Source = Theme.MaximizeIcon20;

            base.HandleWindowStateChanged(state);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="update"></param>
        public static void CloseApplication(bool update = false) {
            SettingsHelper.Save(update);
            foreach (PluginInstance pluginInstance in App.Plugins.Loaded.Cast<PluginInstance>().Where(pluginInstance => pluginInstance.Loaded)) {
                pluginInstance.Instance.Dispose(update);
            }

            System.Threading.Tasks.Task.Run(() => {
                SavedlLogsHelper.SaveCurrentLog(false);
                CloseDelegate(update);
            });
        }

        /// <summary>
        /// </summary>
        /// <param name="update"></param>
        private static void CloseDelegate(bool update = false) {
            // TODO: Implement this, NotifyIcon
            //AppViewModel.Instance.NotifyIcon.Visible = false;
            if (update) {
                try {
                    Process[] updaters = Process.GetProcessesByName("FFXIVAPP.Updater");
                    foreach (Process updater in updaters) {
                        updater.Kill();
                    }

                    if (File.Exists("FFXIVAPP.Updater.exe")) {
                        File.Delete("FFXIVAPP.Updater.Backup.exe");
                    }

                    File.Move("FFXIVAPP.Updater.exe", "FFXIVAPP.Updater.Backup.exe");
                }
                catch (Exception ex) {
                    Logging.Log(Logger, new LogItem(ex, true));
                }

                Process.Start("FFXIVAPP.Updater.Backup.exe", $"{AppViewModel.Instance.DownloadUri} {AppViewModel.Instance.LatestVersion}");
            }

            Environment.Exit(0);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            DispatcherHelper.Invoke(() => CloseApplication(), DispatcherPriority.Send);
            base.OnClosing(e);
        }

        internal void StartSpinner()
        {
            _spinner.Start();
        }

        internal void StopSpinner()
        {
            var rotate = (RotateTransform)PluginUpdateSpinner.RenderTransform;
            _spinner.Stop();
            DispatcherHelper.Invoke(() => rotate.Angle = 0);
        }

        /* TODO: Implement this, NotifyIcon
        private void MetroWindowStateChanged(object sender, EventArgs e) {
            switch (View.WindowState) {
                case WindowState.Minimized:
                    this.ShowInTaskbar = false;
                    AppViewModel.Instance.NotifyIcon.Text = "FFXIVAPP - Minimized";
                    AppViewModel.Instance.NotifyIcon.ContextMenu.MenuItems[0].Enabled = true;
                    break;
                case WindowState.Normal:
                    this.ShowInTaskbar = true;
                    AppViewModel.Instance.NotifyIcon.Text = "FFXIVAPP";
                    AppViewModel.Instance.NotifyIcon.ContextMenu.MenuItems[0].Enabled = false;
                    break;
            }
        }
        */
    }
}