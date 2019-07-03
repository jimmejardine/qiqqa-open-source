using System;
using System.IO;

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
