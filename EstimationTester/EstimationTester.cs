using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;
using System.IO;

namespace EstimationTester
{
	class EstimationTester
	{
		const bool VERBOSE = false;
		static void testFile(Dictionary<string, Vector> vectorDic, string trueEncode, string filename, ref int total, ref int succeedCount, long offset=0, long maxLength = long.MaxValue)
		{
			Dictionary<string, double> scoreDic = new Dictionary<string, double>();
			Vector targetVec = (new TextFile(filename, offset, maxLength)).Analyze();
			string name = "ascii";
			double max = 0;

			foreach (KeyValuePair<string, Vector> pair in vectorDic)
			{
				Vector cmpVec = pair.Value;
				double distance = cmpVec.CalcDistance(targetVec);
				if (distance > max)
				{
					name = pair.Key;
					max = distance;
				}
				scoreDic.Add(pair.Key, distance);
			}
			if (name == null)
			{
				return;
			}
			total++;
			if (trueEncode != null)
			{
				if (trueEncode == name)
				{
					succeedCount++;
				}
				else if (VERBOSE)
				{
					Console.WriteLine("Failed: " + filename);
					Console.WriteLine("It might be...: <<" + name + ">> now:" + succeedCount + "/" + total + " (" + (succeedCount * 100 / total) + " %)");
					foreach (KeyValuePair<string, double> pair in scoreDic)
					{
						Console.WriteLine("Distance of " + pair.Key + ":" + pair.Value);
					}
				}

			}
		}
		static void testDir(Dictionary<string, Vector> vectorDic, List<string> filePathList, string trueEncode, ref int total, ref int succeedCount, long offset = 0, long maxLength = long.MaxValue)
		{
			foreach (string path in filePathList)
			{
				if (Directory.Exists(path))
				{
					foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
					{
						testFile(vectorDic, trueEncode, file, ref total, ref succeedCount, offset, maxLength);
					}
				}
				else if (File.Exists(path))
				{
					testFile(vectorDic, trueEncode, path, ref total, ref succeedCount, offset, maxLength);
				}
				else
				{
					Console.WriteLine("Not found: " + path + " ignored.");
				}
			}
		}
		static void Main(string[] args)
		{
			Console.WriteLine("Auto Estimation Tester");
			Console.WriteLine("[usage] ./Detector.exe [-e encoding] [-l offset length step] input-text-file");
			Console.WriteLine("-----");
			string input;
			Dictionary<string, Vector> vectorDic = new Dictionary<string, Vector>();
			List<string> filePathList = new List<string>();
			string trueEncode = null;
			long step = -1;
			long offset = -1;
			long max_length = -1;

			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "-e")
				{
					if (i < args.Length - 1)
					{
						trueEncode = args[i + 1];
						++i;
					}
					else
					{
						Console.WriteLine("Please input format.");
						return;
					}
				}
				else if (args[i] == "-l")
				{
					if (i < args.Length - 3)
					{
						offset = Convert.ToInt64(args[i + 1]);
						max_length = Convert.ToInt64(args[i + 2]);
						step = Convert.ToInt64(args[i + 3]);
						i += 3;
					}
					else
					{
						Console.WriteLine("Please input length.");
						return;
					}
				}
				else
				{
					filePathList.Add(args[i]);
				}
			}
			if (trueEncode == null)
			{
				Console.WriteLine("Please input true encoding set.");
				return;
			}
			Vector mask = null;
			if(File.Exists("./mask.vec")){
				mask = new Vector(null, "mask.vec");
			}
			input = args[0];
			Console.Write("loading document sample vectors...");
			vectorDic.Add("euc", new Vector(mask, "./euc.vec"));
			vectorDic.Add("jis", new Vector(mask, "./jis.vec"));
			vectorDic.Add("sjis", new Vector(mask, "./sjis.vec"));
			vectorDic.Add("utf8", new Vector(mask, "./utf8.vec"));
			Console.WriteLine("loaded.");

			if (max_length <= 0)
			{
				int total = 0;
				int succeedCount = 0;
				testDir(vectorDic, filePathList, trueEncode, ref total, ref succeedCount);
				if (total > 0)
				{
					Console.WriteLine("---------------");
					Console.WriteLine("Estimation Succeeded: " + succeedCount + "/" + total + " (" + (succeedCount * 100 / total) + " %)");
				}
				else
				{
					Console.WriteLine("No files found.");
				}
			}
			else
			{
				int[] total = new int[step];
				int[] succeedCount = new int[step];
				for (int i = 0; i < step; i++)
				{
					long length = max_length * (i + 1) / step;
					total[i] = 0;
					succeedCount[i] = 0;
					testDir(vectorDic, filePathList, trueEncode, ref total[i], ref succeedCount[i], offset, length);
					if (total[i] > 0)
					{
						Console.WriteLine("Stage:"+(i+1)+" / Length: "+length);
						Console.WriteLine("Rate: " + succeedCount[i] + "/" + total[i] + " (" + (succeedCount[i] * 100 / total[i]) + " %)");
					}
					else
					{
						Console.WriteLine("No files found.");
						return;
					}
				}
				StreamWriter outFile = new StreamWriter("./estimate.csv", true);
				if((new FileInfo("./estimate.csv")).Length <= 0){
					for (int i = 0; i < step; i++)
					{
						long length = max_length * (i + 1) / step;
						outFile.Write(Convert.ToString(length) + ",");
					}
					outFile.WriteLine();
				}
				for (int i = 0; i < step; i++)
				{
					outFile.Write("=" + succeedCount[i] + "/" + total[i]+",");
				}
				outFile.WriteLine();
				outFile.Close();
			}
		}
	}
}
