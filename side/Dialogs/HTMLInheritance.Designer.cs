namespace MasaoPlus.Dialogs
{
	// Token: 0x02000018 RID: 24
	public partial class HTMLInheritance : global::System.Windows.Forms.Form
	{
		// Token: 0x060000CD RID: 205 RVA: 0x00002879 File Offset: 0x00000A79
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00012974 File Offset: 0x00010B74
		private void InitializeComponent()
		{
			this.label1 = new global::System.Windows.Forms.Label();
			this.RuntimeSet = new global::System.Windows.Forms.ComboBox();
			this.groupBox1 = new global::System.Windows.Forms.GroupBox();
			this.SeekHeaderFooter = new global::System.Windows.Forms.CheckBox();
			this.PNLabel = new global::System.Windows.Forms.Label();
			this.ProjectName = new global::System.Windows.Forms.TextBox();
			this.RootDir = new global::System.Windows.Forms.TextBox();
			this.RDLabel = new global::System.Windows.Forms.Label();
			this.RootDirBrowse = new global::System.Windows.Forms.Button();
			this.button2 = new global::System.Windows.Forms.Button();
			this.OK = new global::System.Windows.Forms.Button();
			this.StatusText = new global::System.Windows.Forms.Label();
			this.label3 = new global::System.Windows.Forms.Label();
			this.TargetFile = new global::System.Windows.Forms.TextBox();
			this.label2 = new global::System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.ForeColor = global::System.Drawing.Color.Red;
			this.label1.Location = new global::System.Drawing.Point(12, 87);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(101, 12);
			this.label1.TabIndex = 7;
			this.label1.Text = "解析に使うランタイム";
			this.RuntimeSet.BackColor = global::System.Drawing.Color.LightPink;
			this.RuntimeSet.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.RuntimeSet.DropDownWidth = 400;
			this.RuntimeSet.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.RuntimeSet.ForeColor = global::System.Drawing.Color.Red;
			this.RuntimeSet.FormattingEnabled = true;
			this.RuntimeSet.Location = new global::System.Drawing.Point(119, 84);
			this.RuntimeSet.Name = "RuntimeSet";
			this.RuntimeSet.Size = new global::System.Drawing.Size(320, 20);
			this.RuntimeSet.TabIndex = 8;
			this.groupBox1.Controls.Add(this.SeekHeaderFooter);
			this.groupBox1.Location = new global::System.Drawing.Point(12, 132);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new global::System.Drawing.Size(427, 41);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "解析オプション";
			this.SeekHeaderFooter.AutoSize = true;
			this.SeekHeaderFooter.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.SeekHeaderFooter.Location = new global::System.Drawing.Point(6, 18);
			this.SeekHeaderFooter.Name = "SeekHeaderFooter";
			this.SeekHeaderFooter.Size = new global::System.Drawing.Size(265, 16);
			this.SeekHeaderFooter.TabIndex = 0;
			this.SeekHeaderFooter.Text = "ヘッダとフッタも解析[appletタグ使用時のみ有効](&H)";
			this.SeekHeaderFooter.UseVisualStyleBackColor = true;
			this.PNLabel.AutoSize = true;
			this.PNLabel.Location = new global::System.Drawing.Point(12, 36);
			this.PNLabel.Name = "PNLabel";
			this.PNLabel.Size = new global::System.Drawing.Size(68, 12);
			this.PNLabel.TabIndex = 2;
			this.PNLabel.Text = "プロジェクト名";
			this.ProjectName.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.ProjectName.Location = new global::System.Drawing.Point(119, 34);
			this.ProjectName.Name = "ProjectName";
			this.ProjectName.Size = new global::System.Drawing.Size(320, 19);
			this.ProjectName.TabIndex = 3;
			this.ProjectName.Text = "NewProject";
			this.ProjectName.TextChanged += new global::System.EventHandler(this.ProjectName_TextChanged);
			this.RootDir.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.RootDir.Location = new global::System.Drawing.Point(119, 59);
			this.RootDir.Name = "RootDir";
			this.RootDir.Size = new global::System.Drawing.Size(293, 19);
			this.RootDir.TabIndex = 5;
			this.RootDir.TextChanged += new global::System.EventHandler(this.RootDir_TextChanged);
			this.RDLabel.AutoSize = true;
			this.RDLabel.Location = new global::System.Drawing.Point(12, 61);
			this.RDLabel.Name = "RDLabel";
			this.RDLabel.Size = new global::System.Drawing.Size(82, 12);
			this.RDLabel.TabIndex = 4;
			this.RDLabel.Text = "ルートディレクトリ";
			this.RootDirBrowse.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.RootDirBrowse.Location = new global::System.Drawing.Point(418, 58);
			this.RootDirBrowse.Name = "RootDirBrowse";
			this.RootDirBrowse.Size = new global::System.Drawing.Size(21, 19);
			this.RootDirBrowse.TabIndex = 6;
			this.RootDirBrowse.Text = "...";
			this.RootDirBrowse.UseVisualStyleBackColor = true;
			this.button2.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.button2.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.button2.Location = new global::System.Drawing.Point(337, 179);
			this.button2.Name = "button2";
			this.button2.Size = new global::System.Drawing.Size(102, 30);
			this.button2.TabIndex = 12;
			this.button2.Text = "キャンセル";
			this.button2.UseVisualStyleBackColor = true;
			this.OK.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.OK.Location = new global::System.Drawing.Point(229, 179);
			this.OK.Name = "OK";
			this.OK.Size = new global::System.Drawing.Size(102, 30);
			this.OK.TabIndex = 11;
			this.OK.Text = "OK";
			this.OK.UseVisualStyleBackColor = true;
			this.OK.Click += new global::System.EventHandler(this.OK_Click);
			this.StatusText.AutoSize = true;
			this.StatusText.Location = new global::System.Drawing.Point(16, 169);
			this.StatusText.Name = "StatusText";
			this.StatusText.Size = new global::System.Drawing.Size(0, 12);
			this.StatusText.TabIndex = 10;
			this.label3.AutoSize = true;
			this.label3.Location = new global::System.Drawing.Point(12, 11);
			this.label3.Name = "label3";
			this.label3.Size = new global::System.Drawing.Size(87, 12);
			this.label3.TabIndex = 0;
			this.label3.Text = "解析対象ファイル";
			this.TargetFile.BackColor = global::System.Drawing.SystemColors.ControlLight;
			this.TargetFile.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.TargetFile.ForeColor = global::System.Drawing.SystemColors.InactiveCaptionText;
			this.TargetFile.Location = new global::System.Drawing.Point(119, 9);
			this.TargetFile.Name = "TargetFile";
			this.TargetFile.ReadOnly = true;
			this.TargetFile.Size = new global::System.Drawing.Size(320, 19);
			this.TargetFile.TabIndex = 1;
			this.TargetFile.Text = "NewProject";
			this.label2.AutoSize = true;
			this.label2.ForeColor = global::System.Drawing.Color.Red;
			this.label2.Location = new global::System.Drawing.Point(117, 105);
			this.label2.Name = "label2";
			this.label2.Size = new global::System.Drawing.Size(278, 24);
			this.label2.TabIndex = 13;
			this.label2.Text = "正しいランタイムでないと、プロジェクトの作成に失敗したり、\r\nプロジェクトが開けなくなったりします。";
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = global::System.Drawing.SystemColors.Control;
			base.ClientSize = new global::System.Drawing.Size(451, 221);
			base.ControlBox = false;
			base.Controls.Add(this.label2);
			base.Controls.Add(this.TargetFile);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.StatusText);
			base.Controls.Add(this.OK);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.PNLabel);
			base.Controls.Add(this.ProjectName);
			base.Controls.Add(this.RootDir);
			base.Controls.Add(this.RDLabel);
			base.Controls.Add(this.RootDirBrowse);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.RuntimeSet);
			base.Controls.Add(this.label1);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "HTMLInheritance";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "HTMLからプロジェクトを作成";
			base.Load += new global::System.EventHandler(this.Inheritance_Load);
			base.Shown += new global::System.EventHandler(this.Inheritance_Shown);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x040000C0 RID: 192
		private global::System.ComponentModel.IContainer components;

		// Token: 0x040000C1 RID: 193
		private global::System.Windows.Forms.Label label1;

		// Token: 0x040000C2 RID: 194
		private global::System.Windows.Forms.ComboBox RuntimeSet;

		// Token: 0x040000C3 RID: 195
		private global::System.Windows.Forms.GroupBox groupBox1;

		// Token: 0x040000C4 RID: 196
		private global::System.Windows.Forms.CheckBox SeekHeaderFooter;

		// Token: 0x040000C5 RID: 197
		private global::System.Windows.Forms.Label PNLabel;

		// Token: 0x040000C6 RID: 198
		private global::System.Windows.Forms.TextBox ProjectName;

		// Token: 0x040000C7 RID: 199
		private global::System.Windows.Forms.TextBox RootDir;

		// Token: 0x040000C8 RID: 200
		private global::System.Windows.Forms.Label RDLabel;

		// Token: 0x040000C9 RID: 201
		private global::System.Windows.Forms.Button RootDirBrowse;

		// Token: 0x040000CA RID: 202
		private global::System.Windows.Forms.Button button2;

		// Token: 0x040000CB RID: 203
		private global::System.Windows.Forms.Button OK;

		// Token: 0x040000CC RID: 204
		private global::System.Windows.Forms.Label StatusText;

		// Token: 0x040000CD RID: 205
		private global::System.Windows.Forms.Label label3;

		// Token: 0x040000CE RID: 206
		private global::System.Windows.Forms.TextBox TargetFile;

		// Token: 0x040000CF RID: 207
		private global::System.Windows.Forms.Label label2;
	}
}
