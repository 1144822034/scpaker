using Engine.Media;
using System;

namespace Engine.Content
{
	[ContentReader("Engine.Media.StreamingSource")]
	public class StreamingSourceContentReader : IContentReader
	{
		public object Read(ContentStream stream, object existingObject)
		{
			if (existingObject == null)
			{
				SoundFileFormat format = SoundData.DetermineFileFormat(stream);
				return SoundData.Stream(stream.Duplicate(), format);
			}
			throw new NotSupportedException();
		}
	}
}
