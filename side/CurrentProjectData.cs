using System;

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

        public int LayerCount
        {
            get
            {
                if (UseLayer && project != null)
                {
                    return Global.state.EdittingStage switch
                    {
                        0 => project.LayerData != null ? project.LayerData.Count : 1,
                        1 => project.LayerData2 != null ? project.LayerData2.Count : 1,
                        2 => project.LayerData3 != null ? project.LayerData3.Count : 1,
                        3 => project.LayerData4 != null ? project.LayerData4.Count : 1,
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
