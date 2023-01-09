using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace MasaoPlus
{
	public class State
	{
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

		public Point MapPointTranslated
		{
			get
			{
				double num = 1.0 / Global.config.draw.ZoomIndex;
				return new Point((int)((double)this.MapPoint.X * num), (int)((double)this.MapPoint.Y * num));
			}
		}

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

		public event State.UpdateCurrentChip UpdateCurrentChipInvoke;

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

		public bool UseBuffered
		{
			get
			{
				return !this.ForceNoBuffering && Global.config.draw.SkipBufferedDraw;
			}
		}

		public Color Background = Color.FromArgb(0, 255, 255);

		public bool DrawUnactiveLayer = true;

		public bool TransparentUnactiveLayer = true;

		public Point MapPoint = default(Point);

		public Size MapMoveMax = default(Size);

		private ChipsData CurrentChipData = default(ChipsData);

		public Process Testrun;

		private bool efg = true;

		private bool editFlag;

		public string[] QuickTestrunSource;

		public Dictionary<string, string> ChipRegister = new Dictionary<string, string>();

		public bool ParseCommandline;

		public string RunFile;

		public bool ForceNoBuffering;

		public int EdittingStage;

		public bool MapEditMode;

		public bool TestrunAll;

		public delegate void UpdateCurrentChip();
	}
}
