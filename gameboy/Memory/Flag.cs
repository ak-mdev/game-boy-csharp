using System;

namespace gameboy
{
	public enum Flag : byte
	{
		Zero = 1 << 7,
		Negative = 1 << 6,
		HalfCarry = 1 << 5,
		Carry = 1 << 4
	}
}

