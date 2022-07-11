using CommunityToolkit.WinUI.Helpers;
#if APPCENTER

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;

#endif
#if APPCENTER
using Microsoft.AppCenter.Crashes;
#endif

using System.Runtime.InteropServices.WindowsRuntime;
//using Windows.Storage;
using static CnV.AppS;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
//using Microsoft.UI.Xaml.Controls;
//using Microsoft.Web.WebView2.Core;
//using ZLogger;
//using Cysharp.Text;
using Microsoft.UI.Input;
//using System.Windows.Input;
//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Microsoft.UI.Input.Experimental;
//using Microsoft.ApplicationModel;
//using Microsoft.ApplicationModel.Activation;
//using Microsoft.ApplicationModel.DataTransfer;
using Windows.Globalization.NumberFormatting;
using Windows.System;
//using Windows.UI.Core;
//using Windows.UI.Input;
using MenuFlyout = Microsoft.UI.Xaml.Controls.MenuFlyout;
using MenuFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;
using MenuFlyoutSubItem = Microsoft.UI.Xaml.Controls.MenuFlyoutSubItem;
using ToggleMenuFlyoutItem = Microsoft.UI.Xaml.Controls.ToggleMenuFlyoutItem;

using Microsoft.UI.Xaml.Input;
//using Windows.UI.Core;
using Microsoft.UI;
using DecimalFormatter = Windows.Globalization.NumberFormatting.DecimalFormatter;

namespace CnV
{


	using Microsoft.UI.Windowing;
	using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;
	using Microsoft.Windows.AppLifecycle;
	//// using PInvoke
	using Services;

	using System.Reflection;
	using System.Windows.Input;

	using Views;

	/// <summary>
	/// App
	/// </summary>
	public sealed partial class App:Application
	{

		//public static ref State => ref AppS.state;
		//		static IConfigurationRoot configuration;


//		private Lazy<ActivationService> _activationService;
////		public static bool processingTasksStarted;

//		private ActivationService ActivationService
//		{
//			get { return _activationService.Value; }
//		}
		internal static void FilterNans(NumberBox sender,NumberBoxValueChangedEventArgs args)
		{
			if(Double.IsNaN(sender.Value) || Double.IsNaN(args.NewValue))
			{
				Log($"{args.NewValue} <= {args.OldValue} v: {sender.Value}");

				sender.Value =0;
			}
			
		}
		internal static void FilterPositive(NumberBox sender,NumberBoxValueChangedEventArgs args)
		{
			if(Double.IsNaN(sender.Value) || Double.IsNaN(args.NewValue))
			{
				Log($"{args.NewValue} <= {args.OldValue} v: {sender.Value}");

				sender.Value =0;
			}
			else if(sender.Value < 0) {
				Note.Show($"{sender.Header} Cannot be negative");
				sender.Value = 0;
			}
		}



		// can only be called from UI thread
		//private static CoreVirtualKeyStates GetKeyState(VirtualKey key)
		//{
		//	var window = CoreWindow.GetForCurrentThread();
		//	if (window == null)
		//	{
		//		Assert(false);
		//		return CoreVirtualKeyStates.None;
		//	}

		//	return window.GetAsyncKeyState(key);
		//}
		//public static bool IsKeyDown(VirtualKey key)
		//{
		//	var state = GetKeyState(key);
		//	return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
		//}



		//static void OnKeyUp(CoreWindow sender, KeyEventArgs args)
		//{
		//	var key = args.VirtualKey;
		//	OnKeyUp(key);
		//}




		//static bool webViewInFront = false;

		//static void OnKeyDown(CoreWindow sender, KeyEventArgs args)
		//{
		//	Note.Show("Key!");
		//	var key = args.VirtualKey;

		//	OnKeyDown(key);

		//}





		static DateTimeOffset activeStart = DateTimeOffset.UtcNow;
		//private static Microsoft.Extensions.Configuration.IConfigurationRoot BuildConfig()
		//{
		//	var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

		//	var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) ||
		//						devEnvironmentVariable.ToLower() == "development";

		//	var builder = new ConfigurationBuilder();
		//	// tell the builder to look for the appsettings.json file
		//	builder
		//		.AddJsonFile("appsettings.json",optional: false,reloadOnChange: false);

		//	//only add secrets in development
		//	if(isDevelopment)
		//	{
		//		builder.AddUserSecrets<Program>();
		//	}

		//	return builder.Build();
		//}
		//public static FontFamily CnVFont;


		//private bool Window_Closing() //object sender,WindowClosingEventArgs e)
		//{
		//	Log("Closing!");
		//	if(state == State.closed)
		//		return true;
		//	if( state == State.closing)
		//		return false;
		//	state = State.closing;

		//	SwitchToBackground().ContinueWith( (_)=> 
		//		{
		//		state = State.closed;
		//		AppS.DispatchOnUIThread(window.Close);
		//	});
		//	return false;

		//}

		//private static void Window_Closed(object sender,WindowEventArgs args)
		//{

		//	Log($"WindowClosed!  {isForeground} {args.Handled}");
		//	//			//	Assert(state == State.closed);
		//	//			SwitchToBackground();
		//}

		private void Content_PreviewKeyUp(object sender,Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			AppS.UpdateKeyStates();
			var key = e.OriginalKey;
			//Trace("KeyUp" + key);
			switch(key)
			{
				case VirtualKey.Shift:
				case VirtualKey.LeftShift:
				case VirtualKey.RightShift:
					//		Trace("Shift Up");
					shiftPressed = false;
					break;
				case VirtualKey.Control:
				case VirtualKey.LeftControl:
				case VirtualKey.RightControl:

					controlPressed = false;
					break;

			}

			InputRecieved();

		}

		private void Content_PreviewKeyDown(object sender,Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			AppS.UpdateKeyStates();
			var key = e.OriginalKey;
			switch(key)
			{
				case VirtualKey.Shift:
				case VirtualKey.LeftShift:
				case VirtualKey.RightShift:
					//Trace("Shift Down");

					shiftPressed = true;
					break;
				case VirtualKey.Control:
				case VirtualKey.LeftControl:
				case VirtualKey.RightControl:

					controlPressed = true;
					break;

			}

			InputRecieved();
			ShellPage.DoKeyDown(key);
		}

		private static void DebugSettings_BindingFailed(object sender,BindingFailedEventArgs e)
		{
			Log(e.Message);
		}



		//private void CoreWindow_Closed(CoreWindow sender,CoreWindowEventArgs args)
		//{
		//	Log("Close");
		//	state = State.closing;
		//	CnVServer.CloseWebView();
		//	TabPage.CloseAllTabWindows();
		//}

		//private async void MainView_HostedViewClosing(CoreApplicationView sender,HostedViewClosingEventArgs args)
		//{
		//	var defer = args.GetDeferral();
		//	Log("Close");
		//	state = State.closing;
		//	CnVServer.CloseWebView();
		//	await TabPage.CloseAllTabWindows();
		//}


		//protected override async void OnActivated(IActivatedEventArgs args)
		//{
		//	var activation = args as IActivatedEventArgs;
		//	globalQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
		//	if (activation != null && activation.PreviousExecutionState == ApplicationExecutionState.Running)
		//	{
		//		Window.Current.Activate();
		//		//	isForeground = true;

		//		// Todo:  Handle arguments and stuff
		//		// Ensure the current window is active
		//		if (args is ToastNotificationActivatedEventArgs toastActivationArgs)
		//		{
		//			// Obtain the arguments from the notification
		//			var toastArgs = System.Web.HttpUtility.ParseQueryString(toastActivationArgs.Argument);
		//			// Obtain any user input (text boxes, menu selections) from the notification
		//			ValueSet userInput = toastActivationArgs.UserInput;
		//			foreach (var op in toastArgs.AllKeys)
		//			{
		//				if (op == "incomingNotification")
		//				{
		//					Task.Delay(3000).ContinueWith(async (_) =>
		//					{
		//						while (IncomingTab.instance == null)
		//							await Task.Delay(500);
		//						AppS.DispatchOnUIThreadLow(IncomingTab.instance.Show);

		//					});
		//				}
		//			}
		//			// TODO: Show the corresponding content
		//		}


		//		return;
		//	}

		//	await OnLaunchedOrActivated(args);



		//	//var configuration = new ConfigurationBuilder()
		//	//                                .AddJsonFile("appsettings.json", false, true)
		//	//                                .Build();




		//	//    CreateDefaultBuilder(args)
		//	//        .ConfigureWebHostDefaults(webBuilder =>
		//	//        {
		//	//            webBuilder.UseStartup<Startup>();
		//	//        }).Build().Run();


		//	//ILogger logger;

		//	//using (var serviceProvider = new ServiceCollection()
		//	//    .AddLogging(cfg =>
		//	//    {
		//	//        cfg.AddConfiguration(configuration.GetSection("Logging"));
		//	//        cfg.AddConsole();
		//	//    })
		//	//    .BuildServiceProvider())
		//	//{
		//	//    logger = serviceProvider.GetService<ILogger<App>>();
		//	//}

		//	//logger.LogInformation("logger information");
		//	//logger.LogWarning("logger warning");


		//	//using (var listener = new LoggerTraceListener(logger))
		//	//{
		//	//    System.Diagnostics.Trace.Listeners.Add(listener);
		//	//    TraceSources.Instance.InitLoggerTraceListener(listener);

		//	//    TraceLover.DoSomething();
		//	//    TraceSourceLover.DoSomething();
		//	//}


		//}

		//public static void SetupCoreWindowInputHooks()
		//{

		//	Assert(CoreApplication.Views.Count==1);

		//	var window = CoreApplication.MainView.CoreWindow;
		//	{
		//		//Log($"{view.TitleBar.ToString()} {view.IsMain} ");

		//	//	window.PointerMoved += OnPointerMoved;
		//		window.PointerPressed += OnPointerPressed; ;
		//	//	window.PointerExited+=Window_PointerExited; ;
		//	//	window.PointerEntered+=Window_PointerEntered; ;
		//		window.KeyDown += OnKeyDown;
		//		window.KeyUp += OnKeyUp;

		//	}
		//}

		//private static void Window_PointerEntered(CoreWindow sender,PointerEventArgs args)
		//{
		//	ShellPage.UpdateMousePosition(args,ShellPage.instance);
		//	ShellPage.UpdateFocus();
		//}

		//private static void Window_PointerExited(CoreWindow sender,PointerEventArgs args)
		//{
		//	ShellPage.UpdateMousePosition(args, ShellPage.instance);
		//	ShellPage.UpdateFocus();
		//}

		//private static void Window_PointerEntered(CoreWindow sender,PointerEventArgs args)
		//{
		//	args.KeyModifiers.UpdateKeyModifiers();
		//	Log("Pointer enter");
		//	ShellPage.UpdateFocus();

		//}

		//private static void Window_PointerExited(CoreWindow sender,PointerEventArgs args)
		//{
		//	args.KeyModifiers.UpdateKeyModifiers();
		//	Log("Pointer exit");
		//	ShellPage.UpdateFocus();
		//}

		//public static void OnPointerPressed(CoreWindow sender, PointerEventArgs e)
		//{
		////	ShellPage.UpdateMousePosition(e);

		//	var prop = e.CurrentPoint.Properties.PointerUpdateKind;
		//	if (OnPointerPressed(prop))
		//		e.Handled = true;
		////	ShellPage.UpdateFocus();
		//}

		// Uses Task Await
		//internal static async void ProcessThrottledTasks()
		//{
		//	for(;;)
		//	{


		//		try
		//		{
		//			if(!throttledTasks.IsEmpty)
		//			{
		//				if(throttledTasks.TryDequeue(out var t))
		//					await t().ConfigureAwait(false);

		//			}
		//		}
		//		catch(Exception _exception)
		//		{
		//			Debug.LogEx(_exception);
		//		}


		//		await Task.Delay(500).ConfigureAwait(false);
		//	}
		//}

		//public static void EnqeueTask(Func<Task> a)
		//{
		//	throttledTasks.Enqueue(a);
		//}

		//public static bool OnPointerPressed(PointerUpdateKind prop)
		//{
		//	var rv = false;
		//	switch(prop)
		//	{
		//		case PointerUpdateKind.XButton1Pressed:
		//			NavStack.Back(true);

		//			Log("XButton1");
		//			rv = true;
		//			break;
		//		case PointerUpdateKind.XButton2Pressed:
		//			NavStack.Forward(true);
		//			Log("XButton2");
		//			rv = true;
		//			break;
		//	}

		//	InputRecieved();
		//	return rv;
		//}

		public static void InputRecieved() => AppS.InputRecieved();

		//private static void OnPointerMoved(CoreWindow sender, PointerEventArgs args)
		//{
		//	//	ShellPage.UpdateMousePosition(args);
		//	//			args.KeyModifiers.UpdateKeyModifiers();

		//	// reset timer if active
		//	//	InputRecieved();
		//	ShellPage.UpdateMousePosition(args,ShellPage.instance);
		//	ShellPage.UpdateFocus();
		//}


		//private ActivationService CreateActivationService()
		//{
		//	return new ActivationService(); //this, null, new Lazy<UIElement>(()=> new Views.ShellPage()));
		//}

		//public static Task EnqueueAsync(DispatcherQueue dispatcher,Func<Task> function,DispatcherQueuePriority priority = 0)
		//{
		//	//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//	if(dispatcher.HasThreadAccess)
		//	{
		//		try
		//		{
		//			Task task = function();
		//			if(task != null)
		//			{
		//				return task;
		//			}

		//			return Task.FromException(new InvalidOperationException("The Task returned by function cannot be null."));
		//		}
		//		catch(Exception exception)
		//		{
		//			return Task.FromException(exception);
		//		}
		//	}

		//	return TryEnqueueAsync(dispatcher,function,priority);
		//	static Task TryEnqueueAsync(DispatcherQueue dispatcher,Func<Task> function,DispatcherQueuePriority priority)
		//	{
		//		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//		//IL_002b: Expected O, but got Unknown
		//		Func<Task> function2 = function;
		//		TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
		//		if(!dispatcher.TryEnqueue(priority,async ()=>
		//		{
		//			await function2();
		//			taskCompletionSource.SetResult(null);
		//		}))
		//		{
		//			taskCompletionSource.SetException(new InvalidOperationException("Failed to enqueue the operation"));
		//		}

		//		return taskCompletionSource.Task;
		//	}
		//}

		//public static async Task WaitWhileUiSemaBusy()
		//{
		//	Log($"Lock sema: {uiSema.CurrentCount}");
		//	for (; ; )
		//	{
		//		if (!App.isUISemaLocked)
		//			break;

		//		await uiSema.WaitAsync();
		//		uiSema.Release();
		//		await Task.Delay(500); // if there is another thread waiting on the sema, let them go first
		//	}

		//}



		//public static int pendingDispatch;
		//public static int pendingDispatchMax=10;
		//public static void DispatchStart()
		//{
		//	++pendingDispatch;
		//	if(pendingDispatch > pendingDispatchMax)
		//	{
		//		pendingDispatchMax = pendingDispatch + 5;
		//		Trace("PendingDispatch: " + pendingDispatch);
		//	}
		//}
		//public static void DispatchEnd() => --pendingDispatch;

		//public static void DispatchOnUIThreadSneakyLow(DispatcherQueueHandler action)
		//{
		//	var d = GlobalDispatcher();
		//	// run it immediately if we can
		//	if (d.HasThreadAccess && d.CurrentPriority <= DispatcherQueuePriority.Low)
		//		action();
		//	else
		//		d.RunAsync(DispatcherQueuePriority.Low, action);
		//}
		//public static async Task DispatchOnUIThreadSneakyLowAwait(DispatcherQueueHandler action)
		//{
		//	var d = GlobalDispatcher();
		//	// run it immediately if we can
		//	if (d.HasThreadAccess && d.CurrentPriority <= DispatcherQueuePriority.Low)
		//		action();
		//	else
		//		await d.RunAsync(DispatcherQueuePriority.Low, action);
		//}


		// We only have 1 UI thread here
		//public static bool IsKeyPressedControl()
		//{
		//    var window = CoreWindow.GetForCurrentThread();
		//    if (window == null)
		//    {
		//        return false;
		//    }

		//    return window.GetKeyState(VirtualKey.Control) == CoreVirtualKeyStates.Down;

		//}
		//public static bool IsKeyPressedShift()
		//{
		//    var window = CoreWindow.GetForCurrentThread();
		//    if (window == null)
		//    {
		//        return false;
		//    }

		//    return window.GetKeyState(VirtualKey.Shift) == CoreVirtualKeyStates.Down;
		//}



		///        public static DumbCollection<City> emptyCityList = new DumbCollection<City>();
		public static PercentFormatter percentFormatter = new PercentFormatter()
		{
			FractionDigits = 1,IsGrouped = false,
			NumberRounder  = new SignificantDigitsNumberRounder() { SignificantDigits = 1 }
		};

		public static DecimalFormatter formatter2Digit = new DecimalFormatter()
		{
			FractionDigits = 2,IsGrouped = true,
			NumberRounder  = new SignificantDigitsNumberRounder() { SignificantDigits = 2 }
		};

		public static DecimalFormatter formatterInt = new DecimalFormatter()
		{ FractionDigits = 0,IsGrouped = true,IsDecimalPointAlwaysDisplayed = false };

		public static DecimalFormatter formatterSeconds = new DecimalFormatter()
		{ FractionDigits = 0,IsGrouped = false,IntegerDigits = 2,IsDecimalPointAlwaysDisplayed = false };



		public static async Task<string> GetClipboardText()
		{
			try
			{
				return (await Windows.ApplicationModel.DataTransfer.Clipboard.GetContent().GetTextAsync()) ?? string.Empty;
			}
			catch(Exception ex)
			{
				LogEx(ex);
				return string.Empty;
			}
		}


		//		public static VirtualKeyModifiers canvasKeyModifiers;
		// HTML control messs wit this
		//		public static VirtualKeyModifiers keyModifiers => canvasKeyModifiers;



		//{
		//	get
		//	{
		//		var rv = VirtualKeyModifiers.None;
		//		if (IsKeyPressedShift())
		//			rv |= VirtualKeyModifiers.Shift;
		//		if (IsKeyPressedControl())
		//			rv |= VirtualKeyModifiers.Control;
		//		return rv;
		//	}
		//}

		public static InputCursor cursorDefault = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
		public static InputCursor cursorQuickBuild = InputSystemCursor.Create(InputSystemCursorShape.Person);
		public static InputCursor cursorMoveStart = InputSystemCursor.Create(InputSystemCursorShape.SizeNortheastSouthwest);
		public static InputCursor cursorMoveEnd = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthwestSoutheast);
		public static InputCursor cursorLayout = InputSystemCursor.Create(InputSystemCursorShape.Pin);
		public static InputCursor cursorDestroy = InputSystemCursor.Create(InputSystemCursorShape.UniversalNo);
		public static InputCursor cursorUpgrade = InputSystemCursor.Create(InputSystemCursorShape.UpArrow);
		public static InputCursor cursorDowngrade = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthSouth);

		public static Windows.Storage.ApplicationDataContainer ClientSettings()
		{
			var appData = Windows.Storage.ApplicationData.Current;
			if(appData.RoamingStorageQuota > 4)
				return appData.RoamingSettings;
			else
				return appData.LocalSettings;
		}

		public static async Task<byte[]> GetContent(string filename)
		{
			var uri = new Uri("ms-appx:///" + filename);
			var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);

			var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);

			return buffer.ToArray();
		}




		//	public static class UserAgent
		//	{
		//		const int URLMON_OPTION_USERAGENT = 0x10000001;

		//		[DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
		//		private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength,
		//														int dwReserved);

		//		[DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
		//		private static extern int UrlMkGetSessionOption(int     dwOption, StringBuilder pBuffer, int dwBufferLength,
		//														ref int pdwBufferLength, int dwReserved);

		//		public static string GetUserAgent()
		//		{
		//			int capacity = 255;
		//			var buf      = new StringBuilder(capacity);
		//			int length   = 0;

		//			UrlMkGetSessionOption(URLMON_OPTION_USERAGENT, buf, capacity, ref length, 0);

		//			return buf.ToString();
		//		}

		//		public static void SetUserAgent(string agent)
		//		{
		//			var hr = UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, agent, agent.Length, 0);
		//			var ex = Marshal.GetExceptionForHR(hr);
		//			if (null != ex)
		//			{
		//				throw ex;
		//			}
		//		}

		//		public static void AppendUserAgent(string suffix)
		//		{
		//			SetUserAgent(GetUserAgent() + suffix);
		//		}

		//	}

	}

	public static class AApp
	{


		public static MenuFlyoutItem CreateMenuItem(string text,Action command,object context = null)
		{
			var rv = new MenuFlyoutItem() { Text = text };
			rv.DataContext = context;
			if(command != null)
				rv.Click += (_,_) => command();
			return rv;
		}
		public static MenuFlyoutItem CreateMenuItem(string text,System.Windows.Input.ICommand command,object parameter,object context = null)
		{
			return new MenuFlyoutItem() { Text = text,Command = command,CommandParameter = parameter,DataContext = context };
		}
		public static MenuFlyoutItem CreateMenuItem(string text,RoutedEventHandler command,object context = null)
		{
			var rv = new MenuFlyoutItem() { Text = text };
			rv.DataContext = context;
			if(command != null)
				rv.Click += command;
			
			return rv;
		}
		public static MenuFlyoutItem CreateMenuItem(string text,bool isChecked,Action<bool> command)
		{
			var rv = new ToggleMenuFlyoutItem() { Text = text,IsChecked = isChecked };

			rv.Click += (sender,_) => command((sender as ToggleMenuFlyoutItem).IsChecked);
			return rv;
		}
		public static MenuFlyoutItem AddItem(this MenuFlyout menu,string text,RoutedEventHandler command,object context = null)
		{
			var rv = CreateMenuItem(text,command,context);

			menu.Items.Add(rv);
			return rv;
		}
		public static MenuFlyoutItem AddItem(this MenuFlyoutSubItem menu,string text,RoutedEventHandler command,object context = null)
		{
			var rv = CreateMenuItem(text,command,context);

			menu.Items.Add(rv);
			return rv;
		}
		public static MenuFlyoutItem AddItem(this MenuFlyout menu,string text,bool isChecked,Action<bool> command)
		{
			var rv = CreateMenuItem(text,isChecked,command);

			menu.Items.Add(rv);
			return rv;
		}
		public static MenuFlyoutItem AddItem(this MenuFlyoutSubItem menu,string text,bool isChecked,Action<bool> command)
		{
			var rv = CreateMenuItem(text,isChecked,command);

			menu.Items.Add(rv);
			return rv;
		}
		public static MenuFlyoutSubItem AddSubMenu(this MenuFlyout menu,string text)
		{
			var rv = new MenuFlyoutSubItem() { Text = text };

			menu.Items.Add(rv);
			return rv;
		}
		public static MenuFlyoutSubItem AddSubMenu(this MenuFlyoutSubItem menu,string text)
		{
			var rv = new MenuFlyoutSubItem() { Text = text };

			menu.Items.Add(rv);
			return rv;
		}
		public static void RemoveEmpy(this MenuFlyout menu)
		{
			for(int i = menu.Items.Count;--i >= 0;)
			{
				if(menu.Items[i] is MenuFlyoutSubItem sub)
				{
					if(sub.Items.Count == 0)
						menu.Items.RemoveAt(i);
				}
			}
		}

		public static MenuFlyoutItem AddItem(this MenuFlyout menu,string text,IconElement icon,Action command)
		{
			var rv = new MenuFlyoutItem() { Text = text,Icon= icon };
			if(command != null)
				rv.Click += (_,_) => command();
			menu.Items.Add(rv);
			return rv;
		}

		internal static void Show(this MenuFlyout flyout,UIElement sender,ContextRequestedEventArgs? args)
		{
			if(args is not null && args.TryGetPosition(sender,out var c))
			{
				flyout.ShowAt(sender,c);
			}
			else if(args is not null && args.TryGetPosition(ShellPage.rootGrid,out var c2))
			{
				flyout.ShowAt(ShellPage.rootGrid,c2);
			}
			else
			{
				flyout.ShowAt(sender,(sender.ActualSize*0.5f).AsPoint() );
				
			}
		}

		public static MenuFlyoutItem AddItem(this MenuFlyout menu,string text,Symbol symbol,Action command) => AddItem(menu,text,new SymbolIcon(symbol),command);
		public static MenuFlyoutItem AddItem(this MenuFlyout menu,string text,Symbol symbol,UIColor iconColor,Action command) => AddItem(menu,text,new SymbolIcon(symbol) { Foreground=AppS.Brush(iconColor) },command);
		public static MenuFlyoutItem AddItem(this MenuFlyout menu,string text,char glyph,Action command) => AddItem(menu,text,new FontIcon() {FontFamily=XamlHelper.cnvFont, Glyph=glyph.ToString() },command);
		public static MenuFlyoutItem AddItem(this MenuFlyout menu,string text,Action command) => AddItem(menu,text,icon: null,command: command);
		public static void AddSeparator(this MenuFlyout menu) => menu.Items.Add( new MenuFlyoutSeparator() );

		public static MenuFlyoutItem AddItem(this MenuFlyoutSubItem menu,string text,Action command)
		{
			var rv = new MenuFlyoutItem() { Text = text };
			if(command != null)
				rv.Click += (_,_) => command();
			menu.Items.Add(rv);
			return rv;
		}
		public static MenuFlyoutItem AddItem(this MenuFlyout menu,ICommand command,object parameter = null)
		{
			var rv = new MenuFlyoutItem() { Command = command,CommandParameter=parameter };
			menu.Items.Add(rv);
			return rv;
		}

		public static MenuFlyoutItem AddItem<T>(this MenuFlyout menu,StandardUICommandKind kind,T parameter,Action<T> action)
		{
			var rv = new MenuFlyoutItem() { Command = kind.Create(action),CommandParameter=parameter };
			menu.Items.Add(rv);
			return rv;
		}
		public static MenuFlyoutItem AddItem<T>(this MenuFlyout menu,StandardUICommandKind kind,Action action)
		{
			var rv = new MenuFlyoutItem() { Command = kind.Create(action) };
			menu.Items.Add(rv);
			return rv;
		}

	//	private static void C_ExecuteRequested(XamlUICommand sender,ExecuteRequestedEventArgs args) => throw new NotImplementedException();

		// must be on the right thread for this
		//public static void Set(this InputCursor c) 
		//{
		//	// is this thread safe?
		//	//if(ShellPage.coreInputSource != null)
		//	//{				
		//	//	ShellPage.coreInputSource.DispatcherQueue.EnqueueAsync(() =>

		//	//		ShellPage.coreInputSource.PointerCursor = type,DispatcherQueuePriority.Low);

		//	//}
		////	AppS.QueueOnUIThread( () =>	CoreWindow.GetForCurrentThread().PointerCursor = type);
		//}

		public static bool IsLocalPointOver(this FrameworkElement e,int x,int y)
		{
			return x >=0 && y >= 0 && x < e.ActualWidth && y < e.ActualHeight;
		}
		public static bool IsParentPointOver(this FrameworkElement e,double x,double y)
		{
			var off = e.ActualOffset;
			return IsLocalPointOver(e,x-off.X,y-off.Y);
		}
		public static bool IsLocalPointOver(this FrameworkElement e,double x,double y)
		{
			return x >=0 && y >= 0 && x < e.ActualWidth && y < e.ActualHeight;
		}


		//	static void LoadConfig()
		//{
		//	var config = BuildConfig();

		//	// Get the Google Spreadsheet Config Values
		//	var serviceAccount = config["GOOGLE_SERVICE_ACCOUNT"];
		//	var documentId = config["GOOGLE_SPREADSHEET_ID"];
		//	var jsonCredsPath = config["GOOGLE_JSON_CREDS_PATH"];

		//	// In this case the json creds file is stored locally, but you can store this however you want to (Azure Key Vault, HSM, etc)
		//	var jsonCredsContent = File.ReadAllText(jsonCredsPath);

		//	// Create a new SheetHelper class
		//	var sheetHelper = new SheetHelper(documentId,serviceAccount,"");
		//	sheetHelper.Init(jsonCredsContent);

		//	// Get all the rows for the first 2 columns in the spreadsheet
		//	var rows = sheetHelper.GetRows(new SheetRange("",1,1,2));

		//	// Write all the values from the result set
		//	foreach(var row in rows)
		//	{
		//		foreach(var col in row)
		//		{
		//			Console.Write($"{col}\t");
		//		}
		//		Console.Write("\n");
		//	}

		//	// export a csv file from the current spreadsheet and tab
		//	var exporter = new SheetExporter(sheetHelper);

		//	var filepath = @"output.csv";

		//	using(var stream = new FileStream(filepath,FileMode.Create))
		//	{
		//		var range = new SheetRange("",1,1,2);
		//		exporter.ExportAsCsv(range,stream);
		//	}
		//}

	}

}
