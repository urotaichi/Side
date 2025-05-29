using System;
using System.Collections.Generic;

namespace MasaoPlus
{
    public class CurrentProjectData
    {
        public static bool UseLayer
        {
            get
            {
                return !Global.state.MapEditMode && Global.cpd.runtime != null && Global.cpd.runtime.Definitions.LayerSize.bytesize != 0;
            }
        }

        public Project project;

        public Runtime runtime;

        public LayerObject EditingMap;

        public LayerObject EditingLayer;

        public List<LayerObject> EditingLayers
        {
            get
            {
                if (project != null)
                {
                    return Global.state.EdittingStage switch
                    {
                        0 => project.LayerData,
                        1 => project.LayerData2,
                        2 => project.LayerData3,
                        3 => project.LayerData4,
                        _ => null,
                    };
                }
                return null;
            }
        }

        public int LayerCount
        {
            get
            {
                if (UseLayer && project != null)
                {
                    return Global.state.EdittingStage switch
                    {
                        0 => project.LayerData?.Count ?? 1,
                        1 => project.LayerData2?.Count ?? 1,
                        2 => project.LayerData3?.Count ?? 1,
                        3 => project.LayerData4?.Count ?? 1,
                        _ => 0,
                    };
                }
                return 0;
            }
        }

        public int MainOrder
        {
            get
            {
                if (UseLayer && project != null)
                {
                    return Global.state.EdittingStage switch
                    {
                        0 => project.Runtime.Definitions.LayerSize.mainOrder,
                        1 => project.Runtime.Definitions.LayerSize2.mainOrder,
                        2 => project.Runtime.Definitions.LayerSize3.mainOrder,
                        3 => project.Runtime.Definitions.LayerSize4.mainOrder,
                        _ => 0,
                    };
                }
                return 0;
            }
        }

        public ChipsData[] Mapchip;

        public ChipsData[] Layerchip;

        public ChipsData[] Worldchip;

        public ChipsData[] VarietyChip;

        public ChipsData[] CustomPartsChip;

        public string where = "";

        public string filename = "";
    }
}
