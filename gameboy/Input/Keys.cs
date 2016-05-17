using System;
using System.Runtime.InteropServices;

namespace GameBoy.Input
{
	[StructLayout(LayoutKind.Explicit, Pack=1)]
	public class Keys
	{
		[FieldOffset(0)]
		public byte A = 1;
		[FieldOffset(1)]
		public byte B = 1;
		[FieldOffset(2)]
		public byte Select = 1;
		[FieldOffset(3)]
		public byte Start = 1;
		[FieldOffset(0)]
		public byte Faces = 4;

		[FieldOffset(4)]
		public byte Right = 1;
		[FieldOffset(5)]
		public byte Left = 1;
		[FieldOffset(6)]
		public byte Up = 1;
		[FieldOffset(7)]
		public byte Down = 1;
		[FieldOffset(4)]
		public byte Directions = 4;

		[FieldOffset(0)]
		public byte C;
	}
}

