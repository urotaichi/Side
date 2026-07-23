using System;
using System.Diagnostics;
using System.IO;
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

        public string GetUserDataRootPath()
        {
#if MICROSOFT_STORE
            string userDataRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SideData");
            Directory.CreateDirectory(userDataRoot);

            EnsureStoreDataAsset(userDataRoot, RuntimeDir);
            EnsureStoreDataAsset(userDataRoot, "pictures");
            EnsureStoreDataAsset(userDataRoot, "projects");
            EnsureStoreDataAsset(userDataRoot, "readme.txt");
            EnsureStoreDataAsset(userDataRoot, "使い方.txt");
            EnsureStoreDataAsset(userDataRoot, "勝手にFx14説明書.txt");
            EnsureStoreDataAsset(userDataRoot, ConfigFile);

            return userDataRoot;
#else
            return AppContext.BaseDirectory;
#endif
        }

        public string GetUserDataPath(string relativePath)
        {
            return Path.Combine(GetUserDataRootPath(), relativePath);
        }

        public string GetRuntimeDirectoryPath()
        {
            return GetUserDataPath(RuntimeDir);
        }

        private void EnsureStoreDataAsset(string localRoot, string relativePath)
        {
            string sourcePath = Path.Combine(AppContext.BaseDirectory, relativePath);
            string destinationPath = Path.Combine(localRoot, relativePath);

            if (!File.Exists(sourcePath) && !Directory.Exists(sourcePath))
            {
                return;
            }

            if (File.Exists(destinationPath) || Directory.Exists(destinationPath))
            {
                return;
            }

            if (Directory.Exists(sourcePath))
            {
                CopyDirectory(sourcePath, destinationPath);
            }
            else if (File.Exists(sourcePath))
            {
                string destinationDirectory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }
                File.Copy(sourcePath, destinationPath, true);
            }
        }

        private static void CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            Directory.CreateDirectory(destinationDirectory);

            foreach (string file in Directory.GetFiles(sourceDirectory))
            {
                File.Copy(file, Path.Combine(destinationDirectory, Path.GetFileName(file)), true);
            }

            foreach (string directory in Directory.GetDirectories(sourceDirectory))
            {
                CopyDirectory(directory, Path.Combine(destinationDirectory, Path.GetFileName(directory)));
            }
        }

        public string ConfigFile = "side.xml";

        public string UpdateTempData = "su.xml";

        public int GridInterval = 2;

        public int RulerStep = 5;

        public bool RulerPutDot = true;

        public int LineNoPaddingRight = 1;

        public int LineNoPaddingTop = 1;

        public int ZipExtractBufferLength = 10240;

        public string Version;

        public double CheckVersion = 4.82;

        public string EditorIdStr;

#if MICROSOFT_STORE
        public bool IsAutoUpdateEnabled = false;  // Microsoft Store版では自動更新を無効
#else
        public bool IsAutoUpdateEnabled = true;   // 通常版では自動更新を有効
#endif

        public string BaseUpdateServer = "https://urotaichi.com/other/side/side.xml";
    }
}
