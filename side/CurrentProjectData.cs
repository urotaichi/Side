using System;

namespace MasaoPlus
{
	// Token: 0x02000031 RID: 49
	public class CurrentProjectData
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x000032E5 File Offset: 0x000014E5
		public bool UseLayer
		{
			get
			{
				return !Global.state.MapEditMode && Global.cpd.runtime != null && Global.cpd.runtime.Definitions.LayerSize.bytesize != 0;
			}
		}

		// Token: 0x04000255 RID: 597
		public Project project;

		// Token: 0x04000256 RID: 598
		public Runtime runtime;

		// Token: 0x04000257 RID: 599
		public string[] EditingMap;

		// Token: 0x04000258 RID: 600
		public string[] EditingLayer;

		// Token: 0x04000259 RID: 601
		public ChipsData[] Mapchip;

		// Token: 0x0400025A RID: 602
		public ChipsData[] Layerchip;

		// Token: 0x0400025B RID: 603
		public ChipsData[] Worldchip;

		// Token: 0x0400025C RID: 604
		public string where = "";

		// Token: 0x0400025D RID: 605
		public string filename = "";
	}
}
