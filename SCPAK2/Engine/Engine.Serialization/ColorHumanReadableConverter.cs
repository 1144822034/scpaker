using System;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(Color))]
	internal class ColorHumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			Color color = (Color)value;
			if (color.A != byte.MaxValue)
			{
				return HumanReadableConverter.ValuesListToString(',', new int[4]
				{
					color.R,
					color.G,
					color.B,
					color.A
				});
			}
			return HumanReadableConverter.ValuesListToString(',', new int[3]
			{
				color.R,
				color.G,
				color.B
			});
		}

		public object ConvertFromString(Type type, string data)
		{
			if (data[0] == '#')
			{
				if (data.Length == 7)
				{
					byte r = (byte)(16 * HexToDecimal(data[1]) + HexToDecimal(data[2]));
					byte g = (byte)(16 * HexToDecimal(data[3]) + HexToDecimal(data[4]));
					byte b = (byte)(16 * HexToDecimal(data[5]) + HexToDecimal(data[6]));
					return new Color(r, g, b);
				}
				if (data.Length == 9)
				{
					byte r2 = (byte)(16 * HexToDecimal(data[1]) + HexToDecimal(data[2]));
					byte g2 = (byte)(16 * HexToDecimal(data[3]) + HexToDecimal(data[4]));
					byte b2 = (byte)(16 * HexToDecimal(data[5]) + HexToDecimal(data[6]));
					byte a = (byte)(16 * HexToDecimal(data[7]) + HexToDecimal(data[8]));
					return new Color(r2, g2, b2, a);
				}
				throw new Exception();
			}
			int[] array = HumanReadableConverter.ValuesListFromString<int>(',', data);
			if (array.Length == 3)
			{
				return new Color(array[0], array[1], array[2]);
			}
			if (array.Length == 4)
			{
				return new Color(array[0], array[1], array[2], array[3]);
			}
			throw new Exception();
		}

		public static int HexToDecimal(char digit)
		{
			if (digit >= '0' && digit <= '9')
			{
				return digit - 48;
			}
			if (digit >= 'A' && digit <= 'F')
			{
				return digit - 65 + 10;
			}
			if (digit >= 'a' && digit <= 'f')
			{
				return digit - 97 + 10;
			}
			throw new Exception();
		}
	}
}
