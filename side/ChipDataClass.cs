using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MasaoPlus
{
	// Token: 0x02000020 RID: 32
	[XmlType("ChipDefinition")]
	[Serializable]
	public class ChipDataClass
	{
		// Token: 0x060000FA RID: 250 RVA: 0x00017508 File Offset: 0x00015708
		public static ChipDataClass ParseXML(string file)
		{
			ChipDataClass result;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(ChipDataClass));
				using (FileStream fileStream = new FileStream(file, FileMode.Open))
				{
					ChipDataClass chipDataClass = (ChipDataClass)xmlSerializer.Deserialize(fileStream);
					result = chipDataClass;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("チップ定義を開けませんでした。" + Environment.NewLine + ex.Message, "オープン失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				result = null;
			}
			return result;
		}

		// Token: 0x0400012D RID: 301
		public ChipsData[] Mapchip = new ChipsData[0];

		// Token: 0x0400012E RID: 302
		public ChipsData[] Layerchip = new ChipsData[0];

		// Token: 0x0400012F RID: 303
		public ChipsData[] WorldChip = new ChipsData[0];
	}
}
