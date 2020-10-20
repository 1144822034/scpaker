namespace Hjg.Pngcs.Chunks
{
	internal class ChunkPredicateId : ChunkPredicate
	{
		public readonly string id;

		public ChunkPredicateId(string id)
		{
			this.id = id;
		}

		public bool Matches(PngChunk c)
		{
			return c.Id.Equals(id);
		}
	}
}
