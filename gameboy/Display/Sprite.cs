using System;
using System.Collections.Generic;

namespace GameBoy.Graphics
{
	public class Sprite
	{
		public byte X { get; set; }

		public byte Y { get; set; }

		public byte Tile { get; set; }

		public SpriteOptions Options { get; set; }

		public Sprite ()
		{
			Options = new SpriteOptions ();
		}

		public static IEnumerable<Sprite> FromBytes (byte[] bytes)
		{
			for (int i = 0; i < bytes.Length - 7; i++) {
				var sprite = new Sprite ();
				sprite.Y = bytes [i++];
				sprite.X = bytes [i++];
				sprite.Tile = bytes [i++];
				sprite.Options.Palette = bytes [i++];
				sprite.Options.HFlip = bytes [i++];
				sprite.Options.VFlip = bytes [i++];
				sprite.Options.Priority = bytes [i];
				yield return sprite;
			}
		}
	}
}

