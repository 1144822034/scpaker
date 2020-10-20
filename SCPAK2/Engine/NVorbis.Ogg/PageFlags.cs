using System;

namespace NVorbis.Ogg
{
	[Flags]
	internal enum PageFlags
	{
		None = 0x0,
		ContinuesPacket = 0x1,
		BeginningOfStream = 0x2,
		EndOfStream = 0x4
	}
}
