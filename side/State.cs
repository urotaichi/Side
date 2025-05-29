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
                if (!CurrentProjectData.UseLayer || EditingForeground)
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
                if (!CurrentProjectData.UseLayer || EditingForeground)
                {
                    return GUIDesigner.CurrentStageSize.StageByteWidth;
                }
                return GUIDesigner.CurrentLayerSize.StageByteWidth;
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
                if (!CurrentProjectData.UseLayer || EditingForeground)
                {
                    return GUIDesigner.CurrentStageSize;
                }
                return GUIDesigner.CurrentLayerSize;
            }
        }

        /// <summary>
        /// 現在3rdMapDataを使用しているかどうか。Use3rdMapDataがtrueかつMapEditModeがfalseの場合にtrueを返す。
        /// </summary>
        public bool Use3rdMapDataCurrently
        {
            get
            {
                return Global.cpd.project != null && Global.cpd.project.Use3rdMapData && !MapEditMode;
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
                return new Point((int)(MapPoint.X * num), (int)(MapPoint.Y * num));
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
                MapPoint.X = (int)(value.X * Global.cpd.runtime.Definitions.ChipSize.Width * Global.config.draw.ZoomIndex);
                MapPoint.Y = (int)(value.Y * Global.cpd.runtime.Definitions.ChipSize.Height * Global.config.draw.ZoomIndex);
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
                // 現在選択しているチップが変わったら発火
                UpdateCurrentChipInvoke?.Invoke();
            }
        }

        public event UpdateCurrentChip UpdateCurrentChipInvoke;

        public ChipsData CurrentCustomPartsChip
        {
            get
            {
                return CurrentCustomPartsChipData;
            }
            set
            {
                CurrentCustomPartsChipData = value;
                // 現在選択しているカスタムパーツが変わったら発火
                UpdateCurrentCustomPartsChipInvoke?.Invoke();
            }
        }

        public event UpdateCurrentCustomPartsChip UpdateCurrentCustomPartsChipInvoke;

        public bool EditingForeground
        {
            get
            {
                return !CurrentProjectData.UseLayer || efg;
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

        private ChipsData CurrentCustomPartsChipData = default;

        public Process Testrun;

        private bool efg = true;

        private bool editFlag;

        public LayerObject QuickTestrunSource;

        public Dictionary<string, string> ChipRegister = [];

        public bool ParseCommandline;

        public string RunFile;

        public bool ForceNoBuffering;

        public int EdittingStage;

        public int EdittingLayerIndex = -1;

        public bool MapEditMode;

        public bool TestrunAll;

        public delegate void UpdateCurrentChip();

        public delegate void UpdateCurrentCustomPartsChip();

        public bool StageSizeChanged = false;

        public Size DefaultStageSize = new(180, 30);

        public Size MinimumStageSize = new(16, 10);

        public Size MaximumStageSize = new(500, 500);
    }
}
