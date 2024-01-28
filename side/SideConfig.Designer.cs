namespace MasaoPlus
{
	public partial class SideConfig : global::System.Windows.Forms.Form
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
			this.Cancel = new global::System.Windows.Forms.Button();
			this.Accept = new global::System.Windows.Forms.Button();
			this.tabControl1 = new global::System.Windows.Forms.TabControl();
			this.tabPage1 = new global::System.Windows.Forms.TabPage();
			this.UsePropTextDialog = new global::System.Windows.Forms.CheckBox();
			this.OutPutInititalSourceCode = new global::System.Windows.Forms.CheckBox();
			this.WrapPropText = new global::System.Windows.Forms.CheckBox();
			this.IntegrateEditorId = new global::System.Windows.Forms.CheckBox();
			this.EnableAutoUpdate = new global::System.Windows.Forms.CheckBox();
			this.ReversePosition = new global::System.Windows.Forms.CheckBox();
			this.OutputFileEncode = new global::System.Windows.Forms.ComboBox();
			this.OFELabel = new global::System.Windows.Forms.Label();
			this.tabPage2 = new global::System.Windows.Forms.TabPage();
			this.WheelHorz = new global::System.Windows.Forms.CheckBox();
			this.StartWithCL = new global::System.Windows.Forms.CheckBox();
			this.groupBox1 = new global::System.Windows.Forms.GroupBox();
			this.ReuseDraw = new global::System.Windows.Forms.CheckBox();
			this.ChipSkip = new global::System.Windows.Forms.CheckBox();
			this.BufferingDraw = new global::System.Windows.Forms.CheckBox();
			this.ClassicSelectionMode = new global::System.Windows.Forms.ComboBox();
			this.label3 = new global::System.Windows.Forms.Label();
			this.EnableAlpha = new global::System.Windows.Forms.CheckBox();
			this.ExDraw = new global::System.Windows.Forms.CheckBox();
			this.RCContextMenu = new global::System.Windows.Forms.CheckBox();
			this.CursorPageScroll = new global::System.Windows.Forms.CheckBox();
			this.UseClassicCLInterp = new global::System.Windows.Forms.CheckBox();
			this.UseZoomInterp = new global::System.Windows.Forms.CheckBox();
			this.tabPage3 = new global::System.Windows.Forms.TabPage();
			this.UseQTRun = new global::System.Windows.Forms.CheckBox();
			this.UnuseIntegBrow = new global::System.Windows.Forms.GroupBox();
			this.BPSelect = new global::System.Windows.Forms.Button();
			this.BrowPath = new global::System.Windows.Forms.TextBox();
			this.KillExternalBrow = new global::System.Windows.Forms.CheckBox();
			this.BPLabel = new global::System.Windows.Forms.Label();
			this.UseIntegBrow = new global::System.Windows.Forms.CheckBox();
			this.TFNLabel = new global::System.Windows.Forms.Label();
			this.TempFileName = new global::System.Windows.Forms.TextBox();
			this.Regist = new global::System.Windows.Forms.TabPage();
			this.label2 = new global::System.Windows.Forms.Label();
			this.UnregistProjFile = new global::System.Windows.Forms.Button();
			this.RegistProjFile = new global::System.Windows.Forms.Button();
			this.label1 = new global::System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.UnuseIntegBrow.SuspendLayout();
			this.Regist.SuspendLayout();
			base.SuspendLayout();
			this.Cancel.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.Cancel.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(459), base.LogicalToDeviceUnits(283));
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(108, 27));
			this.Cancel.TabIndex = 2;
			this.Cancel.Text = "キャンセル";
			this.Cancel.UseVisualStyleBackColor = true;
			this.Accept.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.Accept.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(345), base.LogicalToDeviceUnits(283));
			this.Accept.Name = "Accept";
			this.Accept.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(108, 27));
			this.Accept.TabIndex = 1;
			this.Accept.Text = "OK";
			this.Accept.UseVisualStyleBackColor = true;
			this.Accept.Click += new global::System.EventHandler(this.Accept_Click);
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.Regist);
			this.tabControl1.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(12), base.LogicalToDeviceUnits(12));
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(555, 265));
			this.tabControl1.TabIndex = 0;
			this.tabPage1.Controls.Add(this.WrapPropText);
			this.tabPage1.Controls.Add(this.OutPutInititalSourceCode);
			this.tabPage1.Controls.Add(this.UsePropTextDialog);
			this.tabPage1.Controls.Add(this.IntegrateEditorId);
			this.tabPage1.Controls.Add(this.EnableAutoUpdate);
			this.tabPage1.Controls.Add(this.ReversePosition);
			this.tabPage1.Controls.Add(this.OutputFileEncode);
			this.tabPage1.Controls.Add(this.OFELabel);
			this.tabPage1.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(4), base.LogicalToDeviceUnits(22));
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new global::System.Windows.Forms.Padding(3);
			this.tabPage1.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(547, 239));
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "システム";
			this.tabPage1.UseVisualStyleBackColor = true;
			this.WrapPropText.AutoSize = true;
			this.WrapPropText.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(142));
			this.WrapPropText.Name = "WrapPropText";
			this.WrapPropText.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(212, 16));
			this.WrapPropText.TabIndex = 7;
			this.WrapPropText.Text = "長いプロパティ名を改行する(&B)";
			this.WrapPropText.UseVisualStyleBackColor = true;
			this.OutPutInititalSourceCode.AutoSize = true;
			this.OutPutInititalSourceCode.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(120));
			this.OutPutInititalSourceCode.Name = "OutPutInititalSourceCode";
			this.OutPutInititalSourceCode.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(212, 16));//?
			this.OutPutInititalSourceCode.TabIndex = 6;
			this.OutPutInititalSourceCode.Text = "初期値のパラメータを出力する(2.8ではチェックの有無に関わらず出力)(&P)";
			this.OutPutInititalSourceCode.UseVisualStyleBackColor = true;
			this.UsePropTextDialog.AutoSize = true;
			this.UsePropTextDialog.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(98));
			this.UsePropTextDialog.Name = "UsePropTextDialog";
			this.UsePropTextDialog.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(362, 16));
			this.UsePropTextDialog.TabIndex = 5;
			this.UsePropTextDialog.Text = "プロパティエディタの複数行テキスト項目はダイアログを用いて編集する(&D)";
			this.UsePropTextDialog.UseVisualStyleBackColor = true;
			this.IntegrateEditorId.AutoSize = true;
			this.IntegrateEditorId.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(76));
			this.IntegrateEditorId.Name = "IntegrateEditorId";
			this.IntegrateEditorId.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(212, 16));
			this.IntegrateEditorId.TabIndex = 4;
			this.IntegrateEditorId.Text = "HTMLにエディタ識別コードを埋め込む(&I)";
			this.IntegrateEditorId.UseVisualStyleBackColor = true;
			this.EnableAutoUpdate.AutoSize = true;
			this.EnableAutoUpdate.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(54));
			this.EnableAutoUpdate.Name = "EnableAutoUpdate";
			this.EnableAutoUpdate.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(168, 16));
			this.EnableAutoUpdate.TabIndex = 3;
			this.EnableAutoUpdate.Text = "起動時に更新をチェックする(&A)";
			this.EnableAutoUpdate.UseVisualStyleBackColor = true;
			this.ReversePosition.AutoSize = true;
			this.ReversePosition.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(32));
			this.ReversePosition.Name = "ReversePosition";
			this.ReversePosition.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(432, 16));
			this.ReversePosition.TabIndex = 2;
			this.ReversePosition.Text = "エディタとチップリスト/プロパティエディタの配置を左右逆にする(&R)[Sideの再起動が必要]";
			this.ReversePosition.UseVisualStyleBackColor = true;
			this.OutputFileEncode.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.OutputFileEncode.FormattingEnabled = true;
			this.OutputFileEncode.Items.AddRange(new object[]
			{
				"UTF-8",
				"Shift-JIS",
				"EUC",
				"ISO-2022-JP"
			});
			this.OutputFileEncode.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(130), base.LogicalToDeviceUnits(6));
			this.OutputFileEncode.Name = "OutputFileEncode";
			this.OutputFileEncode.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(183, 20));
			this.OutputFileEncode.TabIndex = 1;
			this.OFELabel.AutoSize = true;
			this.OFELabel.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(9));
			this.OFELabel.Name = "OFELabel";
			this.OFELabel.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(118, 12));
			this.OFELabel.TabIndex = 0;
			this.OFELabel.Text = "出力ファイルのエンコード";
			this.tabPage2.Controls.Add(this.WheelHorz);
			this.tabPage2.Controls.Add(this.StartWithCL);
			this.tabPage2.Controls.Add(this.groupBox1);
			this.tabPage2.Controls.Add(this.ClassicSelectionMode);
			this.tabPage2.Controls.Add(this.label3);
			this.tabPage2.Controls.Add(this.EnableAlpha);
			this.tabPage2.Controls.Add(this.ExDraw);
			this.tabPage2.Controls.Add(this.RCContextMenu);
			this.tabPage2.Controls.Add(this.CursorPageScroll);
			this.tabPage2.Controls.Add(this.UseClassicCLInterp);
			this.tabPage2.Controls.Add(this.UseZoomInterp);
			this.tabPage2.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(4), base.LogicalToDeviceUnits(22));
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new global::System.Windows.Forms.Padding(3);
			this.tabPage2.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(547, 239));
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "グラフィカルデザイナ";
			this.tabPage2.UseVisualStyleBackColor = true;
			this.WheelHorz.AutoSize = true;
			this.WheelHorz.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(246), base.LogicalToDeviceUnits(98));
			this.WheelHorz.Name = "WheelHorz";
			this.WheelHorz.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(191, 16));
			this.WheelHorz.TabIndex = 9;
			this.WheelHorz.Text = "ホイールスクロールで横移動する(&W)";
			this.WheelHorz.UseVisualStyleBackColor = true;
			this.StartWithCL.AutoSize = true;
			this.StartWithCL.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(98));
			this.StartWithCL.Name = "StartWithCL";
			this.StartWithCL.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(207, 16));
			this.StartWithCL.TabIndex = 8;
			this.StartWithCL.Text = "クラシックチップリスト状態で起動する(&C)";
			this.StartWithCL.UseVisualStyleBackColor = true;
			this.groupBox1.Controls.Add(this.ReuseDraw);
			this.groupBox1.Controls.Add(this.ChipSkip);
			this.groupBox1.Controls.Add(this.BufferingDraw);
			this.groupBox1.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(142));
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(535, 91));
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "詳細";
			this.ReuseDraw.AutoSize = true;
			this.ReuseDraw.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(62));
			this.ReuseDraw.Name = "ReuseDraw";
			this.ReuseDraw.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(525, 16));
			this.ReuseDraw.TabIndex = 2;
			this.ReuseDraw.Text = "描画を再利用する(&R)[描画処理が高速化することがあります](テスト版につき正常動作しないことがあります)";
			this.ReuseDraw.UseVisualStyleBackColor = true;
			this.ReuseDraw.CheckedChanged += new global::System.EventHandler(this.ReuseDraw_CheckedChanged);
			this.ChipSkip.AutoSize = true;
			this.ChipSkip.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(40));
			this.ChipSkip.Name = "ChipSkip";
			this.ChipSkip.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(376, 16));
			this.ChipSkip.TabIndex = 1;
			this.ChipSkip.Text = "チップ定義最初のチップの描画を省略する(&S)[速度が向上する事もあります]\r\n";
			this.ChipSkip.UseVisualStyleBackColor = true;
			this.ChipSkip.CheckedChanged += new global::System.EventHandler(this.ChipSkip_CheckedChanged);
			this.BufferingDraw.AutoSize = true;
			this.BufferingDraw.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(18));
			this.BufferingDraw.Name = "BufferingDraw";
			this.BufferingDraw.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(517, 16));
			this.BufferingDraw.TabIndex = 0;
			this.BufferingDraw.Text = "ペンツール以外の描画をバッファリングする(&B)[案内描画が目に見えて速くなりますが、他の速度が落ちます]";
			this.BufferingDraw.UseVisualStyleBackColor = true;
			this.ClassicSelectionMode.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ClassicSelectionMode.FormattingEnabled = true;
			this.ClassicSelectionMode.Items.AddRange(new object[]
			{
				"Side オリジナル",
				"Masao Tool ++風",
				"正男ツール風",
				"選択表示なし(正男スタジオ風)"
			});
			this.ClassicSelectionMode.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(165), base.LogicalToDeviceUnits(6));
			this.ClassicSelectionMode.Name = "ClassicSelectionMode";
			this.ClassicSelectionMode.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(237, 20));
			this.ClassicSelectionMode.TabIndex = 1;
			this.label3.AutoSize = true;
			this.label3.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(9));
			this.label3.Name = "label3";
			this.label3.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(153, 12));
			this.label3.TabIndex = 0;
			this.label3.Text = "クラシックチップリストの選択表示";
			this.EnableAlpha.AutoSize = true;
			this.EnableAlpha.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(76));
			this.EnableAlpha.Name = "EnableAlpha";
			this.EnableAlpha.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(207, 16));
			this.EnableAlpha.TabIndex = 6;
			this.EnableAlpha.Text = "線や四角描画の表示を半透過する(&T)";
			this.EnableAlpha.UseVisualStyleBackColor = true;
			this.ExDraw.AutoSize = true;
			this.ExDraw.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(246), base.LogicalToDeviceUnits(76));
			this.ExDraw.Name = "ExDraw";
			this.ExDraw.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(187, 16));
			this.ExDraw.TabIndex = 7;
			this.ExDraw.Text = "拡張描画する(&X)[非常におすすめ]";
			this.ExDraw.UseVisualStyleBackColor = true;
			this.RCContextMenu.AutoSize = true;
			this.RCContextMenu.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(246), base.LogicalToDeviceUnits(54));
			this.RCContextMenu.Name = "RCContextMenu";
			this.RCContextMenu.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(191, 16));
			this.RCContextMenu.TabIndex = 5;
			this.RCContextMenu.Text = "右クリック時にメニューを表示する(&M)";
			this.RCContextMenu.UseVisualStyleBackColor = true;
			this.CursorPageScroll.AutoSize = true;
			this.CursorPageScroll.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(54));
			this.CursorPageScroll.Name = "CursorPageScroll";
			this.CursorPageScroll.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(214, 16));
			this.CursorPageScroll.TabIndex = 4;
			this.CursorPageScroll.Text = "カーソルスクロールをページ単位にする(&P)";
			this.CursorPageScroll.UseVisualStyleBackColor = true;
			this.UseClassicCLInterp.AutoSize = true;
			this.UseClassicCLInterp.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(247), base.LogicalToDeviceUnits(32));
			this.UseClassicCLInterp.Name = "UseClassicCLInterp";
			this.UseClassicCLInterp.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(241, 16));
			this.UseClassicCLInterp.TabIndex = 3;
			this.UseClassicCLInterp.Text = "クラシックチップリストのチップを補完描画する(&C)";
			this.UseClassicCLInterp.UseVisualStyleBackColor = true;
			this.UseZoomInterp.AutoSize = true;
			this.UseZoomInterp.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(32));
			this.UseZoomInterp.Name = "UseZoomInterp";
			this.UseZoomInterp.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(200, 16));
			this.UseZoomInterp.TabIndex = 2;
			this.UseZoomInterp.Text = "拡大/縮小描画時、補完描画する(&I)";
			this.UseZoomInterp.UseVisualStyleBackColor = true;
			this.tabPage3.Controls.Add(this.UseQTRun);
			this.tabPage3.Controls.Add(this.UnuseIntegBrow);
			this.tabPage3.Controls.Add(this.UseIntegBrow);
			this.tabPage3.Controls.Add(this.TFNLabel);
			this.tabPage3.Controls.Add(this.TempFileName);
			this.tabPage3.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(4), base.LogicalToDeviceUnits(22));
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new global::System.Windows.Forms.Padding(3);
			this.tabPage3.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(547, 239));
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "テスト実行";
			this.tabPage3.UseVisualStyleBackColor = true;
			this.UseQTRun.AutoSize = true;
			this.UseQTRun.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(133));
			this.UseQTRun.Name = "UseQTRun";
			this.UseQTRun.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(490, 16));
			this.UseQTRun.TabIndex = 4;
			this.UseQTRun.Text = "マップを中クリックした時にそこからテスト実行する(&Q)[チップ定義の2番目が正男である必要があります]";
			this.UseQTRun.UseVisualStyleBackColor = true;
			this.UnuseIntegBrow.Controls.Add(this.BPSelect);
			this.UnuseIntegBrow.Controls.Add(this.BrowPath);
			this.UnuseIntegBrow.Controls.Add(this.KillExternalBrow);
			this.UnuseIntegBrow.Controls.Add(this.BPLabel);
			this.UnuseIntegBrow.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(8), base.LogicalToDeviceUnits(56));
			this.UnuseIntegBrow.Name = "UnuseIntegBrow";
			this.UnuseIntegBrow.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(533, 71));
			this.UnuseIntegBrow.TabIndex = 3;
			this.UnuseIntegBrow.TabStop = false;
			this.UnuseIntegBrow.Text = "内蔵ブラウザを利用しない(外部ブラウザ利用)時の設定";
			this.BPSelect.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(484), base.LogicalToDeviceUnits(38));
			this.BPSelect.Name = "BPSelect";
			this.BPSelect.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(28, 23));
			this.BPSelect.TabIndex = 3;
			this.BPSelect.Text = "...";
			this.BPSelect.UseVisualStyleBackColor = true;
			this.BPSelect.Click += new global::System.EventHandler(this.BPSelect_Click);
			this.BrowPath.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(236), base.LogicalToDeviceUnits(40));
			this.BrowPath.Name = "BrowPath";
			this.BrowPath.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(242, 19));
			this.BrowPath.TabIndex = 2;
			this.KillExternalBrow.AutoSize = true;
			this.KillExternalBrow.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(10), base.LogicalToDeviceUnits(18));
			this.KillExternalBrow.Name = "KillExternalBrow";
			this.KillExternalBrow.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(419, 16));
			this.KillExternalBrow.TabIndex = 0;
			this.KillExternalBrow.Text = "外部ブラウザ実行時、フォーカスがSideに戻ったら実行したブラウザを自動終了する(&K)";
			this.KillExternalBrow.UseVisualStyleBackColor = true;
			this.BPLabel.AutoSize = true;
			this.BPLabel.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(8), base.LogicalToDeviceUnits(43));
			this.BPLabel.Name = "BPLabel";
			this.BPLabel.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(222, 12));
			this.BPLabel.TabIndex = 1;
			this.BPLabel.Text = "ブラウザのパス(空白で関連付けられたブラウザ)";
			this.UseIntegBrow.AutoSize = true;
			this.UseIntegBrow.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(8), base.LogicalToDeviceUnits(34));
			this.UseIntegBrow.Name = "UseIntegBrow";
			this.UseIntegBrow.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(123, 16));
			this.UseIntegBrow.TabIndex = 2;
			this.UseIntegBrow.Text = "内蔵ブラウザを使う(&I)";
			this.UseIntegBrow.UseVisualStyleBackColor = true;
			this.UseIntegBrow.CheckedChanged += new global::System.EventHandler(this.UseIntegBrow_CheckedChanged);
			this.TFNLabel.AutoSize = true;
			this.TFNLabel.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(9));
			this.TFNLabel.Name = "TFNLabel";
			this.TFNLabel.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(166, 12));
			this.TFNLabel.TabIndex = 0;
			this.TFNLabel.Text = "テンポラリファイル名(拡張子は除く)";
			this.TempFileName.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(178), base.LogicalToDeviceUnits(6));
			this.TempFileName.Name = "TempFileName";
			this.TempFileName.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(276, 19));
			this.TempFileName.TabIndex = 1;
			this.Regist.Controls.Add(this.label2);
			this.Regist.Controls.Add(this.UnregistProjFile);
			this.Regist.Controls.Add(this.RegistProjFile);
			this.Regist.Controls.Add(this.label1);
			this.Regist.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(4), base.LogicalToDeviceUnits(22));
			this.Regist.Name = "Regist";
			this.Regist.Padding = new global::System.Windows.Forms.Padding(3);
			this.Regist.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(547, 239));
			this.Regist.TabIndex = 3;
			this.Regist.Text = "関連付け";
			this.Regist.UseVisualStyleBackColor = true;
			this.label2.AutoSize = true;
			this.label2.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(169), base.LogicalToDeviceUnits(40));
			this.label2.Margin = new global::System.Windows.Forms.Padding(3);
			this.label2.Name = "label2";
			this.label2.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(312, 36));
			this.label2.TabIndex = 3;
			this.label2.Text = "Vista環境ではUACによる昇格が必要になる場合があります。\r\nまた、ボタンが押せない場合は必要な権限が無い恐れがあります。\r\nPowerUser権限以上で実行してください。\r\n";
			this.UnregistProjFile.FlatStyle = global::System.Windows.Forms.FlatStyle.System;
			this.UnregistProjFile.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(8), base.LogicalToDeviceUnits(60));
			this.UnregistProjFile.Name = "UnregistProjFile";
			this.UnregistProjFile.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(155, 30));
			this.UnregistProjFile.TabIndex = 2;
			this.UnregistProjFile.Text = "関連付けを解除する";
			this.UnregistProjFile.UseVisualStyleBackColor = true;
			this.UnregistProjFile.Click += new global::System.EventHandler(this.UnregistProjFile_Click);
			this.RegistProjFile.FlatStyle = global::System.Windows.Forms.FlatStyle.System;
			this.RegistProjFile.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(8), base.LogicalToDeviceUnits(24));
			this.RegistProjFile.Name = "RegistProjFile";
			this.RegistProjFile.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(155, 30));
			this.RegistProjFile.TabIndex = 1;
			this.RegistProjFile.Text = "関連付けする";
			this.RegistProjFile.UseVisualStyleBackColor = true;
			this.RegistProjFile.Click += new global::System.EventHandler(this.RegistProjFile_Click);
			this.label1.AutoSize = true;
			this.label1.Location = new global::System.Drawing.Point(base.LogicalToDeviceUnits(6), base.LogicalToDeviceUnits(6));
			this.label1.Margin = new global::System.Windows.Forms.Padding(3);
			this.label1.Name = "label1";
			this.label1.Size = base.LogicalToDeviceUnits(new global::System.Drawing.Size(357, 12));
			this.label1.TabIndex = 0;
			this.label1.Text = "Side プロジェクトファイルやランタイムファイルに対しての関連付けが行えます。\r\n";
			base.AcceptButton = this.Accept;
			//base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			//base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.Cancel;
			base.ClientSize = base.LogicalToDeviceUnits(new global::System.Drawing.Size(579, 322));
			base.ControlBox = false;
			base.Controls.Add(this.tabControl1);
			base.Controls.Add(this.Accept);
			base.Controls.Add(this.Cancel);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "SideConfig";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Side 設定";
			base.Load += new global::System.EventHandler(this.SideConfig_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this.UnuseIntegBrow.ResumeLayout(false);
			this.UnuseIntegBrow.PerformLayout();
			this.Regist.ResumeLayout(false);
			this.Regist.PerformLayout();
			base.ResumeLayout(false);
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.Button Cancel;

		private global::System.Windows.Forms.Button Accept;

		private global::System.Windows.Forms.TabControl tabControl1;

		private global::System.Windows.Forms.TabPage tabPage1;

		private global::System.Windows.Forms.TabPage tabPage2;

		private global::System.Windows.Forms.ComboBox OutputFileEncode;

		private global::System.Windows.Forms.Label OFELabel;

		private global::System.Windows.Forms.TabPage tabPage3;

		private global::System.Windows.Forms.Label TFNLabel;

		private global::System.Windows.Forms.TextBox TempFileName;

		private global::System.Windows.Forms.CheckBox UseIntegBrow;

		private global::System.Windows.Forms.GroupBox UnuseIntegBrow;

		private global::System.Windows.Forms.Button BPSelect;

		private global::System.Windows.Forms.TextBox BrowPath;

		private global::System.Windows.Forms.CheckBox KillExternalBrow;

		private global::System.Windows.Forms.Label BPLabel;

		private global::System.Windows.Forms.CheckBox UseZoomInterp;

		private global::System.Windows.Forms.CheckBox UseClassicCLInterp;

		private global::System.Windows.Forms.CheckBox UseQTRun;

		private global::System.Windows.Forms.CheckBox CursorPageScroll;

		private global::System.Windows.Forms.TabPage Regist;

		private global::System.Windows.Forms.Button UnregistProjFile;

		private global::System.Windows.Forms.Button RegistProjFile;

		private global::System.Windows.Forms.Label label1;

		private global::System.Windows.Forms.Label label2;

		private global::System.Windows.Forms.CheckBox ReversePosition;

		private global::System.Windows.Forms.CheckBox EnableAutoUpdate;

		private global::System.Windows.Forms.CheckBox RCContextMenu;

		private global::System.Windows.Forms.CheckBox ExDraw;

		private global::System.Windows.Forms.CheckBox EnableAlpha;

		private global::System.Windows.Forms.ComboBox ClassicSelectionMode;

		private global::System.Windows.Forms.Label label3;

		private global::System.Windows.Forms.GroupBox groupBox1;

		private global::System.Windows.Forms.CheckBox BufferingDraw;

		private global::System.Windows.Forms.CheckBox ChipSkip;

		private global::System.Windows.Forms.CheckBox ReuseDraw;

		private global::System.Windows.Forms.CheckBox IntegrateEditorId;

		private global::System.Windows.Forms.CheckBox UsePropTextDialog;

		private global::System.Windows.Forms.CheckBox OutPutInititalSourceCode;

		private global::System.Windows.Forms.CheckBox WrapPropText;

		private global::System.Windows.Forms.CheckBox StartWithCL;

		private global::System.Windows.Forms.CheckBox WheelHorz;
	}
}
