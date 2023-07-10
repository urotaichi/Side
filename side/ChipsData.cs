using System;
using System.Xml.Serialization;

namespace MasaoPlus
{
	[XmlType("chips")]
	[Serializable]
	public struct ChipsData
	{
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

		[XmlAttribute("char")]
		public string character;

        [XmlAttribute("code")]
        public int code;

        [XmlAttribute("color")]
		public string color;

		[XmlAttribute("rel")]
		public string relation;

		[XmlElement("chip")]
		public ChipData[] Chips;
	}
}
