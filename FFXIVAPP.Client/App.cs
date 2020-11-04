// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   App.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FFXIVAPP.Client
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Xml;
    using System.Xml.Linq;
    using Avalonia;
    using Avalonia.ReactiveUI;
    using FFXIVAPP.Client.Helpers;
    using FFXIVAPP.Client.Models;
    using FFXIVAPP.Client.Properties;
    using FFXIVAPP.Common.Helpers;
    using FFXIVAPP.Common.Models;
    using FFXIVAPP.Common.Utilities;

    using NLog;
    using NLog.Config;

    internal partial class App
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static List<object> _availableAudioDevices;

        private static IEnumerable<NetworkInterface> _availableNetworkInterfaces;

        // TODO: Implement this, Audio
        internal static IEnumerable<object> AvailableAudioDevices
        {
            get
            {
                //return _availableAudioDevices ?? (_availableAudioDevices = new List<object>(DirectSoundOut.Devices.Where(d => d.Guid != Guid.Empty)));
                return _availableAudioDevices ?? (_availableAudioDevices = new List<object>());
            }
        }

        public static string[] MArgs { get; private set; }

        internal static IEnumerable<NetworkInterface> AvailableNetworkInterfaces
        {
            get
            {
                return _availableNetworkInterfaces ?? (_availableNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces());
            }
        }

        internal static PluginHost Plugins
        {
            get
            {
                return PluginHost.Instance;
            }
        }

        /// <summary>
        /// Get root path for this program
        /// </summary>
        internal static string RootPath { get; private set; }

        /// <summary>
        ///     Application Entry Point.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            MArgs = args?.ToArray() ?? new string[0];

            ConfigureNLog();
            Settings.Default.PropertyChanged += SettingsPropertyChanged;
            Settings.Default.SettingChanging += SettingsSettingChanging;
            CheckSettings();

            try
            {
                // TODO: If we move to warp-packer we need to come up with a better solution than this
                //       because warp-packer will unpack the program into an temporary folder and execute it from there
                RootPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                AppBuilder.Configure<App>()
                                .UsePlatformDetect()
                                .LogToDebug()
                                .UseReactiveUI()
                                .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Logging.Log(Logger, new LogItem(ex, true));
            }
        }

        private static void CheckSettings()
        {
            Common.Constants.EnableNLog = Settings.Default.EnableNLog;
            try
            {
                if (!Settings.Default.Application_UpgradeRequired)
                {
                    Settings.Default.Reload();
                    return;
                }

                Settings.Default.Upgrade();
                Settings.Default.Reload();
                Settings.Default.Application_UpgradeRequired = false;
            }
            catch (Exception)
            {
                SettingsHelper.Default();
            }
        }

        /// <summary>
        /// </summary>
        private static void ConfigureNLog()
        {
            var hasLocal = false;
            const string fileName = "./FFXIVAPP.Client.exe.nlog";
            if (File.Exists(fileName))
            {
                hasLocal = true;
            }

            var resource = ResourceHelper.StreamResource(Constants.AppPack + "Resources.FFXIVAPP.Client.exe.nlog");
            if (resource == null)
            {
                return;
            }

            StringReader stringReader;
            if (hasLocal)
            {
                stringReader = new StringReader(XElement.Load(fileName).ToString());
            }
            else
            {
                stringReader = new StringReader(XElement.Load(resource).ToString());
            }

            using (XmlReader xmlReader = XmlReader.Create(stringReader))
            {
                LogManager.Configuration = new XmlLoggingConfiguration(xmlReader, null);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private static void SettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Logging.Log(Logger, $"PropertyChanged : {e.PropertyName}");
            try
            {
                switch (e.PropertyName)
                {
                    case "CharacterName":
                        Constants.CharacterName = Settings.Default.CharacterName;
                        break;
                    case "GameLanguage":
                        Constants.GameLanguage = Settings.Default.GameLanguage;
                        break;
                    case "UILanguage":
                        if (AppViewModel.Instance.UILanguages.Any(ui => ui.Language == Settings.Default.UILanguage))
                        {
                            UILanguage uiLanguage = AppViewModel.Instance.UILanguages.First(ui => ui.Language == Settings.Default.UILanguage);
                            Constants.CultureInfo = Settings.Default.Culture = uiLanguage.CultureInfo;
                            LocaleHelper.Update(Settings.Default.Culture);
                        }

                        break;
                    case "ServerName":
                        Constants.ServerName = Settings.Default.ServerName;
                        break;
                    case "EnableNLog":
                        Common.Constants.EnableNLog = Constants.EnableNLog = Settings.Default.EnableNLog;
                        break;
                    case "EnableNetworkReading":
                        Common.Constants.EnableNetworkReading = Constants.EnableNetworkReading = Settings.Default.EnableNetworkReading;
                        if (Settings.Default.EnableNetworkReading)
                        {
                            if (!Initializer.NetworkWorking && Constants.IsOpen)
                            {
                                Initializer.StartNetworkWorker();
                            }
                        }
                        else
                        {
                            Initializer.StopNetworkWorker();
                        }

                        break;
                    case "NetworkUseWinPCap":
                        if (Initializer.NetworkWorking)
                        {
                            Initializer.RefreshNetworkWorker();
                        }

                        break;
                    case "EnableHelpLabels":
                        Constants.EnableHelpLabels = Settings.Default.EnableHelpLabels;
                        break;
                    case "Theme":
                        Constants.Theme = Settings.Default.Theme;
                        break;
                    case "UIScale":
                        Constants.UIScale = Settings.Default.UIScale;
                        break;
                    case "TopMost":
                        if (ShellView.View != null)
                        {
                            ShellView.View.Topmost = Settings.Default.TopMost;
                        }

                        break;
                    case "DefaultAudioDevice":
                        if (Settings.Default.DefaultAudioDevice == "System Default")
                        {
                            Common.Constants.DefaultAudioDevice = Guid.Empty;
                        }
                        else
                        {
                            /* TODO: Implement this, Audio
                            foreach (var audioDevice in AvailableAudioDevices.Where(device => device.Guid != Guid.Empty))
                            {
                                if (audioDevice.Description == Settings.Default.DefaultAudioDevice)
                                {
                                    Common.Constants.DefaultAudioDevice = audioDevice.Guid;
                                }
                            }
                            */
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(Logger, new LogItem(ex, true));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private static void SettingsSettingChanging(object sender, SettingChangingEventArgs e)
        {
            Logging.Log(Logger, $"SettingChanging : [{e.SettingKey},{e.NewValue}]");
        }
    }
}