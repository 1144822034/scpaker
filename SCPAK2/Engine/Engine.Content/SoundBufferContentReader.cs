using Engine.Audio;
using Engine.Media;
using System;
using System.IO;

namespace Engine.Content
{
	[ContentReader("Engine.Audio.SoundBuffer")]
	public class SoundBufferContentReader : IContentReader
	{
		public object Read(ContentStream stream, object existingObject)
		{
			if (existingObject == null)
			{
				BinaryReader binaryReader = new BinaryReader(stream);
				bool flag = binaryReader.ReadBoolean();
				int channelsCount = binaryReader.ReadInt32();
				int samplingFrequency = binaryReader.ReadInt32();
				int bytesCount = binaryReader.ReadInt32();
				if (flag)
				{
					MemoryStream memoryStream = new MemoryStream();
					using (StreamingSource streamingSource = Ogg.Stream(stream))
					{
						streamingSource.CopyTo(memoryStream);
						if (memoryStream.Length > int.MaxValue)
						{
							throw new InvalidOperationException("Audio data too long.");
						}
						memoryStream.Position = 0L;
						return new SoundBuffer(memoryStream, (int)memoryStream.Length, channelsCount, samplingFrequency);
					}
				}
				return new SoundBuffer(stream, bytesCount, channelsCount, samplingFrequency);
			}
			throw new NotSupportedException();
		}
	}
}
