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
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConfigurationOwner));
                using FileStream fileStream = new FileStream(file, FileMode.Open);
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
			Categories = new List<string>();
			RelIndex = new int[28];
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

		[XmlIgnore]
		public Color Background
		{
			get
			{
				Colors colors = new Colors(Configurations[RelIndex[0]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default;
				colors.c = value;
				Configurations[RelIndex[0]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public Color Background2
		{
			get
			{
				Colors colors = new Colors(Configurations[RelIndex[1]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default;
				colors.c = value;
				Configurations[RelIndex[1]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public Color Background3
		{
			get
			{
				Colors colors = new Colors(Configurations[RelIndex[2]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default;
				colors.c = value;
				Configurations[RelIndex[2]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public Color Background4
		{
			get
			{
				Colors colors = new Colors(Configurations[RelIndex[3]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default;
				colors.c = value;
				Configurations[RelIndex[3]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public Color BackgroundM
		{
			get
			{
				Colors colors = new Colors(Configurations[RelIndex[4]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default;
				colors.c = value;
				Configurations[RelIndex[4]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public string TitleImage
		{
			get
			{
				return Configurations[RelIndex[5]].Value;
			}
			set
			{
				Configurations[RelIndex[5]].Value = value;
			}
		}

		[XmlIgnore]
		public string EndingImage
		{
			get
			{
				return Configurations[RelIndex[6]].Value;
			}
			set
			{
				Configurations[RelIndex[6]].Value = value;
			}
		}

		[XmlIgnore]
		public string GameoverImage
		{
			get
			{
				return Configurations[RelIndex[7]].Value;
			}
			set
			{
				Configurations[RelIndex[7]].Value = value;
			}
		}

		[XmlIgnore]
		public string PatternImage
		{
			get
			{
				return Configurations[RelIndex[8]].Value;
			}
			set
			{
				Configurations[RelIndex[8]].Value = value;
			}
		}

		[XmlIgnore]
		public string LayerImage
		{
			get
			{
				return Configurations[RelIndex[9]].Value;
			}
			set
			{
				Configurations[RelIndex[9]].Value = value;
			}
		}

		[XmlIgnore]
		public int StageNum
		{
			get
			{
				return int.Parse(Configurations[RelIndex[10]].Value);
			}
			set
			{
				Configurations[RelIndex[10]].Value = value.ToString();
			}
		}

		[XmlIgnore]
		public int StageStart
		{
			get
			{
				return int.Parse(Configurations[RelIndex[11]].Value);
			}
			set
			{
				Configurations[RelIndex[11]].Value = value.ToString();
			}
		}

		[XmlIgnore]
		public bool UseWorldmap
		{
			get
			{
				return Configurations[RelIndex[12]].Value == "2";
			}
			set
			{
				if (value)
				{
					Configurations[RelIndex[12]].Value = "2";
					return;
				}
				Configurations[RelIndex[12]].Value = "1";
			}
        }

        [XmlIgnore]
		public Color Grenade1
		{
			get
			{
				Colors colors = new Colors(Configurations[RelIndex[13]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default;
				colors.c = value;
				Configurations[RelIndex[13]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public Color Grenade2
		{
			get
			{
				Colors colors = new Colors(Configurations[RelIndex[14]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default;
				colors.c = value;
				Configurations[RelIndex[14]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public Color Mizunohadou
		{
			get
			{
				Colors colors = new Colors(Configurations[RelIndex[15]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default;
				colors.c = value;
				Configurations[RelIndex[15]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public Color Firebar1
		{
			get
			{
				Colors colors = new Colors(Configurations[RelIndex[16]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default;
				colors.c = value;
				Configurations[RelIndex[16]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public Color Firebar2
		{
			get
			{
				Colors colors = new Colors(Configurations[RelIndex[17]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default;
				colors.c = value;
				Configurations[RelIndex[17]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public string OribossImage
		{
			get
			{
				if (RelIndex[18] != default) return Configurations[RelIndex[18]].Value;
				else return null;
			}
			set
			{
				Configurations[RelIndex[18]].Value = value;
			}
		}

		[XmlIgnore]
		public string HaikeiImage
		{
			get
			{
				if (RelIndex[19] != default) return Configurations[RelIndex[19]].Value;
				else return null;
			}
			set
			{
				Configurations[RelIndex[19]].Value = value;
			}
		}

		[XmlIgnore]
		public string HaikeiImage2
		{
			get
			{
				if (RelIndex[20] != default) return Configurations[RelIndex[20]].Value;
				else return null;
			}
			set
			{
				Configurations[RelIndex[20]].Value = value;
			}
		}

		[XmlIgnore]
		public string HaikeiImage3
		{
			get
			{
				if (RelIndex[21] != default) return Configurations[RelIndex[21]].Value;
				else return null;
			}
			set
			{
				Configurations[RelIndex[21]].Value = value;
			}
		}

		[XmlIgnore]
		public string HaikeiImage4
		{
			get
			{
				if (RelIndex[22] != default) return Configurations[RelIndex[22]].Value;
				else return null;
			}
			set
			{
				Configurations[RelIndex[22]].Value = value;
			}
		}

		[XmlIgnore]
		public string SecondHaikeiImage
		{
			get
			{
				if (RelIndex[23] != default) return Configurations[RelIndex[23]].Value;
				else return null;
			}
			set
			{
				Configurations[RelIndex[23]].Value = value;
			}
		}

		[XmlIgnore]
		public string SecondHaikeiImage2
		{
			get
			{
				if (RelIndex[24] != default) return Configurations[RelIndex[24]].Value;
				else return null;
			}
			set
			{
				Configurations[RelIndex[24]].Value = value;
			}
		}

		[XmlIgnore]
		public string SecondHaikeiImage3
		{
			get
			{
				if (RelIndex[25] != default) return Configurations[RelIndex[25]].Value;
				else return null;
			}
			set
			{
				Configurations[RelIndex[25]].Value = value;
			}
		}

		[XmlIgnore]
		public string SecondHaikeiImage4
		{
			get
			{
				if (RelIndex[26] != default) return Configurations[RelIndex[26]].Value;
				else return null;
			}
			set
			{
				Configurations[RelIndex[26]].Value = value;
			}
		}

		[XmlIgnore]
		public string ChizuImage
		{
			get
			{
				return Configurations[RelIndex[27]].Value;
			}
			set
			{
				Configurations[RelIndex[27]].Value = value;
			}
		}

		[XmlElement("param")]
		public ConfigParam[] Configurations = new ConfigParam[0];

		[XmlIgnore]
		public List<string> Categories = new List<string>();

		[XmlIgnore]
		private int[] RelIndex;
	}
}
