using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MasaoPlus
{
    [XmlType("Configurations")]
    [Serializable]
    public class ConfigurationOwner
    {
        public static ConfigurationOwner LoadXML(string file)
        {
            ConfigurationOwner result;
            try
            {
                XmlSerializer xmlSerializer = new(typeof(ConfigurationOwner));
                using FileStream fileStream = new(file, FileMode.Open);
                ConfigurationOwner configurationOwner = (ConfigurationOwner)xmlSerializer.Deserialize(fileStream);
                configurationOwner.ConfigReady();
                result = configurationOwner;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定定義ファイルを開けませんでした。{Environment.NewLine}{ex.Message}", "オープン失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                result = null;
            }
            return result;
        }

        // パラメータ設定読み込み
        public void ConfigReady()
        {
            Categories = [];
            RelIndex = new int[29];
            bool[] array = new bool[12];
            array[0] = false;
            array[1] = false;
            array[2] = false;
            array[3] = false;
            array[4] = false;
            array[5] = false;
            array[6] = false;
            array[7] = false;
            for (int i = 0; i < Configurations.Length; i++)
            {
                ConfigParam configParam = Configurations[i];
                if (configParam.Category == null)
                {
                    Configurations[i].Category = "未設定";
                }
                if (!Categories.Contains(configParam.Category))
                {
                    Categories.Add(configParam.Category);
                }
                if (configParam.ChipRelation != null && configParam.ChipRelation != "")
                {
                    if (Global.state.ChipRegister.ContainsKey(configParam.ChipRelation))
                    {
                        Global.state.ChipRegister[configParam.ChipRelation] = configParam.Value;
                    }
                    else
                    {
                        Global.state.ChipRegister.Add(configParam.ChipRelation, configParam.Value);
                    }
                }
                switch (configParam.Relation)
                {
                    case "BACKGROUND":
                        RelIndex[0] = i;
                        array[0] = true;
                        break;
                    case "BACKGROUND2":
                        RelIndex[1] = i;
                        array[1] = true;
                        break;
                    case "BACKGROUND3":
                        RelIndex[2] = i;
                        array[2] = true;
                        break;
                    case "BACKGROUND4":
                        RelIndex[3] = i;
                        array[3] = true;
                        break;
                    case "BACKGROUNDM":
                        RelIndex[4] = i;
                        array[4] = true;
                        break;
                    case "TITLE":
                        RelIndex[5] = i;
                        array[5] = true;
                        break;
                    case "ENDING":
                        RelIndex[6] = i;
                        array[6] = true;
                        break;
                    case "GAMEOVER":
                        RelIndex[7] = i;
                        array[7] = true;
                        break;
                    case "PATTERN":
                        RelIndex[8] = i;
                        array[8] = true;
                        break;
                    case "LAYERCHIP":
                        RelIndex[9] = i;
                        break;
                    case "STAGENUM":
                        RelIndex[10] = i;
                        array[9] = true;
                        break;
                    case "STAGESTART":
                        RelIndex[11] = i;
                        array[10] = true;
                        break;
                    case "STAGESELECT":
                        RelIndex[12] = i;
                        array[11] = true;
                        break;
                }
                switch (configParam.Name)
                {
                    case "grenade_@1":
                        RelIndex[13] = i;
                        break;
                    case "grenade_@2":
                        RelIndex[14] = i;
                        break;
                    case "mizunohadou_@":
                        RelIndex[15] = i;
                        break;
                    case "firebar_@1":
                        RelIndex[16] = i;
                        break;
                    case "firebar_@2":
                        RelIndex[17] = i;
                        break;
                    case "filename_oriboss_left1":
                        RelIndex[18] = i;
                        break;
                    case "filename_haikei":
                        RelIndex[19] = i;
                        break;
                    case "filename_haikei2":
                        RelIndex[20] = i;
                        break;
                    case "filename_haikei3":
                        RelIndex[21] = i;
                        break;
                    case "filename_haikei4":
                        RelIndex[22] = i;
                        break;
                    case "filename_second_haikei":
                        RelIndex[23] = i;
                        break;
                    case "filename_second_haikei2":
                        RelIndex[24] = i;
                        break;
                    case "filename_second_haikei3":
                        RelIndex[25] = i;
                        break;
                    case "filename_second_haikei4":
                        RelIndex[26] = i;
                        break;
                    case "filename_chizu":
                        RelIndex[27] = i;
                        break;
                    case "mcs_screen_size":
                        RelIndex[28] = i;
                        break;
                }
            }
            bool[] array2 = array;
            for (int j = 0; j < array2.Length; j++)
            {
                if (!array2[j])
                {
                    throw new Exception("必須設定が含まれていません。");
                }
            }
        }

        // ヘルパーメソッド
        private Color GetColorValue(int index)
        {
            Colors colors = new(Configurations[RelIndex[index]].Value);
            return colors.c;
        }

        private void SetColorValue(int index, Color value)
        {
            Colors colors = default;
            colors.c = value;
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
