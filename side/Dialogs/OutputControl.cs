using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MasaoPlus.Dialogs
{
	public partial class OutputControl : Form
	{
		public OutputControl(string MoveToDir)
		{
			this.InitializeComponent();
			this.mdir = MoveToDir;
		}

		private void OutputControl_Load(object sender, EventArgs e)
		{
			this.OutputSelector.Items.Clear();
			foreach (string path in Directory.GetFiles(Global.cpd.where, "*", SearchOption.TopDirectoryOnly))
			{
				this.OutputSelector.Items.Add(Path.GetFileName(path));
			}
		}

		private void AllCheck_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.OutputSelector.Items.Count; i++)
			{
				this.OutputSelector.SetItemChecked(i, true);
			}
		}

		private void AllUncheck_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.OutputSelector.Items.Count; i++)
			{
				this.OutputSelector.SetItemChecked(i, false);
			}
		}

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

		private string mdir;
	}
}
