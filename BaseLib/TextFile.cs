using System;
using System.Collections.Generic;
using System.IO;

namespace BaseLib
{
	public class TextFile
	{
		private string filename;
		private long offset;
		private long length;
		public TextFile(string path, long offset = 0, long length = long.MaxValue)
		{
			this.filename = path;
			this.offset = offset;
			this.length = length;
			if (!System.IO.File.Exists(this.filename))
			{
				throw new System.IO.FileNotFoundException("not found:" + this.filename);
			}
		}
		public Vector Analyze(Vector vec = null)
		{
			if (vec == null)
			{
				vec = new Vector(0);
			}

			byte[] buffer = new byte[1024 * 1024];
			uint[,] vector = vec.Fix();
			long total = 0;
			using (FileStream str = new FileStream(this.filename, FileMode.Open))
			{
				int last = str.ReadByte();
				if (last < 0)
				{
					return vec;
				}
				int size;
				while ((size = str.Read(buffer, 0, 1024 * 1024)) > 0)
				{
					for (int i = 0; i < size; i++)
					{
						total++;
						if(offset < total){
							int now = buffer[i];
							vector[last, now]++;
							last = now;
							if (total >= this.length+this.offset)
							{
								return vec;
							}
						}
					}
				}
			}
			return vec;
		}
	}
}
