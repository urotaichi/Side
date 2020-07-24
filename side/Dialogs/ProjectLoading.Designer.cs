namespace MasaoPlus.Dialogs
{
	// Token: 0x02000014 RID: 20
	public partial class ProjectLoading : global::System.Windows.Forms.Form
	{
		// Token: 0x060000B7 RID: 183 RVA: 0x000027AA File Offset: 0x000009AA
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000120BC File Offset: 0x000102BC
		private void InitializeComponent()
		{
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.WaiterText = new global::System.Windows.Forms.Label();
			this.progressBar1 = new global::System.Windows.Forms.ProgressBar();
			base.SuspendLayout();
			this.WaiterText.AutoSize = true;
			this.WaiterText.Font = new global::System.Drawing.Font("MS UI Gothic", 9f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 128);
			this.WaiterText.Location = new global::System.Drawing.Point(12 * DeviceDpi / 96, 9 * DeviceDpi / 96);
			this.WaiterText.Name = "WaiterText";
			this.WaiterText.Size = new global::System.Drawing.Size(155 * DeviceDpi / 96, 12 * DeviceDpi / 96);
			this.WaiterText.TabIndex = 0;
			this.WaiterText.Text = "プロジェクトを読み込んでいます...";
			this.progressBar1.Location = new global::System.Drawing.Point(14 * DeviceDpi / 96, 24 * DeviceDpi / 96);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new global::System.Drawing.Size(336 * DeviceDpi / 96, 21 * DeviceDpi / 96);
			this.progressBar1.Style = global::System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progressBar1.TabIndex = 1;
			//base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			//base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(362 * DeviceDpi / 96, 57 * DeviceDpi / 96);
			base.ControlBox = false;
			base.Controls.Add(this.progressBar1);
			base.Controls.Add(this.WaiterText);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "ProjectLoading";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "お待ちください...";
			base.Shown += new global::System.EventHandler(this.ProjectLoading_Shown);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x040000B7 RID: 183
		private global::System.ComponentModel.IContainer components;

		// Token: 0x040000B8 RID: 184
		private global::System.Windows.Forms.Label WaiterText;

		// Token: 0x040000B9 RID: 185
		private global::System.Windows.Forms.ProgressBar progressBar1;
	}
}
