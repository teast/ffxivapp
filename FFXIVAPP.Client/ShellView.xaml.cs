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
    using FFXIVAPP.Client.Properties;
    using FFXIVAPP.Client.Views;
    using FFXIVAPP.Common.Helpers;
    using FFXIVAPP.Common.Models;
    using FFXIVAPP.Common.Utilities;

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
        public Grid LayoutRoot { get; }
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

        public ObservableCollection<TabItem> PluginsTCItems { get; }
        
        public ShellView() {
            View = this;
            View.Topmost = true;
            this.InitializeComponent();
            LanguageSelect = this.FindControl<ComboBox>("LanguageSelect");
            save = this.FindControl<Button>("save");
            screenshot = this.FindControl<Button>("screenshot");
            LayoutRoot = this.FindControl<Grid>("LayoutRoot");
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

            // To spin the refresh icon
            var rotate = (RotateTransform)PluginUpdateSpinner.RenderTransform;
            _spinner = new Timer(25);
            _spinner.Elapsed += (s, a) => {
                DispatcherHelper.Invoke(() => rotate.Angle = rotate.Angle + 10);
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // TODO: Kickstart bootstrapper...
            System.Threading.Tasks.Task.Run(() => AppBootstrapper.Instance);
            //var p = AppBootstrapper.Instance;
        }

        public bool IsRendered { get; set; }

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
            // TODO: Implement this
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
        public override void EndInit()
        {
            base.EndInit();

            if (this.IsRendered) {
                return;
            }

            this.IsRendered = true;

            if (string.IsNullOrWhiteSpace(Settings.Default.UILanguage)) {
                Settings.Default.UILanguage = Settings.Default.GameLanguage;
            }
            else {
                LocaleHelper.Update(Settings.Default.Culture);
            }

            DispatcherHelper.Invoke(
                delegate {
                    Initializer.LoadAvailableSources();
                    Initializer.LoadAvailablePlugins();
                    Initializer.CheckUpdates();
                    Initializer.SetGlobals();

                    Initializer.StartMemoryWorkers();
                    if (Settings.Default.EnableNetworkReading && !Initializer.NetworkWorking) {
                        Initializer.StartNetworkWorker();
                    }
                });

            Initializer.GetHomePlugin();
            Initializer.UpdatePluginConstants();

            View.Topmost = Settings.Default.TopMost;

            /* TODO: Implement this
            ThemeHelper.ChangeTheme(Settings.Default.Theme, null);
            AppViewModel.Instance.NotifyIcon.Text = "FFXIVAPP";
            AppViewModel.Instance.NotifyIcon.ContextMenu.MenuItems[0].Enabled = false;
            */
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

        /* TODO: Implement this
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