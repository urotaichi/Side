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
                return Typestr switch
                {
                    "bool" => Types.b,
                    "bool21" => Types.b2,
                    "bool10" => Types.b0,
                    "int" => Types.i,
                    "string" => Types.s,
                    "text" => Types.t,
                    "file" => Types.f,
                    "file_img" => Types.f_i,
                    "file_audio" => Types.f_a,
                    "list" => Types.l,
                    "list_athletic" => Types.l_a,
                    "color" => Types.c,
                    _ => Types.UnKnown,
                };
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
