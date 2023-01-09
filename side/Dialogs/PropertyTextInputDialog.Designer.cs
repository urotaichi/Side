namespace MasaoPlus.Dialogs
{
	public partial class PropertyTextInputDialog : global::System.Windows.Forms.Form
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
			this.InputText = new global::System.Windows.Forms.TextBox();
			this.button1 = new global::System.Windows.Forms.Button();
			this.button2 = new global::System.Windows.Forms.Button();
			base.SuspendLayout();
			this.InputText.AcceptsReturn = true;
			this.InputText.AcceptsTab = true;
			this.InputText.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.InputText.Location = new global::System.Drawing.Point(12, 12);
			this.InputText.Multiline = true;
			this.InputText.Name = "InputText";
			this.InputText.Size = new global::System.Drawing.Size(315, 77);
			this.InputText.TabIndex = 0;
			this.button1.Anchor = (global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Right);
			this.button1.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new global::System.Drawing.Point(155, 95);
			this.button1.Name = "button1";
			this.button1.Size = new global::System.Drawing.Size(83, 27);
			this.button1.TabIndex = 1;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			this.button2.Anchor = (global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Right);
			this.button2.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new global::System.Drawing.Point(244, 95);
			this.button2.Name = "button2";
			this.button2.Size = new global::System.Drawing.Size(83, 27);
			this.button2.TabIndex = 2;
			this.button2.Text = "キャンセル";
			this.button2.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.button2;
			base.ClientSize = new global::System.Drawing.Size(339, 129);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.InputText);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "PropertyTextInputDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "プロパティ テキストの入力";
			base.Shown += new global::System.EventHandler(this.PropertyTextInputDialog_Shown);
			base.FormClosing += new global::System.Windows.Forms.FormClosingEventHandler(this.PropertyTextInputDialog_FormClosing);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.TextBox InputText;

		private global::System.Windows.Forms.Button button1;

		private global::System.Windows.Forms.Button button2;
	}
}
