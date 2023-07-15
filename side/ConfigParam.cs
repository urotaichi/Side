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
                        return Types.b;
                    case "bool21":
                        return Types.b2;
                    case "bool10":
                        return Types.b0;
                    case "int":
                        return Types.i;
                    case "string":
                        return Types.s;
                    case "text":
                        return Types.t;
                    case "file":
                        return Types.f;
                    case "file_img":
                        return Types.f_i;
                    case "file_audio":
                        return Types.f_a;
                    case "list":
                        return Types.l;
                    case "list_athletic":
                        return Types.l_a;
                    case "color":
                        return Types.c;
                }
                return Types.UnKnown;
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
