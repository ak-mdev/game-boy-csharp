using System;

namespace gameboy
{
	public class InstructionSet
	{
		private Memory memory;

		private Instruction[] instructions;

		public InstructionSet (Memory memory)
		{
			this.memory = memory;
			instructions = GetInstructions ();
		}

		public Instruction this [byte key] {
			get {
				return instructions [key];
			}
		}

		private Instruction[] GetInstructions ()
		{
			return new Instruction[] {
			#region 0x0*
				new Instruction ("NOP", 2, Instruction.NOP), //0x00
				new Instruction ("LD BC, nn", 6, (ushort value) => {
					memory.Registers.BC = value;
				}), //0x01
				new Instruction ("LD (BC), A", 4, () => {
					memory.WriteByte (memory.Registers.BC, memory.Registers.A);
				}), //0x02
				new Instruction ("INC BC", 4, () => {
					memory.Registers.BC++;
				}), //0x03
				new Instruction ("INC B", 2, () => {
					memory.Registers.B = Instruction.Increment (memory.Registers.B, memory);
				}), //0x04
				new Instruction ("DEC B", 2, () => {
					memory.Registers.B = Instruction.Decrement (memory.Registers.B, memory);
				}), //0x05
				new Instruction ("LD B, n", 4, (byte value) => {
					memory.Registers.B = value;
				}), //0x06
				new Instruction ("RLC A", 4, Instruction.Undefined), //0x07
				new Instruction ("LD (nn), SP", 10, (ushort value) => {
					memory.WriteShort (value, memory.Registers.SP);
				}), //0x08
				new Instruction ("ADD HL, BC", 4, () => {
					Instruction.Add (ref memory.Registers.HL, memory.Registers.BC, memory);
				}), //0x09
				new Instruction ("LD A, (BC)", 4, () => {
					memory.Registers.A = memory.ReadByte (memory.Registers.BC);
				}), //0x0A
				new Instruction ("DEC BC", 4, () => {
					memory.Registers.BC--;
				}), //0x0B
				new Instruction ("INC C", 2, () => {
					memory.Registers.C = Instruction.Increment (memory.Registers.C, memory);
				}), //0x0C
				new Instruction ("DEC C", 2, () => {
					memory.Registers.C = Instruction.Decrement (memory.Registers.C, memory);
				}), //0x0D
				new Instruction ("LD C, n", 4, (byte value) => {
					memory.Registers.C = value;
				}), //0x0E
				new Instruction ("RRC A", 4, Instruction.Undefined), //0x0F
			#endregion

			#region 0x1*
				new Instruction ("STOP", 2, () => {
					throw new NotImplementedException ("Stop not implemented");
				}), //0x10
				new Instruction ("LD DE, nn", 6, (ushort value) => {
					memory.Registers.DE = value;
				}), //0x11
				new Instruction ("LD (DE), A", 4, () => {
					memory.WriteByte (memory.Registers.DE, memory.Registers.A);
				}), //0x12
				new Instruction ("INC DE", 4, () => {
					memory.Registers.DE++;
				}), //0x13
				new Instruction ("INC D", 2, () => {
					memory.Registers.D = Instruction.Increment (memory.Registers.D, memory);
				}), //0x14
				new Instruction ("DEC D", 2, () => {
					memory.Registers.D = Instruction.Decrement (memory.Registers.D, memory);
				}), //0x15
				new Instruction ("LD D, n", 4, (byte value) => {
					memory.Registers.D = value;
				}), //0x16
				new Instruction ("RL A", 4, Instruction.Undefined), //0x17
				new Instruction ("JR n", 4, Instruction.Undefined), //0x18
				new Instruction ("ADD HL, DE", 4, () => {
					Instruction.Add (ref memory.Registers.HL, memory.Registers.DE, memory);
				}), //0x19
				new Instruction ("LD A, (DE)", 4, () => {
					memory.Registers.A = memory.ReadByte (memory.Registers.DE);
				}), //0x1A
				new Instruction ("DEC DE", 4, () => {
					memory.Registers.DE--;
				}), //0x1B
				new Instruction ("INC E", 2, () => {
					memory.Registers.E = Instruction.Increment (memory.Registers.E, memory);
				}), //0x1C
				new Instruction ("DEC E", 2, () => {
					memory.Registers.E = Instruction.Decrement (memory.Registers.E, memory);
				}), //0x1D
				new Instruction ("LD E, n", 4, (byte value) => {
					memory.Registers.E = value;
				}), //0x1E
				new Instruction ("RR A", 4, Instruction.Undefined), //0x1F
			#endregion

			#region 0x2*
				new Instruction ("JR NZ, n", 0, Instruction.Undefined), //0x20
				new Instruction ("LD HL, nn", 6, (ushort value) => {
					memory.Registers.HL = value;
				}), //0x21
				new Instruction ("LDI (HL), A", 4, () => {
					memory.WriteByte (memory.Registers.HL++, memory.Registers.A);
				}), //0x22
				new Instruction ("INC HL", 4, () => {
					memory.Registers.HL++;
				}), //0x23
				new Instruction ("INC H", 2, () => {
					memory.Registers.H = Instruction.Increment (memory.Registers.H, memory);
				}), //0x24
				new Instruction ("DEC H", 2, () => {
					memory.Registers.H = Instruction.Decrement (memory.Registers.H, memory);
				}), //0x25
				new Instruction ("LD H, n", 4, (byte value) => {
					memory.Registers.H = value;
				}), //0x26
				new Instruction ("DAA", 2, Instruction.Undefined), //0x27
				new Instruction ("JR Z, n", 0, Instruction.Undefined), //0x28
				new Instruction ("ADD HL, HL", 4, () => {
					Instruction.Add (ref memory.Registers.HL, memory.Registers.HL, memory);
				}), //0x29
				new Instruction ("LDI A, (HL)", 4, () => {
					memory.Registers.A = memory.ReadByte (memory.Registers.HL++); 
				}), //0x2A
				new Instruction ("DEC HL", 4, () => {
					memory.Registers.HL++;
				}), //0x2B
				new Instruction ("INC L", 2, () => {
					memory.Registers.L = Instruction.Increment (memory.Registers.L, memory);
				}), //0x2C
				new Instruction ("DEC L", 2, () => {
					memory.Registers.L = Instruction.Decrement (memory.Registers.L, memory);
				}), //0x2D
				new Instruction ("LD L, n", 4, (byte value) => {
					memory.Registers.L = value;
				}), //0x2E
				new Instruction ("CPL", 2, Instruction.Undefined), //0x2F
			#endregion

			#region 0x3*
				new Instruction ("JR NC, n", 4, Instruction.Undefined), //0x30
				new Instruction ("LD SP, nn", 6, (ushort value) => {
					memory.Registers.SP = value;
				}), //0x31
				new Instruction ("LDD (HL), A", 4, () => {
					memory.WriteByte (memory.Registers.HL, memory.Registers.A);
					memory.Registers.HL--;
				}), //0x32
				new Instruction ("INC SP", 4, () => {
					memory.Registers.SP++;
				}), //0x33
				new Instruction ("INC (HL)", 6, () => {
					memory.WriteByte (memory.Registers.HL, Instruction.Increment (memory.ReadByte (memory.Registers.HL), memory));
				}), //0x34
				new Instruction ("DEC (HL)", 6, () => {
					memory.WriteByte (memory.Registers.HL, Instruction.Decrement (memory.ReadByte (memory.Registers.HL), memory));
				}), //0x35
				new Instruction ("LD (HL), n", 6, (byte value) => {
					memory.WriteByte (memory.Registers.HL, value);
				}), //0x36
				new Instruction ("SCF", 2, Instruction.Undefined), //0x37
				new Instruction ("JR C, n", 0, Instruction.Undefined), //0x38
				new Instruction ("ADD HL, SP", 4, () => {
					Instruction.Add (ref memory.Registers.HL, memory.Registers.SP, memory);
				}), //0x39
				new Instruction ("LDD A, (HL)", 4, () => {
					memory.Registers.A = memory.ReadByte (memory.Registers.HL--);
				}), //0x3A
				new Instruction ("DEC SP", 4, () => {
					memory.Registers.SP--;
				}), //0x3B
				new Instruction ("INC A", 2, () => {
					memory.Registers.A = Instruction.Increment (memory.Registers.A, memory);
				}), //0x3C
				new Instruction ("DEC A", 2, () => {
					memory.Registers.A = Instruction.Decrement (memory.Registers.A, memory);
				}), //0x3D
				new Instruction ("LD A, n", 4, (byte value) => {
					memory.Registers.A = value;
				}), //0x3E
				new Instruction ("CCF", 2, Instruction.Undefined), //0x3F
			#endregion

			#region 0x4*
				new Instruction ("LD B, B", 2, () => {
					memory.Registers.B = memory.Registers.B;
				}), //0x40
				new Instruction ("LD B, C", 2, () => {
					memory.Registers.B = memory.Registers.C;
				}), //0x41
				new Instruction ("LD B, D", 2, () => {
					memory.Registers.B = memory.Registers.D;
				}), //0x42
				new Instruction ("LD B, E", 2, () => {
					memory.Registers.B = memory.Registers.E;
				}), //0x43
				new Instruction ("LD B, H", 2, () => {
					memory.Registers.B = memory.Registers.H;
				}), //0x44
				new Instruction ("LD B, L", 2, () => {
					memory.Registers.B = memory.Registers.L;
				}), //0x45
				new Instruction ("LD B, (HL)", 4, () => {
					memory.Registers.B = memory.ReadByte (memory.Registers.HL);
				}), //0x46
				new Instruction ("LD B, A", 2, () => {
					memory.Registers.B = memory.Registers.A;
				}), //0x47
				new Instruction ("LD C, B", 2, () => {
					memory.Registers.C = memory.Registers.B;
				}), //0x48
				new Instruction ("LD C, C", 2, () => {
					memory.Registers.C = memory.Registers.C;
				}), //0x49
				new Instruction ("LD C, D", 2, () => {
					memory.Registers.C = memory.Registers.D;
				}), //0x4A
				new Instruction ("LD C, E", 2, () => {
					memory.Registers.C = memory.Registers.E;
				}), //0x4B
				new Instruction ("LD C, H", 2, () => {
					memory.Registers.C = memory.Registers.H;
				}), //0x4C
				new Instruction ("LD C, L", 2, () => {
					memory.Registers.C = memory.Registers.L;
				}), //0x4D
				new Instruction ("LD C, (HL)", 4, () => {
					memory.Registers.C = memory.ReadByte (memory.Registers.HL);
				}), //0x4E
				new Instruction ("LD C, A", 2, () => {
					memory.Registers.C = memory.Registers.A;
				}), //0x4F
			#endregion

			#region 0x5*
				new Instruction ("LD D, B", 2, () => {
					memory.Registers.D = memory.Registers.B;
				}), //0x50
				new Instruction ("LD D, C", 2, () => {
					memory.Registers.D = memory.Registers.C;
				}), //0x51
				new Instruction ("LD D, D", 2, () => {
					memory.Registers.D = memory.Registers.D;
				}), //0x52
				new Instruction ("LD D, E", 2, () => {
					memory.Registers.D = memory.Registers.E;
				}), //0x53
				new Instruction ("LD D, H", 2, () => {
					memory.Registers.D = memory.Registers.H;
				}), //0x54
				new Instruction ("LD D, L", 2, () => {
					memory.Registers.D = memory.Registers.L;
				}), //0x55
				new Instruction ("LD D, (HL)", 4, () => {
					memory.Registers.D = memory.ReadByte (memory.Registers.HL);
				}), //0x56
				new Instruction ("LD D, A", 2, () => {
					memory.Registers.D = memory.Registers.A;
				}), //0x57
				new Instruction ("LD E, B", 2, () => {
					memory.Registers.E = memory.Registers.B;
				}), //0x58
				new Instruction ("LD E, C", 2, () => {
					memory.Registers.E = memory.Registers.C;
				}), //0x59
				new Instruction ("LD E, D", 2, () => {
					memory.Registers.E = memory.Registers.D;
				}), //0x5A
				new Instruction ("LD E, E", 2, () => {
					memory.Registers.E = memory.Registers.E;
				}), //0x5B
				new Instruction ("LD E, H", 2, () => {
					memory.Registers.E = memory.Registers.H;
				}), //0x5C
				new Instruction ("LD E, L", 2, () => {
					memory.Registers.E = memory.Registers.L;
				}), //0x5D
				new Instruction ("LD E, (HL)", 4, () => {
					memory.Registers.E = memory.ReadByte (memory.Registers.HL);
				}), //0x5E
				new Instruction ("LD E, A", 2, () => {
					memory.Registers.E = memory.Registers.A;
				}), //0x5F
			#endregion

			#region 0x6*
				new Instruction ("LD H, B", 2, () => {
					memory.Registers.H = memory.Registers.B;
				}), //0x60
				new Instruction ("LD H, C", 2, () => {
					memory.Registers.H = memory.Registers.C;
				}), //0x61
				new Instruction ("LD H, D", 2, () => {
					memory.Registers.H = memory.Registers.D;
				}), //0x62
				new Instruction ("LD H, E", 2, () => {
					memory.Registers.H = memory.Registers.E;
				}), //0x63
				new Instruction ("LD H, H", 2, () => {
					memory.Registers.H = memory.Registers.H;
				}), //0x64
				new Instruction ("LD H, L", 2, () => {
					memory.Registers.H = memory.Registers.L;
				}), //0x65
				new Instruction ("LD H, (HL)", 4, () => {
					memory.Registers.H = memory.ReadByte (memory.Registers.HL);
				}), //0x66
				new Instruction ("LD H, A", 2, () => {
					memory.Registers.H = memory.Registers.A;
				}), //0x67
				new Instruction ("LD L, B", 2, () => {
					memory.Registers.L = memory.Registers.B;
				}), //0x68
				new Instruction ("LD L, C", 2, () => {
					memory.Registers.L = memory.Registers.C;
				}), //0x69
				new Instruction ("LD L, D", 2, () => {
					memory.Registers.L = memory.Registers.D;
				}), //0x6A
				new Instruction ("LD L, E", 2, () => {
					memory.Registers.L = memory.Registers.E;
				}), //0x6B
				new Instruction ("LD L, H", 2, () => {
					memory.Registers.L = memory.Registers.H;
				}), //0x6C
				new Instruction ("LD L, L", 2, () => {
					memory.Registers.L = memory.Registers.L;
				}), //0x6D
				new Instruction ("LD L, (HL)", 4, () => {
					memory.Registers.L = memory.ReadByte (memory.Registers.HL);
				}), //0x6E
				new Instruction ("LD L, A", 2, () => {
					memory.Registers.L = memory.Registers.A;
				}), //0x6F
			#endregion

			#region 0x7*
				new Instruction ("LD (HL), B", 4, () => {
					memory.WriteByte (memory.Registers.HL, memory.Registers.B);
				}), //0x70
				new Instruction ("LD (HL), C", 4, () => {
					memory.WriteByte (memory.Registers.HL, memory.Registers.C);
				}), //0x71
				new Instruction ("LD (HL), D", 4, () => {
					memory.WriteByte (memory.Registers.HL, memory.Registers.D);
				}), //0x72
				new Instruction ("LD (HL), E", 4, () => {
					memory.WriteByte (memory.Registers.HL, memory.Registers.E);
				}), //0x73
				new Instruction ("LD (HL), H", 4, () => {
					memory.WriteByte (memory.Registers.HL, memory.Registers.H);
				}), //0x74
				new Instruction ("LD (HL), L", 4, () => {
					memory.WriteByte (memory.Registers.HL, memory.Registers.L);
				}), //0x75
				new Instruction ("HALT", 2, Instruction.Undefined), //0x76
				new Instruction ("LD (HL), A", 4, () => {
					memory.WriteByte (memory.Registers.HL, memory.Registers.A);
				}), //0x77
				new Instruction ("LD A, B", 2, () => {
					memory.Registers.A = memory.Registers.B;
				}), //0x78
				new Instruction ("LD A, C", 2, () => {
					memory.Registers.A = memory.Registers.C;
				}), //0x79
				new Instruction ("LD A, D", 2, () => {
					memory.Registers.A = memory.Registers.D;
				}), //0x7A
				new Instruction ("LD A, E", 2, () => {
					memory.Registers.A = memory.Registers.E;
				}), //0x7B
				new Instruction ("LD A, H", 2, () => {
					memory.Registers.A = memory.Registers.H;
				}), //0x7C
				new Instruction ("LD A, L", 2, () => {
					memory.Registers.A = memory.Registers.L;
				}), //0x7D
				new Instruction ("LD A, (HL)", 4, () => {
					memory.Registers.A = memory.ReadByte (memory.Registers.HL);
				}), //0x7E
				new Instruction ("LD A, A", 2, () => {
					memory.Registers.A = memory.Registers.A;
				}), //0x7F
			#endregion

			#region 0x8*
				new Instruction ("ADD A, B", 2, () => {
					Instruction.Add (ref memory.Registers.A, memory.Registers.B, memory);
				}), //0x80
				new Instruction ("ADD A, C", 2, () => {
					Instruction.Add (ref memory.Registers.A, memory.Registers.C, memory);
				}), //0x81
				new Instruction ("ADD A, D", 2, () => {
					Instruction.Add (ref memory.Registers.A, memory.Registers.D, memory);
				}), //0x82
				new Instruction ("ADD A, E", 2, () => {
					Instruction.Add (ref memory.Registers.A, memory.Registers.E, memory);
				}), //0x83
				new Instruction ("ADD A, H", 2, () => {
					Instruction.Add (ref memory.Registers.A, memory.Registers.H, memory);
				}), //0x84
				new Instruction ("ADD A, L", 2, () => {
					Instruction.Add (ref memory.Registers.A, memory.Registers.L, memory);
				}), //0x85
				new Instruction ("ADD A, (HL)", 4, () => {
					Instruction.Add (ref memory.Registers.A, memory.ReadByte (memory.Registers.HL), memory);
				}), //0x86
				new Instruction ("ADD A, A", 2, () => {
					Instruction.Add (ref memory.Registers.A, memory.Registers.A, memory);
				}), //0x87
				new Instruction ("ADC A, B", 2, () => {
					Instruction.AddWithCarry (memory.Registers.B, memory);
				}), //0x88
				new Instruction ("ADC A, C", 2, () => {
					Instruction.AddWithCarry (memory.Registers.C, memory);
				}), //0x89
				new Instruction ("ADC A, D", 2, () => {
					Instruction.AddWithCarry (memory.Registers.D, memory);
				}), //0x8A
				new Instruction ("ADC A, E", 2, () => {
					Instruction.AddWithCarry (memory.Registers.E, memory);
				}), //0x8B
				new Instruction ("ADC A, H", 2, () => {
					Instruction.AddWithCarry (memory.Registers.H, memory);
				}), //0x8C
				new Instruction ("ADC A, L", 2, () => {
					Instruction.AddWithCarry (memory.Registers.L, memory);
				}), //0x8D
				new Instruction ("ADC A, (HL)", 4, () => {
					Instruction.AddWithCarry (memory.ReadByte (memory.Registers.HL), memory);
				}), //0x8E
				new Instruction ("ADC A, A", 2, () => {
					Instruction.AddWithCarry (memory.Registers.A, memory);
				}), //0x8F
			#endregion

			#region 0x9*
				new Instruction ("SUB A, B", 2, () => {
					Instruction.Subtract (memory.Registers.B, memory);
				}), //0x90
				new Instruction ("SUB A, C", 2, () => {
					Instruction.Subtract (memory.Registers.C, memory);
				}), //0x91
				new Instruction ("SUB A, D", 2, () => {
					Instruction.Subtract (memory.Registers.D, memory);
				}), //0x92
				new Instruction ("SUB A, E", 2, () => {
					Instruction.Subtract (memory.Registers.E, memory);
				}), //0x93
				new Instruction ("SUB A, H", 2, () => {
					Instruction.Subtract (memory.Registers.H, memory);
				}), //0x94
				new Instruction ("SUB A, L", 2, () => {
					Instruction.Subtract (memory.Registers.L, memory);
				}), //0x95
				new Instruction ("SUB A, (HL)", 4, () => {
					Instruction.Subtract (memory.ReadByte (memory.Registers.HL), memory);
				}), //0x96
				new Instruction ("SUB A, A", 2, () => {
					Instruction.Subtract (memory.Registers.A, memory);
				}), //0x97
				new Instruction ("SBC A, B", 2, () => {
					Instruction.SubtractWithCarry (memory.Registers.B, memory);
				}), //0x98
				new Instruction ("SBC A, C", 2, () => {
					Instruction.SubtractWithCarry (memory.Registers.C, memory);
				}), //0x99
				new Instruction ("SBC A, D", 2, () => {
					Instruction.SubtractWithCarry (memory.Registers.D, memory);
				}), //0x9A
				new Instruction ("SBC A, E", 2, () => {
					Instruction.SubtractWithCarry (memory.Registers.E, memory);
				}), //0x9B
				new Instruction ("SBC A, H", 2, () => {
					Instruction.SubtractWithCarry (memory.Registers.H, memory);
				}), //0x9C
				new Instruction ("SBC A, L", 2, () => {
					Instruction.SubtractWithCarry (memory.Registers.L, memory);
				}), //0x9D
				new Instruction ("SBC A, (HL)", 4, () => {
					Instruction.SubtractWithCarry (memory.ReadByte (memory.Registers.HL), memory);
				}), //0x9E
				new Instruction ("SBC A, A", 2, () => {
					Instruction.SubtractWithCarry (memory.Registers.A, memory);
				}), //0x9F
			#endregion

			#region 0xA*
				new Instruction ("AND B", 2, () => {
					Instruction.And (memory.Registers.B, memory);
				}), //0xA0
				new Instruction ("AND C", 2, () => {
					Instruction.And (memory.Registers.C, memory);
				}), //0xA1
				new Instruction ("AND D", 2, () => {
					Instruction.And (memory.Registers.D, memory);
				}), //0xA2
				new Instruction ("AND E", 2, () => {
					Instruction.And (memory.Registers.E, memory);
				}), //0xA3
				new Instruction ("AND H", 2, () => {
					Instruction.And (memory.Registers.H, memory);
				}), //0xA4
				new Instruction ("AND L", 2, () => {
					Instruction.And (memory.Registers.L, memory);
				}), //0xA5
				new Instruction ("AND (HL)", 4, () => {
					Instruction.And (memory.ReadByte (memory.Registers.HL), memory);
				}), //0xA6
				new Instruction ("AND A", 2, () => {
					Instruction.And (memory.Registers.A, memory);
				}), //0xA7
				new Instruction ("XOR B", 2, () => {
					Instruction.ExclusiveOr (memory.Registers.B, memory);
				}), //0xA8
				new Instruction ("XOR C", 2, () => {
					Instruction.ExclusiveOr (memory.Registers.C, memory);
				}), //0xA9
				new Instruction ("XOR D", 2, () => {
					Instruction.ExclusiveOr (memory.Registers.D, memory);
				}), //0xAA
				new Instruction ("XOR E", 2, () => {
					Instruction.ExclusiveOr (memory.Registers.E, memory);
				}), //0xAB
				new Instruction ("XOR H", 2, () => {
					Instruction.ExclusiveOr (memory.Registers.H, memory);
				}), //0xAC
				new Instruction ("XOR L", 2, () => {
					Instruction.ExclusiveOr (memory.Registers.L, memory);
				}), //0xAD
				new Instruction ("XOR (HL)", 4, () => {
					Instruction.ExclusiveOr (memory.ReadByte (memory.Registers.HL), memory);
				}), //0xAE
				new Instruction ("XOR A", 2, () => {
					Instruction.ExclusiveOr (memory.Registers.A, memory);
				}), //0xAF
			#endregion

			#region 0xB*
				new Instruction ("OR B", 2, () => {
					Instruction.Or (memory.Registers.B, memory);
				}), //0xB0
				new Instruction ("OR C", 2, () => {
					Instruction.Or (memory.Registers.C, memory);
				}), //0xB1
				new Instruction ("OR D", 2, () => {
					Instruction.Or (memory.Registers.D, memory);
				}), //0xB2
				new Instruction ("OR E", 2, () => {
					Instruction.Or (memory.Registers.E, memory);
				}), //0xB3
				new Instruction ("OR H", 2, () => {
					Instruction.Or (memory.Registers.H, memory);
				}), //0xB4
				new Instruction ("OR L", 2, () => {
					Instruction.Or (memory.Registers.L, memory);
				}), //0xB5
				new Instruction ("OR (HL)", 4, () => {
					Instruction.Or (memory.ReadByte (memory.Registers.HL), memory);
				}), //0xB6
				new Instruction ("OR A", 2, () => {
					Instruction.Or (memory.Registers.A, memory);
				}), //0xB7
				new Instruction ("CP B", 2, () => {
					Instruction.Compare (memory.Registers.B, memory);
				}), //0xB8
				new Instruction ("CP C", 2, () => {
					Instruction.Compare (memory.Registers.C, memory);
				}), //0xB9
				new Instruction ("CP D", 2, () => {
					Instruction.Compare (memory.Registers.D, memory);
				}), //0xBA
				new Instruction ("CP E", 2, () => {
					Instruction.Compare (memory.Registers.E, memory);
				}), //0xBB
				new Instruction ("CP H", 2, () => {
					Instruction.Compare (memory.Registers.H, memory);
				}), //0xBC
				new Instruction ("CP L", 2, () => {
					Instruction.Compare (memory.Registers.L, memory);
				}), //0xBD
				new Instruction ("CP (HL)", 4, () => {
					Instruction.Compare (memory.ReadByte (memory.Registers.HL), memory);
				}), //0xBE
				new Instruction ("CP A", 2, () => {
					Instruction.Compare (memory.Registers.A, memory);
				}), //0xBF
			#endregion

			#region 0xC*
				new Instruction ("RET NZ", 0, Instruction.Undefined), //0xC0
				new Instruction ("POP BC", 6, () => {
					memory.Registers.BC = memory.ReadFromStack ();
				}), //0xC1
				new Instruction ("JP NZ, nn", 0, Instruction.Undefined), //0xC2
				new Instruction ("JP nn", 6, Instruction.Undefined), //0xC3
				new Instruction ("CALL NZ, nn", 0, Instruction.Undefined), //0xC4
				new Instruction ("PUSH BC", 8, () => {
					memory.WriteToStack (memory.Registers.BC);
				}), //0xC5
				new Instruction ("ADD A, n", 4, (byte value) => {
					Instruction.Add (ref memory.Registers.A, value);
				}), //0xC6
				new Instruction ("RST 0", 8, () => {
					memory.WriteToStack (memory.Registers.PC);
					memory.Registers.PC = 0x0000;
				}), //0xC7
				new Instruction ("RET Z", 0, Instruction.Undefined), //0xC8
				new Instruction ("RET", 2, () => {
					memory.Registers.PC = memory.ReadFromStack ();
				}), //0xC9
				new Instruction ("JP Z, nn", 0, Instruction.Undefined), //0xCA
				new Instruction ("CB n", 0, Instruction.Undefined), //0xCB
				new Instruction ("CALL Z, nn", 0, Instruction.Undefined), //0xCC
				new Instruction ("CALL nn", 6, (ushort value) => {
					memory.WriteToStack (memory.Registers.PC);
					memory.Registers.PC = value;
				}), //0xCD
				new Instruction ("ADC A, n", 4, (byte value) => {
					Instruction.AddWithCarry (value);
				}), //0xCE
				new Instruction ("RST 8", 8, () => {
					memory.WriteToStack (memory.Registers.PC);
					memory.Registers.PC = 0x0008;
				}), //0xCF
			#endregion

			#region 0xD*
				new Instruction ("RET NC", 0, Instruction.Undefined), //0xD0
				new Instruction ("POP DE", 6, () => {
					memory.Registers.DE = memory.ReadFromStack ();
				}), //0xD1
				new Instruction ("JP NC, nn", 0, Instruction.Undefined), //0xD2
				new Instruction ("", 0, Instruction.Undefined), //0xD3
				new Instruction ("CALL NC, nn", 0, Instruction.Undefined), //0xD4
				new Instruction ("PUSH DE", 8, () => {
					memory.WriteToStack (memory.Registers.DE); 
				}), //0xD5
				new Instruction ("SUB A, n", 4, (byte value) => {
					Instruction.Subtract (value);
				}), //0xD6
				new Instruction ("RST 10", 8, () => {
					memory.WriteToStack (memory.Registers.PC);
					memory.Registers.PC = 0x0010;
				}), //0xD7
				new Instruction ("RET C", 0, Instruction.Undefined), //0xD8
				new Instruction ("RETI", 8, Instruction.Undefined), //0xD9
				new Instruction ("JP C, nn", 0, Instruction.Undefined), //0xDA
				new Instruction ("", 0, Instruction.Undefined), //0xDB
				new Instruction ("CALL C, nn", 0, Instruction.Undefined), //0xDC
				new Instruction ("", 0, Instruction.Undefined), //0xDD
				new Instruction ("SBC A, n", 4, (byte value) => {
					Instruction.SubtractWithCarry (value);
				}), //0xDE
				new Instruction ("RST 18", 8, () => {
					memory.WriteToStack (memory.Registers.PC);
					memory.Registers.PC = 0x0018;
				}), //0xDF
			#endregion

			#region 0xE*
				new Instruction ("LDH (n), A", 6, (byte value) => {
					memory.WriteByte (0xff00 + value, memory.Registers.A);
				}), //0xE0
				new Instruction ("POP HL", 6, () => {
					memory.Registers.HL = memory.ReadFromStack ();
				}), //0xE1
				new Instruction ("LDH (C), A", 4, () => {
					memory.WriteByte ((ushort)(0xff00 + memory.Registers.C), memory.Registers.A); 
				}), //0xE2
				new Instruction ("", 0, Instruction.Undefined), //0xE3
				new Instruction ("", 0, Instruction.Undefined), //0xE4
				new Instruction ("PUSH HL", 8, () => {
					memory.WriteToStack (memory.Registers.HL);
				}), //0xE5
				new Instruction ("AND n", 4, (byte value) => {
					memory.Registers.A &= value;

					memory.ClearFlag (Flag.Carry | Flag.Negative);
					memory.SetFlag (Flag.HalfCarry);
					if (memory.Registers.A) {
						memory.ClearFlag (Flag.Zero);
					} else {
						memory.SetFlag (Flag.Zero);
					}
				}), //0xE6
				new Instruction ("RST 20", 8, () => {
					memory.WriteToStack (memory.Registers.PC);
					memory.Registers.PC = 0x0020;
				}), //0xE7
				new Instruction ("ADD SP, d", 8, Instruction.Undefined), //0xE8
				new Instruction ("JP (HL)", 2, Instruction.Undefined), //0xE9
				new Instruction ("LD (nn), A", 8, (ushort value) => {
					memory.WriteByte (value, memory.Registers.A);
				}), //0xEA
				new Instruction ("", 0, Instruction.Undefined), //0xEB
				new Instruction ("", 0, Instruction.Undefined), //0xEC
				new Instruction ("", 0, Instruction.Undefined), //0xED
				new Instruction ("XOR n", 4, (byte value) => {
					Instruction.ExclusiveOr (value); 
				}), //0xEE
				new Instruction ("RST 28", 8, () => {
					memory.WriteToStack (memory.Registers.PC);
					memory.Registers.PC = 0x0028;
				}), //0xEF
			#endregion

			#region 0xF*
				new Instruction ("LDH A, (n)", 6, (byte value) => {
					memory.Registers.A = memory.ReadByte (0xff00 + value);
				}), //0xF0
				new Instruction ("POP AF", 6, () => {
					memory.Registers.AF = memory.ReadFromStack ();
				}), //0xF1
				new Instruction ("LD A, (C)", 4, () => {
					memory.Registers.A = memory.ReadByte ((ushort)(0xff00 + memory.Registers.C));
				}), //0xF2
				new Instruction ("DI", 2, Instruction.Undefined), //0xF3
				new Instruction ("", 0, Instruction.Undefined), //0xF4
				new Instruction ("PUSH AF", 8, () => {
					memory.WriteToStack (memory.Registers.AF);
				}), //0xF5
				new Instruction ("OR n", 4, (byte value) => {
					Instruction.Or (value);
				}), //0xF6
				new Instruction ("RST 30", 8, () => {
					memory.WriteToStack (memory.Registers.PC);
					memory.Registers.PC = 0x0030;
				}), //0xF7
				new Instruction ("LD HL, SP+r8", 6, (byte value) => {
					int result = memory.Registers.SP + (sbyte)value;

					if (result & 0xffff0000) {
						memory.SetFlag (Flag.Carry);
					} else {
						memory.ClearFlag (Flag.Carry);
					}

					if (((memory.Registers.SP & 0x0f) + (value & 0x0f)) > 0x0f) {
						memory.SetFlag (Flag.HalfCarry);
					} else {
						memory.ClearFlag (Flag.HalfCarry);
					}

					memory.ClearFlag (Flag.Zero | Flag.Negative);

					memory.Registers.HL = (ushort)(result & 0xffff);
				}), //0xF8
				new Instruction ("LD SP, HL", 4, () => {
					memory.Registers.SP = memory.Registers.HL;
				}), //0xF9
				new Instruction ("LD A, (nn)", 8, (ushort value) => {
					memory.Registers.A = memory.ReadByte (value);
				}), //0xFA
				new Instruction ("EI", 2, Instruction.Undefined), //0xFB
				new Instruction ("", 0, Instruction.Undefined), //0xFC
				new Instruction ("", 0, Instruction.Undefined), //0xFD
				new Instruction ("CP n", 4, (byte value) => {
					memory.SetFlag (Flag.Negative);

					if (memory.Registers.A == value) {
						memory.SetFlag (Flag.Zero);
					} else {
						memory.ClearFlag (Flag.Zero);
					}

					if (value > memory.Registers.A) {
						memory.SetFlag (Flag.Carry);
					} else {
						memory.ClearFlag (Flag.Carry);
					}

					if ((value & 0x0f) > (memory.Registers.A & 0x0f)) {
						memory.SetFlag (Flag.HalfCarry);
					} else {
						memory.ClearFlag (Flag.HalfCarry);
					}
				}), //0xFE
				new Instruction ("RST 38", 8, () => {
					memory.WriteToStack (memory.Registers.PC);
					memory.Registers.PC = 0x0038;
				})  //0xFF
			#endregion
			
			};
		}
	}
}