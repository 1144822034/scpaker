using Engine.Graphics;
using Engine.Media;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Engine.Content
{
	public static class ContentCache
	{
		public class ContentDescription
		{
			public string TypeName;

			public ContentStream Stream;

			public long Position;

			public long BytesCount;

			public List<CachedItemWrapperBase> CachedItemWrappers;

			public object Content;
		}

		public abstract class CachedItemWrapperBase
		{
			internal abstract void Clear();
		}

		public class CachedItemWrapper<T> : CachedItemWrapperBase
		{
			public string m_name;

			internal bool m_isCreated;

			internal T m_value;

			public CachedItemWrapper(string name)
			{
				m_name = name;
				ContentDescription contentDescription = GetContentDescription(name, throwIfNotFound: true);
				if (contentDescription.CachedItemWrappers == null)
				{
					contentDescription.CachedItemWrappers = new List<CachedItemWrapperBase>();
				}
				contentDescription.CachedItemWrappers.Add(this);
			}

			internal override void Clear()
			{
				m_isCreated = false;
				m_value = default(T);
			}

			public T Get()
			{
				if (!m_isCreated)
				{
					m_value = Get<T>(m_name);
					m_isCreated = true;
				}
				return m_value;
			}
		}

		public static object m_lock = new object();

		public static bool m_isInitialized;

		public static Dictionary<string, IContentReader> m_contentReadersByTypeName = new Dictionary<string, IContentReader>();

		public static HashSet<Assembly> m_scannedAssemblies = new HashSet<Assembly>();

		public static Dictionary<string, ContentDescription> m_contentDescriptionsByName = new Dictionary<string, ContentDescription>();

		public static Dictionary<string, List<ContentInfo>> m_contentInfosByFolderName = new Dictionary<string, List<ContentInfo>>();

		public static void AddPackage(string path, byte[] tocPad = null, byte[] contentPad = null)
		{
			Stream stream = Storage.OpenFile(path, OpenFileMode.Read);
			AddPackage(()=>stream, tocPad, contentPad);
		}

		public static void AddPackage(Func<Stream> streamFactory, byte[] tocPad = null, byte[] contentPad = null)
		{
			lock (m_lock)
			{
				if (!m_isInitialized)
				{
					m_isInitialized = true;
					Display.DeviceReset += Display_DeviceReset;
				}
				ContentStream contentStream = new ContentStream(streamFactory);
				using (BinaryReader binaryReader = new BinaryReader(contentStream, Encoding.UTF8, leaveOpen: true))
				{
					byte[] array = new byte[4];
					if (binaryReader.Read(array, 0, array.Length) != array.Length || array[0] != 80 || array[1] != 75 || array[2] != 50 || array[3] != 0)
					{
						throw new InvalidOperationException("Invalid package header.");
					}
					contentStream.Pad = tocPad;
					long num = binaryReader.ReadInt64();
					int num2 = binaryReader.ReadInt32();
					for (int i = 0; i < num2; i++)
					{
						string name = binaryReader.ReadString();
						string typeName = binaryReader.ReadString();
						long position = binaryReader.ReadInt64() + num;
						long bytesCount = binaryReader.ReadInt64();
						SetContentDescription(name, new ContentDescription
						{
							TypeName = typeName,
							Stream = contentStream,
							Position = position,
							BytesCount = bytesCount
						});
					}
					contentStream.Pad = contentPad;
				}
			}
		}

		public static bool IsContent(object content)
		{
			lock (m_lock)
			{
				return m_contentDescriptionsByName.Values.Any((ContentDescription cd) => content == cd.Content);
			}
		}

		public static object Get(string name, bool throwIfNotFound = true)
		{
			lock (m_lock)
			{
				ContentDescription contentDescription = GetContentDescription(name, throwIfNotFound);
				if (contentDescription != null)
				{
					if (contentDescription.Content == null)
					{
						LoadContent(contentDescription);
					}
					return contentDescription.Content;
				}
				return null;
			}
		}

		public static object Get(Type type, string name, bool throwIfNotFound = true)
		{
			object obj = Get(name, throwIfNotFound);
			if (!type.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo()))
			{
				throw new InvalidOperationException($"Cannot cast content \"{name}\" of type \"{obj.GetType().FullName}\" to type \"{type}\".");
			}
			return obj;
		}

		public static T Get<T>(string name, bool throwIfNotFound = true)
		{
			object obj = Get(name, throwIfNotFound);
			try
			{
				return (T)obj;
			}
			catch (InvalidCastException)
			{
				throw new InvalidOperationException($"Cannot cast content \"{name}\" of type \"{obj.GetType().FullName}\" to type \"{typeof(T)}\".");
			}
		}

		public static void Set(string name, object value)
		{
			lock (m_lock)
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (m_contentDescriptionsByName.ContainsKey(name))
				{
					Dispose(name);
				}
				SetContentDescription(name, new ContentDescription
				{
					Content = value,
					TypeName = value.GetType().Name
				});
			}
		}

		public static ReadOnlyList<ContentInfo> List(string folderName = "")
		{
			if (folderName == null)
			{
				throw new ArgumentNullException("folderName");
			}
			lock (m_lock)
			{
				if (!m_contentInfosByFolderName.TryGetValue(folderName, out List<ContentInfo> value))
				{
					value = new List<ContentInfo>();
					foreach (KeyValuePair<string, ContentDescription> item in m_contentDescriptionsByName)
					{
						if (string.IsNullOrEmpty(folderName) || (item.Key.Length > folderName.Length && item.Key[folderName.Length] == '/' && item.Key.StartsWith(folderName)))
						{
							value.Add(new ContentInfo
							{
								Name = item.Key,
								TypeName = item.Value.TypeName
							});
						}
					}
					m_contentInfosByFolderName.Add(folderName, value);
				}
				return new ReadOnlyList<ContentInfo>(value);
			}
		}

		public static void Dispose(string name)
		{
			lock (m_lock)
			{
				ContentDescription contentDescription = GetContentDescription(name, throwIfNotFound: true);
				(contentDescription.Content as IDisposable)?.Dispose();
				contentDescription.Content = null;
				if (contentDescription.CachedItemWrappers != null)
				{
					foreach (CachedItemWrapperBase cachedItemWrapper in contentDescription.CachedItemWrappers)
					{
						cachedItemWrapper.Clear();
					}
				}
			}
		}

		public static void Dispose()
		{
			lock (m_lock)
			{
				foreach (string key in m_contentDescriptionsByName.Keys)
				{
					Dispose(key);
				}
			}
		}

		public static void SetContentDescription(string name, ContentDescription contentDescription)
		{
			if (m_contentInfosByFolderName.Count > 0 && (!m_contentDescriptionsByName.TryGetValue(name, out ContentDescription value) || value.TypeName != contentDescription.TypeName))
			{
				m_contentInfosByFolderName.Clear();
			}
			m_contentDescriptionsByName[name] = contentDescription;
		}

		public static ContentDescription GetContentDescription(string name, bool throwIfNotFound)
		{
			if (!m_contentDescriptionsByName.TryGetValue(name, out ContentDescription value) && throwIfNotFound)
			{
				throw new InvalidOperationException($"Content \"{name}\" not found in ContentCache.");
			}
			return value;
		}

		public static void LoadContent(ContentDescription contentDescription)
		{
			if (contentDescription.Stream != null)
			{
				IContentReader contentReader = GetContentReader(contentDescription.TypeName);
				ContentStream stream = PrepareContentStream(contentDescription);
				contentDescription.Content = contentReader.Read(stream, contentDescription.Content);
				return;
			}
			throw new InvalidOperationException("Cannot load manually added content item. Item must have been disposed and accessed again.");
		}

		public static ContentStream PrepareContentStream(ContentDescription contentDescription)
		{
			contentDescription.Stream.Position = contentDescription.Position;
			return contentDescription.Stream.CreateSubstream(contentDescription.BytesCount);
		}

		public static IContentReader GetContentReader(string contentTypeName)
		{
			lock (m_lock)
			{
				if (!m_contentReadersByTypeName.TryGetValue(contentTypeName, out IContentReader value))
				{
					ScanAssembliesForContentReaders();
					m_contentReadersByTypeName.TryGetValue(contentTypeName, out value);
				}
				if (value == null)
				{
					throw new InvalidOperationException($"Content reader for \"{contentTypeName}\" not found in any of the loaded assemblies.");
				}
				return value;
			}
		}

		public static void ScanAssembliesForContentReaders()
		{
			foreach (Assembly item in TypeCache.LoadedAssemblies.Where((Assembly a) => !TypeCache.IsKnownSystemAssembly(a)))
			{
				if (!m_scannedAssemblies.Contains(item))
				{
					foreach (TypeInfo definedType in item.DefinedTypes)
					{
						ContentReaderAttribute customAttribute = definedType.GetCustomAttribute<ContentReaderAttribute>();
						if (customAttribute != null && !m_contentReadersByTypeName.ContainsKey(customAttribute.ContentTypeName))
						{
							IContentReader value = (IContentReader)Activator.CreateInstance(definedType.AsType());
							m_contentReadersByTypeName.Add(customAttribute.ContentTypeName, value);
						}
					}
					m_scannedAssemblies.Add(item);
				}
			}
		}

		public static void Display_DeviceReset()
		{
			lock (m_lock)
			{
				foreach (KeyValuePair<string, ContentDescription> item in m_contentDescriptionsByName)
				{
					if (item.Value.Content is Texture2D || item.Value.Content is Model || item.Value.Content is BitmapFont)
					{
						LoadContent(item.Value);
					}
				}
			}
		}
	}
}
