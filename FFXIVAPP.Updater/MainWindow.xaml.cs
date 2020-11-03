// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   MainWindow.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FFXIVAPP.Updater
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Interactivity;
    using Avalonia.Layout;
    using Avalonia.Markup.Xaml;
    using Avalonia.VisualTree;
    using FFXIVAPP.Common.Helpers;
    using NLog;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly WebClient _webClient = new WebClient();

        private readonly ProgressBar ProgressBarSingle;
        private readonly ProgressBar ProgressBarFiles;
        private bool _shown = false;
        public MainWindow() {
            this.InitializeComponent();

            ProgressBarSingle = this.FindControl<ProgressBar>("ProgressBarSingle");
            ProgressBarFiles = this.FindControl<ProgressBar>("ProgressBarFiles");
            this.FindControl<DockPanel>("CustChrome").PointerPressed += (s, e) =>
            {
                BeginMoveDrag(e);
            };
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double width = this.MinWidth, height = this.MinHeight;
            for (var i = 0; i < this.VisualChildren.Count; i++)
            {
                IVisual visual = this.VisualChildren[i];
                if (visual is ILayoutable layoutable)
                {
                    layoutable.Measure(Size.Infinity);
                    width = Math.Max(layoutable.DesiredSize.Width, width);
                    height = Math.Max(layoutable.DesiredSize.Height, height);
                }
            }

            return new Size(width, height);
        }
        
        public override void Show()
        {
            base.Show();

            WindowLoaded();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// </summary>
        private void CleanupTemporary(string path) {
            try {
                FileInfo[] fileInfos = new DirectoryInfo(path).GetFiles();
                foreach (FileInfo fileInfo in fileInfos.Where(t => t.Extension == ".tmp" || t.Extension == ".PendingOverwrite")) {
                    fileInfo.Delete();
                }
            }
            catch (Exception) {
                // IGNORED
            }
        }

        private void CloseUpdater_OnClick(object sender, RoutedEventArgs e) {
            this.Close();
        }

        /// <summary>
        /// </summary>
        private void DownloadUpdate() {
            try
            {
                this._webClient.DownloadFileCompleted += this.WebClientOnDownloadFileCompleted;
                this._webClient.DownloadProgressChanged += this.WebClientOnDownloadProgressChanged;
App.PrintDebug($"Starting download of package...");                
                this._webClient.DownloadFileAsync(new Uri(App.DownloadUri), MainWindowViewModel.Instance.ZipFileName);
            }
            catch (Exception)
            {
                Environment.Exit(0);
            }
        }

        private void ExtractAndClean() {
            var path = Directory.GetCurrentDirectory();
App.PrintDebug($"Cleaning temp directory...");                
            this.CleanupTemporary(path);
App.PrintDebug($"Opening zip file...");                
            var zip = ZipFile.OpenRead(MainWindowViewModel.Instance.ZipFileName);
            var progress = 0;
            var count = Math.Max(zip.Entries.Count, 1);
            MainWindowViewModel.Instance.ExtractProgress = (double)progress/count;
App.PrintDebug($"  {count} entries to extract... to {path}");
            foreach (var entry in zip.Entries) {
                try {
                    if (entry.Length == 0 || string.IsNullOrEmpty(entry.Name)) {
                        App.PrintDebug($"{progress}: skipping length: {entry.Length}");
                        continue;
                    }
                    if (entry.Name.Contains("FFXIVAPP.Client.exe.nlog") && File.Exists("FFXIVAPP.Client.exe.nlog")) {
                        continue;
                    }

App.PrintDebug($"{progress}: {entry.Name}...");
                    entry.ExtractToFile(Path.Combine(path, entry.Name), true);
                    progress++;
                    MainWindowViewModel.Instance.ExtractProgress = (double)progress/count;
                }
                catch (Exception ex) {
                    // IGNORED
                    App.PrintDebug($"ERROR extracting {entry.Name}: {ex.Message}");
                }
            }

App.PrintDebug($"Extraction done...");
            this._webClient.Dispose();
            try {
                var file = "FFXIVAPP.Client.exe";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    file = "FFXIVAPP.Client";
App.PrintDebug($"changing FFXIVAPP.Client to executable...");
                    var proc = Process.Start("chmod", "+x FFXIVAPP.Client");
                    var result = proc.WaitForExit(5000);
App.PrintDebug($"changing to executable result: {result} ({proc.ExitCode})");
                }

App.PrintDebug($"Starting {file}...");
                var m = new Process {
                    StartInfo = {
                        FileName = file
                    }
                };
                var result2 = m.Start();
App.PrintDebug($"  result: {result2}...");
            }
            catch (Exception) {
                // IGNORED
            }
            finally {
                this._webClient.DownloadFileCompleted -= this.WebClientOnDownloadFileCompleted;
                this._webClient.DownloadProgressChanged -= this.WebClientOnDownloadProgressChanged;
                this.CleanupTemporary(path);
                DispatcherHelper.Invoke(() => this.Close());
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="asyncCompletedEventArgs"></param>
        private void WebClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs) {
App.PrintDebug($"Going to extract in another task...");                
            Task.Run(delegate {
                this.ExtractAndClean();
            });
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="downloadProgressChangedEventArgs"></param>
        private void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs) {
            var totalBytes = Math.Max(downloadProgressChangedEventArgs.TotalBytesToReceive, 1);
            var bytesIn = Math.Min(downloadProgressChangedEventArgs.BytesReceived, totalBytes);
            MainWindowViewModel.Instance.DownloadProgress = (double)bytesIn / totalBytes;
        }

        private void WindowLoaded() {
            if (_shown)
                return;
            _shown = true;
            if (App.DownloadUri == null || App.Version == null) {
                this.Close();
            }
            else {
                MainWindowViewModel.Instance.ZipFileName = $"FFXIVAPP_.zip";
                Process[] app = Process.GetProcessesByName("FFXIVAPP.Client");
                foreach (Process p in app) {
                    try {
                        p.Kill();
                    }
                    catch (Exception) {
                        // IGNORED
                    }
                }

                Task.Run(delegate {
                    this.DownloadUpdate();
                });
            }
        }
    }
}