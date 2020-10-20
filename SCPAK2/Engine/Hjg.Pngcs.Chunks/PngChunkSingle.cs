namespace Hjg.Pngcs.Chunks
{
	internal abstract class PngChunkSingle : PngChunk
	{
		public PngChunkSingle(string id, ImageInfo imgInfo)
			: base(id, imgInfo)
		{
		}

		public sealed override bool AllowsMultiple()
		{
			return false;
		}

		public override int GetHashCode()
		{
			int num = 1;
			return 31 * num + ((Id != null) ? Id.GetHashCode() : 0);
		}

		public override bool Equals(object obj)
		{
			if (obj is PngChunkSingle && Id != null)
			{
				return Id.Equals(((PngChunkSingle)obj).Id);
			}
			return false;
		}
	}
}
