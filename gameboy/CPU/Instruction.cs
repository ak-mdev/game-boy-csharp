using System;
using GameBoy.Memory;

namespace GameBoy.CPU
{
	public class Instruction
	{
		public string Disassembly { get; protected set; }

		public byte Cycles { get; protected set; }

		public byte Ticks { get; protected set; }

		public byte Operands { get; protected set; }

		public Action Handler { get; set; }

		#region Constructors

		public Instruction (string disassembly, byte ticks)
		{
			Disassembly = disassembly;
			Ticks = ticks;
			Cycles = (byte)(ticks << 1);
		}

		public Instruction (string disassembly, byte ticks, Action handler) : this (disassembly, ticks)
		{
			Operands = 0;
			Handler = handler;
		}

		#endregion

		#region Base Instructions

		public static void Undefined (Ram memory)
		{
			memory.Registers.PC--;

			var instruction = memory.ReadByte (memory.Registers.PC);

			Console.WriteLine ("Undefined instruction {0}!\n\nCheck stdout for more details.", instruction.ToString ("X"));

			Program.Quit ();
		}

		public static void NOP ()
		{
			// Complete
		}

		public static void Add (ref byte register, byte value, Ram memory)
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

		public static void Add (ref ushort register, ushort value, Ram memory)
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

		public static void AddWithCarry (byte value, Ram memory)
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

		public static void Subtract (byte value, Ram memory)
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

		public static void SubtractWithCarry (byte value, Ram memory)
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

		public static void And (byte value, Ram memory)
		{
			memory.Registers.A &= value;

			if (memory.Registers.A != 0)
				memory.ClearFlag (Flag.Zero);
			else
				memory.SetFlag (Flag.Zero);

			memory.ClearFlag (Flag.Carry | Flag.Negative);
			memory.SetFlag (Flag.HalfCarry);
		}

		public static void Or (byte value, Ram memory)
		{
			memory.Registers.A |= value;

			if (memory.Registers.A != 0)
				memory.ClearFlag (Flag.Zero);
			else
				memory.SetFlag (Flag.Zero);

			memory.ClearFlag (Flag.Carry | Flag.Negative | Flag.HalfCarry);
		}

		public static void ExclusiveOr (byte value, Ram memory)
		{
			memory.Registers.A ^= value;

			if (memory.Registers.A != 0)
				memory.ClearFlag (Flag.Zero);
			else
				memory.SetFlag (Flag.Zero);

			memory.ClearFlag (Flag.Carry | Flag.Negative | Flag.HalfCarry);
		}

		public static void Compare (byte value, Ram memory)
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

		public static byte Increment (byte value, Ram memory)
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

		public static byte Decrement (byte value, Ram memory)
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

		#region CB instructions

		public static byte Rlc (byte value, Ram ram)
		{
			var carry = (value & 0x80) >> 7;

			if ((value & 0x80) != 0) {
				ram.SetFlag (Flag.Carry);
			} else {
				ram.ClearFlag (Flag.Carry);
			}

			value <<= 1;
			value += (byte)carry;

			if (value != 0) {
				ram.ClearFlag (Flag.Zero);
			} else {
				ram.SetFlag (Flag.Zero);
			}

			ram.ClearFlag (Flag.Negative | Flag.HalfCarry);

			return value;
		}

		public static byte Rrc (byte value, Ram ram)
		{
			var carry = value & 0x01;

			value >>= 1;

			if (carry != 0) {
				ram.SetFlag (Flag.Carry);
				value |= 0x80;
			} else {
				ram.ClearFlag (Flag.Carry);
			}

			if (value != 0) {
				ram.ClearFlag (Flag.Zero);
			} else {
				ram.SetFlag (Flag.Zero);
			}

			ram.ClearFlag (Flag.Negative | Flag.HalfCarry);

			return value;
		}

		public static byte Rl (byte value, Ram ram)
		{
			var carry = ram.IsFlagSet (Flag.Carry) ? 1 : 0;

			if ((value & 0x80) != 0) {
				ram.SetFlag (Flag.Carry);
			} else {
				ram.ClearFlag (Flag.Carry);
			}

			value <<= 1;
			value += (byte)carry;

			if (value != 0) {
				ram.ClearFlag (Flag.Zero);
			} else {
				ram.SetFlag (Flag.Zero);
			}

			ram.ClearFlag (Flag.Negative | Flag.HalfCarry);

			return value;
		}

		public static byte Rr (byte value, Ram ram)
		{
			value >>= 1;
			if (ram.IsFlagSet (Flag.Carry)) {
				value |= 0x80;
			}

			if ((value & 0x01) != 0) {
				ram.SetFlag (Flag.Carry);
			} else {
				ram.ClearFlag (Flag.Carry);
			}

			if (value != 0) {
				ram.ClearFlag (Flag.Zero);
			} else {
				ram.SetFlag (Flag.Zero);
			}

			ram.ClearFlag (Flag.Negative | Flag.HalfCarry);

			return value;
		}

		public static byte Sla (byte value, Ram ram)
		{
			if ((value & 0x80) != 0) {
				ram.SetFlag (Flag.Carry);
			} else {
				ram.ClearFlag (Flag.Carry);
			}

			value <<= 1;

			if (value != 0) {
				ram.ClearFlag (Flag.Zero);
			} else {
				ram.SetFlag (Flag.Zero);
			}

			ram.ClearFlag (Flag.Negative | Flag.HalfCarry);

			return value;
		}

		public static byte Sra (byte value, Ram ram)
		{
			if ((value & 0x01) != 0) {
				ram.SetFlag (Flag.Carry);
			} else {
				ram.ClearFlag (Flag.Carry);
			}

			value = (byte)((value & 0x80) | (value >> 1));

			if (value != 0) {
				ram.ClearFlag (Flag.Zero);
			} else {
				ram.SetFlag (Flag.Zero);
			}

			ram.ClearFlag (Flag.Negative | Flag.HalfCarry);

			return value;
		}

		public static byte Swap (byte value, Ram ram)
		{
			value = (byte)(((value & 0xf) << 4) | ((value & 0xf0) >> 4));

			if (value != 0) {
				ram.ClearFlag (Flag.Zero);
			} else {
				ram.SetFlag (Flag.Zero);
			}

			ram.ClearFlag (Flag.Negative | Flag.HalfCarry | Flag.Carry);

			return value;
		}

		public static byte Srl (byte value, Ram ram)
		{
			if ((value & 0x01) != 0) {
				ram.SetFlag (Flag.Carry);
			} else {
				ram.ClearFlag (Flag.Carry);
			}

			value >>= 1;

			if (value != 0) {
				ram.ClearFlag (Flag.Zero);
			} else {
				ram.SetFlag (Flag.Zero);
			}

			ram.ClearFlag (Flag.Negative | Flag.HalfCarry);

			return value;
		}

		public static void Bit (byte bit, byte value, Ram ram)
		{
			if ((value & bit) != 0) {
				ram.ClearFlag (Flag.Zero);
			} else {
				ram.SetFlag (Flag.Zero);
			}

			ram.ClearFlag (Flag.Negative);
			ram.SetFlag (Flag.HalfCarry);
		}

		public static byte Set (byte bit, byte value)
		{
			value |= bit;
			return value;
		}

		#endregion

	}

	public class Instruction<T> : Instruction
	{
		public new Action<T> Handler { get; set; }

		#region Constructors

		public Instruction (string disassembly, byte ticks, Action<T> handler) : base (disassembly, ticks)
		{
			if (typeof(byte).IsAssignableFrom (typeof(T))) {
				Operands = 1;
				Handler = handler;
			} else if (typeof(ushort).IsAssignableFrom (typeof(T))) {
				Operands = 2;
				Handler = handler;
			} else {
				Operands = 0;
				Handler = (T value) => NOP ();
			}
		}

		#endregion
	}
}

