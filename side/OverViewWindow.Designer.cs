namespace MasaoPlus
{
	public partial class OverViewWindow : global::System.Windows.Forms.Form
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
			this.OverViewViewer = new global::MasaoPlus.Controls.OverViewer();
			base.SuspendLayout();
			this.OverViewViewer.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.OverViewViewer.Location = new global::System.Drawing.Point(0, 0);
			this.OverViewViewer.Margin = new global::System.Windows.Forms.Padding(0);
			this.OverViewViewer.Name = "OverViewViewer";
			this.OverViewViewer.Size = new global::System.Drawing.Size(180 * DeviceDpi / 96, 30 * DeviceDpi / 96);// マップ幅、マップ高さ
			this.OverViewViewer.TabIndex = 0;
			this.OverViewViewer.MouseMove += new global::System.Windows.Forms.MouseEventHandler(this.OverViewViewer_MouseMove);
			this.OverViewViewer.MouseDown += new global::System.Windows.Forms.MouseEventHandler(this.OverViewViewer_MouseDown);
			//base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			//base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(283 * DeviceDpi / 96, 57 * DeviceDpi / 96);
			base.Controls.Add(this.OverViewViewer);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.KeyPreview = true;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "OverViewWindow";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = global::System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "オーバービュー";
			base.Load += new global::System.EventHandler(this.OverViewWindow_Load);
			base.Activated += new global::System.EventHandler(this.OverViewWindow_Activated);
			base.KeyDown += new global::System.Windows.Forms.KeyEventHandler(this.OverViewWindow_KeyDown);
			base.ResumeLayout(false);
		}

		private global::System.ComponentModel.IContainer components;

		private global::MasaoPlus.Controls.OverViewer OverViewViewer;
	}
}
