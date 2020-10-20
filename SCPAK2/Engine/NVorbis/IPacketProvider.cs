using System;

namespace NVorbis
{
	internal interface IPacketProvider : IDisposable
	{
		int StreamSerial
		{
			get;
		}

		bool CanSeek
		{
			get;
		}

		long ContainerBits
		{
			get;
		}

		event EventHandler<ParameterChangeEventArgs> ParameterChange;

		int GetTotalPageCount();

		DataPacket GetNextPacket();

		DataPacket PeekNextPacket();

		DataPacket GetPacket(int packetIndex);

		long GetGranuleCount();

		DataPacket FindPacket(long granulePos, Func<DataPacket, DataPacket, int> packetGranuleCountCallback);

		void SeekToPacket(DataPacket packet, int preRoll);
	}
}
