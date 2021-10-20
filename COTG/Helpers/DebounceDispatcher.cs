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

namespace COTG
{
	public class Debounce
	{
		public Func<Task> func;
		public int debounceDelay = 250;
		public int throttleDelay = 750;
		public bool runOnUiThread;
		TickT nextCall;

	
 		//			public TaskCompletionSource<bool> complete;

		enum State:int
		{
			idle,
			pending,
			running,
		}
		// needed to Atomic operations
		int _state;
		State state
		{
			get => (State)_state;
			set => _state = (int)value;
		}
		public Debounce(Func<Task> _func)
		{
			func = _func;
		}
		public void Go(bool throttled = false,bool runAgainIfStarted = true,int delayOverride = 0)
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
						if(runAgainIfStarted && !throttled)
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
				Go(throttled,runAgainIfStarted,delayOverride);
				return;
			}
			nextCall = nextCall.Max(ATime.TickCount + delayOverride);
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
						   await Task.Delay( (int)(dt + 32) ).ConfigureAwait(false);
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

				   nextCall = ATime.TickCount + throttleDelay;
				   // someone might have changed us back to pending
				   if(state == State.running)
				   {
					   state = State.idle;
					   break;
				   }

			   }

		   });
		}
			static ConcurrentDictionary<long,Debounce> debouceCache = new();
			public static void Q(Func<Task> action,int ms = 200,
				bool runOnUIThread = false,
				long hash = 0L,
				[CallerFilePath] string uniqueKey = "",
				[CallerLineNumber] int uniqueNumber = 0)

			{
				Assert(uniqueNumber != 0);
				Assert(uniqueKey.Length > 1);

				var key = hash == 0L ? ((long)uniqueKey.XxHash() + (long)uniqueNumber) : hash;

				var token = debouceCache.GetOrAdd(key,
					(_key) => //key not found - create new

						new Debounce(action) { debounceDelay = ms,throttleDelay = ms,runOnUiThread = runOnUIThread }
					);
				token.Go();

			}
		
	}
	public class DebounceTask
	{
			public Func<Task> func;
			public int debounceDelay = 250;
			public int throttleDelay = 750;
			public bool runOnUiThread;
			TaskCompletionSource onComplete = null;
			TickT nextCall;
			TickT throttleEnd;
			public bool isInCooldown => (throttleEnd-ATime.TickCount) > 0;

			public DebounceTask(Func<Task> _func)
			{
				func = _func;
			}
			public Task Go(int delayOverride = 0)
			{
				if(delayOverride == 0)
					delayOverride = debounceDelay;

				var rv = onComplete;

				if(rv != null)
				{
					// pending
					// delay further or if it is running, add a throttle timeout
					var nextT = ATime.TickCount +delayOverride;
					if(nextT - nextCall > 0)
					{
						nextCall = nextT;
					}
					return rv.Task;
				}
				if(isInCooldown)
					return Task.CompletedTask;

				rv = new();
				if(Interlocked.CompareExchange(ref onComplete,rv,null)  != null)
				{
					// another task got to it first
					// try again
					// let rv get garbage collected
					return Go(delayOverride);
				}


				nextCall = nextCall.Max(ATime.TickCount + delayOverride);
			
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
					throttleEnd = ATime.TickCount + throttleDelay;
					var nextT = throttleEnd;
					if(nextT - nextCall > 0)
					{
						nextT = throttleEnd;
					}
					var wait = onComplete;
					onComplete = null;

					// ready for next call..
					// any
					wait.SetResult();

				});
				return rv.Task;
			}

	}
	
}
