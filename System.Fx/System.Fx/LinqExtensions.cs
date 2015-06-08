using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace System.Linq
{
	public static class LinqExtensions
	{
		public static IEnumerable<T> NotNull<T>(this IEnumerable<T> collection)
		{
			return collection ?? Enumerable.Empty<T>();
		}

		internal static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
		{
			foreach (var item in collection) 
				action(item);
		}

		public static IEnumerable<T> RecursivelySelectMany<T>(
			this IEnumerable<T> collection,
			Func<T, IEnumerable<T>> selector)
		{
			foreach (var item in collection)
			{
				foreach (var subitem in RecursivelySelectMany(selector(item), selector))
					yield return subitem;
				yield return item;
			}
		}

		public static IEnumerable<T> Lookahead<T>(
			this IEnumerable<T> collection, CancellationToken? token = null)
		{
			token = token ?? CancellationToken.None;
			var queue = new BlockingCollection<T>(new ConcurrentQueue<T>());
			Task.Factory.StartNew(() => {
				try
				{
					collection.ForEach(i => queue.Add(i, token.Value));
				}
				finally
				{
					queue.CompleteAdding();
				}
			}, token.Value, TaskCreationOptions.LongRunning, TaskScheduler.Default);
			return queue.GetConsumingEnumerable(token.Value);
		}
	}
}
