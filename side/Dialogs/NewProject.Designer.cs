namespace MasaoPlus.Dialogs
{
	public partial class NewProject : global::System.Windows.Forms.Form
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
			this.components = new global::System.ComponentModel.Container();
			this.groupBox2 = new global::System.Windows.Forms.GroupBox();
			this.LayerPatternBrowse = new global::System.Windows.Forms.Button();
			this.LayerPattern = new global::System.Windows.Forms.TextBox();
			this.LPLabel = new global::System.Windows.Forms.Label();
			this.label1 = new global::System.Windows.Forms.Label();
			this.GameoverBrowse = new global::System.Windows.Forms.Button();
			this.GameoverImage = new global::System.Windows.Forms.TextBox();
			this.GOLabel = new global::System.Windows.Forms.Label();
			this.EndingImageBrowse = new global::System.Windows.Forms.Button();
			this.EndingImage = new global::System.Windows.Forms.TextBox();
			this.EILabel = new global::System.Windows.Forms.Label();
			this.TitleImageBrowse = new global::System.Windows.Forms.Button();
			this.TitleImage = new global::System.Windows.Forms.TextBox();
			this.TILabel = new global::System.Windows.Forms.Label();
			this.MapChipBrowse = new global::System.Windows.Forms.Button();
			this.MapChip = new global::System.Windows.Forms.TextBox();
			this.MCLabel = new global::System.Windows.Forms.Label();
			this.RuntimeSet = new global::System.Windows.Forms.ComboBox();
			this.RSLabel = new global::System.Windows.Forms.Label();
			this.ProjectName = new global::System.Windows.Forms.TextBox();
			this.PNLabel = new global::System.Windows.Forms.Label();
			this.RootDirBrowse = new global::System.Windows.Forms.Button();
			this.RootDir = new global::System.Windows.Forms.TextBox();
			this.RDLabel = new global::System.Windows.Forms.Label();
			this.Cancel = new global::System.Windows.Forms.Button();
			this.OK = new global::System.Windows.Forms.Button();
			this.HelpTip = new global::System.Windows.Forms.ToolTip(this.components);
			this.StageNum = new global::System.Windows.Forms.NumericUpDown();
			this.label2 = new global::System.Windows.Forms.Label();
			this.LayerUnsupNotice = new global::System.Windows.Forms.Label();
			this.groupBox2.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.StageNum).BeginInit();
			base.SuspendLayout();
			this.groupBox2.Controls.Add(this.LayerUnsupNotice);
			this.groupBox2.Controls.Add(this.LayerPatternBrowse);
			this.groupBox2.Controls.Add(this.LayerPattern);
			this.groupBox2.Controls.Add(this.LPLabel);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.GameoverBrowse);
			this.groupBox2.Controls.Add(this.GameoverImage);
			this.groupBox2.Controls.Add(this.GOLabel);
			this.groupBox2.Controls.Add(this.EndingImageBrowse);
			this.groupBox2.Controls.Add(this.EndingImage);
			this.groupBox2.Controls.Add(this.EILabel);
			this.groupBox2.Controls.Add(this.TitleImageBrowse);
			this.groupBox2.Controls.Add(this.TitleImage);
			this.groupBox2.Controls.Add(this.TILabel);
			this.groupBox2.Controls.Add(this.MapChipBrowse);
			this.groupBox2.Controls.Add(this.MapChip);
			this.groupBox2.Controls.Add(this.MCLabel);
			this.groupBox2.Location = new global::System.Drawing.Point(12, 88);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new global::System.Drawing.Size(515, 159);
			this.groupBox2.TabIndex = 7;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "画像設定(空白でデフォルトの画像を利用します)";
			this.LayerPatternBrowse.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.LayerPatternBrowse.Location = new global::System.Drawing.Point(490, 43);
			this.LayerPatternBrowse.Name = "LayerPatternBrowse";
			this.LayerPatternBrowse.Size = new global::System.Drawing.Size(19, 19);
			this.LayerPatternBrowse.TabIndex = 10;
			this.LayerPatternBrowse.Text = "...";
			this.LayerPatternBrowse.UseVisualStyleBackColor = true;
			this.LayerPatternBrowse.Click += new global::System.EventHandler(this.LayerPatternBrowse_Click);
			this.LayerPattern.BackColor = global::System.Drawing.Color.White;
			this.LayerPattern.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.LayerPattern.ForeColor = global::System.Drawing.Color.Black;
			this.LayerPattern.Location = new global::System.Drawing.Point(110, 43);
			this.LayerPattern.Name = "LayerPattern";
			this.LayerPattern.Size = new global::System.Drawing.Size(374, 19);
			this.LayerPattern.TabIndex = 9;
			this.HelpTip.SetToolTip(this.LayerPattern, "パターン画像として使うgifファイルまたはpngファイルを指定してください。");
			this.LayerPattern.TextChanged += new global::System.EventHandler(this.ValidPathEmptiable);
			this.LPLabel.AutoSize = true;
			this.LPLabel.Location = new global::System.Drawing.Point(6, 45);
			this.LPLabel.Name = "LPLabel";
			this.LPLabel.Size = new global::System.Drawing.Size(92, 12);
			this.LPLabel.TabIndex = 8;
			this.LPLabel.Text = "レイヤーチップ画像";
			this.label1.AutoSize = true;
			this.label1.Location = new global::System.Drawing.Point(237, 143);
			this.label1.Margin = new global::System.Windows.Forms.Padding(3, 3, 3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(272, 12);
			this.label1.TabIndex = 20;
			this.label1.Text = "※指定された画像はプロジェクトフォルダへコピーされます。";
			this.GameoverBrowse.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.GameoverBrowse.Location = new global::System.Drawing.Point(490, 118);
			this.GameoverBrowse.Name = "GameoverBrowse";
			this.GameoverBrowse.Size = new global::System.Drawing.Size(19, 19);
			this.GameoverBrowse.TabIndex = 19;
			this.GameoverBrowse.Text = "...";
			this.GameoverBrowse.UseVisualStyleBackColor = true;
			this.GameoverBrowse.Click += new global::System.EventHandler(this.GameoverBrowse_Click);
			this.GameoverImage.BackColor = global::System.Drawing.Color.White;
			this.GameoverImage.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.GameoverImage.ForeColor = global::System.Drawing.Color.Black;
			this.GameoverImage.Location = new global::System.Drawing.Point(110, 118);
			this.GameoverImage.Name = "GameoverImage";
			this.GameoverImage.Size = new global::System.Drawing.Size(374, 19);
			this.GameoverImage.TabIndex = 18;
			this.HelpTip.SetToolTip(this.GameoverImage, "ゲームオーバー時に表示する画像として使うgif画像またはpng画像を指定してください。");
			this.GameoverImage.TextChanged += new global::System.EventHandler(this.ValidPathEmptiable);
			this.GOLabel.AutoSize = true;
			this.GOLabel.Location = new global::System.Drawing.Point(6, 121);
			this.GOLabel.Name = "GOLabel";
			this.GOLabel.Size = new global::System.Drawing.Size(98, 12);
			this.GOLabel.TabIndex = 17;
			this.GOLabel.Text = "ゲームオーバー画像";
			this.EndingImageBrowse.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.EndingImageBrowse.Location = new global::System.Drawing.Point(490, 92);
			this.EndingImageBrowse.Name = "EndingImageBrowse";
			this.EndingImageBrowse.Size = new global::System.Drawing.Size(19, 19);
			this.EndingImageBrowse.TabIndex = 16;
			this.EndingImageBrowse.Text = "...";
			this.EndingImageBrowse.UseVisualStyleBackColor = true;
			this.EndingImageBrowse.Click += new global::System.EventHandler(this.EndingImageBrowse_Click);
			this.EndingImage.BackColor = global::System.Drawing.Color.White;
			this.EndingImage.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.EndingImage.ForeColor = global::System.Drawing.Color.Black;
			this.EndingImage.Location = new global::System.Drawing.Point(110, 93);
			this.EndingImage.Name = "EndingImage";
			this.EndingImage.Size = new global::System.Drawing.Size(374, 19);
			this.EndingImage.TabIndex = 15;
			this.HelpTip.SetToolTip(this.EndingImage, "クリア後に表示する画像として使うgif画像またはpng画像を指定してください。");
			this.EndingImage.TextChanged += new global::System.EventHandler(this.ValidPathEmptiable);
			this.EILabel.AutoSize = true;
			this.EILabel.Location = new global::System.Drawing.Point(6, 95);
			this.EILabel.Name = "EILabel";
			this.EILabel.Size = new global::System.Drawing.Size(82, 12);
			this.EILabel.TabIndex = 14;
			this.EILabel.Text = "エンディング画像";
			this.TitleImageBrowse.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.TitleImageBrowse.Location = new global::System.Drawing.Point(490, 68);
			this.TitleImageBrowse.Name = "TitleImageBrowse";
			this.TitleImageBrowse.Size = new global::System.Drawing.Size(19, 19);
			this.TitleImageBrowse.TabIndex = 13;
			this.TitleImageBrowse.Text = "...";
			this.TitleImageBrowse.UseVisualStyleBackColor = true;
			this.TitleImageBrowse.Click += new global::System.EventHandler(this.TitleImageBrowse_Click);
			this.TitleImage.BackColor = global::System.Drawing.Color.White;
			this.TitleImage.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.TitleImage.ForeColor = global::System.Drawing.Color.Black;
			this.TitleImage.Location = new global::System.Drawing.Point(110, 68);
			this.TitleImage.Name = "TitleImage";
			this.TitleImage.Size = new global::System.Drawing.Size(374, 19);
			this.TitleImage.TabIndex = 12;
			this.HelpTip.SetToolTip(this.TitleImage, "タイトル画像として使うgif画像またはpng画像を指定してください。");
			this.TitleImage.TextChanged += new global::System.EventHandler(this.ValidPathEmptiable);
			this.TILabel.AutoSize = true;
			this.TILabel.Location = new global::System.Drawing.Point(8, 70);
			this.TILabel.Name = "TILabel";
			this.TILabel.Size = new global::System.Drawing.Size(64, 12);
			this.TILabel.TabIndex = 11;
			this.TILabel.Text = "タイトル画像";
			this.MapChipBrowse.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.MapChipBrowse.Location = new global::System.Drawing.Point(490, 18);
			this.MapChipBrowse.Name = "MapChipBrowse";
			this.MapChipBrowse.Size = new global::System.Drawing.Size(19, 19);
			this.MapChipBrowse.TabIndex = 7;
			this.MapChipBrowse.Text = "...";
			this.MapChipBrowse.UseVisualStyleBackColor = true;
			this.MapChipBrowse.Click += new global::System.EventHandler(this.MapChipBrowse_Click);
			this.MapChip.BackColor = global::System.Drawing.Color.White;
			this.MapChip.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.MapChip.ForeColor = global::System.Drawing.Color.Black;
			this.MapChip.Location = new global::System.Drawing.Point(110, 18);
			this.MapChip.Name = "MapChip";
			this.MapChip.Size = new global::System.Drawing.Size(374, 19);
			this.MapChip.TabIndex = 6;
			this.HelpTip.SetToolTip(this.MapChip, "パターン画像として使うgifファイルまたはpngファイルを指定してください。");
			this.MapChip.TextChanged += new global::System.EventHandler(this.ValidPathEmptiable);
			this.MCLabel.AutoSize = true;
			this.MCLabel.Location = new global::System.Drawing.Point(6, 20);
			this.MCLabel.Name = "MCLabel";
			this.MCLabel.Size = new global::System.Drawing.Size(66, 12);
			this.MCLabel.TabIndex = 5;
			this.MCLabel.Text = "パターン画像";
			this.RuntimeSet.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.RuntimeSet.DropDownWidth = 500;
			this.RuntimeSet.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.RuntimeSet.FormattingEnabled = true;
			this.RuntimeSet.Location = new global::System.Drawing.Point(111, 62);
			this.RuntimeSet.Name = "RuntimeSet";
			this.RuntimeSet.Size = new global::System.Drawing.Size(416, 20);
			this.RuntimeSet.TabIndex = 1;
			this.HelpTip.SetToolTip(this.RuntimeSet, "利用する正男のランタイム定義を指定します。\r\nインストールディレクトリ以下のruntimeフォルダから読み込んでいます。");
			this.RuntimeSet.SelectedIndexChanged += new global::System.EventHandler(this.RuntimeSet_SelectedIndexChanged);
			this.RSLabel.AutoSize = true;
			this.RSLabel.Font = new global::System.Drawing.Font("MS UI Gothic", 9f, global::System.Drawing.FontStyle.Bold, global::System.Drawing.GraphicsUnit.Point, 128);
			this.RSLabel.Location = new global::System.Drawing.Point(12, 65);
			this.RSLabel.Name = "RSLabel";
			this.RSLabel.Size = new global::System.Drawing.Size(82, 12);
			this.RSLabel.TabIndex = 0;
			this.RSLabel.Text = "ランタイムセット";
			this.ProjectName.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.ProjectName.Location = new global::System.Drawing.Point(111, 12);
			this.ProjectName.Name = "ProjectName";
			this.ProjectName.Size = new global::System.Drawing.Size(205, 19);
			this.ProjectName.TabIndex = 1;
			this.ProjectName.Text = "NewProject";
			this.HelpTip.SetToolTip(this.ProjectName, "プロジェクト名を指定します。\r\nここに入力された名前でフォルダとプロジェクトを作ります。");
			this.ProjectName.TextChanged += new global::System.EventHandler(this.ValidText);
			this.PNLabel.AutoSize = true;
			this.PNLabel.Location = new global::System.Drawing.Point(12, 14);
			this.PNLabel.Name = "PNLabel";
			this.PNLabel.Size = new global::System.Drawing.Size(68, 12);
			this.PNLabel.TabIndex = 0;
			this.PNLabel.Text = "プロジェクト名";
			this.RootDirBrowse.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.RootDirBrowse.Location = new global::System.Drawing.Point(508, 37);
			this.RootDirBrowse.Name = "RootDirBrowse";
			this.RootDirBrowse.Size = new global::System.Drawing.Size(19, 19);
			this.RootDirBrowse.TabIndex = 6;
			this.RootDirBrowse.Text = "...";
			this.RootDirBrowse.UseVisualStyleBackColor = true;
			this.RootDirBrowse.Click += new global::System.EventHandler(this.RootDirBrowse_Click);
			this.RootDir.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.RootDir.Location = new global::System.Drawing.Point(111, 37);
			this.RootDir.Name = "RootDir";
			this.RootDir.Size = new global::System.Drawing.Size(391, 19);
			this.RootDir.TabIndex = 5;
			this.HelpTip.SetToolTip(this.RootDir, "プロジェクトフォルダをこのフォルダの直下に作成します。\r\n指定したフォルダはプロジェクトフォルダのルートフォルダとなります。");
			this.RootDir.TextChanged += new global::System.EventHandler(this.ValidDir);
			this.RDLabel.AutoSize = true;
			this.RDLabel.Location = new global::System.Drawing.Point(12, 39);
			this.RDLabel.Name = "RDLabel";
			this.RDLabel.Size = new global::System.Drawing.Size(82, 12);
			this.RDLabel.TabIndex = 4;
			this.RDLabel.Text = "ルートディレクトリ";
			this.Cancel.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.Cancel.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.Cancel.Location = new global::System.Drawing.Point(418, 253);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new global::System.Drawing.Size(109, 23);
			this.Cancel.TabIndex = 10;
			this.Cancel.Text = "キャンセル";
			this.Cancel.UseVisualStyleBackColor = true;
			this.OK.FlatStyle = global::System.Windows.Forms.FlatStyle.Popup;
			this.OK.Location = new global::System.Drawing.Point(303, 253);
			this.OK.Name = "OK";
			this.OK.Size = new global::System.Drawing.Size(109, 23);
			this.OK.TabIndex = 9;
			this.OK.Text = "OK";
			this.OK.UseVisualStyleBackColor = true;
			this.OK.Click += new global::System.EventHandler(this.OK_Click);
			this.HelpTip.ToolTipTitle = "プロジェクト作成のヘルプ";
			this.StageNum.Font = new global::System.Drawing.Font("MS UI Gothic", 9.75f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 128);
			this.StageNum.Location = new global::System.Drawing.Point(485, 11);
			global::System.Windows.Forms.NumericUpDown stageNum = this.StageNum;
			int[] array = new int[4];
			array[0] = 4;
			stageNum.Maximum = new decimal(array);
			global::System.Windows.Forms.NumericUpDown stageNum2 = this.StageNum;
			int[] array2 = new int[4];
			array2[0] = 1;
			stageNum2.Minimum = new decimal(array2);
			this.StageNum.Name = "StageNum";
			this.StageNum.Size = new global::System.Drawing.Size(42, 20);
			this.StageNum.TabIndex = 3;
			this.HelpTip.SetToolTip(this.StageNum, "ステージ数を1-4の範囲内で指定できます。");
			global::System.Windows.Forms.NumericUpDown stageNum3 = this.StageNum;
			int[] array3 = new int[4];
			array3[0] = 1;
			stageNum3.Value = new decimal(array3);
			this.label2.AutoSize = true;
			this.label2.Location = new global::System.Drawing.Point(338, 14);
			this.label2.Name = "label2";
			this.label2.Size = new global::System.Drawing.Size(141, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "ステージ数(後から変更可能)";
			this.LayerUnsupNotice.BackColor = global::System.Drawing.Color.LightGray;
			this.LayerUnsupNotice.ForeColor = global::System.Drawing.Color.Gray;
			this.LayerUnsupNotice.Location = new global::System.Drawing.Point(108, 43);
			this.LayerUnsupNotice.Margin = new global::System.Windows.Forms.Padding(3);
			this.LayerUnsupNotice.Name = "LayerUnsupNotice";
			this.LayerUnsupNotice.Size = new global::System.Drawing.Size(401, 19);
			this.LayerUnsupNotice.TabIndex = 21;
			this.LayerUnsupNotice.Text = "ランタイムがレイヤー編集に対応していません";
			this.LayerUnsupNotice.TextAlign = global::System.Drawing.ContentAlignment.MiddleCenter;
			this.LayerUnsupNotice.Visible = false;
			base.AcceptButton = this.OK;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.Cancel;
			base.ClientSize = new global::System.Drawing.Size(539, 288);
			base.ControlBox = false;
			base.Controls.Add(this.StageNum);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.OK);
			base.Controls.Add(this.Cancel);
			base.Controls.Add(this.groupBox2);
			base.Controls.Add(this.PNLabel);
			base.Controls.Add(this.ProjectName);
			base.Controls.Add(this.RootDir);
			base.Controls.Add(this.RDLabel);
			base.Controls.Add(this.RootDirBrowse);
			base.Controls.Add(this.RuntimeSet);
			base.Controls.Add(this.RSLabel);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "NewProject";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "新しいプロジェクト";
			base.Load += new global::System.EventHandler(this.NewProject_Load);
			base.Shown += new global::System.EventHandler(this.NewProject_Shown);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((global::System.ComponentModel.ISupportInitialize)this.StageNum).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.GroupBox groupBox2;

		private global::System.Windows.Forms.ComboBox RuntimeSet;

		private global::System.Windows.Forms.Label RSLabel;

		private global::System.Windows.Forms.Button MapChipBrowse;

		private global::System.Windows.Forms.TextBox MapChip;

		private global::System.Windows.Forms.Label MCLabel;

		private global::System.Windows.Forms.TextBox ProjectName;

		private global::System.Windows.Forms.Label PNLabel;

		private global::System.Windows.Forms.Button RootDirBrowse;

		private global::System.Windows.Forms.TextBox RootDir;

		private global::System.Windows.Forms.Label RDLabel;

		private global::System.Windows.Forms.Button Cancel;

		private global::System.Windows.Forms.Button OK;

		private global::System.Windows.Forms.Button GameoverBrowse;

		private global::System.Windows.Forms.TextBox GameoverImage;

		private global::System.Windows.Forms.Label GOLabel;

		private global::System.Windows.Forms.Button EndingImageBrowse;

		private global::System.Windows.Forms.TextBox EndingImage;

		private global::System.Windows.Forms.Label EILabel;

		private global::System.Windows.Forms.Button TitleImageBrowse;

		private global::System.Windows.Forms.TextBox TitleImage;

		private global::System.Windows.Forms.Label TILabel;

		private global::System.Windows.Forms.Label label1;

		private global::System.Windows.Forms.ToolTip HelpTip;

		private global::System.Windows.Forms.Button LayerPatternBrowse;

		private global::System.Windows.Forms.TextBox LayerPattern;

		private global::System.Windows.Forms.Label LPLabel;

		private global::System.Windows.Forms.Label label2;

		private global::System.Windows.Forms.NumericUpDown StageNum;

		private global::System.Windows.Forms.Label LayerUnsupNotice;
	}
}
