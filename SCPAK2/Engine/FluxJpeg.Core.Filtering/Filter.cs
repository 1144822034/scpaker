using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FluxJpeg.Core.Filtering
{
	internal abstract class Filter
	{
		public int _newWidth;

		public int _newHeight;

		public byte[][,] _sourceData;

		public byte[][,] _destinationData;

		public bool _color;

		public FilterProgressEventArgs progressArgs = new FilterProgressEventArgs();

		public event EventHandler<FilterProgressEventArgs> ProgressChanged;

		public void UpdateProgress(double progress)
		{
			progressArgs.Progress = progress;
			if (this.ProgressChanged != null)
			{
				this.ProgressChanged(this, progressArgs);
			}
		}

		public byte[][,] Apply(byte[][,] imageData, int newWidth, int newHeight)
		{
			_newHeight = newHeight;
			_newWidth = newWidth;
			_color = (imageData.Length != 1);
			_destinationData = Image.CreateRaster(newWidth, newHeight, imageData.Length);
			_sourceData = imageData;
			ApplyFilter();
			return _destinationData;
		}

		public abstract void ApplyFilter();
	}
}
