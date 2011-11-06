using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BaseLib;

namespace Dumper
{
	class EntryPoint
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Vector Dumper");
			Console.WriteLine("usage: Dumper [input dir/file]... [outfile]");
			Console.WriteLine("----");
			if (args.Length < 2)
			{
				Dictionary<string, Vector> vectorDic = new Dictionary<string, Vector>();

				Console.Write("Analyzing[ascii]...");
				vectorDic.Add("ascii", new FileSet("./docs_ascii").Analyze());
				Console.WriteLine("Analyzed.");
				
				Console.Write("Analyzing[euc]...");
				vectorDic.Add("euc", new FileSet("./docs_euc").Analyze());
				Console.WriteLine("Analyzed.");
				
				Console.Write("Analyzing[jis]...");
				vectorDic.Add("jis", new FileSet("./docs_jis").Analyze());
				Console.WriteLine("Analyzed.");
				
				Console.Write("Analyzing[sjis]...");
				vectorDic.Add("sjis", new FileSet("./docs_sjis").Analyze());
				Console.WriteLine("Analyzed.");

				Console.Write("Analyzing[utf8]...");
				vectorDic.Add("utf8", new FileSet("./docs_utf8").Analyze());
				Console.WriteLine("Analyzed.");
				//Visualize
				foreach (KeyValuePair<string, Vector> pair in vectorDic)
				{
					pair.Value.Export(pair.Key+".vec");
				}
			}
			else
			{
				List<string> dumpList = new List<string>();
				for (int i = 0; i < args.Length - 1; i++)
				{
					dumpList.Add(args[i]);
					Console.WriteLine("Adding..." + args[i]);
				}
				string dumpName = args[1];
				FileSet set = new FileSet(dumpList);
				Console.Write("Analyzing...");
				set.Analyze().Export(dumpName);
				Console.WriteLine("Analyzed.");
			}
		}
	}
}
