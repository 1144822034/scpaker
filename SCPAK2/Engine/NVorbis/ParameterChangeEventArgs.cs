using System;

namespace NVorbis
{
	internal class ParameterChangeEventArgs : EventArgs
	{
		public DataPacket FirstPacket
		{
			get;
			set;
		}

		public ParameterChangeEventArgs(DataPacket firstPacket)
		{
			FirstPacket = firstPacket;
		}
	}
}
