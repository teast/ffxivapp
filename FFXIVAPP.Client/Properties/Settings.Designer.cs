﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using FFXIVAPP.Client.Models;
using Newtonsoft.Json;

namespace FFXIVAPP.Client.Properties
{

    internal delegate void SettingChangingEventHandler(object sender, SettingChangingEventArgs e);

    internal sealed class Settings : INotifyPropertyChanged
    {
        private static Lazy<Settings> defaultInstance = new Lazy<Settings>(() =>
        {
            var s = new Settings();
            s.Reload();
            return s;
        });

        public static Settings Default => defaultInstance.Value;

        public event PropertyChangedEventHandler PropertyChanged;
        public event SettingChangingEventHandler SettingChanging;

        #region fields
        private bool _enableNLog;
        private bool _application_UpgradeRequired;
        private string _characterName;
        private string _gameLanguage;
        private string _uILanguage;
        private CultureInfo _culture;
        private bool _cultureSet;
        private string _serverName;
        private bool _enableNetworkReading;
        private bool _enableHelpLabels;
        private string _theme;
        private string _uIScale;
        private string _defaultAudioDevice;
        private string _homePlugin;
        private bool _topMost;
        private bool _useLocalMemoryJSONDataCache;
        private List<string> _defaultAudioDeviceList;
        private List<string> _homePluginList;
        private string _cICUID;
        private double _actorWorkerRefresh;
        private double _chatLogWorkerRefresh;
        private double _hotBarRecastWorkerRefresh;
        private double _inventoryWorkerRefresh;
        private double _partyInfoWorkerRefresh;
        private double _playerInfoWorkerRefresh;
        private double _targetWorkerRefresh;
        private bool _saveLog;
        private ObservableCollection<string> _gameLanguageList;
        private ObservableCollection<string> _serverList;
        private ObservableCollection<PluginSourceItem> _availableSources;
        #endregion

        #region Properties
        public ObservableCollection<PluginSourceItem> AvailableSources
        {
            get => _availableSources;
            set => Set(ref _availableSources, value);
        }

        public bool EnableNLog
        {
            get => _enableNLog;
            set => Set(ref _enableNLog, value);
        }
        public bool Application_UpgradeRequired
        {
            get => _application_UpgradeRequired;
            set => Set(ref _application_UpgradeRequired, value);
        }
        public string CharacterName
        {
            get => _characterName;
            set => Set(ref _characterName, value);
        }
        public string GameLanguage
        {
            get => _gameLanguage;
            set => Set(ref _gameLanguage, value);
        }
        public string UILanguage
        {
            get => _uILanguage;
            set => Set(ref _uILanguage, value);
        }
        public CultureInfo Culture
        {
            get => _culture;
            set => Set(ref _culture, value);
        }
        public bool CultureSet
        {
            get => _cultureSet;
            set => Set(ref _cultureSet, value);
        }
        public string ServerName
        {
            get => _serverName;
            set => Set(ref _serverName, value);
        }
        public bool EnableNetworkReading
        {
            get => _enableNetworkReading;
            set => Set(ref _enableNetworkReading, value);
        }
        public bool EnableHelpLabels
        {
            get => _enableHelpLabels;
            set => Set(ref _enableHelpLabels, value);
        }
        public string Theme
        {
            get => _theme;
            set => Set(ref _theme, value);
        }
        public string UIScale
        {
            get => _uIScale;
            set => Set(ref _uIScale, value);
        }
        public string DefaultAudioDevice
        {
            get => _defaultAudioDevice;
            set => Set(ref _defaultAudioDevice, value);
        }
        public string HomePlugin
        {
            get => _homePlugin;
            set => Set(ref _homePlugin, value);
        }
        public bool TopMost
        {
            get => _topMost;
            set => Set(ref _topMost, value);
        }
        public bool UseLocalMemoryJSONDataCache
        {
            get => _useLocalMemoryJSONDataCache;
            set => Set(ref _useLocalMemoryJSONDataCache, value);
        }

        private readonly string _path;

        public List<string> DefaultAudioDeviceList
        {
            get => _defaultAudioDeviceList;
            set => Set(ref _defaultAudioDeviceList, value);
        }
        public List<string> HomePluginList
        {
            get => _homePluginList;
            set => Set(ref _homePluginList, value);
        }
        public string CICUID
        {
            get => _cICUID;
            set => Set(ref _cICUID, value);
        }
        public double ActorWorkerRefresh
        {
            get => _actorWorkerRefresh;
            set => Set(ref _actorWorkerRefresh, value);
        }
        public double ChatLogWorkerRefresh
        {
            get => _chatLogWorkerRefresh;
            set => Set(ref _chatLogWorkerRefresh, value);
        }
        public double HotBarRecastWorkerRefresh
        {
            get => _hotBarRecastWorkerRefresh;
            set => Set(ref _hotBarRecastWorkerRefresh, value);
        }
        public double InventoryWorkerRefresh
        {
            get => _inventoryWorkerRefresh;
            set => Set(ref _inventoryWorkerRefresh, value);
        }
        public double PartyInfoWorkerRefresh
        {
            get => _partyInfoWorkerRefresh;
            set => Set(ref _partyInfoWorkerRefresh, value);
        }
        public double PlayerInfoWorkerRefresh
        {
            get => _playerInfoWorkerRefresh;
            set => Set(ref _playerInfoWorkerRefresh, value);
        }
        public double TargetWorkerRefresh
        {
            get => _targetWorkerRefresh;
            set => Set(ref _targetWorkerRefresh, value);
        }
        public bool SaveLog
        {
            get => _saveLog;
            set => Set(ref _saveLog, value);
        }

        public ObservableCollection<string> GameLanguageList
        {
            get => _gameLanguageList;
            set => Set(ref _gameLanguageList, value);
        }

        public ObservableCollection<string> ServerList
        {
            get => _serverList;
            set => Set(ref _serverList, value);
        }
        #endregion

        public Settings()
        {
            object[] att = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            var companyName = ((AssemblyCompanyAttribute)att[0]).Company;
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _path = Path.Combine(appDataPath, companyName);

            GameLanguageList = new ObservableCollection<string>();
            ServerList = new ObservableCollection<string>();
            AvailableSources = new ObservableCollection<PluginSourceItem>();

            Reset();
        }

        public void Reset()
        {
            DefaultAudioDeviceList = new List<string>();
            HomePluginList = new List<string>();
            GameLanguage = "English";
            EnableNLog = true;
            GameLanguageList.Clear();
            GameLanguageList.Add("English");
            GameLanguageList.Add("Japanese");
            GameLanguageList.Add("French");
            GameLanguageList.Add("German");
            GameLanguageList.Add("Chinese");
            GameLanguageList.Add("Korean");

            #region serverlist
            ServerList.Clear();
            ServerList.Add("Adamantoise");
            ServerList.Add("Aegis");
            ServerList.Add("Alexander");
            ServerList.Add("Anima");
            ServerList.Add("Asura");
            ServerList.Add("Atomos");
            ServerList.Add("Bahamut");
            ServerList.Add("Balmung");
            ServerList.Add("Behemoth");
            ServerList.Add("Belias");
            ServerList.Add("Brynhildr");
            ServerList.Add("Cactuar");
            ServerList.Add("Carbuncle");
            ServerList.Add("Cerberus");
            ServerList.Add("Chocobo");
            ServerList.Add("Coeurl");
            ServerList.Add("Diabolos");
            ServerList.Add("Durandal");
            ServerList.Add("Excalibur");
            ServerList.Add("Exodus");
            ServerList.Add("Faerie");
            ServerList.Add("Famfrit");
            ServerList.Add("Fenrir");
            ServerList.Add("Garuda");
            ServerList.Add("Gilgamesh");
            ServerList.Add("Goblin");
            ServerList.Add("Gungnir");
            ServerList.Add("Hades");
            ServerList.Add("Hyperion");
            ServerList.Add("Ifrit");
            ServerList.Add("Ixion");
            ServerList.Add("Jenova");
            ServerList.Add("Kujata");
            ServerList.Add("Lamia");
            ServerList.Add("Leviathan");
            ServerList.Add("Lich");
            ServerList.Add("Malboro");
            ServerList.Add("Mandragora");
            ServerList.Add("Masamune");
            ServerList.Add("Mateus");
            ServerList.Add("Midgardsormr");
            ServerList.Add("Moogle");
            ServerList.Add("Odin");
            ServerList.Add("Pandaemonium");
            ServerList.Add("Phoenix");
            ServerList.Add("Ragnarok");
            ServerList.Add("Ramuh");
            ServerList.Add("Ridill");
            ServerList.Add("Sargatanas");
            ServerList.Add("Shinryu");
            ServerList.Add("Shiva");
            ServerList.Add("Siren");
            ServerList.Add("Tiamat");
            ServerList.Add("Titan");
            ServerList.Add("Tonberry");
            ServerList.Add("Typhon");
            ServerList.Add("Ultima");
            ServerList.Add("Ultros");
            ServerList.Add("Unicorn");
            ServerList.Add("Valefor");
            ServerList.Add("Yojimbo");
            ServerList.Add("Zalera");
            ServerList.Add("Zeromus");
            #endregion

            ActorWorkerRefresh = 100;
            ChatLogWorkerRefresh = 250;
            HotBarRecastWorkerRefresh = 100;
            InventoryWorkerRefresh = 100;
            PartyInfoWorkerRefresh = 1000;
            PlayerInfoWorkerRefresh = 1000;
            TargetWorkerRefresh = 100;
        }

        public void Save()
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            var file = Path.Combine(_path, "FFXIVAPP_config.json");
            File.WriteAllText(file, JsonConvert.SerializeObject(this), Encoding.UTF8);
        }

        public void Reload()
        {
            var file = Path.Combine(_path, "FFXIVAPP_config.json");
            if (!File.Exists(file))
                return;

            var content = File.ReadAllText(file);
            var config = JsonConvert.DeserializeObject<Settings>(content);

            var type = typeof(Settings);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // Hack to handle ObservableCollection...
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(ObservableCollection<>))
                {
                    var dest = property.GetValue(this) as dynamic;
                    var source = property.GetValue(config) as dynamic;
                    dest.Clear();
                    AddRange(dest, source);

                    continue;
                }

                property.SetValue(this, property.GetValue(config));
            }
        }

        private void AddRange<T>(ObservableCollection<T> dest, ObservableCollection<T> source)
        {
            foreach (var items in source.Distinct())
                dest.Add(items);
        }

        public void Upgrade()
        {

        }

        private void Set<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    internal class SettingChangingEventArgs : EventArgs
    {
        public string SettingKey { get; }
        public string NewValue { get; }

        public SettingChangingEventArgs(string settingKey, string newValue)
        {
            SettingKey = settingKey;
            NewValue = newValue;
        }
    }
}
