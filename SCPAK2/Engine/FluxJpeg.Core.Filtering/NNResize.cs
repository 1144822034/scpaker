namespace FluxJpeg.Core.Filtering
{
	internal class NNResize : Filter
	{
		public override void ApplyFilter()
		{
			int length = _sourceData[0].GetLength(0);
			int length2 = _sourceData[0].GetLength(1);
			double num = (double)length / (double)_newWidth;
			double num2 = (double)length2 / (double)_newHeight;
			double num3 = 0.5 * num;
			double num4 = 0.5 * num2;
			for (int i = 0; i < _newHeight; i++)
			{
				int num5 = (int)num4;
				num3 = 0.0;
				UpdateProgress((double)i / (double)_newHeight);
				for (int j = 0; j < _newWidth; j++)
				{
					int num6 = (int)num3;
					_destinationData[0][j, i] = _sourceData[0][num6, num5];
					if (_color)
					{
						_destinationData[1][j, i] = _sourceData[1][num6, num5];
						_destinationData[2][j, i] = _sourceData[2][num6, num5];
					}
					num3 += num;
				}
				num4 += num2;
			}
		}
	}
}
