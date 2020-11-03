// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   App.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace FFXIVAPP.Updater {
    public partial class App : Application { 
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow()
                {
                    DataContext = MainWindowViewModel.Instance
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}