using System;

namespace GameBoy.CPU
{
	public enum Interrupts : byte
	{
		VBlank = (1 << 0),
		LcdStat = (1 << 1),
		Timer = (1 << 2),
		Serial = (1 << 3),
		Joypad = (1 << 4)
	}

	public delegate void Handler (object sender, EventArgs e);
	public class Interrupt
	{
		public byte Master { get; set; }

		public byte Enable { get; set; }

		public byte Flags { get; set; }

		public void Step ()
		{
			if (Master != 0 && Enable != 0 && Flags != 0) {
				var fire = Enable & Flags;

				if ((fire & (byte)Interrupts.VBlank) != 0) {
					Flags &= (byte)(~Interrupts.VBlank);
					OnVBlank ();
				}

				if ((fire & (byte)Interrupts.LcdStat) != 0) {
					Flags &= (byte)(~Interrupts.LcdStat);
					OnLcdStat ();
				}

				if ((fire & (byte)Interrupts.Timer) != 0) {
					Flags &= (byte)(~Interrupts.Timer);
					OnTimer ();
				}

				if ((fire & (byte)Interrupts.Serial) != 0) {
					Flags &= (byte)(~Interrupts.Serial);
					OnSerial ();
				}

				if ((fire & (byte)Interrupts.Joypad) != 0) {
					Flags &= (byte)(~Interrupts.Joypad);
					OnJoypad ();
				}
			}
		}

		public event Handler VBlank;
		public event Handler LcdStat;
		public event Handler Timer;
		public event Handler Serial;
		public event Handler Joypad;
		public event Handler Return;

		public void OnVBlank ()
		{
			if (VBlank != null)
				VBlank (this, new EventArgs ());
		}

		public void OnLcdStat ()
		{
			if (LcdStat != null)
				LcdStat (this, new EventArgs ());
		}

		public void OnTimer ()
		{
			if (Timer != null)
				Timer (this, new EventArgs ());
		}

		public void OnSerial ()
		{
			if (Serial != null)
				Serial (this, new EventArgs ());
		}

		public void OnJoypad ()
		{
			if (Joypad != null)
				Joypad (this, new EventArgs ());
		}

		public void OnReturn ()
		{
			if (Return != null)
				Return (this, new EventArgs ());
		}
	}
}

