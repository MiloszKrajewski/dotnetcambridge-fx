using System.IO;

namespace System.Fx.Demo
{
	public class Solution1
	{
		public static void DeleteDirectoryTree(DirectoryInfo directory)
		{
			foreach (var child in directory.GetDirectories())
			{
				DeleteDirectoryTree(child);
			}

			foreach (var file in directory.GetFiles())
			{
				file.Delete();
			}

			directory.Delete();
		}
	}
}
