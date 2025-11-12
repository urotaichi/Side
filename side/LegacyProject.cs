using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MasaoPlus
{
    /// <summary>
    /// 旧形式のプロジェクトファイルを読み込むための互換クラス
    /// </summary>
    [XmlType("MasaoProject")]
    [Serializable]
    public class LegacyProject
    {
        public string Name = "";

        public double ProjVer;

        public Runtime Runtime = new();

        public bool Use3rdMapData = false;

        public ChipsData[] CustomPartsDefinition;

        public LayerObject StageData = [];

        public LayerObject StageData2 = [];

        public LayerObject StageData3 = [];

        public LayerObject StageData4 = [];

        [XmlArray("LayerData")]
        public string[] OldLayerData = [];

        [XmlArray("LayerData2")]
        public string[] OldLayerData2 = [];

        [XmlArray("LayerData3")]
        public string[] OldLayerData3 = [];

        [XmlArray("LayerData4")]
        public string[] OldLayerData4 = [];

        public LayerObject MapData = [];

        public ConfigurationOwner Config = new();

        /// <summary>
        /// LegacyProjectからProjectに変換するメソッド
        /// </summary>
        public Project ToCurrentProject()
        {
            var project = new Project
            {
                Name = Name,
                ProjVer = ProjVer,
                Runtime = Runtime,
                Use3rdMapData = Use3rdMapData,
                CustomPartsDefinition = CustomPartsDefinition,
                StageData = StageData,
                StageData2 = StageData2,
                StageData3 = StageData3,
                StageData4 = StageData4,
                MapData = MapData,
                Config = Config
            };

            // 旧形式のLayerDataを新形式に変換
            if (OldLayerData != null && OldLayerData.Length > 0)
            {
                project.LayerData = [[.. OldLayerData]];
            }

            if (OldLayerData2 != null && OldLayerData2.Length > 0)
            {
                project.LayerData2 = [[.. OldLayerData2]];
            }

            if (OldLayerData3 != null && OldLayerData3.Length > 0)
            {
                project.LayerData3 = [[.. OldLayerData3]];
            }

            if (OldLayerData4 != null && OldLayerData4.Length > 0)
            {
                project.LayerData4 = [[.. OldLayerData4]];
            }

            return project;
        }
    }
}
