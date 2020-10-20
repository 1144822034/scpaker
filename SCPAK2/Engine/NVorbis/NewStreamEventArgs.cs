using System;

namespace NVorbis
{
	internal class NewStreamEventArgs : EventArgs
	{
		public IPacketProvider PacketProvider
		{
			get;
			set;
		}

		public bool IgnoreStream
		{
			get;
			set;
		}

		public NewStreamEventArgs(IPacketProvider packetProvider)
		{
			if (packetProvider == null)
			{
				throw new ArgumentNullException("packetProvider");
			}
			PacketProvider = packetProvider;
		}
	}
}
