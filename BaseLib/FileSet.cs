using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BaseLib
{
	public class FileSet
	{
		private List<TextFile> fileList = new List<TextFile>();
		private Vector vector = null;

		public FileSet(string path)
		{
			extractPath(path);
		}
		public FileSet(List<string> list)
		{
			foreach (string path in list)
			{
				extractPath(path);
			}
		}
		public FileSet(List<TextFile> list)
		{
			fileList.AddRange(list);
		}
		private void extractPath(string path)
		{
			if (Directory.Exists(path))
			{
				foreach (string filename in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
				{
					fileList.Add(new TextFile(filename));
				}
			}
			else if (File.Exists(path))
			{
				fileList.Add(new TextFile(path));
			}
			else
			{
				throw new FileNotFoundException("file not found", path);
			}
		}
		public Vector Analyze()
		{
			if (vector != null)
			{
				return vector;
			}
			vector = new Vector(0);
			foreach (TextFile file in fileList)
			{
				file.Analyze(this.vector);
			}
			return vector;
		}
	}
}
