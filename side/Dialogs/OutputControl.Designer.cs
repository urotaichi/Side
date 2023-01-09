namespace MasaoPlus.Dialogs
{
	public partial class OutputControl : global::System.Windows.Forms.Form
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.OutputSelector = new global::System.Windows.Forms.CheckedListBox();
			this.label1 = new global::System.Windows.Forms.Label();
			this.OK = new global::System.Windows.Forms.Button();
			this.Cancel = new global::System.Windows.Forms.Button();
			this.AllCheck = new global::System.Windows.Forms.Button();
			this.AllUncheck = new global::System.Windows.Forms.Button();
			this.AllWiseCheck = new global::System.Windows.Forms.Button();
			base.SuspendLayout();
			this.OutputSelector.CheckOnClick = true;
			this.OutputSelector.FormattingEnabled = true;
			this.OutputSelector.HorizontalScrollbar = true;
			this.OutputSelector.IntegralHeight = false;
			this.OutputSelector.Location = new global::System.Drawing.Point(14, 27);
			this.OutputSelector.Name = "OutputSelector";
			this.OutputSelector.Size = new global::System.Drawing.Size(419, 176);
			this.OutputSelector.TabIndex = 1;
			this.label1.AutoSize = true;
			this.label1.Location = new global::System.Drawing.Point(12, 9);
			this.label1.Margin = new global::System.Windows.Forms.Padding(3, 0, 3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(259, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "チェックされたファイルをファイルの出力先にコピーします。";
			this.OK.Location = new global::System.Drawing.Point(231, 238);
			this.OK.Name = "OK";
			this.OK.Size = new global::System.Drawing.Size(98, 25);
			this.OK.TabIndex = 5;
			this.OK.Text = "OK";
			this.OK.UseVisualStyleBackColor = true;
			this.OK.Click += new global::System.EventHandler(this.OK_Click);
			this.Cancel.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.Cancel.Location = new global::System.Drawing.Point(335, 238);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new global::System.Drawing.Size(98, 25);
			this.Cancel.TabIndex = 6;
			this.Cancel.Text = "キャンセル";
			this.Cancel.UseVisualStyleBackColor = true;
			this.AllCheck.Location = new global::System.Drawing.Point(14, 209);
			this.AllCheck.Name = "AllCheck";
			this.AllCheck.Size = new global::System.Drawing.Size(89, 23);
			this.AllCheck.TabIndex = 2;
			this.AllCheck.Text = "すべてチェック";
			this.AllCheck.UseVisualStyleBackColor = true;
			this.AllCheck.Click += new global::System.EventHandler(this.AllCheck_Click);
			this.AllUncheck.Location = new global::System.Drawing.Point(109, 209);
			this.AllUncheck.Name = "AllUncheck";
			this.AllUncheck.Size = new global::System.Drawing.Size(47, 23);
			this.AllUncheck.TabIndex = 3;
			this.AllUncheck.Text = "解除";
			this.AllUncheck.UseVisualStyleBackColor = true;
			this.AllUncheck.Click += new global::System.EventHandler(this.AllUncheck_Click);
			this.AllWiseCheck.Location = new global::System.Drawing.Point(162, 209);
			this.AllWiseCheck.Name = "AllWiseCheck";
			this.AllWiseCheck.Size = new global::System.Drawing.Size(271, 23);
			this.AllWiseCheck.TabIndex = 4;
			this.AllWiseCheck.Text = "必要無いと思われるファイルを除いてすべてチェック";
			this.AllWiseCheck.UseVisualStyleBackColor = true;
			this.AllWiseCheck.Click += new global::System.EventHandler(this.AllWiseCheck_Click);
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(445, 275);
			base.Controls.Add(this.AllWiseCheck);
			base.Controls.Add(this.AllUncheck);
			base.Controls.Add(this.AllCheck);
			base.Controls.Add(this.Cancel);
			base.Controls.Add(this.OK);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.OutputSelector);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "OutputControl";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "出力の確認";
			base.Load += new global::System.EventHandler(this.OutputControl_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.CheckedListBox OutputSelector;

		private global::System.Windows.Forms.Label label1;

		private global::System.Windows.Forms.Button OK;

		private global::System.Windows.Forms.Button Cancel;

		private global::System.Windows.Forms.Button AllCheck;

		private global::System.Windows.Forms.Button AllUncheck;

		private global::System.Windows.Forms.Button AllWiseCheck;
	}
}
