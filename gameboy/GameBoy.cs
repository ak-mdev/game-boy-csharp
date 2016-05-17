using System;
using GameBoy.CPU;
using GameBoy.Graphics;
using GameBoy.Input;
using GameBoy.IO;
using GameBoy.Memory;
using GameBoy.Sound;

namespace GameBoy
{
	public class GameBoy
	{
		//private Random random = new Random ();
		private bool stopped = false;

		public Gpu Gpu { get; set; }

		public Cpu Cpu { get; set; }

		public Ram Memory { get; set; }

		public InputHandler Input { get; set; }

		public SoundHandler Sound { get; set; }

		public bool Running { get; set; }

		public GameBoy (string filename)
		{
			var rom = new Rom (filename);
			Memory = new Ram (rom);
			Cpu = new Cpu (Memory);
		}

		public void Start ()
		{
			Reset ();

			while (Running) {
				Cpu.Step ();
				Gpu.Step (Cpu);
				Cpu.Interrupt.Step ();
			}
		}

		public void Reset ()
		{
			Memory.Reset ();
			Cpu.Reset ();
			Input.Reset ();
			Gpu.Reset ();

			Running = true;
		}
	}
}

