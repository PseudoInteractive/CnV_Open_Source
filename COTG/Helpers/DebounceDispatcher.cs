using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static COTG.Debug;

//using Windows.UI.Core;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.UI.Dispatching;
using System.Collections.Immutable;
using CommunityToolkit.WinUI.UI;

namespace COTG
{
	public class Debounce
	{
		public Func<Task> func;
		public int debounceDelay = 250;
		public int throttleDelay = 750;
		public bool runOnUiThread;
		TickT nextCall;
		public int throttleOrDebounceDelay => throttleDelay == -1 ? debounceDelay : throttleDelay;
	
 		//			public TaskCompletionSource<bool> complete;

		enum State:int
		{
			idle,
			pending,
			running,
		}
		// needed to Atomic operations
		int _state;
		/// <summary>
		/// If set, 
		/// </summary>
		public bool throttled;
		TaskCompletionSource taskCompletionSource;
		State state
		{
			get => (State)_state;
			set => _state = (int)value;
		}
		public Debounce(Func<Task> _func)
		{
			func = _func;
		}
		public void Go(int delayOverride = 0)
		{
			if(delayOverride == 0)
				delayOverride = debounceDelay;
			switch(state)
			{
				case State.idle:
					if(throttled)
					{
						// cooldown
						var dt = nextCall - ATime.TickCount;
						if(dt > 0)
							return;
					}
					break;
				case State.pending:
					{
						// delay further
						var nextT = ATime.TickCount +delayOverride;
						if(nextT - nextCall > 0)
						{
							nextCall = nextT;
						}
						return;
					}
				case State.running:
					{
						if( !throttled)
							state = State.pending; // we are already in the inner loop, tell the current one to restart once done

						return;
					}
				default:
					{
						Assert(false);
						return;
					}
			}

			if(Interlocked.CompareExchange(ref _state,(int)State.pending,(int)State.idle)  != (int)State.idle)
			{
				// another task got to it first
				// try again
				Go(delayOverride);
				return;
			}
			nextCall = nextCall.Max(ATime.TickCount + throttleOrDebounceDelay);
			Task.Run(async () =>
		   {

			   for(;;)
			   {
				   Assert(state == State.pending);
				   for(;;)
				   {
					   var dt = nextCall - ATime.TickCount;
					   if(dt <= 0)
					   {

						   break;
					   }
					   else
					   {
						   await Task.Delay( ((int)(dt + 32)).Min(200) ).ConfigureAwait(false);
					   }
				   }
				   try
				   {

					   state = State.running;
					   
					   if(runOnUiThread)
						   await App.DispatchOnUIThreadTask(func).ConfigureAwait(false);
					   else
						   await func().ConfigureAwait(false);
				   }
				   catch(Exception ex)
				   {
					   COTG.Debug.LogEx(ex);
				   }

				   nextCall = ATime.TickCount + throttleOrDebounceDelay;
				   
				   var t = taskCompletionSource;
				   if(t!= null)
				   {
					   taskCompletionSource=new();
					   if(t!=null)
						   t.TrySetResult();
				   }
				   // someone might have changed us back to pending
				   if(state == State.running)
				   {
					   state = State.idle;
					   break;
				   }

			   }

		   });
		}

		public Task GoAsync(int delayOverride = 0)
		{
			if(taskCompletionSource == null)
			{
				Interlocked.CompareExchange(ref taskCompletionSource,new(),null);
			}
			Go(delayOverride);
			var rv = taskCompletionSource;
			if(rv!=null)
				return rv.Task;
			else
				return Task.CompletedTask;
		}

		static ConcurrentDictionary<long,Debounce> debouceCache = new();
		public static void Q(Func<Task> action,int debounceT = 200,int throttleT = -1,
			bool runOnUIThread = false,

			long hash = 0L,
			[CallerFilePath] string uniqueKey = "",
			[CallerLineNumber] int uniqueNumber = 0
			)

			{
				Assert(uniqueNumber != 0);
				Assert(uniqueKey.Length > 1);

				var key = hash == 0L ? ((long)uniqueKey.XxHash() + (long)uniqueNumber) : hash;

				var token = debouceCache.GetOrAdd(key,
					(_key) => //key not found - create new

						new Debounce(action) { debounceDelay = debounceT,throttleDelay = throttleT,runOnUiThread = runOnUIThread }
					);
				token.Go(debounceT);

			}
		
	}

	// 1 argument debounced
	public class Debounce1<T> : Debounce
	{
		Task _func()
		{
			return func1(arg);
		}
		Func<T,Task> func1;
		T arg;

		public Debounce1(Func<T,Task> _func1) : base(null)
		{
			func1 = _func1;
			func = _func;
		}
		// use Arg from Last Call
		public void Go(T _arg,int delayOverride = 0)
		{
			arg = _arg;
			base.Go(delayOverride);
		}


	}
		// these will return immediately in the throttle phase, if this is not desired set the throttle delay to 0
	public class DebounceTask
	{
			public Func<Task> func;
			public int debounceDelay = 250;
			public int throttleDelay = 750;
			public bool runOnUiThread;
			public bool throttled;
			TaskCompletionSource onComplete = null;
			TickT nextCall;
			TickT throttleEnd;
			public bool isInCooldown => (throttleEnd-ATime.TickCount) > 0;

			public DebounceTask(Func<Task> _func)
			{
				func = _func;
			}
			public Task Go(int delayOverride = -1)
			{
				if(delayOverride == -1)
					delayOverride = debounceDelay;
				
				Debug.Log("Go: " + (int)((nextCall-ATime.TickCount)*10) + " delay: " + delayOverride/100 ); 

				var rv = onComplete;

				if(rv != null)
				{
					// pending
					// delay further or if it is running, add a throttle timeout
				//	nextCall = nextCall.Max( ATime.TickCount +throttleDelay);
					Debug.Log("next TickT: " + (int)((nextCall-ATime.TickCount)*0.1f) ); 
					return rv.Task;
				}

				if (throttled && isInCooldown)
				{
					nextCall = nextCall.Max( ATime.TickCount+debounceDelay);
					return Task.CompletedTask;
				}

				rv = new();
				if(Interlocked.CompareExchange(ref onComplete,rv,null)  != null)
				{
					// another task got to it first
					// try again
					// let rv get garbage collected
					return Go(delayOverride);
				}


				nextCall = nextCall.Max(ATime.TickCount + delayOverride);
				Debug.Log("next Tick1: " + (int)((nextCall-ATime.TickCount)*0.1f) ); 

				Task.Run(async () =>
				{

					Assert(onComplete != null);
					for(;;)
					{
						var dt = nextCall - ATime.TickCount;
						if(dt <= 0)
						{

							break;
						}
						else
						{
							await Task.Delay( (int)(dt + 32)).ConfigureAwait(false);
						}
					}
					// this will be updated again when the task is complete
					nextCall = nextCall.Max( ATime.TickCount + throttleDelay);
					// Ready for next call
					// We are not yet done, but a call incoming while we are processing should only be completed when that call completes
					// There is a potential for overlap
					var wait = onComplete;
					onComplete = null;
					try
					{

						if(runOnUiThread)
							await App.DispatchOnUIThreadTask(func).ConfigureAwait(false);
						else
							await func().ConfigureAwait(false);
					}
					catch(Exception ex)
					{
						COTG.Debug.LogEx(ex);
					}
					Debug.Log("next Tick2: " + (int)((nextCall-ATime.TickCount)*10) ); 

					// ready for next call..
					// any
					wait.SetResult();

				});
				return rv.Task;
			}

	}
	
}
