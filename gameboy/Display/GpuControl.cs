using System;

namespace GameBoy.Graphics
{
	public enum GpuControl : byte
	{
		BgEnable = (1 << 0),
		SpriteEnable = (1 << 1),
		SpriteVDouble = (1 << 2),
		Tilemap = (1 << 3),
		Tileset = (1 << 4),
		WindowEnable = (1 << 5),
		WindowTilemap = (1 << 6),
		DisplayEnable = (1 << 7)
	}
}

