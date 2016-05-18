using System;
using GameBoy.CPU;
using GameBoy.Graphics;
using GameBoy.Input;
using GameBoy.IO;
using GameBoy.Memory;
using GameBoy.Sound;
using OpenTK;
using OpenTK.Input;

namespace GameBoy
{
	public class GameBoy
	{
		public Gpu Gpu { get; set; }

		public Cpu Cpu { get; set; }

		public Interrupt Interrupt { get; set; }

		public Ram Memory { get; set; }

		public InputHandler Input { get; set; }

		public SoundHandler Sound { get; set; }

		public bool Running { get; set; }

		public GameBoy (string filename)
		{
			Interrupt = new Interrupt ();

			Interrupt.VBlank += OnVBlank;
			Interrupt.LcdStat += OnLcdStat;
			Interrupt.Timer += OnTimer;
			Interrupt.Serial += OnSerial;
			Interrupt.Joypad += OnJoypad;

			Gpu = new Gpu (Interrupt);
			Input = new InputHandler ();
			var rom = new Rom (filename);
			Memory = new Ram (rom, Gpu, Input, Interrupt);
			Cpu = new Cpu ();
		}

		public void Start ()
		{
			Memory.Rom.Loaded += (sender, e) => {
				Reset ();

				Gpu.Display.UpdateFrame += OnUpdate;
				Gpu.Display.KeyDown += OnKeyDown;
				Gpu.Display.KeyUp += OnKeyUp;

				Gpu.Display.Run (60.0);
			};
			Memory.Rom.Load ();
		}

		public void Reset ()
		{
			Interrupt.Master = 1;
			Interrupt.Enable = 0;
			Interrupt.Flags = 0;

			Memory.Reset ();
			Cpu.Reset (Memory, Interrupt);
			Input.Reset ();
			Gpu.Reset ();

			Running = true;
		}

		public void OnUpdate (object sender, FrameEventArgs e)
		{
			if (!Running) {
				return;
			}
			if (Debug.Enabled) {
				Debug.RealtimeDebug (Memory, Cpu.InstructionSet, Interrupt);
			}
			Cpu.Step (Memory);
			Gpu.Step (Cpu.Ticks);
			Interrupt.Step ();
		}

		public void OnKeyDown (object sender, KeyboardKeyEventArgs e)
		{
			if (e.Key == Key.Escape) {
				Console.WriteLine ("ESC pressed, quitting!");
				Debug.PrintRegisters (Memory, Interrupt);
				Program.Quit ();
				//PostQuitMessage(0);
			} else if (e.Key == Key.X) {
				Input.Keys.A = 0;
				Running = true;
			} else if (e.Key == Key.Z) {
				Input.Keys.B = 0;
				Running = true;
			} else if (e.Key == Key.Back) {
				Input.Keys.Select = 0;
				Running = true;
			} else if (e.Key == Key.Enter) {
				Input.Keys.Start = 0;
				Running = true;
			} else if (e.Key == Key.Up) {
				Input.Keys.Up = 0;
				Running = true;
			} else if (e.Key == Key.Down) {
				Input.Keys.Down = 0;
				Running = true;
			} else if (e.Key == Key.Left) {
				Input.Keys.Left = 0;
				Running = true;
			} else if (e.Key == Key.Right) {
				Input.Keys.Right = 0;
				Running = true;
			} else if (e.Key == Key.Minus) {
				Gpu.Display.Close ();
				Start ();
			} else if (e.Key == Key.Space) {
				Debug.Enabled = true;
			}
		}

		public void OnKeyUp (object sender, KeyboardKeyEventArgs e)
		{
			if (e.Key == Key.X) {
				Input.Keys.A = 1;
				Running = true;
			} else if (e.Key == Key.Z) {
				Input.Keys.B = 1;
				Running = true;
			} else if (e.Key == Key.Back) {
				Input.Keys.Select = 1;
				Running = true;
			} else if (e.Key == Key.Enter) {
				Input.Keys.Start = 1;
				Running = true;
			} else if (e.Key == Key.Up) {
				Input.Keys.Up = 1;
				Running = true;
			} else if (e.Key == Key.Down) {
				Input.Keys.Down = 1;
				Running = true;
			} else if (e.Key == Key.Left) {
				Input.Keys.Left = 1;
				Running = true;
			} else if (e.Key == Key.Right) {
				Input.Keys.Right = 1;
				Running = true;
			}
		}

		public void OnVBlank (object sender, EventArgs e)
		{
			Gpu.Display.DrawFrameBuffer ();

			Interrupt.Master = 0;
			Memory.WriteToStack (Memory.Registers.PC);
			Memory.Registers.PC = 0x40;

			Cpu.Ticks += 12;
		}

		public void OnLcdStat (object sender, EventArgs e)
		{
			Interrupt.Master = 0;
			Memory.WriteToStack (Memory.Registers.PC);
			Memory.Registers.PC = 0x48;

			Cpu.Ticks += 12;
		}

		public void OnTimer (object sender, EventArgs e)
		{
			Interrupt.Master = 0;
			Memory.WriteToStack (Memory.Registers.PC);
			Memory.Registers.PC = 0x50;

			Cpu.Ticks += 12;
		}

		public void OnSerial (object sender, EventArgs e)
		{
			Interrupt.Master = 0;
			Memory.WriteToStack (Memory.Registers.PC);
			Memory.Registers.PC = 0x58;

			Cpu.Ticks += 12;
		}

		public void OnJoypad (object sender, EventArgs e)
		{
			Interrupt.Master = 0;
			Memory.WriteToStack (Memory.Registers.PC);
			Memory.Registers.PC = 0x60;

			Cpu.Ticks += 12;
		}

		public void OnReturn (object sender, EventArgs e)
		{
			Interrupt.Master = 1;
			Memory.Registers.PC = Memory.ReadFromStack ();
		}
	}
}

