using System;
using System.Collections.Generic;

namespace GameBoy.Graphics
{
	public struct Color
	{
		public byte R { get; set; }

		public byte G { get; set; }

		public byte B { get; set; }

		public byte A { get; set; }

		public Color (byte r, byte g, byte b) : this (r, g, b, 255)
		{
		}

		public Color (byte r, byte g, byte b, byte a) : this()
		{
			A = r;
			G = g;
			B = b;
			A = a;
		}
	}
}

