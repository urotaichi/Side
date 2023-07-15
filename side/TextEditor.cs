using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MasaoPlus.Properties;

namespace MasaoPlus
{
    public class TextEditor : UserControl
    {
        public TextEditor()
        {
            InitializeComponent();
        }

        private void TextEditor_Load(object sender, EventArgs e)
        {
            StageTextEditor.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
        }

        public void InitEditor()
        {
            StageTextEditor.SelectionStart = 0;
            TextLineNo.Refresh();
            TextRuler.Refresh();
        }

        public void AddBuffer()
        {
            if (Buffering)
            {
                if (TextBuffer.Count != ci + 1)
                {
                    TextBuffer.RemoveRange(ci + 1, TextBuffer.Count - (ci + 1));
                    CursorBuffer.RemoveRange(ci + 1, CursorBuffer.Count - (ci + 1));
                }
                TextBuffer.Add((string)StageTextEditor.Text.Clone());
                CursorBuffer.Add((StageTextEditor.SelectionStart == -1) ? 0 : StageTextEditor.SelectionStart);
                ci = TextBuffer.Count - 1;
                if (ci != 0)
                {
                    Global.state.EditFlag = true;
                }
            }
            SetUndoRedo();
        }

        public void Undo()
        {
            if (ci <= 0)
            {
                return;
            }
            Global.state.EditFlag = true;
            ci--;
            Buffering = false;
            StageTextEditor.Text = (string)TextBuffer[ci].Clone();
            StageTextEditor.SelectionStart = CursorBuffer[ci];
            Buffering = true;
            SetUndoRedo();
        }

        public void Redo()
        {
            if (ci >= TextBuffer.Count - 1)
            {
                return;
            }
            Global.state.EditFlag = true;
            ci++;
            Buffering = false;
            StageTextEditor.Text = (string)TextBuffer[ci].Clone();
            StageTextEditor.SelectionStart = CursorBuffer[ci];
            Buffering = true;
            SetUndoRedo();
        }

        private void SetUndoRedo()
        {
            if (ci == 0)
            {
                TextUndo.Enabled = false;
                Global.MainWnd.TMUndo.Enabled = false;
            }
            else
            {
                TextUndo.Enabled = true;
                Global.MainWnd.TMUndo.Enabled = true;
            }
            if (ci == TextBuffer.Count - 1)
            {
                TextRedo.Enabled = false;
                Global.MainWnd.TMRedo.Enabled = false;
                return;
            }
            TextRedo.Enabled = true;
            Global.MainWnd.TMRedo.Enabled = true;
        }

        public void BufferClear()
        {
            TextBuffer.Clear();
            CursorBuffer.Clear();
            ci = TextBuffer.Count - 1;
            SetUndoRedo();
        }

        private void TextBoxOwnerPanel_Resize(object sender, EventArgs e)
        {
            StageTextEditor.Width = TextBoxOwnerPanel.Width - 2;
            StageTextEditor.Height = TextBoxOwnerPanel.Height - 2;
        }

        private void StageTextEditor_KeyDown(object sender, KeyEventArgs e)
        {
            TextLineNo.Refresh();
            TextRuler.Refresh();
            LineInfoUpdate();
            if (!e.Control)
            {
                if (e.Modifiers == Keys.None)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.F7:
                            Global.MainWnd.EditPatternChip_Click(this, new EventArgs());
                            return;
                        case Keys.F8:
                            Global.MainWnd.EditBackground_Click(this, new EventArgs());
                            break;
                        default:
                            return;
                    }
                }
                return;
            }
            Keys keyCode = e.KeyCode;
            switch (keyCode)
            {
                case Keys.Y:
                    Redo();
                    return;
                case Keys.Z:
                    Undo();
                    return;
                default:
                    switch (keyCode)
                    {
                        case Keys.F10:
                            Global.MainWnd.ProjectConfig_Click(this, new EventArgs());
                            return;
                        case Keys.F11:
                            Global.MainWnd.MSysConfig_Click(this, new EventArgs());
                            return;
                        default:
                            return;
                    }
            }
        }

        private void StageTextEditor_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void StageTextEditor_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void StageTextEditor_VScroll(object sender, EventArgs e)
        {
            TextLineNo.Refresh();
            TextRuler.Refresh();
            LineInfoUpdate();
        }

        private void LineInfoUpdate()
        {
            if (StageTextEditor.Text == "")
            {
                TextPositionInfo.Text = "";
                LineInfo.Text = "";
                TextStatus.Text = "入力がありません";
                return;
            }
            int num = StageTextEditor.GetFirstCharIndexOfCurrentLine();
            int lineFromCharIndex = StageTextEditor.GetLineFromCharIndex(num);
            num = StageTextEditor.SelectionStart - num;
            TextPositionInfo.Text = $"{lineFromCharIndex}行 {num}文字";
            int length = StageTextEditor.Lines[lineFromCharIndex].Length;
            LineInfo.Text = $"{StageTextEditor.Lines.Length}行 {length}文字";
            if (length == Global.state.GetCByteWidth)
            {
                TextStatus.Text = "完了";
                return;
            }
            if (length > Global.state.GetCByteWidth)
            {
                int num2 = length - Global.state.GetCByteWidth;
                TextStatus.Text = $"現在の行は{num2}文字過剰です。";
                return;
            }
            int num3 = Global.state.GetCByteWidth - length;
            TextStatus.Text = $"現在の行は{num3}文字足りていません。";
        }

        private void TextLineNo_Paint(object sender, PaintEventArgs e)
        {
            int num = Native.USER32.SendMessage(StageTextEditor.Handle, 206U, 0, 0);
            int lastVisibleRowIndex = GetLastVisibleRowIndex();
            int num2 = num;
            while (num2 <= lastVisibleRowIndex && num2 < StageTextEditor.Lines.Length)
            {
                int num3 = StageTextEditor.GetFirstCharIndexFromLine(num2);
                num3 = StageTextEditor.GetPositionFromCharIndex(num3).Y;
                int num4 = (int)e.Graphics.MeasureString(num2.ToString(), StageTextEditor.Font).Width;
                if (num2 == StageTextEditor.GetLineFromCharIndex(StageTextEditor.GetFirstCharIndexOfCurrentLine()))
                {
                    int height = (int)e.Graphics.MeasureString(num2.ToString(), StageTextEditor.Font).Height;
                    e.Graphics.FillRectangle(Brushes.RoyalBlue, new Rectangle(new Point(0, num3), new Size(TextLineNo.Width, height)));
                    DrawText(e.Graphics, num2.ToString(), StageTextEditor.Font, new Point(TextLineNo.Width - num4 - Global.definition.LineNoPaddingRight, num3 + Global.definition.LineNoPaddingTop), Color.White);
                }
                else if (StageTextEditor.Lines[num2].Length == Global.state.GetCByteWidth && num2 < Global.cpd.runtime.Definitions.StageSize.y)
                {
                    DrawText(e.Graphics, num2.ToString(), StageTextEditor.Font, new Point(TextLineNo.Width - num4 - Global.definition.LineNoPaddingRight, num3 + Global.definition.LineNoPaddingTop), Color.Black);
                }
                else
                {
                    int height2 = (int)e.Graphics.MeasureString(num2.ToString(), StageTextEditor.Font).Height;
                    e.Graphics.FillRectangle(Brushes.Red, new Rectangle(new Point(0, num3), new Size(TextLineNo.Width, height2)));
                    DrawText(e.Graphics, num2.ToString(), StageTextEditor.Font, new Point(TextLineNo.Width - num4 - Global.definition.LineNoPaddingRight, num3 + Global.definition.LineNoPaddingTop), Color.White);
                }
                num2++;
            }
        }

        private void DrawText(Graphics g, string s, Font f, Point p, Color c)
        {
            if (Global.config.localSystem.TextEditorGDIMode)
            {
                TextRenderer.DrawText(g, s, f, p, c);
                return;
            }
            using Brush brush = new SolidBrush(c);
            g.DrawString(s, f, brush, p);
        }

        public bool CanConvertTextSource()
        {
            if (StageTextEditor.Lines.Length != Global.state.GetCSSize.y)
            {
                return false;
            }
            foreach (string text in StageTextEditor.Lines)
            {
                if (text.Length != Global.state.GetCByteWidth)
                {
                    return false;
                }
            }
            return true;
        }

        private int GetLastVisibleRowIndex()
        {
            Point pt = new Point(0, StageTextEditor.Height);
            int charIndexFromPosition = StageTextEditor.GetCharIndexFromPosition(pt);
            return StageTextEditor.GetLineFromCharIndex(charIndexFromPosition);
        }

        private void TextRuler_Paint(object sender, PaintEventArgs e)
        {
            int lineFromCharIndex = StageTextEditor.GetLineFromCharIndex(StageTextEditor.GetFirstCharIndexOfCurrentLine());
            int rulerStep = Global.definition.RulerStep;
            int num;
            if (StageTextEditor.GetFirstCharIndexFromLine(lineFromCharIndex + 1) == -1)
            {
                num = StageTextEditor.TextLength - StageTextEditor.GetFirstCharIndexFromLine(lineFromCharIndex) - 1;
            }
            else
            {
                num = StageTextEditor.GetFirstCharIndexFromLine(lineFromCharIndex + 1) - StageTextEditor.GetFirstCharIndexFromLine(lineFromCharIndex) - 1;
            }
            int firstCharIndexFromLine = StageTextEditor.GetFirstCharIndexFromLine(lineFromCharIndex);
            Point positionFromCharIndex = StageTextEditor.GetPositionFromCharIndex(StageTextEditor.SelectionStart);
            Point positionFromCharIndex2 = StageTextEditor.GetPositionFromCharIndex(StageTextEditor.SelectionStart + 1);
            e.Graphics.FillRectangle(Brushes.RoyalBlue, new Rectangle(new Point(positionFromCharIndex.X, 0), new Size((positionFromCharIndex2.X - positionFromCharIndex.X < 0) ? 5 : (positionFromCharIndex2.X - positionFromCharIndex.X), TextRuler.Height)));
            bool flag = false;
            for (int i = 0; i <= num; i += rulerStep)
            {
                if (Global.definition.RulerPutDot)
                {
                    flag = (i % (rulerStep * 2) != 0);
                }
                positionFromCharIndex = StageTextEditor.GetPositionFromCharIndex(firstCharIndexFromLine + i);
                if (positionFromCharIndex.X >= (flag ? 1 : ((int)e.Graphics.MeasureString(i.ToString(), StageTextEditor.Font).Width * -1)))
                {
                    if (positionFromCharIndex.X > TextRuler.Width)
                    {
                        return;
                    }
                    if (flag)
                    {
                        e.Graphics.DrawLine(Pens.Black, new Point(positionFromCharIndex.X, TextRuler.Height / 2), new Point(positionFromCharIndex.X, TextRuler.Height));
                    }
                    else
                    {
                        e.Graphics.DrawLine(Pens.Black, new Point(positionFromCharIndex.X, 0), new Point(positionFromCharIndex.X, TextRuler.Height));
                        int num2 = (int)e.Graphics.MeasureString(i.ToString(), StageTextEditor.Font).Height;
                        DrawText(e.Graphics, i.ToString(), StageTextEditor.Font, new Point(positionFromCharIndex.X, TextRuler.Height - num2), Color.Black);
                    }
                }
            }
        }

        private void StageTextEditor_TextChanged(object sender, EventArgs e)
        {
            if (CanConvertTextSource())
            {
                EnableTranslate.Enabled = true;
                EnableTranslate.Text = "テストプレイ/GUIコンバート可能";
                TextTestRun.Enabled = true;
                Global.MainWnd.TMTestRun.Enabled = true;
            }
            else
            {
                EnableTranslate.Enabled = false;
                EnableTranslate.Text = "テストプレイ/GUIコンバート不可能";
                TextTestRun.Enabled = false;
                Global.MainWnd.TMTestRun.Enabled = false;
            }
            AddBuffer();
        }

        private void TextUndo_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void TextRedo_Click(object sender, EventArgs e)
        {
            Redo();
        }

        private void TextCut_Click(object sender, EventArgs e)
        {
            StageTextEditor.Cut();
        }

        private void TextCopy_Click(object sender, EventArgs e)
        {
            StageTextEditor.Copy();
        }

        private void TextPaste_Click(object sender, EventArgs e)
        {
            StageTextEditor.Paste();
        }

        private void TextTestRun_Click(object sender, EventArgs e)
        {
            Global.MainWnd.Testrun();
        }

        private void TextProjConfig_Click(object sender, EventArgs e)
        {
            Global.MainWnd.ProjectConfig_Click(sender, e);
        }

        private void StageTextEditor_SelectionChanged(object sender, EventArgs e)
        {
            TextLineNo.Refresh();
            TextRuler.Refresh();
            LineInfoUpdate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            TextEditorTable = new TableLayoutPanel();
            TextRuler = new PictureBox();
            TextLineNo = new PictureBox();
            TextBoxOwnerPanel = new Panel();
            StageTextEditor = new RichTextBox();
            TextEditorStatus = new StatusStrip();
            TextPositionInfo = new ToolStripStatusLabel();
            TextStatus = new ToolStripStatusLabel();
            LineInfo = new ToolStripStatusLabel();
            EnableTranslate = new ToolStripStatusLabel();
            TextEditorTool = new ToolStrip();
            StageLayer = new ToolStripDropDownButton();
            PatternChipLayer = new ToolStripMenuItem();
            BackgroundLayer = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            TextUndo = new ToolStripButton();
            TextRedo = new ToolStripButton();
            toolStripSeparator5 = new ToolStripSeparator();
            TextCut = new ToolStripButton();
            TextCopy = new ToolStripButton();
            TextPaste = new ToolStripButton();
            toolStripSeparator9 = new ToolStripSeparator();
            TextProjConfig = new ToolStripButton();
            TextTestRun = new ToolStripButton();
            TextEditorTable.SuspendLayout();
            ((ISupportInitialize)TextRuler).BeginInit();
            ((ISupportInitialize)TextLineNo).BeginInit();
            TextBoxOwnerPanel.SuspendLayout();
            TextEditorStatus.SuspendLayout();
            TextEditorTool.SuspendLayout();
            SuspendLayout();
            TextEditorTable.BackColor = SystemColors.Control;
            TextEditorTable.ColumnCount = 2;
            TextEditorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40f));
            TextEditorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            TextEditorTable.Controls.Add(TextRuler, 1, 0);
            TextEditorTable.Controls.Add(TextLineNo, 0, 1);
            TextEditorTable.Controls.Add(TextBoxOwnerPanel, 1, 1);
            TextEditorTable.Dock = DockStyle.Fill;
            TextEditorTable.Location = new Point(0, 25);
            TextEditorTable.Name = "TextEditorTable";
            TextEditorTable.RowCount = 2;
            TextEditorTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 23f));
            TextEditorTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            TextEditorTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
            TextEditorTable.Size = new Size(553, 215);
            TextEditorTable.TabIndex = 5;
            TextRuler.BackColor = Color.White;
            TextRuler.BorderStyle = BorderStyle.FixedSingle;
            TextRuler.Dock = DockStyle.Fill;
            TextRuler.Location = new Point(41, 1);
            TextRuler.Margin = new Padding(1);
            TextRuler.Name = "TextRuler";
            TextRuler.Size = new Size(511, 21);
            TextRuler.TabIndex = 2;
            TextRuler.TabStop = false;
            TextRuler.Paint += TextRuler_Paint;
            TextLineNo.BackColor = Color.White;
            TextLineNo.BorderStyle = BorderStyle.FixedSingle;
            TextLineNo.Dock = DockStyle.Fill;
            TextLineNo.Location = new Point(1, 24);
            TextLineNo.Margin = new Padding(1);
            TextLineNo.Name = "TextLineNo";
            TextLineNo.Padding = new Padding(0, 0, 3, 0);
            TextLineNo.Size = new Size(38, 190);
            TextLineNo.TabIndex = 3;
            TextLineNo.TabStop = false;
            TextLineNo.Paint += TextLineNo_Paint;
            TextBoxOwnerPanel.BackColor = Color.Gray;
            TextBoxOwnerPanel.Controls.Add(StageTextEditor);
            TextBoxOwnerPanel.Dock = DockStyle.Fill;
            TextBoxOwnerPanel.Location = new Point(40, 23);
            TextBoxOwnerPanel.Margin = new Padding(0);
            TextBoxOwnerPanel.Name = "TextBoxOwnerPanel";
            TextBoxOwnerPanel.Padding = new Padding(1);
            TextBoxOwnerPanel.Size = new Size(513, 192);
            TextBoxOwnerPanel.TabIndex = 1;
            StageTextEditor.BorderStyle = BorderStyle.None;
            StageTextEditor.DetectUrls = false;
            StageTextEditor.Dock = DockStyle.Fill;
            StageTextEditor.Font = new Font("ＭＳ ゴシック", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 128);
            StageTextEditor.Location = new Point(1, 1);
            StageTextEditor.Margin = new Padding(1);
            StageTextEditor.Name = "StageTextEditor";
            StageTextEditor.Size = new Size(511, 190);
            StageTextEditor.TabIndex = 0;
            StageTextEditor.Text = "";
            StageTextEditor.WordWrap = false;
            StageTextEditor.KeyDown += StageTextEditor_KeyDown;
            StageTextEditor.VScroll += StageTextEditor_VScroll;
            StageTextEditor.MouseUp += StageTextEditor_MouseDown;
            StageTextEditor.HScroll += StageTextEditor_VScroll;
            StageTextEditor.SelectionChanged += StageTextEditor_SelectionChanged;
            StageTextEditor.MouseMove += StageTextEditor_MouseDown;
            StageTextEditor.MouseDown += StageTextEditor_MouseDown;
            StageTextEditor.KeyUp += StageTextEditor_KeyUp;
            StageTextEditor.TextChanged += StageTextEditor_TextChanged;
            TextEditorStatus.Items.AddRange(new ToolStripItem[]
            {
                TextPositionInfo,
                TextStatus,
                LineInfo,
                EnableTranslate
            });
            TextEditorStatus.Location = new Point(0, 240);
            TextEditorStatus.Name = "TextEditorStatus";
            TextEditorStatus.Size = new Size(553, 27);
            TextEditorStatus.SizingGrip = false;
            TextEditorStatus.TabIndex = 6;
            TextEditorStatus.Text = "statusStrip1";
            TextPositionInfo.BorderSides = ToolStripStatusLabelBorderSides.Right;
            TextPositionInfo.Name = "TextPositionInfo";
            TextPositionInfo.Size = new Size(42, 22);
            TextPositionInfo.Text = "None";
            TextPositionInfo.TextAlign = ContentAlignment.MiddleLeft;
            TextStatus.Name = "TextStatus";
            TextStatus.Size = new Size(226, 22);
            TextStatus.Spring = true;
            TextStatus.Text = "Ready.";
            TextStatus.TextAlign = ContentAlignment.MiddleLeft;
            LineInfo.BorderSides = (ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right);
            LineInfo.Name = "LineInfo";
            LineInfo.Size = new Size(66, 22);
            LineInfo.Text = "0行 0文字";
            EnableTranslate.Enabled = false;
            EnableTranslate.Name = "EnableTranslate";
            EnableTranslate.Size = new Size(204, 22);
            EnableTranslate.Text = "テストプレイ/GUIコンバート不可能";
            TextEditorTool.Items.AddRange(new ToolStripItem[]
            {
                StageLayer,
                toolStripSeparator1,
                TextUndo,
                TextRedo,
                toolStripSeparator5,
                TextCut,
                TextCopy,
                TextPaste,
                toolStripSeparator9,
                TextProjConfig,
                TextTestRun
            });
            TextEditorTool.Location = new Point(0, 0);
            TextEditorTool.Name = "TextEditorTool";
            TextEditorTool.Size = new Size(553, 25);
            TextEditorTool.TabIndex = 7;
            TextEditorTool.Text = "toolStrip1";
            StageLayer.DisplayStyle = ToolStripItemDisplayStyle.Image;
            StageLayer.DropDownItems.AddRange(new ToolStripItem[]
            {
                PatternChipLayer,
                BackgroundLayer
            });
            StageLayer.Image = Resources.layers;
            StageLayer.ImageTransparentColor = Color.Magenta;
            StageLayer.Name = "StageLayer";
            StageLayer.Size = new Size(29, 22);
            StageLayer.Text = "レイヤーの変更";
            PatternChipLayer.Checked = true;
            PatternChipLayer.CheckState = CheckState.Checked;
            PatternChipLayer.Name = "PatternChipLayer";
            PatternChipLayer.ShortcutKeyDisplayString = "F7";
            PatternChipLayer.Size = new Size(247, 22);
            PatternChipLayer.Text = "パターンチップレイヤー(&P)";
            BackgroundLayer.Name = "BackgroundLayer";
            BackgroundLayer.ShortcutKeyDisplayString = "F8";
            BackgroundLayer.Size = new Size(247, 22);
            BackgroundLayer.Text = "背景チップレイヤー(&B)";
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            TextUndo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TextUndo.Image = Resources.undo;
            TextUndo.ImageTransparentColor = Color.Magenta;
            TextUndo.Name = "TextUndo";
            TextUndo.Size = new Size(23, 22);
            TextUndo.Text = "元に戻す";
            TextUndo.Click += TextUndo_Click;
            TextRedo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TextRedo.Image = Resources.redo;
            TextRedo.ImageTransparentColor = Color.Magenta;
            TextRedo.Name = "TextRedo";
            TextRedo.Size = new Size(23, 22);
            TextRedo.Text = "やり直し";
            TextRedo.Click += TextRedo_Click;
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 25);
            TextCut.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TextCut.Image = Resources.cut;
            TextCut.ImageTransparentColor = Color.Magenta;
            TextCut.Name = "TextCut";
            TextCut.Size = new Size(23, 22);
            TextCut.Text = "切り取り";
            TextCut.Click += TextCut_Click;
            TextCopy.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TextCopy.Image = Resources.copy;
            TextCopy.ImageTransparentColor = Color.Magenta;
            TextCopy.Name = "TextCopy";
            TextCopy.Size = new Size(23, 22);
            TextCopy.Text = "コピー";
            TextCopy.Click += TextCopy_Click;
            TextPaste.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TextPaste.Image = Resources.paste;
            TextPaste.ImageTransparentColor = Color.Magenta;
            TextPaste.Name = "TextPaste";
            TextPaste.Size = new Size(23, 22);
            TextPaste.Text = "ペースト";
            TextPaste.Click += TextPaste_Click;
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(6, 25);
            TextProjConfig.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TextProjConfig.Image = Resources.map_edit;
            TextProjConfig.ImageTransparentColor = Color.Magenta;
            TextProjConfig.Name = "TextProjConfig";
            TextProjConfig.Size = new Size(23, 22);
            TextProjConfig.Text = "プロジェクトの設定";
            TextProjConfig.Click += TextProjConfig_Click;
            TextTestRun.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TextTestRun.Image = Resources.testrunstage;
            TextTestRun.ImageTransparentColor = Color.Magenta;
            TextTestRun.Name = "TextTestRun";
            TextTestRun.Size = new Size(23, 22);
            TextTestRun.Text = "テスト実行";
            TextTestRun.Click += TextTestRun_Click;
            AutoScaleDimensions = new SizeF(6f, 12f);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(TextEditorTable);
            Controls.Add(TextEditorTool);
            Controls.Add(TextEditorStatus);
            Name = "TextEditor";
            Size = new Size(553, 267);
            Load += TextEditor_Load;
            TextEditorTable.ResumeLayout(false);
            ((ISupportInitialize)TextRuler).EndInit();
            ((ISupportInitialize)TextLineNo).EndInit();
            TextBoxOwnerPanel.ResumeLayout(false);
            TextEditorStatus.ResumeLayout(false);
            TextEditorStatus.PerformLayout();
            TextEditorTool.ResumeLayout(false);
            TextEditorTool.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private List<string> TextBuffer = new List<string>();

        private List<int> CursorBuffer = new List<int>();

        private int ci = -1;

        private bool Buffering = true;

        private IContainer components;

        private TableLayoutPanel TextEditorTable;

        private PictureBox TextRuler;

        private PictureBox TextLineNo;

        private Panel TextBoxOwnerPanel;

        private StatusStrip TextEditorStatus;

        private ToolStripStatusLabel TextPositionInfo;

        private ToolStripStatusLabel TextStatus;

        private ToolStripStatusLabel LineInfo;

        private ToolStripStatusLabel EnableTranslate;

        private ToolStrip TextEditorTool;

        private ToolStripButton TextUndo;

        private ToolStripButton TextRedo;

        private ToolStripSeparator toolStripSeparator5;

        private ToolStripButton TextCut;

        private ToolStripButton TextCopy;

        private ToolStripButton TextPaste;

        private ToolStripSeparator toolStripSeparator9;

        private ToolStripButton TextProjConfig;

        private ToolStripButton TextTestRun;

        public RichTextBox StageTextEditor;

        private ToolStripSeparator toolStripSeparator1;

        public ToolStripDropDownButton StageLayer;

        public ToolStripMenuItem PatternChipLayer;

        public ToolStripMenuItem BackgroundLayer;
    }
}
