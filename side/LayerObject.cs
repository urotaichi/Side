using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace MasaoPlus
{
    [Serializable]
    public class LayerObject : ICloneable, IEnumerable<string>
    {
        public string[] Strings;

        [XmlIgnore]
        public string Source;

        public LayerObject()
        {
            Strings = [];
        }

        public int Length => Strings.Length;

        public string this[int index]
        {
            get => Strings[index];
            set => Strings[index] = value;
        }

        // ICloneable実装
        public object Clone()
        {
            return new LayerObject
            {
                Strings = (string[])Strings.Clone(),
                Source = Source
            };
        }

        // IEnumerable<string>実装
        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)Strings).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Strings.GetEnumerator();
        }

        // 便利なメソッド
        public void Add(string item)
        {
            Array.Resize(ref Strings, Strings.Length + 1);
            Strings[^1] = item;
        }
    }
}
