namespace MasaoPlus.Dialogs
{
	public partial class ProjInheritance : global::System.Windows.Forms.Form
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
			this.label1 = new global::System.Windows.Forms.Label();
			this.NewProjName = new global::System.Windows.Forms.TextBox();
			this.CancelBtn = new global::System.Windows.Forms.Button();
			this.OKBtn = new global::System.Windows.Forms.Button();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new global::System.Drawing.Point(12 * DeviceDpi / 96, 9 * DeviceDpi / 96);
			this.label1.Margin = new global::System.Windows.Forms.Padding(3, 0, 3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(349 * DeviceDpi / 96, 36 * DeviceDpi / 96);
			this.label1.TabIndex = 0;
			this.label1.Text = "新しく作成するプロジェクトの名前を指定してください。\r\nこのプロジェクトファイルは、継承元プロジェクトのフォルダ内に作成されます。\r\nまた、画像などは継承元プロジェクトと同一のものを用います。";
			this.NewProjName.Location = new global::System.Drawing.Point(14 * DeviceDpi / 96, 53 * DeviceDpi / 96);
			this.NewProjName.Name = "NewProjName";
			this.NewProjName.Size = new global::System.Drawing.Size(367 * DeviceDpi / 96, 19 * DeviceDpi / 96);
			this.NewProjName.TabIndex = 1;
			this.NewProjName.TextChanged += new global::System.EventHandler(this.NewProjName_TextChanged);
			this.CancelBtn.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new global::System.Drawing.Point(290 * DeviceDpi / 96, 78 * DeviceDpi / 96);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new global::System.Drawing.Size(89 * DeviceDpi / 96, 25 * DeviceDpi / 96);
			this.CancelBtn.TabIndex = 3;
			this.CancelBtn.Text = "キャンセル";
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.OKBtn.Location = new global::System.Drawing.Point(195 * DeviceDpi / 96, 78 * DeviceDpi / 96);
			this.OKBtn.Name = "OKBtn";
			this.OKBtn.Size = new global::System.Drawing.Size(89 * DeviceDpi / 96, 25 * DeviceDpi / 96);
			this.OKBtn.TabIndex = 2;
			this.OKBtn.Text = "OK";
			this.OKBtn.UseVisualStyleBackColor = true;
			this.OKBtn.Click += new global::System.EventHandler(this.OKBtn_Click);
			base.AcceptButton = this.OKBtn;
			//base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			//base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.CancelBtn;
			base.ClientSize = new global::System.Drawing.Size(391 * DeviceDpi / 96, 115 * DeviceDpi / 96);
			base.ControlBox = false;
			base.Controls.Add(this.OKBtn);
			base.Controls.Add(this.CancelBtn);
			base.Controls.Add(this.NewProjName);
			base.Controls.Add(this.label1);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "ProjInheritance";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "プロジェクトの継承";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.Label label1;

		private global::System.Windows.Forms.Button CancelBtn;

		private global::System.Windows.Forms.Button OKBtn;

		private global::System.Windows.Forms.TextBox NewProjName;
	}
}
