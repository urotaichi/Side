using System;
using System.Xml.Serialization;

namespace MasaoPlus
{
	// Token: 0x02000021 RID: 33
	[XmlType("chips")]
	[Serializable]
	public struct ChipsData
	{
		// Token: 0x060000FC RID: 252 RVA: 0x00017590 File Offset: 0x00015790
		public ChipData GetCSChip()
		{
			if (this.Chips == null)
			{
				return default(ChipData);
			}
			if (this.relation == "" || this.relation == null || !Global.state.ChipRegister.ContainsKey(this.relation))
			{
				return this.Chips[0];
			}
			int num = 0;
			string b = Global.state.ChipRegister[this.relation];
			foreach (ChipData chipData in this.Chips)
			{
				if (chipData.value == b)
				{
					return this.Chips[num];
				}
				num++;
			}
			return this.Chips[0];
		}

		// Token: 0x04000130 RID: 304
		[XmlAttribute("char")]
		public string character;

		// Token: 0x04000131 RID: 305
		[XmlAttribute("color")]
		public string color;

		// Token: 0x04000132 RID: 306
		[XmlAttribute("rel")]
		public string relation;

		// Token: 0x04000133 RID: 307
		[XmlElement("chip")]
		public ChipData[] Chips;
	}
}
