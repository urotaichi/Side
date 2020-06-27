using System;
using System.Xml.Serialization;

namespace MasaoPlus
{
	// Token: 0x02000026 RID: 38
	[XmlType("param")]
	[Serializable]
	public struct ConfigParam
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000120 RID: 288 RVA: 0x00017F68 File Offset: 0x00016168
		[XmlIgnore]
		public ConfigParam.Types Type
		{
			get
			{
				string typestr;
				switch (typestr = this.Typestr)
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

		// Token: 0x0400014E RID: 334
		[XmlAttribute("type")]
		public string Typestr;

		// Token: 0x0400014F RID: 335
		[XmlAttribute("value")]
		public string Value;

		// Token: 0x04000150 RID: 336
		[XmlAttribute("desc")]
		public string Description;

		// Token: 0x04000151 RID: 337
		[XmlAttribute("name")]
		public string Name;

		// Token: 0x04000152 RID: 338
		[XmlAttribute("rel")]
		public string Relation;

		// Token: 0x04000153 RID: 339
		[XmlAttribute("chip")]
		public string ChipRelation;

		// Token: 0x04000154 RID: 340
		[XmlAttribute("category")]
		public string Category;

		// Token: 0x04000155 RID: 341
		[XmlAttribute("row")]
		public int Rows;

		// Token: 0x04000156 RID: 342
		[XmlAttribute("stage")]
		public int RequireStages;

		// Token: 0x04000157 RID: 343
		[XmlElement("list")]
		public string[] ListItems;

		// Token: 0x02000027 RID: 39
		public enum Types
		{
            // Token: 0x04000159 RID: 345
            b, b2, b0,
			// Token: 0x0400015A RID: 346
			s,
			// Token: 0x0400015B RID: 347
			i,
			// Token: 0x0400015C RID: 348
			t,
            // Token: 0x0400015D RID: 349
            f, f_i, f_a,
            // Token: 0x0400015E RID: 350
            l, l_a,
			// Token: 0x0400015F RID: 351
			c,
			// Token: 0x04000160 RID: 352
			UnKnown
		}
	}
}
