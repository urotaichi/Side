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

            EditorIdStr = $"Created By:{AppName} - the {AppNameFull} v{Version}";
        }

        public string AppName = "Side";

        public string AppNameFull = "Supermasao Integrated Development Environment";

        public double CProjVer = 2.0;

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

        public double CheckVersion = 4.80;

        public string EditorIdStr;

#if MICROSOFT_STORE
        public bool IsAutoUpdateEnabled = false;  // Microsoft Store版では自動更新を無効
#else
        public bool IsAutoUpdateEnabled = true;   // 通常版では自動更新を有効
#endif

        public string BaseUpdateServer = "https://urotaichi.com/other/side/side.xml";
    }
}
