using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;

namespace CharacterRatio
{
	class CharacterRatio
	{
		static void analyze(string name, Vector vec)
		{
			ulong ascii = 0;
			ulong non_ascii = 0;
			uint[,] ivec = vec.Read();
			for (int i = 0; i < BaseLib.Vector.SIZE / 2; i++)
			{
				for (int j = 0; j < BaseLib.Vector.SIZE;j++ )
				{
					ascii += ivec[i, j];
				}
			}
			for (int i = BaseLib.Vector.SIZE / 2; i < BaseLib.Vector.SIZE; i++)
			{
				for (int j = 0; j < BaseLib.Vector.SIZE; j++)
				{
					non_ascii += ivec[i, j];
				}
			}
			ulong sum = ascii + non_ascii;
			Console.WriteLine("["+name+"]\nASCII: "+ascii+"("+(ascii * 100 / sum)+"%) / Non-ASCII:"+non_ascii+"("+(non_ascii * 100 / sum)+"%) total:"+sum);
		}
		static void Main(string[] args)
		{
			if (args.Length > 0)
			{
				foreach (string str in args)
				{
					analyze(str, new FileSet(str).Analyze());
				}
			}
			else
			{
				analyze("ascii", new Vector(null, "./ascii.vec"));
				analyze("euc", new Vector(null, "./euc.vec"));
				analyze("jis", new Vector(null, "./jis.vec"));
				analyze("sjis", new Vector(null, "./sjis.vec"));
				analyze("utf8", new Vector(null, "./utf8.vec"));
			}
		}
	}
}
