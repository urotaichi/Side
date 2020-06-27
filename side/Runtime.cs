using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MasaoPlus
{
	// Token: 0x0200001C RID: 28
	[XmlType("MasaoRuntime")]
	[Serializable]
	public class Runtime
	{
		// Token: 0x060000F2 RID: 242 RVA: 0x00017214 File Offset: 0x00015414
		public static Runtime ParseXML(string file)
		{
			Runtime result;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Runtime));
				using (FileStream fileStream = new FileStream(file, FileMode.Open))
				{
					Runtime runtime = (Runtime)xmlSerializer.Deserialize(fileStream);
					result = runtime;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("ランタイム定義を開けませんでした。" + Environment.NewLine + ex.Message, "オープン失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				result = null;
			}
			return result;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00002952 File Offset: 0x00000B52
		public static string[] CheckFiles(string cdir, Runtime chkr)
		{
			return Runtime.CheckFiles(cdir, chkr, true);
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0001729C File Offset: 0x0001549C
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

		// Token: 0x040000FF RID: 255
		public Runtime.DefinedData Definitions = new Runtime.DefinedData();

		// Token: 0x04000100 RID: 256
		public Runtime.ConfigurationData DefaultConfigurations = new Runtime.ConfigurationData();

		// Token: 0x0200001D RID: 29
		[Serializable]
		public class DefinedData
		{
			// Token: 0x04000101 RID: 257
			public string Name = "";

			// Token: 0x04000102 RID: 258
			public string Author = "";

			// Token: 0x04000103 RID: 259
			public string Update = "";

			// Token: 0x04000104 RID: 260
			public string Package = "";

			// Token: 0x04000105 RID: 261
			public string Configurations = "";

			// Token: 0x04000106 RID: 262
			public string ChipDefinition = "";

			// Token: 0x04000107 RID: 263
			public string ChipExtender = "";

			// Token: 0x04000108 RID: 264
			public string Chipset = "";

			// Token: 0x04000109 RID: 265
			public double DefVersion;

			// Token: 0x0400010A RID: 266
			public double RequireLower;

			// Token: 0x0400010B RID: 267
			public Size ChipSize = Size.Empty;

			// Token: 0x0400010C RID: 268
			public int StageSplit;

			// Token: 0x0400010D RID: 269
			public string ParamName = "";

			// Token: 0x0400010E RID: 270
			public string ParamName2 = "";

			// Token: 0x0400010F RID: 271
			public string ParamName3 = "";

			// Token: 0x04000110 RID: 272
			public string ParamName4 = "";

			// Token: 0x04000111 RID: 273
			public int LayerSplit;

			// Token: 0x04000112 RID: 274
			public string LayerName = "";

			// Token: 0x04000113 RID: 275
			public string LayerName2 = "";

			// Token: 0x04000114 RID: 276
			public string LayerName3 = "";

			// Token: 0x04000115 RID: 277
			public string LayerName4 = "";

			// Token: 0x04000116 RID: 278
			public string MapName = "";

			// Token: 0x04000117 RID: 279
			public Runtime.DefinedData.StageSizeData StageSize = new Runtime.DefinedData.StageSizeData();

			// Token: 0x04000118 RID: 280
			public Runtime.DefinedData.StageSizeData LayerSize = new Runtime.DefinedData.StageSizeData();

			// Token: 0x04000119 RID: 281
			public Runtime.DefinedData.StageSizeData MapSize = new Runtime.DefinedData.StageSizeData();

			public int MaxAthleticNumber;

			// Token: 0x0200001E RID: 30
			[Serializable]
			public class StageSizeData
			{
				// Token: 0x17000017 RID: 23
				// (get) Token: 0x060000F7 RID: 247 RVA: 0x0000297A File Offset: 0x00000B7A
				[XmlIgnore]
				public int StageByteWidth
				{
					get
					{
						return this.x * this.bytesize;
					}
				}

				// Token: 0x0400011A RID: 282
				[XmlAttribute]
				public int x;

				// Token: 0x0400011B RID: 283
				[XmlAttribute]
				public int y;

				// Token: 0x0400011C RID: 284
				[XmlAttribute]
				public int bytesize;
			}
		}

		// Token: 0x0200001F RID: 31
		[Serializable]
		public class ConfigurationData
		{
			// Token: 0x0400011D RID: 285
			public HTMLReplaceData[] OutputReplace = new HTMLReplaceData[0];

			// Token: 0x0400011E RID: 286
			public string HeaderHTML = "";

			public string MiddleHTML = "";

			// Token: 0x0400011F RID: 287
			public string FooterHTML = "";

			// Token: 0x04000120 RID: 288
			public string Parameter = "";

			// Token: 0x04000121 RID: 289
			public string StageParam = "";

			// Token: 0x04000122 RID: 290
			public string StageParam2 = "";

			// Token: 0x04000123 RID: 291
			public string StageParam3 = "";

			// Token: 0x04000124 RID: 292
			public string StageParam4 = "";

			// Token: 0x04000125 RID: 293
			public string LayerParam = "";

			// Token: 0x04000126 RID: 294
			public string LayerParam2 = "";

			// Token: 0x04000127 RID: 295
			public string LayerParam3 = "";

			// Token: 0x04000128 RID: 296
			public string LayerParam4 = "";

			// Token: 0x04000129 RID: 297
			public string MapParam = "";

			// Token: 0x0400012A RID: 298
			public string FileExt = "";

			// Token: 0x0400012B RID: 299
			public string OutputDir = "";

			// Token: 0x0400012C RID: 300
			public string RunFile = "";
		}
	}
}
