using System;
using System.Xml.Serialization;

namespace MasaoPlus
{
    [XmlType("repl")]
    [Serializable]
    public struct HTMLReplaceData
    {
        public HTMLReplaceData(string n, string v)
        {
            Name = n;
            Value = v;
        }

        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("value")]
        public string Value;
    }
}
