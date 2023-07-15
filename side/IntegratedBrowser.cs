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
            InitializeComponent();
            InitializeAsync();
        }

        private void Browser_StatusTextChanged(object sender, Object e)
        {
            Status.Text = Browser.CoreWebView2.StatusBarText;
        }

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
                    Enabled = false;
                    Status.Text = "統合ブラウザは利用不可能です。";
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ブラウザを起動できませんでした。" + Environment.NewLine + ex.Message, "外部アプリケーション起動エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                return true;
            }
            Enabled = true;
            Browser.CoreWebView2.Navigate(str);
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
            URL.Text = e.Uri.ToString();
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
            Browser.GoBack();
        }

        private void Forward_Click(object sender, EventArgs e)
        {
            Browser.GoForward();
        }

        private void Reload_Click(object sender, EventArgs e)
        {
            Browser.Reload();
        }

        private void ReRun_Click(object sender, EventArgs e)
        {
            if (Global.config.testRun.UseIntegratedBrowser)
            {
                Subsystem.MakeTestrun(0);
                Browser.CoreWebView2.Navigate(Subsystem.GetTempFileWhere());
            }
        }

        private void OnWeb_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.config.localSystem.UsingWebBrowser != "" && Global.config.localSystem.UsingWebBrowser != null)
                {
                    Global.state.Testrun = Process.Start(Global.config.localSystem.UsingWebBrowser, Browser.Source.ToString());
                }
                else
                {
                    Global.state.Testrun = Process.Start(Browser.Source.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ブラウザを起動できませんでした。" + Environment.NewLine + ex.Message, "外部アプリケーション起動エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            MainToolStrip = new ToolStrip();
            Back = new ToolStripButton();
            Forward = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            Reload = new ToolStripButton();
            ReRun = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            URL = new ToolStripLabel();
            OnWeb = new ToolStripButton();
            MainStatusStrip = new StatusStrip();
            Status = new ToolStripStatusLabel();
            //this.Progress = new ToolStripProgressBar();
            Browser = new WebView2();
            //this.contextMenuStrip1 = new ContextMenuStrip(this.components);
            //this.testToolStripMenuItem = new ToolStripMenuItem();
            MainToolStrip.SuspendLayout();
            MainStatusStrip.SuspendLayout();
            //this.contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            MainToolStrip.Items.AddRange(new ToolStripItem[]
            {
                Back,
                Forward,
                toolStripSeparator1,
                Reload,
                ReRun,
                toolStripSeparator2,
                URL,
                OnWeb
            });
            MainToolStrip.Location = new Point(0, 0);
            MainToolStrip.Name = "MainToolStrip";
            MainToolStrip.Size = new Size(414, 25);
            MainToolStrip.TabIndex = 0;
            MainToolStrip.Text = "toolStrip1";
            Back.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Back.Image = Resources.back;
            Back.ImageTransparentColor = Color.Magenta;
            Back.Name = "Back";
            Back.Size = new Size(23, 22);
            Back.Text = "戻る";
            Back.Click += Back_Click;
            Forward.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Forward.Image = Resources.next;
            Forward.ImageTransparentColor = Color.Magenta;
            Forward.Name = "Forward";
            Forward.Size = new Size(23, 22);
            Forward.Text = "進む";
            Forward.Click += Forward_Click;
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            Reload.DisplayStyle = ToolStripItemDisplayStyle.Image;
            Reload.Image = Resources.refresh;
            Reload.ImageTransparentColor = Color.Magenta;
            Reload.Name = "Reload";
            Reload.Size = new Size(23, 22);
            Reload.Text = "更新";
            Reload.Click += Reload_Click;
            ReRun.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ReRun.Image = Resources.testrunstage;
            ReRun.ImageTransparentColor = Color.Magenta;
            ReRun.Name = "ReRun";
            ReRun.Size = new Size(23, 22);
            ReRun.Text = "再度テストラン";
            ReRun.Click += ReRun_Click;
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            URL.Name = "URL";
            URL.Size = new Size(48, 22);
            URL.Text = "No File";
            URL.TextAlign = ContentAlignment.MiddleLeft;
            OnWeb.Alignment = ToolStripItemAlignment.Right;
            OnWeb.DisplayStyle = ToolStripItemDisplayStyle.Image;
            OnWeb.Image = Resources.web;
            OnWeb.ImageTransparentColor = Color.Magenta;
            OnWeb.Name = "OnWeb";
            OnWeb.Size = new Size(23, 22);
            OnWeb.Text = "関連付けられたブラウザで開く";
            OnWeb.Click += OnWeb_Click;
            MainStatusStrip.Items.AddRange(new ToolStripItem[]
            {
                Status,
				//this.Progress
			});
            MainStatusStrip.Location = new Point(0, 197);
            MainStatusStrip.Name = "MainStatusStrip";
            MainStatusStrip.Size = new Size(414, 23);
            MainStatusStrip.TabIndex = 1;
            MainStatusStrip.Text = "statusStrip1";
            Status.Name = "Status";
            Status.Size = new Size(297, 18);
            Status.Spring = true;
            Status.Text = "完了";
            Status.TextAlign = ContentAlignment.MiddleLeft;
            //this.Progress.Name = "Progress";
            //this.Progress.Size = new Size(100, 17);
            //this.Browser.ContextMenuStrip = this.contextMenuStrip1;
            Browser.Dock = DockStyle.Fill;
            Browser.Location = new Point(0, 25);
            Browser.MinimumSize = new Size(20, 20);
            Browser.Name = "Browser";
            Browser.Size = new Size(414, 172);
            Browser.TabIndex = 2;
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
            AutoScaleDimensions = new SizeF(6f, 12f);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(Browser);
            Controls.Add(MainStatusStrip);
            Controls.Add(MainToolStrip);
            Name = "IntegratedBrowser";
            Size = new Size(414, 220);
            MainToolStrip.ResumeLayout(false);
            MainToolStrip.PerformLayout();
            MainStatusStrip.ResumeLayout(false);
            MainStatusStrip.PerformLayout();
            //this.contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        async void InitializeAsync()
        {
            var webView2Environment = await CoreWebView2Environment.CreateAsync(null, "cache");
            await Browser.EnsureCoreWebView2Async(webView2Environment);
            Browser.CoreWebView2.IsMuted = false;
            Browser.CoreWebView2.StatusBarTextChanged += Browser_StatusTextChanged;
            Browser.CoreWebView2.NavigationStarting += Browser_Navigating;
            Browser.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
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
