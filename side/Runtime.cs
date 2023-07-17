using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MasaoPlus
{
    [XmlType("MasaoRuntime")]
    [Serializable]
    public class Runtime
    {
        public static Runtime ParseXML(string file)
        {
            Runtime result;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Runtime));
                using FileStream fileStream = new FileStream(file, FileMode.Open);
                Runtime runtime = (Runtime)xmlSerializer.Deserialize(fileStream);
                result = runtime;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ランタイム定義を開けませんでした。{Environment.NewLine}{ex.Message}", "オープン失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                result = null;
            }
            return result;
        }

        public static string[] CheckFiles(string cdir, Runtime chkr)
        {
            return CheckFiles(cdir, chkr, true);
        }

        public static string[] CheckFiles(string cdir, Runtime chkr, bool CheckConfig)
        {
            string[] array = new string[]
            {
                chkr.Definitions.Package,
                chkr.Definitions.Configurations,
                chkr.Definitions.ChipExtender
            };
            if (!CheckConfig)
            {
                array = new string[]
                {
                    chkr.Definitions.Package,
                    chkr.Definitions.ChipExtender
                };
            }
            List<string> list = new List<string>();
            foreach (string text in array)
            {
                if (!File.Exists(Path.Combine(cdir, text)))
                {
                    list.Add(text);
                }
            }
            return list.ToArray();
        }

        public DefinedData Definitions = new DefinedData();

        public ConfigurationData DefaultConfigurations = new ConfigurationData();

        [Serializable]
        public class DefinedData
        {
            public string Name = "";

            public string Author = "";

            public string Update = "";

            public string Package = "";

            public string Configurations = "";

            public string ChipDefinition = "";

            public string ChipExtender = "";

            public string Chipset = "";

            public double DefVersion;

            public double RequireLower;

            public Size ChipSize = Size.Empty;

            public int StageSplit;

            public string ParamName = "";

            public string ParamName2 = "";

            public string ParamName3 = "";

            public string ParamName4 = "";

            public int LayerSplit;

            public string LayerName = "";

            public string LayerName2 = "";

            public string LayerName3 = "";

            public string LayerName4 = "";

            public string MapName = "";

            public StageSizeData StageSize = new StageSizeData();

            public StageSizeData LayerSize = new StageSizeData();

            public StageSizeData MapSize = new StageSizeData();

            public int MaxAthleticNumber;

            [Serializable]
            public class StageSizeData
            {
                [XmlIgnore]
                public int StageByteWidth
                {
                    get
                    {
                        if (Global.cpd.project.Use3rdMapData && !Global.state.MapEditMode)
                        {
                            return x;
                        }
                        else
                        {
                            return x * bytesize;
                        }
                    }
                }

                [XmlAttribute]
                public int x;

                [XmlAttribute]
                public int y;

                [XmlAttribute]
                public int bytesize;
            }
        }

        [Serializable]
        public class ConfigurationData
        {
            public HTMLReplaceData[] OutputReplace = new HTMLReplaceData[0];

            public string HeaderHTML = "";

            public string MiddleHTML = "";

            public string FooterHTML = "";

            public string Parameter = "";

            public string StageParam = "";

            public string StageParam2 = "";

            public string StageParam3 = "";

            public string StageParam4 = "";

            public string LayerParam = "";

            public string LayerParam2 = "";

            public string LayerParam3 = "";

            public string LayerParam4 = "";

            public string MapParam = "";

            public string FileExt = "";

            public string OutputDir = "";

            public string RunFile = "";
        }
    }
}
