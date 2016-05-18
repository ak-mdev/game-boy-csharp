using System;
using GameBoy.Memory;

namespace GameBoy.CPU
{
	public class InstructionSet
	{
		private Instruction[] instructions;

		public InstructionSet (Cpu cpu, Ram ram, Interrupt interrupt)
		{
			instructions = GetInstructions (cpu, ram, interrupt);
		}

		public Instruction this [byte key] {
			get {
				return instructions [key];
			}
		}

		private Instruction[] GetInstructions (Cpu cpu, Ram ram, Interrupt interrupt)
		{
			return new Instruction[] {
			#region 0x0*
				new Instruction ("NOP", 2, Instruction.NOP), //0x00
				new Instruction<ushort> ("LD BC, nn", 6, (ushort value) => {
					ram.Registers.BC = value;
				}), //0x01
				new Instruction ("LD (BC), A", 4, () => {
					ram.WriteByte (ram.Registers.BC, ram.Registers.A);
				}), //0x02
				new Instruction ("INC BC", 4, () => {
					ram.Registers.BC++;
				}), //0x03
				new Instruction ("INC B", 2, () => {
					ram.Registers.B = Instruction.Increment (ram.Registers.B, ram);
				}), //0x04
				new Instruction ("DEC B", 2, () => {
					ram.Registers.B = Instruction.Decrement (ram.Registers.B, ram);
				}), //0x05
				new Instruction<byte> ("LD B, n", 4, (byte value) => {
					ram.Registers.B = value;
				}), //0x06
				new Instruction ("RLC A", 4, () => {
					var carry = (byte)((ram.Registers.A & 0x80) >> 7);
					if (carry != 0) {
						ram.SetFlag (Flag.Carry);
					} else {
						ram.ClearFlag (Flag.Carry);
					}

					ram.Registers.A <<= 1;
					ram.Registers.A += carry;

					ram.ClearFlag (Flag.Negative | Flag.Zero | Flag.HalfCarry);
				}), //0x07
				new Instruction<ushort> ("LD (nn), SP", 10, (ushort value) => {
					ram.WriteShort (value, ram.Registers.SP);
				}), //0x08
				new Instruction ("ADD HL, BC", 4, () => {
					Instruction.Add (ref ram.Registers.HL, ram.Registers.BC, ram);
				}), //0x09
				new Instruction ("LD A, (BC)", 4, () => {
					ram.Registers.A = ram.ReadByte (ram.Registers.BC);
				}), //0x0A
				new Instruction ("DEC BC", 4, () => {
					ram.Registers.BC--;
				}), //0x0B
				new Instruction ("INC C", 2, () => {
					ram.Registers.C = Instruction.Increment (ram.Registers.C, ram);
				}), //0x0C
				new Instruction ("DEC C", 2, () => {
					ram.Registers.C = Instruction.Decrement (ram.Registers.C, ram);
				}), //0x0D
				new Instruction<byte> ("LD C, n", 4, (byte value) => {
					ram.Registers.C = value;
				}), //0x0E
				new Instruction ("RRC A", 4, () => {
					var carry = (byte)(ram.Registers.A & 0x01);
					if (carry != 0) {
						ram.SetFlag (Flag.Carry);
					} else {
						ram.ClearFlag (Flag.Carry);
					}

					ram.Registers.A >>= 1;
					if (carry != 0) {
						ram.Registers.A |= 0x80;
					}

					ram.ClearFlag (Flag.Negative | Flag.Zero | Flag.HalfCarry);
				}), //0x0F
			#endregion

			#region 0x1*
				new Instruction ("STOP", 2, () => {
					Program.Quit ();
				}), //0x10
				new Instruction<ushort> ("LD DE, nn", 6, (ushort value) => {
					ram.Registers.DE = value;
				}), //0x11
				new Instruction ("LD (DE), A", 4, () => {
					ram.WriteByte (ram.Registers.DE, ram.Registers.A);
				}), //0x12
				new Instruction ("INC DE", 4, () => {
					ram.Registers.DE++;
				}), //0x13
				new Instruction ("INC D", 2, () => {
					ram.Registers.D = Instruction.Increment (ram.Registers.D, ram);
				}), //0x14
				new Instruction ("DEC D", 2, () => {
					ram.Registers.D = Instruction.Decrement (ram.Registers.D, ram);
				}), //0x15
				new Instruction<byte> ("LD D, n", 4, (byte value) => {
					ram.Registers.D = value;
				}), //0x16
				new Instruction ("RL A", 4, () => {
					var carry = ram.IsFlagSet (Flag.Carry) ? 1 : 0;

					if ((ram.Registers.A & 0x80) != 0) {
						ram.SetFlag (Flag.Carry);
					} else {
						ram.ClearFlag (Flag.Carry);
					}

					ram.Registers.A <<= 1;
					ram.Registers.A += (byte)carry;

					ram.ClearFlag (Flag.Negative | Flag.Zero | Flag.HalfCarry);
				}), //0x17
				new Instruction <byte> ("JR n", 4, (byte value) => {
					ram.Registers.PC += value;
				}), //0x18
				new Instruction ("ADD HL, DE", 4, () => {
					Instruction.Add (ref ram.Registers.HL, ram.Registers.DE, ram);
				}), //0x19
				new Instruction ("LD A, (DE)", 4, () => {
					ram.Registers.A = ram.ReadByte (ram.Registers.DE);
				}), //0x1A
				new Instruction ("DEC DE", 4, () => {
					ram.Registers.DE--;
				}), //0x1B
				new Instruction ("INC E", 2, () => {
					ram.Registers.E = Instruction.Increment (ram.Registers.E, ram);
				}), //0x1C
				new Instruction ("DEC E", 2, () => {
					ram.Registers.E = Instruction.Decrement (ram.Registers.E, ram);
				}), //0x1D
				new Instruction<byte> ("LD E, n", 4, (byte value) => {
					ram.Registers.E = value;
				}), //0x1E
				new Instruction ("RR A", 4, () => {
					int carry = (ram.IsFlagSet (Flag.Carry) ? 1 : 0) << 7;

					if ((ram.Registers.A & 0x01) != 0) {
						ram.SetFlag (Flag.Carry);
					} else {
						ram.ClearFlag (Flag.Carry);
					}

					ram.Registers.A >>= 1;
					ram.Registers.A += (byte)carry;

					ram.ClearFlag (Flag.Negative | Flag.Zero | Flag.HalfCarry);
				}), //0x1F
			#endregion

			#region 0x2*
				new Instruction<byte> ("JR NZ, n", 0, (byte value) => {
					if (ram.IsFlagSet (Flag.Zero)) {
						cpu.Ticks += 8;
					} else {
						ram.Registers.PC += (ushort)((sbyte)value);
						cpu.Ticks += 12;
					}
				}), //0x20
				new Instruction<ushort> ("LD HL, nn", 6, (ushort value) => {
					ram.Registers.HL = value;
				}), //0x21
				new Instruction ("LDI (HL), A", 4, () => {
					ram.WriteByte (ram.Registers.HL++, ram.Registers.A);
				}), //0x22
				new Instruction ("INC HL", 4, () => {
					ram.Registers.HL++;
				}), //0x23
				new Instruction ("INC H", 2, () => {
					ram.Registers.H = Instruction.Increment (ram.Registers.H, ram);
				}), //0x24
				new Instruction ("DEC H", 2, () => {
					ram.Registers.H = Instruction.Decrement (ram.Registers.H, ram);
				}), //0x25
				new Instruction<byte> ("LD H, n", 4, (byte value) => {
					ram.Registers.H = value;
				}), //0x26
				new Instruction ("DAA", 2, () => {
					var s = (ushort)ram.Registers.A;

					if (ram.IsFlagSet (Flag.Negative)) {
						if (ram.IsFlagSet (Flag.HalfCarry)) {
							s = (ushort)((s - 0x06) & 0xFF);
						}
						if (ram.IsFlagSet (Flag.Carry)) {
							s -= 0x60;
						}
					} else {
						if (ram.IsFlagSet (Flag.HalfCarry) || (s & 0xF) > 9) {
							s += 0x06;
						}
						if (ram.IsFlagSet (Flag.Carry) || s > 0x9F) {
							s += 0x60;
						}
					}

					ram.Registers.A = (byte)s;
					ram.ClearFlag (Flag.HalfCarry);

					if (ram.Registers.A != 0) {
						ram.ClearFlag (Flag.Zero);
					} else {
						ram.SetFlag (Flag.Zero);
					}

					if (s >= 0x100) {
						ram.SetFlag (Flag.Carry);
					}
				}), //0x27
				new Instruction <byte> ("JR Z, n", 0, (byte value) => {
					if (ram.IsFlagSet (Flag.Zero)) {
						ram.Registers.PC += (ushort)((sbyte)value);
						cpu.Ticks += 12;
					} else {
						cpu.Ticks += 8;
					}
				}), //0x28
				new Instruction ("ADD HL, HL", 4, () => {
					Instruction.Add (ref ram.Registers.HL, ram.Registers.HL, ram);
				}), //0x29
				new Instruction ("LDI A, (HL)", 4, () => {
					ram.Registers.A = ram.ReadByte (ram.Registers.HL++); 
				}), //0x2A
				new Instruction ("DEC HL", 4, () => {
					ram.Registers.HL++;
				}), //0x2B
				new Instruction ("INC L", 2, () => {
					ram.Registers.L = Instruction.Increment (ram.Registers.L, ram);
				}), //0x2C
				new Instruction ("DEC L", 2, () => {
					ram.Registers.L = Instruction.Decrement (ram.Registers.L, ram);
				}), //0x2D
				new Instruction<byte> ("LD L, n", 4, (byte value) => {
					ram.Registers.L = value;
				}), //0x2E
				new Instruction ("CPL", 2, () => {
					ram.Registers.A = (byte)(~ram.Registers.A);
					ram.SetFlag (Flag.Negative | Flag.HalfCarry); 
				}), //0x2F
			#endregion

			#region 0x3*
				new Instruction<byte> ("JR NC, n", 4, (byte value) => {
					if (ram.IsFlagSet (Flag.Carry)) {
						cpu.Ticks += 8;
					} else {
						ram.Registers.PC += value;
						cpu.Ticks += 12;
					}

				}), //0x30
				new Instruction<ushort> ("LD SP, nn", 6, (ushort value) => {
					ram.Registers.SP = value;
				}), //0x31
				new Instruction ("LDD (HL), A", 4, () => {
					ram.WriteByte (ram.Registers.HL, ram.Registers.A);
					ram.Registers.HL--;
				}), //0x32
				new Instruction ("INC SP", 4, () => {
					ram.Registers.SP++;
				}), //0x33
				new Instruction ("INC (HL)", 6, () => {
					ram.WriteByte (ram.Registers.HL, Instruction.Increment (ram.ReadByte (ram.Registers.HL), ram));
				}), //0x34
				new Instruction ("DEC (HL)", 6, () => {
					ram.WriteByte (ram.Registers.HL, Instruction.Decrement (ram.ReadByte (ram.Registers.HL), ram));
				}), //0x35
				new Instruction<byte> ("LD (HL), n", 6, (byte value) => {
					ram.WriteByte (ram.Registers.HL, value);
				}), //0x36
				new Instruction ("SCF", 2, () => {
					ram.SetFlag (Flag.Carry);
					ram.ClearFlag (Flag.Negative | Flag.HalfCarry);
				}), //0x37
				new Instruction<byte> ("JR C, n", 0, (byte value) => {
					if (ram.IsFlagSet (Flag.Carry)) {
						ram.Registers.PC += value;
						cpu.Ticks += 12;
					} else {
						cpu.Ticks += 8;
					}
				}), //0x38
				new Instruction ("ADD HL, SP", 4, () => {
					Instruction.Add (ref ram.Registers.HL, ram.Registers.SP, ram);
				}), //0x39
				new Instruction ("LDD A, (HL)", 4, () => {
					ram.Registers.A = ram.ReadByte (ram.Registers.HL--);
				}), //0x3A
				new Instruction ("DEC SP", 4, () => {
					ram.Registers.SP--;
				}), //0x3B
				new Instruction ("INC A", 2, () => {
					ram.Registers.A = Instruction.Increment (ram.Registers.A, ram);
				}), //0x3C
				new Instruction ("DEC A", 2, () => {
					ram.Registers.A = Instruction.Decrement (ram.Registers.A, ram);
				}), //0x3D
				new Instruction<byte> ("LD A, n", 4, (byte value) => {
					ram.Registers.A = value;
				}), //0x3E
				new Instruction ("CCF", 2, () => {
					if (ram.IsFlagSet (Flag.Carry)) {
						ram.ClearFlag (Flag.Carry);
					} else {
						ram.SetFlag (Flag.Carry);
					}

					ram.ClearFlag (Flag.Negative | Flag.HalfCarry);
				}), //0x3F
			#endregion

			#region 0x4*
				new Instruction ("LD B, B", 2, () => {
					ram.Registers.B = ram.Registers.B;
				}), //0x40
				new Instruction ("LD B, C", 2, () => {
					ram.Registers.B = ram.Registers.C;
				}), //0x41
				new Instruction ("LD B, D", 2, () => {
					ram.Registers.B = ram.Registers.D;
				}), //0x42
				new Instruction ("LD B, E", 2, () => {
					ram.Registers.B = ram.Registers.E;
				}), //0x43
				new Instruction ("LD B, H", 2, () => {
					ram.Registers.B = ram.Registers.H;
				}), //0x44
				new Instruction ("LD B, L", 2, () => {
					ram.Registers.B = ram.Registers.L;
				}), //0x45
				new Instruction ("LD B, (HL)", 4, () => {
					ram.Registers.B = ram.ReadByte (ram.Registers.HL);
				}), //0x46
				new Instruction ("LD B, A", 2, () => {
					ram.Registers.B = ram.Registers.A;
				}), //0x47
				new Instruction ("LD C, B", 2, () => {
					ram.Registers.C = ram.Registers.B;
				}), //0x48
				new Instruction ("LD C, C", 2, () => {
					ram.Registers.C = ram.Registers.C;
				}), //0x49
				new Instruction ("LD C, D", 2, () => {
					ram.Registers.C = ram.Registers.D;
				}), //0x4A
				new Instruction ("LD C, E", 2, () => {
					ram.Registers.C = ram.Registers.E;
				}), //0x4B
				new Instruction ("LD C, H", 2, () => {
					ram.Registers.C = ram.Registers.H;
				}), //0x4C
				new Instruction ("LD C, L", 2, () => {
					ram.Registers.C = ram.Registers.L;
				}), //0x4D
				new Instruction ("LD C, (HL)", 4, () => {
					ram.Registers.C = ram.ReadByte (ram.Registers.HL);
				}), //0x4E
				new Instruction ("LD C, A", 2, () => {
					ram.Registers.C = ram.Registers.A;
				}), //0x4F
			#endregion

			#region 0x5*
				new Instruction ("LD D, B", 2, () => {
					ram.Registers.D = ram.Registers.B;
				}), //0x50
				new Instruction ("LD D, C", 2, () => {
					ram.Registers.D = ram.Registers.C;
				}), //0x51
				new Instruction ("LD D, D", 2, () => {
					ram.Registers.D = ram.Registers.D;
				}), //0x52
				new Instruction ("LD D, E", 2, () => {
					ram.Registers.D = ram.Registers.E;
				}), //0x53
				new Instruction ("LD D, H", 2, () => {
					ram.Registers.D = ram.Registers.H;
				}), //0x54
				new Instruction ("LD D, L", 2, () => {
					ram.Registers.D = ram.Registers.L;
				}), //0x55
				new Instruction ("LD D, (HL)", 4, () => {
					ram.Registers.D = ram.ReadByte (ram.Registers.HL);
				}), //0x56
				new Instruction ("LD D, A", 2, () => {
					ram.Registers.D = ram.Registers.A;
				}), //0x57
				new Instruction ("LD E, B", 2, () => {
					ram.Registers.E = ram.Registers.B;
				}), //0x58
				new Instruction ("LD E, C", 2, () => {
					ram.Registers.E = ram.Registers.C;
				}), //0x59
				new Instruction ("LD E, D", 2, () => {
					ram.Registers.E = ram.Registers.D;
				}), //0x5A
				new Instruction ("LD E, E", 2, () => {
					ram.Registers.E = ram.Registers.E;
				}), //0x5B
				new Instruction ("LD E, H", 2, () => {
					ram.Registers.E = ram.Registers.H;
				}), //0x5C
				new Instruction ("LD E, L", 2, () => {
					ram.Registers.E = ram.Registers.L;
				}), //0x5D
				new Instruction ("LD E, (HL)", 4, () => {
					ram.Registers.E = ram.ReadByte (ram.Registers.HL);
				}), //0x5E
				new Instruction ("LD E, A", 2, () => {
					ram.Registers.E = ram.Registers.A;
				}), //0x5F
			#endregion

			#region 0x6*
				new Instruction ("LD H, B", 2, () => {
					ram.Registers.H = ram.Registers.B;
				}), //0x60
				new Instruction ("LD H, C", 2, () => {
					ram.Registers.H = ram.Registers.C;
				}), //0x61
				new Instruction ("LD H, D", 2, () => {
					ram.Registers.H = ram.Registers.D;
				}), //0x62
				new Instruction ("LD H, E", 2, () => {
					ram.Registers.H = ram.Registers.E;
				}), //0x63
				new Instruction ("LD H, H", 2, () => {
					ram.Registers.H = ram.Registers.H;
				}), //0x64
				new Instruction ("LD H, L", 2, () => {
					ram.Registers.H = ram.Registers.L;
				}), //0x65
				new Instruction ("LD H, (HL)", 4, () => {
					ram.Registers.H = ram.ReadByte (ram.Registers.HL);
				}), //0x66
				new Instruction ("LD H, A", 2, () => {
					ram.Registers.H = ram.Registers.A;
				}), //0x67
				new Instruction ("LD L, B", 2, () => {
					ram.Registers.L = ram.Registers.B;
				}), //0x68
				new Instruction ("LD L, C", 2, () => {
					ram.Registers.L = ram.Registers.C;
				}), //0x69
				new Instruction ("LD L, D", 2, () => {
					ram.Registers.L = ram.Registers.D;
				}), //0x6A
				new Instruction ("LD L, E", 2, () => {
					ram.Registers.L = ram.Registers.E;
				}), //0x6B
				new Instruction ("LD L, H", 2, () => {
					ram.Registers.L = ram.Registers.H;
				}), //0x6C
				new Instruction ("LD L, L", 2, () => {
					ram.Registers.L = ram.Registers.L;
				}), //0x6D
				new Instruction ("LD L, (HL)", 4, () => {
					ram.Registers.L = ram.ReadByte (ram.Registers.HL);
				}), //0x6E
				new Instruction ("LD L, A", 2, () => {
					ram.Registers.L = ram.Registers.A;
				}), //0x6F
			#endregion

			#region 0x7*
				new Instruction ("LD (HL), B", 4, () => {
					ram.WriteByte (ram.Registers.HL, ram.Registers.B);
				}), //0x70
				new Instruction ("LD (HL), C", 4, () => {
					ram.WriteByte (ram.Registers.HL, ram.Registers.C);
				}), //0x71
				new Instruction ("LD (HL), D", 4, () => {
					ram.WriteByte (ram.Registers.HL, ram.Registers.D);
				}), //0x72
				new Instruction ("LD (HL), E", 4, () => {
					ram.WriteByte (ram.Registers.HL, ram.Registers.E);
				}), //0x73
				new Instruction ("LD (HL), H", 4, () => {
					ram.WriteByte (ram.Registers.HL, ram.Registers.H);
				}), //0x74
				new Instruction ("LD (HL), L", 4, () => {
					ram.WriteByte (ram.Registers.HL, ram.Registers.L);
				}), //0x75
				new Instruction ("HALT", 2, () => {
					if (interrupt.Master == 1) {
						//HALT EXECUTION UNTIL AN INTERRUPT OCCURS
					} else {
						ram.Registers.PC++;
					}
				}), //0x76
				new Instruction ("LD (HL), A", 4, () => {
					ram.WriteByte (ram.Registers.HL, ram.Registers.A);
				}), //0x77
				new Instruction ("LD A, B", 2, () => {
					ram.Registers.A = ram.Registers.B;
				}), //0x78
				new Instruction ("LD A, C", 2, () => {
					ram.Registers.A = ram.Registers.C;
				}), //0x79
				new Instruction ("LD A, D", 2, () => {
					ram.Registers.A = ram.Registers.D;
				}), //0x7A
				new Instruction ("LD A, E", 2, () => {
					ram.Registers.A = ram.Registers.E;
				}), //0x7B
				new Instruction ("LD A, H", 2, () => {
					ram.Registers.A = ram.Registers.H;
				}), //0x7C
				new Instruction ("LD A, L", 2, () => {
					ram.Registers.A = ram.Registers.L;
				}), //0x7D
				new Instruction ("LD A, (HL)", 4, () => {
					ram.Registers.A = ram.ReadByte (ram.Registers.HL);
				}), //0x7E
				new Instruction ("LD A, A", 2, () => {
					ram.Registers.A = ram.Registers.A;
				}), //0x7F
			#endregion

			#region 0x8*
				new Instruction ("ADD A, B", 2, () => {
					Instruction.Add (ref ram.Registers.A, ram.Registers.B, ram);
				}), //0x80
				new Instruction ("ADD A, C", 2, () => {
					Instruction.Add (ref ram.Registers.A, ram.Registers.C, ram);
				}), //0x81
				new Instruction ("ADD A, D", 2, () => {
					Instruction.Add (ref ram.Registers.A, ram.Registers.D, ram);
				}), //0x82
				new Instruction ("ADD A, E", 2, () => {
					Instruction.Add (ref ram.Registers.A, ram.Registers.E, ram);
				}), //0x83
				new Instruction ("ADD A, H", 2, () => {
					Instruction.Add (ref ram.Registers.A, ram.Registers.H, ram);
				}), //0x84
				new Instruction ("ADD A, L", 2, () => {
					Instruction.Add (ref ram.Registers.A, ram.Registers.L, ram);
				}), //0x85
				new Instruction ("ADD A, (HL)", 4, () => {
					Instruction.Add (ref ram.Registers.A, ram.ReadByte (ram.Registers.HL), ram);
				}), //0x86
				new Instruction ("ADD A, A", 2, () => {
					Instruction.Add (ref ram.Registers.A, ram.Registers.A, ram);
				}), //0x87
				new Instruction ("ADC A, B", 2, () => {
					Instruction.AddWithCarry (ram.Registers.B, ram);
				}), //0x88
				new Instruction ("ADC A, C", 2, () => {
					Instruction.AddWithCarry (ram.Registers.C, ram);
				}), //0x89
				new Instruction ("ADC A, D", 2, () => {
					Instruction.AddWithCarry (ram.Registers.D, ram);
				}), //0x8A
				new Instruction ("ADC A, E", 2, () => {
					Instruction.AddWithCarry (ram.Registers.E, ram);
				}), //0x8B
				new Instruction ("ADC A, H", 2, () => {
					Instruction.AddWithCarry (ram.Registers.H, ram);
				}), //0x8C
				new Instruction ("ADC A, L", 2, () => {
					Instruction.AddWithCarry (ram.Registers.L, ram);
				}), //0x8D
				new Instruction ("ADC A, (HL)", 4, () => {
					Instruction.AddWithCarry (ram.ReadByte (ram.Registers.HL), ram);
				}), //0x8E
				new Instruction ("ADC A, A", 2, () => {
					Instruction.AddWithCarry (ram.Registers.A, ram);
				}), //0x8F
			#endregion

			#region 0x9*
				new Instruction ("SUB A, B", 2, () => {
					Instruction.Subtract (ram.Registers.B, ram);
				}), //0x90
				new Instruction ("SUB A, C", 2, () => {
					Instruction.Subtract (ram.Registers.C, ram);
				}), //0x91
				new Instruction ("SUB A, D", 2, () => {
					Instruction.Subtract (ram.Registers.D, ram);
				}), //0x92
				new Instruction ("SUB A, E", 2, () => {
					Instruction.Subtract (ram.Registers.E, ram);
				}), //0x93
				new Instruction ("SUB A, H", 2, () => {
					Instruction.Subtract (ram.Registers.H, ram);
				}), //0x94
				new Instruction ("SUB A, L", 2, () => {
					Instruction.Subtract (ram.Registers.L, ram);
				}), //0x95
				new Instruction ("SUB A, (HL)", 4, () => {
					Instruction.Subtract (ram.ReadByte (ram.Registers.HL), ram);
				}), //0x96
				new Instruction ("SUB A, A", 2, () => {
					Instruction.Subtract (ram.Registers.A, ram);
				}), //0x97
				new Instruction ("SBC A, B", 2, () => {
					Instruction.SubtractWithCarry (ram.Registers.B, ram);
				}), //0x98
				new Instruction ("SBC A, C", 2, () => {
					Instruction.SubtractWithCarry (ram.Registers.C, ram);
				}), //0x99
				new Instruction ("SBC A, D", 2, () => {
					Instruction.SubtractWithCarry (ram.Registers.D, ram);
				}), //0x9A
				new Instruction ("SBC A, E", 2, () => {
					Instruction.SubtractWithCarry (ram.Registers.E, ram);
				}), //0x9B
				new Instruction ("SBC A, H", 2, () => {
					Instruction.SubtractWithCarry (ram.Registers.H, ram);
				}), //0x9C
				new Instruction ("SBC A, L", 2, () => {
					Instruction.SubtractWithCarry (ram.Registers.L, ram);
				}), //0x9D
				new Instruction ("SBC A, (HL)", 4, () => {
					Instruction.SubtractWithCarry (ram.ReadByte (ram.Registers.HL), ram);
				}), //0x9E
				new Instruction ("SBC A, A", 2, () => {
					Instruction.SubtractWithCarry (ram.Registers.A, ram);
				}), //0x9F
			#endregion

			#region 0xA*
				new Instruction ("AND B", 2, () => {
					Instruction.And (ram.Registers.B, ram);
				}), //0xA0
				new Instruction ("AND C", 2, () => {
					Instruction.And (ram.Registers.C, ram);
				}), //0xA1
				new Instruction ("AND D", 2, () => {
					Instruction.And (ram.Registers.D, ram);
				}), //0xA2
				new Instruction ("AND E", 2, () => {
					Instruction.And (ram.Registers.E, ram);
				}), //0xA3
				new Instruction ("AND H", 2, () => {
					Instruction.And (ram.Registers.H, ram);
				}), //0xA4
				new Instruction ("AND L", 2, () => {
					Instruction.And (ram.Registers.L, ram);
				}), //0xA5
				new Instruction ("AND (HL)", 4, () => {
					Instruction.And (ram.ReadByte (ram.Registers.HL), ram);
				}), //0xA6
				new Instruction ("AND A", 2, () => {
					Instruction.And (ram.Registers.A, ram);
				}), //0xA7
				new Instruction ("XOR B", 2, () => {
					Instruction.ExclusiveOr (ram.Registers.B, ram);
				}), //0xA8
				new Instruction ("XOR C", 2, () => {
					Instruction.ExclusiveOr (ram.Registers.C, ram);
				}), //0xA9
				new Instruction ("XOR D", 2, () => {
					Instruction.ExclusiveOr (ram.Registers.D, ram);
				}), //0xAA
				new Instruction ("XOR E", 2, () => {
					Instruction.ExclusiveOr (ram.Registers.E, ram);
				}), //0xAB
				new Instruction ("XOR H", 2, () => {
					Instruction.ExclusiveOr (ram.Registers.H, ram);
				}), //0xAC
				new Instruction ("XOR L", 2, () => {
					Instruction.ExclusiveOr (ram.Registers.L, ram);
				}), //0xAD
				new Instruction ("XOR (HL)", 4, () => {
					Instruction.ExclusiveOr (ram.ReadByte (ram.Registers.HL), ram);
				}), //0xAE
				new Instruction ("XOR A", 2, () => {
					Instruction.ExclusiveOr (ram.Registers.A, ram);
				}), //0xAF
			#endregion

			#region 0xB*
				new Instruction ("OR B", 2, () => {
					Instruction.Or (ram.Registers.B, ram);
				}), //0xB0
				new Instruction ("OR C", 2, () => {
					Instruction.Or (ram.Registers.C, ram);
				}), //0xB1
				new Instruction ("OR D", 2, () => {
					Instruction.Or (ram.Registers.D, ram);
				}), //0xB2
				new Instruction ("OR E", 2, () => {
					Instruction.Or (ram.Registers.E, ram);
				}), //0xB3
				new Instruction ("OR H", 2, () => {
					Instruction.Or (ram.Registers.H, ram);
				}), //0xB4
				new Instruction ("OR L", 2, () => {
					Instruction.Or (ram.Registers.L, ram);
				}), //0xB5
				new Instruction ("OR (HL)", 4, () => {
					Instruction.Or (ram.ReadByte (ram.Registers.HL), ram);
				}), //0xB6
				new Instruction ("OR A", 2, () => {
					Instruction.Or (ram.Registers.A, ram);
				}), //0xB7
				new Instruction ("CP B", 2, () => {
					Instruction.Compare (ram.Registers.B, ram);
				}), //0xB8
				new Instruction ("CP C", 2, () => {
					Instruction.Compare (ram.Registers.C, ram);
				}), //0xB9
				new Instruction ("CP D", 2, () => {
					Instruction.Compare (ram.Registers.D, ram);
				}), //0xBA
				new Instruction ("CP E", 2, () => {
					Instruction.Compare (ram.Registers.E, ram);
				}), //0xBB
				new Instruction ("CP H", 2, () => {
					Instruction.Compare (ram.Registers.H, ram);
				}), //0xBC
				new Instruction ("CP L", 2, () => {
					Instruction.Compare (ram.Registers.L, ram);
				}), //0xBD
				new Instruction ("CP (HL)", 4, () => {
					Instruction.Compare (ram.ReadByte (ram.Registers.HL), ram);
				}), //0xBE
				new Instruction ("CP A", 2, () => {
					Instruction.Compare (ram.Registers.A, ram);
				}), //0xBF
			#endregion

			#region 0xC*
				new Instruction ("RET NZ", 0, () => {
					if (ram.IsFlagSet (Flag.Zero)) {
						cpu.Ticks += 8;
					} else {
						ram.Registers.PC = ram.ReadFromStack ();
						cpu.Ticks += 20;
					}
				}), //0xC0
				new Instruction ("POP BC", 6, () => {
					ram.Registers.BC = ram.ReadFromStack ();
				}), //0xC1
				new Instruction <ushort> ("JP NZ, nn", 0, (ushort value) => {
					if (ram.IsFlagSet (Flag.Zero)) {
						cpu.Ticks += 12;
					} else {
						ram.Registers.PC = value;
						cpu.Ticks += 16;
					}
				}), //0xC2
				new Instruction <ushort> ("JP nn", 6, (ushort value) => {
					ram.Registers.PC = value;
				}), //0xC3
				new Instruction <ushort> ("CALL NZ, nn", 0, (ushort value) => {
					if (ram.IsFlagSet (Flag.Zero)) {
						cpu.Ticks += 12;
					} else {
						ram.WriteToStack (ram.Registers.PC);
						ram.Registers.PC = value;
						cpu.Ticks += 24;
					}

				}), //0xC4
				new Instruction ("PUSH BC", 8, () => {
					ram.WriteToStack (ram.Registers.BC);
				}), //0xC5
				new Instruction<byte> ("ADD A, n", 4, (byte value) => {
					Instruction.Add (ref ram.Registers.A, value, ram);
				}), //0xC6
				new Instruction ("RST 0", 8, () => {
					ram.WriteToStack (ram.Registers.PC);
					ram.Registers.PC = 0x0000;
				}), //0xC7
				new Instruction ("RET Z", 0, () => {
					if (ram.IsFlagSet (Flag.Zero)) {
						ram.Registers.PC = ram.ReadFromStack ();
						cpu.Ticks += 20;
					} else {
						cpu.Ticks += 8;
					}
				}), //0xC8
				new Instruction ("RET", 2, () => {
					ram.Registers.PC = ram.ReadFromStack ();
				}), //0xC9
				new Instruction<ushort> ("JP Z, nn", 0, (ushort value) => {
					if (ram.IsFlagSet (Flag.Zero)) {
						ram.Registers.PC = value;
						cpu.Ticks += 16;
					} else {
						cpu.Ticks += 12;
					}
				}), //0xCA
				new Instruction <byte> ("CB n", 0, Instruction.CB), //0xCB
				new Instruction<ushort> ("CALL Z, nn", 0, (ushort value) => {
					if (ram.IsFlagSet (Flag.Zero)) {
						ram.WriteToStack (ram.Registers.PC);
						ram.Registers.PC = value;
						cpu.Ticks += 24;
					} else {
						cpu.Ticks += 12;
					}
				}), //0xCC
				new Instruction<ushort> ("CALL nn", 6, (ushort value) => {
					ram.WriteToStack (ram.Registers.PC);
					ram.Registers.PC = value;
				}), //0xCD
				new Instruction<byte> ("ADC A, n", 4, (byte value) => {
					Instruction.AddWithCarry (value, ram);
				}), //0xCE
				new Instruction ("RST 8", 8, () => {
					ram.WriteToStack (ram.Registers.PC);
					ram.Registers.PC = 0x0008;
				}), //0xCF
			#endregion

			#region 0xD*
				new Instruction ("RET NC", 0, () => {
					if (ram.IsFlagSet (Flag.Carry)) {
						cpu.Ticks += 8;
					} else {
						ram.Registers.PC = ram.ReadFromStack ();
						cpu.Ticks += 20;
					}
				}), //0xD0
				new Instruction ("POP DE", 6, () => {
					ram.Registers.DE = ram.ReadFromStack ();
				}), //0xD1
				new Instruction <ushort> ("JP NC, nn", 0, (ushort value) => {
					if (!ram.IsFlagSet (Flag.Carry)) {
						ram.Registers.PC = value;
						cpu.Ticks += 16;
					} else {
						cpu.Ticks += 12;
					}
				}), //0xD2
				new Instruction ("", 0, () => {
					Instruction.Undefined (ram);
				}), //0xD3
				new Instruction <ushort> ("CALL NC, nn", 0, (ushort value) => {
					if (!ram.IsFlagSet (Flag.Carry)) {
						ram.WriteToStack (ram.Registers.PC);
						ram.Registers.PC = value;
						cpu.Ticks += 24;
					} else {
						cpu.Ticks += 12;
					}
				}), //0xD4
				new Instruction ("PUSH DE", 8, () => {
					ram.WriteToStack (ram.Registers.DE); 
				}), //0xD5
				new Instruction<byte> ("SUB A, n", 4, (byte value) => {
					Instruction.Subtract (value, ram);
				}), //0xD6
				new Instruction ("RST 10", 8, () => {
					ram.WriteToStack (ram.Registers.PC);
					ram.Registers.PC = 0x0010;
				}), //0xD7
				new Instruction ("RET C", 0, () => {
					if (ram.IsFlagSet (Flag.Carry)) {
						ram.Registers.PC = ram.ReadFromStack ();
						cpu.Ticks += 20;
					} else {
						cpu.Ticks += 8;
					}
				}), //0xD8
				new Instruction ("RETI", 8, () => {
					interrupt.OnReturn ();
				}), //0xD9
				new Instruction <ushort> ("JP C, nn", 0, (ushort value) => {
					if (ram.IsFlagSet (Flag.Carry)) {
						ram.Registers.PC = value;
						cpu.Ticks += 16;
					} else {
						cpu.Ticks += 12;
					}
				}), //0xDA
				new Instruction ("", 0, () => {
					Instruction.Undefined (ram);
				}), //0xDB
				new Instruction <ushort> ("CALL C, nn", 0, (ushort value) => {
					if (ram.IsFlagSet (Flag.Carry)) {
						ram.WriteToStack (ram.Registers.PC);
						ram.Registers.PC = value;
						cpu.Ticks += 24;
					} else {
						cpu.Ticks += 12;
					}
				}), //0xDC
				new Instruction ("", 0, () => {
					Instruction.Undefined (ram);
				}), //0xDD
				new Instruction<byte> ("SBC A, n", 4, (byte value) => {
					Instruction.SubtractWithCarry (value, ram);
				}), //0xDE
				new Instruction ("RST 18", 8, () => {
					ram.WriteToStack (ram.Registers.PC);
					ram.Registers.PC = 0x0018;
				}), //0xDF
			#endregion

			#region 0xE*
				new Instruction<byte> ("LDH (n), A", 6, (byte value) => {
					ram.WriteByte ((ushort)(0xff00 + value), ram.Registers.A);
				}), //0xE0
				new Instruction ("POP HL", 6, () => {
					ram.Registers.HL = ram.ReadFromStack ();
				}), //0xE1
				new Instruction ("LDH (C), A", 4, () => {
					ram.WriteByte ((ushort)(0xff00 + ram.Registers.C), ram.Registers.A); 
				}), //0xE2
				new Instruction ("", 0, () => {
					Instruction.Undefined (ram);
				}), //0xE3
				new Instruction ("", 0, () => {
					Instruction.Undefined (ram);
				}), //0xE4
				new Instruction ("PUSH HL", 8, () => {
					ram.WriteToStack (ram.Registers.HL);
				}), //0xE5
				new Instruction<byte> ("AND n", 4, (byte value) => {
					ram.Registers.A &= value;

					ram.ClearFlag (Flag.Carry | Flag.Negative);
					ram.SetFlag (Flag.HalfCarry);
					if (ram.Registers.A != 0) {
						ram.ClearFlag (Flag.Zero);
					} else {
						ram.SetFlag (Flag.Zero);
					}
				}), //0xE6
				new Instruction ("RST 20", 8, () => {
					ram.WriteToStack (ram.Registers.PC);
					ram.Registers.PC = 0x0020;
				}), //0xE7
				new Instruction <byte> ("ADD SP, d", 8, (byte value) => {
					var result = ram.Registers.SP + value;

					if ((result & 0xffff0000) != 0) {
						ram.SetFlag (Flag.Carry);
					} else {
						ram.ClearFlag (Flag.Carry);
					}

					ram.Registers.SP = (ushort)(result & 0xffff);

					if (((ram.Registers.SP & 0x0f) + (value & 0x0f)) > 0x0f) {
						ram.SetFlag (Flag.HalfCarry);
					} else {
						ram.ClearFlag (Flag.HalfCarry);
					}

					ram.ClearFlag (Flag.Zero | Flag.Negative);
				}), //0xE8
				new Instruction ("JP (HL)", 2, () => {
					ram.Registers.PC = ram.Registers.HL;
				}), //0xE9
				new Instruction<ushort> ("LD (nn), A", 8, (ushort value) => {
					ram.WriteByte (value, ram.Registers.A);
				}), //0xEA
				new Instruction ("", 0, () => {
					Instruction.Undefined (ram);
				}), //0xEB
				new Instruction ("", 0, () => {
					Instruction.Undefined (ram);
				}), //0xEC
				new Instruction ("", 0, () => {
					Instruction.Undefined (ram);
				}), //0xED
				new Instruction<byte> ("XOR n", 4, (byte value) => {
					Instruction.ExclusiveOr (value, ram); 
				}), //0xEE
				new Instruction ("RST 28", 8, () => {
					ram.WriteToStack (ram.Registers.PC);
					ram.Registers.PC = 0x0028;
				}), //0xEF
			#endregion

			#region 0xF*
				new Instruction<byte> ("LDH A, (n)", 6, (byte value) => {
					ram.Registers.A = ram.ReadByte ((ushort)(0xff00 + value));
				}), //0xF0
				new Instruction ("POP AF", 6, () => {
					ram.Registers.AF = ram.ReadFromStack ();
				}), //0xF1
				new Instruction ("LD A, (C)", 4, () => {
					ram.Registers.A = ram.ReadByte ((ushort)(0xff00 + ram.Registers.C));
				}), //0xF2
				new Instruction ("DI", 2, () => {
					interrupt.Master = 0; 
				}), //0xF3
				new Instruction ("", 0, () => {
					Instruction.Undefined (ram);
				}), //0xF4
				new Instruction ("PUSH AF", 8, () => {
					ram.WriteToStack (ram.Registers.AF);
				}), //0xF5
				new Instruction<byte> ("OR n", 4, (byte value) => {
					Instruction.Or (value, ram);
				}), //0xF6
				new Instruction ("RST 30", 8, () => {
					ram.WriteToStack (ram.Registers.PC);
					ram.Registers.PC = 0x0030;
				}), //0xF7
				new Instruction<byte> ("LD HL, SP+r8", 6, (byte value) => {
					int result = ram.Registers.SP + (sbyte)value;

					if ((result & 0xffff0000) != 0) {
						ram.SetFlag (Flag.Carry);
					} else {
						ram.ClearFlag (Flag.Carry);
					}

					if (((ram.Registers.SP & 0x0f) + (value & 0x0f)) > 0x0f) {
						ram.SetFlag (Flag.HalfCarry);
					} else {
						ram.ClearFlag (Flag.HalfCarry);
					}

					ram.ClearFlag (Flag.Zero | Flag.Negative);

					ram.Registers.HL = (ushort)(result & 0xffff);
				}), //0xF8
				new Instruction ("LD SP, HL", 4, () => {
					ram.Registers.SP = ram.Registers.HL;
				}), //0xF9
				new Instruction<ushort> ("LD A, (nn)", 8, (ushort value) => {
					ram.Registers.A = ram.ReadByte (value);
				}), //0xFA
				new Instruction ("EI", 2, () => {
					interrupt.Master = 1;
				}), //0xFB
				new Instruction ("", 0, () => {
					Instruction.Undefined (ram);
				}), //0xFC
				new Instruction ("", 0, () => {
					Instruction.Undefined (ram);
				}), //0xFD
				new Instruction<byte> ("CP n", 4, (byte value) => {
					ram.SetFlag (Flag.Negative);

					if (ram.Registers.A == value) {
						ram.SetFlag (Flag.Zero);
					} else {
						ram.ClearFlag (Flag.Zero);
					}

					if (value > ram.Registers.A) {
						ram.SetFlag (Flag.Carry);
					} else {
						ram.ClearFlag (Flag.Carry);
					}

					if ((value & 0x0f) > (ram.Registers.A & 0x0f)) {
						ram.SetFlag (Flag.HalfCarry);
					} else {
						ram.ClearFlag (Flag.HalfCarry);
					}
				}), //0xFE
				new Instruction ("RST 38", 8, () => {
					ram.WriteToStack (ram.Registers.PC);
					ram.Registers.PC = 0x0038;
				})  //0xFF
			#endregion
			
			};
		}

		private Instruction[] GetCBInstructions (Ram ram)
		{
			return new Instruction[] {
				
				#region 0x0*
				new Instruction<byte> ("RLC B", 8, (byte value) => {
				}), // 0x00
				new Instruction<byte> ("RLC C", 8, (byte value) => {
				}), // 0x01
				new Instruction<byte> ("RLC D", 8, (byte value) => {
				}), // 0x02
				new Instruction<byte> ("RLC E", 8, (byte value) => {
				}), // 0x03
				new Instruction<byte> ("RLC H", 8, (byte value) => {
				}), // 0x04
				new Instruction<byte> ("RLC L", 8, (byte value) => {
				}), // 0x05
				new Instruction<byte> ("RLC (HL)", 16, (byte value) => {
				}), // 0x06
				new Instruction<byte> ("RLC A", 8, (byte value) => {
				}), // 0x07
				new Instruction<byte> ("RRC B", 8, (byte value) => {
				}), // 0x08
				new Instruction<byte> ("RRC C", 8, (byte value) => {
				}), // 0x09
				new Instruction<byte> ("RRC D", 8, (byte value) => {
				}), // 0x0A
				new Instruction<byte> ("RRC E", 8, (byte value) => {
				}), // 0x0B
				new Instruction<byte> ("RRC H", 8, (byte value) => {
				}), // 0x0C
				new Instruction<byte> ("RRC L", 8, (byte value) => {
				}), // 0x0D
				new Instruction<byte> ("RRC (HL)", 16, (byte value) => {
				}), // 0x0E
				new Instruction<byte> ("RRC A", 8, (byte value) => {
				}), // 0x0F
				#endregion

				#region 0x1*
				new Instruction<byte> ("RL B", 8, (byte value) => {
				}), // 0x10
				new Instruction<byte> ("RL C", 8, (byte value) => {
				}), // 0x11
				new Instruction<byte> ("RL D", 8, (byte value) => {
				}), // 0x12
				new Instruction<byte> ("RL E", 8, (byte value) => {
				}), // 0x13
				new Instruction<byte> ("RL H", 8, (byte value) => {
				}), // 0x14
				new Instruction<byte> ("RL L", 8, (byte value) => {
				}), // 0x15
				new Instruction<byte> ("RL (HL)", 16, (byte value) => {
				}), // 0x16
				new Instruction<byte> ("RL A", 8, (byte value) => {
				}), // 0x17
				new Instruction<byte> ("RR B", 8, (byte value) => {
				}), // 0x18
				new Instruction<byte> ("RR C", 8, (byte value) => {
				}), // 0x19
				new Instruction<byte> ("RR D", 8, (byte value) => {
				}), // 0x1A
				new Instruction<byte> ("RR E", 8, (byte value) => {
				}), // 0x1B
				new Instruction<byte> ("RR H", 8, (byte value) => {
				}), // 0x1C
				new Instruction<byte> ("RR L", 8, (byte value) => {
				}), // 0x1D
				new Instruction<byte> ("RR (HL)", 16, (byte value) => {
				}), // 0x1E
				new Instruction<byte> ("RR A", 8, (byte value) => {
				}), // 0x1F
				#endregion

				#region 0x2*
				new Instruction<byte> ("SLA B", 8, (byte value) => {
				}), // 0x20
				new Instruction<byte> ("SLA C", 8, (byte value) => {
				}), // 0x21
				new Instruction<byte> ("SLA D", 8, (byte value) => {
				}), // 0x22
				new Instruction<byte> ("SLA E", 8, (byte value) => {
				}), // 0x23
				new Instruction<byte> ("SLA H", 8, (byte value) => {
				}), // 0x24
				new Instruction<byte> ("SLA L", 8, (byte value) => {
				}), // 0x25
				new Instruction<byte> ("SLA (HL)", 16, (byte value) => {
				}), // 0x26
				new Instruction<byte> ("SLA A", 8, (byte value) => {
				}), // 0x27
				new Instruction<byte> ("SRA B", 8, (byte value) => {
				}), // 0x28
				new Instruction<byte> ("SRA C", 8, (byte value) => {
				}), // 0x29
				new Instruction<byte> ("SRA D", 8, (byte value) => {
				}), // 0x2A
				new Instruction<byte> ("SRA E", 8, (byte value) => {
				}), // 0x2B
				new Instruction<byte> ("SRA H", 8, (byte value) => {
				}), // 0x2C
				new Instruction<byte> ("SRA L", 8, (byte value) => {
				}), // 0x2D
				new Instruction<byte> ("SRA (HL)", 16, (byte value) => {
				}), // 0x2E
				new Instruction<byte> ("SRA A", 8, (byte value) => {
				}), // 0x2F
				#endregion

				#region 0x3*
				new Instruction<byte> ("SWAP B", 8, (byte value) => {
				}), // 0x30
				new Instruction<byte> ("SWAP C", 8, (byte value) => {
				}), // 0x31
				new Instruction<byte> ("SWAP D", 8, (byte value) => {
				}), // 0x32
				new Instruction<byte> ("SWAP E", 8, (byte value) => {
				}), // 0x33
				new Instruction<byte> ("SWAP H", 8, (byte value) => {
				}), // 0x34
				new Instruction<byte> ("SWAP L", 8, (byte value) => {
				}), // 0x35
				new Instruction<byte> ("SWAP (HL)", 16, (byte value) => {
				}), // 0x36
				new Instruction<byte> ("SWAP A", 8, (byte value) => {
				}), // 0x37
				new Instruction<byte> ("SRL B", 8, (byte value) => {
				}), // 0x38
				new Instruction<byte> ("SRL C", 8, (byte value) => {
				}), // 0x39
				new Instruction<byte> ("SRL D", 8, (byte value) => {
				}), // 0x3A
				new Instruction<byte> ("SRL E", 8, (byte value) => {
				}), // 0x3B
				new Instruction<byte> ("SRL H", 8, (byte value) => {
				}), // 0x3C
				new Instruction<byte> ("SRL L", 8, (byte value) => {
				}), // 0x3D
				new Instruction<byte> ("SRL (HL)", 16, (byte value) => {
				}), // 0x3E
				new Instruction<byte> ("SRL A", 8, (byte value) => {
				}), // 0x3F
				#endregion

				#region 0x4*
				new Instruction<byte> ("BIT 0, B", 8, (byte value) => {
				}), // 0x40
				new Instruction<byte> ("BIT 0, C", 8, (byte value) => {
				}), // 0x41
				new Instruction<byte> ("BIT 0, D", 8, (byte value) => {
				}), // 0x42
				new Instruction<byte> ("BIT 0, E", 8, (byte value) => {
				}), // 0x43
				new Instruction<byte> ("BIT 0, H", 8, (byte value) => {
				}), // 0x44
				new Instruction<byte> ("BIT 0, L", 8, (byte value) => {
				}), // 0x45
				new Instruction<byte> ("BIT 0, (HL)", 12, (byte value) => {
				}), // 0x46
				new Instruction<byte> ("BIT 0, A", 8, (byte value) => {
				}), // 0x47
				new Instruction<byte> ("BIT 1, B", 8, (byte value) => {
				}), // 0x48
				new Instruction<byte> ("BIT 1, C", 8, (byte value) => {
				}), // 0x49
				new Instruction<byte> ("BIT 1, D", 8, (byte value) => {
				}), // 0x4A
				new Instruction<byte> ("BIT 1, E", 8, (byte value) => {
				}), // 0x4B
				new Instruction<byte> ("BIT 1, H", 8, (byte value) => {
				}), // 0x4C
				new Instruction<byte> ("BIT 1, L", 8, (byte value) => {
				}), // 0x4D
				new Instruction<byte> ("BIT 1, (HL)", 12, (byte value) => {
				}), // 0x4E
				new Instruction<byte> ("BIT 1, A", 8, (byte value) => {
				}), // 0x4F
				#endregion

				#region 0x5*
				new Instruction<byte> ("BIT 2, B", 8, (byte value) => {
				}), // 0x50
				new Instruction<byte> ("BIT 2, C", 8, (byte value) => {
				}), // 0x51
				new Instruction<byte> ("BIT 2, D", 8, (byte value) => {
				}), // 0x52
				new Instruction<byte> ("BIT 2, E", 8, (byte value) => {
				}), // 0x53
				new Instruction<byte> ("BIT 2, H", 8, (byte value) => {
				}), // 0x54
				new Instruction<byte> ("BIT 2, L", 8, (byte value) => {
				}), // 0x55
				new Instruction<byte> ("BIT 2, (HL)", 12, (byte value) => {
				}), // 0x56
				new Instruction<byte> ("BIT 2, A", 8, (byte value) => {
				}), // 0x57
				new Instruction<byte> ("BIT 3, B", 8, (byte value) => {
				}), // 0x58
				new Instruction<byte> ("BIT 3, C", 8, (byte value) => {
				}), // 0x59
				new Instruction<byte> ("BIT 3, D", 8, (byte value) => {
				}), // 0x5A
				new Instruction<byte> ("BIT 3, E", 8, (byte value) => {
				}), // 0x5B
				new Instruction<byte> ("BIT 3, H", 8, (byte value) => {
				}), // 0x5C
				new Instruction<byte> ("BIT 3, L", 8, (byte value) => {
				}), // 0x5D
				new Instruction<byte> ("BIT 3, (HL)", 12, (byte value) => {
				}), // 0x5E
				new Instruction<byte> ("BIT 3, A", 8, (byte value) => {
				}), // 0x5F
				#endregion

				#region 0x6*
				new Instruction<byte> ("BIT 4, B", 8, (byte value) => {
				}), // 0x60
				new Instruction<byte> ("BIT 4, C", 8, (byte value) => {
				}), // 0x61
				new Instruction<byte> ("BIT 4, D", 8, (byte value) => {
				}), // 0x62
				new Instruction<byte> ("BIT 4, E", 8, (byte value) => {
				}), // 0x63
				new Instruction<byte> ("BIT 4, H", 8, (byte value) => {
				}), // 0x64
				new Instruction<byte> ("BIT 4, L", 8, (byte value) => {
				}), // 0x65
				new Instruction<byte> ("BIT 4, (HL)", 12, (byte value) => {
				}), // 0x66
				new Instruction<byte> ("BIT 4, A", 8, (byte value) => {
				}), // 0x67
				new Instruction<byte> ("BIT 5, B", 8, (byte value) => {
				}), // 0x68
				new Instruction<byte> ("BIT 5, C", 8, (byte value) => {
				}), // 0x69
				new Instruction<byte> ("BIT 5, D", 8, (byte value) => {
				}), // 0x6A
				new Instruction<byte> ("BIT 5, E", 8, (byte value) => {
				}), // 0x6B
				new Instruction<byte> ("BIT 6, H", 8, (byte value) => {
				}), // 0x6C
				new Instruction<byte> ("BIT 6, L", 8, (byte value) => {
				}), // 0x6D
				new Instruction<byte> ("BIT 5, (HL)", 12, (byte value) => {
				}), // 0x6E
				new Instruction<byte> ("BIT 5, A", 8, (byte value) => {
				}), // 0x6F
				#endregion

				#region 0x7*
				new Instruction<byte> ("BIT 6, B", 8, (byte value) => {
				}), // 0x70
				new Instruction<byte> ("BIT 6, C", 8, (byte value) => {
				}), // 0x71
				new Instruction<byte> ("BIT 6, D", 8, (byte value) => {
				}), // 0x72
				new Instruction<byte> ("BIT 6, E", 8, (byte value) => {
				}), // 0x73
				new Instruction<byte> ("BIT 6, H", 8, (byte value) => {
				}), // 0x74
				new Instruction<byte> ("BIT 6, L", 8, (byte value) => {
				}), // 0x75
				new Instruction<byte> ("BIT 6, (HL)", 12, (byte value) => {
				}), // 0x76
				new Instruction<byte> ("BIT 6, A", 8, (byte value) => {
				}), // 0x77
				new Instruction<byte> ("BIT 7, B", 8, (byte value) => {
				}), // 0x78
				new Instruction<byte> ("BIT 7, C", 8, (byte value) => {
				}), // 0x79
				new Instruction<byte> ("BIT 7, D", 8, (byte value) => {
				}), // 0x7A
				new Instruction<byte> ("BIT 7, E", 8, (byte value) => {
				}), // 0x7B
				new Instruction<byte> ("BIT 7, H", 8, (byte value) => {
				}), // 0x7C
				new Instruction<byte> ("BIT 7, L", 8, (byte value) => {
				}), // 0x7D
				new Instruction<byte> ("BIT 7, (HL)", 12, (byte value) => {
				}), // 0x7E
				new Instruction<byte> ("BIT 7, A", 8, (byte value) => {
				}), // 0x7F
				#endregion

				#region 0x8*
				new Instruction<byte> ("RES 0, B", 8, (byte value) => {
				}), // 0x80
				new Instruction<byte> ("RES 0, C", 8, (byte value) => {
				}), // 0x81
				new Instruction<byte> ("RES 0, D", 8, (byte value) => {
				}), // 0x82
				new Instruction<byte> ("RES 0, E", 8, (byte value) => {
				}), // 0x83
				new Instruction<byte> ("RES 0, H", 8, (byte value) => {
				}), // 0x84
				new Instruction<byte> ("RES 0, L", 8, (byte value) => {
				}), // 0x85
				new Instruction<byte> ("RES 0, (HL)", 12, (byte value) => {
				}), // 0x86
				new Instruction<byte> ("RES 0, A", 8, (byte value) => {
				}), // 0x87
				new Instruction<byte> ("RES 1, B", 8, (byte value) => {
				}), // 0x88
				new Instruction<byte> ("RES 1, C", 8, (byte value) => {
				}), // 0x89
				new Instruction<byte> ("RES 1, D", 8, (byte value) => {
				}), // 0x8A
				new Instruction<byte> ("RES 1, E", 8, (byte value) => {
				}), // 0x8B
				new Instruction<byte> ("RES 1, H", 8, (byte value) => {
				}), // 0x8C
				new Instruction<byte> ("RES 1, L", 8, (byte value) => {
				}), // 0x8D
				new Instruction<byte> ("RES 1, (HL)", 12, (byte value) => {
				}), // 0x8E
				new Instruction<byte> ("RES 1, A", 8, (byte value) => {
				}), // 0x8F
				#endregion

				#region 0x9*
				new Instruction<byte> ("RES 2, B", 8, (byte value) => {
				}), // 0x90
				new Instruction<byte> ("RES 2, C", 8, (byte value) => {
				}), // 0x91
				new Instruction<byte> ("RES 2, D", 8, (byte value) => {
				}), // 0x92
				new Instruction<byte> ("RES 2, E", 8, (byte value) => {
				}), // 0x93
				new Instruction<byte> ("RES 2, H", 8, (byte value) => {
				}), // 0x94
				new Instruction<byte> ("RES 2, L", 8, (byte value) => {
				}), // 0x95
				new Instruction<byte> ("RES 2, (HL)", 12, (byte value) => {
				}), // 0x96
				new Instruction<byte> ("RES 2, A", 8, (byte value) => {
				}), // 0x97
				new Instruction<byte> ("RES 3, B", 8, (byte value) => {
				}), // 0x98
				new Instruction<byte> ("RES 3, C", 8, (byte value) => {
				}), // 0x99
				new Instruction<byte> ("RES 3, D", 8, (byte value) => {
				}), // 0x9A
				new Instruction<byte> ("RES 3, E", 8, (byte value) => {
				}), // 0x9B
				new Instruction<byte> ("RES 3, H", 8, (byte value) => {
				}), // 0x9C
				new Instruction<byte> ("RES 3, L", 8, (byte value) => {
				}), // 0x9D
				new Instruction<byte> ("RES 3, (HL)", 12, (byte value) => {
				}), // 0x9E
				new Instruction<byte> ("RES 3, A", 8, (byte value) => {
				}), // 0x9F
				#endregion

				#region 0xA*
				new Instruction<byte> ("RES 4, B", 8, (byte value) => {
				}), // 0xA0
				new Instruction<byte> ("RES 4, C", 8, (byte value) => {
				}), // 0xA1
				new Instruction<byte> ("RES 4, D", 8, (byte value) => {
				}), // 0xA2
				new Instruction<byte> ("RES 4, E", 8, (byte value) => {
				}), // 0xA3
				new Instruction<byte> ("RES 4, H", 8, (byte value) => {
				}), // 0xA4
				new Instruction<byte> ("RES 4, L", 8, (byte value) => {
				}), // 0xA5
				new Instruction<byte> ("RES 4, (HL)", 12, (byte value) => {
				}), // 0xA6
				new Instruction<byte> ("RES 4, A", 8, (byte value) => {
				}), // 0xA7
				new Instruction<byte> ("RES 5, B", 8, (byte value) => {
				}), // 0xA8
				new Instruction<byte> ("RES 5, C", 8, (byte value) => {
				}), // 0xA9
				new Instruction<byte> ("RES 5, D", 8, (byte value) => {
				}), // 0xAA
				new Instruction<byte> ("RES 5, E", 8, (byte value) => {
				}), // 0xAB
				new Instruction<byte> ("RES 5, H", 8, (byte value) => {
				}), // 0xAC
				new Instruction<byte> ("RES 5, L", 8, (byte value) => {
				}), // 0xAD
				new Instruction<byte> ("RES 5, (HL)", 12, (byte value) => {
				}), // 0xAE
				new Instruction<byte> ("RES 5, A", 8, (byte value) => {
				}), // 0xAF
				#endregion

				#region 0xB*
				new Instruction<byte> ("RES 6, B", 8, (byte value) => {
				}), // 0xB0
				new Instruction<byte> ("RES 6, C", 8, (byte value) => {
				}), // 0xB1
				new Instruction<byte> ("RES 6, D", 8, (byte value) => {
				}), // 0xB2
				new Instruction<byte> ("RES 6, E", 8, (byte value) => {
				}), // 0xB3
				new Instruction<byte> ("RES 6, H", 8, (byte value) => {
				}), // 0xB4
				new Instruction<byte> ("RES 6, L", 8, (byte value) => {
				}), // 0xB5
				new Instruction<byte> ("RES 6, (HL)", 12, (byte value) => {
				}), // 0xB6
				new Instruction<byte> ("RES 6, A", 8, (byte value) => {
				}), // 0xB7
				new Instruction<byte> ("RES 7, B", 8, (byte value) => {
				}), // 0xB8
				new Instruction<byte> ("RES 7, C", 8, (byte value) => {
				}), // 0xB9
				new Instruction<byte> ("RES 7, D", 8, (byte value) => {
				}), // 0xBA
				new Instruction<byte> ("RES 7, E", 8, (byte value) => {
				}), // 0xBB
				new Instruction<byte> ("RES 7, H", 8, (byte value) => {
				}), // 0xBC
				new Instruction<byte> ("RES 7, L", 8, (byte value) => {
				}), // 0xBD
				new Instruction<byte> ("RES 7, (HL)", 12, (byte value) => {
				}), // 0xBE
				new Instruction<byte> ("RES 7, A", 8, (byte value) => {
				}), // 0xBF
				#endregion

				#region 0xC*
				new Instruction<byte> ("SET 0, B", 8, (byte value) => {
				}), // 0xC0
				new Instruction<byte> ("SET 0, C", 8, (byte value) => {
				}), // 0xC1
				new Instruction<byte> ("SET 0, D", 8, (byte value) => {
				}), // 0xC2
				new Instruction<byte> ("SET 0, E", 8, (byte value) => {
				}), // 0xC3
				new Instruction<byte> ("SET 0, H", 8, (byte value) => {
				}), // 0xC4
				new Instruction<byte> ("SET 0, L", 8, (byte value) => {
				}), // 0xC5
				new Instruction<byte> ("SET 0, (HL)", 12, (byte value) => {
				}), // 0xC6
				new Instruction<byte> ("SET 0, A", 8, (byte value) => {
				}), // 0xC7
				new Instruction<byte> ("SET 1, B", 8, (byte value) => {
				}), // 0xC8
				new Instruction<byte> ("SET 1, C", 8, (byte value) => {
				}), // 0xC9
				new Instruction<byte> ("SET 1, D", 8, (byte value) => {
				}), // 0xCA
				new Instruction<byte> ("SET 1, E", 8, (byte value) => {
				}), // 0xCB
				new Instruction<byte> ("SET 1, H", 8, (byte value) => {
				}), // 0xCC
				new Instruction<byte> ("SET 1, L", 8, (byte value) => {
				}), // 0xCD
				new Instruction<byte> ("SET 1, (HL)", 12, (byte value) => {
				}), // 0xCE
				new Instruction<byte> ("SET 1, A", 8, (byte value) => {
				}), // 0xCF
				#endregion

				#region 0xD*
				new Instruction<byte> ("SET 2, B", 8, (byte value) => {
				}), // 0xD0
				new Instruction<byte> ("SET 2, C", 8, (byte value) => {
				}), // 0xD1
				new Instruction<byte> ("SET 2, D", 8, (byte value) => {
				}), // 0xD2
				new Instruction<byte> ("SET 2, E", 8, (byte value) => {
				}), // 0xD3
				new Instruction<byte> ("SET 2, H", 8, (byte value) => {
				}), // 0xD4
				new Instruction<byte> ("SET 2, L", 8, (byte value) => {
				}), // 0xD5
				new Instruction<byte> ("SET 2, (HL)", 12, (byte value) => {
				}), // 0xD6
				new Instruction<byte> ("SET 2, A", 8, (byte value) => {
				}), // 0xD7
				new Instruction<byte> ("SET 3, B", 8, (byte value) => {
				}), // 0xD8
				new Instruction<byte> ("SET 3, C", 8, (byte value) => {
				}), // 0xD9
				new Instruction<byte> ("SET 3, D", 8, (byte value) => {
				}), // 0xDA
				new Instruction<byte> ("SET 3, E", 8, (byte value) => {
				}), // 0xDB
				new Instruction<byte> ("SET 3, H", 8, (byte value) => {
				}), // 0xDC
				new Instruction<byte> ("SET 3, L", 8, (byte value) => {
				}), // 0xDD
				new Instruction<byte> ("SET 3, (HL)", 12, (byte value) => {
				}), // 0xDE
				new Instruction<byte> ("SET 3, A", 8, (byte value) => {
				}), // 0xDF
				#endregion

				#region 0xE*
				new Instruction<byte> ("SET 4, B", 8, (byte value) => {
				}), // 0xE0
				new Instruction<byte> ("SET 4, C", 8, (byte value) => {
				}), // 0xE1
				new Instruction<byte> ("SET 4, D", 8, (byte value) => {
				}), // 0xE2
				new Instruction<byte> ("SET 4, E", 8, (byte value) => {
				}), // 0xE3
				new Instruction<byte> ("SET 4, H", 8, (byte value) => {
				}), // 0xE4
				new Instruction<byte> ("SET 4, L", 8, (byte value) => {
				}), // 0xE5
				new Instruction<byte> ("SET 4, (HL)", 12, (byte value) => {
				}), // 0xE6
				new Instruction<byte> ("SET 4, A", 8, (byte value) => {
				}), // 0xE7
				new Instruction<byte> ("SET 5, B", 8, (byte value) => {
				}), // 0xE8
				new Instruction<byte> ("SET 5, C", 8, (byte value) => {
				}), // 0xE9
				new Instruction<byte> ("SET 5, D", 8, (byte value) => {
				}), // 0xEA
				new Instruction<byte> ("SET 5, E", 8, (byte value) => {
				}), // 0xEB
				new Instruction<byte> ("SET 5, H", 8, (byte value) => {
				}), // 0xEC
				new Instruction<byte> ("SET 5, L", 8, (byte value) => {
				}), // 0xED
				new Instruction<byte> ("SET 5, (HL)", 12, (byte value) => {
				}), // 0xEE
				new Instruction<byte> ("SET 5, A", 8, (byte value) => {
				}), // 0xEF
				#endregion

				#region 0xF*
				new Instruction<byte> ("SET 6, B", 8, (byte value) => {
				}), // 0xF0
				new Instruction<byte> ("SET 6, C", 8, (byte value) => {
				}), // 0xF1
				new Instruction<byte> ("SET 6, D", 8, (byte value) => {
				}), // 0xF2
				new Instruction<byte> ("SET 6, E", 8, (byte value) => {
				}), // 0xF3
				new Instruction<byte> ("SET 6, H", 8, (byte value) => {
				}), // 0xF4
				new Instruction<byte> ("SET 6, L", 8, (byte value) => {
				}), // 0xF5
				new Instruction<byte> ("SET 6, (HL)", 12, (byte value) => {
				}), // 0xF6
				new Instruction<byte> ("SET 6, A", 8, (byte value) => {
				}), // 0xF7
				new Instruction<byte> ("SET 7, B", 8, (byte value) => {
				}), // 0xF8
				new Instruction<byte> ("SET 7, C", 8, (byte value) => {
				}), // 0xF9
				new Instruction<byte> ("SET 7, D", 8, (byte value) => {
				}), // 0xFA
				new Instruction<byte> ("SET 7, E", 8, (byte value) => {
				}), // 0xFB
				new Instruction<byte> ("SET 7, H", 8, (byte value) => {
				}), // 0xFC
				new Instruction<byte> ("SET 7, L", 8, (byte value) => {
				}), // 0xFD
				new Instruction<byte> ("SET 7, (HL)", 12, (byte value) => {
				}), // 0xFE
				new Instruction<byte> ("SET 7, A", 8, (byte value) => {
				}), // 0xFF
				#endregion

			};
		}
	}
}