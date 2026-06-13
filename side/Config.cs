using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MasaoPlus
{
    public class Config
    {
        public static Config ParseXML(string file)
        {
            XmlSerializer xmlSerializer = new(typeof(Config));
            Config result;
            using (FileStream fileStream = new(file, FileMode.Open))
            {
                Config config = (Config)xmlSerializer.Deserialize(fileStream);
                config.localSystem.FileEncoding = Encoding.GetEncoding(config.localSystem.FileEncStr);
                result = config;
            }
            return result;
        }

        public void SaveXML(string file)
        {
            XmlSerializer xmlSerializer = new(typeof(Config));
            using FileStream fileStream = new(file, FileMode.Create);
            localSystem.FileEncStr = localSystem.FileEncoding.WebName;
            xmlSerializer.Serialize(fileStream, this);
        }

        public Draw draw = new();

        public LocalSystem localSystem = new();

        public TestRun testRun = new();

        public LastData lastData = new();

        public class Draw
        {
            public double ZoomIndex = 1.0;

            public bool DrawGrid = true;

            public bool StageInterpolation;

            public bool ClassicChipListInterpolation = true;

            public bool PageScroll;

            public bool RightClickMenu = true;

            public bool ExtendDraw = true;

            public bool AlphaBlending = true;

            public bool UseBufferingDraw = true;

            public bool UseBufferingMemoryDraw = true;

            public SelectionDrawMode SelDrawMode;

            public bool SkipFirstChip = true;

            public bool SkipBufferedDraw;

            public bool StartUpWithClassicCL = true;

            public bool HScrollDefault;

            public enum SelectionDrawMode
            {
                SideOriginal,
                mtpp,
                MTool,
                None
            }
        }

        public class LocalSystem
        {
            [XmlIgnore]
            public Encoding FileEncoding = Encoding.UTF8;

            public string FileEncStr = "";

            public string UsingWebBrowser = "";

            public bool ShiftFocusShift = true;

            public string UpdateServer = "https://urotaichi.com/other/side/side.xml";

            public bool ReverseTabView;

#if MICROSOFT_STORE
            // Microsoft Store版では自動更新は常に無効（設定は可能だが無視される）
            private bool _checkAutoUpdate = false;
            public bool CheckAutoUpdate 
            { 
                get => false; // 常にfalseを返す
                set { /* 設定は無視 */ } 
            }
#else
            public bool CheckAutoUpdate { get; set; } = true;
#endif

            public bool TextEditorGDIMode = true;

            public bool IntegrateEditorId;

            public bool UsePropExTextEditor = true;

            //初期値を出力するかどうか　デフォルトでは出力しない
            public bool OutPutInititalSourceCode;

            //枠に収まりきらないプロパティを改行して表示するかどうか　デフォルトでは改行して表示する
            public bool WrapPropText = true;

            //ダークモード設定 (Classic:ライトモード, System:システム設定に従う, Dark:ダークモード)
            public SystemColorMode DarkMode = SystemColorMode.System;
        }

        public class TestRun
        {
            public string TempFile = "~temp";

            public bool UseIntegratedBrowser = true;

            public bool KillTestrunOnFocus = true;

            public bool QuickTestrun = true;
        }

        public class LastData
        {
            public string ProjDirF
            {
                get
                {
                    if (ProjDir == "")
                    {
#if MICROSOFT_STORE
                        // Microsoft Store版では Documents/Side Projects を使用
                        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Side Projects");
#else
                        // 通常版では従来通りアプリケーションフォルダ内
                        return Path.Combine(Application.StartupPath, "projects");
#endif
                    }
                    return ProjDir;
                }
            }

            public string PictDirF
            {
                get
                {
                    if (PictDir == "")
                    {
#if MICROSOFT_STORE
                        // Microsoft Store版では Documents/Side Projects/Pictures を使用
                        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Side Projects", "Pictures");
#else
                        // 通常版では従来通りアプリケーションフォルダ内
                        return Path.Combine(Application.StartupPath, "pictures\\default");
#endif
                    }
                    return PictDir;
                }
            }

            public double SpliterDist = 0.3;

            public FormWindowState WndState;

            public Point WndPoint = new(120, 120);

            public Size WndSize = new(800, 500);

            public string ProjDir = "";

            public string PictDir = "";

            public string DefaultChip = "pattern.gif";

            public string DefaultLayerChip = "mapchip.gif";

            public string DefaultTitleImage = "title.gif";

            public string DefaultGameoverImage = "gameover.gif";

            public string DefaultEndingImage = "ending.gif";
        }
    }
}
