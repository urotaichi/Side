using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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
                XmlSerializer xmlSerializer = new(typeof(Project));
                using FileStream fileStream = new(file, FileMode.Open);
                Project project = (Project)xmlSerializer.Deserialize(fileStream);
                project.Config.ConfigReady();
                result = project;
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

        public static void Convert3rdMapData(string[] StageData, int bytesize = 1)
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

        public static void Convert2ndMapData(string[] StageData, int bytesize = 1)
        {
            for (int i = 0; i < StageData.Length; i++)
            {
                if (!StageData[i].Contains(',')) continue;
                string[] array = StageData[i].Split(',');
                for (int j = 0; j < array.Length; j ++)
                {
                    if(int.TryParse(array[j], out int result))
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

        private static void SetStageSize(ref Runtime.DefinedData.StageSizeData size, Runtime.DefinedData.StageSizeData baseSize)
        {
            size.x = baseSize.x;
            size.y = baseSize.y;
            size.bytesize = baseSize.bytesize;
        }

        private static void SetStageData(string[] data, int x, string character)
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

        public static ChipDataClass SetAllStageData(Project project, string projPath, Project PrevProject = null)
        {
            Project baseProject = PrevProject ?? project;
            if (baseProject.Runtime.Definitions.LayerSize.bytesize != 0)
            {
                if (baseProject.Runtime.Definitions.LayerSize.x < Global.state.MinimumStageSize.Width) baseProject.Runtime.Definitions.LayerSize.x = Global.state.DefaultStageSize.Width;
                if (baseProject.Runtime.Definitions.LayerSize.y < Global.state.MinimumStageSize.Height) baseProject.Runtime.Definitions.LayerSize.y = Global.state.DefaultStageSize.Height;
                SetStageSize(ref baseProject.Runtime.Definitions.LayerSize2, baseProject.Runtime.Definitions.LayerSize);
                SetStageSize(ref baseProject.Runtime.Definitions.LayerSize3, baseProject.Runtime.Definitions.LayerSize);
                SetStageSize(ref baseProject.Runtime.Definitions.LayerSize4, baseProject.Runtime.Definitions.LayerSize);
                project.LayerData = new string[baseProject.Runtime.Definitions.LayerSize.y];
                project.LayerData2 = new string[baseProject.Runtime.Definitions.LayerSize2.y];
                project.LayerData3 = new string[baseProject.Runtime.Definitions.LayerSize3.y];
                project.LayerData4 = new string[baseProject.Runtime.Definitions.LayerSize4.y];
            }
            if (baseProject.Runtime.Definitions.StageSize.x < Global.state.MinimumStageSize.Width) baseProject.Runtime.Definitions.StageSize.x = Global.state.DefaultStageSize.Width;
            if (baseProject.Runtime.Definitions.StageSize.y < Global.state.MinimumStageSize.Height) baseProject.Runtime.Definitions.StageSize.y = Global.state.DefaultStageSize.Height;
            SetStageSize(ref baseProject.Runtime.Definitions.StageSize2, baseProject.Runtime.Definitions.StageSize);
            SetStageSize(ref baseProject.Runtime.Definitions.StageSize3, baseProject.Runtime.Definitions.StageSize);
            SetStageSize(ref baseProject.Runtime.Definitions.StageSize4, baseProject.Runtime.Definitions.StageSize);
            project.StageData = new string[baseProject.Runtime.Definitions.StageSize.y];
            project.StageData2 = new string[baseProject.Runtime.Definitions.StageSize2.y];
            project.StageData3 = new string[baseProject.Runtime.Definitions.StageSize3.y];
            project.StageData4 = new string[baseProject.Runtime.Definitions.StageSize4.y];
            project.MapData = new string[baseProject.Runtime.Definitions.MapSize.y];
            ChipDataClass chipDataClass = ChipDataClass.ParseXML(Path.Combine(Path.GetDirectoryName(projPath), project.Runtime.Definitions.ChipDefinition));
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
                SetStageData(project.LayerData, baseProject.Runtime.Definitions.LayerSize.x, character);
                SetStageData(project.LayerData2, baseProject.Runtime.Definitions.LayerSize2.x, character);
                SetStageData(project.LayerData3, baseProject.Runtime.Definitions.LayerSize3.x, character);
                SetStageData(project.LayerData4, baseProject.Runtime.Definitions.LayerSize4.x, character);
            }
            return chipDataClass;
        }

        public string Name = "";

        public double ProjVer;

        public Runtime Runtime = new();

        public bool Use3rdMapData = false;

        public ChipsData[] CustomPartsDefinition;

        public string[] StageData = [];

        public string[] StageData2 = [];

        public string[] StageData3 = [];

        public string[] StageData4 = [];

        public string[] LayerData = [];

        public string[] LayerData2 = [];

        public string[] LayerData3 = [];

        public string[] LayerData4 = [];

        public string[] MapData = [];

        public ConfigurationOwner Config = new();
    }
}
