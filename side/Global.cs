using System;

namespace MasaoPlus
{
    internal static class Global
    {
        public const int ScrollbarWidth = 20;

        public static MainWindow MainWnd = null;

        public static Definition definition = new();

        public static Config config = new();

        public static State state = new();

        public static CurrentProjectData cpd = new();
    }
}
