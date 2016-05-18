using System;
using GameBoy.Memory;

namespace GameBoy.CPU
{
	public class Cpu
	{
		public InstructionSet InstructionSet { get; private set; }

		public ulong Ticks { get; set; }

		public Cpu ()
		{
		}

		public void Reset (Ram ram, Interrupt interrupt)
		{
			Ticks = 0;
			InstructionSet = new InstructionSet (this, ram, interrupt);
		}

		public void Step (Ram ram)
		{
			var instructionIndex = ram.ReadByte (ram.Registers.PC++);
			var instruction = InstructionSet [instructionIndex];

			ushort operand = 0;
			if (instruction.Operands == 1) {
				operand = (ushort)ram.ReadByte (ram.Registers.PC);
			} else if (instruction.Operands == 2) {
				operand = ram.ReadShort (ram.Registers.PC);
			}

			ram.Registers.PC += instruction.Operands;

			if (instruction.Operands == 0) {
				instruction.Handler.Invoke ();
			} else if (instruction.Operands == 1) {
				var byteInstruction = (Instruction<byte>)instruction;
				byteInstruction.Handler ((byte)operand);
			} else if (instruction.Operands == 2) {
				var shortInstruction = (Instruction<ushort>)instruction;
				shortInstruction.Handler (operand);
			}

			Ticks += instruction.Ticks;
		}
	}
}

