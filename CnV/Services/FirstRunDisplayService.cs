﻿//using System;
//using System.Threading.Tasks;

//using COTG.Views;

//using CommunityToolkit.WinUI.Helpers;

//using Windows.ApplicationModel.Core;
//using Windows.UI.Core;

//namespace COTG.Services
//{
//    public static class FirstRunDisplayService
//    {
//        private static bool shown = false;

//        internal static async Task ShowIfAppropriateAsync()
//        {
//            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
//                DispatcherQueuePriority.Normal, async () =>
//                {
//                    if (SystemInformation.IsFirstRun && !shown)
//                    {
//                        //shown = true;
//                        //var dialog = new FirstRunDialog();
//                        //await dialog.ShowAsync();
//                    }
//                });
//        }
//    }
//}