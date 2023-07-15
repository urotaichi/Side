using System;
using System.Xml.Serialization;

namespace MasaoPlus
{
	[XmlType("param")]
	[Serializable]
	public struct ConfigParam
	{
		[XmlIgnore]
		public Types Type
		{
			get
			{
				string typestr;
				switch (typestr = Typestr)
				{
				case "bool":
					return ConfigParam.Types.b;
				case "bool21":
					return ConfigParam.Types.b2;
				case "bool10":
					return ConfigParam.Types.b0;
				case "int":
					return ConfigParam.Types.i;
				case "string":
					return ConfigParam.Types.s;
				case "text":
					return ConfigParam.Types.t;
				case "file":
					return ConfigParam.Types.f;
				case "file_img":
					return ConfigParam.Types.f_i;
				case "file_audio":
					return ConfigParam.Types.f_a;
				case "list":
					return ConfigParam.Types.l;
				case "list_athletic":
					return ConfigParam.Types.l_a;
				case "color":
					return ConfigParam.Types.c;
				}
				return ConfigParam.Types.UnKnown;
			}
		}

		[XmlAttribute("type")]
		public string Typestr;

		[XmlAttribute("value")]
		public string Value;

		[XmlAttribute("desc")]
		public string Description;

		[XmlAttribute("name")]
		public string Name;

		[XmlAttribute("rel")]
		public string Relation;

		[XmlAttribute("chip")]
		public string ChipRelation;

		[XmlAttribute("category")]
		public string Category;

		[XmlAttribute("row")]
		public int Rows;

		[XmlAttribute("stage")]
		public int RequireStages;

		[XmlElement("list")]
		public string[] ListItems;

		public enum Types
		{
            b, b2, b0,
			s,
			i,
			t,
            f, f_i, f_a,
            l, l_a,
			c,
			UnKnown
		}
	}
}
