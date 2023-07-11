using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MasaoPlus
{
	[XmlType("MasaoProject")]
	[Serializable]
	public class Project
	{
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

		public string Name = "";

		public double ProjVer;

		public Runtime Runtime = new Runtime();

        public bool Use3rdMapData = false;

        public string[] StageData = new string[0];

		public string[] StageData2 = new string[0];

		public string[] StageData3 = new string[0];

		public string[] StageData4 = new string[0];

		public string[] LayerData = new string[0];

		public string[] LayerData2 = new string[0];

		public string[] LayerData3 = new string[0];

		public string[] LayerData4 = new string[0];

		public string[] MapData = new string[0];

		public ConfigurationOwner Config = new ConfigurationOwner();
	}
}
