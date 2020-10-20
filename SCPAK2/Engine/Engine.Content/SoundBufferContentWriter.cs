using Engine.Media;
using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Content
{
	[ContentWriter("Engine.Audio.SoundBuffer")]
	public class SoundBufferContentWriter : IContentWriter
	{
		public string Sound;

		[Optional]
		public bool OggCompressed;

		public IEnumerable<string> GetDependencies()
		{
			yield return Sound;
		}

		public void Write(string projectDirectory, Stream stream)
		{
			using (Stream stream2 = Storage.OpenFile(Storage.CombinePaths(projectDirectory, Sound), OpenFileMode.Read))
			{
				if (Ogg.IsOggStream(stream2))
				{
					StreamingSource streamingSource = Ogg.Stream(stream2);
					if (OggCompressed)
					{
						if (stream2.Length > int.MaxValue)
						{
							throw new InvalidOperationException("Audio data too long.");
						}
						BinaryWriter binaryWriter = new BinaryWriter(stream);
						binaryWriter.Write(OggCompressed);
						binaryWriter.Write(streamingSource.ChannelsCount);
						binaryWriter.Write(streamingSource.SamplingFrequency);
						binaryWriter.Write((int)stream2.Length);
						stream2.Position = 0L;
						stream2.CopyTo(stream);
					}
					else
					{
						WritePcm(stream, streamingSource);
					}
				}
				else
				{
					if (!Wav.IsWavStream(stream2))
					{
						throw new InvalidOperationException("Unrecognized sound format.");
					}
					if (OggCompressed)
					{
						throw new InvalidOperationException("Ogg compression not available for WAV format sound files.");
					}
					WritePcm(stream, Wav.Stream(stream2));
				}
			}
		}

		public static void WritePcm(Stream stream, StreamingSource streamingSource)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(value: false);
			binaryWriter.Write(streamingSource.ChannelsCount);
			binaryWriter.Write(streamingSource.SamplingFrequency);
			long position = stream.Position;
			binaryWriter.Write(0);
			long position2 = stream.Position;
			streamingSource.CopyTo(stream);
			long position3 = stream.Position;
			long num = position3 - position2;
			if (num > int.MaxValue)
			{
				throw new InvalidOperationException("Audio data too long.");
			}
			stream.Position = position;
			binaryWriter.Write((int)num);
			stream.Position = position3;
		}
	}
}
