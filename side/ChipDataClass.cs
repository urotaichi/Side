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
				MessageBox.Show("チップ定義を開けませんでした。" + Environment.NewLine + ex.Message, "オープン失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				result = null;
			}
			return result;
		}

		public ChipsData[] Mapchip = new ChipsData[0];

		public ChipsData[] Layerchip = new ChipsData[0];

		public ChipsData[] WorldChip = new ChipsData[0];

        public ChipsData[] VarietyChip = new ChipsData[0];
    }
}
