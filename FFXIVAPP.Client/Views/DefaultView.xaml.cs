// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultView.xaml.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   DefaultView.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FFXIVAPP.Client.Views {
    /// <summary>
    ///     Interaction logic for DefaultView.xaml
    /// </summary>
    public class DefaultView: UserControl {
        public static DefaultView View;

        public DefaultView() {
            View = this;
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}