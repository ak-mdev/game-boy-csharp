using System;

namespace GameBoy.Graphics
{
	public class Sprite
	{
		/*

struct sprite {
	#ifdef LITTLE_E
		unsigned char y;
		unsigned char x;
		unsigned char tile;
		struct options {
				unsigned char priority : 1;
				unsigned char vFlip : 1;
				unsigned char hFlip : 1;
				unsigned char palette : 1;
		};
	#else
		unsigned char y;
		unsigned char x;
		unsigned char tile;
		struct options {
			//unsigned char dummy : 4;
			unsigned char palette : 1;
			unsigned char hFlip : 1;
			unsigned char vFlip : 1;
			unsigned char priority : 1;
		};
	#endif
};
		 */
		public Sprite ()
		{
		}
	}
}

