// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateView.xaml.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   UpdateView.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FFXIVAPP.Common.Helpers;

namespace FFXIVAPP.Client.Views
{
    /// <summary>
    ///     Interaction logic for UpdateView.xaml
    /// </summary>
    public class UpdateView : UserControl
    {
        public static UpdateView View;
        private Timer _spinner;

        public Grid LayoutRoot { get; }
        public TabControl UpdateTC { get; }
        public Grid AvailableLoadingInformation { get; }
        public TextBlock AvailableLoadingProgressMessage { get; }
        public DataGrid AvailableDG { get; }
        public TextBlock StatusInfo { get; }
        public Image PluginUpdateSpinner { get; }
        public TextBox TSource { get; }
        public DataGrid PluginSourceDG { get; }

        public UpdateView()
        {
            View = this;
            this.InitializeComponent();
            LayoutRoot = this.FindControl<Grid>("LayoutRoot");
            UpdateTC = this.FindControl<TabControl>("UpdateTC");
            AvailableLoadingInformation = this.FindControl<Grid>("AvailableLoadingInformation");
            AvailableLoadingProgressMessage = this.FindControl<TextBlock>("AvailableLoadingProgressMessage");
            AvailableDG = this.FindControl<DataGrid>("AvailableDG");
            PluginSourceDG = this.FindControl<DataGrid>("PluginSourceDG");
            StatusInfo = this.FindControl<TextBlock>("StatusInfo");
            PluginUpdateSpinner = this.FindControl<Image>("PluginUpdateSpinner");
            TSource = this.FindControl<TextBox>("TSource");
            
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
    }
}