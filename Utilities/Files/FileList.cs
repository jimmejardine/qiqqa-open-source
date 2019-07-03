using System;
using System.Collections;
using System.IO;

namespace Utilities.Files
{
	public class FileList
	{
		protected ArrayList _filenames;

		public FileList()
		{
			_filenames = new ArrayList();
		}

		public void clear()
		{
			_filenames.Clear();
		}
		
		public void addFile(String filename)
		{
			_filenames.Add(filename);
		}

		public void addFiles(String[] filenames)
		{
			for (int i = 0; i < filenames.Length; ++i)
			{
				addFile(filenames[i]);
			}
		}

		public void addDirectoryContents(String directoryname, String filter)
		{
			String[] filenames = Directory.GetFiles(directoryname, filter);
			for (int i = 0; i < filenames.Length; ++i)
			{			
				String filename = filenames[i];
				_filenames.Add(filename);
			}
		}

		public void addDirectoryContentsRecursively(String directoryname, String filter)
		{
			// First add the images in this directory
			addDirectoryContents(directoryname, filter);

			// And then recurse through each subdirectory
			String[] subdirectories = Directory.GetDirectories(directoryname);
			for (int i = 0; i < subdirectories.Length; ++i)
			{
				String subdirectoryname = subdirectories[i];
				addDirectoryContentsRecursively(subdirectoryname, filter);
			}
		}

		public IEnumerator GetEnumerator()
		{
			return _filenames.GetEnumerator();
		}

		public Int32 Count
		{
			get
			{
				return _filenames.Count;
			}
		}
	}
}
