// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   MainWindowViewModel.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FFXIVAPP.Updater {
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    internal sealed class MainWindowViewModel : INotifyPropertyChanged {
        private static Lazy<MainWindowViewModel> _instance = new Lazy<MainWindowViewModel>(() => new MainWindowViewModel());

        private double _extractProgress;
        private double _downloadProgress;
        private string _zipFileName;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public static MainWindowViewModel Instance {
            get {
                return _instance.Value;
            }
        }

        public string ZipFileName
        {
            get
            {
                return _zipFileName;
            }
            set
            {
                _zipFileName = value;
                RaisePropertyChanged();
            }
        }

        public double DownloadProgress
        {
            get
            {
                return _downloadProgress;
            }
            set
            {
                _downloadProgress = value;
                RaisePropertyChanged();
            }
        }

        public double ExtractProgress
        {
            get
            {
                return _extractProgress;
            }
            set
            {
                _extractProgress = value;
                RaisePropertyChanged();
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string caller = "") {
            this.PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }
    }
}