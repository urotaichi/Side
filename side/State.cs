using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace MasaoPlus
{
	// Token: 0x02000048 RID: 72
	public class State
	{
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000270 RID: 624 RVA: 0x0002A6E8 File Offset: 0x000288E8
		public int GetCByte
		{
			get
			{
				if (Global.state.MapEditMode)
				{
					return Global.cpd.runtime.Definitions.MapSize.bytesize;
				}
				if (!Global.cpd.UseLayer || this.EditingForeground)
				{
					return Global.cpd.runtime.Definitions.StageSize.bytesize;
				}
				return Global.cpd.runtime.Definitions.LayerSize.bytesize;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000271 RID: 625 RVA: 0x0002A764 File Offset: 0x00028964
		public int GetCByteWidth
		{
			get
			{
				if (Global.state.MapEditMode)
				{
					return Global.cpd.runtime.Definitions.MapSize.StageByteWidth;
				}
				if (!Global.cpd.UseLayer || this.EditingForeground)
				{
					return Global.cpd.runtime.Definitions.StageSize.StageByteWidth;
				}
				return Global.cpd.runtime.Definitions.LayerSize.StageByteWidth;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000272 RID: 626 RVA: 0x0002A7E0 File Offset: 0x000289E0
		public Runtime.DefinedData.StageSizeData GetCSSize
		{
			get
			{
				if (Global.state.MapEditMode)
				{
					return Global.cpd.runtime.Definitions.MapSize;
				}
				if (!Global.cpd.UseLayer || this.EditingForeground)
				{
					return Global.cpd.runtime.Definitions.StageSize;
				}
				return Global.cpd.runtime.Definitions.LayerSize;
			}
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0002A84C File Offset: 0x00028A4C
		public void AdjustMapPoint()
		{
			if (this.MapPoint.X > this.MapMoveMax.Width)
			{
				this.MapPoint.X = this.MapMoveMax.Width;
			}
			else if (this.MapPoint.X < 0)
			{
				this.MapPoint.X = 0;
			}
			if (this.MapPoint.Y > this.MapMoveMax.Height)
			{
				this.MapPoint.Y = this.MapMoveMax.Height;
			}
			else if (this.MapPoint.Y < 0)
			{
				this.MapPoint.Y = 0;
			}
			if (this.MapMoveMax.Width <= 0)
			{
				this.MapPoint.X = 0;
			}
			if (this.MapMoveMax.Height <= 0)
			{
				this.MapPoint.Y = 0;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000274 RID: 628 RVA: 0x0002A924 File Offset: 0x00028B24
		public Point MapPointTranslated
		{
			get
			{
				double num = 1.0 / Global.config.draw.ZoomIndex;
				return new Point((int)((double)this.MapPoint.X * num), (int)((double)this.MapPoint.Y * num));
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000275 RID: 629 RVA: 0x0002A970 File Offset: 0x00028B70
		// (set) Token: 0x06000276 RID: 630 RVA: 0x0002A9CC File Offset: 0x00028BCC
		public Point MapPointMap
		{
			get
			{
				return new Point(this.MapPoint.X / Global.cpd.runtime.Definitions.ChipSize.Width, this.MapPoint.Y / Global.cpd.runtime.Definitions.ChipSize.Height);
			}
			set
			{
				this.MapPoint.X = value.X * Global.cpd.runtime.Definitions.ChipSize.Width;
				this.MapPoint.Y = value.Y * Global.cpd.runtime.Definitions.ChipSize.Height;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000277 RID: 631 RVA: 0x0002AA34 File Offset: 0x00028C34
		// (set) Token: 0x06000278 RID: 632 RVA: 0x0002AA90 File Offset: 0x00028C90
		public Point MapPointTranslatedMap
		{
			get
			{
				Point mapPointTranslated = this.MapPointTranslated;
				return new Point(mapPointTranslated.X / Global.cpd.runtime.Definitions.ChipSize.Width, mapPointTranslated.Y / Global.cpd.runtime.Definitions.ChipSize.Height);
			}
			set
			{
				this.MapPoint.X = (int)((double)(value.X * Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex);
				this.MapPoint.Y = (int)((double)(value.Y * Global.cpd.runtime.Definitions.ChipSize.Height) * Global.config.draw.ZoomIndex);
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600027A RID: 634 RVA: 0x0000388F File Offset: 0x00001A8F
		// (set) Token: 0x06000279 RID: 633 RVA: 0x00003873 File Offset: 0x00001A73
		public ChipsData CurrentChip
		{
			get
			{
				return this.CurrentChipData;
			}
			set
			{
				this.CurrentChipData = value;
				if (this.UpdateCurrentChipInvoke != null)
				{
					this.UpdateCurrentChipInvoke();
				}
			}
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x0600027B RID: 635 RVA: 0x00003897 File Offset: 0x00001A97
		// (remove) Token: 0x0600027C RID: 636 RVA: 0x000038B0 File Offset: 0x00001AB0
		public event State.UpdateCurrentChip UpdateCurrentChipInvoke;

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600027D RID: 637 RVA: 0x000038C9 File Offset: 0x00001AC9
		// (set) Token: 0x0600027E RID: 638 RVA: 0x000038DF File Offset: 0x00001ADF
		public bool EditingForeground
		{
			get
			{
				return !Global.cpd.UseLayer || this.efg;
			}
			set
			{
				this.efg = value;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000280 RID: 640 RVA: 0x000038FB File Offset: 0x00001AFB
		// (set) Token: 0x0600027F RID: 639 RVA: 0x000038E8 File Offset: 0x00001AE8
		public bool EditFlag
		{
			get
			{
				return this.editFlag;
			}
			set
			{
				this.editFlag = value;
				Global.MainWnd.UpdateTitle();
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000281 RID: 641 RVA: 0x00003903 File Offset: 0x00001B03
		public bool UseBuffered
		{
			get
			{
				return !this.ForceNoBuffering && Global.config.draw.SkipBufferedDraw;
			}
		}

		// Token: 0x04000311 RID: 785
		public Color Background = Color.FromArgb(0, 255, 255);

		// Token: 0x04000312 RID: 786
		public bool DrawUnactiveLayer = true;

		// Token: 0x04000313 RID: 787
		public bool TransparentUnactiveLayer = true;

		// Token: 0x04000314 RID: 788
		public Point MapPoint = default(Point);

		// Token: 0x04000315 RID: 789
		public Size MapMoveMax = default(Size);

		// Token: 0x04000316 RID: 790
		private ChipsData CurrentChipData = default(ChipsData);

		// Token: 0x04000318 RID: 792
		public Process Testrun;

		// Token: 0x04000319 RID: 793
		private bool efg = true;

		// Token: 0x0400031A RID: 794
		private bool editFlag;

		// Token: 0x0400031B RID: 795
		public string[] QuickTestrunSource;

		// Token: 0x0400031C RID: 796
		public Dictionary<string, string> ChipRegister = new Dictionary<string, string>();

		// Token: 0x0400031D RID: 797
		public bool ParseCommandline;

		// Token: 0x0400031E RID: 798
		public string RunFile;

		// Token: 0x0400031F RID: 799
		public bool ForceNoBuffering;

		// Token: 0x04000320 RID: 800
		public int EdittingStage;

		// Token: 0x04000321 RID: 801
		public bool MapEditMode;

		// Token: 0x04000322 RID: 802
		public bool TestrunAll;

		// Token: 0x02000049 RID: 73
		// (Invoke) Token: 0x06000284 RID: 644
		public delegate void UpdateCurrentChip();
	}
}
