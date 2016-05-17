using System;

namespace gameboy
{
	public class Memory
	{
		public Register Registers { get; set; }

		public Rom Rom { get; set; }

		public Memory (Rom rom) {
			Rom = rom;
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
			throw new NotImplementedException ("ReadByte not yet implemented");
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
			throw new NotImplementedException ("WriteByte not yet implemented");
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

