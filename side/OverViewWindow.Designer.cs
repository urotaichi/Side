namespace MasaoPlus
{
	// Token: 0x0200002E RID: 46
	public partial class OverViewWindow : global::System.Windows.Forms.Form
	{
		// Token: 0x060001AD RID: 429 RVA: 0x00003173 File Offset: 0x00001373
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00023ED8 File Offset: 0x000220D8
		private void InitializeComponent()
		{
			this.OverViewViewer = new global::MasaoPlus.Controls.OverViewer();
			base.SuspendLayout();
			this.OverViewViewer.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.OverViewViewer.Location = new global::System.Drawing.Point(0, 0);
			this.OverViewViewer.Margin = new global::System.Windows.Forms.Padding(0);
			this.OverViewViewer.Name = "OverViewViewer";
			this.OverViewViewer.Size = new global::System.Drawing.Size(180, 30);
			this.OverViewViewer.TabIndex = 0;
			this.OverViewViewer.MouseMove += new global::System.Windows.Forms.MouseEventHandler(this.OverViewViewer_MouseMove);
			this.OverViewViewer.MouseDown += new global::System.Windows.Forms.MouseEventHandler(this.OverViewViewer_MouseDown);
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(283, 57);
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

		// Token: 0x0400023E RID: 574
		private global::System.ComponentModel.IContainer components;

		// Token: 0x0400023F RID: 575
		private global::MasaoPlus.Controls.OverViewer OverViewViewer;
	}
}
