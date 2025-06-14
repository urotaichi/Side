﻿namespace MasaoPlus.Dialogs
{
	public partial class StartUp : global::System.Windows.Forms.Form
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
			this.components = new global::System.ComponentModel.Container();
			this.WelcomeLabel = new global::System.Windows.Forms.Label();
			this.Exit = new global::System.Windows.Forms.Button();
			this.ExMenuStrip = new global::System.Windows.Forms.ContextMenuStrip(this.components);
			this.CallConfig = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new global::System.Windows.Forms.ToolStripSeparator();
			this.CallRuntimeManager = new global::System.Windows.Forms.ToolStripMenuItem();
			this.CallSideUpdate = new global::System.Windows.Forms.ToolStripMenuItem();
			this.ExMenu = new global::System.Windows.Forms.Button();
			this.CmdInheritNew = new global::MasaoPlus.Controls.CommandLinkButton();
			this.CmdOpenFile = new global::MasaoPlus.Controls.CommandLinkButton();
			this.CmdNewProj = new global::MasaoPlus.Controls.CommandLinkButton();
			this.ExMenuStrip.SuspendLayout();
			base.SuspendLayout();
			this.WelcomeLabel.AutoSize = true;
			this.WelcomeLabel.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(12), base.LogicalToDeviceUnits(9));
			this.WelcomeLabel.Name = "WelcomeLabel";
			this.WelcomeLabel.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(96, 12));
			this.WelcomeLabel.TabIndex = 0;
			this.WelcomeLabel.Text = "Quick Description";
			this.Exit.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.Exit.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.Exit.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(147), base.LogicalToDeviceUnits(164));
			this.Exit.Name = "Exit";
			this.Exit.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(237, 24));
			this.Exit.TabIndex = 5;
			this.Exit.Text = "終了(&X)";
			this.Exit.UseVisualStyleBackColor = true;
			this.Exit.Click += new global::System.EventHandler(this.Exit_Click);
			this.ExMenuStrip.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.CallConfig,
				this.toolStripMenuItem1,
				this.CallRuntimeManager,
				this.CallSideUpdate
			});
			this.ExMenuStrip.Name = "ExMenuStrip";
			this.ExMenuStrip.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(219, 76));
			this.CallConfig.Image = new IconImageView(DeviceDpi, global::MasaoPlus.Properties.Resources.cog).View();
			this.CallConfig.Name = "CallConfig";
			this.CallConfig.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(218, 22));
			this.CallConfig.Text = "Sideの設定(&C)";
			this.CallConfig.Click += new global::System.EventHandler(this.CallConfig_Click);
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(215, 6));
			this.CallRuntimeManager.Image = new IconImageView(DeviceDpi, global::MasaoPlus.Properties.Resources.world_add).View();
			this.CallRuntimeManager.Name = "CallRuntimeManager";
			this.CallRuntimeManager.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(218, 22));
			this.CallRuntimeManager.Text = "ランタイム マネージャ(&R)";
			this.CallRuntimeManager.Click += new global::System.EventHandler(this.CallRuntimeManager_Click);
			this.CallSideUpdate.Image = new IconImageView(DeviceDpi, global::MasaoPlus.Properties.Resources.refresh).View();
			this.CallSideUpdate.Name = "CallSideUpdate";
			this.CallSideUpdate.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(218, 22));
			this.CallSideUpdate.Text = "Sideの更新(&U)";
			this.CallSideUpdate.Click += new global::System.EventHandler(this.CallSideUpdate_Click);
			this.ExMenu.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.ExMenu.Image = new IconImageView(DeviceDpi, global::MasaoPlus.Properties.Resources.bullet_arrow_down).View();
			this.ExMenu.ImageAlign = global::System.Drawing.ContentAlignment.MiddleRight;
			this.ExMenu.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(12), base.LogicalToDeviceUnits(164));
			this.ExMenu.Name = "ExMenu";
			this.ExMenu.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(129, 24));
			this.ExMenu.TabIndex = 4;
			this.ExMenu.Text = "その他(&O)";
			this.ExMenu.TextImageRelation = global::System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.ExMenu.UseVisualStyleBackColor = true;
			this.ExMenu.Click += new global::System.EventHandler(this.ExMenu_Click);
			this.CmdInheritNew.Description = "既存のプロジェクトの設定を使って、新しいプロジェクトを作成します。";
			this.CmdInheritNew.DialogResult = global::System.Windows.Forms.DialogResult.None;
			this.CmdInheritNew.Image = new IconImageView(DeviceDpi, global::MasaoPlus.Properties.Resources.page_white_get).View();
			this.CmdInheritNew.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(12), base.LogicalToDeviceUnits(123));
			this.CmdInheritNew.Name = "CmdInheritNew";
			this.CmdInheritNew.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(372, 34));
			this.CmdInheritNew.TabIndex = 3;
			this.CmdInheritNew.Text = "過去のプロジェクトを継承して新規作成";
			global::MasaoPlus.Controls.CommandLinkButton cmdInheritNew = this.CmdInheritNew;
			cmdInheritNew.Click = (global::System.EventHandler)global::System.Delegate.Combine(cmdInheritNew.Click, new global::System.EventHandler(this.InheritNew_Click));
			this.CmdOpenFile.Description = "既存のプロジェクトを開いたり、\r\n正男HTML/JS/JSONから新しいプロジェクトを作成したりします。";
			this.CmdOpenFile.DialogResult = global::System.Windows.Forms.DialogResult.None;
			this.CmdOpenFile.Image = new IconImageView(DeviceDpi, global::MasaoPlus.Properties.Resources.open).View();
			this.CmdOpenFile.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(12), base.LogicalToDeviceUnits(67));
			this.CmdOpenFile.Name = "CmdOpenFile";
			this.CmdOpenFile.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(372, 50));
			this.CmdOpenFile.TabIndex = 2;
			this.CmdOpenFile.Text = "プロジェクトや正男HTMLを開く";
			global::MasaoPlus.Controls.CommandLinkButton cmdOpenFile = this.CmdOpenFile;
			cmdOpenFile.Click = (global::System.EventHandler)global::System.Delegate.Combine(cmdOpenFile.Click, new global::System.EventHandler(this.OpenFile_Click));
			this.CmdNewProj.Description = "空のプロジェクトを作成し、ステージ編集を開始します。";
			this.CmdNewProj.DialogResult = global::System.Windows.Forms.DialogResult.None;
			this.CmdNewProj.Image = new IconImageView(DeviceDpi, global::MasaoPlus.Properties.Resources._new).View();
			this.CmdNewProj.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(12), base.LogicalToDeviceUnits(27));
			this.CmdNewProj.Name = "CmdNewProj";
			this.CmdNewProj.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(372, 34));
			this.CmdNewProj.TabIndex = 1;
			this.CmdNewProj.Text = "新しいプロジェクト";
			global::MasaoPlus.Controls.CommandLinkButton cmdNewProj = this.CmdNewProj;
			cmdNewProj.Click = (global::System.EventHandler)global::System.Delegate.Combine(cmdNewProj.Click, new global::System.EventHandler(this.NewProj_Click));
			//base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			//base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.Exit;
			base.ClientSize = base.LogicalToDeviceUnits(new global::System.Drawing.Size(396, 198));
			base.ControlBox = false;
			base.Controls.Add(this.CmdInheritNew);
			base.Controls.Add(this.WelcomeLabel);
			base.Controls.Add(this.CmdOpenFile);
			base.Controls.Add(this.CmdNewProj);
			base.Controls.Add(this.Exit);
			base.Controls.Add(this.ExMenu);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "StartUp";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Sideの開始";
			base.Load += new global::System.EventHandler(this.StartUp_Load);
			base.Shown += new global::System.EventHandler(this.StartUp_Shown);
			base.FormClosed += new global::System.Windows.Forms.FormClosedEventHandler(this.StartUp_FormClosed);
			this.ExMenuStrip.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.Label WelcomeLabel;

		private global::System.Windows.Forms.Button ExMenu;

		private global::System.Windows.Forms.Button Exit;

		private global::System.Windows.Forms.ContextMenuStrip ExMenuStrip;

		private global::System.Windows.Forms.ToolStripMenuItem CallConfig;

		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;

		private global::System.Windows.Forms.ToolStripMenuItem CallRuntimeManager;

		private global::System.Windows.Forms.ToolStripMenuItem CallSideUpdate;

		private global::MasaoPlus.Controls.CommandLinkButton CmdNewProj;

		private global::MasaoPlus.Controls.CommandLinkButton CmdOpenFile;

		private global::MasaoPlus.Controls.CommandLinkButton CmdInheritNew;
	}
}