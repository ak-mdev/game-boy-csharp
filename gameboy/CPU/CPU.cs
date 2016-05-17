using System;

namespace gameboy
{
	public class CPU
	{
		private Memory memory;

		InstructionSet InstructionSet { get; }

		ulong Ticks { get; set; }

		public CPU (Memory memory)
		{
			this.memory = memory;
			InstructionSet = new InstructionSet (this.memory);
		}

		public void Step ()
		{
			var instructionIndex = memory.ReadByte (memory.Registers.PC++);
			var instruction = InstructionSet [instructionIndex];

			ushort operand;
			switch (instruction.Operands) {
			case 0:
				instruction.Handler0 ();
				break;
			case 1:
				operand = (ushort)memory.ReadByte (memory.Registers.PC);
				instruction.Handler1 ((byte)operand);
				break;
			case 2:
				operand = memory.ReadShort (memory.Registers.PC);
				instruction.Handler2 (operand);
				break;
			}
			memory.Registers.PC += instruction.Operands;

			Ticks += instruction.Ticks;
		}
	}
}

