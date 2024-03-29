﻿namespace MasaoPlus.Dialogs
{
	public partial class HTMLInheritance : global::System.Windows.Forms.Form
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
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
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
			this.label1.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(12), base.LogicalToDeviceUnits(87));
			this.label1.Name = "label1";
			this.label1.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(101, 12));
			this.label1.TabIndex = 7;
			this.label1.Text = "解析に使うランタイム";
			this.RuntimeSet.BackColor = global::System.Drawing.Color.LightPink;
			this.RuntimeSet.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.RuntimeSet.DropDownWidth = 400;
			this.RuntimeSet.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.RuntimeSet.ForeColor = global::System.Drawing.Color.Red;
			this.RuntimeSet.FormattingEnabled = true;
			this.RuntimeSet.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(119), base.LogicalToDeviceUnits(84));
			this.RuntimeSet.Name = "RuntimeSet";
			this.RuntimeSet.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(320, 20));
			this.RuntimeSet.TabIndex = 8;
			this.groupBox1.Controls.Add(this.SeekHeaderFooter);
			this.groupBox1.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(12), base.LogicalToDeviceUnits(132));
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(427, 41));
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "解析オプション";
			this.SeekHeaderFooter.AutoSize = true;
			this.SeekHeaderFooter.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.SeekHeaderFooter.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(18));
			this.SeekHeaderFooter.Name = "SeekHeaderFooter";
			this.SeekHeaderFooter.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(265, 16));
			this.SeekHeaderFooter.TabIndex = 0;
			this.SeekHeaderFooter.Text = "ヘッダとフッタも解析[appletタグ使用時のみ有効](&H)";
			this.SeekHeaderFooter.UseVisualStyleBackColor = true;
			this.PNLabel.AutoSize = true;
			this.PNLabel.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(12), base.LogicalToDeviceUnits(36));
			this.PNLabel.Name = "PNLabel";
			this.PNLabel.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(68, 12));
			this.PNLabel.TabIndex = 2;
			this.PNLabel.Text = "プロジェクト名";
			this.ProjectName.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.ProjectName.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(119), base.LogicalToDeviceUnits(34));
			this.ProjectName.Name = "ProjectName";
			this.ProjectName.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(320, 19));
			this.ProjectName.TabIndex = 3;
			this.ProjectName.Text = "NewProject";
			this.ProjectName.TextChanged += new global::System.EventHandler(this.ProjectName_TextChanged);
			this.RootDir.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.RootDir.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(119), base.LogicalToDeviceUnits(59));
			this.RootDir.Name = "RootDir";
			this.RootDir.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(293, 19));
			this.RootDir.TabIndex = 5;
			this.RootDir.TextChanged += new global::System.EventHandler(this.RootDir_TextChanged);
			this.RDLabel.AutoSize = true;
			this.RDLabel.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(12), base.LogicalToDeviceUnits(61));
			this.RDLabel.Name = "RDLabel";
			this.RDLabel.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(82, 12));
			this.RDLabel.TabIndex = 4;
			this.RDLabel.Text = "ルートディレクトリ";
			this.RootDirBrowse.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.RootDirBrowse.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(418), base.LogicalToDeviceUnits(58));
			this.RootDirBrowse.Name = "RootDirBrowse";
			this.RootDirBrowse.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(21, 19));
			this.RootDirBrowse.TabIndex = 6;
			this.RootDirBrowse.Text = "...";
			this.RootDirBrowse.UseVisualStyleBackColor = true;
			this.button2.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.button2.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.button2.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(337), base.LogicalToDeviceUnits(179));
			this.button2.Name = "button2";
			this.button2.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(102, 30));
			this.button2.TabIndex = 12;
			this.button2.Text = "キャンセル";
			this.button2.UseVisualStyleBackColor = true;
			this.OK.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.OK.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(229), base.LogicalToDeviceUnits(179));
			this.OK.Name = "OK";
			this.OK.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(102, 30));
			this.OK.TabIndex = 11;
			this.OK.Text = "OK";
			this.OK.UseVisualStyleBackColor = true;
			this.OK.Click += new global::System.EventHandler(this.OK_Click);
			this.StatusText.AutoSize = true;
			this.StatusText.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(16), base.LogicalToDeviceUnits(169));
			this.StatusText.Name = "StatusText";
			this.StatusText.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(0, 12));
			this.StatusText.TabIndex = 10;
			this.label3.AutoSize = true;
			this.label3.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(12), base.LogicalToDeviceUnits(11));
			this.label3.Name = "label3";
			this.label3.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(87, 12));
			this.label3.TabIndex = 0;
			this.label3.Text = "解析対象ファイル";
			this.TargetFile.BackColor = global::System.Drawing.SystemColors.ControlLight;
			this.TargetFile.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.TargetFile.ForeColor = global::System.Drawing.SystemColors.InactiveCaptionText;
			this.TargetFile.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(119), base.LogicalToDeviceUnits(9));
			this.TargetFile.Name = "TargetFile";
			this.TargetFile.ReadOnly = true;
			this.TargetFile.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(320, 19));
			this.TargetFile.TabIndex = 1;
			this.TargetFile.Text = "NewProject";
			this.label2.AutoSize = true;
			this.label2.ForeColor = global::System.Drawing.Color.Red;
			this.label2.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(117), base.LogicalToDeviceUnits(105));
			this.label2.Name = "label2";
			this.label2.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(278, 24));
			this.label2.TabIndex = 13;
			this.label2.Text = "正しいランタイムでないと、プロジェクトの作成に失敗したり、\r\nプロジェクトが開けなくなったりします。";
			//base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			//base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = global::System.Drawing.SystemColors.Control;
			base.ClientSize = base.LogicalToDeviceUnits(new global::System.Drawing.Size(451, 221));
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

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.Label label1;

		private global::System.Windows.Forms.ComboBox RuntimeSet;

		private global::System.Windows.Forms.GroupBox groupBox1;

		private global::System.Windows.Forms.CheckBox SeekHeaderFooter;

		private global::System.Windows.Forms.Label PNLabel;

		private global::System.Windows.Forms.TextBox ProjectName;

		private global::System.Windows.Forms.TextBox RootDir;

		private global::System.Windows.Forms.Label RDLabel;

		private global::System.Windows.Forms.Button RootDirBrowse;

		private global::System.Windows.Forms.Button button2;

		private global::System.Windows.Forms.Button OK;

		private global::System.Windows.Forms.Label StatusText;

		private global::System.Windows.Forms.Label label3;

		private global::System.Windows.Forms.TextBox TargetFile;

		private global::System.Windows.Forms.Label label2;
	}
}
