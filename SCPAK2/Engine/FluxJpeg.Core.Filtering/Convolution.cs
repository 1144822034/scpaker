using System;

namespace FluxJpeg.Core.Filtering
{
	internal class Convolution
	{
		public struct FilterJob
		{
			public float[] filter;

			public int start;

			public int end;

			public GrayImage data;

			public GrayImage result;

			public int destPtr;
		}

		public static readonly Convolution Instance = new Convolution();

		public GrayImage GaussianConv(GrayImage data, double std)
		{
			float[] filter = GaussianFilter(std);
			return Conv2DSeparable(data, filter);
		}

		public float[] GaussianFilter(double std)
		{
			double num = std * std;
			int num2 = (int)Math.Ceiling(Math.Sqrt(-1.0 * num * Math.Log(0.0099999997764825821)));
			float[] array = new float[num2];
			double num3 = -1.0;
			for (int i = 0; i < num2; i++)
			{
				double num4 = Math.Exp(-0.5 * (double)(i * i) / num);
				array[i] = (float)num4;
				num3 += 2.0 * num4;
			}
			for (int j = 0; j < num2; j++)
			{
				array[j] /= (float)num3;
			}
			return array;
		}

		public GrayImage Conv2DSeparable(GrayImage data, float[] filter)
		{
			GrayImage data2 = Filter1DSymmetric(data, filter, transpose: true);
			return Filter1DSymmetric(data2, filter, transpose: true);
		}

		public GrayImage Filter1DSymmetric(GrayImage data, float[] filter, bool transpose)
		{
			GrayImage result = transpose ? new GrayImage(data.Height, data.Width) : new GrayImage(data.Width, data.Height);
			FilterJob filterJob = default(FilterJob);
			filterJob.filter = filter;
			filterJob.data = data;
			filterJob.destPtr = 0;
			filterJob.result = result;
			filterJob.start = 0;
			filterJob.end = data.Height;
			FilterJob filterJob2 = filterJob;
			if (transpose)
			{
				FilterPartSymmetricT(filterJob2);
			}
			else
			{
				FilterPartSymmetric(filterJob2);
			}
			return result;
		}

		public void FilterPartSymmetricT(object filterJob)
		{
			FilterJob filterJob2 = (FilterJob)filterJob;
			GrayImage data = filterJob2.data;
			float[] scan = data.Scan0;
			float[] filter = filterJob2.filter;
			GrayImage result = filterJob2.result;
			int num = filter.Length - 1;
			for (int i = filterJob2.start; i < filterJob2.end; i++)
			{
				int num2 = i * data.Width;
				for (int j = 0; j < num; j++)
				{
					float num3 = scan[num2] * filter[0];
					for (int k = 1; k < j + 1; k++)
					{
						num3 += (scan[num2 + k] + scan[num2 - k]) * filter[k];
					}
					for (int l = j + 1; l < filter.Length; l++)
					{
						num3 += (scan[num2 + l] + scan[num2 + l]) * filter[l];
					}
					result[i, j] = num3;
					num2++;
				}
				for (int m = num; m < data.Width - num; m++)
				{
					float num4 = scan[num2] * filter[0];
					for (int n = 1; n < filter.Length; n++)
					{
						num4 += (scan[num2 + n] + scan[num2 - n]) * filter[n];
					}
					result[i, m] = num4;
					num2++;
				}
				for (int num5 = data.Width - num; num5 < data.Width; num5++)
				{
					float num6 = scan[num2] * filter[0];
					for (int num7 = 1; num7 < data.Width - num5; num7++)
					{
						num6 += (scan[num2 + num7] + scan[num2 - num7]) * filter[num7];
					}
					for (int num8 = data.Width - num5; num8 < filter.Length; num8++)
					{
						num6 += (scan[num2 - num8] + scan[num2 - num8]) * filter[num8];
					}
					result[i, num5] = num6;
					num2++;
				}
			}
		}

		public void FilterPartSymmetric(object filterJob)
		{
			FilterJob filterJob2 = (FilterJob)filterJob;
			GrayImage data = filterJob2.data;
			float[] scan = data.Scan0;
			float[] filter = filterJob2.filter;
			float[] scan2 = filterJob2.result.Scan0;
			int num = filter.Length - 1;
			int destPtr = filterJob2.destPtr;
			for (int i = filterJob2.start; i < filterJob2.end; i++)
			{
				int num2 = i * data.Width;
				for (int j = 0; j < num; j++)
				{
					float num3 = scan[num2] * filter[0];
					for (int k = 1; k < j + 1; k++)
					{
						num3 += (scan[num2 + k] + scan[num2 - k]) * filter[k];
					}
					for (int l = j + 1; l < filter.Length; l++)
					{
						num3 += (scan[num2 + l] + scan[num2 + l]) * filter[l];
					}
					scan2[destPtr++] = num3;
					num2++;
				}
				for (int m = num; m < data.Width - num; m++)
				{
					float num4 = scan[num2] * filter[0];
					for (int n = 1; n < filter.Length; n++)
					{
						num4 += (scan[num2 + n] + scan[num2 - n]) * filter[n];
					}
					scan2[destPtr++] = num4;
					num2++;
				}
				for (int num5 = data.Width - num; num5 < data.Width; num5++)
				{
					float num6 = scan[num2] * filter[0];
					for (int num7 = 0; num7 < data.Width - num5; num7++)
					{
						num6 += (scan[num2 + num7] + scan[num2 - num7]) * filter[num7];
					}
					for (int num8 = data.Width - num5; num8 < filter.Length; num8++)
					{
						num6 += (scan[num2 + num8] + scan[num2 - num8]) * filter[num8];
					}
					scan2[destPtr++] = num6;
					num2++;
				}
			}
		}

		public GrayImage Conv2DSymmetric(GrayImage data, GrayImage opLR)
		{
			int num = opLR.Width - 1;
			int num2 = opLR.Height - 1;
			GrayImage grayImage = new GrayImage(data.Width + 2 * num, data.Height + 2 * num2);
			int num3 = 0;
			for (int i = 0; i < data.Height; i++)
			{
				int num4 = (i + num2) * (data.Width + 2 * num) + num;
				for (int j = 0; j < data.Width; j++)
				{
					grayImage.Scan0[num4 + j] = data.Scan0[num3];
					num3++;
				}
			}
			return Conv2DSymm(grayImage, opLR);
		}

		public GrayImage Conv2DSymm(GrayImage data, GrayImage opLR)
		{
			if (opLR.Width % 2 != 0 || opLR.Height % 2 != 0)
			{
				throw new ArgumentException("Operator must have an even number of rows and columns.");
			}
			int num = opLR.Width - 1;
			int num2 = opLR.Height - 1;
			GrayImage grayImage = new GrayImage(data.Width - 2 * num, data.Height - 2 * num2);
			for (int i = num2; i < data.Height - num2; i++)
			{
				for (int j = num; j < data.Width - num; j++)
				{
					float num3 = data[j, i] * opLR.Scan0[0];
					for (int k = 1; k < opLR.Height; k++)
					{
						num3 += (data[j, i + k] + data[j, i - k]) * opLR[0, k];
					}
					for (int l = 1; l < opLR.Width; l++)
					{
						num3 += (data[j + l, i] + data[j - l, i]) * opLR[l, 0];
					}
					int num4 = 1;
					for (int m = 1; m < opLR.Height; m++)
					{
						int num5 = (i + m) * data.Width + j;
						int num6 = (i - m) * data.Width + j;
						for (int n = 1; n < opLR.Width; n++)
						{
							num3 += (data.Scan0[num5 + n] + data.Scan0[num6 + n] + data.Scan0[num5 - n] + data.Scan0[num6 - n]) * opLR.Scan0[num4];
							num4++;
						}
						num4++;
					}
					grayImage[j - num, i - num2] = num3;
				}
			}
			return grayImage;
		}

		public GrayImage Conv2D(GrayImage data, GrayImage op)
		{
			GrayImage grayImage = new GrayImage(data.Width, data.Height);
			if (op.Width % 2 == 0 || op.Height % 2 == 0)
			{
				throw new ArgumentException("Operator must have an odd number of rows and columns.");
			}
			int num = op.Width / 2;
			int num2 = op.Height / 2;
			for (int i = 0; i < data.Height; i++)
			{
				for (int j = 0; j < data.Width; j++)
				{
					float num3 = 0f;
					float num4 = 0f;
					for (int k = 0; k < op.Height; k++)
					{
						int num5 = i - num2 + k;
						if (num5 < 0 || num5 >= data.Height)
						{
							continue;
						}
						for (int l = 0; l < op.Width; l++)
						{
							int num6 = j - num + l;
							if (num6 >= 0 && num6 < data.Width)
							{
								float num7 = op[l, k];
								num4 += Math.Abs(num7);
								num3 += data[num6, num5] * num7;
							}
						}
					}
					grayImage[j, i] = num3 / num4;
				}
			}
			return grayImage;
		}
	}
}
