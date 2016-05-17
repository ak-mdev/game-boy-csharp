using System;

namespace gameboy
{
	public delegate void Handler ();
	public delegate void ByteHandler (byte value);
	public delegate void UShortHandler (ushort value);

	public class Instruction
	{
		public string Disassembly { get; set; }

		public byte Cycles { get; private set; }

		public byte Ticks { get; private set; }

		public byte Operands { get; set; }

		public Handler Handler0 { get; set; }

		public ByteHandler Handler1 { get; set; }

		public UShortHandler Handler2 { get; set; }

		#region Constructors

		public Instruction (string disassembly, byte ticks) : this (disassembly, ticks, 0, NOP)
		{
		}

		public Instruction (string disassembly, byte ticks, Handler handler) : this (disassembly, ticks, 0, handler)
		{
		}

		public Instruction (string disassembly, byte ticks, ByteHandler handler) : this (disassembly, ticks, 1, handler)
		{
		}

		public Instruction (string disassembly, byte ticks, UShortHandler handler) : this (disassembly, ticks, 2, handler)
		{
		}

		private Instruction (string disassembly, byte ticks, byte operands, Handler handler)
		{
			Disassembly = disassembly;
			Ticks = ticks;
			Cycles = (byte)(ticks << 1);
			Operands = operands;
			Handler0 = handler;
		}

		private Instruction (string disassembly, byte ticks, byte operands, ByteHandler handler)
		{
			Disassembly = disassembly;
			Ticks = ticks;
			Cycles = (byte)(ticks << 1);
			Operands = operands;
			Handler1 = handler;
		}

		private Instruction (string disassembly, byte ticks, byte operands, UShortHandler handler)
		{
			Disassembly = disassembly;
			Ticks = ticks;
			Cycles = (byte)(ticks << 1);
			Operands = operands;
			Handler2 = handler;
		}

		#endregion

		#region Base Instructions

		public static void Undefined ()
		{
			Console.WriteLine ("Command not yet implemented");
		}

		public static void NOP ()
		{
			// Complete
		}

		public static void Add (ref byte register, byte value, Memory memory)
		{
			var result = (byte)(register + value);
			if ((result & 0xffff0000) != 0)
				memory.SetFlag (Flag.Carry);
			else
				memory.ClearFlag (Flag.Carry);

			register = (byte)(result & 0xffff);

			if (((register & 0x0f) + (value & 0x0f)) > 0x0f)
				memory.SetFlag (Flag.HalfCarry);
			else
				memory.ClearFlag (Flag.HalfCarry);
			
			memory.ClearFlag (Flag.Negative);
		}

		public static void Add (ref ushort register, ushort value, Memory memory)
		{
			var result = (ushort)(register + value);
			if ((result & 0xffff0000) != 0)
				memory.SetFlag (Flag.Carry);
			else
				memory.ClearFlag (Flag.Carry);

			register = (ushort)(result & 0xffff);

			if (((register & 0x0f) + (value & 0x0f)) > 0x0f)
				memory.SetFlag (Flag.HalfCarry);
			else
				memory.ClearFlag (Flag.HalfCarry);

			memory.ClearFlag (Flag.Negative);
		}

		public static void AddWithCarry (byte value, Memory memory)
		{
			value += (byte)(memory.IsFlagSet (Flag.Carry) ? 1 : 0);

			var result = (byte)(memory.Registers.A + value);

			if ((result & 0xff00) != 0)
				memory.SetFlag (Flag.Carry);
			else
				memory.ClearFlag (Flag.Carry);

			if (value == memory.Registers.A)
				memory.SetFlag (Flag.Zero);
			else
				memory.ClearFlag (Flag.Zero);

			if (((value & 0x0f) + (memory.Registers.A & 0x0f)) > 0x0f)
				memory.SetFlag (Flag.HalfCarry);
			else
				memory.ClearFlag (Flag.HalfCarry);

			memory.SetFlag (Flag.Negative);

			memory.Registers.A = (byte)(result & 0xff);
		}

		public static void Subtract (byte value, Memory memory)
		{
			memory.SetFlag (Flag.Negative);

			if (value > memory.Registers.A)
				memory.SetFlag (Flag.Carry);
			else
				memory.ClearFlag (Flag.Carry);

			if (value == memory.Registers.A)
				memory.SetFlag (Flag.Zero);
			else
				memory.ClearFlag (Flag.Zero);

			if ((value & 0x0f) > (memory.Registers.A & 0x0f))
				memory.SetFlag (Flag.HalfCarry);
			else
				memory.ClearFlag (Flag.HalfCarry);

			memory.Registers.A -= value;
		}

		public static void SubtractWithCarry (byte value, Memory memory)
		{
			value += (byte)(memory.IsFlagSet (Flag.Carry) ? 1 : 0);

			memory.SetFlag (Flag.Negative);

			if (value > memory.Registers.A)
				memory.SetFlag (Flag.Carry);
			else
				memory.ClearFlag (Flag.Carry);

			if (value == memory.Registers.A)
				memory.SetFlag (Flag.Zero);
			else
				memory.ClearFlag (Flag.Zero);
			
			if ((value & 0x0f) > (memory.Registers.A & 0x0f))
				memory.SetFlag (Flag.HalfCarry);
			else
				memory.ClearFlag (Flag.HalfCarry);
			
			memory.Registers.A -= value;
		}

		public static void And (byte value, Memory memory)
		{
			memory.Registers.A &= value;

			if (memory.Registers.A != 0)
				memory.ClearFlag (Flag.Zero);
			else
				memory.SetFlag (Flag.Zero);

			memory.ClearFlag (Flag.Carry | Flag.Negative);
			memory.SetFlag (Flag.HalfCarry);
		}

		public static void Or (byte value, Memory memory)
		{
			memory.Registers.A |= value;

			if (memory.Registers.A != 0)
				memory.ClearFlag (Flag.Zero);
			else
				memory.SetFlag (Flag.Zero);

			memory.ClearFlag (Flag.Carry | Flag.Negative | Flag.HalfCarry);
		}

		public static void ExclusiveOr (byte value, Memory memory)
		{
			memory.Registers.A ^= value;

			if (memory.Registers.A != 0)
				memory.ClearFlag (Flag.Zero);
			else
				memory.SetFlag (Flag.Zero);

			memory.ClearFlag (Flag.Carry | Flag.Negative | Flag.HalfCarry);
		}

		public static void Compare (byte value, Memory memory)
		{
			if (memory.Registers.A == value)
				memory.SetFlag (Flag.Zero);
			else
				memory.ClearFlag (Flag.Zero);

			if (value > memory.Registers.A)
				memory.SetFlag (Flag.Carry);
			else
				memory.ClearFlag (Flag.Carry);

			if ((value & 0x0f) > (memory.Registers.A & 0x0f))
				memory.SetFlag (Flag.HalfCarry);
			else
				memory.ClearFlag (Flag.HalfCarry);

			memory.SetFlag (Flag.Negative);
		}

		public static byte Increment (byte value, Memory memory)
		{
			if ((value & 0x0f) == 0x0f)
				memory.SetFlag (Flag.HalfCarry);
			else
				memory.ClearFlag (Flag.HalfCarry);

			value++;

			if (value != 0)
				memory.ClearFlag (Flag.Zero);
			else
				memory.SetFlag (Flag.Zero);
			
			memory.SetFlag (Flag.Negative);

			return value;
		}

		public static byte Decrement (byte value, Memory memory)
		{
			if ((value & 0x0f) != 0)
				memory.ClearFlag (Flag.HalfCarry);
			else
				memory.SetFlag (Flag.HalfCarry);

			value--;

			if (value != 0)
				memory.ClearFlag (Flag.Zero);
			else
				memory.SetFlag (Flag.Zero);

			memory.SetFlag (Flag.Negative);

			return value;
		}

		#endregion

	}
}

