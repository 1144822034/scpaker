namespace Hjg.Pngcs.Chunks
{
	internal interface ChunkPredicate
	{
		bool Matches(PngChunk chunk);
	}
}
