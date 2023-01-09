namespace MasaoPlus.Dialogs
{
	public partial class ResetRuntime : global::System.Windows.Forms.Form
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
			this.label1 = new global::System.Windows.Forms.Label();
			this.RuntimeView = new global::System.Windows.Forms.ListView();
			this.RTN = new global::System.Windows.Forms.ColumnHeader();
			this.Author = new global::System.Windows.Forms.ColumnHeader();
			this.ELayer = new global::System.Windows.Forms.ColumnHeader();
			this.Cancel = new global::System.Windows.Forms.Button();
			this.Accept = new global::System.Windows.Forms.Button();
			this.StateLabel = new global::System.Windows.Forms.Label();
			this.groupBox1 = new global::System.Windows.Forms.GroupBox();
			this.label2 = new global::System.Windows.Forms.Label();
			this.groupBox2 = new global::System.Windows.Forms.GroupBox();
			this.NoTouch = new global::System.Windows.Forms.CheckBox();
			this.ChipMethod = new global::System.Windows.Forms.ComboBox();
			this.PutNullChip = new global::System.Windows.Forms.CheckBox();
			this.ChipDefinitionValidation = new global::System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new global::System.Drawing.Point(6, 15);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(326, 48);
			this.label1.TabIndex = 0;
			this.label1.Text = "変更後、移行できない設定値は初期状態へ戻ります。\r\n移行元にあったファイルは上書きされることがあります。\r\nレイヤー未使用から使用へ移行する場合、画像の設定が必要です。\r\nまた、完全にデータを移行できないことがあります。ご注意ください。";
			this.RuntimeView.Columns.AddRange(new global::System.Windows.Forms.ColumnHeader[]
			{
				this.RTN,
				this.Author,
				this.ELayer
			});
			this.RuntimeView.FullRowSelect = true;
			this.RuntimeView.GridLines = true;
			this.RuntimeView.HideSelection = false;
			this.RuntimeView.Location = new global::System.Drawing.Point(12, 107);
			this.RuntimeView.MultiSelect = false;
			this.RuntimeView.Name = "RuntimeView";
			this.RuntimeView.Size = new global::System.Drawing.Size(458, 92);
			this.RuntimeView.TabIndex = 1;
			this.RuntimeView.UseCompatibleStateImageBehavior = false;
			this.RuntimeView.View = global::System.Windows.Forms.View.Details;
			this.RTN.Text = "ランタイム名";
			this.RTN.Width = 250;
			this.Author.Text = "作者";
			this.Author.Width = 100;
			this.ELayer.Text = "レイヤー";
			this.ELayer.Width = 60;
			this.Cancel.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.Cancel.Location = new global::System.Drawing.Point(358, 275);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new global::System.Drawing.Size(112, 28);
			this.Cancel.TabIndex = 2;
			this.Cancel.Text = "キャンセル";
			this.Cancel.UseVisualStyleBackColor = true;
			this.Accept.Location = new global::System.Drawing.Point(240, 275);
			this.Accept.Name = "Accept";
			this.Accept.Size = new global::System.Drawing.Size(112, 28);
			this.Accept.TabIndex = 3;
			this.Accept.Text = "OK";
			this.Accept.UseVisualStyleBackColor = true;
			this.Accept.Click += new global::System.EventHandler(this.Accept_Click);
			this.StateLabel.AutoSize = true;
			this.StateLabel.Location = new global::System.Drawing.Point(18, 283);
			this.StateLabel.Name = "StateLabel";
			this.StateLabel.Size = new global::System.Drawing.Size(0, 12);
			this.StateLabel.TabIndex = 4;
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.ForeColor = global::System.Drawing.Color.Red;
			this.groupBox1.Location = new global::System.Drawing.Point(12, 30);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new global::System.Drawing.Size(457, 71);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "警告";
			this.label2.AutoSize = true;
			this.label2.Location = new global::System.Drawing.Point(12, 12);
			this.label2.Margin = new global::System.Windows.Forms.Padding(3);
			this.label2.Name = "label2";
			this.label2.Size = new global::System.Drawing.Size(406, 12);
			this.label2.TabIndex = 1;
			this.label2.Text = "最新版のランタイム定義への差し替えや、別のランタイム定義への切り替えができます。";
			this.groupBox2.Controls.Add(this.NoTouch);
			this.groupBox2.Controls.Add(this.ChipMethod);
			this.groupBox2.Controls.Add(this.PutNullChip);
			this.groupBox2.Controls.Add(this.ChipDefinitionValidation);
			this.groupBox2.Location = new global::System.Drawing.Point(12, 205);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new global::System.Drawing.Size(456, 64);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "ステージデータ移行モード";
			this.NoTouch.AutoSize = true;
			this.NoTouch.Location = new global::System.Drawing.Point(313, 44);
			this.NoTouch.Name = "NoTouch";
			this.NoTouch.Size = new global::System.Drawing.Size(137, 16);
			this.NoTouch.TabIndex = 5;
			this.NoTouch.Text = "手を加えない(&N)[!危険]";
			this.NoTouch.UseVisualStyleBackColor = true;
			this.NoTouch.CheckedChanged += new global::System.EventHandler(this.NoTouch_CheckedChanged);
			this.ChipMethod.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ChipMethod.FormattingEnabled = true;
			this.ChipMethod.Items.AddRange(new object[]
			{
				"文字で合わせる",
				"マップチップの位置で合わせる",
				"定義リストの順番で合わせる"
			});
			this.ChipMethod.Location = new global::System.Drawing.Point(127, 18);
			this.ChipMethod.Name = "ChipMethod";
			this.ChipMethod.Size = new global::System.Drawing.Size(323, 20);
			this.ChipMethod.TabIndex = 4;
			this.PutNullChip.AutoSize = true;
			this.PutNullChip.Checked = true;
			this.PutNullChip.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.PutNullChip.Enabled = false;
			this.PutNullChip.Location = new global::System.Drawing.Point(8, 44);
			this.PutNullChip.Name = "PutNullChip";
			this.PutNullChip.Size = new global::System.Drawing.Size(291, 16);
			this.PutNullChip.TabIndex = 3;
			this.PutNullChip.Text = "適合しなかったら空白(新チップ定義の1番目)で埋める(&S)";
			this.PutNullChip.UseVisualStyleBackColor = true;
			this.ChipDefinitionValidation.AutoSize = true;
			this.ChipDefinitionValidation.Checked = true;
			this.ChipDefinitionValidation.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.ChipDefinitionValidation.Location = new global::System.Drawing.Point(8, 20);
			this.ChipDefinitionValidation.Name = "ChipDefinitionValidation";
			this.ChipDefinitionValidation.Size = new global::System.Drawing.Size(113, 16);
			this.ChipDefinitionValidation.TabIndex = 2;
			this.ChipDefinitionValidation.Text = "チップ定義照合(&V)";
			this.ChipDefinitionValidation.UseVisualStyleBackColor = true;
			this.ChipDefinitionValidation.CheckedChanged += new global::System.EventHandler(this.ChipDefinitionValidation_CheckedChanged);
			base.AcceptButton = this.Accept;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.Cancel;
			base.ClientSize = new global::System.Drawing.Size(482, 315);
			base.ControlBox = false;
			base.Controls.Add(this.groupBox2);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.StateLabel);
			base.Controls.Add(this.Accept);
			base.Controls.Add(this.Cancel);
			base.Controls.Add(this.RuntimeView);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "ResetRuntime";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ランタイムの再セット";
			base.Shown += new global::System.EventHandler(this.ResetRuntime_Shown);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.Label label1;

		private global::System.Windows.Forms.ListView RuntimeView;

		private global::System.Windows.Forms.Button Cancel;

		private global::System.Windows.Forms.Button Accept;

		private global::System.Windows.Forms.ColumnHeader RTN;

		private global::System.Windows.Forms.ColumnHeader Author;

		private global::System.Windows.Forms.ColumnHeader ELayer;

		private global::System.Windows.Forms.Label StateLabel;

		private global::System.Windows.Forms.GroupBox groupBox1;

		private global::System.Windows.Forms.Label label2;

		private global::System.Windows.Forms.GroupBox groupBox2;

		private global::System.Windows.Forms.CheckBox ChipDefinitionValidation;

		private global::System.Windows.Forms.CheckBox PutNullChip;

		private global::System.Windows.Forms.ComboBox ChipMethod;

		private global::System.Windows.Forms.CheckBox NoTouch;
	}
}
