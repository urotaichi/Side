using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace MasaoPlus.Properties
{
	// Token: 0x02000019 RID: 25
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x000028A0 File Offset: 0x00000AA0
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x040000D5 RID: 213
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
