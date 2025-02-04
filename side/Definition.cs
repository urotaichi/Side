using System;
using System.Diagnostics;
using System.Reflection;

namespace MasaoPlus
{
    public class Definition
    {
        public Definition()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            Version = $"{version.Major}.{version.Minor}.{version.Build}";

            EditorIdStr = $"/* [MI]Created By:Side - the Supermasao Integrated Development Environment v{Version}[/MI] */";
        }

        public string AppName = "Side";

        public string AppNameFull = "Supermasao Integrated Development Environment";

        public double CProjVer = 1.0;

        public string ProjExt = ".spj";

        public string ProjFileType = "Project";

        public string ProjFileDescription = "Side プロジェクトファイル";

        public string RuntimeArchiveExt = ".srp";

        public string RuntimeFileType = "RuntimePackage";

        public string RuntimeFileDescription = "Side ランタイムパッケージ";

        public string Dump = "dmp.txt";

        public string RuntimeDir = "runtime";

        public string ConfigFile = "side.xml";

        public string UpdateTempData = "su.xml";

        public int GridInterval = 2;

        public int RulerStep = 5;

        public bool RulerPutDot = true;

        public int LineNoPaddingRight = 1;

        public int LineNoPaddingTop = 1;

        public int ZipExtractBufferLength = 10240;

        public string Version;

        public double CheckVersion = 4.21;

        public string EditorIdStr;

        public bool IsAutoUpdateEnabled = true;

        public string BaseUpdateServer = "https://urotaichi.com/other/side/side.xml";
    }
}
