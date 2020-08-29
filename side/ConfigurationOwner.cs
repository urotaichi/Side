using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MasaoPlus
{
	// Token: 0x02000025 RID: 37
	[XmlType("Configurations")]
	[Serializable]
	public class ConfigurationOwner
	{
		// Token: 0x06000103 RID: 259 RVA: 0x000178E8 File Offset: 0x00015AE8
		public static ConfigurationOwner LoadXML(string file)
		{
			ConfigurationOwner result;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConfigurationOwner));
				using (FileStream fileStream = new FileStream(file, FileMode.Open))
				{
					ConfigurationOwner configurationOwner = (ConfigurationOwner)xmlSerializer.Deserialize(fileStream);
					configurationOwner.ConfigReady();
					result = configurationOwner;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("設定定義ファイルを開けませんでした。" + Environment.NewLine + ex.Message, "オープン失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				result = null;
			}
			return result;
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00017978 File Offset: 0x00015B78
		// パラメータ設定読み込み
		public void ConfigReady()
		{
			this.Categories = new List<string>();
			this.RelIndex = new int[28];
			bool[] array = new bool[12];
			array[0] = false;
			array[1] = false;
			array[2] = false;
			array[3] = false;
			array[4] = false;
			array[5] = false;
			array[6] = false;
			array[7] = false;
			for (int i = 0; i < this.Configurations.Length; i++)
			{
				ConfigParam configParam = this.Configurations[i];
				if (configParam.Category == null)
				{
					this.Configurations[i].Category = "未設定";
				}
				if (!this.Categories.Contains(configParam.Category))
				{
					this.Categories.Add(configParam.Category);
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
					this.RelIndex[0] = i;
					array[0] = true;
					break;
				case "BACKGROUND2":
					this.RelIndex[1] = i;
					array[1] = true;
					break;
				case "BACKGROUND3":
					this.RelIndex[2] = i;
					array[2] = true;
					break;
				case "BACKGROUND4":
					this.RelIndex[3] = i;
					array[3] = true;
					break;
				case "BACKGROUNDM":
					this.RelIndex[4] = i;
					array[4] = true;
					break;
				case "TITLE":
					this.RelIndex[5] = i;
					array[5] = true;
					break;
				case "ENDING":
					this.RelIndex[6] = i;
					array[6] = true;
					break;
				case "GAMEOVER":
					this.RelIndex[7] = i;
					array[7] = true;
					break;
				case "PATTERN":
					this.RelIndex[8] = i;
					array[8] = true;
					break;
				case "LAYERCHIP":
					this.RelIndex[9] = i;
					break;
				case "STAGENUM":
					this.RelIndex[10] = i;
					array[9] = true;
					break;
				case "STAGESTART":
					this.RelIndex[11] = i;
					array[10] = true;
					break;
				case "STAGESELECT":
					this.RelIndex[12] = i;
					array[11] = true;
					break;
				}
				switch (configParam.Name)
				{
				case "grenade_@1":
					this.RelIndex[13] = i;
					break;
				case "grenade_@2":
					this.RelIndex[14] = i;
					break;
				case "mizunohadou_@":
					this.RelIndex[15] = i;
					break;
				case "firebar_@1":
					this.RelIndex[16] = i;
					break;
				case "firebar_@2":
					this.RelIndex[17] = i;
					break;
				case "filename_oriboss_left1":
					this.RelIndex[18] = i;
					break;
				case "filename_haikei":
					this.RelIndex[19] = i;
					break;
				case "filename_haikei2":
					this.RelIndex[20] = i;
					break;
				case "filename_haikei3":
					this.RelIndex[21] = i;
					break;
				case "filename_haikei4":
					this.RelIndex[22] = i;
					break;
				case "filename_second_haikei":
					this.RelIndex[23] = i;
					break;
				case "filename_second_haikei2":
					this.RelIndex[24] = i;
					break;
				case "filename_second_haikei3":
					this.RelIndex[25] = i;
					break;
				case "filename_second_haikei4":
					this.RelIndex[26] = i;
					break;
				case "filename_chizu":
					this.RelIndex[27] = i;
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

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000105 RID: 261 RVA: 0x00017CC0 File Offset: 0x00015EC0
		// (set) Token: 0x06000106 RID: 262 RVA: 0x00017CF4 File Offset: 0x00015EF4
		[XmlIgnore]
		public Color Background
		{
			get
			{
				Colors colors = new Colors(this.Configurations[this.RelIndex[0]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default(Colors);
				colors.c = value;
				this.Configurations[this.RelIndex[0]].Value = colors.ToString();
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000107 RID: 263 RVA: 0x00017D38 File Offset: 0x00015F38
		// (set) Token: 0x06000108 RID: 264 RVA: 0x00017D6C File Offset: 0x00015F6C
		[XmlIgnore]
		public Color Background2
		{
			get
			{
				Colors colors = new Colors(this.Configurations[this.RelIndex[1]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default(Colors);
				colors.c = value;
				this.Configurations[this.RelIndex[1]].Value = colors.ToString();
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000109 RID: 265 RVA: 0x00017DB0 File Offset: 0x00015FB0
		// (set) Token: 0x0600010A RID: 266 RVA: 0x00017DE4 File Offset: 0x00015FE4
		[XmlIgnore]
		public Color Background3
		{
			get
			{
				Colors colors = new Colors(this.Configurations[this.RelIndex[2]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default(Colors);
				colors.c = value;
				this.Configurations[this.RelIndex[2]].Value = colors.ToString();
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600010B RID: 267 RVA: 0x00017E28 File Offset: 0x00016028
		// (set) Token: 0x0600010C RID: 268 RVA: 0x00017E5C File Offset: 0x0001605C
		[XmlIgnore]
		public Color Background4
		{
			get
			{
				Colors colors = new Colors(this.Configurations[this.RelIndex[3]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default(Colors);
				colors.c = value;
				this.Configurations[this.RelIndex[3]].Value = colors.ToString();
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600010D RID: 269 RVA: 0x00017EA0 File Offset: 0x000160A0
		// (set) Token: 0x0600010E RID: 270 RVA: 0x00017ED4 File Offset: 0x000160D4
		[XmlIgnore]
		public Color BackgroundM
		{
			get
			{
				Colors colors = new Colors(this.Configurations[this.RelIndex[4]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default(Colors);
				colors.c = value;
				this.Configurations[this.RelIndex[4]].Value = colors.ToString();
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600010F RID: 271 RVA: 0x000029C5 File Offset: 0x00000BC5
		// (set) Token: 0x06000110 RID: 272 RVA: 0x000029DF File Offset: 0x00000BDF
		[XmlIgnore]
		public string TitleImage
		{
			get
			{
				return this.Configurations[this.RelIndex[5]].Value;
			}
			set
			{
				this.Configurations[this.RelIndex[5]].Value = value;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000111 RID: 273 RVA: 0x000029FA File Offset: 0x00000BFA
		// (set) Token: 0x06000112 RID: 274 RVA: 0x00002A14 File Offset: 0x00000C14
		[XmlIgnore]
		public string EndingImage
		{
			get
			{
				return this.Configurations[this.RelIndex[6]].Value;
			}
			set
			{
				this.Configurations[this.RelIndex[6]].Value = value;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000113 RID: 275 RVA: 0x00002A2F File Offset: 0x00000C2F
		// (set) Token: 0x06000114 RID: 276 RVA: 0x00002A49 File Offset: 0x00000C49
		[XmlIgnore]
		public string GameoverImage
		{
			get
			{
				return this.Configurations[this.RelIndex[7]].Value;
			}
			set
			{
				this.Configurations[this.RelIndex[7]].Value = value;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000115 RID: 277 RVA: 0x00002A64 File Offset: 0x00000C64
		// (set) Token: 0x06000116 RID: 278 RVA: 0x00002A7E File Offset: 0x00000C7E
		[XmlIgnore]
		public string PatternImage
		{
			get
			{
				return this.Configurations[this.RelIndex[8]].Value;
			}
			set
			{
				this.Configurations[this.RelIndex[8]].Value = value;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000117 RID: 279 RVA: 0x00002A99 File Offset: 0x00000C99
		// (set) Token: 0x06000118 RID: 280 RVA: 0x00002AB4 File Offset: 0x00000CB4
		[XmlIgnore]
		public string LayerImage
		{
			get
			{
				return this.Configurations[this.RelIndex[9]].Value;
			}
			set
			{
				this.Configurations[this.RelIndex[9]].Value = value;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00002AD0 File Offset: 0x00000CD0
		// (set) Token: 0x0600011A RID: 282 RVA: 0x00002AF0 File Offset: 0x00000CF0
		[XmlIgnore]
		public int StageNum
		{
			get
			{
				return int.Parse(this.Configurations[this.RelIndex[10]].Value);
			}
			set
			{
				this.Configurations[this.RelIndex[10]].Value = value.ToString();
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600011B RID: 283 RVA: 0x00002B12 File Offset: 0x00000D12
		// (set) Token: 0x0600011C RID: 284 RVA: 0x00002B32 File Offset: 0x00000D32
		[XmlIgnore]
		public int StageStart
		{
			get
			{
				return int.Parse(this.Configurations[this.RelIndex[11]].Value);
			}
			set
			{
				this.Configurations[this.RelIndex[11]].Value = value.ToString();
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600011D RID: 285 RVA: 0x00002B54 File Offset: 0x00000D54
		// (set) Token: 0x0600011E RID: 286 RVA: 0x00017F18 File Offset: 0x00016118
		[XmlIgnore]
		public bool UseWorldmap
		{
			get
			{
				return this.Configurations[this.RelIndex[12]].Value == "2";
			}
			set
			{
				if (value)
				{
					this.Configurations[this.RelIndex[12]].Value = "2";
					return;
				}
				this.Configurations[this.RelIndex[12]].Value = "1";
			}
		}

		[XmlIgnore]
		public Color Grenade1
		{
			get
			{
				Colors colors = new Colors(this.Configurations[this.RelIndex[13]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default(Colors);
				colors.c = value;
				this.Configurations[this.RelIndex[13]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public Color Grenade2
		{
			get
			{
				Colors colors = new Colors(this.Configurations[this.RelIndex[14]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default(Colors);
				colors.c = value;
				this.Configurations[this.RelIndex[14]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public Color Mizunohadou
		{
			get
			{
				Colors colors = new Colors(this.Configurations[this.RelIndex[15]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default(Colors);
				colors.c = value;
				this.Configurations[this.RelIndex[15]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public Color Firebar1
		{
			get
			{
				Colors colors = new Colors(this.Configurations[this.RelIndex[16]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default(Colors);
				colors.c = value;
				this.Configurations[this.RelIndex[16]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public Color Firebar2
		{
			get
			{
				Colors colors = new Colors(this.Configurations[this.RelIndex[17]].Value);
				return colors.c;
			}
			set
			{
				Colors colors = default(Colors);
				colors.c = value;
				this.Configurations[this.RelIndex[17]].Value = colors.ToString();
			}
		}

		[XmlIgnore]
		public string OribossImage
		{
			get
			{
				if (this.RelIndex[18] != default) return this.Configurations[this.RelIndex[18]].Value;
				else return null;
			}
			set
			{
				this.Configurations[this.RelIndex[18]].Value = value;
			}
		}

		[XmlIgnore]
		public string HaikeiImage
		{
			get
			{
				if (this.RelIndex[19] != default) return this.Configurations[this.RelIndex[19]].Value;
				else return null;
			}
			set
			{
				this.Configurations[this.RelIndex[19]].Value = value;
			}
		}

		[XmlIgnore]
		public string HaikeiImage2
		{
			get
			{
				if (this.RelIndex[20] != default) return this.Configurations[this.RelIndex[20]].Value;
				else return null;
			}
			set
			{
				this.Configurations[this.RelIndex[20]].Value = value;
			}
		}

		[XmlIgnore]
		public string HaikeiImage3
		{
			get
			{
				if (this.RelIndex[21] != default) return this.Configurations[this.RelIndex[21]].Value;
				else return null;
			}
			set
			{
				this.Configurations[this.RelIndex[21]].Value = value;
			}
		}

		[XmlIgnore]
		public string HaikeiImage4
		{
			get
			{
				if (this.RelIndex[22] != default) return this.Configurations[this.RelIndex[22]].Value;
				else return null;
			}
			set
			{
				this.Configurations[this.RelIndex[22]].Value = value;
			}
		}

		[XmlIgnore]
		public string SecondHaikeiImage
		{
			get
			{
				if (this.RelIndex[23] != default) return this.Configurations[this.RelIndex[23]].Value;
				else return null;
			}
			set
			{
				this.Configurations[this.RelIndex[23]].Value = value;
			}
		}

		[XmlIgnore]
		public string SecondHaikeiImage2
		{
			get
			{
				if (this.RelIndex[24] != default) return this.Configurations[this.RelIndex[24]].Value;
				else return null;
			}
			set
			{
				this.Configurations[this.RelIndex[24]].Value = value;
			}
		}

		[XmlIgnore]
		public string SecondHaikeiImage3
		{
			get
			{
				if (this.RelIndex[25] != default) return this.Configurations[this.RelIndex[25]].Value;
				else return null;
			}
			set
			{
				this.Configurations[this.RelIndex[25]].Value = value;
			}
		}

		[XmlIgnore]
		public string SecondHaikeiImage4
		{
			get
			{
				if (this.RelIndex[26] != default) return this.Configurations[this.RelIndex[26]].Value;
				else return null;
			}
			set
			{
				this.Configurations[this.RelIndex[26]].Value = value;
			}
		}

		[XmlIgnore]
		public string ChizuImage
		{
			get
			{
				return this.Configurations[this.RelIndex[27]].Value;
			}
			set
			{
				this.Configurations[this.RelIndex[27]].Value = value;
			}
		}

		// Token: 0x0400014B RID: 331
		[XmlElement("param")]
		public ConfigParam[] Configurations = new ConfigParam[0];

		// Token: 0x0400014C RID: 332
		[XmlIgnore]
		public List<string> Categories = new List<string>();

		// Token: 0x0400014D RID: 333
		[XmlIgnore]
		private int[] RelIndex;
	}
}
