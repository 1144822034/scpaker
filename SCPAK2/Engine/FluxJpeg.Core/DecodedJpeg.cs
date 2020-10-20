using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FluxJpeg.Core
{
	internal class DecodedJpeg
	{
		public Image _image;

		internal int[] BlockWidth;

		internal int[] BlockHeight;

		internal int Precision = 8;

		internal int[] HsampFactor = new int[3]
		{
			1,
			1,
			1
		};

		internal int[] VsampFactor = new int[3]
		{
			1,
			1,
			1
		};

		internal bool[] lastColumnIsDummy = new bool[3];

		internal bool[] lastRowIsDummy = new bool[3];

		internal int[] compWidth;

		internal int[] compHeight;

		internal int MaxHsampFactor;

		internal int MaxVsampFactor;

		public List<JpegHeader> _metaHeaders;

		public Image Image => _image;

		public bool HasJFIF
		{
			get;
			set;
		}

		public IList<JpegHeader> MetaHeaders => new ReadOnlyCollection<JpegHeader>(_metaHeaders);

		public DecodedJpeg(Image image, IEnumerable<JpegHeader> metaHeaders)
		{
			_image = image;
			_metaHeaders = ((metaHeaders == null) ? new List<JpegHeader>(0) : new List<JpegHeader>(metaHeaders));
			foreach (JpegHeader metaHeader in _metaHeaders)
			{
				if (metaHeader.IsJFIF)
				{
					HasJFIF = true;
					break;
				}
			}
			int componentCount = _image.ComponentCount;
			compWidth = new int[componentCount];
			compHeight = new int[componentCount];
			BlockWidth = new int[componentCount];
			BlockHeight = new int[componentCount];
			Initialize();
		}

		public DecodedJpeg(Image image)
			: this(image, null)
		{
			_metaHeaders = new List<JpegHeader>();
			string s = "Jpeg Codec | fluxcapacity.net ";
			_metaHeaders.Add(new JpegHeader
			{
				Marker = 254,
				Data = Encoding.UTF8.GetBytes(s)
			});
		}

		public void Initialize()
		{
			int width = _image.Width;
			int height = _image.Height;
			MaxHsampFactor = 1;
			MaxVsampFactor = 1;
			for (int i = 0; i < _image.ComponentCount; i++)
			{
				MaxHsampFactor = Math.Max(MaxHsampFactor, HsampFactor[i]);
				MaxVsampFactor = Math.Max(MaxVsampFactor, VsampFactor[i]);
			}
			for (int i = 0; i < _image.ComponentCount; i++)
			{
				compWidth[i] = ((width % 8 != 0) ? ((int)Math.Ceiling((double)width / 8.0) * 8) : width) / MaxHsampFactor * HsampFactor[i];
				if (compWidth[i] != width / MaxHsampFactor * HsampFactor[i])
				{
					lastColumnIsDummy[i] = true;
				}
				BlockWidth[i] = (int)Math.Ceiling((double)compWidth[i] / 8.0);
				compHeight[i] = ((height % 8 != 0) ? ((int)Math.Ceiling((double)height / 8.0) * 8) : height) / MaxVsampFactor * VsampFactor[i];
				if (compHeight[i] != height / MaxVsampFactor * VsampFactor[i])
				{
					lastRowIsDummy[i] = true;
				}
				BlockHeight[i] = (int)Math.Ceiling((double)compHeight[i] / 8.0);
			}
		}
	}
}
