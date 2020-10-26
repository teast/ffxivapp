// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TabItemHelper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   TabItemHelper.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FFXIVAPP.Client.Helpers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Layout;
    using Avalonia.Media.Imaging;
    using FFXIVAPP.Client.Models;
    using FFXIVAPP.Client.ViewModels;
    using FFXIVAPP.Common.Helpers;
    using FFXIVAPP.Common.Models;
    using FFXIVAPP.Common.Utilities;
    using FFXIVAPP.ResourceFiles;

    using NLog;

    internal static class TabItemHelper {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        /// <param name="image"> </param>
        /// <param name="name"> </param>
        /// <returns> </returns>
        public static StackPanel ImageHeader(Bitmap image, string name) {
            var stackPanelFactory = new StackPanel { Orientation = Orientation.Horizontal };
            var imageFactory = new Image { 
                Width = 24,
                Height = 24,
                [ToolTip.TipProperty] = name, 
                Source = image 
            };
            var labelFactory = new TextBlock {
                Name = "TheLabel",
                Text = name,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = Thickness.Parse("5,0,0,0") 
            };

            /* TODO: Implement this
            Binding binding = BindingHelper.VisibilityBinding(Settings.Default, "EnableHelpLabels");
            labelFactory.SetBinding(UIElement.VisibilityProperty, binding);
            labelFactory.SetValue(ContentControl.ContentProperty, name);
            */

            stackPanelFactory.Children.Add(imageFactory);
            stackPanelFactory.Children.Add(labelFactory);
            return stackPanelFactory;
        }

        public static void LoadPluginTabItem(PluginInstance pluginInstance) {
            try {
                if (!pluginInstance.Loaded) {
                    return;
                }

                var pluginName = pluginInstance.Instance.FriendlyName;
                if (SettingsViewModel.Instance.HomePluginList.Any(p => p.ToUpperInvariant().StartsWith(pluginName.ToUpperInvariant()))) {
                    pluginName = $"{pluginName}[{new Random().Next(1000, 9999)}]";
                }

                SettingsViewModel.Instance.HomePluginList.Add(pluginName);
                TabItem tabItem = null;
                var creater = new AutoResetEvent(false);
                DispatcherHelper.Invoke(() => {
                    try
                    {
                        tabItem = pluginInstance.Instance.CreateTab();
                        tabItem.Name = Regex.Replace(pluginInstance.Instance.Name, @"[^A-Za-z]", string.Empty);
                        var iconfile = Path.Combine(Path.GetDirectoryName(pluginInstance.AssemblyPath), pluginInstance.Instance.Icon);
                        Bitmap icon = Theme.DefaultPluginLogo;
                        icon = File.Exists(iconfile)
                                ? ImageUtilities.LoadImageFromStream(iconfile)
                                : icon;
                        tabItem.Header = ImageHeader(icon, pluginInstance.Instance.FriendlyName);
                    }
                    catch (Exception ex) {
                        Logging.Log(Logger, new LogItem(ex, true));
                    }
                    finally
                    {
                        creater.Set();
                    }
                });

                creater.WaitOne();
                if (tabItem != null)
                    AppViewModel.Instance.PluginTabItems.Add(tabItem);
            }
            catch (Exception ex) {
                Logging.Log(Logger, new LogItem(ex, true));
            }
        }
    }
}