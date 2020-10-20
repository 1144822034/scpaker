using System;

namespace NVorbis
{
	internal interface IVorbisStreamStatus
	{
		int EffectiveBitRate
		{
			get;
		}

		int InstantBitRate
		{
			get;
		}

		TimeSpan PageLatency
		{
			get;
		}

		TimeSpan PacketLatency
		{
			get;
		}

		TimeSpan SecondLatency
		{
			get;
		}

		long OverheadBits
		{
			get;
		}

		long AudioBits
		{
			get;
		}

		int PagesRead
		{
			get;
		}

		int TotalPages
		{
			get;
		}

		bool Clipped
		{
			get;
		}

		void ResetStats();
	}
}
