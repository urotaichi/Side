using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MasaoPlus.Dialogs
{
	// Token: 0x02000029 RID: 41
	public partial class OutputControl : Form
	{
		// Token: 0x06000127 RID: 295 RVA: 0x00002BF9 File Offset: 0x00000DF9
		public OutputControl(string MoveToDir)
		{
			this.InitializeComponent();
			this.mdir = MoveToDir;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00018600 File Offset: 0x00016800
		private void OutputControl_Load(object sender, EventArgs e)
		{
			this.OutputSelector.Items.Clear();
			foreach (string path in Directory.GetFiles(Global.cpd.where, "*", SearchOption.TopDirectoryOnly))
			{
				this.OutputSelector.Items.Add(Path.GetFileName(path));
			}
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0001865C File Offset: 0x0001685C
		private void AllCheck_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.OutputSelector.Items.Count; i++)
			{
				this.OutputSelector.SetItemChecked(i, true);
			}
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00018694 File Offset: 0x00016894
		private void AllUncheck_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.OutputSelector.Items.Count; i++)
			{
				this.OutputSelector.SetItemChecked(i, false);
			}
		}

		// Token: 0x0600012B RID: 299 RVA: 0x000186CC File Offset: 0x000168CC
		private void AllWiseCheck_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.OutputSelector.Items.Count; i++)
			{
				string text = Path.Combine(Global.cpd.where, this.OutputSelector.Items[i].ToString());
				text = Path.GetExtension(text);
				if (text != Global.definition.ProjExt && text != ".html" && text != ".sdx")
				{
					this.OutputSelector.SetItemChecked(i, true);
				}
				else
				{
					this.OutputSelector.SetItemChecked(i, false);
				}
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00018774 File Offset: 0x00016974
		private void OK_Click(object sender, EventArgs e)
		{
			base.Enabled = false;
			this.OK.Text = "出力中...";
			this.OK.Refresh();
			Application.DoEvents();
			if (this.OutputSelector.CheckedItems.Count != 0)
			{
				foreach (object obj in this.OutputSelector.CheckedItems)
				{
					string sourceFileName = Path.Combine(Global.cpd.where, obj.ToString());
					string text = Path.Combine(this.mdir, obj.ToString());
					if (!File.Exists(text) || MessageBox.Show(text + "はすでに存在しています。" + Environment.NewLine + "上書きしてもよろしいですか？", "上書きの確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.No)
					{
						File.Copy(sourceFileName, text, true);
					}
				}
			}
			base.DialogResult = DialogResult.OK;
			base.Close();
		}

		// Token: 0x0400016C RID: 364
		private string mdir;
	}
}
