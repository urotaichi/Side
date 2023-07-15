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
				if (!Global.cpd.UseLayer || EditingForeground)
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
				if (!Global.cpd.UseLayer || EditingForeground)
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
				if (!Global.cpd.UseLayer || EditingForeground)
				{
					return Global.cpd.runtime.Definitions.StageSize;
				}
				return Global.cpd.runtime.Definitions.LayerSize;
			}
		}

		public void AdjustMapPoint()
		{
			if (MapPoint.X > MapMoveMax.Width)
			{
				MapPoint.X = MapMoveMax.Width;
			}
			else if (MapPoint.X < 0)
			{
				MapPoint.X = 0;
			}
			if (MapPoint.Y > MapMoveMax.Height)
			{
				MapPoint.Y = MapMoveMax.Height;
			}
			else if (MapPoint.Y < 0)
			{
				MapPoint.Y = 0;
			}
			if (MapMoveMax.Width <= 0)
			{
				MapPoint.X = 0;
			}
			if (MapMoveMax.Height <= 0)
			{
				MapPoint.Y = 0;
			}
		}

		public Point MapPointTranslated
		{
			get
			{
				double num = 1.0 / Global.config.draw.ZoomIndex;
				return new Point((int)((double)MapPoint.X * num), (int)((double)MapPoint.Y * num));
			}
		}

		public Point MapPointMap
		{
			get
			{
				return new Point(MapPoint.X / Global.cpd.runtime.Definitions.ChipSize.Width, MapPoint.Y / Global.cpd.runtime.Definitions.ChipSize.Height);
			}
			set
			{
				MapPoint.X = value.X * Global.cpd.runtime.Definitions.ChipSize.Width;
				MapPoint.Y = value.Y * Global.cpd.runtime.Definitions.ChipSize.Height;
			}
		}

		public Point MapPointTranslatedMap
		{
			get
			{
				Point mapPointTranslated = MapPointTranslated;
				return new Point(mapPointTranslated.X / Global.cpd.runtime.Definitions.ChipSize.Width, mapPointTranslated.Y / Global.cpd.runtime.Definitions.ChipSize.Height);
			}
			set
			{
				MapPoint.X = (int)((double)(value.X * Global.cpd.runtime.Definitions.ChipSize.Width) * Global.config.draw.ZoomIndex);
				MapPoint.Y = (int)((double)(value.Y * Global.cpd.runtime.Definitions.ChipSize.Height) * Global.config.draw.ZoomIndex);
			}
		}

		public ChipsData CurrentChip
		{
			get
			{
				return CurrentChipData;
			}
			set
			{
				CurrentChipData = value;
				if (UpdateCurrentChipInvoke != null)
				{
					UpdateCurrentChipInvoke();
				}
			}
		}

		public event UpdateCurrentChip UpdateCurrentChipInvoke;

		public bool EditingForeground
		{
			get
			{
				return !Global.cpd.UseLayer || efg;
			}
			set
			{
				efg = value;
			}
		}

		public bool EditFlag
		{
			get
			{
				return editFlag;
			}
			set
			{
				editFlag = value;
				Global.MainWnd.UpdateTitle();
			}
		}

		public bool UseBuffered
		{
			get
			{
				return !ForceNoBuffering && Global.config.draw.SkipBufferedDraw;
			}
		}

		public Color Background = Color.FromArgb(0, 255, 255);

		public bool DrawUnactiveLayer = true;

		public bool TransparentUnactiveLayer = true;

		public Point MapPoint = default;

		public Size MapMoveMax = default;

		private ChipsData CurrentChipData = default;

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
