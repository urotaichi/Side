using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Generic;

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
                // まずファイルからバージョン情報を読み取る
                double version = 0.0;

                try
                {
                    using var reader = new System.Xml.XmlTextReader(file);
                    while (reader.Read())
                    {
                        if (reader.NodeType == System.Xml.XmlNodeType.Element)
                        {
                            if (reader.Name == "ProjVer")
                            {
                                reader.Read();
                                if (reader.NodeType == System.Xml.XmlNodeType.Text &&
                                    double.TryParse(reader.Value, out version))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // XMLの読み取りに失敗した場合はデフォルトの処理を続行
                }

                using FileStream fileStream = new(file, FileMode.Open);
                // バージョンと形式に基づいた読み込み方法の選択
                if (version < Global.definition.CProjVer)
                {
                    // 旧形式として読み込む
                    XmlSerializer legacySerializer = new(typeof(LegacyProject));
                    LegacyProject legacyProject = (LegacyProject)legacySerializer.Deserialize(fileStream);
                    legacyProject.Config.ConfigReady();
                    result = legacyProject.ToCurrentProject();
                }
                else
                {
                    // 現行形式で読み込む
                    XmlSerializer xmlSerializer = new(typeof(Project));
                    Project project = (Project)xmlSerializer.Deserialize(fileStream);
                    project.Config.ConfigReady();
                    result = project;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"プロジェクトファイルを開けませんでした。{Environment.NewLine}{ex.Message}", "オープン失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                result = null;
            }
            return result;
        }

        //public static Project ParseBinary(string file)
        //{
        //    BinaryFormatter binaryFormatter = new BinaryFormatter();
        //    Project result;
        //    using (FileStream fileStream = new FileStream(file, FileMode.Create))
        //    {
        //        Project project = (Project)binaryFormatter.Deserialize(fileStream);
        //        project.Config.ConfigReady();
        //        result = project;
        //    }
        //    return result;
        //}

        public void SaveXML(string file)
        {
            ProjVer = Global.definition.CProjVer;
            XmlSerializer xmlSerializer = new(typeof(Project));
            try
            {
                using FileStream fileStream = new(file, FileMode.Create);
                xmlSerializer.Serialize(fileStream, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"プロジェクトの保存に失敗しました。{Environment.NewLine}{ex.Message}", "保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        //public void SaveBinary(string file)
        //{
        //    BinaryFormatter binaryFormatter = new BinaryFormatter();
        //    using FileStream fileStream = new FileStream(file, FileMode.Create);
        //    using DeflateStream deflateStream = new DeflateStream(fileStream, CompressionMode.Compress);
        //    binaryFormatter.Serialize(deflateStream, this);
        //}

        public static void Convert3rdMapData(LayerObject StageData, int bytesize = 1)
        {
            for (int i = 0; i < StageData.Length; i++)
            {
                if (StageData[i].Contains(',')) continue;
                char[] array = StageData[i].ToCharArray();
                int[] array2 = new int[array.Length / bytesize];
                for (int j = 0; j < array.Length; j += bytesize)
                {
                    if (bytesize == 1)
                    {
                        if (array[j] == '.') array2[j] = 0;
                        else array2[j] = array[j];
                    }
                    else
                    {
                        char[] chararray = new char[bytesize];
                        for (int k = 0; k < bytesize; k++) chararray[k] = array[j + k];
                        var str = string.Join("", chararray);

                        if (str == "..") array2[j / bytesize] = 0;
                        else array2[j / bytesize] = int.Parse(str, System.Globalization.NumberStyles.HexNumber);
                    }
                }
                StageData[i] = string.Join(",", array2);
            }
        }

        public static void Convert2ndMapData(LayerObject StageData, int bytesize = 1)
        {
            for (int i = 0; i < StageData.Length; i++)
            {
                if (!StageData[i].Contains(',')) continue;
                string[] array = StageData[i].Split(',');
                for (int j = 0; j < array.Length; j++)
                {
                    if (int.TryParse(array[j], out int result))
                    {
                        string c = ChipDataClass.CodeToChar(result, bytesize);
                        if (bytesize == 1 && Global.MainWnd.MainDesigner.DrawItemRef.ContainsKey(c) || bytesize == 2 && Global.MainWnd.MainDesigner.DrawLayerRef.ContainsKey(c))
                        {
                            array[j] = c;
                        }
                        else
                        {
                            array[j] = ChipDataClass.CodeToChar(0, bytesize);
                        }
                    }
                    else
                    {
                        array[j] = ChipDataClass.CodeToChar(0, bytesize);
                    }
                }
                StageData[i] = string.Join("", array);
            }
        }

        private static void CheckStageSize(ref Runtime.DefinedData.StageSizeData size)
        {
            if (size.x < Global.state.MinimumStageSize.Width) size.x = Global.state.DefaultStageSize.Width;
            if (size.y < Global.state.MinimumStageSize.Height) size.y = Global.state.DefaultStageSize.Height;
        }

        private static void CheckStageSize(ref Runtime.DefinedData.LayerSizeData size)
        {
            if (size.x < Global.state.MinimumStageSize.Width) size.x = Global.state.DefaultStageSize.Width;
            if (size.y < Global.state.MinimumStageSize.Height) size.y = Global.state.DefaultStageSize.Height;
        }

        private static void SetStageSize(ref Runtime.DefinedData.StageSizeData size, Runtime.DefinedData.StageSizeData baseSize)
        {
            size.x = baseSize.x;
            size.y = baseSize.y;
            size.bytesize = baseSize.bytesize;
        }

        private static void SetStageSize(ref Runtime.DefinedData.LayerSizeData size, Runtime.DefinedData.StageSizeData baseSize)
        {
            size.x = baseSize.x;
            size.y = baseSize.y;
            size.bytesize = baseSize.bytesize;
        }

        private static void SetStageData(LayerObject data, int x, string character)
        {
            for (int i = 0; i < data.Length; i++)
            {
                StringBuilder stringBuilder = new();
                for (int j = 0; j < x; j++)
                {
                    stringBuilder.Append(character);
                }
                data[i] = stringBuilder.ToString();
            }
        }

        private static void SetLayerData(List<LayerObject> data, int x, string character)
        {
            foreach (var layer in data)
            {
                for (int i = 0; i < layer.Length; i++)
                {
                    StringBuilder stringBuilder = new();
                    for (int j = 0; j < x; j++)
                    {
                        stringBuilder.Append(character);
                    }
                    layer[i] = stringBuilder.ToString();
                }
            }
        }

        public static ChipDataClass SetAllStageData(Project project, string projPath, Project PrevProject = null)
        {
            Project baseProject = PrevProject ?? project;
            if (baseProject.Runtime.Definitions.LayerSize.bytesize != 0)
            {
                CheckStageSize(ref baseProject.Runtime.Definitions.LayerSize);
                SetStageSize(ref baseProject.Runtime.Definitions.LayerSize2, baseProject.Runtime.Definitions.LayerSize);
                SetStageSize(ref baseProject.Runtime.Definitions.LayerSize3, baseProject.Runtime.Definitions.LayerSize);
                SetStageSize(ref baseProject.Runtime.Definitions.LayerSize4, baseProject.Runtime.Definitions.LayerSize);
                project.LayerData = [[.. new string[baseProject.Runtime.Definitions.LayerSize.y]]];
                project.LayerData2 = [[.. new string[baseProject.Runtime.Definitions.LayerSize2.y]]];
                project.LayerData3 = [[.. new string[baseProject.Runtime.Definitions.LayerSize3.y]]];
                project.LayerData4 = [[.. new string[baseProject.Runtime.Definitions.LayerSize4.y]]];
            }
            CheckStageSize(ref baseProject.Runtime.Definitions.StageSize);
            SetStageSize(ref baseProject.Runtime.Definitions.StageSize2, baseProject.Runtime.Definitions.StageSize);
            SetStageSize(ref baseProject.Runtime.Definitions.StageSize3, baseProject.Runtime.Definitions.StageSize);
            SetStageSize(ref baseProject.Runtime.Definitions.StageSize4, baseProject.Runtime.Definitions.StageSize);
            project.StageData = [.. new string[baseProject.Runtime.Definitions.StageSize.y]];
            project.StageData2 = [.. new string[baseProject.Runtime.Definitions.StageSize2.y]];
            project.StageData3 = [.. new string[baseProject.Runtime.Definitions.StageSize3.y]];
            project.StageData4 = [.. new string[baseProject.Runtime.Definitions.StageSize4.y]];
            project.MapData = [.. new string[baseProject.Runtime.Definitions.MapSize.y]];
            ChipDataClass chipDataClass = ChipDataClass.ParseXML(Path.Combine(projPath, project.Runtime.Definitions.ChipDefinition));
            string character = chipDataClass.Mapchip[0].character;
            SetStageData(project.StageData, baseProject.Runtime.Definitions.StageSize.x, character);
            SetStageData(project.StageData2, baseProject.Runtime.Definitions.StageSize2.x, character);
            SetStageData(project.StageData3, baseProject.Runtime.Definitions.StageSize3.x, character);
            SetStageData(project.StageData4, baseProject.Runtime.Definitions.StageSize4.x, character);
            character = chipDataClass.WorldChip[0].character;
            SetStageData(project.MapData, baseProject.Runtime.Definitions.MapSize.x, character);
            if (baseProject.Runtime.Definitions.LayerSize.bytesize != 0)
            {
                character = chipDataClass.Layerchip[0].character;
                SetLayerData(project.LayerData, baseProject.Runtime.Definitions.LayerSize.x, character);
                SetLayerData(project.LayerData2, baseProject.Runtime.Definitions.LayerSize2.x, character);
                SetLayerData(project.LayerData3, baseProject.Runtime.Definitions.LayerSize3.x, character);
                SetLayerData(project.LayerData4, baseProject.Runtime.Definitions.LayerSize4.x, character);
            }
            return chipDataClass;
        }

        public string Name = "";

        public double ProjVer;

        public Runtime Runtime = new();

        public bool Use3rdMapData = false;

        public ChipsData[] CustomPartsDefinition;

        public LayerObject StageData = [];

        public LayerObject StageData2 = [];

        public LayerObject StageData3 = [];

        public LayerObject StageData4 = [];

        [XmlArray("LayerData")]
        [XmlArrayItem("LayerObject")]
        public List<LayerObject> LayerData = [];

        [XmlArray("LayerData2")]
        [XmlArrayItem("LayerObject")]
        public List<LayerObject> LayerData2 = [];

        [XmlArray("LayerData3")]
        [XmlArrayItem("LayerObject")]
        public List<LayerObject> LayerData3 = [];

        [XmlArray("LayerData4")]
        [XmlArrayItem("LayerObject")]
        public List<LayerObject> LayerData4 = [];
        public LayerObject MapData = [];

        public ConfigurationOwner Config = new();
    }
}
