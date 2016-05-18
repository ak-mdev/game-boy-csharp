using System;
using GameBoy.IO;
using GameBoy.Input;
using GameBoy.Graphics;
using GameBoy.CPU;

namespace GameBoy.Memory
{
	public class Ram
	{
		private Random random = new Random ();
		private Gpu gpu;
		private InputHandler input;
		private Interrupt interrupt;

		private readonly byte[] IO_RESET = new byte[0x100] {
			0x0F, 0x00, 0x7C, 0xFF, 0x00, 0x00, 0x00, 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01,
			0x80, 0xBF, 0xF3, 0xFF, 0xBF, 0xFF, 0x3F, 0x00, 0xFF, 0xBF, 0x7F, 0xFF, 0x9F, 0xFF, 0xBF, 0xFF,
			0xFF, 0x00, 0x00, 0xBF, 0x77, 0xF3, 0xF1, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0xFF,
			0x91, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFC, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x7E, 0xFF, 0xFE,
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x3E, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xC0, 0xFF, 0xC1, 0x00, 0xFE, 0xFF, 0xFF, 0xFF,
			0xF8, 0xFF, 0x00, 0x00, 0x00, 0x8F, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xCE, 0xED, 0x66, 0x66, 0xCC, 0x0D, 0x00, 0x0B, 0x03, 0x73, 0x00, 0x83, 0x00, 0x0C, 0x00, 0x0D,
			0x00, 0x08, 0x11, 0x1F, 0x88, 0x89, 0x00, 0x0E, 0xDC, 0xCC, 0x6E, 0xE6, 0xDD, 0xDD, 0xD9, 0x99,
			0xBB, 0xBB, 0x67, 0x63, 0x6E, 0x0E, 0xEC, 0xCC, 0xDD, 0xDC, 0x99, 0x9F, 0xBB, 0xB9, 0x33, 0x3E,
			0x45, 0xEC, 0x52, 0xFA, 0x08, 0xB7, 0x07, 0x5D, 0x01, 0xFD, 0xC0, 0xFF, 0x08, 0xFC, 0x00, 0xE5,
			0x0B, 0xF8, 0xC2, 0xCE, 0xF4, 0xF9, 0x0F, 0x7F, 0x45, 0x6D, 0x3D, 0xFE, 0x46, 0x97, 0x33, 0x5E,
			0x08, 0xEF, 0xF1, 0xFF, 0x86, 0x83, 0x24, 0x74, 0x12, 0xFC, 0x00, 0x9F, 0xB4, 0xB7, 0x06, 0xD5,
			0xD0, 0x7A, 0x00, 0x9E, 0x04, 0x5F, 0x41, 0x2F, 0x1D, 0x77, 0x36, 0x75, 0x81, 0xAA, 0x70, 0x3A,
			0x98, 0xD1, 0x71, 0x02, 0x4D, 0x01, 0xC1, 0xFF, 0x0D, 0x00, 0xD3, 0x05, 0xF9, 0x00, 0x0B, 0x00
		};

		public Register Registers { get; set; }

		public Rom Rom { get; set; }

		public byte[] Sram { get; set; }

		public byte[] Io { get; set; }

		public byte[] Oam { get; set; }

		public byte[] Vram { get; set; }

		public byte[] Wram { get; set; }

		public byte[] Hram { get; set; }

		public Ram (Rom rom, Gpu gpu, InputHandler input, Interrupt interrupt)
		{
			Rom = rom;
			Registers = new Register ();
			Sram = new byte[0x2000];
			Io = new byte[0x100];
			Vram = new byte[0x2000];
			Oam = new byte[0x100];
			Wram = new byte[0x2000];
			Hram = new byte[0x80];

			this.gpu = gpu;
			this.input = input;
			this.interrupt = interrupt;
		}

		public void Reset ()
		{
			#region Reset buffers
			Array.Clear (Sram, 0, Sram.Length);
			Array.Copy (IO_RESET, Io, IO_RESET.Length);
			Array.Clear (Oam, 0, Oam.Length);
			Array.Clear (Vram, 0, Vram.Length);
			Array.Clear (Wram, 0, Wram.Length);
			Array.Clear (Hram, 0, Hram.Length);
			#endregion

			#region Reset registers
			Registers.A = 0x01;
			Registers.F = 0xb0;
			Registers.B = 0x00;
			Registers.C = 0x13;
			Registers.D = 0x00;
			Registers.E = 0xd8;
			Registers.H = 0x01;
			Registers.L = 0x4d;
			Registers.SP = 0xfffe;
			Registers.PC = 0x100;
			#endregion

			#region Reset constants
			WriteByte (0xFF05, 0);
			WriteByte (0xFF06, 0);
			WriteByte (0xFF07, 0);
			WriteByte (0xFF10, 0x80);
			WriteByte (0xFF11, 0xBF);
			WriteByte (0xFF12, 0xF3);
			WriteByte (0xFF14, 0xBF);
			WriteByte (0xFF16, 0x3F);
			WriteByte (0xFF17, 0x00);
			WriteByte (0xFF19, 0xBF);
			WriteByte (0xFF1A, 0x7A);
			WriteByte (0xFF1B, 0xFF);
			WriteByte (0xFF1C, 0x9F);
			WriteByte (0xFF1E, 0xBF);
			WriteByte (0xFF20, 0xFF);
			WriteByte (0xFF21, 0x00);
			WriteByte (0xFF22, 0x00);
			WriteByte (0xFF23, 0xBF);
			WriteByte (0xFF24, 0x77);
			WriteByte (0xFF25, 0xF3);
			WriteByte (0xFF26, 0xF1);
			WriteByte (0xFF40, 0x91);
			WriteByte (0xFF42, 0x00);
			WriteByte (0xFF43, 0x00);
			WriteByte (0xFF45, 0x00);
			WriteByte (0xFF47, 0xFC);
			WriteByte (0xFF48, 0xFF);
			WriteByte (0xFF49, 0xFF);
			WriteByte (0xFF4A, 0x00);
			WriteByte (0xFF4B, 0x00);
			WriteByte (0xFFFF, 0x00);
			#endregion
		}

		public bool IsFlagSet (Flag flag)
		{
			return ((Registers.F & (byte)flag) != 0);
		}

		public void SetFlag (Flag flag)
		{
			Registers.F |= (byte)flag;
		}

		public void ClearFlag (Flag flag)
		{
			Registers.F &= (byte)(~((byte)flag));
		}

		public void Copy (ushort destination, ushort source, ushort length)
		{
			for (uint i = 0; i < length; i++) {
				var sourceValue = ReadByte ((ushort)(source + i));
				WriteByte ((ushort)(destination + i), sourceValue);
			}
		}

		public byte ReadByte (ushort address)
		{
			if (address <= 0x7fff) {
				return Rom.Cart [address];
			} else if (address >= 0xa000 && address <= 0xbfff) {
				return Sram [address - 0xa000];
			} else if (address >= 0x8000 && address <= 0x9fff) {
				return Vram [address - 0x8000];
			} else if (address >= 0xc000 && address <= 0xdfff) {
				return Wram [address - 0xc000];
			} else if (address >= 0xe000 && address <= 0xfdff) {
				return Wram [address - 0xe000];
			} else if (address >= 0xfe00 && address <= 0xfeff) {
				return Oam [address - 0xfe00];
			} else if (address == 0xff04) {
				return (byte)random.Next ();
			} else if (address == 0xff40) {
				return gpu.Display.Control;
			} else if (address == 0xff42) {
				return gpu.Display.ScrollY;
			} else if (address == 0xff43) {
				return gpu.Display.ScrollX;
			} else if (address == 0xff44) {
				return gpu.Display.Scanline;
			} else if (address == 0xff00) {
				if ((Io [0x00] & 0x20) == 0) {
					return (byte)(0xc0 | input.Keys.Faces | 0x10);
				} else if ((Io [0x00] & 0x10) == 0) {
					return (byte)(0xc0 | input.Keys.Directions | 0x20);
				} else if ((Io [0x00] & 0x30) == 0) {
					return 0xff;
				} else {
					return 0;
				}
			} else if (address == 0xff0f) {
				return interrupt.Flags;
			} else if (address == 0xffff) {
				return interrupt.Enable;
			} else if (address >= 0xff80 && address <= 0xfffe) {
				return Hram [address - 0xff80];
			} else if (address >= 0xff00 && address <= 0xff7f) {
				return Io [address - 0xff00];
			}
			return 0;
		}

		public ushort ReadShort (ushort address)
		{
			return (ushort)(ReadByte (address) | (ReadByte ((ushort)(address + 1)) << 8));
		}

		public ushort ReadFromStack ()
		{
			var value = ReadShort (Registers.SP);
			Registers.SP += 2;
			return value;
		}

		public void WriteByte (ushort address, byte value)
		{
			if(address == 0xff00) {
				Debug.Enabled = true;
			}
			if (address >= 0xa000 && address <= 0xbfff) {
				Sram [address - 0xa000] = value;
			} else if (address >= 0x8000 && address <= 0x9fff) {
				Vram [address - 0x8000] = value;
				if (address <= 0x97ff) {
					gpu.UpdateTile (address, value, Vram);
				}
			}

			if (address >= 0xc000 && address <= 0xdfff) {
				Wram [address - 0xc000] = value;
			} else if (address >= 0xe000 && address <= 0xfdff) {
				Wram [address - 0xe000] = value;
			} else if (address >= 0xfe00 && address <= 0xfeff) {
				Oam [address - 0xfe00] = value;
			} else if (address >= 0xff80 && address <= 0xfffe) {
				Hram [address - 0xff80] = value;
			} else if (address == 0xff40) {
				gpu.Display.Control = value;
			} else if (address == 0xff42) {
				gpu.Display.ScrollY = value;
			} else if (address == 0xff43) {
				gpu.Display.ScrollX = value;
			} else if (address == 0xff46) {
				Copy (0xfe00, (ushort)(value << 8), 160); // OAM DMA
			} else if (address == 0xff47) { // write only
				for (int i = 0; i < 4; i++) {
					gpu.Display.BackgroundPalette [i] = gpu.Display.PALETTE [(value >> (i * 2)) & 3];
				}
			} else if (address == 0xff48) { // write only
				for (int i = 0; i < 4; i++) {
					gpu.Display.SpritePalette [0] [i] = gpu.Display.PALETTE [(value >> (i * 2)) & 3];
				}
			} else if (address == 0xff49) { // write only
				for (int i = 0; i < 4; i++) {
					gpu.Display.SpritePalette [1] [i] = gpu.Display.PALETTE [(value >> (i * 2)) & 3];
				}
			} else if (address >= 0xff00 && address <= 0xff7f) {
				Io [address - 0xff00] = value;
			} else if (address == 0xff0f) {
				interrupt.Flags = value;
			} else if (address == 0xffff) {
				interrupt.Enable = value;
			}
		}

		public void WriteShort (ushort address, ushort value)
		{
			WriteByte (address, (byte)(value & 0x00ff));
			WriteByte ((ushort)(address + 1), (byte)((value & 0xff00) >> 8));
		}

		public void WriteToStack (ushort value)
		{
			Registers.SP -= 2;
			WriteShort (Registers.SP, value);
		}
	}
}

