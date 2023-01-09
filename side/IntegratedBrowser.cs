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
	public class IntegratedBrowser : UserControl
	{
		public IntegratedBrowser()
		{
			this.InitializeComponent();
			//this.Browser.StatusTextChanged += this.Browser_StatusTextChanged;
			this.InitializeAsync();
		}

		/*
		private void Browser_StatusTextChanged(object sender, EventArgs e)
		{
			this.Status.Text = this.Browser.StatusText;
		}
		*/

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
		private void Browser_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
		{
			this.Progress.Value = (int)((double)e.CurrentProgress / (double)e.MaximumProgress) * 100;
		}
		*/
		
		private void Browser_Navigating(object sender, CoreWebView2NavigationStartingEventArgs e)
		{
			//this.Progress.Visible = true;
			this.URL.Text = e.Uri.ToString();
		}

		/*
		private void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			this.URL.Text = e.Url.ToString();
		}
		*/

		/*
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

		private void Back_Click(object sender, EventArgs e)
		{
			this.Browser.GoBack();
		}

		private void Forward_Click(object sender, EventArgs e)
		{
			this.Browser.GoForward();
		}

		private void Reload_Click(object sender, EventArgs e)
		{
			this.Browser.Reload();
		}

		private void ReRun_Click(object sender, EventArgs e)
		{
			if (Global.config.testRun.UseIntegratedBrowser)
			{
				Subsystem.MakeTestrun(0);
				this.Browser.CoreWebView2.Navigate(Subsystem.GetTempFileWhere());
			}
		}

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

		private IContainer components;

		private ToolStrip MainToolStrip;

		private StatusStrip MainStatusStrip;

		private ToolStripStatusLabel Status;

		private ToolStripButton Back;

		private ToolStripButton Forward;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripButton Reload;

		private ToolStripButton ReRun;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripButton OnWeb;

		//private ToolStripProgressBar Progress;

		private WebView2 Browser;

		private ToolStripLabel URL;

		//private ContextMenuStrip contextMenuStrip1;

		//private ToolStripMenuItem testToolStripMenuItem;
	}
}
