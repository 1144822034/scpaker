using FluxJpeg.Core.Filtering;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FluxJpeg.Core
{
	internal class ImageResizer
	{
		public ResizeProgressChangedEventArgs progress = new ResizeProgressChangedEventArgs();

		public Image _input;

		public event EventHandler<ResizeProgressChangedEventArgs> ProgressChanged;
		public ImageResizer(Image input)
		{
			_input = input;
		}

		public static bool ResizeNeeded(Image image, int maxEdgeLength)
		{
			return ((image.Width > image.Height) ? ((double)maxEdgeLength / (double)image.Width) : ((double)maxEdgeLength / (double)image.Height)) < 1.0;
		}

		public Image ResizeToScale(int maxEdgeLength, ResamplingFilters technique)
		{
			double num = 0.0;
			num = ((_input.Width <= _input.Height) ? ((double)maxEdgeLength / (double)_input.Height) : ((double)maxEdgeLength / (double)_input.Width));
			if (num >= 1.0)
			{
				throw new ResizeNotNeededException();
			}
			return ResizeToScale(num, technique);
		}

		public Image ResizeToScale(int maxWidth, int maxHeight, ResamplingFilters technique)
		{
			double num = (double)maxWidth / (double)_input.Width;
			double num2 = (double)maxHeight / (double)_input.Height;
			double num3 = 0.0;
			num3 = ((!(num < num2)) ? num2 : num);
			if (num3 >= 1.0)
			{
				throw new ResizeNotNeededException();
			}
			return ResizeToScale(num3, technique);
		}

		public Image ResizeToScale(double scale, ResamplingFilters technique)
		{
			int height = (int)(scale * (double)_input.Height);
			int width = (int)(scale * (double)_input.Width);
			Filter resizeFilter = GetResizeFilter(technique);
			return PerformResize(resizeFilter, width, height);
		}

		public Image PerformResize(Filter resizeFilter, int width, int height)
		{
			return new Image(_input.ColorModel, resizeFilter.Apply(_input.Raster, width, height))
			{
				DensityX = _input.DensityX,
				DensityY = _input.DensityY
			};
		}

		public Filter GetResizeFilter(ResamplingFilters technique)
		{
			Filter filter;
			switch (technique)
			{
			case ResamplingFilters.NearestNeighbor:
				filter = new NNResize();
				break;
			case ResamplingFilters.LowpassAntiAlias:
				filter = new LowpassResize();
				break;
			default:
				throw new NotSupportedException();
			}
			filter.ProgressChanged += ResizeProgressChanged;
			return filter;
		}

		public Image Resize(int width, int height, ResamplingFilters technique)
		{
			Filter resizeFilter = GetResizeFilter(technique);
			return PerformResize(resizeFilter, width, height);
		}

		public void ResizeProgressChanged(object sender, FilterProgressEventArgs e)
		{
			progress.Progress = e.Progress;
			if (this.ProgressChanged != null)
			{
				this.ProgressChanged(this, progress);
			}
		}
	}
}
