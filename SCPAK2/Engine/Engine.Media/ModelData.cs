using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Media
{
	public class ModelData
	{
		public List<ModelBoneData> Bones = new List<ModelBoneData>();

		public List<ModelMeshData> Meshes = new List<ModelMeshData>();

		public List<ModelBuffersData> Buffers = new List<ModelBuffersData>();

		public static ModelFileFormat DetermineFileFormat(Stream stream)
		{
			if (Collada.IsColladaStream(stream))
			{
				return ModelFileFormat.Collada;
			}
			throw new InvalidOperationException("Unsupported model file format.");
		}

		public static ModelFileFormat DetermineFileFormat(string extension)
		{
			if (extension.Equals(".dae", StringComparison.OrdinalIgnoreCase))
			{
				return ModelFileFormat.Collada;
			}
			throw new InvalidOperationException("Unsupported model file format.");
		}

		public static ModelData Load(Stream stream, ModelFileFormat format)
		{
			if (format == ModelFileFormat.Collada)
			{
				return Collada.Load(stream);
			}
			throw new InvalidOperationException("Unsupported model file format.");
		}

		public static ModelData Load(string fileName, ModelFileFormat format)
		{
			using (Stream stream = Storage.OpenFile(fileName, OpenFileMode.Read))
			{
				return Load(stream, format);
			}
		}

		public static ModelData Load(Stream stream)
		{
			PeekStream peekStream = new PeekStream(stream, 256);
			ModelFileFormat format = DetermineFileFormat(peekStream.GetInitialBytesStream());
			return Load(peekStream, format);
		}

		public static ModelData Load(string fileName)
		{
			using (Stream stream = Storage.OpenFile(fileName, OpenFileMode.Read))
			{
				return Load(stream);
			}
		}
	}
}
