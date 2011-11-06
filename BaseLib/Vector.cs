using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Numerics;

namespace BaseLib
{
	public class Vector
	{
		public const int START = 0;
		public const int SIZE = 256;
		private readonly static Vector DEFAULT_MASK = new Vector();
		private uint[,] vector = new uint[SIZE, SIZE];
		private double length = 0;
		private Vector mask;
		private Vector()
		{
			for (int i = 0; i < SIZE; i++)
				for (int j = 0; j < SIZE; j++)
				{
					vector[i, j] = 1;
				}
		}
		public Vector(uint def)
		{
			this.mask = DEFAULT_MASK;
			for (int i = 0; i < SIZE; i++)
				for (int j = 0; j < SIZE; j++)
				{
					vector[i, j] = def;
				}
		}
		public Vector(Vector mask)
		{
			if (mask == null)
			{
				this.mask = DEFAULT_MASK;
			}
			else
			{
				this.mask = mask;
			}
		}
		public Vector(Vector mask, string path) : this(mask)
		{
			using (FileStream fstr = new FileStream(path, FileMode.Open))
			{
				BinaryReader reader = new BinaryReader(fstr);
				for (int i = 0; i < SIZE; i++)
					for (int j = 0; j < SIZE; j++)
				{
					vector[i, j] = reader.ReadUInt32();
				}
			}
		}
		public void Export(string path)
		{
			using (FileStream fstr = new FileStream(path, FileMode.Create))
			{
				BinaryWriter writer = new BinaryWriter(fstr);
				for (int i = 0; i < SIZE; i++)
					for (int j = 0; j < SIZE; j++)
					{
						writer.Write(vector[i, j]);
					}
			}
		}
		public static Vector operator *(Vector a, Vector b)
		{
			Vector mask = null;
			if (a.mask != null && b.mask != null)
			{
				mask = a.mask * b.mask;
			}
			else if(a.mask == null && b.mask != null){
				mask = b.mask;
			}
			else
			{
				mask = DEFAULT_MASK;
			}
			Vector c = new Vector(mask);
			for (int i = 0; i < SIZE; i++)
				for (int j = 0; j < SIZE; j++)
				{
					c.vector[i, j] = a.vector[i, j] * b.vector[i, j] * mask.vector[i,j];
				}
			return c;
		}
		public uint[,] Fix()
		{
			length = 0;
			return vector;
		}
		public uint[,] Read()
		{
			return vector;
		}
		private double GetLength()
		{
			if (length <= 0)
			{
				ulong sum = 0;
				for (int i = START; i < SIZE; i++)
					for (int j = START; j < SIZE; j++)
					{
						/*
						if (i < 128 || j < 128)
						{
							continue;
						}*/
						ulong dval = (ulong)vector[i, j] * mask.vector[i,j];
						sum += dval * dval;
					}
				length = Math.Sqrt(sum);
			}
			return length;
		}
		public double CalcDistance(Vector target)
		{
			Vector mask = this.mask * target.mask;
			ulong sum = 0;
			for (int i = START; i < SIZE; i++)
				for (int j = START; j < SIZE; j++)
			{
				/*
				if(i < 128 || j < 128)
				{
					continue;
				}*/
				sum += (ulong)target.vector[i, j] * vector[i, j] * mask.vector[i,j];
			}
			return sum / target.GetLength() / GetLength();
		}
	}
}
