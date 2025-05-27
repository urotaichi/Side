using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MasaoPlus
{
    [XmlType("Configurations")]
    [Serializable]
    public class ConfigurationOwner
    {
        // 必須設定のマッピング
        private static readonly Dictionary<string, (int Index, int RequiredFlag)> RelationMapping = new()
        {
            { "BACKGROUND", (0, 0) },
            { "BACKGROUND2", (1, 1) },
            { "BACKGROUND3", (2, 2) },
            { "BACKGROUND4", (3, 3) },
            { "BACKGROUNDM", (4, 4) },
            { "TITLE", (5, 5) },
            { "ENDING", (6, 6) },
            { "GAMEOVER", (7, 7) },
            { "PATTERN", (8, 8) },
            { "LAYERCHIP", (9, -1) },
            { "STAGENUM", (10, 9) },
            { "STAGESTART", (11, 10) },
            { "STAGESELECT", (12, 11) }
        };

        private static readonly Dictionary<string, int> NameMapping = new()
        {
            { "grenade_@1", 13 },
            { "grenade_@2", 14 },
            { "mizunohadou_@", 15 },
            { "firebar_@1", 16 },
            { "firebar_@2", 17 },
            { "filename_oriboss_left1", 18 },
            { "filename_haikei", 19 },
            { "filename_haikei2", 20 },
            { "filename_haikei3", 21 },
            { "filename_haikei4", 22 },
            { "filename_second_haikei", 23 },
            { "filename_second_haikei2", 24 },
            { "filename_second_haikei3", 25 },
            { "filename_second_haikei4", 26 },
            { "filename_chizu", 27 },
            { "mcs_screen_size", 28 }
        };

        public static ConfigurationOwner LoadXML(string file)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(ConfigurationOwner));
                using var fileStream = new FileStream(file, FileMode.Open);
                var configurationOwner = (ConfigurationOwner)xmlSerializer.Deserialize(fileStream);
                configurationOwner.ConfigReady();
                return configurationOwner;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定定義ファイルを開けませんでした。{Environment.NewLine}{ex.Message}", 
                    "オープン失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return null;
            }
        }

        // パラメータ設定読み込み
        public void ConfigReady()
        {
            Categories = [];
            RelIndex = new int[29];
            var requiredFlags = new bool[12]; // デフォルトで全てfalse

            for (int i = 0; i < Configurations.Length; i++)
            {
                var configParam = Configurations[i];
                
                // カテゴリ設定
                ProcessCategory(configParam, i);
                
                // チップ関係設定
                ProcessChipRelation(configParam);
                
                // Relation設定
                ProcessRelation(configParam, i, requiredFlags);
                
                // Name設定
                ProcessName(configParam, i);
            }

            ValidateRequiredSettings(requiredFlags);
        }

        private void ProcessCategory(ConfigParam configParam, int index)
        {
            if (string.IsNullOrEmpty(configParam.Category))
            {
                Configurations[index].Category = "未設定";
            }
            
            if (!Categories.Contains(configParam.Category))
            {
                Categories.Add(configParam.Category);
            }
        }

        private static void ProcessChipRelation(ConfigParam configParam)
        {
            if (string.IsNullOrEmpty(configParam.ChipRelation)) return;

            var register = Global.state.ChipRegister;
            if (!register.TryAdd(configParam.ChipRelation, configParam.Value))
            {
                register[configParam.ChipRelation] = configParam.Value;
            }
        }

        private void ProcessRelation(ConfigParam configParam, int index, bool[] requiredFlags)
        {
            if (string.IsNullOrEmpty(configParam.Relation)) return;

            if (RelationMapping.TryGetValue(configParam.Relation, out var mapping))
            {
                RelIndex[mapping.Index] = index;
                if (mapping.RequiredFlag >= 0)
                {
                    requiredFlags[mapping.RequiredFlag] = true;
                }
            }
        }

        private void ProcessName(ConfigParam configParam, int index)
        {
            if (string.IsNullOrEmpty(configParam.Name)) return;

            if (NameMapping.TryGetValue(configParam.Name, out var mappingIndex))
            {
                RelIndex[mappingIndex] = index;
            }
        }

        private static void ValidateRequiredSettings(bool[] requiredFlags)
        {
            if (requiredFlags.Any(flag => !flag))
            {
                throw new Exception("必須設定が含まれていません。");
            }
        }

        // ヘルパーメソッド
        private Color GetColorValue(int index)
        {
            var colors = new Colors(Configurations[RelIndex[index]].Value);
            return colors.c;
        }

        private void SetColorValue(int index, Color value)
        {
            var colors = new Colors { c = value };
            Configurations[RelIndex[index]].Value = colors.ToString();
        }

        private string GetStringValue(int index)
        {
            return Configurations[RelIndex[index]].Value;
        }

        private void SetStringValue(int index, string value)
        {
            Configurations[RelIndex[index]].Value = value;
        }

        private string GetOptionalStringValue(int index)
        {
            return RelIndex[index] != default ? Configurations[RelIndex[index]].Value : null;
        }

        // プロパティ群
        [XmlIgnore]
        public Color Background
        {
            get => GetColorValue(0);
            set => SetColorValue(0, value);
        }

        [XmlIgnore]
        public Color Background2
        {
            get => GetColorValue(1);
            set => SetColorValue(1, value);
        }

        [XmlIgnore]
        public Color Background3
        {
            get => GetColorValue(2);
            set => SetColorValue(2, value);
        }

        [XmlIgnore]
        public Color Background4
        {
            get => GetColorValue(3);
            set => SetColorValue(3, value);
        }

        [XmlIgnore]
        public Color BackgroundM
        {
            get => GetColorValue(4);
            set => SetColorValue(4, value);
        }

        [XmlIgnore]
        public string TitleImage
        {
            get => GetStringValue(5);
            set => SetStringValue(5, value);
        }

        [XmlIgnore]
        public string EndingImage
        {
            get => GetStringValue(6);
            set => SetStringValue(6, value);
        }

        [XmlIgnore]
        public string GameoverImage
        {
            get => GetStringValue(7);
            set => SetStringValue(7, value);
        }

        [XmlIgnore]
        public string PatternImage
        {
            get => GetStringValue(8);
            set => SetStringValue(8, value);
        }

        [XmlIgnore]
        public string LayerImage
        {
            get => GetStringValue(9);
            set => SetStringValue(9, value);
        }

        [XmlIgnore]
        public int StageNum
        {
            get => int.Parse(GetStringValue(10));
            set => SetStringValue(10, value.ToString());
        }

        [XmlIgnore]
        public int StageStart
        {
            get => int.Parse(GetStringValue(11));
            set => SetStringValue(11, value.ToString());
        }

        [XmlIgnore]
        public bool UseWorldmap
        {
            get => GetStringValue(12) == "2";
            set => SetStringValue(12, value ? "2" : "1");
        }

        [XmlIgnore]
        public Color Grenade1
        {
            get => GetColorValue(13);
            set => SetColorValue(13, value);
        }

        [XmlIgnore]
        public Color Grenade2
        {
            get => GetColorValue(14);
            set => SetColorValue(14, value);
        }

        [XmlIgnore]
        public Color Mizunohadou
        {
            get => GetColorValue(15);
            set => SetColorValue(15, value);
        }

        [XmlIgnore]
        public Color Firebar1
        {
            get => GetColorValue(16);
            set => SetColorValue(16, value);
        }

        [XmlIgnore]
        public Color Firebar2
        {
            get => GetColorValue(17);
            set => SetColorValue(17, value);
        }

        [XmlIgnore]
        public string OribossImage
        {
            get => GetOptionalStringValue(18);
            set => SetStringValue(18, value);
        }

        [XmlIgnore]
        public string HaikeiImage
        {
            get => GetOptionalStringValue(19);
            set => SetStringValue(19, value);
        }

        [XmlIgnore]
        public string HaikeiImage2
        {
            get => GetOptionalStringValue(20);
            set => SetStringValue(20, value);
        }

        [XmlIgnore]
        public string HaikeiImage3
        {
            get => GetOptionalStringValue(21);
            set => SetStringValue(21, value);
        }

        [XmlIgnore]
        public string HaikeiImage4
        {
            get => GetOptionalStringValue(22);
            set => SetStringValue(22, value);
        }

        [XmlIgnore]
        public string SecondHaikeiImage
        {
            get => GetOptionalStringValue(23);
            set => SetStringValue(23, value);
        }

        [XmlIgnore]
        public string SecondHaikeiImage2
        {
            get => GetOptionalStringValue(24);
            set => SetStringValue(24, value);
        }

        [XmlIgnore]
        public string SecondHaikeiImage3
        {
            get => GetOptionalStringValue(25);
            set => SetStringValue(25, value);
        }

        [XmlIgnore]
        public string SecondHaikeiImage4
        {
            get => GetOptionalStringValue(26);
            set => SetStringValue(26, value);
        }

        [XmlIgnore]
        public string ChizuImage
        {
            get => GetStringValue(27);
            set => SetStringValue(27, value);
        }

        [XmlIgnore]
        public string ScreenSize
        {
            get => GetStringValue(28);
            set => SetStringValue(28, value);
        }

        [XmlElement("param")]
        public ConfigParam[] Configurations = [];

        [XmlIgnore]
        public List<string> Categories = [];

        [XmlIgnore]
        private int[] RelIndex;
    }
}
