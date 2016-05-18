using System;

namespace GameBoy.Input
{
	public class InputHandler
	{
		public Keys Keys { get; set; }

		public InputHandler ()
		{
			Keys = new Keys ();
		}

		public void Reset ()
		{
			Keys.A = 1;
			Keys.B = 1;
			Keys.Select = 1;
			Keys.Start = 1;
			Keys.Right = 1;
			Keys.Left = 1;
			Keys.Up = 1;
			Keys.Down = 1;
		}
	}
}

