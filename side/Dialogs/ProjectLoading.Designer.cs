namespace MasaoPlus.Dialogs
{
	public partial class ProjectLoading : global::System.Windows.Forms.Form
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
			this.WaiterText = new global::System.Windows.Forms.Label();
			this.progressBar1 = new global::System.Windows.Forms.ProgressBar();
			base.SuspendLayout();
			this.WaiterText.AutoSize = true;
			this.WaiterText.Font = new global::System.Drawing.Font("MS UI Gothic", 9f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 128);
			this.WaiterText.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(12), base.LogicalToDeviceUnits(9));
			this.WaiterText.Name = "WaiterText";
			this.WaiterText.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(155, 12));
			this.WaiterText.TabIndex = 0;
			this.WaiterText.Text = "プロジェクトを読み込んでいます...";
			this.progressBar1.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(14), base.LogicalToDeviceUnits(24));
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(336, 21));
			this.progressBar1.Style = global::System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progressBar1.TabIndex = 1;
			//base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			//base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = base.LogicalToDeviceUnits(new global::System.Drawing.Size(362, 57));
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

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.Label WaiterText;

		private global::System.Windows.Forms.ProgressBar progressBar1;
	}
}
