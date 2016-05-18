using System;

namespace GameBoy.Graphics
{
	public class SpriteOptions
	{
		public byte Priority { get; set; }

		public byte VFlip { get; set; }

		public byte HFlip { get; set; }

		public byte Palette { get; set; }

		public SpriteOptions ()
		{
			Priority = 1;
			VFlip = 1;
			HFlip = 1;
			Palette = 1;
		}
	}
}

