using System;

namespace Hjg.Pngcs
{
	internal class FilterWriteStrategy
	{
		public static readonly int COMPUTE_STATS_EVERY_N_LINES = 8;

		public readonly ImageInfo imgInfo;

		public readonly FilterType configuredType;

		public FilterType currentType;

		public int lastRowTested = -1000000;

		public double[] lastSums = new double[5];

		public double[] lastEntropies = new double[5];

		public double[] preference = new double[5]
		{
			1.1,
			1.1,
			1.1,
			1.1,
			1.2
		};

		public int discoverEachLines = -1;

		public double[] histogram1 = new double[256];

		internal FilterWriteStrategy(ImageInfo imgInfo, FilterType configuredType)
		{
			this.imgInfo = imgInfo;
			this.configuredType = configuredType;
			if (configuredType < FilterType.FILTER_NONE)
			{
				if ((imgInfo.Rows < 8 && imgInfo.Cols < 8) || imgInfo.Indexed || imgInfo.BitDepth < 8)
				{
					currentType = FilterType.FILTER_NONE;
				}
				else
				{
					currentType = FilterType.FILTER_PAETH;
				}
			}
			else
			{
				currentType = configuredType;
			}
			if (configuredType == FilterType.FILTER_AGGRESSIVE)
			{
				discoverEachLines = COMPUTE_STATS_EVERY_N_LINES;
			}
			if (configuredType == FilterType.FILTER_VERYAGGRESSIVE)
			{
				discoverEachLines = 1;
			}
		}

		internal bool shouldTestAll(int rown)
		{
			if (discoverEachLines > 0 && lastRowTested + discoverEachLines <= rown)
			{
				currentType = FilterType.FILTER_UNKNOWN;
				return true;
			}
			return false;
		}

		internal void setPreference(double none, double sub, double up, double ave, double paeth)
		{
			preference = new double[5]
			{
				none,
				sub,
				up,
				ave,
				paeth
			};
		}

		internal bool computesStatistics()
		{
			return discoverEachLines > 0;
		}

		internal void fillResultsForFilter(int rown, FilterType type, double sum, int[] histo, bool tentative)
		{
			lastRowTested = rown;
			lastSums[(int)type] = sum;
			if (histo == null)
			{
				return;
			}
			double num = (rown == 0) ? 0.0 : 0.3;
			double num2 = 1.0 - num;
			double num3 = 0.0;
			for (int i = 0; i < 256; i++)
			{
				double num4 = (double)histo[i] / (double)imgInfo.Cols;
				num4 = histogram1[i] * num + num4 * num2;
				if (tentative)
				{
					num3 += ((num4 > 1E-08) ? (num4 * Math.Log(num4)) : 0.0);
				}
				else
				{
					histogram1[i] = num4;
				}
			}
			lastEntropies[(int)type] = 0.0 - num3;
		}

		internal FilterType gimmeFilterType(int rown, bool useEntropy)
		{
			if (currentType == FilterType.FILTER_UNKNOWN)
			{
				if (rown == 0)
				{
					currentType = FilterType.FILTER_SUB;
				}
				else
				{
					double num = double.MaxValue;
					for (int i = 0; i < 5; i++)
					{
						double num2 = useEntropy ? lastEntropies[i] : lastSums[i];
						num2 /= preference[i];
						if (num2 <= num)
						{
							num = num2;
							currentType = (FilterType)i;
						}
					}
				}
			}
			if (configuredType == FilterType.FILTER_CYCLIC)
			{
				currentType = (FilterType)((int)(currentType + 1) % 5);
			}
			return currentType;
		}
	}
}
