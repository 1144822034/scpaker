using System;

namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkICCP : PngChunkSingle
	{
		public const string ID = "iCCP";

		public string profileName;

		public byte[] compressedProfile;

		public PngChunkICCP(ImageInfo info)
			: base("iCCP", info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = createEmptyChunk(profileName.Length + compressedProfile.Length + 2, alloc: true);
			Array.Copy(ChunkHelper.ToBytes(profileName), 0, chunkRaw.Data, 0, profileName.Length);
			chunkRaw.Data[profileName.Length] = 0;
			chunkRaw.Data[profileName.Length + 1] = 0;
			Array.Copy(compressedProfile, 0, chunkRaw.Data, profileName.Length + 2, compressedProfile.Length);
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw chunk)
		{
			int num = ChunkHelper.PosNullByte(chunk.Data);
			profileName = PngHelperInternal.charsetLatin1.GetString(chunk.Data, 0, num);
			if ((chunk.Data[num + 1] & 0xFF) != 0)
			{
				throw new Exception("bad compression for ChunkTypeICCP");
			}
			int num2 = chunk.Data.Length - (num + 2);
			compressedProfile = new byte[num2];
			Array.Copy(chunk.Data, num + 2, compressedProfile, 0, num2);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkICCP pngChunkICCP = (PngChunkICCP)other;
			profileName = pngChunkICCP.profileName;
			compressedProfile = new byte[pngChunkICCP.compressedProfile.Length];
			Array.Copy(pngChunkICCP.compressedProfile, compressedProfile, compressedProfile.Length);
		}

		public void SetProfileNameAndContent(string name, string profile)
		{
			SetProfileNameAndContent(name, ChunkHelper.ToBytes(profileName));
		}

		public void SetProfileNameAndContent(string name, byte[] profile)
		{
			profileName = name;
			compressedProfile = ChunkHelper.compressBytes(profile, compress: true);
		}

		public string GetProfileName()
		{
			return profileName;
		}

		public byte[] GetProfile()
		{
			return ChunkHelper.compressBytes(compressedProfile, compress: false);
		}

		public string GetProfileAsString()
		{
			return ChunkHelper.ToString(GetProfile());
		}
	}
}
