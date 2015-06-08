using System.Collections.Generic;
using System.Diagnostics;

namespace System.Fx.Demo
{
	public class ColorConsoleTraceListener: ConsoleTraceListener
	{
		private const ConsoleColor DefaultColor = ConsoleColor.White;

		private readonly Dictionary<TraceEventType, ConsoleColor> _eventColor = 
			new Dictionary<TraceEventType, ConsoleColor>();

		public ColorConsoleTraceListener()
		{
			_eventColor.Add(TraceEventType.Verbose, ConsoleColor.Gray);
			_eventColor.Add(TraceEventType.Information, ConsoleColor.Cyan);
			_eventColor.Add(TraceEventType.Warning, ConsoleColor.Yellow);
			_eventColor.Add(TraceEventType.Error, ConsoleColor.Red);
			_eventColor.Add(TraceEventType.Critical, ConsoleColor.Magenta);
		}

		public override void TraceEvent(
			TraceEventCache eventCache, 
			string source, TraceEventType eventType, int id, string message)
		{
			TraceEvent(eventCache, source, eventType, id, "{0}", message);
		}

		public override void TraceEvent(
			TraceEventCache eventCache, 
			string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			lock (typeof(Console))
			{
				var originalColor = Console.ForegroundColor;
				Console.ForegroundColor = GetEventColor(eventType, DefaultColor);
				base.TraceEvent(eventCache, source, eventType, id, format, args);
				Console.ForegroundColor = originalColor;
			}
		}

		private ConsoleColor GetEventColor(TraceEventType eventType, ConsoleColor defaultColor)
		{
			ConsoleColor color;
			return _eventColor.TryGetValue(eventType, out color) ? color : defaultColor;
		}
	}
}
