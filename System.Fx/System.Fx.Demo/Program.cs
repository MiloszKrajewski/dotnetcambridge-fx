using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Fx.Demo
{
	class Program
	{
		internal static IEnumerable<int> Generate()
		{
			for (var i = 0; i < 10; i++)
			{
				Trace.TraceInformation("generate: {0}", i);
				yield return i;
			}
		}

		internal static void Consume(IEnumerable<int> collection)
		{
			foreach (var i in collection)
			{
				Trace.TraceInformation("consume: {0}", i);
				Thread.Sleep(1000);
			}
		}

		static void Main(string[] args)
		{
			Trace.Listeners.Add(new ColorConsoleTraceListener());

			Generate().Lookahead().Apply(x => Consume(x));

			Console.ReadLine();
		}
	}
}
