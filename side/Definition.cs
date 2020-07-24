using System;
using System.Diagnostics;
using System.Reflection;

namespace MasaoPlus
{
	// Token: 0x02000032 RID: 50
	public class Definition
	{
		// Token: 0x0400025E RID: 606
		public string AppName = "Side";

		// Token: 0x0400025F RID: 607
		public string AppNameFull = "Supermasao Integrated Development Environment";

		// Token: 0x04000260 RID: 608
		public double CProjVer = 1.0;

		// Token: 0x04000261 RID: 609
		public string ProjExt = ".spj";

		// Token: 0x04000262 RID: 610
		public string ProjFileType = "Project";

		// Token: 0x04000263 RID: 611
		public string ProjFileDescription = "Side プロジェクトファイル";

		// Token: 0x04000264 RID: 612
		public string RuntimeArchiveExt = ".srp";

		// Token: 0x04000265 RID: 613
		public string RuntimeFileType = "RuntimePackage";

		// Token: 0x04000266 RID: 614
		public string RuntimeFileDescription = "Side ランタイムパッケージ";

		// Token: 0x04000267 RID: 615
		public string Dump = "dmp.txt";

		// Token: 0x04000268 RID: 616
		public string RuntimeDir = "runtime";

		// Token: 0x04000269 RID: 617
		public string ConfigFile = "side.xml";

		// Token: 0x0400026A RID: 618
		public string UpdateTempData = "su.xml";

		// Token: 0x0400026B RID: 619
		public int GridInterval = 2;

		// Token: 0x0400026C RID: 620
		public int RulerStep = 5;

		// Token: 0x0400026D RID: 621
		public bool RulerPutDot = true;

		// Token: 0x0400026E RID: 622
		public int LineNoPaddingRight = 1;

		// Token: 0x0400026F RID: 623
		public int LineNoPaddingTop = 1;

		// Token: 0x04000270 RID: 624
		public int ZipExtractBufferLength = 10240;

		// Token: 0x04000271 RID: 625
		public string Version = FileVersionInfo.GetVersionInfo((new Uri(Assembly.GetExecutingAssembly().CodeBase)).LocalPath).FileVersion;

		// Token: 0x04000272 RID: 626
		public double CheckVersion = 2.10;

		// Token: 0x04000273 RID: 627
		public string EditorIdStr = "/* [MI]Created By:Side - the Supermasao Integrated Development Environment v2.1.0[/MI] */";

		// Token: 0x04000274 RID: 628
		public bool IsAutoUpdateEnabled = true;

		// Token: 0x04000275 RID: 629
		public string BaseUpdateServer = "https://urotaichi.com/other/side/side.xml";
	}
}
