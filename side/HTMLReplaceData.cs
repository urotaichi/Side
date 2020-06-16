using System;
using System.Xml.Serialization;

namespace MasaoPlus
{
	// Token: 0x02000024 RID: 36
	[XmlType("repl")]
	[Serializable]
	public struct HTMLReplaceData
	{
		// Token: 0x06000102 RID: 258 RVA: 0x000029B5 File Offset: 0x00000BB5
		public HTMLReplaceData(string n, string v)
		{
			this.Name = n;
			this.Value = v;
		}

		// Token: 0x04000149 RID: 329
		[XmlAttribute("name")]
		public string Name;

		// Token: 0x0400014A RID: 330
		[XmlAttribute("value")]
		public string Value;
	}
}
