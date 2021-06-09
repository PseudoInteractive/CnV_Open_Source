using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static COTG.Debug;

using Windows.UI.Core;

namespace COTG
{
		public class Debounce
		{
			public Func<Task> func;
			public int debounceDelay = 250;
			public int throttleDelay = 750;
			public bool runOnUiThead;
			int nextCall = Environment.TickCount;
			enum State
			{
				idle,
				pending,
				running,


			};
			State state;
			public Debounce(Func<Task> _func)
			{
				func = _func;
			}
			public void Go()
			{
				switch (state)
				{
					case State.idle:
						//{
						//	var dt = nextCall - Environment.TickCount;
						//	if (dt > 0)
						//		return;
						//}
						break;
					case  State.pending:
						{
							var nextT = Environment.TickCount + debounceDelay;
							if (nextT - nextCall > 0)
							{
								nextCall = nextT;
							}
							return;
					}
					case State.running:
					{
						state = State.pending; // we are already in the inner loop, leave the current one running
						return;
					}
				default:
					{
						Assert(false);
						return;
					}
				}
				state = State.pending;
				nextCall = nextCall.Max( Environment.TickCount + debounceDelay );
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
							   await Task.Delay(dt + 10);
						   }
					   }
					   try
					   {
						   state = State.running;
						   if (runOnUiThead)
							   await App.DispatchOnUIThreadTask(func);
						   else
							   await func();
					   }
					   catch (Exception ex)
					   {
						   COTG.Debug.LogEx(ex);
					   }
					   nextCall = Environment.TickCount + throttleDelay;
					   if (state == State.running)
					   {
						   state = State.idle;
						   break;
					   }

				   }
				   
			   });

			}

	}
}
