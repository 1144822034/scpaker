namespace Hjg.Pngcs.Chunks
{
	internal class ChunkPredicateId2 : ChunkPredicate
	{
		public readonly string id;

		public readonly string innerid;

		public ChunkPredicateId2(string id, string inner)
		{
			this.id = id;
			innerid = inner;
		}

		public bool Matches(PngChunk c)
		{
			if (!c.Id.Equals(id))
			{
				return false;
			}
			if (c is PngChunkTextVar && !((PngChunkTextVar)c).GetKey().Equals(innerid))
			{
				return false;
			}
			if (c is PngChunkSPLT && !((PngChunkSPLT)c).PalName.Equals(innerid))
			{
				return false;
			}
			return true;
		}
	}
}
