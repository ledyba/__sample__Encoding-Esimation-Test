using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;
using System.IO;
using System.Drawing;

namespace Visualizer
{
	public class ImageVisualizer
	{
		private static Color CreateColorFromHSV(float h, float s, float v)
		{
			if (s == 0.0f)
			{
				return Color.Black;
			}
			h = (h % 360 + 360) % 360;
			int range = (int)h / 60;
			float f = h / 60 - range;
			float p = v * (1 - s);
			float q = v * (1 - f * s);
			float t = v * (1 - (1 - f) * s);
			switch (range)
			{
				case 0:
					return Color.FromArgb((int)(v * 255), (int)(t * 255), (int)(p * 255));
				case 1:
					return Color.FromArgb((int)(q * 255), (int)(v * 255), (int)(p * 255));
				case 2:
					return Color.FromArgb((int)(p * 255), (int)(v * 255), (int)(t * 255));
				case 3:
					return Color.FromArgb((int)(p * 255), (int)(q * 255), (int)(v * 255));
				case 4:
					return Color.FromArgb((int)(t * 255), (int)(p * 255), (int)(v * 255));
				case 5:
					return Color.FromArgb((int)(v * 255), (int)(p * 255), (int)(q * 255));
				default:
					throw new Exception("Invalid range:"+h);
			}
		}
		public static void Draw(Vector vec, string filename)
		{
			Bitmap bmp = new Bitmap(256, 256);
			uint max = 1;
			uint[,] vector = vec.Read();
			foreach (uint count in vector)
			{
				max = Math.Max(max, count);
			}
			for (int x = 0; x < 256; x++)
				for (int y = 0; y < 256; y++)
				{
					double param = Math.Log(vector[x, y]+1, max+1);
					bmp.SetPixel(x, y, CreateColorFromHSV((float)(330.0f * param) + 240, 1, 1));
				}
			bmp.Save(filename);
		}
	}
	class EntryPoint
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Vector Visualizer");
			Console.WriteLine("[usage] ./Visualizer.exe <files...> <output image> or");
			Console.WriteLine("./Visualizer.exe <NONE>");
			Console.WriteLine("-----");
			if (args.Length > 1)
			{
				List<string> fileList = new List<string>();
				for(int i=0;i<args.Length-1;i++)
				{
					fileList.Add(args[i]);
					Console.WriteLine("Adding..."+args[i]);
				}
				FileSet fileSet = new FileSet(fileList);
				Console.Write("Analyzing...");
				ImageVisualizer.Draw(fileSet.Analyze(), args.Last<string>());
				Console.WriteLine("Analyzed.");
			}
			else
			{
				Dictionary<string, Vector> vectorDic = new Dictionary<string, Vector>();
				Vector mask = null;
				if(File.Exists("mask.vec"))
				{
					Console.Write("Loading[mask]...");
					mask = new Vector(null, "./mask.vec");
					vectorDic.Add("mask", mask);
					Console.WriteLine("Loaded.");
				}

				Console.Write("Loading[ascii]...");
				vectorDic.Add("ascii", new Vector(null, "./ascii.vec"));
				if(mask != null)
				{
					vectorDic.Add("ascii_masked", new Vector(null, "./ascii.vec") * mask);
				}
				Console.WriteLine("Loaded.");

				Console.Write("Loading[euc]...");
				vectorDic.Add("euc", new Vector(null, "./euc.vec"));
				if (mask != null)
				{
					vectorDic.Add("euc_masked", new Vector(null, "./euc.vec") * mask);
				}
				Console.WriteLine("Loaded.");

				Console.Write("Loading[jis]...");
				vectorDic.Add("jis", new Vector(null, "./jis.vec"));
				if (mask != null)
				{
					vectorDic.Add("jis_masked", new Vector(null, "./jis.vec") * mask);
				}
				Console.WriteLine("Loaded.");

				Console.Write("Loading[sjis]...");
				vectorDic.Add("sjis", new Vector(null, "./sjis.vec"));
				if (mask != null)
				{
					vectorDic.Add("sjis_masked", new Vector(null, "./sjis.vec") * mask);
				}
				Console.WriteLine("Loaded.");

				Console.Write("Loading[utf8]...");
				vectorDic.Add("utf8", new Vector(null, "./utf8.vec"));
				if (mask != null)
				{
					vectorDic.Add("utf8_masked", new Vector(null, "./utf8.vec") * mask);
				}
				Console.WriteLine("Loaded.");
				//Visualize
				foreach (KeyValuePair<string, Vector> pair in vectorDic)
				{
					ImageVisualizer.Draw(pair.Value, pair.Key + ".png");
				}
			}
		}
	}
}
