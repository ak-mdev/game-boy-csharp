using System;

namespace GameBoy.Memory
{
	[Flags]
	public enum Flag : byte
	{
		Zero = 0x80,
		Negative = 0x40,
		HalfCarry = 0x20,
		Carry = 0x10,
		All = 0xF0,
		None = 0x00
	}
}