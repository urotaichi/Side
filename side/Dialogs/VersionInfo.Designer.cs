namespace MasaoPlus.Dialogs
{
	// Token: 0x02000038 RID: 56
	public partial class VersionInfo : global::System.Windows.Forms.Form
	{
		// Token: 0x0600022B RID: 555 RVA: 0x00003768 File Offset: 0x00001968
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0002928C File Offset: 0x0002748C
		private void InitializeComponent()
		{
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::MasaoPlus.Dialogs.VersionInfo));
			this.OKButton = new global::System.Windows.Forms.Button();
			this.AppName = new global::System.Windows.Forms.Label();
			this.pictureBox1 = new global::System.Windows.Forms.PictureBox();
			this.label2 = new global::System.Windows.Forms.Label();
			this.linkLabel1 = new global::System.Windows.Forms.LinkLabel();
			this.ViewDat = new global::System.Windows.Forms.RichTextBox();
			this.VersionLabel = new global::System.Windows.Forms.Label();
			this.panel1 = new global::System.Windows.Forms.Panel();
			this.linkLabel2 = new global::System.Windows.Forms.LinkLabel();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.OKButton.Location = new global::System.Drawing.Point(357 * DeviceDpi / 96, 201 * DeviceDpi / 96);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new global::System.Drawing.Size(128 * DeviceDpi / 96, 28 * DeviceDpi / 96);
			this.OKButton.TabIndex = 4;
			this.OKButton.Text = "OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new global::System.EventHandler(this.OKButton_Click);
			this.AppName.AutoSize = true;
			this.AppName.Font = new global::System.Drawing.Font("メイリオ", 11.25f, global::System.Drawing.FontStyle.Bold, global::System.Drawing.GraphicsUnit.Point, 128);
			this.AppName.Location = new global::System.Drawing.Point(51 * DeviceDpi / 96, 12 * DeviceDpi / 96);
			this.AppName.Name = "AppName";
			this.AppName.Size = new global::System.Drawing.Size(411 * DeviceDpi / 96, 23 * DeviceDpi / 96);
			this.AppName.TabIndex = 0;
			this.AppName.Text = "Supermasao Integrated Development Environment";
			this.pictureBox1.Image = global::MasaoPlus.Properties.Resources.side;
			this.pictureBox1.Location = new global::System.Drawing.Point(12 * DeviceDpi / 96, 12 * DeviceDpi / 96);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new global::System.Drawing.Size(32 * DeviceDpi / 96, 32 * DeviceDpi / 96);
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			this.label2.AutoSize = true;
			this.label2.Location = new global::System.Drawing.Point(12 * DeviceDpi / 96, 209 * DeviceDpi / 96);
			this.label2.Name = "label2";
			this.label2.Size = new global::System.Drawing.Size(114 * DeviceDpi / 96, 12 * DeviceDpi / 96);
			this.label2.TabIndex = 2;
			this.label2.Text = "© 2020 urotaichi";
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new global::System.Drawing.Point(152 * DeviceDpi / 96, 209 * DeviceDpi / 96);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new global::System.Drawing.Size(106 * DeviceDpi / 96, 12 * DeviceDpi / 96);
			this.linkLabel1.TabIndex = 3;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "https://urotaichi.com/";
			this.linkLabel1.LinkClicked += new global::System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
			this.ViewDat.BackColor = global::System.Drawing.Color.White;
			this.ViewDat.BorderStyle = global::System.Windows.Forms.BorderStyle.None;
			this.ViewDat.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.ViewDat.Font = new global::System.Drawing.Font("ＭＳ ゴシック", 9f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 128);
			this.ViewDat.Location = new global::System.Drawing.Point(1 * DeviceDpi / 96, 1 * DeviceDpi / 96);
			this.ViewDat.Margin = new global::System.Windows.Forms.Padding(1);
			this.ViewDat.Name = "ViewDat";
			this.ViewDat.ReadOnly = true;
			this.ViewDat.Size = new global::System.Drawing.Size(428 * DeviceDpi / 96, 136 * DeviceDpi / 96);
			this.ViewDat.TabIndex = 0;
			this.ViewDat.Text = componentResourceManager.GetString("ViewDat.Text");
			this.ViewDat.WordWrap = false;
			this.ViewDat.LinkClicked += new global::System.Windows.Forms.LinkClickedEventHandler(this.ViewDat_LinkClicked);
			this.VersionLabel.Font = new global::System.Drawing.Font("メイリオ", 9f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			this.VersionLabel.Location = new global::System.Drawing.Point(341 * DeviceDpi / 96, 25 * DeviceDpi / 96);
			this.VersionLabel.Name = "VersionLabel";
			this.VersionLabel.Size = new global::System.Drawing.Size(144 * DeviceDpi / 96, 28 * DeviceDpi / 96);
			this.VersionLabel.TabIndex = 1;
			this.VersionLabel.Text = "v 0.00";
			this.VersionLabel.TextAlign = global::System.Drawing.ContentAlignment.BottomRight;
			this.panel1.BackColor = global::System.Drawing.Color.SteelBlue;
			this.panel1.Controls.Add(this.ViewDat);
			this.panel1.Location = new global::System.Drawing.Point(55 * DeviceDpi / 96, 57 * DeviceDpi / 96);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new global::System.Windows.Forms.Padding(1);
			this.panel1.Size = new global::System.Drawing.Size(430 * DeviceDpi / 96, 138 * DeviceDpi / 96);
			this.panel1.TabIndex = 8;
			this.linkLabel2.AutoSize = true;
			this.linkLabel2.Location = new global::System.Drawing.Point(244 * DeviceDpi / 96, 209 * DeviceDpi / 96);
			this.linkLabel2.Name = "linkLabel2";
			this.linkLabel2.Size = new global::System.Drawing.Size(101 * DeviceDpi / 96, 12 * DeviceDpi / 96);
			this.linkLabel2.TabIndex = 9;
			this.linkLabel2.TabStop = true;
			this.linkLabel2.Text = "";
			this.linkLabel2.LinkClicked += new global::System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelClicked);
			base.AcceptButton = this.OKButton;
			//base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			//base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(497 * DeviceDpi / 96, 239 * DeviceDpi / 96);
			base.ControlBox = false;
			base.Controls.Add(this.linkLabel2);
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.linkLabel1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.pictureBox1);
			base.Controls.Add(this.AppName);
			base.Controls.Add(this.OKButton);
			base.Controls.Add(this.VersionLabel);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "VersionInfo";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About Side";
			base.Load += new global::System.EventHandler(this.VersionInfo_Load);
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
			this.panel1.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x040002B2 RID: 690
		private global::System.ComponentModel.IContainer components;

		// Token: 0x040002B3 RID: 691
		private global::System.Windows.Forms.Button OKButton;

		// Token: 0x040002B4 RID: 692
		private global::System.Windows.Forms.Label AppName;

		// Token: 0x040002B5 RID: 693
		private global::System.Windows.Forms.PictureBox pictureBox1;

		// Token: 0x040002B6 RID: 694
		private global::System.Windows.Forms.Label label2;

		// Token: 0x040002B7 RID: 695
		private global::System.Windows.Forms.LinkLabel linkLabel1;

		// Token: 0x040002B8 RID: 696
		private global::System.Windows.Forms.RichTextBox ViewDat;

		// Token: 0x040002B9 RID: 697
		private global::System.Windows.Forms.Label VersionLabel;

		// Token: 0x040002BA RID: 698
		private global::System.Windows.Forms.Panel panel1;

		// Token: 0x040002BB RID: 699
		private global::System.Windows.Forms.LinkLabel linkLabel2;
	}
}
