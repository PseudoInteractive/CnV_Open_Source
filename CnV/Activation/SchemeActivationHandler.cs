﻿//using System;
//using System.Threading.Tasks;

//using COTG.Services;
//using COTG.Views;

//using Windows.ApplicationModel.Activation;

//using static COTG.Debug;

//namespace COTG.Activation
//{
//    internal class SchemeActivationHandler : ActivationHandler<ProtocolActivatedEventArgs>
//    {
//        // By default, this handler expects URIs of the format 'wtsapp:sample?paramName1=paramValue1&paramName2=paramValue2'
//        protected override async Task HandleInternalAsync(ProtocolActivatedEventArgs args)
//        {
//            // Create data from activation Uri in ProtocolActivatedEventArgs
//            var data = new SchemeActivationData(args.Uri);
//            if (data.IsValid)
//            {
//                // TODO
//                Assert(false);
// //               NavigationService.Navigate(data.PageType, data.Parameters);
//            }

//            await Task.CompletedTask;
//        }

//        protected override bool CanHandleInternal(ProtocolActivatedEventArgs args)
//        {
//            // If your app has multiple handlers of ProtocolActivationEventArgs
//            // use this method to determine which to use. (possibly checking args.Uri.Scheme)
//            return true;
//        }
//    }
//}
