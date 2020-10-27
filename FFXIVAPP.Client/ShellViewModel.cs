﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ShellViewModel.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FFXIVAPP.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using Avalonia.Controls;
    using FFXIVAPP.Client.Helpers;
    using FFXIVAPP.Client.Properties;
    using FFXIVAPP.Common.ViewModelBase;

    internal sealed class ShellViewModel : INotifyPropertyChanged {
        private static Lazy<ShellViewModel> _instance = new Lazy<ShellViewModel>(() => new ShellViewModel());

        private static List<string> SupportedGameLanguages = new List<string> {
            "English",
            "Japanese",
            "French",
            "German",
            "Chinese",
            "Korean",
        };

        private TabItem _pluginsTCSelectedItem;

        private TabItem _shellViewTCSelectedItem;

        public ShellViewModel() {
            UpdateTitle();
            this.SetLocaleCommand = new DelegateCommand(SetLocale);
            this.SaveAndClearHistoryCommand = new DelegateCommand(SaveAndClearHistory);
            this.ScreenShotCommand = new DelegateCommand(ScreenShot);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public static ShellViewModel Instance {
            get {
                return _instance.Value;
            }
        }

        public ICommand SaveAndClearHistoryCommand { get; private set; }

        public ICommand ScreenShotCommand { get; private set; }

        public ICommand SetLocaleCommand { get; set; }

        /// <summary>
        /// </summary>
        private static void SaveAndClearHistory() {
            SavedlLogsHelper.SaveCurrentLog();
        }

        /// <summary>
        /// </summary>
        private static void ScreenShot() {
            try {
                /* TODO: Implement this, Screenshot
                var date = DateTime.Now.ToString("yyyy_MM_dd_HH.mm.ss_");
                var fileName = Path.Combine(AppViewModel.Instance.ScreenShotsPath, $"{date}.jpg");
                byte[] screenShot = ScreenCapture.GetJpgImage(ShellView.View, 1, 100);
                var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                using (var stream = new BinaryWriter(fileStream)) {
                    stream.Write(screenShot);
                }
                */
            }
            catch (Exception ex) {
                var title = AppViewModel.Instance.Locale["app_WarningMessage"];
                MessageBoxHelper.ShowMessageAsync(title, ex.Message);
            }
        }

        private static void SetLocale() {
            var uiLanguage = ShellView.View?.LanguageSelect.SelectedItem.ToString();
            if (string.IsNullOrWhiteSpace(uiLanguage)) {
                return;
            }

            if (uiLanguage == Settings.Default.GameLanguage) {
                return;
            }

            if (SupportedGameLanguages.Contains(uiLanguage)) {
                if (uiLanguage == Settings.Default.GameLanguage) {
                    return;
                }

                Action ok = () => {
                    Settings.Default.GameLanguage = uiLanguage;
                };
                Action cancel = () => { };
                var title = AppViewModel.Instance.Locale["app_WarningMessage"];
                var message = AppViewModel.Instance.Locale["app_UILanguageChangeWarningGeneral"];
                if (uiLanguage == "Chinese" || Settings.Default.GameLanguage == "Chinese") {
                    message = message + AppViewModel.Instance.Locale["app_UILanguageChangeWarningChinese"];
                }

                MessageBoxHelper.ShowMessageAsync(title, message, ok, cancel);
            }
            else {
                var title = AppViewModel.Instance.Locale["app_WarningMessage"];
                var message = AppViewModel.Instance.Locale["app_UILanguageChangeWarningNoGameLanguage"];
                MessageBoxHelper.ShowMessageAsync(title, message);
            }
        }

        public TabItem PluginsTCSelectedItem {
            get => _pluginsTCSelectedItem;
            set {
                _pluginsTCSelectedItem = value;

                var selectedItem = (TabItem) ShellView.View.PluginsTC.SelectedItem;
                try {
                    AppViewModel.Instance.Selected = selectedItem.Header.ToString();
                }
                catch (Exception) {
                    AppViewModel.Instance.Selected = "(NONE)";
                }

                UpdateTitle();

                RaisePropertyChanged();
            }
        }

        public TabItem ShellViewTCSelectedItem {
            get => _shellViewTCSelectedItem;
            set {
                _shellViewTCSelectedItem = value;

                UpdateTitle();

                RaisePropertyChanged();
            }
        }
        
        /// <summary>
        /// </summary>
        public static void UpdateTitle() {
            var currentMain = ((TabItem) ShellView.View.ShellViewTC.SelectedItem).Name;
            switch (currentMain) {
                case "PluginsTI":
                    AppViewModel.Instance.AppTitle = $"{AppViewModel.Instance.Selected}";
                    break;
                default:
                    AppViewModel.Instance.AppTitle = currentMain.Substring(0, currentMain.Length - 2);
                    break;
            }
        }

        private void RaisePropertyChanged([CallerMemberName,] string caller = "") {
            this.PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }
    }
}