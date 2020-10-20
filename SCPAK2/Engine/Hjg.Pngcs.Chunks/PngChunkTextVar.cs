namespace Hjg.Pngcs.Chunks
{
	internal abstract class PngChunkTextVar : PngChunkMultiple
	{
		public class PngTxtInfo
		{
			public string title;

			public string author;

			public string description;

			public string creation_time;

			public string software;

			public string disclaimer;

			public string warning;

			public string source;

			public string comment;
		}

		internal string key;

		internal string val;

		public const string KEY_Title = "Title";

		public const string KEY_Author = "Author";

		public const string KEY_Description = "Description";

		public const string KEY_Copyright = "Copyright";

		public const string KEY_Creation_Time = "Creation Time";

		public const string KEY_Software = "Software";

		public const string KEY_Disclaimer = "Disclaimer";

		public const string KEY_Warning = "Warning";

		public const string KEY_Source = "Source";

		public const string KEY_Comment = "Comment";

		internal PngChunkTextVar(string id, ImageInfo info)
			: base(id, info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.NONE;
		}

		public string GetKey()
		{
			return key;
		}

		public string GetVal()
		{
			return val;
		}

		public void SetKeyVal(string key, string val)
		{
			this.key = key;
			this.val = val;
		}
	}
}
