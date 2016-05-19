using System;
using System.IO;
using System.Text;

namespace GameBoy.IO
{
	public delegate void LoadedEventHandler (object sender, EventArgs e);

	public class Rom
	{
		private static readonly byte[] NINTENDO_BITS = new byte[] {
			0xCE, 0xED, 0x66, 0x66, 0xCC, 0x0D, 0x00, 0x0B, 0x03, 0x73, 0x00, 0x83, 0x00, 0x0C, 0x00, 0x0D,
			0x00, 0x08, 0x11, 0x1F, 0x88, 0x89, 0x00, 0x0E, 0xDC, 0xCC, 0x6E, 0xE6, 0xDD, 0xDD, 0xD9, 0x99,
			0xBB, 0xBB, 0x67, 0x63, 0x6E, 0x0E, 0xEC, 0xCC, 0xDD, 0xDC, 0x99, 0x9F, 0xBB, 0xB9, 0x33, 0x3E
		};

		public event LoadedEventHandler Loaded;
		public event LoadedEventHandler Cancelled;

		private string filename;

		public string Name { get; set; }

		public RomType Type { get; set; }

		public int RomSize { get; set; }

		public int RamSize { get; set; }

		public byte[] Cart { get; set; }

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
				if (stream.Length < 0x150) {
					Console.WriteLine ("ROM is too small!");
					OnLoaded (false, new EventArgs ());
					return;
				}
				stream.Position = 0;

				byte[] header = new byte[0x150];
				stream.Read (header, 0, header.Length);

				Console.WriteLine ("{0} the Nintendo Logo test!", CheckNintendoHeader (header) ? "Passes" : "Fails");
				Name = GetROMName (header);
				Console.WriteLine ("Internal ROM name: {0}", Name);

				Type = GetROMType (header);
				Console.WriteLine ("ROM type: {0}", Type.ToString ());
				if (Type != RomType.Plain) {
					Console.WriteLine ("ERROR: Only 32KB games with no mappers are supported!");
					OnLoaded (false, new EventArgs ());
					return;
				}

				RomSize = GetROMSize (header);
				Console.WriteLine ("ROM size: {0}KB", RomSize * 16);

				if (RomSize * 16 != 32) {
					Console.WriteLine ("ERROR: Only 32KB games with no mappers are supported!");
					OnLoaded (false, new EventArgs ());
				} else if (stream.Length != RomSize * 16 * 1024) {
					Console.WriteLine ("ERROR: ROM filesize does not equal ROM size!");
					OnLoaded (false, new EventArgs ());
					return;
				}

				RamSize = GetRAMSize (header);
				Console.WriteLine ("RAM size: {0}", RamSize);

				Console.WriteLine ("{0} the header checksum!", CheckChecksum (header) ? "Passes" : "Fails");

				stream.Position = 0;

				Cart = new byte[stream.Length];
				stream.Read (Cart, 0, Cart.Length);
			}
			OnLoaded (true, new EventArgs ());
		}

		private string GetROMName (byte[] header)
		{
			var nameBuilder = new StringBuilder ();
			for (int i = 0; i < 16; i++) {
				if (header [i + RomOffsets.NAME] == 0x80 || header [i + RomOffsets.NAME] == 0xc0) {
					nameBuilder.Append (' ');
				} else {
					nameBuilder.Append (Convert.ToChar (header [i + RomOffsets.NAME]));
				}
			}
			return nameBuilder.ToString ();
		}

		private RomType GetROMType (byte[] header)
		{
			var typeByte = header [RomOffsets.TYPE];
			if (Enum.IsDefined (typeof(RomType), typeByte)) {
				return (RomType)typeByte;
			} else {
				Console.WriteLine ("ERROR: Unknown ROM type: {0}", Type.ToString ());
				OnLoaded (false, new EventArgs ());
				return RomType.Invalid;
			}
		}

		private int GetROMSize (byte[] header)
		{
			var romSize = (int)header [RomOffsets.ROM_SIZE];
			if ((RomSize & 0xf0) == 0x50) {
				romSize = (int)Math.Pow (2.0, (double)(((0x52) & 0xf) + 1)) + 64;
			} else {
				romSize = (int)Math.Pow (2.0, (double)(RomSize + 1));
			}
			return romSize;
		}

		private int GetRAMSize (byte[] header)
		{
			var ramSize = (int)header [RomOffsets.RAM_SIZE];
			ramSize = (int)Math.Pow (4.0, (double)RamSize) / 2;
			ramSize = (int)Math.Ceiling (RamSize / 8.0f);
			return ramSize;
		}

		private bool CheckNintendoHeader (byte[] header)
		{
			for (int i = 0; i < NINTENDO_BITS.Length; i++) {
				if (header [i + RomOffsets.NINTENDO] != NINTENDO_BITS [i]) {
					return false;
				}
			}
			return true;
		}

		private bool CheckChecksum (byte[] header)
		{
			var checksum = header [RomOffsets.CHECKSUM];

			ushort x = 0;
			var max = RomOffsets.CHECKSUM - 1;
			for (var i = 0x134; i < max; i++) {
				x = (ushort)(x - header [i] - 1);
			}
			Console.WriteLine ("x={0};checksum={1}", x, checksum);
			return (checksum == x);
		}
	}
}