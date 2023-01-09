using System;
using System.Drawing;
using System.Xml.Serialization;

namespace MasaoPlus
{
	public struct ChipData
	{
		[XmlAttribute("value")]
		public string value;

		public Point pattern;

		public Size size;

		public Size view_size;

		public Point center;

		public int rotate;

		public float scale;

		public int repeat;

		public Point xdraw;

		public bool xdbackgrnd;

		public string name;

		public string description;
	}
}
