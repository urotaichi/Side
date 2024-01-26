using System;
using System.Xml.Serialization;

namespace MasaoPlus
{
    [XmlType("repl")]
    [Serializable]
    public struct HTMLReplaceData(string n, string v)
    {
        [XmlAttribute("name")]
        public string Name = n;

        [XmlAttribute("value")]
        public string Value = v;
    }
}
