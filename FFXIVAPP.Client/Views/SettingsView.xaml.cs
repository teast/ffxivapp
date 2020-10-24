// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsView.xaml.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   SettingsView.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FFXIVAPP.Client.Views {
    /// <summary>
    ///     Interaction logic for DefaultView.xaml
    /// </summary>
    public class SettingsView: UserControl {
        public static SettingsView View;

        public Grid LayoutRoot { get; }
        public ComboBox UIScaleSelect { get; }
        public ComboBox GameLanguageSelect { get; }
        public ComboBox HomePluginSelect { get; }
        public ComboBox PIDSelect { get; }
        public Button SetProcess { get; }
        public Button RefreshList { get; }
        public Button RefreshMemoryWorkers { get; }
        public ComboBox ThemeSelect { get; }
        public ComboBox NetworkInterfaceSelect { get; }
        public ComboBox AudioDeviceSelect { get; }
        public TextBox CICUID { get; }
        public TextBox CharacterName { get; }
        public ComboBox ServerList { get; }
        public TextBox TCode { get; }
        public TextBox TColor { get; }
        public DataGrid Colors { get; }

        public SettingsView() {
            View = this;
            this.InitializeComponent();
            LayoutRoot = this.FindControl<Grid>("LayoutRoot");
            UIScaleSelect = this.FindControl<ComboBox>("UIScaleSelect");
            GameLanguageSelect = this.FindControl<ComboBox>("GameLanguageSelect");
            HomePluginSelect = this.FindControl<ComboBox>("HomePluginSelect");
            PIDSelect = this.FindControl<ComboBox>("PIDSelect");
            SetProcess = this.FindControl<Button>("SetProcess");
            RefreshList = this.FindControl<Button>("RefreshList");
            RefreshMemoryWorkers = this.FindControl<Button>("RefreshMemoryWorkers");
            ThemeSelect = this.FindControl<ComboBox>("ThemeSelect");
            NetworkInterfaceSelect = this.FindControl<ComboBox>("NetworkInterfaceSelect");
            AudioDeviceSelect = this.FindControl<ComboBox>("AudioDeviceSelect");
            CICUID = this.FindControl<TextBox>("CICUID");
            CharacterName = this.FindControl<TextBox>("CharacterName");
            ServerList = this.FindControl<ComboBox>("ServerList");
            TCode = this.FindControl<TextBox>("TCode");
            TColor = this.FindControl<TextBox>("TColor");
            Colors = this.FindControl<DataGrid>("Colors");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}