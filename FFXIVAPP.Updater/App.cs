// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   App.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FFXIVAPP.Updater
{
    using System;
    using Avalonia;
    using FFXIVAPP.Common;
    using FFXIVAPP.Common.Models;
    using FFXIVAPP.Common.Utilities;
    using FFXIVAPP.Common.WPF;
    using NLog;

    public partial class App {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static string DownloadUri;
        public static string Version;

        /// <summary>
        ///     Application Entry Point.
        /// </summary>
        [STAThread]
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        public static int Main(string[] args) {
            try
            {
                if (args?.Length != 2)
                    return -1;
                DownloadUri = args[0];
                Version = args[1];
                Constants.EnableNLog = true;
                AppBuilder.Configure<App>()
                                .UsePlatformDetect()
                                .LogToDebug()
                                .StartWithClassicDesktopLifetime(args);
                return 0;
            }
            catch (Exception ex)
            {
                Logging.Log(Logger, new LogItem(ex, true));
                MessageBox.Show(ex.Message);
            }

            return -2;
        }

        public static void PrintDebug(string line)
        {
            //Console.WriteLine(line);
        }
    }
}