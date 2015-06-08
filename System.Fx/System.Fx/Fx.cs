using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Fx
{
	public static class Fx
	{
		public static readonly Void Void = Void.Instance;

		public static Func<Void> ToFunc(this Action action)
		{
			return () => {
				action();
				return Void;
			};
		}

		public static Func<T, Void> ToFunc<T>(this Action<T> action)
		{
			return t => {
				action(t);
				return Void;
			};
		}

		public static T Forgive<T>(this Func<T> func, T defaultValue = default(T))
		{
			try
			{
				return func();
			}
			catch (Exception e)
			{
				Trace.TraceWarning("{0}", e);
				return defaultValue;
			}
		}

		public static void Forgive(this Action action)
		{
			Forgive(action.ToFunc());
		}

		public static T Retry<T>(
			Func<T> action, Func<int, TimeSpan, bool> retry, Action<TimeSpan> wait = null)
		{
			var count = 0;
			var started = DateTimeOffset.Now;
			var exceptions = new List<Exception>();
			wait = wait ?? (_ => { });

			while (true)
			{
				count++;

				try
				{
					return action();
				}
				catch (Exception e)
				{
					Trace.TraceWarning("{0}", e);
					exceptions.Add(e);
				}

				var elapsed = DateTimeOffset.Now.Subtract(started);
				if (!retry(count, elapsed))
					break;
				wait(elapsed);
			}

			throw new AggregateException(exceptions);
		}

		public static void Retry(
			Action action, Func<int, TimeSpan, bool> retry, Action<TimeSpan> wait = null)
		{
			Retry(action.ToFunc(), retry, wait);
		}

		private static ConcurrentDictionary<Guid, Timer> _timerMap =
			new ConcurrentDictionary<Guid, Timer>();

		/// <summary>Defers the operation by given time.</summary>
		/// <param name="timeout">The timeout.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="action">The action.</param>
		private static void Delay(int timeout, CancellationToken cancellationToken, Action action)
		{
			var id = Guid.NewGuid();
			var timer = default(Timer);
			var ready = new ManualResetEventSlim(false);
			var handler = new TimerCallback(_ => {
				ready.Wait();
				ready.Dispose();

				try
				{
					Timer removed;
					var execute = 
						!cancellationToken.IsCancellationRequested &&
						_timerMap.TryRemove(id, out removed);
					if (execute)
						action();
				}
				finally
				{
					timer.Dispose();
				}
			});

			timer = new Timer(handler, null, timeout, Timeout.Infinite);
			_timerMap.TryAdd(id, timer);
			ready.Set();
		}

		public static Func<T> Lazy<T>(Func<T> factory)
		{
			var variable = new Lazy<T>(factory);
			return () => variable.Value;
		}

		public static Func<T> Weak<T>(T value)
			where T: class
		{
			var reference = new WeakReference(value);
			return () => (T)reference.Target;
		}

		public static Func<T> Cache<T>(Func<T> factory)
			where T: class
		{
			var factorySync = new object();
			Func<T> reference = () => null;
			return () => {
				var value = reference();
				if (null == value)
				{
					lock (factorySync)
					{
						value = reference();
						if (null == value)
						{
							value = factory();
							reference = Weak(value);
						}
					}
				}
				return value;
			};
		}

		//----------------------------------------------------------------------------

		public static Action NotNull<T>(this Action action)
		{
			return action ?? (() => { });
		}

		public static Action<T> NotNull<T>(this Action<T> action)
		{
			return action ?? (_ => { });
		}

		public static T Apply<T>(this T subject, Action<T> action)
		{
			action(subject);
			return subject;
		}

		public static U Apply<T, U>(this T subject, Func<T, U> func)
		{
			return func(subject);
		}
	}
}
