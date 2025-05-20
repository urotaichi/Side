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
                if (UseLayer && project != null && project.LayerData != null)
                {
                    return project.LayerData.Count;
                }
                return 1;
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
