using System.Diagnostics;
using System.IO;
using System.Threading;

namespace System.Fx.Demo
{
	public class Solution2
	{
		public void DeleteDirectoryTree(DirectoryInfo directory)
		{
			DirectoryInfo[] children = null;
			try
			{
				children = directory.GetDirectories();
			}
			catch (Exception e)
			{
				Trace.TraceError("{0}", e);
			}
			if (children != null)
			{
				foreach (var child in children)
				{
					DeleteDirectoryTree(child);
				}
			}

			FileInfo[] files = null;
			try
			{
				files = directory.GetFiles();
			}
			catch (Exception e)
			{
				Trace.TraceError("{0}", e);
			}
			if (files != null)
			{
				foreach (var file in files)
				{
					try
					{
						var count = 0;
						while (true)
						{
							count++;
							try
							{
								file.Delete();
								break; // if success
							}
							catch (Exception e)
							{
								if (count >= 5)
									throw;
								Trace.TraceWarning("{0}", e);
								Thread.Sleep(1000);
							}
						}
					}
					catch (Exception e)
					{
						Trace.TraceError("{0}", e);
					}
				}
			}

			try
			{
				directory.Delete();
			}
			catch (Exception e)
			{
				Trace.TraceError("{0}", e);
			}
		}
	}
}
