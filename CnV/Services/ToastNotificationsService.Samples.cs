
//using System;

//using Windows.UI.Notifications;

////using Microsoft.UI.Notifications;

//namespace CnV.Services
//{
//    internal partial class ToastNotificationsService
//    {

//        public void ShowNotification(string info, string tag)
//        {

//            // Create the toast content
//            var content = new ToastContent()
//            {
//                // More about the Launch property at https://docs.microsoft.com/dotnet/api/CommunityToolkit.WinUI.notifications.toastcontent
//                Launch = "IncomingActivationParams",

//                Visual = new ToastVisual()
//                {
//                    BindingGeneric = new ToastBindingGeneric()
//                    {
//                        Children =
//                        {
//                            new AdaptiveText()
//                            {
//                                Text = info
//                            },

//                        }
//                    },


//                },

//                Actions = new ToastActionsCustom()
//                {

//                    Buttons =
//                    {
//                        // More about Toast Buttons at https://docs.microsoft.com/dotnet/api/CommunityToolkit.WinUI.notifications.toastbutton
//                        new ToastButton("Okay", tag+"Okay")
//                        {
//                            ActivationType = ToastActivationType.Foreground,
                           
//                        },


//                        new ToastButtonDismiss(),
//                        new ToastButtonSnooze() {SelectionBoxId="snoozeTime",},


//                    },
//                    Inputs =
//                    {
//                        new ToastSelectionBox("snoozeTime")
//                    {
//                        DefaultSelectionBoxItemId = "5",
//                        Items =
//                        {
//                            new ToastSelectionBoxItem("1", "1 minute"),
//                            new ToastSelectionBoxItem("5", "5 minutes"),
//                            new ToastSelectionBoxItem("15", "15 minutes"),
//                            new ToastSelectionBoxItem("60", "1 hour"),
//                            new ToastSelectionBoxItem("240", "4 hours"),
//                        }
//                    },
//                    }
//                }
//            };


//            // Add the content to the toast
//            var toast = new ToastNotification(content.GetXml())
//            {
//                // TODO WTS: Set a unique identifier for this notification within the notification group. (optional)
//                // More details at https://docs.microsoft.com/uwp/api/windows.ui.notifications.toastnotification.tag
//                Tag = tag+"Tag"
//            };

//            // And show the toast
//            ShowToastNotification(toast);
//        }
//		 public void SpotChanged(string info)
//        {

//			// Create the toast content
//			var content = new ToastContent()
//			{
//				// More about the Launch property at https://docs.microsoft.com/dotnet/api/CommunityToolkit.WinUI.notifications.toastcontent
//				Launch = "SpotChangeActivationParams",
//				Audio = new ToastAudio()
//				{
//					Src = new Uri("ms-winsoundevent:Notification.IM")
//				},
//                Visual = new ToastVisual()
//                {
//                    BindingGeneric = new ToastBindingGeneric()
//                    {
//                        Children =
//                        {
//                            new AdaptiveText()
//                            {
//                                Text = info
//                            },

//                        }
//                    },


//                },

//                Actions = new ToastActionsCustom()
//                {

//                    Buttons =
//                    {
//                        // More about Toast Buttons at https://docs.microsoft.com/dotnet/api/CommunityToolkit.WinUI.notifications.toastbutton
//                        new ToastButton("OK", "incomingNotification")
//                        {
//                            ActivationType = ToastActivationType.Foreground,
                           
//                        },


//                        new ToastButtonDismiss(),
//                        new ToastButtonSnooze() {SelectionBoxId="snoozeTime",},


//                    }
					
                    
//                }
				
//            };


//            // Add the content to the toast
//            var toast = new ToastNotification(content.GetXml())
//            {
//                // TODO WTS: Set a unique identifier for this notification within the notification group. (optional)
//                // More details at https://docs.microsoft.com/uwp/api/windows.ui.notifications.toastnotification.tag
//                Tag = "SpotChangeTag"
//			};

//            // And show the toast
//            ShowToastNotification(toast);
//        }
//    }
//}
