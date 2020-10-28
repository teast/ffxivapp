// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBoxHelper.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2020 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   MessageBoxHelper.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FFXIVAPP.Client.Helpers
{
    using System;
    using Avalonia.Threading;
    using FFXIVAPP.Common.Helpers;
    using FFXIVAPP.Common.WPF;

    internal static class MessageBoxHelper {
        /// <summary>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public static void ShowMessage(string title, string message) {
            HandleMessage(title, message);
        }

        /// <summary>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="okAction"></param>
        /// <param name="cancelAction"></param>
        public static void ShowMessageAsync(string title, string message, Action okAction = null, Action cancelAction = null) {
            HandleMessage(title, message, okAction, cancelAction);
        }

        /// <summary>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="okAction"></param>
        /// <param name="cancelAction"></param>
        private static void HandleMessage(string title, string message, Action okAction = null, Action cancelAction = null) {
            DispatcherHelper.Invoke(
                async delegate {
                    try
                    {
                        if (cancelAction != null) {
                            var result = await MessageBox.Show(message, title, MessageBoxButton.OKCancel);
                            if (result == MessageBoxResult.OK) {
                                if (okAction != null) {
                                    DispatcherHelper.Invoke(okAction, DispatcherPriority.Send);
                                }
                            }

                            if (result == MessageBoxResult.Cancel) {
                                DispatcherHelper.Invoke(cancelAction, DispatcherPriority.Send);
                            }
                        }
                        else {
                            await MessageBox.Show(message, title,MessageBoxButton.OK);
                            if (okAction != null) {
                                DispatcherHelper.Invoke(okAction, DispatcherPriority.Send);
                            }
                        }
                    }
                    catch(Exception)
                    {
                        await MessageBox.Show(title, $"Unable to process MessageBox[{message}]:NotProcessingResult", MessageBoxButton.OK);
                    }

                }, DispatcherPriority.Send);
        }
    }
}