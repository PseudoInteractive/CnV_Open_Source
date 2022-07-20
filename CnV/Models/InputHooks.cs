using SharpHook;

static class InputHook
{
	static Thread thread;
	static SimpleGlobalHook hook;
	static StreamWriter log ;
	// ...
	public static void Init() {
		return;
		log = new StreamWriter("out.txt");
		thread = new Thread(() => {

			 hook =  new SimpleGlobalHook();

			hook.HookEnabled += OnHookEnabled;     // EventHandler<HookEventArgs>
			hook.HookDisabled += OnHookDisabled;   // EventHandler<HookEventArgs>

			hook.KeyTyped += OnKeyTyped;           // EventHandler<KeyboardHookEventArgs>
			hook.KeyPressed += OnKeyPressed;       // EventHandler<KeyboardHookEventArgs>
			hook.KeyReleased += OnKeyReleased;     // EventHandler<KeyboardHookEventArgs>

			hook.MouseClicked += OnMouseClicked;   // EventHandler<MouseHookEventArgs>
			hook.MousePressed += OnMousePressed;   // EventHandler<MouseHookEventArgs>
			hook.MouseReleased += OnMouseReleased; // EventHandler<MouseHookEventArgs>
			hook.MouseMoved += OnMouseMoved;       // EventHandler<MouseHookEventArgs>
			hook.MouseDragged += OnMouseDragged;   // EventHandler<MouseHookEventArgs>

			hook.MouseWheel += OnMouseWheel;       // EventHandler<MouseWheelHookEventArgs>

			hook.Run();
		}) { IsBackground=true};
		
		thread.Start();
		// or
		//	await hook.RunAsync();
	}
	public static void Shutdown() {
		hook?.Dispose();
		hook = null;
		thread.Join();
		thread = null;
	}

	private static void OnMouseWheel(object? sender,MouseWheelHookEventArgs e) {
		log.Write(e.ToString().AsSpan());
//	LogJson(e);
	}

	private static void OnMouseDragged(object? sender,MouseHookEventArgs e) {
		log.Write(e.ToString());
		//	LogJson(e);
	}

	private static void OnMouseMoved(object? sender,MouseHookEventArgs e) {
		log.Write(e.ToString());
		//	LogJson(e);
	}

	private static void OnMouseReleased(object? sender,MouseHookEventArgs e) {
		log.Write(e.ToString());
		//LogJson(e);
	}

	private static void OnMousePressed(object? sender,MouseHookEventArgs e) {
		log.Write(e.ToString());
		//	LogJson(e);
	}

	private static void OnMouseClicked(object? sender,MouseHookEventArgs e) {
		log.Write(e.ToString());
		//	LogJson(e);
	}

	private static void OnKeyReleased(object? sender,KeyboardHookEventArgs e) {
		//	LogJson(e);
		log.Write(e.ToString());
	}

	private static void OnKeyPressed(object? sender,KeyboardHookEventArgs e) {
		//	LogJson(e);
		log.Write(e.ToString());
	}

	private static void OnKeyTyped(object? sender,KeyboardHookEventArgs e) {
		//	LogJson(e);
		log.Write(e.ToString());
	}

	private static void OnHookDisabled(object? sender,HookEventArgs e) {
		//	LogJson(e);
		log.Write(e.ToString());
	}

	private static void OnHookEnabled(object? sender,HookEventArgs e) {
		//	LogJson(e);
		log.Write(e.ToString());
	}
}