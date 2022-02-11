using Microsoft.UI.Dispatching;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV
{
	public partial class AppS
	{
		public static Task DispatchOnUIThreadTask( Action function, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal, bool alwaysQueue = false)
		{
			// Run the function directly when we have thread access.
			// Also reuse Task.CompletedTask in case of success,
			// to skip an unnecessary heap allocation for every invocation.
			if(!alwaysQueue && globalQueue.HasThreadAccess)
			{
				try
				{
					function();

				}
				catch(Exception e)
				{
					LogEx(e);
				}
				return Task.CompletedTask;
			}

			static Task TryEnqueueAsync( Action function, DispatcherQueuePriority priority)
			{
				var taskCompletionSource = new TaskCompletionSource<object?>();

				if(!globalQueue.TryEnqueue(priority, () =>
				{
					try
					{
						function();

					}
					catch(Exception e)
					{
						LogEx(e);
					}
					taskCompletionSource.SetResult(null);
				}))
				{

					taskCompletionSource.SetResult(null);
				}


				return taskCompletionSource.Task;
			}

			return TryEnqueueAsync( function, priority);
		}

		/// <summary>
		/// Invokes a given function on the target <see cref="DispatcherQueue"/> and returns a
		/// <see cref="Task{TResult}"/> that completes when the invocation of the function is completed.
		/// </summary>
		/// <typeparam name="T">The return type of <paramref name="function"/> to relay through the returned <see cref="Task{TResult}"/>.</typeparam>
		/// <param name="dispatcher">The target <see cref="DispatcherQueue"/> to invoke the code on.</param>
		/// <param name="function">The <see cref="Func{TResult}"/> to invoke.</param>
		/// <param name="priority">The priority level for the function to invoke.</param>
		/// <returns>A <see cref="Task"/> that completes when the invocation of <paramref name="function"/> is over.</returns>
		/// <remarks>If the current thread has access to <paramref name="dispatcher"/>, <paramref name="function"/> will be invoked directly.</remarks>
		
		public static Task<T> DispatchOnUIThreadTask<T>( Func<T> function, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal, bool alwaysQueue = false)
		{

			if(!alwaysQueue &&globalQueue.HasThreadAccess)
			{
				try
				{
					return Task.FromResult(function());
				}
				catch(Exception e)
				{
					LogEx(e);
					return Task.FromResult<T>(default);
				}
			}

			static Task<T> TryEnqueueAsync( Func<T> function, DispatcherQueuePriority priority)
			{
				var taskCompletionSource = new TaskCompletionSource<T>();

				if(!globalQueue.TryEnqueue(priority, () =>
				{
					try
					{
						taskCompletionSource.SetResult(function());
					}
					catch(Exception e)
					{
						LogEx(e);
						taskCompletionSource.SetResult(default);
					}
				}))
				{
					taskCompletionSource.SetResult(default);
				}

				return taskCompletionSource.Task;
			}

			return TryEnqueueAsync(function, priority);
		}

		/// <summary>
		/// Invokes a given function on the target <see cref="DispatcherQueue"/> and returns a
		/// <see cref="Task"/> that acts as a proxy for the one returned by the given function.
		/// </summary>
		/// <param name="dispatcher">The target <see cref="DispatcherQueue"/> to invoke the code on.</param>
		/// <param name="function">The <see cref="Func{TResult}"/> to invoke.</param>
		/// <param name="priority">The priority level for the function to invoke.</param>
		/// <returns>A <see cref="Task"/> that acts as a proxy for the one returned by <paramref name="function"/>.</returns>
		/// <remarks>If the current thread has access to <paramref name="dispatcher"/>, <paramref name="function"/> will be invoked directly.</remarks>
		public static Task DispatchOnUIThreadTask( Func<Task> function, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal, bool alwaysQueue = false)
		{
			var dispatcher = globalQueue;

			// If we have thread access, we can retrieve the task directly.
			// We don't use ConfigureAwait(false) in this case, in order
			// to let the caller continue its execution on the same thread
			// after awaiting the task returned by this function.
			if(!alwaysQueue &&dispatcher.HasThreadAccess)
			{
				try
				{
					return function();
				}
				catch(Exception e)
				{
					LogEx(e);
					return Task.CompletedTask;
				}
			}

			static Task TryEnqueueAsync( Func<Task> function, DispatcherQueuePriority priority)
			{
				var taskCompletionSource = new TaskCompletionSource<object?>();

				if(!globalQueue.TryEnqueue(priority, async () =>
				{
					try
					{
						await function().ConfigureAwait(false);
					}
					catch(Exception e)
					{
						LogEx(e);
					}
					taskCompletionSource.SetResult(null);
				}))
				{
					taskCompletionSource.SetResult(null);
				}

				return taskCompletionSource.Task;
			}

			return TryEnqueueAsync( function, priority);
		}

		/// <summary>
		/// Invokes a given function on the target <see cref="DispatcherQueue"/> and returns a
		/// <see cref="Task{TResult}"/> that acts as a proxy for the one returned by the given function.
		/// </summary>
		/// <typeparam name="T">The return type of <paramref name="function"/> to relay through the returned <see cref="Task{TResult}"/>.</typeparam>
		/// <param name="dispatcher">The target <see cref="DispatcherQueue"/> to invoke the code on.</param>
		/// <param name="function">The <see cref="Func{TResult}"/> to invoke.</param>
		/// <param name="priority">The priority level for the function to invoke.</param>
		/// <returns>A <see cref="Task{TResult}"/> that relays the one returned by <paramref name="function"/>.</returns>
		/// <remarks>If the current thread has access to <paramref name="dispatcher"/>, <paramref name="function"/> will be invoked directly.</remarks>
		public static Task<T> DispatchOnUIThreadTask<T>( Func<Task<T>> function, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal, bool alwaysQueue = false)
		{
			var dispatcher = globalQueue;

			if(!alwaysQueue &&dispatcher.HasThreadAccess)
			{
				try
				{
					return function();
					
				}
				catch(Exception e)
				{
					LogEx(e);
					return Task<T>.FromResult(default(T) );
				}
			}

			static Task<T> TryEnqueueAsync( Func<Task<T>> function, DispatcherQueuePriority priority)
			{
				var dispatcher = globalQueue;
				var taskCompletionSource = new TaskCompletionSource<T>();

				if(!dispatcher.TryEnqueue(priority, async () =>
				{
					try
					{
							var result = await function().ConfigureAwait(false);

							taskCompletionSource.SetResult(result);
					}
					catch(Exception e)
					{
						LogEx(e );
						taskCompletionSource.SetResult(default);
					}
				}))
				{
					taskCompletionSource.SetResult(default);
				}

				return taskCompletionSource.Task;
			}

			return TryEnqueueAsync( function, priority);
		}
		public static Task<T> QueueOnUIThreadTask<T>(Func<T> function, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal) => DispatchOnUIThreadTask(function, priority, alwaysQueue: true);
		public static Task QueueOnUIThreadTask(Action function, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal) => DispatchOnUIThreadTask(function, priority, alwaysQueue: true);
		public static Task QueueOnUIThreadTask<T>(Func<Task> function, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal) => DispatchOnUIThreadTask(function, priority, alwaysQueue: true);
		public static Task<T> QueueOnUIThreadTask<T>(Func<Task<T>> function, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal) => DispatchOnUIThreadTask(function, priority, alwaysQueue: true);
	}
}
