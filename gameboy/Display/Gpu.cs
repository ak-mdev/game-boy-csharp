using System;
using GameBoy.CPU;

namespace GameBoy.Graphics
{
	public class Gpu
	{
		private Interrupt interrupt;

		public Display Display { get; set; }

		public ulong Ticks { get; set; }

		public Gpu (Interrupt interrupt)
		{
			this.interrupt = interrupt;
			Display = new Display ();
		}

		public void Reset ()
		{
			Display.Reset ();
			Ticks = 0;
		}

		static GpuMode gpuMode = GpuMode.HBlank;
		static ulong lastTicks = 0;

		public void Step (ulong ticks)
		{
			Ticks += ticks - lastTicks;
			lastTicks = ticks;

			switch (gpuMode) {
			case GpuMode.HBlank:
				if (Ticks >= 204) {
					HBlank ();
					if (Display.Scanline == 143) {
						if ((interrupt.Enable & (byte)Interrupts.VBlank) != 0) {
							interrupt.Flags |= (byte)Interrupts.VBlank;
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
					Display.Scanline++;
					if (Display.Scanline > 153) {
						Display.Scanline = 0;
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

		public void HBlank ()
		{
			Display.Scanline++;
		}

		public void UpdateTile (ushort address, byte value, byte[] vram)
		{
			address &= 0x1ffe;

			var tile = (ushort)((address >> 4) & 511);
			var y = (ushort)((address >> 1) & 7);

			for (byte x = 0; x < 8; x++) {
				byte bitIndex = (byte)(1 << (7 - x));

				//((unsigned char (*)[8][8])tiles)[tile][y][x] = ((vram[address] & bitIndex) ? 1 : 0) + ((vram[address + 1] & bitIndex) ? 2 : 0);
				Display.Tiles [tile, y, x] = (byte)((((vram [address] & bitIndex) != 0) ? 1 : 0) + (((vram [address + 1] & bitIndex) != 0) ? 2 : 0));
			}
		}
	}
}

