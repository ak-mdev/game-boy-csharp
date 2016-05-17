using System;
using GameBoy.Memory;

namespace GameBoy.CPU
{
	public class Cpu
	{
		private Ram Ram;

		public InstructionSet InstructionSet { get; }

		public ulong Ticks { get; set; }

		public Interrupt Interrupt { get; set; }

		public Cpu (Ram Ram)
		{
			this.Ram = Ram;
			InstructionSet = new InstructionSet (this.Ram);
		}

		public void Reset ()
		{
			Interrupt.Master = 1;
			Interrupt.Enable = 0;
			Interrupt.Flags = 0;

			Ticks = 0;
		}

		public void Step ()
		{
			var instructionIndex = Ram.ReadByte (Ram.Registers.PC++);
			var instruction = InstructionSet [instructionIndex];

			ushort operand;
			switch (instruction.Operands) {
			case 0:
				instruction.Handler.DynamicInvoke ();
				break;
			case 1:
				operand = (ushort)Ram.ReadByte (Ram.Registers.PC);
				instruction.Handler.DynamicInvoke ((byte)operand);
				break;
			case 2:
				operand = Ram.ReadShort (Ram.Registers.PC);
				instruction.Handler.DynamicInvoke (operand);
				break;
			}
			Ram.Registers.PC += instruction.Operands;

			Ticks += instruction.Ticks;
		}
	}
}

