using System;
using System.IO;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Utilities.Files
{
	public class FileOpener
	{
		public static StreamReader openStreamReaderWithLocalCheck(String filename)
		{
			if (File.Exists(filename))
			{
				return new StreamReader(filename);
			}

			else
			{
				return new StreamReader(Path.GetFileName(filename));
			}
		}
	}
}
