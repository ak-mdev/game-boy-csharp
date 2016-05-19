using System;
using GameBoy.CPU;
using GameBoy.Memory;

namespace GameBoy
{
	public static class Debug
	{
		public static bool Enabled { get; set; }

		public static void RealtimeDebug(Ram memory, InstructionSet instructions, Interrupt interrupt)
		{
			var debugMessage = string.Empty;

			var instructionIndex = memory.ReadByte (memory.Registers.PC);
			var instruction = instructions [instructionIndex];

			ushort operand = 0;

			if (instruction.Operands == 1) {
				operand = (ushort)memory.ReadByte ((ushort)(memory.Registers.PC + 1));
			} else if (instruction.Operands == 2) {
				operand = memory.ReadShort ((ushort)(memory.Registers.PC + 1));
			}

			debugMessage = instruction.Disassembly;
			if (instruction.Operands > 0) {
				debugMessage += " " + operand;
			}

			debugMessage += $"\n\nAF: {memory.Registers.AF}\n";
			debugMessage += $"BC: {memory.Registers.BC}\n";
			debugMessage += $"DE: {memory.Registers.DE}\n";
			debugMessage += $"HL: {memory.Registers.HL}\n";
			debugMessage += $"SP: {memory.Registers.SP}\n";
			debugMessage += $"PC: {memory.Registers.PC}\n";

			debugMessage += $"\nIME: {interrupt.Master}\n";
			debugMessage += $"IE: {interrupt.Enable}\n";
			debugMessage += $"IF: {interrupt.Flags}\n";

			debugMessage += "\nContinue debugging?\n";

			Console.WriteLine (debugMessage);
			var readIn = Console.ReadLine ();

			Enabled = (readIn == "yes" || readIn == "y");
		}

		public static void PrintRegisters (Ram memory, Interrupt interrupt)
		{
			Console.WriteLine("A: {0}\n", memory.Registers.A);
			Console.WriteLine("F: {0}\n", memory.Registers.F);
			Console.WriteLine("B: {0}\n", memory.Registers.B);
			Console.WriteLine("C: {0}\n", memory.Registers.C);
			Console.WriteLine("D: {0}\n", memory.Registers.D);
			Console.WriteLine("E: {0}\n", memory.Registers.E);
			Console.WriteLine("H: {0}\n", memory.Registers.H);
			Console.WriteLine("L: {0}\n", memory.Registers.L);
			Console.WriteLine("SP: {0}\n", memory.Registers.SP);
			Console.WriteLine("PC: {0}\n", memory.Registers.PC);
			Console.WriteLine("IME: {0}\n", interrupt.Master);
			Console.WriteLine("IE: {0}\n", interrupt.Enable);
			Console.WriteLine("IF: {0}\n", interrupt.Flags);
		}

		public static void Write (string message)
		{
			System.Diagnostics.Debug.Write (message);
		}

		public static void WriteIf (bool condition, string message)
		{
			System.Diagnostics.Debug.WriteIf (condition, message);
		}

		public static void WriteLine (string message)
		{
			System.Diagnostics.Debug.WriteLine (message);
		}

		public static void WriteLine(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine (format, args);
		}

		public static void WriteLineIf (bool condition, string message)
		{
			System.Diagnostics.Debug.WriteIf (condition, message);
		}
	}
}

