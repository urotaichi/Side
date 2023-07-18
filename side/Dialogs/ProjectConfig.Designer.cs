namespace MasaoPlus.Dialogs
{
	public partial class ProjectConfig : global::System.Windows.Forms.Form
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
			this.Cancel = new global::System.Windows.Forms.Button();
			this.Accept = new global::System.Windows.Forms.Button();
			this.MainTab = new global::System.Windows.Forms.TabControl();
			this.A = new global::System.Windows.Forms.TabPage();
            this.UseWorldmap = new global::System.Windows.Forms.CheckBox();
            if (!Global.cpd.runtime.Definitions.Package.Contains("28")) this.Use3rdMapData = new global::System.Windows.Forms.CheckBox();
			this.label17 = new global::System.Windows.Forms.Label();
			this.label15 = new global::System.Windows.Forms.Label();
			this.ProjNum = new global::System.Windows.Forms.NumericUpDown();
			this.ProjectName = new global::System.Windows.Forms.TextBox();
			this.label8 = new global::System.Windows.Forms.Label();
			this.OutExt = new global::System.Windows.Forms.TextBox();
			this.label2 = new global::System.Windows.Forms.Label();
			this.OutDir = new global::System.Windows.Forms.TextBox();
			this.label1 = new global::System.Windows.Forms.Label();
			this.Fom = new global::System.Windows.Forms.TabPage();
			this.label16 = new global::System.Windows.Forms.Label();
			this.label14 = new global::System.Windows.Forms.Label();
			this.label13 = new global::System.Windows.Forms.Label();
			this.label12 = new global::System.Windows.Forms.Label();
			this.label11 = new global::System.Windows.Forms.Label();
			this.label10 = new global::System.Windows.Forms.Label();
			this.label7 = new global::System.Windows.Forms.Label();
			this.label6 = new global::System.Windows.Forms.Label();
			this.MapF = new global::System.Windows.Forms.TextBox();
			this.LayerF4 = new global::System.Windows.Forms.TextBox();
			this.StageF4 = new global::System.Windows.Forms.TextBox();
			this.LayerF3 = new global::System.Windows.Forms.TextBox();
			this.StageF3 = new global::System.Windows.Forms.TextBox();
			this.LayerF2 = new global::System.Windows.Forms.TextBox();
			this.StageF2 = new global::System.Windows.Forms.TextBox();
			this.label5 = new global::System.Windows.Forms.Label();
			this.LayerF = new global::System.Windows.Forms.TextBox();
			this.label4 = new global::System.Windows.Forms.Label();
			this.label3 = new global::System.Windows.Forms.Label();
			this.StageF = new global::System.Windows.Forms.TextBox();

			this.OH = new global::System.Windows.Forms.TabPage();
			this.panel1 = new global::System.Windows.Forms.Panel();
			this.OutHeader = new global::System.Windows.Forms.RichTextBox();

			this.EditContext = new global::System.Windows.Forms.ContextMenuStrip(this.components);
			this.TextUndo = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new global::System.Windows.Forms.ToolStripSeparator();
			this.TextCut = new global::System.Windows.Forms.ToolStripMenuItem();
			this.TextCopy = new global::System.Windows.Forms.ToolStripMenuItem();
			this.TextPaste = new global::System.Windows.Forms.ToolStripMenuItem();

			this.OF = new global::System.Windows.Forms.TabPage();
			this.panel2 = new global::System.Windows.Forms.Panel();
			this.OutFooter = new global::System.Windows.Forms.RichTextBox();

			this.OM = new global::System.Windows.Forms.TabPage();
			this.panel3 = new global::System.Windows.Forms.Panel();
			this.OutMiddle = new global::System.Windows.Forms.RichTextBox();

			this.OutputReplace = new global::System.Windows.Forms.TabPage();
			this.OutputReplaceView = new global::System.Windows.Forms.DataGridView();
			this.ReplName = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ReplValue = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.label9 = new global::System.Windows.Forms.Label();
			this.dataGridViewTextBoxColumn1 = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn2 = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MainTab.SuspendLayout();
			this.A.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.ProjNum).BeginInit();
			this.Fom.SuspendLayout();

			this.OH.SuspendLayout();
			this.panel1.SuspendLayout();

			this.EditContext.SuspendLayout();

			this.OF.SuspendLayout();
			this.panel2.SuspendLayout();

			this.OM.SuspendLayout();
			this.panel3.SuspendLayout();

			this.OutputReplace.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.OutputReplaceView).BeginInit();
			base.SuspendLayout();
			this.Cancel.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.Cancel.Location = new global::System.Drawing.Point(425, 310);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new global::System.Drawing.Size(109, 29);
			this.Cancel.TabIndex = 2;
			this.Cancel.Text = "キャンセル";
			this.Cancel.UseVisualStyleBackColor = true;
			this.Accept.Location = new global::System.Drawing.Point(310, 310);
			this.Accept.Name = "Accept";
			this.Accept.Size = new global::System.Drawing.Size(109, 29);
			this.Accept.TabIndex = 1;
			this.Accept.Text = "OK";
			this.Accept.UseVisualStyleBackColor = true;
			this.Accept.Click += new global::System.EventHandler(this.Accept_Click);
			this.MainTab.Controls.Add(this.A);
			this.MainTab.Controls.Add(this.Fom);
			this.MainTab.Controls.Add(this.OH);
			this.MainTab.Controls.Add(this.OM);
			this.MainTab.Controls.Add(this.OF);
			this.MainTab.Controls.Add(this.OutputReplace);
			this.MainTab.Location = new global::System.Drawing.Point(12, 12);
			this.MainTab.Name = "MainTab";
			this.MainTab.SelectedIndex = 0;
			this.MainTab.Size = new global::System.Drawing.Size(522, 292);
			this.MainTab.TabIndex = 0;
			this.A.Controls.Add(this.UseWorldmap);
            this.A.Controls.Add(this.Use3rdMapData);
            this.A.Controls.Add(this.label17);
			this.A.Controls.Add(this.label15);
			this.A.Controls.Add(this.ProjNum);
			this.A.Controls.Add(this.ProjectName);
			this.A.Controls.Add(this.label8);
			this.A.Controls.Add(this.OutExt);
			this.A.Controls.Add(this.label2);
			this.A.Controls.Add(this.OutDir);
			this.A.Controls.Add(this.label1);
			this.A.Location = new global::System.Drawing.Point(4, 22);
			this.A.Name = "A";
			this.A.Padding = new global::System.Windows.Forms.Padding(3);
			this.A.Size = new global::System.Drawing.Size(514, 266);
			this.A.TabIndex = 0;
			this.A.Text = "全般";
			this.A.UseVisualStyleBackColor = true;
			this.UseWorldmap.AutoSize = true;
			this.UseWorldmap.Location = new global::System.Drawing.Point(166, 100);
			this.UseWorldmap.Name = "UseWorldmap";
			this.UseWorldmap.Size = new global::System.Drawing.Size(198, 16);
			this.UseWorldmap.TabIndex = 10;
			this.UseWorldmap.Text = "マップを利用してステージ選択する(&M)";
			this.UseWorldmap.UseVisualStyleBackColor = true;
			if (!Global.cpd.runtime.Definitions.Package.Contains("28"))
            {
                this.Use3rdMapData.AutoSize = true;
                this.Use3rdMapData.Location = new global::System.Drawing.Point(6, 125);
                this.Use3rdMapData.Name = "Use3rdMapData";
                this.Use3rdMapData.Size = new global::System.Drawing.Size(198, 16);
                this.Use3rdMapData.TabIndex = 10;
                this.Use3rdMapData.Text = "第3版マップデータを有効にする(FX版のみ)(&L)";
                this.Use3rdMapData.UseVisualStyleBackColor = true;
            }
            this.label17.AutoSize = true;
			this.label17.Location = new global::System.Drawing.Point(88, 53);
			this.label17.Name = "label17";
			this.label17.Size = new global::System.Drawing.Size(92, 12);
			this.label17.TabIndex = 9;
			this.label17.Text = "相対パス、空白可";
			this.label15.AutoSize = true;
			this.label15.Location = new global::System.Drawing.Point(29, 101);
			this.label15.Name = "label15";
			this.label15.Size = new global::System.Drawing.Size(55, 12);
			this.label15.TabIndex = 7;
			this.label15.Text = "ステージ数";
			this.ProjNum.Font = new global::System.Drawing.Font("MS UI Gothic", 9.75f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 128);
			this.ProjNum.Location = new global::System.Drawing.Point(90, 98);
			global::System.Windows.Forms.NumericUpDown projNum = this.ProjNum;
			int[] array = new int[4];
			array[0] = 4;
			projNum.Maximum = new decimal(array);
			global::System.Windows.Forms.NumericUpDown projNum2 = this.ProjNum;
			int[] array2 = new int[4];
			array2[0] = 1;
			projNum2.Minimum = new decimal(array2);
			this.ProjNum.Name = "ProjNum";
			this.ProjNum.Size = new global::System.Drawing.Size(70, 20);
			this.ProjNum.TabIndex = 6;
			global::System.Windows.Forms.NumericUpDown projNum3 = this.ProjNum;
			int[] array3 = new int[4];
			array3[0] = 1;
			projNum3.Value = new decimal(array3);
			this.ProjectName.Location = new global::System.Drawing.Point(90, 6);
			this.ProjectName.Name = "ProjectName";
			this.ProjectName.Size = new global::System.Drawing.Size(418, 19);
			this.ProjectName.TabIndex = 1;
			this.label8.AutoSize = true;
			this.label8.Location = new global::System.Drawing.Point(16, 9);
			this.label8.Name = "label8";
			this.label8.Size = new global::System.Drawing.Size(68, 12);
			this.label8.TabIndex = 0;
			this.label8.Text = "プロジェクト名";
			this.OutExt.Location = new global::System.Drawing.Point(90, 73);
			this.OutExt.Name = "OutExt";
			this.OutExt.Size = new global::System.Drawing.Size(100, 19);
			this.OutExt.TabIndex = 5;
			this.label2.AutoSize = true;
			this.label2.Location = new global::System.Drawing.Point(16, 76);
			this.label2.Name = "label2";
			this.label2.Size = new global::System.Drawing.Size(65, 12);
			this.label2.TabIndex = 4;
			this.label2.Text = "出力拡張子";
			this.OutDir.Location = new global::System.Drawing.Point(90, 31);
			this.OutDir.Name = "OutDir";
			this.OutDir.Size = new global::System.Drawing.Size(418, 19);
			this.OutDir.TabIndex = 3;
			this.label1.AutoSize = true;
			this.label1.Location = new global::System.Drawing.Point(6, 34);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(78, 12);
			this.label1.TabIndex = 2;
			this.label1.Text = "出力ディレクトリ";
			this.label1.TextAlign = global::System.Drawing.ContentAlignment.TopRight;
			this.Fom.Controls.Add(this.label16);
			this.Fom.Controls.Add(this.label14);
			this.Fom.Controls.Add(this.label13);
			this.Fom.Controls.Add(this.label12);
			this.Fom.Controls.Add(this.label11);
			this.Fom.Controls.Add(this.label10);
			this.Fom.Controls.Add(this.label7);
			this.Fom.Controls.Add(this.label6);
			this.Fom.Controls.Add(this.MapF);
			this.Fom.Controls.Add(this.LayerF4);
			this.Fom.Controls.Add(this.StageF4);
			this.Fom.Controls.Add(this.LayerF3);
			this.Fom.Controls.Add(this.StageF3);
			this.Fom.Controls.Add(this.LayerF2);
			this.Fom.Controls.Add(this.StageF2);
			this.Fom.Controls.Add(this.label5);
			this.Fom.Controls.Add(this.LayerF);
			this.Fom.Controls.Add(this.label4);
			this.Fom.Controls.Add(this.label3);
			this.Fom.Controls.Add(this.StageF);
			this.Fom.Location = new global::System.Drawing.Point(4, 22);
			this.Fom.Name = "Fom";
			this.Fom.Size = new global::System.Drawing.Size(514, 266);
			this.Fom.TabIndex = 4;
			this.Fom.Text = "フォーマット";
			this.Fom.UseVisualStyleBackColor = true;
			this.label16.AutoSize = true;
			this.label16.Location = new global::System.Drawing.Point(190, 9);
			this.label16.Name = "label16";
			this.label16.Size = new global::System.Drawing.Size(321, 12);
			this.label16.TabIndex = 19;
			this.label16.Text = "{0}に横分割データ、{1}に行数、{2}にマップ本体データが代入されます";
			this.label14.AutoSize = true;
			this.label14.Location = new global::System.Drawing.Point(6, 202);
			this.label14.Name = "label14";
			this.label14.Size = new global::System.Drawing.Size(49, 12);
			this.label14.TabIndex = 18;
			this.label14.Text = "レイヤー4";
			this.label13.AutoSize = true;
			this.label13.Location = new global::System.Drawing.Point(6, 152);
			this.label13.Name = "label13";
			this.label13.Size = new global::System.Drawing.Size(49, 12);
			this.label13.TabIndex = 17;
			this.label13.Text = "レイヤー3";
			this.label12.AutoSize = true;
			this.label12.Location = new global::System.Drawing.Point(6, 102);
			this.label12.Name = "label12";
			this.label12.Size = new global::System.Drawing.Size(49, 12);
			this.label12.TabIndex = 16;
			this.label12.Text = "レイヤー2";
			this.label11.AutoSize = true;
			this.label11.Location = new global::System.Drawing.Point(26, 239);
			this.label11.Name = "label11";
			this.label11.Size = new global::System.Drawing.Size(29, 12);
			this.label11.TabIndex = 15;
			this.label11.Text = "地図";
			this.label10.AutoSize = true;
			this.label10.Location = new global::System.Drawing.Point(6, 177);
			this.label10.Name = "label10";
			this.label10.Size = new global::System.Drawing.Size(49, 12);
			this.label10.TabIndex = 14;
			this.label10.Text = "ステージ4";
			this.label7.AutoSize = true;
			this.label7.Location = new global::System.Drawing.Point(6, 127);
			this.label7.Name = "label7";
			this.label7.Size = new global::System.Drawing.Size(49, 12);
			this.label7.TabIndex = 13;
			this.label7.Text = "ステージ3";
			this.label6.AutoSize = true;
			this.label6.Location = new global::System.Drawing.Point(6, 77);
			this.label6.Name = "label6";
			this.label6.Size = new global::System.Drawing.Size(49, 12);
			this.label6.TabIndex = 12;
			this.label6.Text = "ステージ2";
			this.MapF.Location = new global::System.Drawing.Point(61, 236);
			this.MapF.Name = "MapF";
			this.MapF.Size = new global::System.Drawing.Size(450, 19);
			this.MapF.TabIndex = 11;
			this.LayerF4.Location = new global::System.Drawing.Point(61, 199);
			this.LayerF4.Name = "LayerF4";
			this.LayerF4.Size = new global::System.Drawing.Size(450, 19);
			this.LayerF4.TabIndex = 10;
			this.StageF4.Location = new global::System.Drawing.Point(61, 174);
			this.StageF4.Name = "StageF4";
			this.StageF4.Size = new global::System.Drawing.Size(450, 19);
			this.StageF4.TabIndex = 9;
			this.LayerF3.Location = new global::System.Drawing.Point(61, 149);
			this.LayerF3.Name = "LayerF3";
			this.LayerF3.Size = new global::System.Drawing.Size(450, 19);
			this.LayerF3.TabIndex = 8;
			this.StageF3.Location = new global::System.Drawing.Point(61, 124);
			this.StageF3.Name = "StageF3";
			this.StageF3.Size = new global::System.Drawing.Size(450, 19);
			this.StageF3.TabIndex = 7;
			this.LayerF2.Location = new global::System.Drawing.Point(61, 99);
			this.LayerF2.Name = "LayerF2";
			this.LayerF2.Size = new global::System.Drawing.Size(450, 19);
			this.LayerF2.TabIndex = 6;
			this.StageF2.Location = new global::System.Drawing.Point(61, 74);
			this.StageF2.Name = "StageF2";
			this.StageF2.Size = new global::System.Drawing.Size(450, 19);
			this.StageF2.TabIndex = 5;
			this.label5.AutoSize = true;
			this.label5.Location = new global::System.Drawing.Point(12, 52);
			this.label5.Name = "label5";
			this.label5.Size = new global::System.Drawing.Size(43, 12);
			this.label5.TabIndex = 3;
			this.label5.Text = "レイヤー";
			this.LayerF.Location = new global::System.Drawing.Point(61, 49);
			this.LayerF.Name = "LayerF";
			this.LayerF.Size = new global::System.Drawing.Size(450, 19);
			this.LayerF.TabIndex = 4;
			this.label4.AutoSize = true;
			this.label4.Location = new global::System.Drawing.Point(12, 27);
			this.label4.Name = "label4";
			this.label4.Size = new global::System.Drawing.Size(43, 12);
			this.label4.TabIndex = 1;
			this.label4.Text = "ステージ";
			this.label3.AutoSize = true;
			this.label3.Location = new global::System.Drawing.Point(283, 221);
			this.label3.Name = "label3";
			this.label3.Size = new global::System.Drawing.Size(228, 12);
			this.label3.TabIndex = 0;
			this.label3.Text = "{0}に行数、{1}にマップ本体データが代入されます";
			this.StageF.Location = new global::System.Drawing.Point(61, 24);
			this.StageF.Name = "StageF";
			this.StageF.Size = new global::System.Drawing.Size(450, 19);
			this.StageF.TabIndex = 2;

			this.OH.Controls.Add(this.panel1);
			this.OH.Location = new global::System.Drawing.Point(4, 22);
			this.OH.Name = "OH";
			this.OH.Padding = new global::System.Windows.Forms.Padding(3);
			this.OH.Size = new global::System.Drawing.Size(514, 266);
			this.OH.TabIndex = 1;
			this.OH.Text = "出力ヘッダ";
			this.OH.UseVisualStyleBackColor = true;
			this.panel1.BackColor = global::System.Drawing.Color.CornflowerBlue;
			this.panel1.Controls.Add(this.OutHeader);
			this.panel1.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new global::System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new global::System.Windows.Forms.Padding(1);
			this.panel1.Size = new global::System.Drawing.Size(508, 260);
			this.panel1.TabIndex = 0;
			this.OutHeader.AcceptsTab = true;
			this.OutHeader.BorderStyle = global::System.Windows.Forms.BorderStyle.None;
			this.OutHeader.ContextMenuStrip = this.EditContext;
			this.OutHeader.DetectUrls = false;
			this.OutHeader.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.OutHeader.Font = new global::System.Drawing.Font("ＭＳ ゴシック", 9f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 128);
			this.OutHeader.Location = new global::System.Drawing.Point(1, 1);
			this.OutHeader.Name = "OutHeader";
			this.OutHeader.Size = new global::System.Drawing.Size(506, 258);
			this.OutHeader.TabIndex = 0;
			this.OutHeader.Text = "";

			this.EditContext.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.TextUndo,
				this.toolStripMenuItem1,
				this.TextCut,
				this.TextCopy,
				this.TextPaste
			});
			this.EditContext.Name = "EditContext";
			this.EditContext.Size = new global::System.Drawing.Size(191, 98);
			this.TextUndo.Image = global::MasaoPlus.Properties.Resources.undo;
			this.TextUndo.Name = "TextUndo";
			this.TextUndo.ShortcutKeyDisplayString = "Ctrl+Z";
			this.TextUndo.Size = new global::System.Drawing.Size(190, 22);
			this.TextUndo.Text = "元に戻す(&U)";
			this.TextUndo.Click += new global::System.EventHandler(this.TextUndo_Click);
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new global::System.Drawing.Size(187, 6);
			this.TextCut.Image = global::MasaoPlus.Properties.Resources.cut;
			this.TextCut.Name = "TextCut";
			this.TextCut.ShortcutKeyDisplayString = "Ctrl+X";
			this.TextCut.Size = new global::System.Drawing.Size(190, 22);
			this.TextCut.Text = "切り取り(&X)";
			this.TextCut.Click += new global::System.EventHandler(this.TextCut_Click);
			this.TextCopy.Image = global::MasaoPlus.Properties.Resources.copy;
			this.TextCopy.Name = "TextCopy";
			this.TextCopy.ShortcutKeyDisplayString = "Ctrl+C";
			this.TextCopy.Size = new global::System.Drawing.Size(190, 22);
			this.TextCopy.Text = "コピー(&C)";
			this.TextCopy.Click += new global::System.EventHandler(this.TextCopy_Click);
			this.TextPaste.Image = global::MasaoPlus.Properties.Resources.paste;
			this.TextPaste.Name = "TextPaste";
			this.TextPaste.ShortcutKeyDisplayString = "Ctrl+V";
			this.TextPaste.Size = new global::System.Drawing.Size(190, 22);
			this.TextPaste.Text = "貼り付け(&V)";
			this.TextPaste.Click += new global::System.EventHandler(this.TextPaste_Click);

			this.OM.Controls.Add(this.panel3);
			this.OM.Location = new global::System.Drawing.Point(4, 22);
			this.OM.Name = "OM";
			this.OM.Padding = new global::System.Windows.Forms.Padding(3);
			this.OM.Size = new global::System.Drawing.Size(514, 266);
			this.OM.TabIndex = 2;
			this.OM.Text = "出力中間";
			this.OM.UseVisualStyleBackColor = true;
			this.panel3.BackColor = global::System.Drawing.Color.CornflowerBlue;
			this.panel3.Controls.Add(this.OutMiddle);
			this.panel3.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new global::System.Drawing.Point(3, 3);
			this.panel3.Name = "panel3";
			this.panel3.Padding = new global::System.Windows.Forms.Padding(1);
			this.panel3.Size = new global::System.Drawing.Size(508, 260);
			this.panel3.TabIndex = 1;
			this.OutMiddle.AcceptsTab = true;
			this.OutMiddle.BorderStyle = global::System.Windows.Forms.BorderStyle.None;
			this.OutMiddle.ContextMenuStrip = this.EditContext;
			this.OutMiddle.DetectUrls = false;
			this.OutMiddle.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.OutMiddle.Font = new global::System.Drawing.Font("ＭＳ ゴシック", 9f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 128);
			this.OutMiddle.Location = new global::System.Drawing.Point(1, 1);
			this.OutMiddle.Name = "OutMiddle";
			this.OutMiddle.Size = new global::System.Drawing.Size(506, 258);
			this.OutMiddle.TabIndex = 0;
			this.OutMiddle.Text = "";

			this.OF.Controls.Add(this.panel2);
			this.OF.Location = new global::System.Drawing.Point(4, 22);
			this.OF.Name = "OF";
			this.OF.Padding = new global::System.Windows.Forms.Padding(3);
			this.OF.Size = new global::System.Drawing.Size(514, 266);
			this.OF.TabIndex = 3;
			this.OF.Text = "出力フッタ";
			this.OF.UseVisualStyleBackColor = true;
			this.panel2.BackColor = global::System.Drawing.Color.CornflowerBlue;
			this.panel2.Controls.Add(this.OutFooter);
			this.panel2.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new global::System.Drawing.Point(3, 3);
			this.panel2.Name = "panel2";
			this.panel2.Padding = new global::System.Windows.Forms.Padding(1);
			this.panel2.Size = new global::System.Drawing.Size(508, 260);
			this.panel2.TabIndex = 2;
			this.OutFooter.AcceptsTab = true;
			this.OutFooter.BorderStyle = global::System.Windows.Forms.BorderStyle.None;
			this.OutFooter.ContextMenuStrip = this.EditContext;
			this.OutFooter.DetectUrls = false;
			this.OutFooter.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.OutFooter.Font = new global::System.Drawing.Font("ＭＳ ゴシック", 9f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 128);
			this.OutFooter.Location = new global::System.Drawing.Point(1, 1);
			this.OutFooter.Name = "OutFooter";
			this.OutFooter.Size = new global::System.Drawing.Size(506, 258);
			this.OutFooter.TabIndex = 0;
			this.OutFooter.Text = "";

			this.OutputReplace.Controls.Add(this.OutputReplaceView);
			this.OutputReplace.Controls.Add(this.label9);
			this.OutputReplace.Location = new global::System.Drawing.Point(4, 22);
			this.OutputReplace.Name = "OutputReplace";
			this.OutputReplace.Padding = new global::System.Windows.Forms.Padding(3);
			this.OutputReplace.Size = new global::System.Drawing.Size(514, 266);
			this.OutputReplace.TabIndex = 3;
			this.OutputReplace.Text = "出力リプレース";
			this.OutputReplace.UseVisualStyleBackColor = true;
			this.OutputReplaceView.AllowUserToResizeRows = false;
			this.OutputReplaceView.AutoSizeColumnsMode = global::System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.OutputReplaceView.ColumnHeadersHeightSizeMode = global::System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.OutputReplaceView.Columns.AddRange(new global::System.Windows.Forms.DataGridViewColumn[]
			{
				this.ReplName,
				this.ReplValue
			});
			this.OutputReplaceView.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.OutputReplaceView.Location = new global::System.Drawing.Point(3, 42);
			this.OutputReplaceView.MultiSelect = false;
			this.OutputReplaceView.Name = "OutputReplaceView";
			this.OutputReplaceView.RowHeadersWidthSizeMode = global::System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
			this.OutputReplaceView.RowTemplate.Height = 21;
			this.OutputReplaceView.SelectionMode = global::System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.OutputReplaceView.Size = new global::System.Drawing.Size(508, 221);
			this.OutputReplaceView.TabIndex = 0;
			this.ReplName.AutoSizeMode = global::System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.ReplName.HeaderText = "項目名";
			this.ReplName.Name = "ReplName";
			this.ReplName.SortMode = global::System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.ReplValue.AutoSizeMode = global::System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.ReplValue.FillWeight = 160f;
			this.ReplValue.HeaderText = "置き換え値";
			this.ReplValue.Name = "ReplValue";
			this.ReplValue.SortMode = global::System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.label9.AutoSize = true;
			this.label9.Dock = global::System.Windows.Forms.DockStyle.Top;
			this.label9.Location = new global::System.Drawing.Point(3, 3);
			this.label9.Margin = new global::System.Windows.Forms.Padding(3);
			this.label9.Name = "label9";
			this.label9.Padding = new global::System.Windows.Forms.Padding(0, 0, 0, 3);
			this.label9.Size = new global::System.Drawing.Size(423, 39);
			this.label9.TabIndex = 1;
			this.label9.Text = "出力ヘッダや出力フッタの<?項目名>が、ここで設定した置き換え値に置き換えられます。\r\n最後の行を編集する事で項目を追加、Deleteキーで現在の行を削除することができます。\r\n項目名が空白の行は無視されます。";
			this.dataGridViewTextBoxColumn1.AutoSizeMode = global::System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.dataGridViewTextBoxColumn1.HeaderText = "項目名";
			this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
			this.dataGridViewTextBoxColumn1.SortMode = global::System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.dataGridViewTextBoxColumn2.AutoSizeMode = global::System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.dataGridViewTextBoxColumn2.FillWeight = 160f;
			this.dataGridViewTextBoxColumn2.HeaderText = "置き換え値";
			this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
			this.dataGridViewTextBoxColumn2.SortMode = global::System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.Cancel;
			base.ClientSize = new global::System.Drawing.Size(546, 351);
			base.Controls.Add(this.MainTab);
			base.Controls.Add(this.Accept);
			base.Controls.Add(this.Cancel);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "ProjectConfig";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "プロジェクト設定";
			base.Load += new global::System.EventHandler(this.ProjectConfig_Load);
			this.MainTab.ResumeLayout(false);
			this.A.ResumeLayout(false);
			this.A.PerformLayout();
			((global::System.ComponentModel.ISupportInitialize)this.ProjNum).EndInit();
			this.Fom.ResumeLayout(false);
			this.Fom.PerformLayout();

			this.OH.ResumeLayout(false);
			this.panel1.ResumeLayout(false);

			this.EditContext.ResumeLayout(false);

			this.OF.ResumeLayout(false);
			this.panel2.ResumeLayout(false);

			this.OM.ResumeLayout(false);
			this.panel3.ResumeLayout(false);

			this.OutputReplace.ResumeLayout(false);
			this.OutputReplace.PerformLayout();
			((global::System.ComponentModel.ISupportInitialize)this.OutputReplaceView).EndInit();
			base.ResumeLayout(false);
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.Button Cancel;

		private global::System.Windows.Forms.Button Accept;

		private global::System.Windows.Forms.TabControl MainTab;

		private global::System.Windows.Forms.TabPage A;

		private global::System.Windows.Forms.TabPage OH;

		private global::System.Windows.Forms.TabPage OM;

		private global::System.Windows.Forms.TabPage OF;

		private global::System.Windows.Forms.Panel panel1;

		private global::System.Windows.Forms.RichTextBox OutHeader;

		private global::System.Windows.Forms.Panel panel2;

		private global::System.Windows.Forms.RichTextBox OutFooter;

		private global::System.Windows.Forms.Panel panel3;

		private global::System.Windows.Forms.RichTextBox OutMiddle;

		private global::System.Windows.Forms.TextBox OutExt;

		private global::System.Windows.Forms.Label label2;

		private global::System.Windows.Forms.TextBox OutDir;

		private global::System.Windows.Forms.Label label1;

		private global::System.Windows.Forms.TextBox ProjectName;

		private global::System.Windows.Forms.Label label8;

		private global::System.Windows.Forms.TabPage OutputReplace;

		private global::System.Windows.Forms.DataGridView OutputReplaceView;

		private global::System.Windows.Forms.Label label9;

		private global::System.Windows.Forms.DataGridViewTextBoxColumn ReplName;

		private global::System.Windows.Forms.DataGridViewTextBoxColumn ReplValue;

		private global::System.Windows.Forms.TabPage Fom;

		private global::System.Windows.Forms.Label label13;

		private global::System.Windows.Forms.Label label12;

		private global::System.Windows.Forms.Label label11;

		private global::System.Windows.Forms.Label label10;

		private global::System.Windows.Forms.Label label7;

		private global::System.Windows.Forms.Label label6;

		private global::System.Windows.Forms.TextBox MapF;

		private global::System.Windows.Forms.TextBox LayerF4;

		private global::System.Windows.Forms.TextBox StageF4;

		private global::System.Windows.Forms.TextBox LayerF3;

		private global::System.Windows.Forms.TextBox StageF3;

		private global::System.Windows.Forms.TextBox LayerF2;

		private global::System.Windows.Forms.TextBox StageF2;

		private global::System.Windows.Forms.Label label5;

		private global::System.Windows.Forms.TextBox LayerF;

		private global::System.Windows.Forms.Label label4;

		private global::System.Windows.Forms.Label label3;

		private global::System.Windows.Forms.TextBox StageF;

		private global::System.Windows.Forms.Label label14;

		private global::System.Windows.Forms.Label label15;

		private global::System.Windows.Forms.NumericUpDown ProjNum;

		private global::System.Windows.Forms.Label label17;

		private global::System.Windows.Forms.Label label16;

		private global::System.Windows.Forms.CheckBox UseWorldmap;

        private global::System.Windows.Forms.CheckBox Use3rdMapData;

        private global::System.Windows.Forms.ContextMenuStrip EditContext;

		private global::System.Windows.Forms.ToolStripMenuItem TextUndo;

		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;

		private global::System.Windows.Forms.ToolStripMenuItem TextCut;

		private global::System.Windows.Forms.ToolStripMenuItem TextCopy;

		private global::System.Windows.Forms.ToolStripMenuItem TextPaste;

		private global::System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;

		private global::System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
	}
}
