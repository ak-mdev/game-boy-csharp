using System;
using GameBoy.CPU;

namespace GameBoy.Graphics
{
	public class Gpu
	{
		public Display Display { get; set; }

		public byte[] Vram { get; set; }

		public byte Control { get; set; }

		public byte ScrollX { get; set; }

		public byte ScrollY { get; set; }

		public byte Scanline { get; set; }

		public ulong Ticks { get; set; }

		public byte[][][] Tiles { get; set; }

		public Color[] BackgroundPalette { get; set; }

		public Color[][] SpritePalette { get; set; }

		public Gpu ()
		{
			Vram = new byte[0x2000];
			Tiles = new byte[384] [8] [8];
			BackgroundPalette = new Color[4];
			SpritePalette = new Color[2] [4];
		}


		public void Reset ()
		{
			Array.Clear (Vram, 0, Vram.Length);

			Array.Clear (Tiles, 0, Tiles.Length);
			BackgroundPalette [0] = Display.Palette [0];
			BackgroundPalette [1] = Display.Palette [1];
			BackgroundPalette [2] = Display.Palette [2];
			BackgroundPalette [3] = Display.Palette [3];

			SpritePalette [0] [0] = Display.Palette [0];
			SpritePalette [0] [1] = Display.Palette [1];
			SpritePalette [0] [2] = Display.Palette [2];
			SpritePalette [0] [3] = Display.Palette [3];

			SpritePalette [1] [0] = Display.Palette [0];
			SpritePalette [1] [1] = Display.Palette [1];
			SpritePalette [1] [2] = Display.Palette [2];
			SpritePalette [1] [3] = Display.Palette [3];

			Control = 0;
			ScrollX = 0;
			ScrollY = 0;
			Scanline = 0;
			Ticks = 0;
		}

		static GpuMode gpuMode = GpuMode.HBlank;
		static ulong lastTicks = 0;

		public void Step (Cpu cpu)
		{
			Ticks += cpu.Ticks - lastTicks;
			lastTicks = cpu.Ticks;

			switch (gpuMode) {
			case GpuMode.HBlank:
				if (Ticks >= 204) {
					HBlank ();
					if (Scanline == 143) {
						if ((cpu.Interrupt.Enable & (byte)Interrupts.VBlank) != 0) {
							cpu.Interrupt.Flags |= (byte)Interrupts.VBlank;
						}
						gpuMode = GpuMode.VBlank;
					} else {
						gpuMode = GpuMode.Oam;
					}
					Ticks -= 204;
				}
				break;
			case GpuMode.VBlank:
				if (Ticks >= 456) {
					Scanline++;
					if (Scanline > 153) {
						Scanline = 0;
						gpuMode = GpuMode.Oam;
					}
					Ticks -= 456;
				}
				break;
			case GpuMode.Oam:
				if (Ticks >= 80) {
					gpuMode = GpuMode.Vram;
					Ticks -= 80;
				}
				break;
			case GpuMode.Vram:
				if (Ticks >= 172) {
					gpuMode = GpuMode.HBlank;
					Ticks -= 172;
				}
				break;
			}

		}

		void HBlank ()
		{
			Scanline++;
		}

		void UpdateTile (ushort address, byte value)
		{
			address &= 0x1ffe;

			var tile = (ushort)((address >> 4) & 511);
			var y = (ushort)((address >> 1) & 7);

			for (byte x = 0; x < 8; x++) {
				byte bitIndex = (byte)(1 << (7 - x));

				//((unsigned char (*)[8][8])tiles)[tile][y][x] = ((vram[address] & bitIndex) ? 1 : 0) + ((vram[address + 1] & bitIndex) ? 2 : 0);
				Tiles [tile] [y] [x] = (byte)((((Vram [address] & bitIndex) != 0) ? 1 : 0) + (((Vram [address + 1] & bitIndex) != 0) ? 2 : 0));
			}
		}
	}
}

