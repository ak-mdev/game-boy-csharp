using System;

namespace gameboy
{
	public class GameBoy
	{
		private Random random = new Random ();
		private bool stopped = false;

		public Display Display { get; set; }

		public CPU Cpu { get; set; }

		public Memory Memory { get; set; }

		public Input Input { get; set; }

		public Sound Sound { get; set; }

		public GameBoy (string filename)
		{
			var rom = new Rom (filename);
			Memory = new Memory (rom);
			Cpu = new CPU (Memory);
		}

		public void Start ()
		{
			Reset ();

			while (!stopped) {
				Cpu.Step ();
				//Display.Step ();
				//Cpu.Interrupt ();
			}

			Display.Dispose ();
		}

		public void Reset ()
		{
		}
	}
}

