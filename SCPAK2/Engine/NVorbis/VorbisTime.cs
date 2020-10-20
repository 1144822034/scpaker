using System.IO;

namespace NVorbis
{
	internal abstract class VorbisTime
	{
		public class Time0 : VorbisTime
		{
			internal Time0(VorbisStreamDecoder vorbis)
				: base(vorbis)
			{
			}

			public override void Init(DataPacket packet)
			{
			}
		}

		internal static VorbisTime Init(VorbisStreamDecoder vorbis, DataPacket packet)
		{
			int num = (int)packet.ReadBits(16);
			VorbisTime vorbisTime = null;
			if (num == 0)
			{
				vorbisTime = new Time0(vorbis);
			}
			if (vorbisTime == null)
			{
				throw new InvalidDataException();
			}
			vorbisTime.Init(packet);
			return vorbisTime;
		}

		public VorbisTime(VorbisStreamDecoder vorbis)
		{
		}

		public abstract void Init(DataPacket packet);
	}
}
