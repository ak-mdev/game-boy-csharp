using System;
using System.IO;
using System.Text;

namespace GameBoy.IO
{
	public delegate void LoadedEventHandler (object sender, EventArgs e);

	public class Rom
	{
		public event LoadedEventHandler Loaded;
		public event LoadedEventHandler Cancelled;

		private string filename;

		public string Name { get; set; }

		public RomType Type { get; set; }

		public int RomSize { get; set; }

		public int RamSize { get; set; }

		public byte[] Cart {get;set;}

		public Rom (string file)
		{
			filename = file;

		}

		protected virtual void OnLoaded (bool success, EventArgs e)
		{
			if (success) {
				if (Loaded != null) {
					Loaded (this, e);
				}
			} else {
				if (Cancelled != null) {
					Cancelled (this, e);
				}
			}
		}

		public void Load ()
		{
			using (var stream = new FileStream (filename, FileMode.Open)) {
				if (stream.Length < 0x180) {
					Console.WriteLine ("ROM is too small!");
					OnLoaded (false, new EventArgs ());
					return;
				}
				stream.Position = 0;

				byte[] header = new byte[0x180];
				stream.Read (header, 0, header.Length);
				var nameBuilder = new StringBuilder ();
				for (int i = 0; i < 16; i++) {
					if (header [i + RomOffsets.NAME] == 0x80 || header [i + RomOffsets.NAME] == 0xc0) {
						nameBuilder.Append (' ');
					} else {
						nameBuilder.Append (header [i + RomOffsets.NAME]);
					}
				}
				Name = nameBuilder.ToString ();
				Console.WriteLine ("Internal ROM name: {0}", Name);

				var typeByte = header [RomOffsets.TYPE];
				if (Enum.IsDefined (typeof(RomType), typeByte)) {
					Type = (RomType)typeByte;
				} else {
					Console.WriteLine ("ERROR: Unknown ROM type: {0}", Type.ToString ());
					OnLoaded (false, new EventArgs ());
					return;
				}
				Console.WriteLine ("ROM type: {0}", Type.ToString ());
				if (Type != RomType.Plain) {
					Console.WriteLine ("ERROR: Only 32KB games with no mappers are supported!");
					OnLoaded (false, new EventArgs ());
					return;
				}

				RomSize = header [RomOffsets.ROM_SIZE];
				if ((RomSize & 0xf0) == 0x50) {
					RomSize = (int)Math.Pow (2.0, (double)(((0x52) & 0xf) + 1)) + 64;
				} else {
					RomSize = (int)Math.Pow (2.0, (double)(RomSize + 1));
				}
				Console.WriteLine ("ROM size: {0}KB", RomSize * 16);

				if (RomSize * 16 != 32) {
					Console.WriteLine ("ERROR: Only 32KB games with no mappers are supported!");
					OnLoaded (false, new EventArgs ());
				} else if (stream.Length != RomSize * 16 * 1024) {
					Console.WriteLine ("ERROR: ROM filesize does not equal ROM size!");
					OnLoaded (false, new EventArgs ());
					return;
				}

				RamSize = header [RomOffsets.RAM_SIZE];
				RamSize = (int)Math.Pow (4.0, (double)RamSize) / 2;
				Console.WriteLine ("RAM size: {0}", RamSize);
				RamSize = (int)Math.Ceiling (RamSize / 8.0f);

				stream.Position = 0;

				Cart = new byte[stream.Length];
				stream.Read (Cart, 0, Cart.Length);
			}
			OnLoaded (true, new EventArgs ());
		}
	}
}

