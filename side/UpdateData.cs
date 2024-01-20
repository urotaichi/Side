using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MasaoPlus
{
    [XmlType("SideUpdate")]
    [Serializable]
    public class UpdateData
    {
        public static UpdateData ParseXML(string file)
        {
            UpdateData result;
            try
            {
                XmlSerializer xmlSerializer = new(typeof(UpdateData));
                using FileStream fileStream = new(file, FileMode.Open);
                UpdateData updateData = (UpdateData)xmlSerializer.Deserialize(fileStream);
                result = updateData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"アップデート定義を開けませんでした。{Environment.NewLine}{ex.Message}", "オープン失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                result = null;
            }
            return result;
        }

        public void SaveXML(string file)
        {
            XmlSerializer xmlSerializer = new(typeof(UpdateData));
            try
            {
                using FileStream fileStream = new(file, FileMode.Create);
                xmlSerializer.Serialize(fileStream, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新データの保存に失敗しました。{Environment.NewLine}{ex.Message}", "保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        public string Name = "";

        public string Author = "";

        public string Update = "";

        public double DefVersion;

        public double RequireLower;

        public string Installer = "";
    }
}
