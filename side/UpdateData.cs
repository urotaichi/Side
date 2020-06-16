using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MasaoPlus
{
	// Token: 0x0200001B RID: 27
	[XmlType("SideUpdate")]
	[Serializable]
	public class UpdateData
	{
		// Token: 0x060000EF RID: 239 RVA: 0x0001710C File Offset: 0x0001530C
		public static UpdateData ParseXML(string file)
		{
			UpdateData result;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateData));
				using (FileStream fileStream = new FileStream(file, FileMode.Open))
				{
					UpdateData updateData = (UpdateData)xmlSerializer.Deserialize(fileStream);
					result = updateData;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("アップデート定義を開けませんでした。" + Environment.NewLine + ex.Message, "オープン失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				result = null;
			}
			return result;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00017194 File Offset: 0x00015394
		public void SaveXML(string file)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpdateData));
			try
			{
				using (FileStream fileStream = new FileStream(file, FileMode.Create))
				{
					xmlSerializer.Serialize(fileStream, this);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("更新データの保存に失敗しました。" + Environment.NewLine + ex.Message, "保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		// Token: 0x040000F9 RID: 249
		public string Name = "";

		// Token: 0x040000FA RID: 250
		public string Author = "";

		// Token: 0x040000FB RID: 251
		public string Update = "";

		// Token: 0x040000FC RID: 252
		public double DefVersion;

		// Token: 0x040000FD RID: 253
		public double RequireLower;

		// Token: 0x040000FE RID: 254
		public string Installer = "";
	}
}
