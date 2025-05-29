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

        public LayerObject(string[] stringArray)
        {
            Strings = stringArray ?? [];
        }

        public LayerObject(object stringList)
        {
            if (stringList is IEnumerable<string> enumerable)
            {
                Strings = [.. enumerable];
            }
            else
            {
                throw new ArgumentException("The provided object is not a valid IEnumerable<string>.", nameof(stringList));
            }
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
            return new LayerObject((string[])Strings.Clone());
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

        public void Clear()
        {
            Strings = [];
        }

        public bool Contains(string item)
        {
            return Strings.Contains(item);
        }

        public int IndexOf(string item)
        {
            return Array.IndexOf(Strings, item);
        }
    }
}
