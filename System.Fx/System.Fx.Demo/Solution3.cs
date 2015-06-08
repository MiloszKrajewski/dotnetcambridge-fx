using System.IO;
using System.Threading;
using System.Linq;

// ReSharper disable ConvertClosureToMethodGroup
// ReSharper disable AccessToForEachVariableInClosure

namespace System.Fx.Demo
{
	public class Solution3
	{
		public static void DeleteDirectoryTree(DirectoryInfo directory)
		{
			foreach (var child in Fx.Forgive(() => directory.GetDirectories()).NotNull())
			{
				DeleteDirectoryTree(child);
			}

			foreach (var file in Fx.Forgive(() => directory.GetFiles()).NotNull())
			{
				Fx.Forgive(
					() => Fx.Retry(
						() => file.Delete(),
						(c, _) => c < 5,
						_ => Thread.Sleep(200)));
			}

			Fx.Forgive(() => directory.Delete());
		}
	}
}

// ReSharper restore AccessToForEachVariableInClosure
// ReSharper restore ConvertClosureToMethodGroup
