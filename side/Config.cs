using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MasaoPlus
{
	// Token: 0x02000042 RID: 66
	public class Config
	{
		// Token: 0x06000267 RID: 615
		public static Config ParseXML(string file)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Config));
			Config result;
			using (FileStream fileStream = new FileStream(file, FileMode.Open))
			{
				Config config = (Config)xmlSerializer.Deserialize(fileStream);
				config.localSystem.FileEncoding = Encoding.GetEncoding(config.localSystem.FileEncStr);
				result = config;
			}
			return result;
		}

		// Token: 0x06000268 RID: 616
		public void SaveXML(string file)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Config));
			using (FileStream fileStream = new FileStream(file, FileMode.Create))
			{
				this.localSystem.FileEncStr = this.localSystem.FileEncoding.WebName;
				xmlSerializer.Serialize(fileStream, this);
			}
		}

		// Token: 0x040002DF RID: 735
		public Config.Draw draw = new Config.Draw();

		// Token: 0x040002E0 RID: 736
		public Config.LocalSystem localSystem = new Config.LocalSystem();

		// Token: 0x040002E1 RID: 737
		public Config.TestRun testRun = new Config.TestRun();

		// Token: 0x040002E2 RID: 738
		public Config.LastData lastData = new Config.LastData();

		// Token: 0x02000043 RID: 67
		public class Draw
		{
			// Token: 0x040002E3 RID: 739
			public double ZoomIndex = 1.0;

			// Token: 0x040002E4 RID: 740
			public bool DrawGrid = true;

			// Token: 0x040002E5 RID: 741
			public bool StageInterpolation;

			// Token: 0x040002E6 RID: 742
			public bool ClassicChipListInterpolation = true;

			// Token: 0x040002E7 RID: 743
			public bool PageScroll;

			// Token: 0x040002E8 RID: 744
			public bool RightClickMenu = true;

			// Token: 0x040002E9 RID: 745
			public bool ExtendDraw = true;

			// Token: 0x040002EA RID: 746
			public bool AlphaBlending = true;

			// Token: 0x040002EB RID: 747
			public bool UseBufferingDraw = true;

			// Token: 0x040002EC RID: 748
			public bool UseBufferingMemoryDraw = true;

			// Token: 0x040002ED RID: 749
			public Config.Draw.SelectionDrawMode SelDrawMode;

			// Token: 0x040002EE RID: 750
			public bool SkipFirstChip = true;

			// Token: 0x040002EF RID: 751
			public bool SkipBufferedDraw;

			// Token: 0x040002F0 RID: 752
			public bool StartUpWithClassicCL = true;

			// Token: 0x040002F1 RID: 753
			public bool HScrollDefault;

			// Token: 0x02000044 RID: 68
			public enum SelectionDrawMode
			{
				// Token: 0x040002F3 RID: 755
				SideOriginal,
				// Token: 0x040002F4 RID: 756
				mtpp,
				// Token: 0x040002F5 RID: 757
				MTool,
				// Token: 0x040002F6 RID: 758
				None
			}
		}

		// Token: 0x02000045 RID: 69
		public class LocalSystem
		{
			// Token: 0x040002F7 RID: 759
			[XmlIgnore]
			public Encoding FileEncoding = Encoding.UTF8;

			// Token: 0x040002F8 RID: 760
			public string FileEncStr = "";

			// Token: 0x040002F9 RID: 761
			public string UsingWebBrowser = "";

			// Token: 0x040002FA RID: 762
			public bool ShiftFocusShift = true;

			// Token: 0x040002FB RID: 763
			public string UpdateServer = "https://urotaichi.com/other/side/side.xml";

			// Token: 0x040002FC RID: 764
			public bool ReverseTabView;

			// Token: 0x040002FD RID: 765
			public bool CheckAutoUpdate = true;

			// Token: 0x040002FE RID: 766
			public bool TextEditorGDIMode = true;

			// Token: 0x040002FF RID: 767
			public bool IntegrateEditorId;

			// Token: 0x04000300 RID: 768
			public bool UsePropExTextEditor = true;

			// Token: 0x04000301 RID: 769
			public bool WrapPropText;
		}

		// Token: 0x02000046 RID: 70
		public class TestRun
		{
			// Token: 0x04000302 RID: 770
			public string TempFile = "~temp";

			// Token: 0x04000303 RID: 771
			public bool UseIntegratedBrowser;

			// Token: 0x04000304 RID: 772
			public bool KillTestrunOnFocus = true;

			// Token: 0x04000305 RID: 773
			public bool QuickTestrun = true;
		}

		// Token: 0x02000047 RID: 71
		public class LastData
		{
			// Token: 0x17000051 RID: 81
			// (get) Token: 0x0600026D RID: 621
			public string ProjDirF
			{
				get
				{
					if (this.ProjDir == "")
					{
						return Path.Combine(Application.StartupPath, "projects");
					}
					return this.ProjDir;
				}
			}

			// Token: 0x17000052 RID: 82
			// (get) Token: 0x0600026E RID: 622
			public string PictDirF
			{
				get
				{
					if (this.PictDir == "")
					{
						return Path.Combine(Application.StartupPath, "pictures\\default");
					}
					return this.PictDir;
				}
			}

			// Token: 0x04000306 RID: 774
			public double SpliterDist = 0.3;

			// Token: 0x04000307 RID: 775
			public FormWindowState WndState;

			// Token: 0x04000308 RID: 776
			public Point WndPoint = new Point(120, 120);

			// Token: 0x04000309 RID: 777
			public Size WndSize = new Size(800, 500);

			// Token: 0x0400030A RID: 778
			public string ProjDir = "";

			// Token: 0x0400030B RID: 779
			public string PictDir = "";

			// Token: 0x0400030C RID: 780
			public string DefaultChip = "pattern.gif";

			// Token: 0x0400030D RID: 781
			public string DefaultLayerChip = "mapchip.gif";

			// Token: 0x0400030E RID: 782
			public string DefaultTitleImage = "title.gif";

			// Token: 0x0400030F RID: 783
			public string DefaultGameoverImage = "gameover.gif";

			// Token: 0x04000310 RID: 784
			public string DefaultEndingImage = "ending.gif";
		}
	}
}
