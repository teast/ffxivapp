﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateViewModel.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   UpdateViewModel.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FFXIVAPP.Client.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Cache;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia.Controls;
    using Avalonia.Threading;
    using FFXIVAPP.Client.Models;
    using FFXIVAPP.Client.Properties;
    using FFXIVAPP.Client.Utilities;
    using FFXIVAPP.Client.Views;
    using FFXIVAPP.Common.Helpers;
    using FFXIVAPP.Common.Models;
    using FFXIVAPP.Common.Utilities;
    using FFXIVAPP.Common.ViewModelBase;

    using NLog;

    internal sealed class UpdateViewModel : INotifyPropertyChanged {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static Lazy<UpdateViewModel> _instance = new Lazy<UpdateViewModel>(() => new UpdateViewModel());

        private ObservableCollection<PluginDownloadItem> _availablePlugins;

        private int _availablePluginUpdates;

        public UpdateViewModel() {
            this.RefreshAvailableCommand = new DelegateCommand(RefreshAvailable);
            this.InstallCommand = new DelegateCommand(Install);
            this.UnInstallCommand = new DelegateCommand(UnInstall);
            this.AddOrUpdateSourceCommand = new DelegateCommand(AddOrUpdateSource);
            this.DeleteSourceCommand = new DelegateCommand(DeleteSource);
            this.AvailableDGDoubleClickCommand = new DelegateCommand(AvailableDGDoubleClick);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public static UpdateViewModel Instance {
            get {
                return _instance.Value;
            }
        }

        public ICommand AddOrUpdateSourceCommand { get; private set; }

        public ICommand AvailableDGDoubleClickCommand { get; private set; }

        public ObservableCollection<PluginDownloadItem> AvailablePlugins {
            get {
                return this._availablePlugins ?? (this._availablePlugins = new ObservableCollection<PluginDownloadItem>());
            }

            set {
                this._availablePlugins = value;
                this.RaisePropertyChanged();
            }
        }

        public int AvailablePluginUpdates {
            get {
                return this._availablePluginUpdates;
            }

            set {
                this._availablePluginUpdates = value;
                this.RaisePropertyChanged();
            }
        }

        public ICommand DeleteSourceCommand { get; private set; }

        public ICommand InstallCommand { get; private set; }

        public ICommand RefreshAvailableCommand { get; private set; }

        public ICommand UnInstallCommand { get; private set; }

        /// <summary>
        /// </summary>
        private static void AddOrUpdateSource() {
            Guid selectedId = Guid.Empty;
            try {
                if (UpdateView.View.PluginSourceDG.SelectedItems.Count == 1) {
                    selectedId = new Guid(GetValueBySelectedItem(UpdateView.View.PluginSourceDG, "Key"));
                }
            }
            catch (Exception ex) {
                Logging.Log(Logger, new LogItem(ex, true));
            }

            if (UpdateView.View.TSource.Text.Trim() == string.Empty) {
                return;
            }

            var pluginSourceItem = new PluginSourceItem {
                Enabled = true,
                SourceURI = UpdateView.View.TSource.Text,
            };
            if (selectedId == Guid.Empty) {
                pluginSourceItem.Key = Guid.NewGuid();
                Settings.Default.AvailableSources.Add(pluginSourceItem);
            }
            else {
                pluginSourceItem.Key = selectedId;
                var index = Settings.Default.AvailableSources.TakeWhile(source => source.Key != selectedId).Count();
                Settings.Default.AvailableSources[index] = pluginSourceItem;
            }

            UpdateView.View.PluginSourceDG.SelectedIndex = -1;
            UpdateView.View.TSource.Text = string.Empty;
        }

        private static void AvailableDGDoubleClick() {
            string key;
            try {
                key = GetValueBySelectedItem(UpdateView.View.AvailableDG, "Name");
            }
            catch (Exception ex) {
                Logging.Log(Logger, new LogItem(ex, true));
                return;
            }

            PluginDownloadItem plugin = Instance.AvailablePlugins.FirstOrDefault(p => string.Equals(p.Name, key, Constants.InvariantComparer));
            if (plugin == null) {
                return;
            }

            switch (plugin.Status) {
                case PluginStatus.Installed:
                    UnInstallByKey(plugin.Name);
                    break;
                case PluginStatus.NotInstalled:
                case PluginStatus.UpdateAvailable:
                    InstallByKey(plugin.Name);
                    break;
            }
        }

        /// <summary>
        /// </summary>
        private static void DeleteSource() {
            string key;
            try {
                key = GetValueBySelectedItem(UpdateView.View.PluginSourceDG, "Key");
            }
            catch (Exception ex) {
                Logging.Log(Logger, new LogItem(ex, true));
                return;
            }

            var index = Settings.Default.AvailableSources.TakeWhile(source => source.Key.ToString() != key).Count();
            Settings.Default.AvailableSources.RemoveAt(index);
        }

        /// <summary>
        /// </summary>
        /// <param name="listView"> </param>
        /// <param name="key"> </param>
        private static string GetValueBySelectedItem(DataGrid listView, string key) {
            Type type = listView.SelectedItem.GetType();
            PropertyInfo property = type.GetProperty(key);
            return property.GetValue(listView.SelectedItem, null).ToString();
        }

        /// <summary>
        /// </summary>
        private static void Install() {
            foreach (object selectedItem in UpdateView.View.AvailableDG.SelectedItems) {
                InstallByKey(((PluginDownloadItem) selectedItem).Name);
            }
        }

        private static void InstallByKey(string key, Action asyncAction = null) {
            PluginDownloadItem plugin = Instance.AvailablePlugins.FirstOrDefault(p => string.Equals(p.Name, key, Constants.InvariantComparer));
            if (plugin == null) {
                return;
            }

            UpdateView.View.AvailableLoadingInformation.IsVisible = true;
            UpdateView.View.AvailableLoadingProgressMessage.IsVisible = true;
            Task.Run(delegate {
                var updateCount = 0;
                var updateLimit = plugin.Files.Count;
                var sb = new StringBuilder();
                foreach (PluginFile pluginFile in plugin.Files) {
                    sb.Clear();
                    try {
                        using (var client = new WebClient {
                            CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore),
                        }) {
                            var saveLocation = Path.Combine(App.RootPath, "Plugins", plugin.Name, pluginFile.Location, pluginFile.Name);
                            Directory.CreateDirectory(Path.GetDirectoryName(saveLocation));

                            if (UpdateUtilities.VerifyFile(saveLocation, pluginFile.Checksum)) {
                                // no need to download file, since it hasn't changed
                                // count this file as "updated" for the purpose of checking if the install finished
                                updateCount++;
                            }
                            else {
                                sb.Append(plugin.SourceURI.Trim('/'));
                                var location = pluginFile.Location.Trim('/');
                                if (!string.IsNullOrWhiteSpace(location)) {
                                    sb.AppendFormat("/{0}", location);
                                }

                                sb.AppendFormat("/{0}", pluginFile.Name.Trim('/'));
                                var uri = new Uri(sb.ToString());
                                client.DownloadFileAsync(uri, saveLocation);
                                client.DownloadProgressChanged += delegate {
                                    DispatcherHelper.Invoke(
                                        delegate {
                                            UpdateView.View.AvailableLoadingProgressMessage.Text = $"{pluginFile.Location.Trim('/')}/{pluginFile.Name}";
                                        });
                                };
                                client.DownloadFileCompleted += delegate {
                                    updateCount++;

                                    if (updateCount >= updateLimit) {
                                        DispatcherHelper.Invoke(
                                            delegate {
                                                if (plugin.Status != PluginStatus.Installed) {
                                                    plugin.Status = PluginStatus.Installed;
                                                    if (asyncAction != null) {
                                                        DispatcherHelper.Invoke(asyncAction);
                                                    }
                                                }

                                                UpdateView.View.AvailableLoadingProgressMessage.Text = string.Empty;
                                                UpdateView.View.AvailableLoadingInformation.IsVisible = false;
                                                UpdateView.View.AvailableLoadingProgressMessage.IsVisible = false;
                                            }, DispatcherPriority.Send);
                                    }
                                };
                            }
                        }
                    }
                    catch (Exception) {
                        updateCount++;
                    }
                }

                // need to check here aswell, since if all files mathced hash, none will be downloaded, so the DownloadFileCompleted delegate would never be triggered
                if (updateCount >= updateLimit) {
                    if (plugin.Status != PluginStatus.Installed) {
                        plugin.Status = PluginStatus.Installed;
                        if (asyncAction != null) {
                            DispatcherHelper.Invoke(asyncAction);
                        }
                    }

                    DispatcherHelper.Invoke(
                        delegate {
                            UpdateView.View.AvailableLoadingInformation.IsVisible = false;
                            UpdateView.View.AvailableLoadingProgressMessage.IsVisible = false;
                        }, DispatcherPriority.Send);
                }

                return true;
            });
        }

        private static void RefreshAvailable() {
            Instance.AvailablePlugins.Clear();
            Initializer.LoadAvailablePlugins();
        }

        /// <summary>
        /// </summary>
        private static void UnInstall() {
            foreach (object selectedItem in UpdateView.View.AvailableDG.SelectedItems) {
                UnInstallByKey(((PluginDownloadItem) selectedItem).Name);
            }
        }

        private static void UnInstallByKey(string key, Action asyncAction = null) {
            PluginDownloadItem plugin = Instance.AvailablePlugins.FirstOrDefault(p => string.Equals(p.Name, key, Constants.InvariantComparer));
            if (plugin == null) {
                return;
            }

            DispatcherHelper.Invoke(
                delegate {
                    foreach (PluginDownloadItem pluginDownloadItem in Instance.AvailablePlugins.Where(pluginDownloadItem => string.Equals(pluginDownloadItem.Name, plugin.Name))) {
                        pluginDownloadItem.Status = PluginStatus.NotInstalled;
                    }

                    PluginHost.Instance.UnloadPlugin(plugin.Name);
                    for (var i = ShellView.View.PluginsTCItems.Count - 1; i > 0; i--) {
                        if ((ShellView.View.PluginsTCItems[i]).Name == Regex.Replace(plugin.Name, @"[^A-Za-z]", string.Empty)) {
                            AppViewModel.Instance.PluginTabItems.RemoveAt(i);
                        }
                    }

                    try {
                        var saveLocation = Path.Combine(App.RootPath, "Plugins", plugin.Name);
                        Directory.Delete(saveLocation, true);
                    }
                    catch (Exception ex) {
                        Logging.Log(Logger, new LogItem(ex, true));
                    }
            }, DispatcherPriority.Send);
        }

        private void RaisePropertyChanged([CallerMemberName,] string caller = "") {
            this.PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }
    }
}