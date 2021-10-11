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

namespace COTG
{
		public class Debounce
		{
			public Func<Task> func;
			public int debounceDelay = 250;
			public int throttleDelay = 750;
			public bool runOnUiThread;
			int nextCall = Environment.TickCount;
			
//			public TaskCompletionSource<bool> complete;
		
			enum State : int
			{
				idle,
				pending,
				running,
			}
		// needed to Atomic operations
			int _state;
			State state { 
			get=> (State)_state;
			set=> _state = (int)value;
		}
			public Debounce(Func<Task> _func)
			{
				func = _func;
			}
			public void Go(bool throttled = false,bool runAgainIfStarted=true, int delayOverride=0)
			{
				if(delayOverride == 0)
					delayOverride = debounceDelay;
				switch (state)
				{
					case State.idle:
						if(throttled)
						{
						// cooldown
							var dt = nextCall - Environment.TickCount;
							if (dt > 0)
								return;
						}
						break;
					case  State.pending:
						{
						// delay further
							var nextT = Environment.TickCount +delayOverride;
							if (nextT - nextCall > 0)
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
				
				if(Interlocked.CompareExchange( ref _state,(int)State.pending,(int)State.idle)	!= (int)State.idle)
				{
					// another task got to it first
					// try again
					Go(throttled,runAgainIfStarted,delayOverride);
					return;				
				}
				nextCall = nextCall.Max( Environment.TickCount + delayOverride);
				Task.Run(async () =>
			   {

				   for (; ; )
				   {
					   Assert(state == State.pending);
					   for (; ; )
					   {
						   var dt = nextCall - Environment.TickCount;
						   if (dt <= 0)
						   {

							   break;
						   }
						   else
						   {
							   await Task.Delay(dt + 32).ConfigureAwait(false);
						   }
					   }
					   try
					   {
						   
						   state = State.running;
						   
						   if (runOnUiThread)
							   await App.DispatchOnUIThreadTask(func).ConfigureAwait(false);
						   else
							   await func().ConfigureAwait(false);
					   }
					   catch (Exception ex)
					   {
						   COTG.Debug.LogEx(ex);
					   }
					   
					   nextCall = Environment.TickCount + throttleDelay;
					   // someone might have changed us back to pending
					   if (state == State.running)
					   {
						   state = State.idle;
						   break;
					   }

				   }
				   
			   });

			}


		static ConcurrentDictionary<long,CancellationTokenSource> _tokens = new ();
		public static void Q(DispatcherQueueHandler action,int ms=200,
			bool runOnUIThread=false,
			long hash=0L,
			[CallerFilePath] string uniqueKey = "",
			[CallerLineNumber] int uniqueNumber = 0)

		{
			Assert(uniqueNumber != 0);
			Assert(uniqueKey.Length > 1);

			var key = hash==0L? ((long)uniqueKey.XxHash() + (long)uniqueNumber) : hash;

			var token = _tokens.AddOrUpdate(key,
				(key) => //key not found - create new
				{
					return new CancellationTokenSource();
				},
				(key,existingToken) => //key found - cancel task and recreate
				{
					existingToken.Cancel(); //cancel previous
					return new CancellationTokenSource();
				}
			);

			Task.Delay(ms,token.Token).ContinueWith(task =>
			{
				if(!task.IsCanceled)
				{
					_tokens.TryRemove(key,out _);
					try
					{
						if(runOnUIThread)
							App.DispatchOnUIThread(action);
						else
							action();
					}
					catch(Exception ex)
					{
						LogEx(ex, extra:$"{uniqueKey}({uniqueNumber}): {ex.ToString()}");
					}
				}
			},token.Token).ConfigureAwait(false);
		}
	}
	
}
