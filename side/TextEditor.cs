using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MasaoPlus.Properties;

namespace MasaoPlus
{
	// Token: 0x02000037 RID: 55
	public class TextEditor : UserControl
	{
		// Token: 0x06000208 RID: 520 RVA: 0x000035D8 File Offset: 0x000017D8
		public TextEditor()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000360A File Offset: 0x0000180A
		private void TextEditor_Load(object sender, EventArgs e)
		{
			this.StageTextEditor.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x00003619 File Offset: 0x00001819
		public void InitEditor()
		{
			this.StageTextEditor.SelectionStart = 0;
			this.TextLineNo.Refresh();
			this.TextRuler.Refresh();
		}

		// Token: 0x0600020B RID: 523 RVA: 0x00027864 File Offset: 0x00025A64
		public void AddBuffer()
		{
			if (this.Buffering)
			{
				if (this.TextBuffer.Count != this.ci + 1)
				{
					this.TextBuffer.RemoveRange(this.ci + 1, this.TextBuffer.Count - (this.ci + 1));
					this.CursorBuffer.RemoveRange(this.ci + 1, this.CursorBuffer.Count - (this.ci + 1));
				}
				this.TextBuffer.Add((string)this.StageTextEditor.Text.Clone());
				this.CursorBuffer.Add((this.StageTextEditor.SelectionStart == -1) ? 0 : this.StageTextEditor.SelectionStart);
				this.ci = this.TextBuffer.Count - 1;
				if (this.ci != 0)
				{
					Global.state.EditFlag = true;
				}
			}
			this.SetUndoRedo();
		}

		// Token: 0x0600020C RID: 524 RVA: 0x00027954 File Offset: 0x00025B54
		public void Undo()
		{
			if (this.ci <= 0)
			{
				return;
			}
			Global.state.EditFlag = true;
			this.ci--;
			this.Buffering = false;
			this.StageTextEditor.Text = (string)this.TextBuffer[this.ci].Clone();
			this.StageTextEditor.SelectionStart = this.CursorBuffer[this.ci];
			this.Buffering = true;
			this.SetUndoRedo();
		}

		// Token: 0x0600020D RID: 525 RVA: 0x000279DC File Offset: 0x00025BDC
		public void Redo()
		{
			if (this.ci >= this.TextBuffer.Count - 1)
			{
				return;
			}
			Global.state.EditFlag = true;
			this.ci++;
			this.Buffering = false;
			this.StageTextEditor.Text = (string)this.TextBuffer[this.ci].Clone();
			this.StageTextEditor.SelectionStart = this.CursorBuffer[this.ci];
			this.Buffering = true;
			this.SetUndoRedo();
		}

		// Token: 0x0600020E RID: 526 RVA: 0x00027A70 File Offset: 0x00025C70
		private void SetUndoRedo()
		{
			if (this.ci == 0)
			{
				this.TextUndo.Enabled = false;
				Global.MainWnd.TMUndo.Enabled = false;
			}
			else
			{
				this.TextUndo.Enabled = true;
				Global.MainWnd.TMUndo.Enabled = true;
			}
			if (this.ci == this.TextBuffer.Count - 1)
			{
				this.TextRedo.Enabled = false;
				Global.MainWnd.TMRedo.Enabled = false;
				return;
			}
			this.TextRedo.Enabled = true;
			Global.MainWnd.TMRedo.Enabled = true;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000363D File Offset: 0x0000183D
		public void BufferClear()
		{
			this.TextBuffer.Clear();
			this.CursorBuffer.Clear();
			this.ci = this.TextBuffer.Count - 1;
			this.SetUndoRedo();
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000366E File Offset: 0x0000186E
		private void TextBoxOwnerPanel_Resize(object sender, EventArgs e)
		{
			this.StageTextEditor.Width = this.TextBoxOwnerPanel.Width - 2;
			this.StageTextEditor.Height = this.TextBoxOwnerPanel.Height - 2;
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00027B10 File Offset: 0x00025D10
		private void StageTextEditor_KeyDown(object sender, KeyEventArgs e)
		{
			this.TextLineNo.Refresh();
			this.TextRuler.Refresh();
			this.LineInfoUpdate();
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
				this.Redo();
				return;
			case Keys.Z:
				this.Undo();
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
				break;
			}
		}

		// Token: 0x06000212 RID: 530 RVA: 0x000033FC File Offset: 0x000015FC
		private void StageTextEditor_KeyUp(object sender, KeyEventArgs e)
		{
		}

		// Token: 0x06000213 RID: 531 RVA: 0x000033FC File Offset: 0x000015FC
		private void StageTextEditor_MouseDown(object sender, MouseEventArgs e)
		{
		}

		// Token: 0x06000214 RID: 532 RVA: 0x000036A0 File Offset: 0x000018A0
		private void StageTextEditor_VScroll(object sender, EventArgs e)
		{
			this.TextLineNo.Refresh();
			this.TextRuler.Refresh();
			this.LineInfoUpdate();
		}

		// Token: 0x06000215 RID: 533 RVA: 0x00027BE0 File Offset: 0x00025DE0
		private void LineInfoUpdate()
		{
			if (this.StageTextEditor.Text == "")
			{
				this.TextPositionInfo.Text = "";
				this.LineInfo.Text = "";
				this.TextStatus.Text = "入力がありません";
				return;
			}
			int num = this.StageTextEditor.GetFirstCharIndexOfCurrentLine();
			int lineFromCharIndex = this.StageTextEditor.GetLineFromCharIndex(num);
			num = this.StageTextEditor.SelectionStart - num;
			this.TextPositionInfo.Text = lineFromCharIndex.ToString() + "行 " + num.ToString() + "文字";
			int length = this.StageTextEditor.Lines[lineFromCharIndex].Length;
			this.LineInfo.Text = this.StageTextEditor.Lines.Length.ToString() + "行 " + length.ToString() + "文字";
			if (length == Global.state.GetCByteWidth)
			{
				this.TextStatus.Text = "完了";
				return;
			}
			if (length > Global.state.GetCByteWidth)
			{
				int num2 = length - Global.state.GetCByteWidth;
				this.TextStatus.Text = "現在の行は" + num2.ToString() + "文字過剰です。";
				return;
			}
			int num3 = Global.state.GetCByteWidth - length;
			this.TextStatus.Text = "現在の行は" + num3.ToString() + "文字足りていません。";
		}

		// Token: 0x06000216 RID: 534 RVA: 0x00027D58 File Offset: 0x00025F58
		private void TextLineNo_Paint(object sender, PaintEventArgs e)
		{
			int num = Native.USER32.SendMessage(this.StageTextEditor.Handle, 206U, 0, 0);
			int lastVisibleRowIndex = this.GetLastVisibleRowIndex();
			int num2 = num;
			while (num2 <= lastVisibleRowIndex && num2 < this.StageTextEditor.Lines.Length)
			{
				int num3 = this.StageTextEditor.GetFirstCharIndexFromLine(num2);
				num3 = this.StageTextEditor.GetPositionFromCharIndex(num3).Y;
				int num4 = (int)e.Graphics.MeasureString(num2.ToString(), this.StageTextEditor.Font).Width;
				if (num2 == this.StageTextEditor.GetLineFromCharIndex(this.StageTextEditor.GetFirstCharIndexOfCurrentLine()))
				{
					int height = (int)e.Graphics.MeasureString(num2.ToString(), this.StageTextEditor.Font).Height;
					e.Graphics.FillRectangle(Brushes.RoyalBlue, new Rectangle(new Point(0, num3), new Size(this.TextLineNo.Width, height)));
					this.DrawText(e.Graphics, num2.ToString(), this.StageTextEditor.Font, new Point(this.TextLineNo.Width - num4 - Global.definition.LineNoPaddingRight, num3 + Global.definition.LineNoPaddingTop), Color.White);
				}
				else if (this.StageTextEditor.Lines[num2].Length == Global.state.GetCByteWidth && num2 < Global.cpd.runtime.Definitions.StageSize.y)
				{
					this.DrawText(e.Graphics, num2.ToString(), this.StageTextEditor.Font, new Point(this.TextLineNo.Width - num4 - Global.definition.LineNoPaddingRight, num3 + Global.definition.LineNoPaddingTop), Color.Black);
				}
				else
				{
					int height2 = (int)e.Graphics.MeasureString(num2.ToString(), this.StageTextEditor.Font).Height;
					e.Graphics.FillRectangle(Brushes.Red, new Rectangle(new Point(0, num3), new Size(this.TextLineNo.Width, height2)));
					this.DrawText(e.Graphics, num2.ToString(), this.StageTextEditor.Font, new Point(this.TextLineNo.Width - num4 - Global.definition.LineNoPaddingRight, num3 + Global.definition.LineNoPaddingTop), Color.White);
				}
				num2++;
			}
		}

		// Token: 0x06000217 RID: 535 RVA: 0x00027FE8 File Offset: 0x000261E8
		private void DrawText(Graphics g, string s, Font f, Point p, Color c)
		{
			if (Global.config.localSystem.TextEditorGDIMode)
			{
				TextRenderer.DrawText(g, s, f, p, c);
				return;
			}
			using (Brush brush = new SolidBrush(c))
			{
				g.DrawString(s, f, brush, p);
			}
		}

		// Token: 0x06000218 RID: 536 RVA: 0x00028048 File Offset: 0x00026248
		public bool CanConvertTextSource()
		{
			if (this.StageTextEditor.Lines.Length != Global.state.GetCSSize.y)
			{
				return false;
			}
			foreach (string text in this.StageTextEditor.Lines)
			{
				if (text.Length != Global.state.GetCByteWidth)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x000280AC File Offset: 0x000262AC
		private int GetLastVisibleRowIndex()
		{
			Point pt = new Point(0, this.StageTextEditor.Height);
			int charIndexFromPosition = this.StageTextEditor.GetCharIndexFromPosition(pt);
			return this.StageTextEditor.GetLineFromCharIndex(charIndexFromPosition);
		}

		// Token: 0x0600021A RID: 538 RVA: 0x000280E8 File Offset: 0x000262E8
		private void TextRuler_Paint(object sender, PaintEventArgs e)
		{
			int lineFromCharIndex = this.StageTextEditor.GetLineFromCharIndex(this.StageTextEditor.GetFirstCharIndexOfCurrentLine());
			int rulerStep = Global.definition.RulerStep;
			int num;
			if (this.StageTextEditor.GetFirstCharIndexFromLine(lineFromCharIndex + 1) == -1)
			{
				num = this.StageTextEditor.TextLength - this.StageTextEditor.GetFirstCharIndexFromLine(lineFromCharIndex) - 1;
			}
			else
			{
				num = this.StageTextEditor.GetFirstCharIndexFromLine(lineFromCharIndex + 1) - this.StageTextEditor.GetFirstCharIndexFromLine(lineFromCharIndex) - 1;
			}
			int firstCharIndexFromLine = this.StageTextEditor.GetFirstCharIndexFromLine(lineFromCharIndex);
			Point positionFromCharIndex = this.StageTextEditor.GetPositionFromCharIndex(this.StageTextEditor.SelectionStart);
			Point positionFromCharIndex2 = this.StageTextEditor.GetPositionFromCharIndex(this.StageTextEditor.SelectionStart + 1);
			e.Graphics.FillRectangle(Brushes.RoyalBlue, new Rectangle(new Point(positionFromCharIndex.X, 0), new Size((positionFromCharIndex2.X - positionFromCharIndex.X < 0) ? 5 : (positionFromCharIndex2.X - positionFromCharIndex.X), this.TextRuler.Height)));
			bool flag = false;
			for (int i = 0; i <= num; i += rulerStep)
			{
				if (Global.definition.RulerPutDot)
				{
					flag = (i % (rulerStep * 2) != 0);
				}
				positionFromCharIndex = this.StageTextEditor.GetPositionFromCharIndex(firstCharIndexFromLine + i);
				if (positionFromCharIndex.X >= (flag ? 1 : ((int)e.Graphics.MeasureString(i.ToString(), this.StageTextEditor.Font).Width * -1)))
				{
					if (positionFromCharIndex.X > this.TextRuler.Width)
					{
						return;
					}
					if (flag)
					{
						e.Graphics.DrawLine(Pens.Black, new Point(positionFromCharIndex.X, this.TextRuler.Height / 2), new Point(positionFromCharIndex.X, this.TextRuler.Height));
					}
					else
					{
						e.Graphics.DrawLine(Pens.Black, new Point(positionFromCharIndex.X, 0), new Point(positionFromCharIndex.X, this.TextRuler.Height));
						int num2 = (int)e.Graphics.MeasureString(i.ToString(), this.StageTextEditor.Font).Height;
						this.DrawText(e.Graphics, i.ToString(), this.StageTextEditor.Font, new Point(positionFromCharIndex.X, this.TextRuler.Height - num2), Color.Black);
					}
				}
			}
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00028374 File Offset: 0x00026574
		private void StageTextEditor_TextChanged(object sender, EventArgs e)
		{
			if (this.CanConvertTextSource())
			{
				this.EnableTranslate.Enabled = true;
				this.EnableTranslate.Text = "テストプレイ/GUIコンバート可能";
				this.TextTestRun.Enabled = true;
				Global.MainWnd.TMTestRun.Enabled = true;
			}
			else
			{
				this.EnableTranslate.Enabled = false;
				this.EnableTranslate.Text = "テストプレイ/GUIコンバート不可能";
				this.TextTestRun.Enabled = false;
				Global.MainWnd.TMTestRun.Enabled = false;
			}
			this.AddBuffer();
		}

		// Token: 0x0600021C RID: 540 RVA: 0x000036BE File Offset: 0x000018BE
		private void TextUndo_Click(object sender, EventArgs e)
		{
			this.Undo();
		}

		// Token: 0x0600021D RID: 541 RVA: 0x000036C6 File Offset: 0x000018C6
		private void TextRedo_Click(object sender, EventArgs e)
		{
			this.Redo();
		}

		// Token: 0x0600021E RID: 542 RVA: 0x000036CE File Offset: 0x000018CE
		private void TextCut_Click(object sender, EventArgs e)
		{
			this.StageTextEditor.Cut();
		}

		// Token: 0x0600021F RID: 543 RVA: 0x000036DB File Offset: 0x000018DB
		private void TextCopy_Click(object sender, EventArgs e)
		{
			this.StageTextEditor.Copy();
		}

		// Token: 0x06000220 RID: 544 RVA: 0x000036E8 File Offset: 0x000018E8
		private void TextPaste_Click(object sender, EventArgs e)
		{
			this.StageTextEditor.Paste();
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00002738 File Offset: 0x00000938
		private void TextTestRun_Click(object sender, EventArgs e)
		{
			Global.MainWnd.Testrun();
		}

		// Token: 0x06000222 RID: 546 RVA: 0x000036F5 File Offset: 0x000018F5
		private void TextProjConfig_Click(object sender, EventArgs e)
		{
			Global.MainWnd.ProjectConfig_Click(sender, e);
		}

		// Token: 0x06000223 RID: 547 RVA: 0x000036A0 File Offset: 0x000018A0
		private void StageTextEditor_SelectionChanged(object sender, EventArgs e)
		{
			this.TextLineNo.Refresh();
			this.TextRuler.Refresh();
			this.LineInfoUpdate();
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00003703 File Offset: 0x00001903
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000225 RID: 549 RVA: 0x00028404 File Offset: 0x00026604
		private void InitializeComponent()
		{
			this.TextEditorTable = new TableLayoutPanel();
			this.TextRuler = new PictureBox();
			this.TextLineNo = new PictureBox();
			this.TextBoxOwnerPanel = new Panel();
			this.StageTextEditor = new RichTextBox();
			this.TextEditorStatus = new StatusStrip();
			this.TextPositionInfo = new ToolStripStatusLabel();
			this.TextStatus = new ToolStripStatusLabel();
			this.LineInfo = new ToolStripStatusLabel();
			this.EnableTranslate = new ToolStripStatusLabel();
			this.TextEditorTool = new ToolStrip();
			this.StageLayer = new ToolStripDropDownButton();
			this.PatternChipLayer = new ToolStripMenuItem();
			this.BackgroundLayer = new ToolStripMenuItem();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.TextUndo = new ToolStripButton();
			this.TextRedo = new ToolStripButton();
			this.toolStripSeparator5 = new ToolStripSeparator();
			this.TextCut = new ToolStripButton();
			this.TextCopy = new ToolStripButton();
			this.TextPaste = new ToolStripButton();
			this.toolStripSeparator9 = new ToolStripSeparator();
			this.TextProjConfig = new ToolStripButton();
			this.TextTestRun = new ToolStripButton();
			this.TextEditorTable.SuspendLayout();
			((ISupportInitialize)this.TextRuler).BeginInit();
			((ISupportInitialize)this.TextLineNo).BeginInit();
			this.TextBoxOwnerPanel.SuspendLayout();
			this.TextEditorStatus.SuspendLayout();
			this.TextEditorTool.SuspendLayout();
			base.SuspendLayout();
			this.TextEditorTable.BackColor = SystemColors.Control;
			this.TextEditorTable.ColumnCount = 2;
			this.TextEditorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40f));
			this.TextEditorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.TextEditorTable.Controls.Add(this.TextRuler, 1, 0);
			this.TextEditorTable.Controls.Add(this.TextLineNo, 0, 1);
			this.TextEditorTable.Controls.Add(this.TextBoxOwnerPanel, 1, 1);
			this.TextEditorTable.Dock = DockStyle.Fill;
			this.TextEditorTable.Location = new Point(0, 25);
			this.TextEditorTable.Name = "TextEditorTable";
			this.TextEditorTable.RowCount = 2;
			this.TextEditorTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 23f));
			this.TextEditorTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
			this.TextEditorTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
			this.TextEditorTable.Size = new Size(553, 215);
			this.TextEditorTable.TabIndex = 5;
			this.TextRuler.BackColor = Color.White;
			this.TextRuler.BorderStyle = BorderStyle.FixedSingle;
			this.TextRuler.Dock = DockStyle.Fill;
			this.TextRuler.Location = new Point(41, 1);
			this.TextRuler.Margin = new Padding(1);
			this.TextRuler.Name = "TextRuler";
			this.TextRuler.Size = new Size(511, 21);
			this.TextRuler.TabIndex = 2;
			this.TextRuler.TabStop = false;
			this.TextRuler.Paint += this.TextRuler_Paint;
			this.TextLineNo.BackColor = Color.White;
			this.TextLineNo.BorderStyle = BorderStyle.FixedSingle;
			this.TextLineNo.Dock = DockStyle.Fill;
			this.TextLineNo.Location = new Point(1, 24);
			this.TextLineNo.Margin = new Padding(1);
			this.TextLineNo.Name = "TextLineNo";
			this.TextLineNo.Padding = new Padding(0, 0, 3, 0);
			this.TextLineNo.Size = new Size(38, 190);
			this.TextLineNo.TabIndex = 3;
			this.TextLineNo.TabStop = false;
			this.TextLineNo.Paint += this.TextLineNo_Paint;
			this.TextBoxOwnerPanel.BackColor = Color.Gray;
			this.TextBoxOwnerPanel.Controls.Add(this.StageTextEditor);
			this.TextBoxOwnerPanel.Dock = DockStyle.Fill;
			this.TextBoxOwnerPanel.Location = new Point(40, 23);
			this.TextBoxOwnerPanel.Margin = new Padding(0);
			this.TextBoxOwnerPanel.Name = "TextBoxOwnerPanel";
			this.TextBoxOwnerPanel.Padding = new Padding(1);
			this.TextBoxOwnerPanel.Size = new Size(513, 192);
			this.TextBoxOwnerPanel.TabIndex = 1;
			this.StageTextEditor.BorderStyle = BorderStyle.None;
			this.StageTextEditor.DetectUrls = false;
			this.StageTextEditor.Dock = DockStyle.Fill;
			this.StageTextEditor.Font = new Font("ＭＳ ゴシック", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 128);
			this.StageTextEditor.Location = new Point(1, 1);
			this.StageTextEditor.Margin = new Padding(1);
			this.StageTextEditor.Name = "StageTextEditor";
			this.StageTextEditor.Size = new Size(511, 190);
			this.StageTextEditor.TabIndex = 0;
			this.StageTextEditor.Text = "";
			this.StageTextEditor.WordWrap = false;
			this.StageTextEditor.KeyDown += this.StageTextEditor_KeyDown;
			this.StageTextEditor.VScroll += this.StageTextEditor_VScroll;
			this.StageTextEditor.MouseUp += this.StageTextEditor_MouseDown;
			this.StageTextEditor.HScroll += this.StageTextEditor_VScroll;
			this.StageTextEditor.SelectionChanged += this.StageTextEditor_SelectionChanged;
			this.StageTextEditor.MouseMove += this.StageTextEditor_MouseDown;
			this.StageTextEditor.MouseDown += this.StageTextEditor_MouseDown;
			this.StageTextEditor.KeyUp += this.StageTextEditor_KeyUp;
			this.StageTextEditor.TextChanged += this.StageTextEditor_TextChanged;
			this.TextEditorStatus.Items.AddRange(new ToolStripItem[]
			{
				this.TextPositionInfo,
				this.TextStatus,
				this.LineInfo,
				this.EnableTranslate
			});
			this.TextEditorStatus.Location = new Point(0, 240);
			this.TextEditorStatus.Name = "TextEditorStatus";
			this.TextEditorStatus.Size = new Size(553, 27);
			this.TextEditorStatus.SizingGrip = false;
			this.TextEditorStatus.TabIndex = 6;
			this.TextEditorStatus.Text = "statusStrip1";
			this.TextPositionInfo.BorderSides = ToolStripStatusLabelBorderSides.Right;
			this.TextPositionInfo.Name = "TextPositionInfo";
			this.TextPositionInfo.Size = new Size(42, 22);
			this.TextPositionInfo.Text = "None";
			this.TextPositionInfo.TextAlign = ContentAlignment.MiddleLeft;
			this.TextStatus.Name = "TextStatus";
			this.TextStatus.Size = new Size(226, 22);
			this.TextStatus.Spring = true;
			this.TextStatus.Text = "Ready.";
			this.TextStatus.TextAlign = ContentAlignment.MiddleLeft;
			this.LineInfo.BorderSides = (ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right);
			this.LineInfo.Name = "LineInfo";
			this.LineInfo.Size = new Size(66, 22);
			this.LineInfo.Text = "0行 0文字";
			this.EnableTranslate.Enabled = false;
			this.EnableTranslate.Name = "EnableTranslate";
			this.EnableTranslate.Size = new Size(204, 22);
			this.EnableTranslate.Text = "テストプレイ/GUIコンバート不可能";
			this.TextEditorTool.Items.AddRange(new ToolStripItem[]
			{
				this.StageLayer,
				this.toolStripSeparator1,
				this.TextUndo,
				this.TextRedo,
				this.toolStripSeparator5,
				this.TextCut,
				this.TextCopy,
				this.TextPaste,
				this.toolStripSeparator9,
				this.TextProjConfig,
				this.TextTestRun
			});
			this.TextEditorTool.Location = new Point(0, 0);
			this.TextEditorTool.Name = "TextEditorTool";
			this.TextEditorTool.Size = new Size(553, 25);
			this.TextEditorTool.TabIndex = 7;
			this.TextEditorTool.Text = "toolStrip1";
			this.StageLayer.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.StageLayer.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.PatternChipLayer,
				this.BackgroundLayer
			});
			this.StageLayer.Image = Resources.layers;
			this.StageLayer.ImageTransparentColor = Color.Magenta;
			this.StageLayer.Name = "StageLayer";
			this.StageLayer.Size = new Size(29, 22);
			this.StageLayer.Text = "レイヤーの変更";
			this.PatternChipLayer.Checked = true;
			this.PatternChipLayer.CheckState = CheckState.Checked;
			this.PatternChipLayer.Name = "PatternChipLayer";
			this.PatternChipLayer.ShortcutKeyDisplayString = "F7";
			this.PatternChipLayer.Size = new Size(247, 22);
			this.PatternChipLayer.Text = "パターンチップレイヤー(&P)";
			this.BackgroundLayer.Name = "BackgroundLayer";
			this.BackgroundLayer.ShortcutKeyDisplayString = "F8";
			this.BackgroundLayer.Size = new Size(247, 22);
			this.BackgroundLayer.Text = "背景チップレイヤー(&B)";
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new Size(6, 25);
			this.TextUndo.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.TextUndo.Image = Resources.undo;
			this.TextUndo.ImageTransparentColor = Color.Magenta;
			this.TextUndo.Name = "TextUndo";
			this.TextUndo.Size = new Size(23, 22);
			this.TextUndo.Text = "元に戻す";
			this.TextUndo.Click += this.TextUndo_Click;
			this.TextRedo.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.TextRedo.Image = Resources.redo;
			this.TextRedo.ImageTransparentColor = Color.Magenta;
			this.TextRedo.Name = "TextRedo";
			this.TextRedo.Size = new Size(23, 22);
			this.TextRedo.Text = "やり直し";
			this.TextRedo.Click += this.TextRedo_Click;
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new Size(6, 25);
			this.TextCut.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.TextCut.Image = Resources.cut;
			this.TextCut.ImageTransparentColor = Color.Magenta;
			this.TextCut.Name = "TextCut";
			this.TextCut.Size = new Size(23, 22);
			this.TextCut.Text = "切り取り";
			this.TextCut.Click += this.TextCut_Click;
			this.TextCopy.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.TextCopy.Image = Resources.copy;
			this.TextCopy.ImageTransparentColor = Color.Magenta;
			this.TextCopy.Name = "TextCopy";
			this.TextCopy.Size = new Size(23, 22);
			this.TextCopy.Text = "コピー";
			this.TextCopy.Click += this.TextCopy_Click;
			this.TextPaste.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.TextPaste.Image = Resources.paste;
			this.TextPaste.ImageTransparentColor = Color.Magenta;
			this.TextPaste.Name = "TextPaste";
			this.TextPaste.Size = new Size(23, 22);
			this.TextPaste.Text = "ペースト";
			this.TextPaste.Click += this.TextPaste_Click;
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			this.toolStripSeparator9.Size = new Size(6, 25);
			this.TextProjConfig.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.TextProjConfig.Image = Resources.map_edit;
			this.TextProjConfig.ImageTransparentColor = Color.Magenta;
			this.TextProjConfig.Name = "TextProjConfig";
			this.TextProjConfig.Size = new Size(23, 22);
			this.TextProjConfig.Text = "プロジェクトの設定";
			this.TextProjConfig.Click += this.TextProjConfig_Click;
			this.TextTestRun.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.TextTestRun.Image = Resources.testrunstage;
			this.TextTestRun.ImageTransparentColor = Color.Magenta;
			this.TextTestRun.Name = "TextTestRun";
			this.TextTestRun.Size = new Size(23, 22);
			this.TextTestRun.Text = "テスト実行";
			this.TextTestRun.Click += this.TextTestRun_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.TextEditorTable);
			base.Controls.Add(this.TextEditorTool);
			base.Controls.Add(this.TextEditorStatus);
			base.Name = "TextEditor";
			base.Size = new Size(553, 267);
			base.Load += this.TextEditor_Load;
			this.TextEditorTable.ResumeLayout(false);
			((ISupportInitialize)this.TextRuler).EndInit();
			((ISupportInitialize)this.TextLineNo).EndInit();
			this.TextBoxOwnerPanel.ResumeLayout(false);
			this.TextEditorStatus.ResumeLayout(false);
			this.TextEditorStatus.PerformLayout();
			this.TextEditorTool.ResumeLayout(false);
			this.TextEditorTool.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000295 RID: 661
		private List<string> TextBuffer = new List<string>();

		// Token: 0x04000296 RID: 662
		private List<int> CursorBuffer = new List<int>();

		// Token: 0x04000297 RID: 663
		private int ci = -1;

		// Token: 0x04000298 RID: 664
		private bool Buffering = true;

		// Token: 0x04000299 RID: 665
		private IContainer components;

		// Token: 0x0400029A RID: 666
		private TableLayoutPanel TextEditorTable;

		// Token: 0x0400029B RID: 667
		private PictureBox TextRuler;

		// Token: 0x0400029C RID: 668
		private PictureBox TextLineNo;

		// Token: 0x0400029D RID: 669
		private Panel TextBoxOwnerPanel;

		// Token: 0x0400029E RID: 670
		private StatusStrip TextEditorStatus;

		// Token: 0x0400029F RID: 671
		private ToolStripStatusLabel TextPositionInfo;

		// Token: 0x040002A0 RID: 672
		private ToolStripStatusLabel TextStatus;

		// Token: 0x040002A1 RID: 673
		private ToolStripStatusLabel LineInfo;

		// Token: 0x040002A2 RID: 674
		private ToolStripStatusLabel EnableTranslate;

		// Token: 0x040002A3 RID: 675
		private ToolStrip TextEditorTool;

		// Token: 0x040002A4 RID: 676
		private ToolStripButton TextUndo;

		// Token: 0x040002A5 RID: 677
		private ToolStripButton TextRedo;

		// Token: 0x040002A6 RID: 678
		private ToolStripSeparator toolStripSeparator5;

		// Token: 0x040002A7 RID: 679
		private ToolStripButton TextCut;

		// Token: 0x040002A8 RID: 680
		private ToolStripButton TextCopy;

		// Token: 0x040002A9 RID: 681
		private ToolStripButton TextPaste;

		// Token: 0x040002AA RID: 682
		private ToolStripSeparator toolStripSeparator9;

		// Token: 0x040002AB RID: 683
		private ToolStripButton TextProjConfig;

		// Token: 0x040002AC RID: 684
		private ToolStripButton TextTestRun;

		// Token: 0x040002AD RID: 685
		public RichTextBox StageTextEditor;

		// Token: 0x040002AE RID: 686
		private ToolStripSeparator toolStripSeparator1;

		// Token: 0x040002AF RID: 687
		public ToolStripDropDownButton StageLayer;

		// Token: 0x040002B0 RID: 688
		public ToolStripMenuItem PatternChipLayer;

		// Token: 0x040002B1 RID: 689
		public ToolStripMenuItem BackgroundLayer;
	}
}
