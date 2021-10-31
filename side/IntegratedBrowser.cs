using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MasaoPlus.Properties;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

namespace MasaoPlus
{
	// Token: 0x02000003 RID: 3
	public class IntegratedBrowser : UserControl
	{
		// Token: 0x0600000A RID: 10 RVA: 0x000020B4 File Offset: 0x000002B4
		public IntegratedBrowser()
		{
			this.InitializeComponent();
			//this.Browser.StatusTextChanged += this.Browser_StatusTextChanged;
			this.InitializeAsync();
		}

		/*
		// Token: 0x0600000B RID: 11 RVA: 0x000020D9 File Offset: 0x000002D9
		private void Browser_StatusTextChanged(object sender, EventArgs e)
		{
			this.Status.Text = this.Browser.StatusText;
		}
		*/

		// Token: 0x0600000C RID: 12 RVA: 0x00004168 File Offset: 0x00002368
		public bool Navigate(string str)
		{
			if (!Global.config.testRun.UseIntegratedBrowser)
			{
				try
				{
					if (Global.config.localSystem.UsingWebBrowser != "" && Global.config.localSystem.UsingWebBrowser != null)
					{
						Global.state.Testrun = Process.Start(Global.config.localSystem.UsingWebBrowser, str);
					}
					else
					{
						Global.state.Testrun = Process.Start(str);
					}
					Process testrun = Global.state.Testrun;
					base.Enabled = false;
					this.Status.Text = "統合ブラウザは利用不可能です。";
					return false;
				}
				catch (Exception ex)
				{
					MessageBox.Show("ブラウザを起動できませんでした。" + Environment.NewLine + ex.Message, "外部アプリケーション起動エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				return true;
			}
			base.Enabled = true;
			this.Browser.CoreWebView2.Navigate(str);
			return true;
		}

		/*
		// Token: 0x0600000D RID: 13 RVA: 0x000020F1 File Offset: 0x000002F1
		private void Browser_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
		{
			this.Progress.Value = (int)((double)e.CurrentProgress / (double)e.MaximumProgress) * 100;
		}
		*/
		
		// Token: 0x0600000E RID: 14 RVA: 0x00002111 File Offset: 0x00000311
		private void Browser_Navigating(object sender, CoreWebView2NavigationStartingEventArgs e)
		{
			//this.Progress.Visible = true;
			this.URL.Text = e.Uri.ToString();
		}

		/*
		// Token: 0x0600000F RID: 15 RVA: 0x0000213F File Offset: 0x0000033F
		private void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			this.URL.Text = e.Url.ToString();
		}
		*/

		/*
		// Token: 0x06000010 RID: 16 RVA: 0x00002157 File Offset: 0x00000357
		private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			this.Progress.Visible = false;
		}
		*/
		private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
		{
			e.NewWindow = (CoreWebView2)sender;
			//e.Handled= true;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002165 File Offset: 0x00000365
		private void Back_Click(object sender, EventArgs e)
		{
			this.Browser.GoBack();
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002173 File Offset: 0x00000373
		private void Forward_Click(object sender, EventArgs e)
		{
			this.Browser.GoForward();
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002181 File Offset: 0x00000381
		private void Reload_Click(object sender, EventArgs e)
		{
			this.Browser.Reload();
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000218F File Offset: 0x0000038F
		private void ReRun_Click(object sender, EventArgs e)
		{
			if (Global.config.testRun.UseIntegratedBrowser)
			{
				Subsystem.MakeTestrun(0);
				this.Browser.CoreWebView2.Navigate(Subsystem.GetTempFileWhere());
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000425C File Offset: 0x0000245C
		private void OnWeb_Click(object sender, EventArgs e)
		{
			try
			{
				if (Global.config.localSystem.UsingWebBrowser != "" && Global.config.localSystem.UsingWebBrowser != null)
				{
					Global.state.Testrun = Process.Start(Global.config.localSystem.UsingWebBrowser, this.Browser.Source.ToString());
				}
				else
				{
					Global.state.Testrun = Process.Start(this.Browser.Source.ToString());
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("ブラウザを起動できませんでした。" + Environment.NewLine + ex.Message, "外部アプリケーション起動エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000021B8 File Offset: 0x000003B8
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000431C File Offset: 0x0000251C
		private void InitializeComponent()
		{
			this.components = new Container();
			this.MainToolStrip = new ToolStrip();
			this.Back = new ToolStripButton();
			this.Forward = new ToolStripButton();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.Reload = new ToolStripButton();
			this.ReRun = new ToolStripButton();
			this.toolStripSeparator2 = new ToolStripSeparator();
			this.URL = new ToolStripLabel();
			this.OnWeb = new ToolStripButton();
			this.MainStatusStrip = new StatusStrip();
			this.Status = new ToolStripStatusLabel();
			//this.Progress = new ToolStripProgressBar();
			this.Browser = new WebView2();
			//this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			//this.testToolStripMenuItem = new ToolStripMenuItem();
			this.MainToolStrip.SuspendLayout();
			this.MainStatusStrip.SuspendLayout();
			//this.contextMenuStrip1.SuspendLayout();
			base.SuspendLayout();
			this.MainToolStrip.Items.AddRange(new ToolStripItem[]
			{
				this.Back,
				this.Forward,
				this.toolStripSeparator1,
				this.Reload,
				this.ReRun,
				this.toolStripSeparator2,
				this.URL,
				this.OnWeb
			});
			this.MainToolStrip.Location = new Point(0, 0);
			this.MainToolStrip.Name = "MainToolStrip";
			this.MainToolStrip.Size = new Size(414, 25);
			this.MainToolStrip.TabIndex = 0;
			this.MainToolStrip.Text = "toolStrip1";
			this.Back.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.Back.Image = Resources.back;
			this.Back.ImageTransparentColor = Color.Magenta;
			this.Back.Name = "Back";
			this.Back.Size = new Size(23, 22);
			this.Back.Text = "戻る";
			this.Back.Click += this.Back_Click;
			this.Forward.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.Forward.Image = Resources.next;
			this.Forward.ImageTransparentColor = Color.Magenta;
			this.Forward.Name = "Forward";
			this.Forward.Size = new Size(23, 22);
			this.Forward.Text = "進む";
			this.Forward.Click += this.Forward_Click;
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new Size(6, 25);
			this.Reload.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.Reload.Image = Resources.refresh;
			this.Reload.ImageTransparentColor = Color.Magenta;
			this.Reload.Name = "Reload";
			this.Reload.Size = new Size(23, 22);
			this.Reload.Text = "更新";
			this.Reload.Click += this.Reload_Click;
			this.ReRun.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.ReRun.Image = Resources.testrunstage;
			this.ReRun.ImageTransparentColor = Color.Magenta;
			this.ReRun.Name = "ReRun";
			this.ReRun.Size = new Size(23, 22);
			this.ReRun.Text = "再度テストラン";
			this.ReRun.Click += this.ReRun_Click;
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new Size(6, 25);
			this.URL.Name = "URL";
			this.URL.Size = new Size(48, 22);
			this.URL.Text = "No File";
			this.URL.TextAlign = ContentAlignment.MiddleLeft;
			this.OnWeb.Alignment = ToolStripItemAlignment.Right;
			this.OnWeb.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.OnWeb.Image = Resources.web;
			this.OnWeb.ImageTransparentColor = Color.Magenta;
			this.OnWeb.Name = "OnWeb";
			this.OnWeb.Size = new Size(23, 22);
			this.OnWeb.Text = "関連付けられたブラウザで開く";
			this.OnWeb.Click += this.OnWeb_Click;
			this.MainStatusStrip.Items.AddRange(new ToolStripItem[]
			{
				this.Status,
				//this.Progress
			});
			this.MainStatusStrip.Location = new Point(0, 197);
			this.MainStatusStrip.Name = "MainStatusStrip";
			this.MainStatusStrip.Size = new Size(414, 23);
			this.MainStatusStrip.TabIndex = 1;
			this.MainStatusStrip.Text = "statusStrip1";
			this.Status.Name = "Status";
			this.Status.Size = new Size(297, 18);
			this.Status.Spring = true;
			this.Status.Text = "完了";
			this.Status.TextAlign = ContentAlignment.MiddleLeft;
			//this.Progress.Name = "Progress";
			//this.Progress.Size = new Size(100, 17);
			//this.Browser.ContextMenuStrip = this.contextMenuStrip1;
			this.Browser.Dock = DockStyle.Fill;
			this.Browser.Location = new Point(0, 25);
			this.Browser.MinimumSize = new Size(20, 20);
			this.Browser.Name = "Browser";
			this.Browser.Size = new Size(414, 172);
			this.Browser.TabIndex = 2;
			//this.Browser.ProgressChanged += this.Browser_ProgressChanged;
			//this.Browser.DocumentCompleted += this.Browser_DocumentCompleted;
			//this.Browser.Navigated += this.Browser_Navigated;
			/*
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[]
			{
				this.testToolStripMenuItem
			});
			*/
			//this.contextMenuStrip1.Name = "contextMenuStrip1";
			//this.contextMenuStrip1.Size = new Size(102, 26);
			//this.testToolStripMenuItem.Name = "testToolStripMenuItem";
			//this.testToolStripMenuItem.Size = new Size(101, 22);
			//this.testToolStripMenuItem.Text = "Test";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.Browser);
			base.Controls.Add(this.MainStatusStrip);
			base.Controls.Add(this.MainToolStrip);
			base.Name = "IntegratedBrowser";
			base.Size = new Size(414, 220);
			this.MainToolStrip.ResumeLayout(false);
			this.MainToolStrip.PerformLayout();
			this.MainStatusStrip.ResumeLayout(false);
			this.MainStatusStrip.PerformLayout();
			//this.contextMenuStrip1.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
		async void InitializeAsync()
		{
			var webView2Environment = await CoreWebView2Environment.CreateAsync(null, "cache");
			await this.Browser.EnsureCoreWebView2Async(webView2Environment);
			this.Browser.CoreWebView2.NavigationStarting += this.Browser_Navigating;
			this.Browser.CoreWebView2.NewWindowRequested += this.CoreWebView2_NewWindowRequested;
		}

		// Token: 0x04000009 RID: 9
		private IContainer components;

		// Token: 0x0400000A RID: 10
		private ToolStrip MainToolStrip;

		// Token: 0x0400000B RID: 11
		private StatusStrip MainStatusStrip;

		// Token: 0x0400000C RID: 12
		private ToolStripStatusLabel Status;

		// Token: 0x0400000D RID: 13
		private ToolStripButton Back;

		// Token: 0x0400000E RID: 14
		private ToolStripButton Forward;

		// Token: 0x0400000F RID: 15
		private ToolStripSeparator toolStripSeparator1;

		// Token: 0x04000010 RID: 16
		private ToolStripButton Reload;

		// Token: 0x04000011 RID: 17
		private ToolStripButton ReRun;

		// Token: 0x04000012 RID: 18
		private ToolStripSeparator toolStripSeparator2;

		// Token: 0x04000013 RID: 19
		private ToolStripButton OnWeb;

		// Token: 0x04000014 RID: 20
		//private ToolStripProgressBar Progress;

		// Token: 0x04000015 RID: 21
		private WebView2 Browser;

		// Token: 0x04000016 RID: 22
		private ToolStripLabel URL;

		// Token: 0x04000017 RID: 23
		//private ContextMenuStrip contextMenuStrip1;

		// Token: 0x04000018 RID: 24
		//private ToolStripMenuItem testToolStripMenuItem;
	}
}
