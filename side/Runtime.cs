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
                XmlSerializer xmlSerializer = new(typeof(Runtime));
                using FileStream fileStream = new(file, FileMode.Open);
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
            string[] array =
            [
                chkr.Definitions.Package,
                chkr.Definitions.Configurations,
                chkr.Definitions.ChipExtender
            ];
            if (!CheckConfig)
            {
                array =
                [
                    chkr.Definitions.Package,
                    chkr.Definitions.ChipExtender
                ];
            }
            List<string> list = [];
            foreach (string text in array)
            {
                if (!File.Exists(Path.Combine(cdir, text)))
                {
                    list.Add(text);
                }
            }
            return [.. list];
        }

        public DefinedData Definitions = new();

        public ConfigurationData DefaultConfigurations = new();

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

            public StageSizeData StageSize = new();
            public StageSizeData StageSize2 = new();
            public StageSizeData StageSize3 = new();
            public StageSizeData StageSize4 = new();

            public StageSizeData LayerSize = new();
            public StageSizeData LayerSize2 = new();
            public StageSizeData LayerSize3 = new();
            public StageSizeData LayerSize4 = new();

            public StageSizeData MapSize = new();

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
            public HTMLReplaceData[] OutputReplace = [];

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
