using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;
using System.Numerics;

namespace ExtractFeatureVector
{
	class ExtractFeatureVector
	{
		const int RANK_CNT = 100;
		static void Main(string[] args)
		{
			Dictionary<string, Vector> vectorDic = new Dictionary<string, Vector>();
			Console.Write("loading document sample vectors...");
			Vector ascii = new Vector(null, "./ascii.vec");
			uint[,] asciiVec = ascii.Read();
			vectorDic.Add("ascii", ascii);
			vectorDic.Add("euc", new Vector(null, "./euc.vec"));
			vectorDic.Add("jis", new Vector(null, "./jis.vec"));
			vectorDic.Add("sjis", new Vector(null, "./sjis.vec"));
			vectorDic.Add("utf8", new Vector(null, "./utf8.vec"));
			Console.WriteLine("loaded.");

			Vector sum = new Vector(0);

			double[,] sumVec = new double[Vector.SIZE, Vector.SIZE];
			double max = 0;

			int vecCnt = vectorDic.Count;
			int[,] emptyCount = new int[Vector.SIZE, Vector.SIZE];

			for (int i = 0; i < Vector.SIZE; i++)
				for (int j = 0; j < Vector.SIZE; j++)
				{
					emptyCount[i, j] = 0;
					sumVec[i, j] = 1;
				}

			foreach (KeyValuePair<string, Vector> pair in vectorDic)
			{
				string key = pair.Key;
				Vector vec = pair.Value;
				uint[,] ivec = vec.Read();
				for (int i = 0; i < Vector.SIZE; i++)
					for (int j = 0; j < Vector.SIZE; j++)
					{
						if (ivec[i, j] == 0)
						{
							emptyCount[i, j]++;
						}
						sumVec[i, j] *= ivec[i, j];
					}
			}

			for (int i = 0; i < Vector.SIZE; i++)
				for (int j = 0; j < Vector.SIZE; j++)
				{
					if (sumVec[i, j] == 0)
					{
						continue;
					}
					double log = Math.Log((double)sumVec[i, j]);
					log /= vecCnt;
					sumVec[i, j] = Math.Pow(Math.E, log);
					max = sumVec[i, j] < max ? max : sumVec[i, j];
				}

			uint usedCount = 0;
			uint[,] isumVec = sum.Fix();
			for (int i = 0; i < Vector.SIZE; i++)
				for (int j = 0; j < Vector.SIZE; j++)
				{
					if (
						asciiVec[i,j] != 0 //ASCIIで使われている
						|| emptyCount[i, j] == vecCnt //みんな空
						//|| emptyCount[i, j] <= 2 
						//|| sumVec[i, j] > 0.0f
						)
					{
						isumVec[i, j] = 0;
					}
					else
					{
						isumVec[i, j] = 1;
						usedCount++;
					}
				}
			sum.Export("mask.vec");
			Console.WriteLine("Used elements: " + usedCount+"("+(usedCount * 100 / (Vector.SIZE*Vector.SIZE))+"%)");
		}
	}
}
