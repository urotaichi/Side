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
            if (Chips == null)
            {
                return default;
            }
            if (relation == "" || relation == null || !Global.state.ChipRegister.ContainsKey(relation))
            {
                return Chips[0];
            }
            int num = 0;
            string b = Global.state.ChipRegister[relation];
            foreach (ChipData chipData in Chips)
            {
                if (chipData.value == b)
                {
                    return Chips[num];
                }
                num++;
            }
            return Chips[0];
        }

        [XmlAttribute("char")]
        public string character;

        [XmlAttribute("code")]
        public string code;

        [XmlAttribute("color")]
        public string color;

        [XmlAttribute("rel")]
        public string relation;

        [XmlElement("chip")]
        public ChipData[] Chips;

        [XmlElement("properties")]
        public CustomPartsProperties Properties;
    }
}
