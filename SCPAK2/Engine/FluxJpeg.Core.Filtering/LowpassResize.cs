namespace FluxJpeg.Core.Filtering
{
	internal class LowpassResize : Filter
	{
		public override void ApplyFilter()
		{
			int length = _sourceData[0].GetLength(0);
			int num = _sourceData.Length;
			double std = (double)(length / _newWidth) * 0.5;
			for (int i = 0; i < num; i++)
			{
				GrayImage data = new GrayImage(_sourceData[i]);
				data = Convolution.Instance.GaussianConv(data, std);
				_sourceData[i] = data.ToByteArray2D();
			}
			NNResize nNResize = new NNResize();
			_destinationData = nNResize.Apply(_sourceData, _newWidth, _newHeight);
		}
	}
}
