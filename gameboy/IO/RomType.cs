using System;

namespace GameBoy.IO
{
	public enum RomType : byte
	{
		Invalid = 0xdb,
		Plain = 0x00,
		Mbc1 = 0x01,
		Mbc1Ram = 0x02,
		Mbc1RamBatt = 0x03,
		Mbc2 = 0x05,
		Mbc2Battery = 0x06,
		Ram = 0x08,
		RamBattery = 0x09,
		Mmm01 = 0x0b,
		Mmm01Sram = 0x0c,
		Mmm01SramBatt = 0x0d,
		Mbc3TimerBatt = 0x0f,
		Mbc3TimerRamBatt = 0x10,
		Mbc3 = 0x11,
		Mbc3Ram = 0x12,
		Mbc3RamBatt = 0x13,
		Mbc5 = 0x19,
		Mbc5Ram = 0x1a,
		Mbc5RamBatt = 0x1b,
		Mbc5Rumble = 0x1c,
		Mbc5RumbleSram = 0x1d,
		Mbc5RumbleSramBatt = 0x1e,
		PocketCamera = 0x1f,
		BandaiTama5 = 0xfd,
		HudsonHuc3 = 0xfe,
		HudsonHuc1 = 0xff,
	}
}

