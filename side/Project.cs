using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MasaoPlus
{
	// Token: 0x02000023 RID: 35
	[XmlType("MasaoProject")]
	[Serializable]
	public class Project
	{
		// Token: 0x060000FD RID: 253 RVA: 0x00017670 File Offset: 0x00015870
		public static Project ParseXML(string file)
		{
			Project result;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Project));
				using (FileStream fileStream = new FileStream(file, FileMode.Open))
				{
					Project project = (Project)xmlSerializer.Deserialize(fileStream);
					project.Config.ConfigReady();
					result = project;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("プロジェクトファイルを開けませんでした。" + Environment.NewLine + ex.Message, "オープン失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				result = null;
			}
			return result;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00017704 File Offset: 0x00015904
		public static Project ParseBinary(string file)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			Project result;
			using (FileStream fileStream = new FileStream(file, FileMode.Create))
			{
				Project project = (Project)binaryFormatter.Deserialize(fileStream);
				project.Config.ConfigReady();
				result = project;
			}
			return result;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00017758 File Offset: 0x00015958
		public void SaveXML(string file)
		{
			this.ProjVer = Global.definition.CProjVer;
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Project));
			try
			{
				using (FileStream fileStream = new FileStream(file, FileMode.Create))
				{
					xmlSerializer.Serialize(fileStream, this);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("プロジェクトの保存に失敗しました。" + Environment.NewLine + ex.Message, "保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x000177E8 File Offset: 0x000159E8
		public void SaveBinary(string file)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using (FileStream fileStream = new FileStream(file, FileMode.Create))
			{
				using (DeflateStream deflateStream = new DeflateStream(fileStream, CompressionMode.Compress))
				{
					binaryFormatter.Serialize(deflateStream, this);
				}
			}
		}

		// Token: 0x0400013C RID: 316
		public string Name = "";

		// Token: 0x0400013D RID: 317
		public double ProjVer;

		// Token: 0x0400013E RID: 318
		public Runtime Runtime = new Runtime();

		// Token: 0x0400013F RID: 319
		public string[] StageData = new string[0];

		// Token: 0x04000140 RID: 320
		public string[] StageData2 = new string[0];

		// Token: 0x04000141 RID: 321
		public string[] StageData3 = new string[0];

		// Token: 0x04000142 RID: 322
		public string[] StageData4 = new string[0];

		// Token: 0x04000143 RID: 323
		public string[] LayerData = new string[0];

		// Token: 0x04000144 RID: 324
		public string[] LayerData2 = new string[0];

		// Token: 0x04000145 RID: 325
		public string[] LayerData3 = new string[0];

		// Token: 0x04000146 RID: 326
		public string[] LayerData4 = new string[0];

		// Token: 0x04000147 RID: 327
		public string[] MapData = new string[0];

		// Token: 0x04000148 RID: 328
		public ConfigurationOwner Config = new ConfigurationOwner();
	}
}
