using System;

namespace MasaoPlus
{
	internal static class Global
	{
		public const int ScrollbarWidth = 20;

		public static MainWindow MainWnd = null;

		public static Definition definition = new Definition();

		public static Config config = new Config();

		public static State state = new State();

		public static CurrentProjectData cpd = new CurrentProjectData();
	}
}
