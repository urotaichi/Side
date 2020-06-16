using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MasaoPlus.Controls;
using MasaoPlus.Properties;

namespace MasaoPlus.Dialogs
{
	// Token: 0x0200002F RID: 47
	public partial class StartUp : Form
	{
		// Token: 0x060001B9 RID: 441 RVA: 0x000031F3 File Offset: 0x000013F3
		public StartUp()
		{
			this.InitializeComponent();
			base.DialogResult = DialogResult.None;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00003213 File Offset: 0x00001413
		private void StartUp_Load(object sender, EventArgs e)
		{
			this.WelcomeLabel.Text = Global.definition.AppNameFull + " v" + Global.definition.Version;
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000323E File Offset: 0x0000143E
		private void StartUp_Shown(object sender, EventArgs e)
		{
			Application.DoEvents();
			if (Global.config.localSystem.CheckAutoUpdate)
			{
				Subsystem.UpdateAutoCheck();
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000325B File Offset: 0x0000145B
		private void StartUp_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (base.DialogResult != DialogResult.OK)
			{
				Application.Exit();
			}
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000326B File Offset: 0x0000146B
		private void Exit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0002435C File Offset: 0x0002255C
		private void NewProj_Click(object sender, EventArgs e)
		{
			using (NewProject newProject = new NewProject())
			{
				if (newProject.ShowDialog() == DialogResult.OK)
				{
					this.ProjectPath = newProject.CreatedProject;
					base.DialogResult = DialogResult.OK;
					base.Close();
				}
			}
		}

		// Token: 0x060001BF RID: 447 RVA: 0x000243B0 File Offset: 0x000225B0
		private void OpenFile_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = string.Concat(new string[]
				{
					Global.definition.AppName,
					"プロジェクト及びHTML/XML(*.html,*.xml,*",
					Global.definition.ProjExt,
					")|*.html;*.xml;*",
					Global.definition.ProjExt,
					"|",
					Global.definition.AppName,
					" プロジェクト (*",
					Global.definition.ProjExt,
					")|*",
					Global.definition.ProjExt,
					"|HTML/XML ドキュメント(*.htm*;*.xml)|*.htm*;*.xml|全てのファイル|*.*"
				});
				openFileDialog.InitialDirectory = Global.config.lastData.ProjDirF;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					if (Path.GetExtension(openFileDialog.FileName) == Global.definition.ProjExt)
					{
						this.ProjectPath = openFileDialog.FileName;
						base.DialogResult = DialogResult.OK;
						base.Close();
					}
					else
					{
						using (HTMLInheritance htmlinheritance = new HTMLInheritance(openFileDialog.FileName))
						{
							if (htmlinheritance.ShowDialog() == DialogResult.OK)
							{
								this.ProjectPath = htmlinheritance.ProjectFile;
								base.DialogResult = DialogResult.OK;
								base.Close();
							}
						}
					}
				}
			}
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00003272 File Offset: 0x00001472
		private void ExMenu_Click(object sender, EventArgs e)
		{
			this.ExMenuStrip.Show(this.ExMenu, new Point(0, this.ExMenu.Height));
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x00024528 File Offset: 0x00022728
		private void CallConfig_Click(object sender, EventArgs e)
		{
			using (SideConfig sideConfig = new SideConfig())
			{
				sideConfig.ShowDialog();
			}
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x00023968 File Offset: 0x00021B68
		private void CallRuntimeManager_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = DialogResult.Retry;
			while (dialogResult == DialogResult.Retry)
			{
				using (RuntimeManager runtimeManager = new RuntimeManager())
				{
					dialogResult = runtimeManager.ShowDialog();
				}
			}
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x00023BAC File Offset: 0x00021DAC
		private void CallSideUpdate_Click(object sender, EventArgs e)
		{
			using (WebUpdate webUpdate = new WebUpdate())
			{
				if (webUpdate.ShowDialog() == DialogResult.Retry)
				{
					Global.state.RunFile = (string)webUpdate.runfile.Clone();
					base.Close();
				}
			}
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00024560 File Offset: 0x00022760
		private void InheritNew_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = string.Concat(new string[]
				{
					Global.definition.AppName,
					" プロジェクト (*",
					Global.definition.ProjExt,
					")|*",
					Global.definition.ProjExt
				});
				openFileDialog.InitialDirectory = Global.config.lastData.ProjDirF;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					using (ProjInheritance projInheritance = new ProjInheritance(openFileDialog.FileName))
					{
						if (projInheritance.DialogResult != DialogResult.Abort && projInheritance.ShowDialog() == DialogResult.OK)
						{
							this.ProjectPath = projInheritance.NewProjectName;
							base.DialogResult = DialogResult.OK;
							base.Close();
						}
					}
				}
			}
		}

		// Token: 0x04000242 RID: 578
		public string ProjectPath = "";
	}
}
