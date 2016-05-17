using System;
using System.Runtime.InteropServices;

namespace GameBoy.Memory
{
	[StructLayout(LayoutKind.Explicit, Pack=1)]
	public class Register
	{
		[FieldOffset(0)]
		public byte F;

		[FieldOffset(1)]
		public byte A;

		[FieldOffset(0)]
		public ushort AF;

		[FieldOffset(2)]
		public byte C;

		[FieldOffset(3)]
		public byte B;

		[FieldOffset(2)]
		public ushort BC;

		[FieldOffset(4)]
		public byte E;

		[FieldOffset(5)]
		public byte D;

		[FieldOffset(4)]
		public ushort DE;

		[FieldOffset(6)]
		public byte L;

		[FieldOffset(7)]
		public byte H;

		[FieldOffset(6)]
		public ushort HL;

		[FieldOffset(8)]
		public ushort SP;

		[FieldOffset(9)]
		public ushort PC;
	}
}

