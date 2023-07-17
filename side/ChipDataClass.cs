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

        public static string CharToCode(string character)
        {
            if(character.Length == 2)
            {
                if (character.Equals("..")) return "0";
                else return int.Parse(character, System.Globalization.NumberStyles.HexNumber).ToString();
            }
            else
            {
                if (character.Equals(".")) return "0";
                else return ((int)character.ToCharArray(0, 1)[0]).ToString();
            }
        }

        public static string CodeToChar(int code, int length = 1)
        {
            if (length == 2)
            {
                if (code == 0) return "..";
                else return code.ToString("x2");
            }
            else
            {
                if (code == 0) return ".";
                else return ((char)code).ToString();
            }
        }

        public ChipsData[] Mapchip = new ChipsData[0];

        public ChipsData[] Layerchip = new ChipsData[0];

        public ChipsData[] WorldChip = new ChipsData[0];

        public ChipsData[] VarietyChip = new ChipsData[0];
    }
}
