using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MasaoPlus
{
    [XmlType("ChipDefinition")]
    [Serializable]
    public class ChipDataClass
    {
        public static ChipDataClass ParseXML(string file)
        {
            ChipDataClass result;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ChipDataClass));
                using FileStream fileStream = new FileStream(file, FileMode.Open);
                ChipDataClass chipDataClass = (ChipDataClass)xmlSerializer.Deserialize(fileStream);
                result = chipDataClass;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"チップ定義を開けませんでした。{Environment.NewLine}{ex.Message}", "オープン失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                result = null;
            }
            return result;
        }

        public static int CharToCode(string character)
        {
            if(character.Length == 2)
            {
                if (character.Equals("..")) return 0;
                else return int.Parse(character, System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                if (character.Equals(".")) return 0;
                else return character.ToCharArray(0, 1)[0];
            }
        }

        public static string CodeToChar(object code, int length)
        {
            int c = int.Parse((string)code);
            if (length == 2)
            {
                if (c == 0) return "..";
                else return c.ToString("x2");
            }
            else
            {
                if (c == 0) return ".";
                else return ((char)c).ToString();
            }
        }

        public ChipsData[] Mapchip = new ChipsData[0];

        public ChipsData[] Layerchip = new ChipsData[0];

        public ChipsData[] WorldChip = new ChipsData[0];

        public ChipsData[] VarietyChip = new ChipsData[0];
    }
}
