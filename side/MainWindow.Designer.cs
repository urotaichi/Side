namespace MasaoPlus
{
	// Token: 0x0200002B RID: 43
	public partial class MainWindow : global::System.Windows.Forms.Form
	{
		// Token: 0x06000138 RID: 312 RVA: 0x00002C71 File Offset: 0x00000E71
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0001A914 File Offset: 0x00018B14
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::MasaoPlus.MainWindow));
			this.MainDock = new global::System.Windows.Forms.ToolStripContainer();
			this.MainSplit = new global::System.Windows.Forms.SplitContainer();
			this.SideTab = new global::System.Windows.Forms.TabControl();
			this.ChipPage = new global::System.Windows.Forms.TabPage();
			this.ItemSpliter = new global::System.Windows.Forms.SplitContainer();
			this.ChipDescription = new global::System.Windows.Forms.Label();
			this.ChipChar = new global::System.Windows.Forms.Label();
			this.ChipImage = new global::System.Windows.Forms.PictureBox();
			this.ChipList = new global::System.Windows.Forms.ListBox();
			this.DrawType = new global::System.Windows.Forms.ComboBox();
			this.PropertyTab = new global::System.Windows.Forms.TabPage();
			this.EditTab = new global::System.Windows.Forms.TabControl();
			this.GuiEditor = new global::System.Windows.Forms.TabPage();
			this.EditorSystemPanel = new global::System.Windows.Forms.Panel();
			this.GVirtScroll = new global::System.Windows.Forms.VScrollBar();
			this.GHorzScroll = new global::System.Windows.Forms.HScrollBar();
			this.GuiEditorTool = new global::System.Windows.Forms.ToolStrip();
			this.ShowGrid = new global::System.Windows.Forms.ToolStripButton();
			this.StageZoom = new global::System.Windows.Forms.ToolStripDropDownButton();
			this.StageZoom25 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.StageZoom50 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.StageZoom100 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.StageZoom200 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.StageLayer = new global::System.Windows.Forms.ToolStripDropDownButton();
			this.PatternChipLayer = new global::System.Windows.Forms.ToolStripMenuItem();
			this.BackgroundLayer = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem13 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MDUnactiveLayer = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MTUnactiveLayer = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new global::System.Windows.Forms.ToolStripSeparator();
			this.ItemUndo = new global::System.Windows.Forms.ToolStripButton();
			this.ItemRedo = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator7 = new global::System.Windows.Forms.ToolStripSeparator();
			this.ItemCursor = new global::System.Windows.Forms.ToolStripButton();
			this.ItemPen = new global::System.Windows.Forms.ToolStripButton();
			this.ItemLine = new global::System.Windows.Forms.ToolStripButton();
			this.ItemRect = new global::System.Windows.Forms.ToolStripButton();
			this.ItemFill = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new global::System.Windows.Forms.ToolStripSeparator();
			this.GuiCut = new global::System.Windows.Forms.ToolStripButton();
			this.GuiCopy = new global::System.Windows.Forms.ToolStripButton();
			this.GuiPaste = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator6 = new global::System.Windows.Forms.ToolStripSeparator();
			this.ItemReload = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator8 = new global::System.Windows.Forms.ToolStripSeparator();
			this.GUIProjConfig = new global::System.Windows.Forms.ToolStripButton();
			this.GUIProjTestRun = new global::System.Windows.Forms.ToolStripButton();
			this.GUIEditorStatus = new global::System.Windows.Forms.StatusStrip();
			this.ChipNavigator = new global::System.Windows.Forms.ToolStripStatusLabel();
			this.GuiPositionInfo = new global::System.Windows.Forms.ToolStripStatusLabel();
			this.CurrentStatus = new global::System.Windows.Forms.ToolStripStatusLabel();
			this.TextEditor = new global::System.Windows.Forms.TabPage();
			this.IntegrateTestrun = new global::System.Windows.Forms.TabPage();
			this.GUIMenu = new global::System.Windows.Forms.MenuStrip();
			this.MFile = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MNew = new global::System.Windows.Forms.ToolStripMenuItem();
			this.他の方法で新規MToolStripMenuItem = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MProjectInheritNew = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MConvertHTML = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem16 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MOpen = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MSave = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MSaveAs = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MOutput = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MWriteHTML = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MWriteStagePicture = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MExit = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMEdit = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMUndo = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMRedo = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new global::System.Windows.Forms.ToolStripSeparator();
			this.GMCut = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMCopy = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMPaste = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new global::System.Windows.Forms.ToolStripSeparator();
			this.GMTestRun = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMAllTestrun = new global::System.Windows.Forms.ToolStripMenuItem();
			this.TMEdit = new global::System.Windows.Forms.ToolStripMenuItem();
			this.TMUndo = new global::System.Windows.Forms.ToolStripMenuItem();
			this.TMRedo = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator10 = new global::System.Windows.Forms.ToolStripSeparator();
			this.TMCut = new global::System.Windows.Forms.ToolStripMenuItem();
			this.TMCopy = new global::System.Windows.Forms.ToolStripMenuItem();
			this.TMPaste = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator11 = new global::System.Windows.Forms.ToolStripSeparator();
			this.TMTestRun = new global::System.Windows.Forms.ToolStripMenuItem();
			this.TMAllTestrun = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMStage = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MEditStage1 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MEditStage2 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MEditStage3 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MEditStage4 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem18 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MEditMap = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem19 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MSConfig = new global::System.Windows.Forms.ToolStripMenuItem();
			this.LayerMenu = new global::System.Windows.Forms.ToolStripMenuItem();
			this.EditPatternChip = new global::System.Windows.Forms.ToolStripMenuItem();
			this.EditBackground = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem8 = new global::System.Windows.Forms.ToolStripSeparator();
			this.ViewUnactiveLayer = new global::System.Windows.Forms.ToolStripMenuItem();
			this.TransparentUnactiveLayer = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMTool = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMZoomIndexes = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMZoom25 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMZoom50 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMZoom100 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMZoom200 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMShowGrid = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMRedraw = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem7 = new global::System.Windows.Forms.ToolStripSeparator();
			this.GMCursor = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMPen = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMLine = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMRect = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GMFill = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem17 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MExEdit = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MStageRev = new global::System.Windows.Forms.ToolStripMenuItem();
			this.ShowOverView = new global::System.Windows.Forms.ToolStripMenuItem();
			this.BMView = new global::System.Windows.Forms.ToolStripMenuItem();
			this.BMReload = new global::System.Windows.Forms.ToolStripMenuItem();
			this.BMBrowser = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem6 = new global::System.Windows.Forms.ToolStripSeparator();
			this.BMTestEnd = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MConfiguration = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MProjectConfig = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MSysConfig = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem14 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MResetProjRuntime = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem20 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MReloadImage = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MHelp = new global::System.Windows.Forms.ToolStripMenuItem();
			this.InstalledRuntime = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem15 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MUpdateApp = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MVersion = new global::System.Windows.Forms.ToolStripMenuItem();
			this.MainToolbar = new global::System.Windows.Forms.ToolStrip();
			this.MTNew = new global::System.Windows.Forms.ToolStripButton();
			this.MTOpen = new global::System.Windows.Forms.ToolStripButton();
			this.MTSave = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MTImgOut = new global::System.Windows.Forms.ToolStripButton();
			this.MTOutput = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MTEditConfig = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator5 = new global::System.Windows.Forms.ToolStripSeparator();
			this.MTVersion = new global::System.Windows.Forms.ToolStripButton();
			this.toolStripMenuItem9 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem10 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem11 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem12 = new global::System.Windows.Forms.ToolStripMenuItem();
			this.GuiChipList = new global::MasaoPlus.GUIChipList();
			this.MasaoConfigList = new global::MasaoPlus.Controls.ConfigList();
			this.MainDesigner = new global::MasaoPlus.GUIDesigner();
			this.MainEditor = new global::MasaoPlus.TextEditor();
			this.IntegrateBrowser = new global::MasaoPlus.IntegratedBrowser();
			this.MainDock.ContentPanel.SuspendLayout();
			this.MainDock.TopToolStripPanel.SuspendLayout();
			this.MainDock.SuspendLayout();
			this.MainSplit.Panel1.SuspendLayout();
			this.MainSplit.Panel2.SuspendLayout();
			this.MainSplit.SuspendLayout();
			this.SideTab.SuspendLayout();
			this.ChipPage.SuspendLayout();
			this.ItemSpliter.Panel1.SuspendLayout();
			this.ItemSpliter.Panel2.SuspendLayout();
			this.ItemSpliter.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.ChipImage).BeginInit();
			this.PropertyTab.SuspendLayout();
			this.EditTab.SuspendLayout();
			this.GuiEditor.SuspendLayout();
			this.EditorSystemPanel.SuspendLayout();
			this.GuiEditorTool.SuspendLayout();
			this.GUIEditorStatus.SuspendLayout();
			this.TextEditor.SuspendLayout();
			this.IntegrateTestrun.SuspendLayout();
			this.GUIMenu.SuspendLayout();
			this.MainToolbar.SuspendLayout();
			base.SuspendLayout();
			this.MainDock.ContentPanel.Controls.Add(this.MainSplit);
			this.MainDock.ContentPanel.Size = new global::System.Drawing.Size(674, 309);
			this.MainDock.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.MainDock.Location = new global::System.Drawing.Point(0, 0);
			this.MainDock.Name = "MainDock";
			this.MainDock.Size = new global::System.Drawing.Size(674, 360);
			this.MainDock.TabIndex = 0;
			this.MainDock.Text = "toolStripContainer1";
			this.MainDock.TopToolStripPanel.Controls.Add(this.GUIMenu);
			this.MainDock.TopToolStripPanel.Controls.Add(this.MainToolbar);
			this.MainSplit.BackColor = global::System.Drawing.Color.White;
			this.MainSplit.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.MainSplit.Location = new global::System.Drawing.Point(0, 0);
			this.MainSplit.Name = "MainSplit";
			this.MainSplit.Panel1.Controls.Add(this.SideTab);
			this.MainSplit.Panel2.Controls.Add(this.EditTab);
			this.MainSplit.Size = new global::System.Drawing.Size(674, 309);
			this.MainSplit.SplitterDistance = 185;
			this.MainSplit.TabIndex = 1;
			this.SideTab.Controls.Add(this.ChipPage);
			this.SideTab.Controls.Add(this.PropertyTab);
			this.SideTab.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.SideTab.Location = new global::System.Drawing.Point(0, 0);
			this.SideTab.Name = "SideTab";
			this.SideTab.SelectedIndex = 0;
			this.SideTab.Size = new global::System.Drawing.Size(185, 309);
			this.SideTab.TabIndex = 2;
			this.SideTab.SelectedIndexChanged += new global::System.EventHandler(this.SideTab_SelectedIndexChanged);
			this.ChipPage.Controls.Add(this.ItemSpliter);
			this.ChipPage.Location = new global::System.Drawing.Point(4, 22);
			this.ChipPage.Name = "ChipPage";
			this.ChipPage.Padding = new global::System.Windows.Forms.Padding(3);
			this.ChipPage.Size = new global::System.Drawing.Size(177, 283);
			this.ChipPage.TabIndex = 0;
			this.ChipPage.Text = "チップ";
			this.ChipPage.UseVisualStyleBackColor = true;
			this.ItemSpliter.BackColor = global::System.Drawing.Color.White;
			this.ItemSpliter.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.ItemSpliter.FixedPanel = global::System.Windows.Forms.FixedPanel.Panel1;
			this.ItemSpliter.IsSplitterFixed = true;
			this.ItemSpliter.Location = new global::System.Drawing.Point(3, 3);
			this.ItemSpliter.Margin = new global::System.Windows.Forms.Padding(0);
			this.ItemSpliter.Name = "ItemSpliter";
			this.ItemSpliter.Orientation = global::System.Windows.Forms.Orientation.Horizontal;
			this.ItemSpliter.Panel1.Controls.Add(this.ChipDescription);
			this.ItemSpliter.Panel1.Controls.Add(this.ChipChar);
			this.ItemSpliter.Panel1.Controls.Add(this.ChipImage);
			this.ItemSpliter.Panel2.Controls.Add(this.GuiChipList);
			this.ItemSpliter.Panel2.Controls.Add(this.ChipList);
			this.ItemSpliter.Panel2.Controls.Add(this.DrawType);
			this.ItemSpliter.Size = new global::System.Drawing.Size(171, 277);
			this.ItemSpliter.SplitterDistance = 40;
			this.ItemSpliter.TabIndex = 1;
			this.ItemSpliter.TabStop = false;
			this.ChipDescription.AutoSize = true;
			this.ChipDescription.Font = new global::System.Drawing.Font("MS UI Gothic", 9f, global::System.Drawing.FontStyle.Bold, global::System.Drawing.GraphicsUnit.Point, 128);
			this.ChipDescription.Location = new global::System.Drawing.Point(38, 10);
			this.ChipDescription.Name = "ChipDescription";
			this.ChipDescription.Size = new global::System.Drawing.Size(12, 12);
			this.ChipDescription.TabIndex = 2;
			this.ChipDescription.Text = "-";
			this.ChipChar.AutoSize = true;
			this.ChipChar.Location = new global::System.Drawing.Point(38, 22);
			this.ChipChar.Name = "ChipChar";
			this.ChipChar.Size = new global::System.Drawing.Size(25, 12);
			this.ChipChar.TabIndex = 1;
			this.ChipChar.Text = "[-]-";
			this.ChipImage.Location = new global::System.Drawing.Point(3, 4);
			this.ChipImage.Name = "ChipImage";
			this.ChipImage.Size = new global::System.Drawing.Size(32, 32);
			this.ChipImage.TabIndex = 0;
			this.ChipImage.TabStop = false;
			this.ChipImage.Paint += new global::System.Windows.Forms.PaintEventHandler(this.ChipImage_Paint);
			this.ChipList.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.ChipList.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.ChipList.DrawMode = global::System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.ChipList.FormattingEnabled = true;
			this.ChipList.IntegralHeight = false;
			this.ChipList.ItemHeight = 12;
			this.ChipList.Location = new global::System.Drawing.Point(0, 20);
			this.ChipList.Margin = new global::System.Windows.Forms.Padding(0);
			this.ChipList.Name = "ChipList";
			this.ChipList.Size = new global::System.Drawing.Size(171, 213);
			this.ChipList.TabIndex = 0;
			this.ChipList.DrawItem += new global::System.Windows.Forms.DrawItemEventHandler(this.ChipList_DrawItem);
			this.ChipList.MeasureItem += new global::System.Windows.Forms.MeasureItemEventHandler(this.ChipList_MeasureItem);
			this.ChipList.SelectedIndexChanged += new global::System.EventHandler(this.ChipList_SelectedIndexChanged);
			this.DrawType.Dock = global::System.Windows.Forms.DockStyle.Top;
			this.DrawType.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DrawType.FlatStyle = global::System.Windows.Forms.FlatStyle.System;
			this.DrawType.FormattingEnabled = true;
			this.DrawType.IntegralHeight = false;
			this.DrawType.Items.AddRange(new object[]
			{
				"サムネイル",
				"テキスト",
				"チップ",
				"クラシック"
			});
			this.DrawType.Location = new global::System.Drawing.Point(0, 0);
			this.DrawType.Name = "DrawType";
			this.DrawType.Size = new global::System.Drawing.Size(171, 20);
			this.DrawType.TabIndex = 1;
			this.DrawType.Resize += new global::System.EventHandler(this.DrawType_Resize);
			this.DrawType.SelectedIndexChanged += new global::System.EventHandler(this.DrawType_SelectedIndexChanged);
			this.PropertyTab.Controls.Add(this.MasaoConfigList);
			this.PropertyTab.Location = new global::System.Drawing.Point(4, 22);
			this.PropertyTab.Name = "PropertyTab";
			this.PropertyTab.Padding = new global::System.Windows.Forms.Padding(3);
			this.PropertyTab.Size = new global::System.Drawing.Size(177, 283);
			this.PropertyTab.TabIndex = 1;
			this.PropertyTab.Text = "プロパティ";
			this.PropertyTab.UseVisualStyleBackColor = true;
			this.EditTab.Controls.Add(this.GuiEditor);
			this.EditTab.Controls.Add(this.TextEditor);
			this.EditTab.Controls.Add(this.IntegrateTestrun);
			this.EditTab.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.EditTab.Location = new global::System.Drawing.Point(0, 0);
			this.EditTab.Margin = new global::System.Windows.Forms.Padding(0);
			this.EditTab.Name = "EditTab";
			this.EditTab.SelectedIndex = 0;
			this.EditTab.Size = new global::System.Drawing.Size(485, 309);
			this.EditTab.TabIndex = 0;
			this.EditTab.Selecting += new global::System.Windows.Forms.TabControlCancelEventHandler(this.EditTab_Selecting);
			this.EditTab.Deselecting += new global::System.Windows.Forms.TabControlCancelEventHandler(this.EditTab_Deselecting);
			this.EditTab.SelectedIndexChanged += new global::System.EventHandler(this.EditTab_SelectedIndexChanged);
			this.GuiEditor.Controls.Add(this.EditorSystemPanel);
			this.GuiEditor.Controls.Add(this.GuiEditorTool);
			this.GuiEditor.Controls.Add(this.GUIEditorStatus);
			this.GuiEditor.Location = new global::System.Drawing.Point(4, 22);
			this.GuiEditor.Name = "GuiEditor";
			this.GuiEditor.Size = new global::System.Drawing.Size(477, 283);
			this.GuiEditor.TabIndex = 0;
			this.GuiEditor.Text = "グラフィカルデザイナ";
			this.GuiEditor.UseVisualStyleBackColor = true;
			this.EditorSystemPanel.BackColor = global::System.Drawing.SystemColors.ButtonFace;
			this.EditorSystemPanel.Controls.Add(this.MainDesigner);
			this.EditorSystemPanel.Controls.Add(this.GVirtScroll);
			this.EditorSystemPanel.Controls.Add(this.GHorzScroll);
			this.EditorSystemPanel.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.EditorSystemPanel.Location = new global::System.Drawing.Point(0, 25);
			this.EditorSystemPanel.Margin = new global::System.Windows.Forms.Padding(0);
			this.EditorSystemPanel.Name = "EditorSystemPanel";
			this.EditorSystemPanel.Size = new global::System.Drawing.Size(477, 231);
			this.EditorSystemPanel.TabIndex = 2;
			this.EditorSystemPanel.Resize += new global::System.EventHandler(this.EditorSystemPanel_Resize);
			this.GVirtScroll.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Right);
			this.GVirtScroll.Location = new global::System.Drawing.Point(457, 0);
			this.GVirtScroll.Margin = new global::System.Windows.Forms.Padding(0, 0, 0, 20);
			this.GVirtScroll.Name = "GVirtScroll";
			this.GVirtScroll.Size = new global::System.Drawing.Size(20, 211);
			this.GVirtScroll.TabIndex = 1;
			this.GVirtScroll.Scroll += new global::System.Windows.Forms.ScrollEventHandler(this.GVirtScroll_Scroll);
			this.GHorzScroll.Anchor = (global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.GHorzScroll.Location = new global::System.Drawing.Point(0, 211);
			this.GHorzScroll.Margin = new global::System.Windows.Forms.Padding(0, 0, 20, 0);
			this.GHorzScroll.Name = "GHorzScroll";
			this.GHorzScroll.Size = new global::System.Drawing.Size(457, 20);
			this.GHorzScroll.TabIndex = 2;
			this.GHorzScroll.Scroll += new global::System.Windows.Forms.ScrollEventHandler(this.GHorzScroll_Scroll);
			this.GuiEditorTool.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.ShowGrid,
				this.StageZoom,
				this.StageLayer,
				this.toolStripSeparator2,
				this.ItemUndo,
				this.ItemRedo,
				this.toolStripSeparator7,
				this.ItemCursor,
				this.ItemPen,
				this.ItemLine,
				this.ItemRect,
				this.ItemFill,
				this.toolStripSeparator3,
				this.GuiCut,
				this.GuiCopy,
				this.GuiPaste,
				this.toolStripSeparator6,
				this.ItemReload,
				this.toolStripSeparator8,
				this.GUIProjConfig,
				this.GUIProjTestRun
			});
			this.GuiEditorTool.Location = new global::System.Drawing.Point(0, 0);
			this.GuiEditorTool.Name = "GuiEditorTool";
			this.GuiEditorTool.Size = new global::System.Drawing.Size(477, 25);
			this.GuiEditorTool.TabIndex = 0;
			this.GuiEditorTool.Text = "toolStrip2";
			this.ShowGrid.CheckOnClick = true;
			this.ShowGrid.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ShowGrid.Image = global::MasaoPlus.Properties.Resources.grid;
			this.ShowGrid.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.ShowGrid.Name = "ShowGrid";
			this.ShowGrid.Size = new global::System.Drawing.Size(23, 22);
			this.ShowGrid.Text = "グリッドの表示";
			this.ShowGrid.Click += new global::System.EventHandler(this.ShowGrid_Click);
			this.StageZoom.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.StageZoom.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.StageZoom25,
				this.StageZoom50,
				this.StageZoom100,
				this.StageZoom200
			});
			this.StageZoom.Image = global::MasaoPlus.Properties.Resources.map_magnify;
			this.StageZoom.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.StageZoom.Name = "StageZoom";
			this.StageZoom.Size = new global::System.Drawing.Size(29, 22);
			this.StageZoom.Text = "表示倍率";
			this.StageZoom25.Name = "StageZoom25";
			this.StageZoom25.Size = new global::System.Drawing.Size(109, 22);
			this.StageZoom25.Text = "25%";
			this.StageZoom25.Click += new global::System.EventHandler(this.StageZoom25_Click);
			this.StageZoom50.Name = "StageZoom50";
			this.StageZoom50.Size = new global::System.Drawing.Size(109, 22);
			this.StageZoom50.Text = "50%";
			this.StageZoom50.Click += new global::System.EventHandler(this.StageZoom50_Click);
			this.StageZoom100.Name = "StageZoom100";
			this.StageZoom100.Size = new global::System.Drawing.Size(109, 22);
			this.StageZoom100.Text = "100%";
			this.StageZoom100.Click += new global::System.EventHandler(this.StageZoom100_Click);
			this.StageZoom200.Name = "StageZoom200";
			this.StageZoom200.Size = new global::System.Drawing.Size(109, 22);
			this.StageZoom200.Text = "200%";
			this.StageZoom200.Click += new global::System.EventHandler(this.StageZoom200_Click);
			this.StageLayer.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.StageLayer.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.PatternChipLayer,
				this.BackgroundLayer,
				this.toolStripMenuItem13,
				this.MDUnactiveLayer,
				this.MTUnactiveLayer
			});
			this.StageLayer.Image = global::MasaoPlus.Properties.Resources.layers;
			this.StageLayer.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.StageLayer.Name = "StageLayer";
			this.StageLayer.Size = new global::System.Drawing.Size(29, 22);
			this.StageLayer.Text = "レイヤーの変更";
			this.PatternChipLayer.Checked = true;
			this.PatternChipLayer.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.PatternChipLayer.Name = "PatternChipLayer";
			this.PatternChipLayer.ShortcutKeyDisplayString = "F7";
			this.PatternChipLayer.Size = new global::System.Drawing.Size(275, 22);
			this.PatternChipLayer.Text = "パターンチップレイヤー(&P)";
			this.PatternChipLayer.Click += new global::System.EventHandler(this.EditPatternChip_Click);
			this.BackgroundLayer.Name = "BackgroundLayer";
			this.BackgroundLayer.ShortcutKeyDisplayString = "F8";
			this.BackgroundLayer.Size = new global::System.Drawing.Size(275, 22);
			this.BackgroundLayer.Text = "背景チップレイヤー(&B)";
			this.BackgroundLayer.Click += new global::System.EventHandler(this.EditBackground_Click);
			this.toolStripMenuItem13.Name = "toolStripMenuItem13";
			this.toolStripMenuItem13.Size = new global::System.Drawing.Size(272, 6);
			this.MDUnactiveLayer.Name = "MDUnactiveLayer";
			this.MDUnactiveLayer.Size = new global::System.Drawing.Size(275, 22);
			this.MDUnactiveLayer.Text = "アクティブでないレイヤーも描画(&U)";
			this.MDUnactiveLayer.Click += new global::System.EventHandler(this.ViewUnactiveLayer_Click);
			this.MTUnactiveLayer.Enabled = false;
			this.MTUnactiveLayer.Name = "MTUnactiveLayer";
			this.MTUnactiveLayer.Size = new global::System.Drawing.Size(275, 22);
			this.MTUnactiveLayer.Text = "アクティブでないレイヤーを透過(&T)";
			this.MTUnactiveLayer.Click += new global::System.EventHandler(this.TransparentUnactiveLayer_Click);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new global::System.Drawing.Size(6, 25);
			this.ItemUndo.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ItemUndo.Enabled = false;
			this.ItemUndo.Image = global::MasaoPlus.Properties.Resources.undo;
			this.ItemUndo.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.ItemUndo.Name = "ItemUndo";
			this.ItemUndo.Size = new global::System.Drawing.Size(23, 22);
			this.ItemUndo.Text = "元に戻す";
			this.ItemUndo.Click += new global::System.EventHandler(this.ItemUndo_Click);
			this.ItemRedo.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ItemRedo.Enabled = false;
			this.ItemRedo.Image = global::MasaoPlus.Properties.Resources.redo;
			this.ItemRedo.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.ItemRedo.Name = "ItemRedo";
			this.ItemRedo.Size = new global::System.Drawing.Size(23, 22);
			this.ItemRedo.Text = "やり直し";
			this.ItemRedo.Click += new global::System.EventHandler(this.ItemRedo_Click);
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new global::System.Drawing.Size(6, 25);
			this.ItemCursor.Checked = true;
			this.ItemCursor.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.ItemCursor.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ItemCursor.Image = global::MasaoPlus.Properties.Resources.cursor;
			this.ItemCursor.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.ItemCursor.Name = "ItemCursor";
			this.ItemCursor.Size = new global::System.Drawing.Size(23, 22);
			this.ItemCursor.Text = "カーソル";
			this.ItemCursor.Click += new global::System.EventHandler(this.ItemCursor_Click);
			this.ItemPen.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ItemPen.Image = global::MasaoPlus.Properties.Resources.pencil;
			this.ItemPen.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.ItemPen.Name = "ItemPen";
			this.ItemPen.Size = new global::System.Drawing.Size(23, 22);
			this.ItemPen.Text = "ペン";
			this.ItemPen.Click += new global::System.EventHandler(this.ItemPen_Click);
			this.ItemLine.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ItemLine.Image = global::MasaoPlus.Properties.Resources.line;
			this.ItemLine.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.ItemLine.Name = "ItemLine";
			this.ItemLine.Size = new global::System.Drawing.Size(23, 22);
			this.ItemLine.Text = "直線";
			this.ItemLine.Click += new global::System.EventHandler(this.ItemLine_Click);
			this.ItemRect.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ItemRect.Image = global::MasaoPlus.Properties.Resources.square;
			this.ItemRect.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.ItemRect.Name = "ItemRect";
			this.ItemRect.Size = new global::System.Drawing.Size(23, 22);
			this.ItemRect.Text = "塗り潰し四角形";
			this.ItemRect.Click += new global::System.EventHandler(this.ItemRect_Click);
			this.ItemFill.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ItemFill.Image = global::MasaoPlus.Properties.Resources.paintcan;
			this.ItemFill.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.ItemFill.Name = "ItemFill";
			this.ItemFill.Size = new global::System.Drawing.Size(23, 22);
			this.ItemFill.Text = "領域塗り潰し";
			this.ItemFill.Click += new global::System.EventHandler(this.ItemFill_Click);
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new global::System.Drawing.Size(6, 25);
			this.GuiCut.CheckOnClick = true;
			this.GuiCut.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.GuiCut.Image = global::MasaoPlus.Properties.Resources.cut;
			this.GuiCut.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.GuiCut.Name = "GuiCut";
			this.GuiCut.Size = new global::System.Drawing.Size(23, 22);
			this.GuiCut.Text = "切り取り";
			this.GuiCut.Click += new global::System.EventHandler(this.GuiCut_Click);
			this.GuiCopy.CheckOnClick = true;
			this.GuiCopy.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.GuiCopy.Image = global::MasaoPlus.Properties.Resources.copy;
			this.GuiCopy.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.GuiCopy.Name = "GuiCopy";
			this.GuiCopy.Size = new global::System.Drawing.Size(23, 22);
			this.GuiCopy.Text = "コピー";
			this.GuiCopy.Click += new global::System.EventHandler(this.GuiCopy_Click);
			this.GuiPaste.CheckOnClick = true;
			this.GuiPaste.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.GuiPaste.Image = global::MasaoPlus.Properties.Resources.paste;
			this.GuiPaste.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.GuiPaste.Name = "GuiPaste";
			this.GuiPaste.Size = new global::System.Drawing.Size(23, 22);
			this.GuiPaste.Text = "貼り付け";
			this.GuiPaste.Click += new global::System.EventHandler(this.GuiPaste_Click);
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new global::System.Drawing.Size(6, 25);
			this.ItemReload.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ItemReload.Image = global::MasaoPlus.Properties.Resources.refresh;
			this.ItemReload.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.ItemReload.Name = "ItemReload";
			this.ItemReload.Size = new global::System.Drawing.Size(23, 22);
			this.ItemReload.Text = "ステージを再描画";
			this.ItemReload.Click += new global::System.EventHandler(this.ItemReload_Click);
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new global::System.Drawing.Size(6, 25);
			this.GUIProjConfig.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.GUIProjConfig.Image = global::MasaoPlus.Properties.Resources.map_edit;
			this.GUIProjConfig.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.GUIProjConfig.Name = "GUIProjConfig";
			this.GUIProjConfig.Size = new global::System.Drawing.Size(23, 22);
			this.GUIProjConfig.Text = "プロジェクトの設定";
			this.GUIProjConfig.Click += new global::System.EventHandler(this.GUIProjConfig_Click);
			this.GUIProjTestRun.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.GUIProjTestRun.Image = global::MasaoPlus.Properties.Resources.testrunstage;
			this.GUIProjTestRun.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.GUIProjTestRun.Name = "GUIProjTestRun";
			this.GUIProjTestRun.Size = new global::System.Drawing.Size(23, 22);
			this.GUIProjTestRun.Text = "このステージをテスト実行";
			this.GUIProjTestRun.Click += new global::System.EventHandler(this.ProjTestRun);
			this.GUIEditorStatus.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.ChipNavigator,
				this.GuiPositionInfo,
				this.CurrentStatus
			});
			this.GUIEditorStatus.Location = new global::System.Drawing.Point(0, 256);
			this.GUIEditorStatus.Name = "GUIEditorStatus";
			this.GUIEditorStatus.Size = new global::System.Drawing.Size(477, 27);
			this.GUIEditorStatus.SizingGrip = false;
			this.GUIEditorStatus.TabIndex = 1;
			this.GUIEditorStatus.Text = "statusStrip2";
			this.ChipNavigator.ForeColor = global::System.Drawing.SystemColors.ControlDarkDark;
			this.ChipNavigator.Image = global::MasaoPlus.Properties.Resources.stv_dot;
			this.ChipNavigator.Name = "ChipNavigator";
			this.ChipNavigator.Size = new global::System.Drawing.Size(16, 22);
			this.GuiPositionInfo.BorderSides = global::System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.GuiPositionInfo.Name = "GuiPositionInfo";
			this.GuiPositionInfo.Size = new global::System.Drawing.Size(30, 22);
			this.GuiPositionInfo.Text = "0,0";
			this.CurrentStatus.Name = "CurrentStatus";
			this.CurrentStatus.Size = new global::System.Drawing.Size(416, 22);
			this.CurrentStatus.Spring = true;
			this.CurrentStatus.Text = "完了";
			this.CurrentStatus.TextAlign = global::System.Drawing.ContentAlignment.MiddleLeft;
			this.TextEditor.Controls.Add(this.MainEditor);
			this.TextEditor.Location = new global::System.Drawing.Point(4, 22);
			this.TextEditor.Name = "TextEditor";
			this.TextEditor.Size = new global::System.Drawing.Size(477, 283);
			this.TextEditor.TabIndex = 1;
			this.TextEditor.Text = "テキストエディタ";
			this.TextEditor.UseVisualStyleBackColor = true;
			this.IntegrateTestrun.Controls.Add(this.IntegrateBrowser);
			this.IntegrateTestrun.Location = new global::System.Drawing.Point(4, 22);
			this.IntegrateTestrun.Name = "IntegrateTestrun";
			this.IntegrateTestrun.Padding = new global::System.Windows.Forms.Padding(3);
			this.IntegrateTestrun.Size = new global::System.Drawing.Size(477, 283);
			this.IntegrateTestrun.TabIndex = 2;
			this.IntegrateTestrun.Text = "テスト実行";
			this.IntegrateTestrun.UseVisualStyleBackColor = true;
			this.GUIMenu.Dock = global::System.Windows.Forms.DockStyle.None;
			this.GUIMenu.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.MFile,
				this.GMEdit,
				this.TMEdit,
				this.GMStage,
				this.LayerMenu,
				this.GMTool,
				this.BMView,
				this.MConfiguration,
				this.MHelp
			});
			this.GUIMenu.Location = new global::System.Drawing.Point(0, 0);
			this.GUIMenu.Name = "GUIMenu";
			this.GUIMenu.Size = new global::System.Drawing.Size(674, 26);
			this.GUIMenu.TabIndex = 0;
			this.GUIMenu.Text = "menuStrip1";
			this.MFile.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.MNew,
				this.他の方法で新規MToolStripMenuItem,
				this.toolStripMenuItem16,
				this.MOpen,
				this.toolStripMenuItem1,
				this.MSave,
				this.MSaveAs,
				this.toolStripMenuItem4,
				this.MOutput,
				this.toolStripMenuItem5,
				this.MExit
			});
			this.MFile.Name = "MFile";
			this.MFile.Size = new global::System.Drawing.Size(85, 22);
			this.MFile.Text = "ファイル(&F)";
			this.MNew.Image = global::MasaoPlus.Properties.Resources._new;
			this.MNew.Name = "MNew";
			this.MNew.ShortcutKeyDisplayString = "Ctrl+N";
			this.MNew.ShortcutKeys = (global::System.Windows.Forms.Keys)131150;
			this.MNew.Size = new global::System.Drawing.Size(202, 22);
			this.MNew.Text = "新規(&N)...";
			this.MNew.Click += new global::System.EventHandler(this.MNew_Click);
			this.他の方法で新規MToolStripMenuItem.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.MProjectInheritNew,
				this.MConvertHTML
			});
			this.他の方法で新規MToolStripMenuItem.Name = "他の方法で新規MToolStripMenuItem";
			this.他の方法で新規MToolStripMenuItem.Size = new global::System.Drawing.Size(202, 22);
			this.他の方法で新規MToolStripMenuItem.Text = "他の方法で新規(&M)";
			this.MProjectInheritNew.Name = "MProjectInheritNew";
			this.MProjectInheritNew.Size = new global::System.Drawing.Size(259, 22);
			this.MProjectInheritNew.Text = "プロジェクトを継承して新規(&I)...";
			this.MProjectInheritNew.Click += new global::System.EventHandler(this.MProjectInheritNew_Click);
			this.MConvertHTML.Name = "MConvertHTML";
			this.MConvertHTML.Size = new global::System.Drawing.Size(259, 22);
			this.MConvertHTML.Text = "HTMLを変換して新規(&C)...";
			this.MConvertHTML.Click += new global::System.EventHandler(this.MConvertHTML_Click);
			this.toolStripMenuItem16.Name = "toolStripMenuItem16";
			this.toolStripMenuItem16.Size = new global::System.Drawing.Size(199, 6);
			this.MOpen.Image = global::MasaoPlus.Properties.Resources.open;
			this.MOpen.Name = "MOpen";
			this.MOpen.ShortcutKeyDisplayString = "Ctrl+O";
			this.MOpen.ShortcutKeys = (global::System.Windows.Forms.Keys)131151;
			this.MOpen.Size = new global::System.Drawing.Size(202, 22);
			this.MOpen.Text = "開く(&O)...";
			this.MOpen.Click += new global::System.EventHandler(this.MOpen_Click);
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new global::System.Drawing.Size(199, 6);
			this.MSave.Image = global::MasaoPlus.Properties.Resources.save;
			this.MSave.Name = "MSave";
			this.MSave.ShortcutKeyDisplayString = "Ctrl+S";
			this.MSave.ShortcutKeys = (global::System.Windows.Forms.Keys)131155;
			this.MSave.Size = new global::System.Drawing.Size(202, 22);
			this.MSave.Text = "上書き保存(&S)";
			this.MSave.Click += new global::System.EventHandler(this.MSave_Click);
			this.MSaveAs.Name = "MSaveAs";
			this.MSaveAs.Size = new global::System.Drawing.Size(202, 22);
			this.MSaveAs.Text = "名前をつけて保存(&A)...";
			this.MSaveAs.Click += new global::System.EventHandler(this.MSaveAs_Click);
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new global::System.Drawing.Size(199, 6);
			this.MOutput.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.MWriteHTML,
				this.MWriteStagePicture
			});
			this.MOutput.Name = "MOutput";
			this.MOutput.Size = new global::System.Drawing.Size(202, 22);
			this.MOutput.Text = "出力(&O)";
			this.MWriteHTML.Image = global::MasaoPlus.Properties.Resources.page_white_code;
			this.MWriteHTML.Name = "MWriteHTML";
			this.MWriteHTML.Size = new global::System.Drawing.Size(213, 22);
			this.MWriteHTML.Text = "整形済みファイル(&H)...";
			this.MWriteHTML.Click += new global::System.EventHandler(this.MWriteHTML_Click);
			this.MWriteStagePicture.Image = global::MasaoPlus.Properties.Resources.stv_dot;
			this.MWriteStagePicture.Name = "MWriteStagePicture";
			this.MWriteStagePicture.Size = new global::System.Drawing.Size(213, 22);
			this.MWriteStagePicture.Text = "ステージ全体の画像(&P)...";
			this.MWriteStagePicture.Click += new global::System.EventHandler(this.MWriteStagePicture_Click);
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new global::System.Drawing.Size(199, 6);
			this.MExit.Name = "MExit";
			this.MExit.Size = new global::System.Drawing.Size(202, 22);
			this.MExit.Text = "終了(&X)";
			this.MExit.Click += new global::System.EventHandler(this.MExit_Click);
			this.GMEdit.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.GMUndo,
				this.GMRedo,
				this.toolStripMenuItem2,
				this.GMCut,
				this.GMCopy,
				this.GMPaste,
				this.toolStripMenuItem3,
				this.GMTestRun,
				this.GMAllTestrun
			});
			this.GMEdit.Name = "GMEdit";
			this.GMEdit.Size = new global::System.Drawing.Size(61, 22);
			this.GMEdit.Text = "編集(&E)";
			this.GMUndo.Image = global::MasaoPlus.Properties.Resources.undo;
			this.GMUndo.Name = "GMUndo";
			this.GMUndo.ShortcutKeyDisplayString = "Ctrl+Z";
			this.GMUndo.Size = new global::System.Drawing.Size(290, 22);
			this.GMUndo.Text = "元に戻す(&U)";
			this.GMUndo.Click += new global::System.EventHandler(this.GMUndo_Click);
			this.GMRedo.Image = global::MasaoPlus.Properties.Resources.redo;
			this.GMRedo.Name = "GMRedo";
			this.GMRedo.ShortcutKeyDisplayString = "Ctrl+Y";
			this.GMRedo.Size = new global::System.Drawing.Size(290, 22);
			this.GMRedo.Text = "やり直し(&R)";
			this.GMRedo.Click += new global::System.EventHandler(this.GMRedo_Click);
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new global::System.Drawing.Size(287, 6);
			this.GMCut.Image = global::MasaoPlus.Properties.Resources.cut;
			this.GMCut.Name = "GMCut";
			this.GMCut.ShortcutKeyDisplayString = "Ctrl+X";
			this.GMCut.Size = new global::System.Drawing.Size(290, 22);
			this.GMCut.Text = "切り取り(&X)";
			this.GMCut.Click += new global::System.EventHandler(this.GMCut_Click);
			this.GMCopy.Image = global::MasaoPlus.Properties.Resources.copy;
			this.GMCopy.Name = "GMCopy";
			this.GMCopy.ShortcutKeyDisplayString = "Ctrl+C";
			this.GMCopy.Size = new global::System.Drawing.Size(290, 22);
			this.GMCopy.Text = "コピー(&C)";
			this.GMCopy.Click += new global::System.EventHandler(this.GMCopy_Click);
			this.GMPaste.Image = global::MasaoPlus.Properties.Resources.paste;
			this.GMPaste.Name = "GMPaste";
			this.GMPaste.ShortcutKeyDisplayString = "Ctrl+V";
			this.GMPaste.Size = new global::System.Drawing.Size(290, 22);
			this.GMPaste.Text = "貼り付け(&V)";
			this.GMPaste.Click += new global::System.EventHandler(this.GMPaste_Click);
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new global::System.Drawing.Size(287, 6);
			this.GMTestRun.Image = global::MasaoPlus.Properties.Resources.testrunstage;
			this.GMTestRun.Name = "GMTestRun";
			this.GMTestRun.ShortcutKeyDisplayString = "Ctrl+Space";
			this.GMTestRun.Size = new global::System.Drawing.Size(290, 22);
			this.GMTestRun.Text = "テスト実行(&T)";
			this.GMTestRun.Click += new global::System.EventHandler(this.ProjTestRun);
			this.GMAllTestrun.Name = "GMAllTestrun";
			this.GMAllTestrun.ShortcutKeyDisplayString = "Ctrl+Alt+Space";
			this.GMAllTestrun.Size = new global::System.Drawing.Size(290, 22);
			this.GMAllTestrun.Text = "全体をテスト実行(&A)";
			this.GMAllTestrun.Click += new global::System.EventHandler(this.AllTestrun);
			this.TMEdit.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.TMUndo,
				this.TMRedo,
				this.toolStripSeparator10,
				this.TMCut,
				this.TMCopy,
				this.TMPaste,
				this.toolStripSeparator11,
				this.TMTestRun,
				this.TMAllTestrun
			});
			this.TMEdit.Name = "TMEdit";
			this.TMEdit.Size = new global::System.Drawing.Size(61, 22);
			this.TMEdit.Text = "編集(&E)";
			this.TMEdit.Visible = false;
			this.TMUndo.Image = global::MasaoPlus.Properties.Resources.undo;
			this.TMUndo.Name = "TMUndo";
			this.TMUndo.ShortcutKeyDisplayString = "Ctrl+Z";
			this.TMUndo.Size = new global::System.Drawing.Size(290, 22);
			this.TMUndo.Text = "元に戻す(&U)";
			this.TMUndo.Click += new global::System.EventHandler(this.TMUndo_Click);
			this.TMRedo.Image = global::MasaoPlus.Properties.Resources.redo;
			this.TMRedo.Name = "TMRedo";
			this.TMRedo.ShortcutKeyDisplayString = "Ctrl+Y";
			this.TMRedo.Size = new global::System.Drawing.Size(290, 22);
			this.TMRedo.Text = "やり直し(&R)";
			this.TMRedo.Click += new global::System.EventHandler(this.TMRedo_Click);
			this.toolStripSeparator10.Name = "toolStripSeparator10";
			this.toolStripSeparator10.Size = new global::System.Drawing.Size(287, 6);
			this.TMCut.Image = global::MasaoPlus.Properties.Resources.cut;
			this.TMCut.Name = "TMCut";
			this.TMCut.ShortcutKeyDisplayString = "Ctrl+X";
			this.TMCut.Size = new global::System.Drawing.Size(290, 22);
			this.TMCut.Text = "切り取り(&X)";
			this.TMCut.Click += new global::System.EventHandler(this.TextCut_Click);
			this.TMCopy.Image = global::MasaoPlus.Properties.Resources.copy;
			this.TMCopy.Name = "TMCopy";
			this.TMCopy.ShortcutKeyDisplayString = "Ctrl+C";
			this.TMCopy.Size = new global::System.Drawing.Size(290, 22);
			this.TMCopy.Text = "コピー(&C)";
			this.TMCopy.Click += new global::System.EventHandler(this.TextCopy_Click);
			this.TMPaste.Image = global::MasaoPlus.Properties.Resources.paste;
			this.TMPaste.Name = "TMPaste";
			this.TMPaste.ShortcutKeyDisplayString = "Ctrl+V";
			this.TMPaste.Size = new global::System.Drawing.Size(290, 22);
			this.TMPaste.Text = "貼り付け(&V)";
			this.TMPaste.Click += new global::System.EventHandler(this.TextPaste_Click);
			this.toolStripSeparator11.Name = "toolStripSeparator11";
			this.toolStripSeparator11.Size = new global::System.Drawing.Size(287, 6);
			this.TMTestRun.Image = global::MasaoPlus.Properties.Resources.testrunstage;
			this.TMTestRun.Name = "TMTestRun";
			this.TMTestRun.ShortcutKeyDisplayString = "Ctrl+Space";
			this.TMTestRun.Size = new global::System.Drawing.Size(290, 22);
			this.TMTestRun.Text = "テスト実行(&T)";
			this.TMTestRun.Click += new global::System.EventHandler(this.ProjTestRun);
			this.TMAllTestrun.Name = "TMAllTestrun";
			this.TMAllTestrun.ShortcutKeyDisplayString = "Ctrl+Alt+Space";
			this.TMAllTestrun.Size = new global::System.Drawing.Size(290, 22);
			this.TMAllTestrun.Text = "全体をテスト実行(&A)";
			this.TMAllTestrun.Click += new global::System.EventHandler(this.AllTestrun);
			this.GMStage.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.MEditStage1,
				this.MEditStage2,
				this.MEditStage3,
				this.MEditStage4,
				this.toolStripMenuItem18,
				this.MEditMap,
				this.toolStripMenuItem19,
				this.MSConfig
			});
			this.GMStage.Name = "GMStage";
			this.GMStage.Size = new global::System.Drawing.Size(86, 22);
			this.GMStage.Text = "ステージ(&S)";
			this.MEditStage1.Checked = true;
			this.MEditStage1.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.MEditStage1.Name = "MEditStage1";
			this.MEditStage1.ShortcutKeyDisplayString = "F1";
			this.MEditStage1.Size = new global::System.Drawing.Size(166, 22);
			this.MEditStage1.Text = "ステージ&1";
			this.MEditStage1.Click += new global::System.EventHandler(this.MEditStage1_Click);
			this.MEditStage2.Name = "MEditStage2";
			this.MEditStage2.ShortcutKeyDisplayString = "F2";
			this.MEditStage2.Size = new global::System.Drawing.Size(166, 22);
			this.MEditStage2.Text = "ステージ&2";
			this.MEditStage2.Click += new global::System.EventHandler(this.MEditStage2_Click);
			this.MEditStage3.Name = "MEditStage3";
			this.MEditStage3.ShortcutKeyDisplayString = "F3";
			this.MEditStage3.Size = new global::System.Drawing.Size(166, 22);
			this.MEditStage3.Text = "ステージ&3";
			this.MEditStage3.Click += new global::System.EventHandler(this.MEditStage3_Click);
			this.MEditStage4.Name = "MEditStage4";
			this.MEditStage4.ShortcutKeyDisplayString = "F4";
			this.MEditStage4.Size = new global::System.Drawing.Size(166, 22);
			this.MEditStage4.Text = "ステージ&4";
			this.MEditStage4.Click += new global::System.EventHandler(this.MEditStage4_Click);
			this.toolStripMenuItem18.Name = "toolStripMenuItem18";
			this.toolStripMenuItem18.Size = new global::System.Drawing.Size(163, 6);
			this.MEditMap.Name = "MEditMap";
			this.MEditMap.ShortcutKeyDisplayString = "F6";
			this.MEditMap.Size = new global::System.Drawing.Size(166, 22);
			this.MEditMap.Text = "地図画面(&M)";
			this.MEditMap.Click += new global::System.EventHandler(this.MEditMap_Click);
			this.toolStripMenuItem19.Name = "toolStripMenuItem19";
			this.toolStripMenuItem19.Size = new global::System.Drawing.Size(163, 6);
			this.MSConfig.Name = "MSConfig";
			this.MSConfig.Size = new global::System.Drawing.Size(166, 22);
			this.MSConfig.Text = "設定(&C)...";
			this.MSConfig.Click += new global::System.EventHandler(this.ProjectConfig_Click);
			this.LayerMenu.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.EditPatternChip,
				this.EditBackground,
				this.toolStripMenuItem8,
				this.ViewUnactiveLayer,
				this.TransparentUnactiveLayer
			});
			this.LayerMenu.Name = "LayerMenu";
			this.LayerMenu.Size = new global::System.Drawing.Size(85, 22);
			this.LayerMenu.Text = "レイヤー(&L)";
			this.EditPatternChip.Checked = true;
			this.EditPatternChip.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.EditPatternChip.Name = "EditPatternChip";
			this.EditPatternChip.ShortcutKeyDisplayString = "F7";
			this.EditPatternChip.Size = new global::System.Drawing.Size(275, 22);
			this.EditPatternChip.Text = "パターンチップレイヤー(&P)";
			this.EditPatternChip.Click += new global::System.EventHandler(this.EditPatternChip_Click);
			this.EditBackground.Name = "EditBackground";
			this.EditBackground.ShortcutKeyDisplayString = "F8";
			this.EditBackground.Size = new global::System.Drawing.Size(275, 22);
			this.EditBackground.Text = "背景チップレイヤー(&B)";
			this.EditBackground.Click += new global::System.EventHandler(this.EditBackground_Click);
			this.toolStripMenuItem8.Name = "toolStripMenuItem8";
			this.toolStripMenuItem8.Size = new global::System.Drawing.Size(272, 6);
			this.ViewUnactiveLayer.Name = "ViewUnactiveLayer";
			this.ViewUnactiveLayer.Size = new global::System.Drawing.Size(275, 22);
			this.ViewUnactiveLayer.Text = "アクティブでないレイヤーも描画(&U)";
			this.ViewUnactiveLayer.Click += new global::System.EventHandler(this.ViewUnactiveLayer_Click);
			this.TransparentUnactiveLayer.Enabled = false;
			this.TransparentUnactiveLayer.Name = "TransparentUnactiveLayer";
			this.TransparentUnactiveLayer.Size = new global::System.Drawing.Size(275, 22);
			this.TransparentUnactiveLayer.Text = "アクティブでないレイヤーを透過(&T)";
			this.TransparentUnactiveLayer.Click += new global::System.EventHandler(this.TransparentUnactiveLayer_Click);
			this.GMTool.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.GMZoomIndexes,
				this.GMShowGrid,
				this.GMRedraw,
				this.toolStripMenuItem7,
				this.GMCursor,
				this.GMPen,
				this.GMLine,
				this.GMRect,
				this.GMFill,
				this.toolStripMenuItem17,
				this.MExEdit,
				this.ShowOverView
			});
			this.GMTool.Name = "GMTool";
			this.GMTool.Size = new global::System.Drawing.Size(74, 22);
			this.GMTool.Text = "ツール(&T)";
			this.GMZoomIndexes.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.GMZoom25,
				this.GMZoom50,
				this.GMZoom100,
				this.GMZoom200
			});
			this.GMZoomIndexes.Image = global::MasaoPlus.Properties.Resources.map_magnify;
			this.GMZoomIndexes.Name = "GMZoomIndexes";
			this.GMZoomIndexes.Size = new global::System.Drawing.Size(321, 22);
			this.GMZoomIndexes.Text = "表示倍率(&Z)";
			this.GMZoom25.Name = "GMZoom25";
			this.GMZoom25.ShortcutKeyDisplayString = "F9";
			this.GMZoom25.Size = new global::System.Drawing.Size(138, 22);
			this.GMZoom25.Text = "25%";
			this.GMZoom25.Click += new global::System.EventHandler(this.StageZoom25_Click);
			this.GMZoom50.Name = "GMZoom50";
			this.GMZoom50.ShortcutKeyDisplayString = "F10";
			this.GMZoom50.Size = new global::System.Drawing.Size(138, 22);
			this.GMZoom50.Text = "50%";
			this.GMZoom50.Click += new global::System.EventHandler(this.StageZoom50_Click);
			this.GMZoom100.Name = "GMZoom100";
			this.GMZoom100.ShortcutKeyDisplayString = "F11";
			this.GMZoom100.Size = new global::System.Drawing.Size(138, 22);
			this.GMZoom100.Text = "100%";
			this.GMZoom100.Click += new global::System.EventHandler(this.StageZoom100_Click);
			this.GMZoom200.Name = "GMZoom200";
			this.GMZoom200.ShortcutKeyDisplayString = "F12";
			this.GMZoom200.Size = new global::System.Drawing.Size(138, 22);
			this.GMZoom200.Text = "200%";
			this.GMZoom200.Click += new global::System.EventHandler(this.StageZoom200_Click);
			this.GMShowGrid.CheckOnClick = true;
			this.GMShowGrid.Image = global::MasaoPlus.Properties.Resources.grid;
			this.GMShowGrid.Name = "GMShowGrid";
			this.GMShowGrid.ShortcutKeyDisplayString = "Ctrl+G";
			this.GMShowGrid.Size = new global::System.Drawing.Size(321, 22);
			this.GMShowGrid.Text = "グリッド(&G)";
			this.GMShowGrid.Click += new global::System.EventHandler(this.GMShowGrid_Click);
			this.GMRedraw.Image = global::MasaoPlus.Properties.Resources.refresh;
			this.GMRedraw.Name = "GMRedraw";
			this.GMRedraw.ShortcutKeyDisplayString = "F5";
			this.GMRedraw.ShortcutKeys = global::System.Windows.Forms.Keys.F5;
			this.GMRedraw.Size = new global::System.Drawing.Size(321, 22);
			this.GMRedraw.Text = "描画の更新(&R)";
			this.GMRedraw.Click += new global::System.EventHandler(this.ItemReload_Click);
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Size = new global::System.Drawing.Size(318, 6);
			this.GMCursor.Checked = true;
			this.GMCursor.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.GMCursor.Image = global::MasaoPlus.Properties.Resources.cursor;
			this.GMCursor.Name = "GMCursor";
			this.GMCursor.ShortcutKeyDisplayString = "C";
			this.GMCursor.Size = new global::System.Drawing.Size(321, 22);
			this.GMCursor.Text = "カーソル編集";
			this.GMCursor.Click += new global::System.EventHandler(this.ItemCursor_Click);
			this.GMPen.Image = global::MasaoPlus.Properties.Resources.pencil;
			this.GMPen.Name = "GMPen";
			this.GMPen.ShortcutKeyDisplayString = "D";
			this.GMPen.Size = new global::System.Drawing.Size(321, 22);
			this.GMPen.Text = "ペン編集";
			this.GMPen.Click += new global::System.EventHandler(this.ItemPen_Click);
			this.GMLine.Image = global::MasaoPlus.Properties.Resources.line;
			this.GMLine.Name = "GMLine";
			this.GMLine.ShortcutKeyDisplayString = "S";
			this.GMLine.Size = new global::System.Drawing.Size(321, 22);
			this.GMLine.Text = "線描画編集";
			this.GMLine.Click += new global::System.EventHandler(this.ItemLine_Click);
			this.GMRect.Image = global::MasaoPlus.Properties.Resources.square;
			this.GMRect.Name = "GMRect";
			this.GMRect.ShortcutKeyDisplayString = "R";
			this.GMRect.Size = new global::System.Drawing.Size(321, 22);
			this.GMRect.Text = "四角塗り潰し編集";
			this.GMRect.Click += new global::System.EventHandler(this.ItemRect_Click);
			this.GMFill.Image = global::MasaoPlus.Properties.Resources.paintcan;
			this.GMFill.Name = "GMFill";
			this.GMFill.ShortcutKeyDisplayString = "F";
			this.GMFill.Size = new global::System.Drawing.Size(321, 22);
			this.GMFill.Text = "塗り潰し編集";
			this.GMFill.Click += new global::System.EventHandler(this.ItemFill_Click);
			this.toolStripMenuItem17.Name = "toolStripMenuItem17";
			this.toolStripMenuItem17.Size = new global::System.Drawing.Size(318, 6);
			this.MExEdit.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.MStageRev
			});
			this.MExEdit.Name = "MExEdit";
			this.MExEdit.Size = new global::System.Drawing.Size(321, 22);
			this.MExEdit.Text = "拡張編集(&E)";
			this.MStageRev.Name = "MStageRev";
			this.MStageRev.Size = new global::System.Drawing.Size(202, 22);
			this.MStageRev.Text = "ステージの左右反転(&R)";
			this.MStageRev.Click += new global::System.EventHandler(this.MStageRev_Click);
			this.ShowOverView.Name = "ShowOverView";
			this.ShowOverView.ShortcutKeyDisplayString = "(Space:クイックビュー)";
			this.ShowOverView.Size = new global::System.Drawing.Size(321, 22);
			this.ShowOverView.Text = "オーバービュー(&O)";
			this.ShowOverView.Click += new global::System.EventHandler(this.ShowOverView_Click);
			this.BMView.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.BMReload,
				this.BMBrowser,
				this.toolStripMenuItem6,
				this.BMTestEnd
			});
			this.BMView.Name = "BMView";
			this.BMView.Size = new global::System.Drawing.Size(62, 22);
			this.BMView.Text = "表示(&V)";
			this.BMView.Visible = false;
			this.BMReload.Image = global::MasaoPlus.Properties.Resources.refresh;
			this.BMReload.Name = "BMReload";
			this.BMReload.ShortcutKeys = global::System.Windows.Forms.Keys.F5;
			this.BMReload.Size = new global::System.Drawing.Size(236, 22);
			this.BMReload.Text = "テストプレイの再実行(&R)";
			this.BMBrowser.Image = global::MasaoPlus.Properties.Resources.web;
			this.BMBrowser.Name = "BMBrowser";
			this.BMBrowser.Size = new global::System.Drawing.Size(236, 22);
			this.BMBrowser.Text = "既定のブラウザで実行";
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new global::System.Drawing.Size(233, 6);
			this.BMTestEnd.Image = global::MasaoPlus.Properties.Resources.map_edit;
			this.BMTestEnd.Name = "BMTestEnd";
			this.BMTestEnd.ShortcutKeyDisplayString = "Esc";
			this.BMTestEnd.Size = new global::System.Drawing.Size(236, 22);
			this.BMTestEnd.Text = "エディタに戻る";
			this.MConfiguration.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.MProjectConfig,
				this.MSysConfig,
				this.toolStripMenuItem14,
				this.MResetProjRuntime,
				this.toolStripMenuItem20,
				this.MReloadImage
			});
			this.MConfiguration.Name = "MConfiguration";
			this.MConfiguration.Size = new global::System.Drawing.Size(62, 22);
			this.MConfiguration.Text = "設定(&C)";
			this.MProjectConfig.Image = global::MasaoPlus.Properties.Resources.map_edit;
			this.MProjectConfig.Name = "MProjectConfig";
			this.MProjectConfig.ShortcutKeyDisplayString = "Ctrl+F10";
			this.MProjectConfig.Size = new global::System.Drawing.Size(334, 22);
			this.MProjectConfig.Text = "プロジェクト設定(&C)...";
			this.MProjectConfig.Click += new global::System.EventHandler(this.ProjectConfig_Click);
			this.MSysConfig.Image = global::MasaoPlus.Properties.Resources.cog;
			this.MSysConfig.Name = "MSysConfig";
			this.MSysConfig.ShortcutKeyDisplayString = "Ctrl+F11";
			this.MSysConfig.Size = new global::System.Drawing.Size(334, 22);
			this.MSysConfig.Text = "エディタの設定(&E)...";
			this.MSysConfig.Click += new global::System.EventHandler(this.MSysConfig_Click);
			this.toolStripMenuItem14.Name = "toolStripMenuItem14";
			this.toolStripMenuItem14.Size = new global::System.Drawing.Size(331, 6);
			this.MResetProjRuntime.Name = "MResetProjRuntime";
			this.MResetProjRuntime.Size = new global::System.Drawing.Size(334, 22);
			this.MResetProjRuntime.Text = "現在のプロジェクトのランタイムを再設定(&R)...";
			this.MResetProjRuntime.Click += new global::System.EventHandler(this.MResetProjRuntime_Click);
			this.toolStripMenuItem20.Name = "toolStripMenuItem20";
			this.toolStripMenuItem20.Size = new global::System.Drawing.Size(331, 6);
			this.MReloadImage.Image = global::MasaoPlus.Properties.Resources.refresh;
			this.MReloadImage.Name = "MReloadImage";
			this.MReloadImage.Size = new global::System.Drawing.Size(334, 22);
			this.MReloadImage.Text = "チップ画像のリロード(&R)...";
			this.MReloadImage.Click += new global::System.EventHandler(this.MReloadImage_Click);
			this.MHelp.DropDownItems.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.InstalledRuntime,
				this.toolStripMenuItem15,
				this.MUpdateApp,
				this.MVersion
			});
			this.MHelp.Name = "MHelp";
			this.MHelp.Size = new global::System.Drawing.Size(75, 22);
			this.MHelp.Text = "ヘルプ(&H)";
			this.InstalledRuntime.Image = global::MasaoPlus.Properties.Resources.world_add;
			this.InstalledRuntime.Name = "InstalledRuntime";
			this.InstalledRuntime.Size = new global::System.Drawing.Size(226, 22);
			this.InstalledRuntime.Text = "ランタイムマネージャ(&R)...";
			this.InstalledRuntime.Click += new global::System.EventHandler(this.InstalledRuntime_Click);
			this.toolStripMenuItem15.Name = "toolStripMenuItem15";
			this.toolStripMenuItem15.Size = new global::System.Drawing.Size(223, 6);
			this.MUpdateApp.Image = global::MasaoPlus.Properties.Resources.refresh;
			this.MUpdateApp.Name = "MUpdateApp";
			this.MUpdateApp.Size = new global::System.Drawing.Size(226, 22);
			this.MUpdateApp.Text = "最新版へアップデート(&U)";
			this.MUpdateApp.Click += new global::System.EventHandler(this.MUpdateApp_Click);
			this.MVersion.Image = global::MasaoPlus.Properties.Resources.side1;
			this.MVersion.Name = "MVersion";
			this.MVersion.Size = new global::System.Drawing.Size(226, 22);
			this.MVersion.Text = "バージョン情報(&A)...";
			this.MVersion.Click += new global::System.EventHandler(this.MVersion_Click);
			this.MainToolbar.Dock = global::System.Windows.Forms.DockStyle.None;
			this.MainToolbar.Items.AddRange(new global::System.Windows.Forms.ToolStripItem[]
			{
				this.MTNew,
				this.MTOpen,
				this.MTSave,
				this.toolStripSeparator1,
				this.MTImgOut,
				this.MTOutput,
				this.toolStripSeparator4,
				this.MTEditConfig,
				this.toolStripSeparator5,
				this.MTVersion
			});
			this.MainToolbar.Location = new global::System.Drawing.Point(0, 26);
			this.MainToolbar.Name = "MainToolbar";
			this.MainToolbar.Size = new global::System.Drawing.Size(674, 25);
			this.MainToolbar.Stretch = true;
			this.MainToolbar.TabIndex = 1;
			this.MTNew.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.MTNew.Image = global::MasaoPlus.Properties.Resources._new;
			this.MTNew.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.MTNew.Name = "MTNew";
			this.MTNew.Size = new global::System.Drawing.Size(23, 22);
			this.MTNew.Text = "新規作成";
			this.MTNew.Click += new global::System.EventHandler(this.MNew_Click);
			this.MTOpen.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.MTOpen.Image = global::MasaoPlus.Properties.Resources.open;
			this.MTOpen.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.MTOpen.Name = "MTOpen";
			this.MTOpen.Size = new global::System.Drawing.Size(23, 22);
			this.MTOpen.Text = "プロジェクトを開く";
			this.MTOpen.Click += new global::System.EventHandler(this.MOpen_Click);
			this.MTSave.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.MTSave.Image = global::MasaoPlus.Properties.Resources.save;
			this.MTSave.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.MTSave.Name = "MTSave";
			this.MTSave.Size = new global::System.Drawing.Size(23, 22);
			this.MTSave.Text = "上書き保存";
			this.MTSave.Click += new global::System.EventHandler(this.MSave_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new global::System.Drawing.Size(6, 25);
			this.MTImgOut.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.MTImgOut.Image = global::MasaoPlus.Properties.Resources.stv_dot;
			this.MTImgOut.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.MTImgOut.Name = "MTImgOut";
			this.MTImgOut.Size = new global::System.Drawing.Size(23, 22);
			this.MTImgOut.Text = "ステージ全体の画像を保存";
			this.MTImgOut.Click += new global::System.EventHandler(this.MWriteStagePicture_Click);
			this.MTOutput.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.MTOutput.Image = global::MasaoPlus.Properties.Resources.page_white_code;
			this.MTOutput.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.MTOutput.Name = "MTOutput";
			this.MTOutput.Size = new global::System.Drawing.Size(23, 22);
			this.MTOutput.Text = "HTML書き出し";
			this.MTOutput.Click += new global::System.EventHandler(this.MWriteHTML_Click);
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new global::System.Drawing.Size(6, 25);
			this.MTEditConfig.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.MTEditConfig.Image = global::MasaoPlus.Properties.Resources.cog;
			this.MTEditConfig.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.MTEditConfig.Name = "MTEditConfig";
			this.MTEditConfig.Size = new global::System.Drawing.Size(23, 22);
			this.MTEditConfig.Text = "エディタの設定";
			this.MTEditConfig.Click += new global::System.EventHandler(this.MSysConfig_Click);
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new global::System.Drawing.Size(6, 25);
			this.MTVersion.DisplayStyle = global::System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.MTVersion.Image = global::MasaoPlus.Properties.Resources.side1;
			this.MTVersion.ImageTransparentColor = global::System.Drawing.Color.Magenta;
			this.MTVersion.Name = "MTVersion";
			this.MTVersion.Size = new global::System.Drawing.Size(23, 22);
			this.MTVersion.Text = "バージョン情報";
			this.MTVersion.Click += new global::System.EventHandler(this.MVersion_Click);
			this.toolStripMenuItem9.Checked = true;
			this.toolStripMenuItem9.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem9.Name = "toolStripMenuItem9";
			this.toolStripMenuItem9.ShortcutKeyDisplayString = "F3";
			this.toolStripMenuItem9.Size = new global::System.Drawing.Size(247, 22);
			this.toolStripMenuItem9.Text = "パターンチップレイヤー(&P)";
			this.toolStripMenuItem10.Name = "toolStripMenuItem10";
			this.toolStripMenuItem10.ShortcutKeyDisplayString = "F4";
			this.toolStripMenuItem10.Size = new global::System.Drawing.Size(247, 22);
			this.toolStripMenuItem10.Text = "背景チップレイヤー(&B)";
			this.toolStripMenuItem11.Checked = true;
			this.toolStripMenuItem11.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem11.Name = "toolStripMenuItem11";
			this.toolStripMenuItem11.ShortcutKeyDisplayString = "F3";
			this.toolStripMenuItem11.Size = new global::System.Drawing.Size(247, 22);
			this.toolStripMenuItem11.Text = "パターンチップレイヤー(&P)";
			this.toolStripMenuItem12.Name = "toolStripMenuItem12";
			this.toolStripMenuItem12.ShortcutKeyDisplayString = "F4";
			this.toolStripMenuItem12.Size = new global::System.Drawing.Size(247, 22);
			this.toolStripMenuItem12.Text = "背景チップレイヤー(&B)";
			this.GuiChipList.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.GuiChipList.Location = new global::System.Drawing.Point(0, 20);
			this.GuiChipList.Name = "GuiChipList";
			this.GuiChipList.SelectedIndex = 0;
			this.GuiChipList.Size = new global::System.Drawing.Size(171, 213);
			this.GuiChipList.TabIndex = 2;
			this.GuiChipList.Visible = false;
			this.MasaoConfigList.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.MasaoConfigList.Location = new global::System.Drawing.Point(3, 3);
			this.MasaoConfigList.Name = "MasaoConfigList";
			this.MasaoConfigList.Size = new global::System.Drawing.Size(171, 277);
			this.MasaoConfigList.TabIndex = 0;
			this.MainDesigner.BorderStyle = global::System.Windows.Forms.BorderStyle.FixedSingle;
			this.MainDesigner.CopyPaste = global::MasaoPlus.GUIDesigner.CopyPasteTool.None;
			this.MainDesigner.CurrentTool = global::MasaoPlus.GUIDesigner.EditTool.Cursor;
			this.MainDesigner.Location = new global::System.Drawing.Point(0, 0);
			this.MainDesigner.Margin = new global::System.Windows.Forms.Padding(0);
			this.MainDesigner.Name = "MainDesigner";
			this.MainDesigner.Size = new global::System.Drawing.Size(581, 236);
			this.MainDesigner.TabIndex = 3;
			this.MainDesigner.PreviewKeyDown += new global::System.Windows.Forms.PreviewKeyDownEventHandler(this.MainDesigner_PreviewKeyDown);
			this.MainDesigner.KeyUp += new global::System.Windows.Forms.KeyEventHandler(this.MainDesigner_KeyUp);
			this.MainDesigner.KeyDown += new global::System.Windows.Forms.KeyEventHandler(this.MainDesigner_KeyDown);
			this.MainEditor.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.MainEditor.Location = new global::System.Drawing.Point(0, 0);
			this.MainEditor.Name = "MainEditor";
			this.MainEditor.Size = new global::System.Drawing.Size(477, 283);
			this.MainEditor.TabIndex = 0;
			this.IntegrateBrowser.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.IntegrateBrowser.Location = new global::System.Drawing.Point(3, 3);
			this.IntegrateBrowser.Name = "IntegrateBrowser";
			this.IntegrateBrowser.Size = new global::System.Drawing.Size(471, 277);
			this.IntegrateBrowser.TabIndex = 1;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(674, 360);
			base.Controls.Add(this.MainDock);
			base.Icon = (global::System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.KeyPreview = true;
			base.MainMenuStrip = this.GUIMenu;
			base.Name = "MainWindow";
			this.Text = " Side";
			base.Load += new global::System.EventHandler(this.MainWindow_Load);
			base.Shown += new global::System.EventHandler(this.MainWindow_Shown);
			base.Activated += new global::System.EventHandler(this.MainWindow_Activated);
			base.FormClosed += new global::System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
			base.FormClosing += new global::System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
			base.KeyDown += new global::System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
			this.MainDock.ContentPanel.ResumeLayout(false);
			this.MainDock.TopToolStripPanel.ResumeLayout(false);
			this.MainDock.TopToolStripPanel.PerformLayout();
			this.MainDock.ResumeLayout(false);
			this.MainDock.PerformLayout();
			this.MainSplit.Panel1.ResumeLayout(false);
			this.MainSplit.Panel2.ResumeLayout(false);
			this.MainSplit.ResumeLayout(false);
			this.SideTab.ResumeLayout(false);
			this.ChipPage.ResumeLayout(false);
			this.ItemSpliter.Panel1.ResumeLayout(false);
			this.ItemSpliter.Panel1.PerformLayout();
			this.ItemSpliter.Panel2.ResumeLayout(false);
			this.ItemSpliter.ResumeLayout(false);
			((global::System.ComponentModel.ISupportInitialize)this.ChipImage).EndInit();
			this.PropertyTab.ResumeLayout(false);
			this.EditTab.ResumeLayout(false);
			this.GuiEditor.ResumeLayout(false);
			this.GuiEditor.PerformLayout();
			this.EditorSystemPanel.ResumeLayout(false);
			this.GuiEditorTool.ResumeLayout(false);
			this.GuiEditorTool.PerformLayout();
			this.GUIEditorStatus.ResumeLayout(false);
			this.GUIEditorStatus.PerformLayout();
			this.TextEditor.ResumeLayout(false);
			this.IntegrateTestrun.ResumeLayout(false);
			this.GUIMenu.ResumeLayout(false);
			this.GUIMenu.PerformLayout();
			this.MainToolbar.ResumeLayout(false);
			this.MainToolbar.PerformLayout();
			base.ResumeLayout(false);
		}

		// Token: 0x04000198 RID: 408
		private global::System.ComponentModel.IContainer components;

		// Token: 0x04000199 RID: 409
		private global::System.Windows.Forms.ToolStripContainer MainDock;

		// Token: 0x0400019A RID: 410
		private global::System.Windows.Forms.MenuStrip GUIMenu;

		// Token: 0x0400019B RID: 411
		private global::System.Windows.Forms.ToolStripMenuItem MFile;

		// Token: 0x0400019C RID: 412
		private global::System.Windows.Forms.ToolStripMenuItem MOpen;

		// Token: 0x0400019D RID: 413
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;

		// Token: 0x0400019E RID: 414
		private global::System.Windows.Forms.ToolStripMenuItem MExit;

		// Token: 0x0400019F RID: 415
		private global::System.Windows.Forms.ToolStripMenuItem GMEdit;

		// Token: 0x040001A0 RID: 416
		private global::System.Windows.Forms.ToolStripMenuItem GMUndo;

		// Token: 0x040001A1 RID: 417
		private global::System.Windows.Forms.ToolStripMenuItem GMRedo;

		// Token: 0x040001A2 RID: 418
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;

		// Token: 0x040001A3 RID: 419
		private global::System.Windows.Forms.ToolStripMenuItem GMCut;

		// Token: 0x040001A4 RID: 420
		private global::System.Windows.Forms.ToolStripMenuItem GMCopy;

		// Token: 0x040001A5 RID: 421
		private global::System.Windows.Forms.ToolStripMenuItem GMPaste;

		// Token: 0x040001A6 RID: 422
		private global::System.Windows.Forms.ToolStripMenuItem MConfiguration;

		// Token: 0x040001A7 RID: 423
		private global::System.Windows.Forms.ToolStripMenuItem MSysConfig;

		// Token: 0x040001A8 RID: 424
		private global::System.Windows.Forms.ToolStripMenuItem MHelp;

		// Token: 0x040001A9 RID: 425
		private global::System.Windows.Forms.ToolStripMenuItem MVersion;

		// Token: 0x040001AA RID: 426
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;

		// Token: 0x040001AB RID: 427
		private global::System.Windows.Forms.ToolStripMenuItem MSave;

		// Token: 0x040001AC RID: 428
		private global::System.Windows.Forms.ToolStripMenuItem MSaveAs;

		// Token: 0x040001AD RID: 429
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;

		// Token: 0x040001AE RID: 430
		private global::System.Windows.Forms.ToolStripMenuItem MOutput;

		// Token: 0x040001AF RID: 431
		private global::System.Windows.Forms.ToolStripMenuItem MWriteHTML;

		// Token: 0x040001B0 RID: 432
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;

		// Token: 0x040001B1 RID: 433
		private global::System.Windows.Forms.ToolStripMenuItem MNew;

		// Token: 0x040001B2 RID: 434
		private global::System.Windows.Forms.ToolStripMenuItem GMTestRun;

		// Token: 0x040001B3 RID: 435
		private global::System.Windows.Forms.ToolStripMenuItem GMTool;

		// Token: 0x040001B4 RID: 436
		private global::System.Windows.Forms.ToolStripMenuItem GMRedraw;

		// Token: 0x040001B5 RID: 437
		private global::System.Windows.Forms.ToolStripMenuItem GMShowGrid;

		// Token: 0x040001B6 RID: 438
		private global::System.Windows.Forms.ToolStripMenuItem TMEdit;

		// Token: 0x040001B7 RID: 439
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator10;

		// Token: 0x040001B8 RID: 440
		private global::System.Windows.Forms.ToolStripMenuItem TMCut;

		// Token: 0x040001B9 RID: 441
		private global::System.Windows.Forms.ToolStripMenuItem TMCopy;

		// Token: 0x040001BA RID: 442
		private global::System.Windows.Forms.ToolStripMenuItem TMPaste;

		// Token: 0x040001BB RID: 443
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator11;

		// Token: 0x040001BC RID: 444
		private global::System.Windows.Forms.ToolStripMenuItem BMView;

		// Token: 0x040001BD RID: 445
		private global::System.Windows.Forms.ToolStripMenuItem BMReload;

		// Token: 0x040001BE RID: 446
		private global::System.Windows.Forms.ToolStripMenuItem BMTestEnd;

		// Token: 0x040001BF RID: 447
		private global::System.Windows.Forms.ToolStripMenuItem GMZoomIndexes;

		// Token: 0x040001C0 RID: 448
		private global::System.Windows.Forms.ToolStripMenuItem GMZoom25;

		// Token: 0x040001C1 RID: 449
		private global::System.Windows.Forms.ToolStripMenuItem GMZoom50;

		// Token: 0x040001C2 RID: 450
		private global::System.Windows.Forms.ToolStripMenuItem GMZoom100;

		// Token: 0x040001C3 RID: 451
		private global::System.Windows.Forms.ToolStripMenuItem GMZoom200;

		// Token: 0x040001C4 RID: 452
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;

		// Token: 0x040001C5 RID: 453
		private global::System.Windows.Forms.ToolStripMenuItem GMCursor;

		// Token: 0x040001C6 RID: 454
		private global::System.Windows.Forms.ToolStripMenuItem GMPen;

		// Token: 0x040001C7 RID: 455
		private global::System.Windows.Forms.ToolStripMenuItem GMLine;

		// Token: 0x040001C8 RID: 456
		private global::System.Windows.Forms.ToolStripMenuItem GMRect;

		// Token: 0x040001C9 RID: 457
		private global::System.Windows.Forms.ToolStripMenuItem GMFill;

		// Token: 0x040001CA RID: 458
		private global::System.Windows.Forms.ToolStripMenuItem BMBrowser;

		// Token: 0x040001CB RID: 459
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;

		// Token: 0x040001CC RID: 460
		public global::System.Windows.Forms.ToolStripMenuItem TMUndo;

		// Token: 0x040001CD RID: 461
		public global::System.Windows.Forms.ToolStripMenuItem TMRedo;

		// Token: 0x040001CE RID: 462
		public global::System.Windows.Forms.ToolStripMenuItem TMTestRun;

		// Token: 0x040001CF RID: 463
		private global::System.Windows.Forms.ToolStripMenuItem LayerMenu;

		// Token: 0x040001D0 RID: 464
		private global::System.Windows.Forms.ToolStripMenuItem EditPatternChip;

		// Token: 0x040001D1 RID: 465
		private global::System.Windows.Forms.ToolStripMenuItem EditBackground;

		// Token: 0x040001D2 RID: 466
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;

		// Token: 0x040001D3 RID: 467
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;

		// Token: 0x040001D4 RID: 468
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;

		// Token: 0x040001D5 RID: 469
		private global::System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12;

		// Token: 0x040001D6 RID: 470
		private global::System.Windows.Forms.SplitContainer MainSplit;

		// Token: 0x040001D7 RID: 471
		private global::System.Windows.Forms.SplitContainer ItemSpliter;

		// Token: 0x040001D8 RID: 472
		private global::System.Windows.Forms.Label ChipDescription;

		// Token: 0x040001D9 RID: 473
		private global::System.Windows.Forms.Label ChipChar;

		// Token: 0x040001DA RID: 474
		private global::System.Windows.Forms.PictureBox ChipImage;

		// Token: 0x040001DB RID: 475
		private global::MasaoPlus.GUIChipList GuiChipList;

		// Token: 0x040001DC RID: 476
		private global::System.Windows.Forms.ListBox ChipList;

		// Token: 0x040001DD RID: 477
		private global::System.Windows.Forms.ComboBox DrawType;

		// Token: 0x040001DE RID: 478
		private global::System.Windows.Forms.TabControl EditTab;

		// Token: 0x040001DF RID: 479
		private global::System.Windows.Forms.TabPage GuiEditor;

		// Token: 0x040001E0 RID: 480
		private global::System.Windows.Forms.Panel EditorSystemPanel;

		// Token: 0x040001E1 RID: 481
		public global::MasaoPlus.GUIDesigner MainDesigner;

		// Token: 0x040001E2 RID: 482
		private global::System.Windows.Forms.VScrollBar GVirtScroll;

		// Token: 0x040001E3 RID: 483
		private global::System.Windows.Forms.HScrollBar GHorzScroll;

		// Token: 0x040001E4 RID: 484
		private global::System.Windows.Forms.ToolStrip GuiEditorTool;

		// Token: 0x040001E5 RID: 485
		private global::System.Windows.Forms.ToolStripButton ShowGrid;

		// Token: 0x040001E6 RID: 486
		private global::System.Windows.Forms.ToolStripDropDownButton StageZoom;

		// Token: 0x040001E7 RID: 487
		private global::System.Windows.Forms.ToolStripMenuItem StageZoom25;

		// Token: 0x040001E8 RID: 488
		private global::System.Windows.Forms.ToolStripMenuItem StageZoom50;

		// Token: 0x040001E9 RID: 489
		private global::System.Windows.Forms.ToolStripMenuItem StageZoom100;

		// Token: 0x040001EA RID: 490
		private global::System.Windows.Forms.ToolStripMenuItem StageZoom200;

		// Token: 0x040001EB RID: 491
		private global::System.Windows.Forms.ToolStripDropDownButton StageLayer;

		// Token: 0x040001EC RID: 492
		private global::System.Windows.Forms.ToolStripMenuItem PatternChipLayer;

		// Token: 0x040001ED RID: 493
		private global::System.Windows.Forms.ToolStripMenuItem BackgroundLayer;

		// Token: 0x040001EE RID: 494
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem13;

		// Token: 0x040001EF RID: 495
		private global::System.Windows.Forms.ToolStripMenuItem MDUnactiveLayer;

		// Token: 0x040001F0 RID: 496
		private global::System.Windows.Forms.ToolStripMenuItem MTUnactiveLayer;

		// Token: 0x040001F1 RID: 497
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator2;

		// Token: 0x040001F2 RID: 498
		private global::System.Windows.Forms.ToolStripButton ItemUndo;

		// Token: 0x040001F3 RID: 499
		private global::System.Windows.Forms.ToolStripButton ItemRedo;

		// Token: 0x040001F4 RID: 500
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator7;

		// Token: 0x040001F5 RID: 501
		private global::System.Windows.Forms.ToolStripButton ItemCursor;

		// Token: 0x040001F6 RID: 502
		private global::System.Windows.Forms.ToolStripButton ItemPen;

		// Token: 0x040001F7 RID: 503
		private global::System.Windows.Forms.ToolStripButton ItemLine;

		// Token: 0x040001F8 RID: 504
		private global::System.Windows.Forms.ToolStripButton ItemRect;

		// Token: 0x040001F9 RID: 505
		private global::System.Windows.Forms.ToolStripButton ItemFill;

		// Token: 0x040001FA RID: 506
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator3;

		// Token: 0x040001FB RID: 507
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator6;

		// Token: 0x040001FC RID: 508
		private global::System.Windows.Forms.ToolStripButton ItemReload;

		// Token: 0x040001FD RID: 509
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator8;

		// Token: 0x040001FE RID: 510
		private global::System.Windows.Forms.ToolStripButton GUIProjConfig;

		// Token: 0x040001FF RID: 511
		private global::System.Windows.Forms.ToolStripButton GUIProjTestRun;

		// Token: 0x04000200 RID: 512
		private global::System.Windows.Forms.StatusStrip GUIEditorStatus;

		// Token: 0x04000201 RID: 513
		private global::System.Windows.Forms.ToolStripStatusLabel ChipNavigator;

		// Token: 0x04000202 RID: 514
		private global::System.Windows.Forms.ToolStripStatusLabel GuiPositionInfo;

		// Token: 0x04000203 RID: 515
		private global::System.Windows.Forms.ToolStripStatusLabel CurrentStatus;

		// Token: 0x04000204 RID: 516
		private global::System.Windows.Forms.TabPage TextEditor;

		// Token: 0x04000205 RID: 517
		private global::MasaoPlus.TextEditor MainEditor;

		// Token: 0x04000206 RID: 518
		private global::System.Windows.Forms.TabPage IntegrateTestrun;

		// Token: 0x04000207 RID: 519
		private global::MasaoPlus.IntegratedBrowser IntegrateBrowser;

		// Token: 0x04000208 RID: 520
		private global::System.Windows.Forms.ToolStrip MainToolbar;

		// Token: 0x04000209 RID: 521
		private global::System.Windows.Forms.ToolStripButton MTNew;

		// Token: 0x0400020A RID: 522
		private global::System.Windows.Forms.ToolStripButton MTOpen;

		// Token: 0x0400020B RID: 523
		private global::System.Windows.Forms.ToolStripButton MTSave;

		// Token: 0x0400020C RID: 524
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

		// Token: 0x0400020D RID: 525
		private global::System.Windows.Forms.ToolStripButton MTOutput;

		// Token: 0x0400020E RID: 526
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator4;

		// Token: 0x0400020F RID: 527
		private global::System.Windows.Forms.ToolStripButton MTEditConfig;

		// Token: 0x04000210 RID: 528
		private global::System.Windows.Forms.TabControl SideTab;

		// Token: 0x04000211 RID: 529
		private global::System.Windows.Forms.TabPage ChipPage;

		// Token: 0x04000212 RID: 530
		private global::System.Windows.Forms.TabPage PropertyTab;

		// Token: 0x04000213 RID: 531
		private global::MasaoPlus.Controls.ConfigList MasaoConfigList;

		// Token: 0x04000214 RID: 532
		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator5;

		// Token: 0x04000215 RID: 533
		private global::System.Windows.Forms.ToolStripButton MTVersion;

		// Token: 0x04000216 RID: 534
		private global::System.Windows.Forms.ToolStripMenuItem MProjectConfig;

		// Token: 0x04000217 RID: 535
		private global::System.Windows.Forms.ToolStripMenuItem MWriteStagePicture;

		// Token: 0x04000218 RID: 536
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem14;

		// Token: 0x04000219 RID: 537
		private global::System.Windows.Forms.ToolStripMenuItem MResetProjRuntime;

		// Token: 0x0400021A RID: 538
		private global::System.Windows.Forms.ToolStripMenuItem InstalledRuntime;

		// Token: 0x0400021B RID: 539
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem15;

		// Token: 0x0400021C RID: 540
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem16;

		// Token: 0x0400021D RID: 541
		private global::System.Windows.Forms.ToolStripMenuItem MUpdateApp;

		// Token: 0x0400021E RID: 542
		private global::System.Windows.Forms.ToolStripMenuItem 他の方法で新規MToolStripMenuItem;

		// Token: 0x0400021F RID: 543
		private global::System.Windows.Forms.ToolStripMenuItem MProjectInheritNew;

		// Token: 0x04000220 RID: 544
		private global::System.Windows.Forms.ToolStripMenuItem MConvertHTML;

		// Token: 0x04000221 RID: 545
		public global::System.Windows.Forms.ToolStripButton GuiCut;

		// Token: 0x04000222 RID: 546
		public global::System.Windows.Forms.ToolStripButton GuiCopy;

		// Token: 0x04000223 RID: 547
		public global::System.Windows.Forms.ToolStripButton GuiPaste;

		// Token: 0x04000224 RID: 548
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;

		// Token: 0x04000225 RID: 549
		private global::System.Windows.Forms.ToolStripMenuItem TransparentUnactiveLayer;

		// Token: 0x04000226 RID: 550
		private global::System.Windows.Forms.ToolStripMenuItem ViewUnactiveLayer;

		// Token: 0x04000227 RID: 551
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem17;

		// Token: 0x04000228 RID: 552
		private global::System.Windows.Forms.ToolStripMenuItem MExEdit;

		// Token: 0x04000229 RID: 553
		private global::System.Windows.Forms.ToolStripMenuItem MStageRev;

		// Token: 0x0400022A RID: 554
		private global::System.Windows.Forms.ToolStripMenuItem GMStage;

		// Token: 0x0400022B RID: 555
		private global::System.Windows.Forms.ToolStripMenuItem MEditStage1;

		// Token: 0x0400022C RID: 556
		private global::System.Windows.Forms.ToolStripMenuItem MEditStage2;

		// Token: 0x0400022D RID: 557
		private global::System.Windows.Forms.ToolStripMenuItem MEditStage3;

		// Token: 0x0400022E RID: 558
		private global::System.Windows.Forms.ToolStripMenuItem MEditStage4;

		// Token: 0x0400022F RID: 559
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem18;

		// Token: 0x04000230 RID: 560
		private global::System.Windows.Forms.ToolStripMenuItem MEditMap;

		// Token: 0x04000231 RID: 561
		private global::System.Windows.Forms.ToolStripMenuItem MSConfig;

		// Token: 0x04000232 RID: 562
		private global::System.Windows.Forms.ToolStripButton MTImgOut;

		// Token: 0x04000233 RID: 563
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem19;

		// Token: 0x04000234 RID: 564
		private global::System.Windows.Forms.ToolStripMenuItem GMAllTestrun;

		// Token: 0x04000235 RID: 565
		private global::System.Windows.Forms.ToolStripMenuItem TMAllTestrun;

		// Token: 0x04000236 RID: 566
		private global::System.Windows.Forms.ToolStripSeparator toolStripMenuItem20;

		// Token: 0x04000237 RID: 567
		private global::System.Windows.Forms.ToolStripMenuItem MReloadImage;

		// Token: 0x04000238 RID: 568
		private global::System.Windows.Forms.ToolStripMenuItem ShowOverView;
	}
}
