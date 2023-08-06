using System;

namespace MasaoPlus
{
    public class CurrentProjectData
    {
        public bool UseLayer
        {
            get
            {
                return !Global.state.MapEditMode && Global.cpd.runtime != null && Global.cpd.runtime.Definitions.LayerSize.bytesize != 0;
            }
        }

        public Project project;

        public Runtime runtime;

        public string[] EditingMap;

        public string[] EditingLayer;

        public ChipsData[] Mapchip;

        public ChipsData[] Layerchip;

        public ChipsData[] Worldchip;

        public ChipsData[] VarietyChip;

        public ChipsData[] CustomPartsChip;

        public string where = "";

        public string filename = "";
    }
}
