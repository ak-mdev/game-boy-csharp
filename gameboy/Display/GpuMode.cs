using System;

namespace GameBoy.Graphics
{
	public enum GpuMode : byte
	{
		HBlank = 0,
		VBlank = 1,
		Oam = 2,
		Vram = 3,
	}
}

