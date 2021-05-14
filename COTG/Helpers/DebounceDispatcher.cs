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
			public int debounceDelay = 300;
			public int throttleDelay = 300;
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
						{
							var dt = nextCall - Environment.TickCount;
							if (dt >= 0)
								return;
						}
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
							return;
						}
				default:
					{
						Assert(false);
						return;
					}
				}
				state = State.pending;
				nextCall = Environment.TickCount + debounceDelay;
				Task.Run(async () =>
			   {
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
				   finally
				   {
					   nextCall = Environment.TickCount + throttleDelay;
					   state = State.idle;
				   }
			   });

			}

	}
}
