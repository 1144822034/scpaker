using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FluxJpeg.Core.Decoder
{
	internal class JpegScan
	{
		public List<JpegComponent> components = new List<JpegComponent>();

		public int maxV;

		public int maxH;

		public IList<JpegComponent> Components => new ReadOnlyCollection<JpegComponent>(components);

		internal int MaxH => maxH;

		internal int MaxV => maxV;

		public void AddComponent(byte id, byte factorHorizontal, byte factorVertical, byte quantizationID, byte colorMode)
		{
			JpegComponent item = new JpegComponent(this, id, factorHorizontal, factorVertical, quantizationID, colorMode);
			components.Add(item);
			maxH = components.Max((JpegComponent x) => x.factorH);
			maxV = components.Max((JpegComponent x) => x.factorV);
		}

		public JpegComponent GetComponentById(byte Id)
		{
			return components.First((JpegComponent x) => x.component_id == Id);
		}
	}
}
