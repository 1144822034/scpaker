using System;

namespace Engine.Graphics
{
	[Flags]
	public enum TextAnchor
	{
		Default = 0x0,
		Left = 0x0,
		Top = 0x0,
		HorizontalCenter = 0x1,
		VerticalCenter = 0x2,
		Right = 0x4,
		Bottom = 0x8,
		DisableSnapToPixels = 0x10
	}
}
