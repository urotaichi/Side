using System;

namespace MasaoPlus
{
	// Token: 0x02000030 RID: 48
	internal static class Global
	{
		// Token: 0x0400024F RID: 591
		public const int ScrollbarWidth = 20;

		// Token: 0x04000250 RID: 592
		public static MainWindow MainWnd = null;

		// Token: 0x04000251 RID: 593
		public static Definition definition = new Definition();

		// Token: 0x04000252 RID: 594
		public static Config config = new Config();

		// Token: 0x04000253 RID: 595
		public static State state = new State();

		// Token: 0x04000254 RID: 596
		public static CurrentProjectData cpd = new CurrentProjectData();
	}
}
