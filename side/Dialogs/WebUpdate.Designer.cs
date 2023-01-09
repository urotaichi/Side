namespace MasaoPlus.Dialogs
{
	public partial class WebUpdate : global::System.Windows.Forms.Form
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
			this.StateLabel = new global::System.Windows.Forms.Label();
			this.progressBar1 = new global::System.Windows.Forms.ProgressBar();
			base.SuspendLayout();
			this.StateLabel.AutoSize = true;
			this.StateLabel.Location = new global::System.Drawing.Point(12, 9);
			this.StateLabel.Name = "StateLabel";
			this.StateLabel.Size = new global::System.Drawing.Size(47, 12);
			this.StateLabel.TabIndex = 0;
			this.StateLabel.Text = "準備中...";
			this.progressBar1.Location = new global::System.Drawing.Point(14, 24);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new global::System.Drawing.Size(281, 21);
			this.progressBar1.TabIndex = 1;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(307, 57);
			base.ControlBox = false;
			base.Controls.Add(this.progressBar1);
			base.Controls.Add(this.StateLabel);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "WebUpdate";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Sideの更新";
			base.Shown += new global::System.EventHandler(this.WebUpdate_Shown);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.Label StateLabel;

		private global::System.Windows.Forms.ProgressBar progressBar1;
	}
}
