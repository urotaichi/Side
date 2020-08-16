using System;
using System.Drawing;
using System.Xml.Serialization;

namespace MasaoPlus
{
	// Token: 0x02000022 RID: 34
	public struct ChipData
	{
		// Token: 0x04000134 RID: 308
		[XmlAttribute("value")]
		public string value;

		// Token: 0x04000135 RID: 309
		public Point pattern;

		// Token: 0x04000136 RID: 310
		public Size size;

		public Size view_size;

		// Token: 0x04000137 RID: 311
		public Point center;

		public int rotate;

		public float scale;

		public int repeat;

		// Token: 0x04000138 RID: 312
		public Point xdraw;

		// Token: 0x04000139 RID: 313
		public bool xdbackgrnd;

		// Token: 0x0400013A RID: 314
		public string name;

		// Token: 0x0400013B RID: 315
		public string description;
	}
}
