﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MasaoPlus.Controls;
using MasaoPlus.Dialogs;
using MasaoPlus.Properties;

namespace MasaoPlus
{
	// Token: 0x0200002B RID: 43
	public partial class MainWindow : Form
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600013A RID: 314 RVA: 0x0001F5E0 File Offset: 0x0001D7E0
		// (remove) Token: 0x0600013B RID: 315 RVA: 0x0001F618 File Offset: 0x0001D818
		public event MainWindow.MainDesignerScrollInvoke MainDesignerScroll;

		// Token: 0x0600013C RID: 316 RVA: 0x0001F650 File Offset: 0x0001D850
		public MainWindow()
		{
			Global.MainWnd = this;
			Global.state.UpdateCurrentChipInvoke += this.state_UpdateCurrentChipInvoke;
			this.InitializeComponent();
			this.MainDesigner.MouseMove += this.MainDesigner_MouseMove;
			this.MainDesigner.ChangeBufferInvoke += this.MainDesigner_ChangeBufferInvoke;
			this.MainEditor.PatternChipLayer.Click += this.EditPatternChip_Click;
			this.MainEditor.BackgroundLayer.Click += this.EditBackground_Click;
			this.MainDesigner.MouseWheel += this.MainDesigner_MouseWheel;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0001F70C File Offset: 0x0001D90C
		private void MainDesigner_MouseWheel(object sender, MouseEventArgs e)
		{
			int num = e.Delta / 120 * Global.cpd.runtime.Definitions.ChipSize.Height;
			bool flag = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
			if ((!flag && Global.config.draw.HScrollDefault) || (flag && !Global.config.draw.HScrollDefault))
			{
				State state = Global.state;
				state.MapPoint.X = state.MapPoint.X - num;
			}
			else
			{
				State state2 = Global.state;
				state2.MapPoint.Y = state2.MapPoint.Y - num;
			}
			Global.state.AdjustMapPoint();
			this.CommitScrollbar();
			this.MainDesigner.Refresh();
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0001F7D4 File Offset: 0x0001D9D4
		public void RefreshAll()
		{
			this.UpdateStatus("パラメータを反映しています...");
			Global.state.ForceNoBuffering = true;
			this.GuiChipList.Refresh();
			this.ChipList.Refresh();
			this.MainDesigner.UpdateBackgroundBuffer();
			this.MainDesigner.UpdateForegroundBuffer();
			this.MainDesigner.InitTransparent();
			this.MainDesigner.Refresh();
			this.MainDesigner_ChangeBufferInvoke();
			Global.state.ForceNoBuffering = false;
			this.UpdateStatus("完了");
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00002C90 File Offset: 0x00000E90
		public void Testrun()
		{
			this.EditTab.SelectedIndex = 2;
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0001F858 File Offset: 0x0001DA58
		private void MainDesigner_ChangeBufferInvoke()
		{
			this.ItemUndo.Enabled = (this.MainDesigner.BufferCurrent != 0);
			this.GMUndo.Enabled = this.ItemUndo.Enabled;
			this.ItemRedo.Enabled = (this.MainDesigner.BufferCurrent != this.MainDesigner.StageBuffer.Count - 1);
			this.GMRedo.Enabled = this.ItemRedo.Enabled;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0001F8D8 File Offset: 0x0001DAD8
		private void MainWindow_Load(object sender, EventArgs e)
		{
			this.MUpdateApp.Enabled = Global.definition.IsAutoUpdateEnabled;
			base.SetDesktopLocation(Global.config.lastData.WndPoint.X, Global.config.lastData.WndPoint.Y);
			base.Size = Global.config.lastData.WndSize;
			base.WindowState = Global.config.lastData.WndState;
			this.MainSplit.SplitterDistance = (int)((double)base.Width * Global.config.lastData.SpliterDist);
			if (Global.config.localSystem.ReverseTabView)
			{
				this.EditTab.Parent = this.MainSplit.Panel1;
				this.SideTab.Parent = this.MainSplit.Panel2;
			}
			else
			{
				this.SideTab.Parent = this.MainSplit.Panel1;
				this.EditTab.Parent = this.MainSplit.Panel2;
			}
			if (Global.config.draw.StartUpWithClassicCL)
			{
				this.DrawType.SelectedIndex = 3;
			}
			else
			{
				this.DrawType.SelectedIndex = 0;
			}
			this.UpdateTitle();
			this.ShowGrid.Checked = Global.config.draw.DrawGrid;
			this.GMShowGrid.Checked = Global.config.draw.DrawGrid;
			int num = (int)(Global.config.draw.ZoomIndex * 100.0);
			if (num <= 50)
			{
				if (num == 25)
				{
					this.StageZoom25.Checked = true;
					this.GMZoom25.Checked = true;
					return;
				}
				if (num == 50)
				{
					this.StageZoom50.Checked = true;
					this.GMZoom50.Checked = true;
					return;
				}
			}
			else
			{
				if (num == 100)
				{
					this.StageZoom100.Checked = true;
					this.GMZoom100.Checked = true;
					return;
				}
				if (num == 200)
				{
					this.StageZoom200.Checked = true;
					this.GMZoom200.Checked = true;
					return;
				}
			}
			this.StageZoom100.Checked = true;
			this.GMZoom100.Checked = true;
			Global.config.draw.ZoomIndex = 1.0;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0001FB10 File Offset: 0x0001DD10
		private void MainWindow_Shown(object sender, EventArgs e)
		{
			this.EditorSystemPanel_Resize(this, new EventArgs());
			string text = "";
			if (Environment.GetCommandLineArgs().Length > 1 && !Global.state.ParseCommandline)
			{
				if (Environment.GetCommandLineArgs().Length > 2)
				{
					MessageBox.Show("起動時に指定できるファイルは一つだけです。", "ロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				else
				{
					string text2 = Environment.GetCommandLineArgs()[1];
					if (!File.Exists(text2))
					{
						MessageBox.Show("指定されたファイルが存在しないか、不正なパスです。" + Environment.NewLine + text2, "ロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
					else if (Path.GetExtension(text2) != Global.definition.ProjExt)
					{
						MessageBox.Show("指定されたファイルはプロジェクトファイルではありません。", "ロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
					else
					{
						text = text2;
					}
				}
			}
			if (text == "")
			{
				using (StartUp startUp = new StartUp())
				{
					if (startUp.ShowDialog() != DialogResult.OK)
					{
						base.Close();
						Application.Exit();
						return;
					}
					text = startUp.ProjectPath;
				}
			}
			using (ProjectLoading projectLoading = new ProjectLoading(text))
			{
				if (projectLoading.ShowDialog() == DialogResult.Abort)
				{
					base.Close();
					Application.Restart();
					return;
				}
			}
			this.MainDesigner.AddBuffer();
			this.EditorSystemPanel_Resize(this, new EventArgs());
			this.UpdateScrollbar();
			if (Global.cpd.UseLayer)
			{
				this.LayerState(true);
			}
			else
			{
				this.LayerState(false);
			}
			this.UpdateStageSelector();
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0001FC88 File Offset: 0x0001DE88
		public void UpdateTitle()
		{
			if (Global.cpd.project == null)
			{
				this.Text = Global.definition.AppNameFull + " - " + Global.definition.AppName;
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(Global.cpd.project.Name);
			if (Global.state.EditFlag)
			{
				stringBuilder.Append("*");
			}
			switch (this.EditTab.SelectedIndex)
			{
			case 0:
				stringBuilder.Append(" [グラフィカルデザイナ] - ");
				this.GMTool.Visible = true;
				this.GMEdit.Visible = true;
				this.GMStage.Visible = true;
				this.TMEdit.Visible = false;
				this.BMView.Visible = false;
				break;
			case 1:
				stringBuilder.Append(" [テキストエディタ] - ");
				this.GMTool.Visible = false;
				this.GMEdit.Visible = false;
				this.GMStage.Visible = true;
				this.TMEdit.Visible = true;
				this.BMView.Visible = false;
				break;
			case 2:
				stringBuilder.Append(" [テスト実行] - ");
				this.BMView.Visible = true;
				this.GMTool.Visible = false;
				this.GMEdit.Visible = false;
				this.GMStage.Visible = false;
				this.TMEdit.Visible = false;
				break;
			}
			stringBuilder.Append(Global.definition.AppName);
			this.Text = stringBuilder.ToString();
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0001FE14 File Offset: 0x0001E014
		private void MainDesigner_MouseMove(object sender, MouseEventArgs e)
		{
			Point p = default(Point);
			p.X = (e.X + Global.state.MapPoint.X) / this.MainDesigner.CurrentChipSize.Width;
			p.Y = (e.Y + Global.state.MapPoint.Y) / this.MainDesigner.CurrentChipSize.Height;
			this.GuiPositionInfo.Text = p.X.ToString() + "," + p.Y.ToString();
			if (!GUIDesigner.StageText.IsOverflow(p))
			{
				int num = Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize.bytesize : (Global.state.EditingForeground ? Global.cpd.runtime.Definitions.StageSize.bytesize : Global.cpd.runtime.Definitions.LayerSize.bytesize);
				string text;
				if (Global.state.EditingForeground)
				{
					text = Global.cpd.EditingMap[p.Y].Substring(p.X * num, num);
				}
				else
				{
					text = Global.cpd.EditingLayer[p.Y].Substring(p.X * num, num);
				}
				ChipsData chipsData;
				if (Global.state.MapEditMode)
				{
					if (text == Global.cpd.Worldchip[0].character || !this.MainDesigner.DrawWorldRef.ContainsKey(text))
					{
						this.ChipNavigator.Visible = false;
						return;
					}
					chipsData = this.MainDesigner.DrawWorldRef[text];
				}
				else if (Global.state.EditingForeground)
				{
					if (text == Global.cpd.Mapchip[0].character || !this.MainDesigner.DrawItemRef.ContainsKey(text))
					{
						this.ChipNavigator.Visible = false;
						return;
					}
					chipsData = this.MainDesigner.DrawItemRef[text];
				}
				else
				{
					if (text == Global.cpd.Layerchip[0].character || !this.MainDesigner.DrawLayerRef.ContainsKey(text))
					{
						this.ChipNavigator.Visible = false;
						return;
					}
					chipsData = this.MainDesigner.DrawLayerRef[text];
				}
				ChipData cschip = chipsData.GetCSChip();
				Size size = (cschip.size == default(Size)) ? Global.cpd.runtime.Definitions.ChipSize : cschip.size;
				if (this.ChipNavigator.Image != null)
				{
					this.ChipNavigator.Image.Dispose();
				}
				using (Bitmap bitmap = new Bitmap(size.Width, size.Height))
				{
					using (Graphics graphics = Graphics.FromImage(bitmap))
					{
						using (Brush brush = new SolidBrush(Global.state.Background))
						{
							graphics.FillRectangle(brush, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
						}
						if (Global.state.EditingForeground)
						{
							graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(cschip.pattern, size), GraphicsUnit.Pixel);
						}
						else
						{
							graphics.DrawImage(this.MainDesigner.DrawLayerOrig, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(cschip.pattern, size), GraphicsUnit.Pixel);
						}
					}
					this.ChipNavigator.Image = (Image)bitmap.Clone();
				}
				this.ChipNavigator.Text = string.Concat(new string[]
				{
					"[",
					chipsData.character,
					"]",
					cschip.name,
					"/",
					cschip.description
				});
				this.ChipNavigator.Visible = true;
			}
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00020288 File Offset: 0x0001E488
		public void CommitScrollbar()
		{
			this.GHorzScroll.Value = Global.state.MapPoint.X;
			this.GVirtScroll.Value = Global.state.MapPoint.Y;
			if (this.MainDesignerScroll != null)
			{
				this.MainDesignerScroll();
			}
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00002C9E File Offset: 0x00000E9E
		public void UpdateScrollbar()
		{
			this.UpdateScrollbar(Global.config.draw.ZoomIndex);
		}

		// Token: 0x06000147 RID: 327 RVA: 0x000202DC File Offset: 0x0001E4DC
		public void UpdateScrollbar(double oldZoomIndex)
		{
			double num = Global.config.draw.ZoomIndex / oldZoomIndex;
			Point point = new Point((int)((double)this.GHorzScroll.Value * num), (int)((double)this.GVirtScroll.Value * num));
			Size displaySize = this.MainDesigner.DisplaySize;
			Size mapMoveMax = new Size(displaySize.Width - this.MainDesigner.Width, displaySize.Height - this.MainDesigner.Height);
			Global.state.MapMoveMax = mapMoveMax;
			if (displaySize.Width <= this.MainDesigner.Width)
			{
				this.GHorzScroll.Enabled = false;
			}
			else
			{
				Runtime.DefinedData.StageSizeData stageSizeData = Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize : Global.cpd.runtime.Definitions.StageSize;
				this.GHorzScroll.Enabled = true;
				this.GHorzScroll.LargeChange = (int)((double)displaySize.Width / ((double)stageSizeData.x / 4.0) * Global.config.draw.ZoomIndex);
				this.GHorzScroll.Maximum = mapMoveMax.Width + this.GHorzScroll.LargeChange - 1;
			}
			if (displaySize.Height <= this.MainDesigner.Height)
			{
				this.GVirtScroll.Enabled = false;
			}
			else
			{
				Runtime.DefinedData.StageSizeData stageSizeData2 = Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize : Global.cpd.runtime.Definitions.StageSize;
				this.GVirtScroll.Enabled = true;
				this.GVirtScroll.LargeChange = (int)((double)displaySize.Height / ((double)stageSizeData2.y / 4.0) * Global.config.draw.ZoomIndex);
				this.GVirtScroll.Maximum = mapMoveMax.Height + this.GVirtScroll.LargeChange - 1;
				this.GVirtScroll.Maximum = mapMoveMax.Height + this.GVirtScroll.LargeChange - 1;
			}
			if (this.GHorzScroll.Enabled)
			{
				if (point.X > mapMoveMax.Width)
				{
					this.GHorzScroll.Value = mapMoveMax.Width;
				}
				else
				{
					this.GHorzScroll.Value = point.X;
				}
			}
			else
			{
				this.GHorzScroll.Value = 0;
			}
			Global.state.MapPoint.X = this.GHorzScroll.Value;
			if (this.GVirtScroll.Enabled)
			{
				if (point.Y > mapMoveMax.Height)
				{
					this.GVirtScroll.Value = mapMoveMax.Height;
				}
				else
				{
					this.GVirtScroll.Value = point.Y;
				}
			}
			else
			{
				this.GVirtScroll.Value = 0;
			}
			Global.state.MapPoint.Y = this.GVirtScroll.Value;
			if (this.MainDesignerScroll != null)
			{
				this.MainDesignerScroll();
			}
			this.MainDesigner.Refresh();
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00020604 File Offset: 0x0001E804
		private void EditorSystemPanel_Resize(object sender, EventArgs e)
		{
			this.MainDesigner.Width = this.EditorSystemPanel.Width - 20;
			this.MainDesigner.Height = this.EditorSystemPanel.Height - 20;
			this.GVirtScroll.Left = this.MainDesigner.Width;
			this.GVirtScroll.Height = this.MainDesigner.Height;
			this.GVirtScroll.Refresh();
			this.GHorzScroll.Width = this.MainDesigner.Width;
			this.GHorzScroll.Top = this.MainDesigner.Height;
			this.GHorzScroll.Refresh();
			this.UpdateScrollbar();
		}

		// Token: 0x06000149 RID: 329 RVA: 0x000206B8 File Offset: 0x0001E8B8
		private void EditTab_Deselecting(object sender, TabControlCancelEventArgs e)
		{
			if (e.TabPageIndex == 1 && !this.MainEditor.CanConvertTextSource() && MessageBox.Show(string.Concat(new string[]
			{
				"ステージのテキストが規定の形式を満たしていないため、デザイナでの編集やテスト実行はできません。",
				Environment.NewLine,
				"編集を続行すると、テキストでの編集結果は失われます。",
				Environment.NewLine,
				"続行してもよろしいですか？"
			}), "コンバート失敗", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
			{
				e.Cancel = true;
			}
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00020728 File Offset: 0x0001E928
		private void EditTab_Selecting(object sender, TabControlCancelEventArgs e)
		{
			if (e.TabPageIndex == 2)
			{
				if (Global.state.QuickTestrunSource != null)
				{
					Subsystem.MakeTestrun(Global.state.EdittingStage, Global.state.EdittingStage, Global.state.QuickTestrunSource);
					Global.state.QuickTestrunSource = null;
				}
				else if (Global.state.TestrunAll)
				{
					Subsystem.MakeTestrun(-1);
					Global.state.TestrunAll = false;
				}
				else
				{
					Subsystem.MakeTestrun(Global.state.EdittingStage);
				}
				if (!this.IntegrateBrowser.Navigate(Subsystem.GetTempFileWhere()))
				{
					e.Cancel = true;
				}
			}
		}

		// Token: 0x0600014B RID: 331 RVA: 0x000207C4 File Offset: 0x0001E9C4
		private void EditTab_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.Restartupping)
			{
				return;
			}
			if (this.EditTab.SelectedIndex == 1)
			{
				if (this.StageConvert)
				{
					if (Global.state.EditingForeground)
					{
						this.MainEditor.StageTextEditor.Text = (string)string.Join(Environment.NewLine, Global.cpd.EditingMap).Clone();
					}
					else
					{
						this.MainEditor.StageTextEditor.Text = (string)string.Join(Environment.NewLine, Global.cpd.EditingLayer).Clone();
					}
				}
				else
				{
					this.StageConvert = true;
				}
				this.MainEditor.BufferClear();
				this.MainEditor.AddBuffer();
				this.ChipList.Enabled = false;
				this.GuiChipList.Enabled = false;
				this.MainEditor.InitEditor();
				this.MSave.Enabled = false;
				this.MSaveAs.Enabled = false;
				this.MTSave.Enabled = false;
			}
			else
			{
				if (this.MainEditor.CanConvertTextSource())
				{
					if (Global.state.EditingForeground)
					{
						switch (Global.state.EdittingStage)
						{
						case 0:
							Global.cpd.project.StageData = (string[])this.MainEditor.StageTextEditor.Lines.Clone();
							Global.cpd.EditingMap = Global.cpd.project.StageData;
							break;
						case 1:
							Global.cpd.project.StageData2 = (string[])this.MainEditor.StageTextEditor.Lines.Clone();
							Global.cpd.EditingMap = Global.cpd.project.StageData2;
							break;
						case 2:
							Global.cpd.project.StageData3 = (string[])this.MainEditor.StageTextEditor.Lines.Clone();
							Global.cpd.EditingMap = Global.cpd.project.StageData3;
							break;
						case 3:
							Global.cpd.project.StageData4 = (string[])this.MainEditor.StageTextEditor.Lines.Clone();
							Global.cpd.EditingMap = Global.cpd.project.StageData4;
							break;
						case 4:
							Global.cpd.project.MapData = (string[])this.MainEditor.StageTextEditor.Lines.Clone();
							Global.cpd.EditingMap = Global.cpd.project.MapData;
							break;
						}
						this.MainEditor.StageTextEditor.Text = (string)string.Join(Environment.NewLine, Global.cpd.EditingMap).Clone();
						this.MainEditor.BufferClear();
						this.MainEditor.AddBuffer();
					}
					else
					{
						switch (Global.state.EdittingStage)
						{
						case 0:
							Global.cpd.project.LayerData = (string[])this.MainEditor.StageTextEditor.Lines.Clone();
							Global.cpd.EditingLayer = Global.cpd.project.LayerData;
							break;
						case 1:
							Global.cpd.project.LayerData2 = (string[])this.MainEditor.StageTextEditor.Lines.Clone();
							Global.cpd.EditingLayer = Global.cpd.project.LayerData2;
							break;
						case 2:
							Global.cpd.project.LayerData3 = (string[])this.MainEditor.StageTextEditor.Lines.Clone();
							Global.cpd.EditingLayer = Global.cpd.project.LayerData3;
							break;
						case 3:
							Global.cpd.project.LayerData4 = (string[])this.MainEditor.StageTextEditor.Lines.Clone();
							Global.cpd.EditingLayer = Global.cpd.project.LayerData4;
							break;
						}
						this.MainEditor.StageTextEditor.Text = (string)string.Join(Environment.NewLine, Global.cpd.EditingLayer).Clone();
						this.MainEditor.BufferClear();
						this.MainEditor.AddBuffer();
					}
					this.MainDesigner.ClearBuffer();
					this.MainDesigner.StageSourceToDrawBuffer();
					this.MainDesigner.AddBuffer();
				}
				this.ChipList.Enabled = true;
				this.GuiChipList.Enabled = true;
				this.MSave.Enabled = true;
				this.MSaveAs.Enabled = true;
				this.MTSave.Enabled = true;
				this.UpdateStatus("完了");
			}
			this.UpdateTitle();
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00020CA4 File Offset: 0x0001EEA4
		public void ChipItemReady()
		{
			MainWindow.CRID method = new MainWindow.CRID(this.ChipItemReadyInvoke);
			base.Invoke(method);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00020CC8 File Offset: 0x0001EEC8
		public void ChipItemReadyInvoke()
		{
			this.ChipList.BeginUpdate();
			this.ChipList.Items.Clear();
			ChipsData[] array;
			if (!Global.cpd.UseLayer || Global.state.EditingForeground)
			{
				if (Global.state.MapEditMode)
				{
					array = Global.cpd.Worldchip;
				}
				else
				{
					array = Global.cpd.Mapchip;
				}
			}
			else
			{
				array = Global.cpd.Layerchip;
			}
			ChipData chipData = default(ChipData);
			foreach (ChipsData chipsData in array)
			{
				chipData = chipsData.GetCSChip();
				if (chipData.description == "")
				{
					this.ChipList.Items.Add(chipData.name);
				}
				else
				{
					this.ChipList.Items.Add(chipData.name + "/" + chipData.description);
				}
			}
			if (this.ChipList.Items.Count != 0)
			{
				this.ChipList.SelectedIndex = 0;
			}
			this.ChipList.EndUpdate();
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00002CB5 File Offset: 0x00000EB5
		private void GVirtScroll_Scroll(object sender, ScrollEventArgs e)
		{
			Global.state.MapPoint.Y = this.GVirtScroll.Value;
			this.MainDesigner.Refresh();
			if (this.MainDesignerScroll != null)
			{
				this.MainDesignerScroll();
			}
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00002CEF File Offset: 0x00000EEF
		private void GHorzScroll_Scroll(object sender, ScrollEventArgs e)
		{
			Global.state.MapPoint.X = this.GHorzScroll.Value;
			this.MainDesigner.Refresh();
			if (this.MainDesignerScroll != null)
			{
				this.MainDesignerScroll();
			}
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00002D29 File Offset: 0x00000F29
		private void ShowGrid_Click(object sender, EventArgs e)
		{
			this.GMShowGrid.Checked = this.ShowGrid.Checked;
			Global.config.draw.DrawGrid = this.ShowGrid.Checked;
			this.MainDesigner.Refresh();
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00002D66 File Offset: 0x00000F66
		private void GMShowGrid_Click(object sender, EventArgs e)
		{
			this.ShowGrid.Checked = this.GMShowGrid.Checked;
			Global.config.draw.DrawGrid = this.GMShowGrid.Checked;
			this.MainDesigner.Refresh();
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00020DE0 File Offset: 0x0001EFE0
		private void ZoomInit()
		{
			this.StageZoom25.Checked = false;
			this.StageZoom50.Checked = false;
			this.StageZoom100.Checked = false;
			this.StageZoom200.Checked = false;
			this.GMZoom25.Checked = false;
			this.GMZoom50.Checked = false;
			this.GMZoom100.Checked = false;
			this.GMZoom200.Checked = false;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00020E50 File Offset: 0x0001F050
		private void StageZoom25_Click(object sender, EventArgs e)
		{
			this.ZoomInit();
			this.StageZoom25.Checked = true;
			this.GMZoom25.Checked = true;
			double zoomIndex = Global.config.draw.ZoomIndex;
			Global.config.draw.ZoomIndex = 0.25;
			this.UpdateScrollbar(zoomIndex);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00020EAC File Offset: 0x0001F0AC
		private void StageZoom50_Click(object sender, EventArgs e)
		{
			this.ZoomInit();
			this.StageZoom50.Checked = true;
			this.GMZoom50.Checked = true;
			double zoomIndex = Global.config.draw.ZoomIndex;
			Global.config.draw.ZoomIndex = 0.5;
			this.UpdateScrollbar(zoomIndex);
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00020F08 File Offset: 0x0001F108
		private void StageZoom100_Click(object sender, EventArgs e)
		{
			this.ZoomInit();
			this.StageZoom100.Checked = true;
			this.GMZoom100.Checked = true;
			double zoomIndex = Global.config.draw.ZoomIndex;
			Global.config.draw.ZoomIndex = 1.0;
			this.UpdateScrollbar(zoomIndex);
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00020F64 File Offset: 0x0001F164
		private void StageZoom200_Click(object sender, EventArgs e)
		{
			this.ZoomInit();
			this.StageZoom200.Checked = true;
			this.GMZoom200.Checked = true;
			double zoomIndex = Global.config.draw.ZoomIndex;
			Global.config.draw.ZoomIndex = 2.0;
			this.UpdateScrollbar(zoomIndex);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00020FC0 File Offset: 0x0001F1C0
		private void ChipList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.ChipList.SelectedIndex >= 0)
			{
				if (Global.state.MapEditMode)
				{
					Global.state.CurrentChip = Global.cpd.Worldchip[this.ChipList.SelectedIndex];
					return;
				}
				if (!Global.cpd.UseLayer || Global.state.EditingForeground)
				{
					Global.state.CurrentChip = Global.cpd.Mapchip[this.ChipList.SelectedIndex];
					return;
				}
				Global.state.CurrentChip = Global.cpd.Layerchip[this.ChipList.SelectedIndex];
			}
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00021070 File Offset: 0x0001F270
		private void state_UpdateCurrentChipInvoke()
		{
			this.ChipImage.Refresh();
			this.ChipDescription.Text = Global.state.CurrentChip.GetCSChip().name;
			if (Global.state.CurrentChip.GetCSChip().description != "")
			{
				this.ChipChar.Text = "[" + Global.state.CurrentChip.character + "]" + Global.state.CurrentChip.GetCSChip().description;
			}
			else
			{
				this.ChipChar.Text = "[" + Global.state.CurrentChip.character + "]";
			}
			if (this.DrawType.SelectedIndex == 3)
			{
				this.GuiChipList.Refresh();
				return;
			}
			int num = 0;
			if (Global.state.MapEditMode)
			{
				foreach (ChipsData chipsData in Global.cpd.Worldchip)
				{
					if (Global.state.CurrentChip.character == chipsData.character)
					{
						this.ChipList.SelectedIndex = num;
						return;
					}
					num++;
				}
				return;
			}
			if (!Global.cpd.UseLayer || Global.state.EditingForeground)
			{
				foreach (ChipsData chipsData2 in Global.cpd.Mapchip)
				{
					if (Global.state.CurrentChip.character == chipsData2.character)
					{
						this.ChipList.SelectedIndex = num;
						return;
					}
					num++;
				}
				return;
			}
			foreach (ChipsData chipsData3 in Global.cpd.Layerchip)
			{
				if (Global.state.CurrentChip.character == chipsData3.character)
				{
					this.ChipList.SelectedIndex = num;
					return;
				}
				num++;
			}
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0002126C File Offset: 0x0001F46C
		private void ChipImage_Paint(object sender, PaintEventArgs e)
		{
			if (this.MainDesigner.DrawChipOrig != null)
			{
				e.Graphics.InterpolationMode = InterpolationMode.High;
				ChipData cschip = Global.state.CurrentChip.GetCSChip();
				if (!Global.cpd.UseLayer || Global.state.EditingForeground)
				{
					if (cschip.size == default(Size))
					{
						e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(new Point(0, 0), this.ChipImage.Size), new Rectangle(cschip.pattern, Global.cpd.runtime.Definitions.ChipSize), GraphicsUnit.Pixel);
						return;
					}
					e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(new Point(0, 0), this.ChipImage.Size), new Rectangle(cschip.pattern, cschip.size), GraphicsUnit.Pixel);
					return;
				}
				else
				{
					if (cschip.size == default(Size))
					{
						e.Graphics.DrawImage(this.MainDesigner.DrawLayerOrig, new Rectangle(new Point(0, 0), this.ChipImage.Size), new Rectangle(cschip.pattern, Global.cpd.runtime.Definitions.ChipSize), GraphicsUnit.Pixel);
						return;
					}
					e.Graphics.DrawImage(this.MainDesigner.DrawLayerOrig, new Rectangle(new Point(0, 0), this.ChipImage.Size), new Rectangle(cschip.pattern, cschip.size), GraphicsUnit.Pixel);
				}
			}
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0002140C File Offset: 0x0001F60C
		private void ItemReload_Click(object sender, EventArgs e)
		{
			this.UpdateStatus("描画中...");
			Global.state.ForceNoBuffering = true;
			int tickCount = Environment.TickCount;
			this.MainDesigner.StageSourceToDrawBuffer();
			this.MainDesigner.Refresh();
			Global.state.ForceNoBuffering = false;
			this.UpdateStatus("完了(" + (Environment.TickCount - tickCount).ToString() + "ms)");
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00002DA3 File Offset: 0x00000FA3
		public void UpdateStatus(string t)
		{
			this.CurrentStatus.Text = t;
			this.GUIEditorStatus.Refresh();
		}

		// Token: 0x0600015C RID: 348 RVA: 0x0002147C File Offset: 0x0001F67C
		private void ItemUncheck()
		{
			this.ItemCursor.Checked = false;
			this.ItemPen.Checked = false;
			this.ItemLine.Checked = false;
			this.ItemRect.Checked = false;
			this.ItemFill.Checked = false;
			this.GMCursor.Checked = false;
			this.GMPen.Checked = false;
			this.GMLine.Checked = false;
			this.GMRect.Checked = false;
			this.GMFill.Checked = false;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00002DBC File Offset: 0x00000FBC
		private void ItemCursor_Click(object sender, EventArgs e)
		{
			this.ItemUncheck();
			this.ItemCursor.Checked = true;
			this.GMCursor.Checked = true;
			this.MainDesigner.CurrentTool = GUIDesigner.EditTool.Cursor;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00002DE8 File Offset: 0x00000FE8
		private void ItemPen_Click(object sender, EventArgs e)
		{
			this.ItemUncheck();
			this.ItemPen.Checked = true;
			this.GMPen.Checked = true;
			this.MainDesigner.CurrentTool = GUIDesigner.EditTool.Pen;
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00002E14 File Offset: 0x00001014
		private void ItemLine_Click(object sender, EventArgs e)
		{
			this.ItemUncheck();
			this.ItemLine.Checked = true;
			this.GMLine.Checked = true;
			this.MainDesigner.CurrentTool = GUIDesigner.EditTool.Line;
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00002E40 File Offset: 0x00001040
		private void ItemRect_Click(object sender, EventArgs e)
		{
			this.ItemUncheck();
			this.ItemRect.Checked = true;
			this.GMRect.Checked = true;
			this.MainDesigner.CurrentTool = GUIDesigner.EditTool.Rect;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00002E6C File Offset: 0x0000106C
		private void ItemFill_Click(object sender, EventArgs e)
		{
			this.ItemUncheck();
			this.ItemFill.Checked = true;
			this.GMFill.Checked = true;
			this.MainDesigner.CurrentTool = GUIDesigner.EditTool.Fill;
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00002E98 File Offset: 0x00001098
		private void ItemUndo_Click(object sender, EventArgs e)
		{
			this.MainDesigner.Undo();
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00002EA5 File Offset: 0x000010A5
		private void GMUndo_Click(object sender, EventArgs e)
		{
			this.ItemUndo_Click(this, new EventArgs());
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00002EB3 File Offset: 0x000010B3
		private void ItemRedo_Click(object sender, EventArgs e)
		{
			this.MainDesigner.Redo();
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00002EC0 File Offset: 0x000010C0
		private void GMRedo_Click(object sender, EventArgs e)
		{
			this.ItemRedo_Click(this, new EventArgs());
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00002ECE File Offset: 0x000010CE
		private void GMCut_Click(object sender, EventArgs e)
		{
			this.GuiCut.Checked = !this.GuiCut.Checked;
			this.GuiCut_Click(sender, e);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00002EF1 File Offset: 0x000010F1
		private void GMCopy_Click(object sender, EventArgs e)
		{
			this.GuiCopy.Checked = !this.GuiCopy.Checked;
			this.GuiCopy_Click(sender, e);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00002F14 File Offset: 0x00001114
		private void GMPaste_Click(object sender, EventArgs e)
		{
			this.GuiPaste.Checked = !this.GuiPaste.Checked;
			this.GuiPaste_Click(sender, e);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00021504 File Offset: 0x0001F704
		private void MainDesigner_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			Keys keyCode = e.KeyCode;
			if (keyCode - Keys.Left <= 3)
			{
				e.IsInputKey = true;
				return;
			}
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00021528 File Offset: 0x0001F728
		private void MainDesigner_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.None)
			{
				e.Handled = true;
				Keys keyCode = e.KeyCode;
				if (keyCode <= Keys.F)
				{
					switch (keyCode)
					{
					case Keys.Space:
						using (OverViewWindow overViewWindow = new OverViewWindow())
						{
							overViewWindow.ShowDialog();
							return;
						}
						break;
					case Keys.Prior:
					case Keys.Next:
					case Keys.End:
					case Keys.Home:
						goto IL_2CF;
					case Keys.Left:
						break;
					case Keys.Up:
						if (Global.config.draw.PageScroll)
						{
							this.MoveMainDesigner(new Size(0, this.MainDesigner.Height * -1));
							return;
						}
						this.MoveMainDesigner(new Size(0, Global.cpd.runtime.Definitions.ChipSize.Height * -1));
						return;
					case Keys.Right:
						if (Global.config.draw.PageScroll)
						{
							this.MoveMainDesigner(new Size(this.MainDesigner.Width, 0));
							return;
						}
						this.MoveMainDesigner(new Size(Global.cpd.runtime.Definitions.ChipSize.Width, 0));
						return;
					case Keys.Down:
						if (Global.config.draw.PageScroll)
						{
							this.MoveMainDesigner(new Size(0, this.MainDesigner.Height));
							return;
						}
						this.MoveMainDesigner(new Size(0, Global.cpd.runtime.Definitions.ChipSize.Height));
						return;
					default:
						switch (keyCode)
						{
						case Keys.C:
							this.ItemCursor_Click(this, new EventArgs());
							return;
						case Keys.D:
							this.ItemPen_Click(this, new EventArgs());
							return;
						case Keys.E:
							goto IL_2CF;
						case Keys.F:
							this.ItemFill_Click(this, new EventArgs());
							return;
						default:
							goto IL_2CF;
						}
						break;
					}
					if (Global.config.draw.PageScroll)
					{
						this.MoveMainDesigner(new Size(this.MainDesigner.Width * -1, 0));
						return;
					}
					this.MoveMainDesigner(new Size(Global.cpd.runtime.Definitions.ChipSize.Width * -1, 0));
					return;
				}
				else
				{
					if (keyCode == Keys.R)
					{
						this.ItemRect_Click(this, new EventArgs());
						return;
					}
					if (keyCode == Keys.S)
					{
						this.ItemLine_Click(this, new EventArgs());
						return;
					}
					switch (keyCode)
					{
					case Keys.F1:
						this.StageSelectionChange(0);
						return;
					case Keys.F2:
						this.StageSelectionChange(1);
						return;
					case Keys.F3:
						this.StageSelectionChange(2);
						return;
					case Keys.F4:
						this.StageSelectionChange(3);
						return;
					case Keys.F6:
						this.StageSelectionChange(4);
						return;
					case Keys.F7:
						this.EditPatternChip_Click(this, new EventArgs());
						return;
					case Keys.F8:
						this.EditBackground_Click(this, new EventArgs());
						return;
					case Keys.F9:
						this.StageZoom25_Click(this, new EventArgs());
						return;
					case Keys.F10:
						this.StageZoom50_Click(this, new EventArgs());
						return;
					case Keys.F11:
						this.StageZoom100_Click(this, new EventArgs());
						return;
					case Keys.F12:
						this.StageZoom200_Click(this, new EventArgs());
						return;
					}
				}
				IL_2CF:
				e.Handled = false;
				return;
			}
			if (e.Shift)
			{
				if (e.KeyCode != Keys.ShiftKey)
				{
					return;
				}
				if (this.MainDesigner.CurrentTool != GUIDesigner.EditTool.Cursor)
				{
					this.OldET = this.MainDesigner.CurrentTool;
					this.MainDesigner.CurrentTool = GUIDesigner.EditTool.Cursor;
					return;
				}
			}
			else if (e.Control && e.Control)
			{
				e.Handled = true;
				Keys keyCode2 = e.KeyCode;
				if (keyCode2 <= Keys.G)
				{
					if (keyCode2 == Keys.C)
					{
						this.GMCopy_Click(this, new EventArgs());
						goto IL_3E8;
					}
					if (keyCode2 == Keys.G)
					{
						this.ShowGrid.Checked = !this.ShowGrid.Checked;
						this.ShowGrid_Click(this, new EventArgs());
						goto IL_3E8;
					}
				}
				else
				{
					switch (keyCode2)
					{
					case Keys.V:
						this.GMPaste_Click(this, new EventArgs());
						goto IL_3E8;
					case Keys.W:
						break;
					case Keys.X:
						this.GMCut_Click(this, new EventArgs());
						goto IL_3E8;
					default:
						if (keyCode2 == Keys.F10)
						{
							this.ProjectConfig_Click(this, new EventArgs());
							goto IL_3E8;
						}
						if (keyCode2 == Keys.F11)
						{
							this.MSysConfig_Click(this, new EventArgs());
							goto IL_3E8;
						}
						break;
					}
				}
				e.Handled = false;
				IL_3E8:
				this.MainDesigner.Focus();
			}
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0002193C File Offset: 0x0001FB3C
		public void MoveMainDesigner(Size Scroll)
		{
			State state = Global.state;
			state.MapPoint.X = state.MapPoint.X + Scroll.Width;
			State state2 = Global.state;
			state2.MapPoint.Y = state2.MapPoint.Y + Scroll.Height;
			Global.state.AdjustMapPoint();
			Global.MainWnd.CommitScrollbar();
			this.MainDesigner.Refresh();
		}

		// Token: 0x0600016C RID: 364 RVA: 0x000219B0 File Offset: 0x0001FBB0
		private void MainDesigner_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.None)
			{
				if (e.KeyCode != Keys.ShiftKey)
				{
					return;
				}
				if (this.OldET != GUIDesigner.EditTool.Cursor && this.MainDesigner.CurrentTool == GUIDesigner.EditTool.Cursor)
				{
					this.MainDesigner.CurrentTool = this.OldET;
					this.OldET = GUIDesigner.EditTool.Cursor;
				}
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00002F37 File Offset: 0x00001137
		public void GuiCut_Click(object sender, EventArgs e)
		{
			if (this.GuiCut.Checked)
			{
				this.GuiCopy.Checked = false;
				this.GuiPaste.Checked = false;
				this.MainDesigner.CopyPaste = GUIDesigner.CopyPasteTool.Cut;
				return;
			}
			this.CopyPasteInit();
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00002F71 File Offset: 0x00001171
		public void GuiCopy_Click(object sender, EventArgs e)
		{
			if (this.GuiCopy.Checked)
			{
				this.GuiCut.Checked = false;
				this.GuiPaste.Checked = false;
				this.MainDesigner.CopyPaste = GUIDesigner.CopyPasteTool.Copy;
				return;
			}
			this.CopyPasteInit();
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00021A00 File Offset: 0x0001FC00
		public void GuiPaste_Click(object sender, EventArgs e)
		{
			if (!this.GuiPaste.Checked)
			{
				this.CopyPasteInit();
				return;
			}
			this.MainDesigner.ClipedString = Clipboard.GetText();
			if (this.MainDesigner.CheckBuffer())
			{
				this.GuiCopy.Checked = false;
				this.GuiCut.Checked = false;
				this.MainDesigner.CopyPaste = GUIDesigner.CopyPasteTool.Paste;
				return;
			}
			this.CopyPasteInit();
			this.UpdateStatus("クリップテキストが不正です");
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00002FAB File Offset: 0x000011AB
		public void CopyPasteInit()
		{
			this.GuiCopy.Checked = false;
			this.GuiCut.Checked = false;
			this.GuiPaste.Checked = false;
			this.MainDesigner.CopyPaste = GUIDesigner.CopyPasteTool.None;
			this.MainDesigner.Refresh();
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00002FE8 File Offset: 0x000011E8
		private void GUIProjConfig_Click(object sender, EventArgs e)
		{
			this.ProjectConfig_Click(this, new EventArgs());
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00021A74 File Offset: 0x0001FC74
		private void DrawType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.DrawType.SelectedIndex == 3)
			{
				this.ChipList.Visible = false;
				this.GuiChipList.Visible = true;
				this.GuiChipList.ResizeInvoke();
				this.GuiChipList.Refresh();
				return;
			}
			this.ChipList.Visible = true;
			this.GuiChipList.Visible = false;
			this.ChipList.DrawMode = DrawMode.OwnerDrawFixed;
			this.ChipList.DrawMode = DrawMode.OwnerDrawVariable;
			this.ChipList.Refresh();
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00021AFC File Offset: 0x0001FCFC
		private void ChipList_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index == -1)
			{
				return;
			}
			e.DrawBackground();
			try
			{
				switch (this.DrawType.SelectedIndex)
				{
				case 0:
					using (Brush brush = new SolidBrush(e.ForeColor))
					{
						if (!Global.cpd.UseLayer || Global.state.EditingForeground)
						{
							ChipData cschip;
							if (Global.state.MapEditMode)
							{
								cschip = Global.cpd.Worldchip[e.Index].GetCSChip();
							}
							else
							{
								cschip = Global.cpd.Mapchip[e.Index].GetCSChip();
							}
							if (cschip.size == default(Size))
							{
								e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(e.Bounds.Location, new Size(e.Bounds.Height, e.Bounds.Height)), new Rectangle(cschip.pattern, Global.cpd.runtime.Definitions.ChipSize), GraphicsUnit.Pixel);
							}
							else
							{
								e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(e.Bounds.Location, new Size(e.Bounds.Height, e.Bounds.Height)), new Rectangle(cschip.pattern, cschip.size), GraphicsUnit.Pixel);
							}
						}
						else
						{
							ChipData cschip2;
							if (Global.state.MapEditMode)
							{
								cschip2 = Global.cpd.Worldchip[e.Index].GetCSChip();
							}
							else
							{
								cschip2 = Global.cpd.Mapchip[e.Index].GetCSChip();
							}
							if (Global.cpd.Layerchip[e.Index].GetCSChip().size == default(Size))
							{
								e.Graphics.DrawImage(this.MainDesigner.DrawLayerOrig, new Rectangle(e.Bounds.Location, new Size(e.Bounds.Height, e.Bounds.Height)), new Rectangle(cschip2.pattern, Global.cpd.runtime.Definitions.ChipSize), GraphicsUnit.Pixel);
							}
							else
							{
								e.Graphics.DrawImage(this.MainDesigner.DrawLayerOrig, new Rectangle(e.Bounds.Location, new Size(e.Bounds.Height, e.Bounds.Height)), new Rectangle(cschip2.pattern, cschip2.size), GraphicsUnit.Pixel);
							}
						}
						e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(), e.Font, brush, new Rectangle(e.Bounds.X + e.Bounds.Height, e.Bounds.Y, e.Bounds.Width - e.Bounds.Height, e.Bounds.Height));
						goto IL_6AF;
					}
					break;
				case 1:
					break;
				case 2:
					goto IL_3C9;
				default:
					goto IL_6AF;
				}
				using (Brush brush2 = new SolidBrush(e.ForeColor))
				{
					e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(), e.Font, brush2, e.Bounds);
					goto IL_6AF;
				}
				IL_3C9:
				using (Brush brush3 = new SolidBrush(e.ForeColor))
				{
					int width;
					if (!Global.cpd.UseLayer || Global.state.EditingForeground)
					{
						ChipData cschip3;
						if (Global.state.MapEditMode)
						{
							cschip3 = Global.cpd.Worldchip[e.Index].GetCSChip();
						}
						else
						{
							cschip3 = Global.cpd.Mapchip[e.Index].GetCSChip();
						}
						if (cschip3.size == default(Size))
						{
							e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(e.Bounds.Location, Global.cpd.runtime.Definitions.ChipSize), new Rectangle(cschip3.pattern, Global.cpd.runtime.Definitions.ChipSize), GraphicsUnit.Pixel);
							width = Global.cpd.runtime.Definitions.ChipSize.Width;
						}
						else
						{
							e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(e.Bounds.Location, cschip3.size), new Rectangle(cschip3.pattern, cschip3.size), GraphicsUnit.Pixel);
							width = cschip3.size.Width;
						}
					}
					else
					{
						ChipData cschip4 = Global.cpd.Layerchip[e.Index].GetCSChip();
						if (cschip4.size == default(Size))
						{
							e.Graphics.DrawImage(this.MainDesigner.DrawLayerOrig, new Rectangle(e.Bounds.Location, Global.cpd.runtime.Definitions.ChipSize), new Rectangle(cschip4.pattern, Global.cpd.runtime.Definitions.ChipSize), GraphicsUnit.Pixel);
							width = Global.cpd.runtime.Definitions.ChipSize.Width;
						}
						else
						{
							e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(e.Bounds.Location, cschip4.size), new Rectangle(cschip4.pattern, cschip4.size), GraphicsUnit.Pixel);
							width = cschip4.size.Width;
						}
					}
					e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(), e.Font, brush3, new Rectangle(e.Bounds.X + width, e.Bounds.Y, e.Bounds.Width - width, e.Bounds.Height));
				}
				IL_6AF:
				if (!this.ChipList.Enabled)
				{
					using (Brush brush4 = new SolidBrush(Color.FromArgb(160, Color.White)))
					{
						e.Graphics.FillRectangle(brush4, e.Bounds);
					}
				}
			}
			catch
			{
			}
			e.DrawFocusRectangle();
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00022284 File Offset: 0x00020484
		private void ChipList_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			switch (this.DrawType.SelectedIndex)
			{
			case 0:
				e.ItemHeight = this.ChipList.ItemHeight;
				return;
			case 1:
				e.ItemHeight = this.ChipList.ItemHeight;
				return;
			case 2:
			{
				if (Global.state.MapEditMode)
				{
					Size size = Global.cpd.Worldchip[e.Index].GetCSChip().size;
					int height;
					if (size.Height == 0)
					{
						height = Global.cpd.runtime.Definitions.ChipSize.Height;
					}
					else
					{
						Size size2 = Global.cpd.Worldchip[e.Index].GetCSChip().size;
						height = size2.Height;
					}
					e.ItemHeight = height;
					return;
				}
				if (!Global.cpd.UseLayer || Global.state.EditingForeground)
				{
					Size size3 = Global.cpd.Mapchip[e.Index].GetCSChip().size;
					int height2;
					if (size3.Height == 0)
					{
						height2 = Global.cpd.runtime.Definitions.ChipSize.Height;
					}
					else
					{
						Size size4 = Global.cpd.Mapchip[e.Index].GetCSChip().size;
						height2 = size4.Height;
					}
					e.ItemHeight = height2;
					return;
				}
				Size size5 = Global.cpd.Layerchip[e.Index].GetCSChip().size;
				int height3;
				if (size5.Height == 0)
				{
					height3 = Global.cpd.runtime.Definitions.ChipSize.Height;
				}
				else
				{
					Size size6 = Global.cpd.Layerchip[e.Index].GetCSChip().size;
					height3 = size6.Height;
				}
				e.ItemHeight = height3;
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00002FF6 File Offset: 0x000011F6
		private void TMUndo_Click(object sender, EventArgs e)
		{
			this.MainEditor.Undo();
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00003003 File Offset: 0x00001203
		private void TMRedo_Click(object sender, EventArgs e)
		{
			this.MainEditor.Redo();
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00003010 File Offset: 0x00001210
		private void TextCut_Click(object sender, EventArgs e)
		{
			this.MainEditor.StageTextEditor.Cut();
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00003022 File Offset: 0x00001222
		private void TextCopy_Click(object sender, EventArgs e)
		{
			this.MainEditor.StageTextEditor.Copy();
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00003034 File Offset: 0x00001234
		private void TextPaste_Click(object sender, EventArgs e)
		{
			this.MainEditor.StageTextEditor.Paste();
		}

		// Token: 0x0600017A RID: 378 RVA: 0x00002C90 File Offset: 0x00000E90
		private void ProjTestRun(object sender, EventArgs e)
		{
			this.EditTab.SelectedIndex = 2;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00022460 File Offset: 0x00020660
		private void MainWindow_Activated(object sender, EventArgs e)
		{
			if (this.EditTab.SelectedIndex == 0)
			{
				this.MainDesigner.Refresh();
			}
			if (Global.state.Testrun != null && Global.config.testRun.KillTestrunOnFocus)
			{
				if (!Global.state.Testrun.HasExited && !Global.state.Testrun.CloseMainWindow())
				{
					try
					{
						Global.state.Testrun.Kill();
					}
					catch
					{
						MessageBox.Show("ブラウザプロセスを終了できませんでした。" + Environment.NewLine + "ブラウザを終了する設定をオフにします。", "終了失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						Global.config.testRun.KillTestrunOnFocus = false;
					}
				}
				Global.state.Testrun.Close();
				Global.state.Testrun.Dispose();
			}
			Global.state.Testrun = null;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00003046 File Offset: 0x00001246
		private void MainDesigner_Enter(object sender, EventArgs e)
		{
			if (this.OldET != GUIDesigner.EditTool.Cursor && this.MainDesigner.CurrentTool == GUIDesigner.EditTool.Cursor)
			{
				this.MainDesigner.CurrentTool = this.OldET;
				this.OldET = GUIDesigner.EditTool.Cursor;
			}
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0002254C File Offset: 0x0002074C
		private void RestartUp(string su)
		{
			this.Restartupping = true;
			this.MEditStage1_Click(this, new EventArgs());
			this.EditPatternChip_Click(this, new EventArgs());
			Global.state.EditingForeground = true;
			this.EditTab.SelectedIndex = 0;
			this.SideTab.SelectedIndex = 0;
			this.ChipList.Enabled = true;
			this.GuiChipList.Enabled = true;
			this.MSave.Enabled = true;
			this.MSaveAs.Enabled = true;
			this.MTSave.Enabled = true;
			Global.state.EditFlag = false;
			using (ProjectLoading projectLoading = new ProjectLoading(su))
			{
				if (projectLoading.ShowDialog() == DialogResult.Abort)
				{
					base.Close();
					Application.Restart();
					return;
				}
			}
			this.MainDesigner.ClearBuffer();
			this.MainDesigner.AddBuffer();
			this.EditorSystemPanel_Resize(this, new EventArgs());
			this.UpdateScrollbar();
			if (Global.cpd.UseLayer)
			{
				this.LayerState(true);
			}
			else
			{
				this.LayerState(false);
			}
			this.UpdateStageSelector();
			this.Restartupping = false;
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00003075 File Offset: 0x00001275
		private void LayerState(bool enabled)
		{
			this.ViewUnactiveLayer.Enabled = enabled;
			this.LayerMenu.Visible = enabled;
			this.StageLayer.Visible = enabled;
			this.MainEditor.StageLayer.Visible = enabled;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0002266C File Offset: 0x0002086C
		private void MNew_Click(object sender, EventArgs e)
		{
			if (Global.state.EditFlag && MessageBox.Show(string.Concat(new string[]
			{
				"データが保存されていません。",
				Environment.NewLine,
				"保存していないデータは失われます。",
				Environment.NewLine,
				"続行してよろしいですか？"
			}), "保存の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
			{
				return;
			}
			using (NewProject newProject = new NewProject())
			{
				if (newProject.ShowDialog() == DialogResult.OK)
				{
					this.RestartUp(newProject.CreatedProject);
				}
			}
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00022700 File Offset: 0x00020900
		private void MOpen_Click(object sender, EventArgs e)
		{
			if (Global.state.EditFlag && MessageBox.Show(string.Concat(new string[]
			{
				"データが保存されていません。",
				Environment.NewLine,
				"保存していないデータは失われます。",
				Environment.NewLine,
				"続行してよろしいですか？"
			}), "保存の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
			{
				return;
			}
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = string.Concat(new string[]
				{
					Global.definition.AppName,
					" プロジェクト (*",
					Global.definition.ProjExt,
					")|*",
					Global.definition.ProjExt,
					"|全てのファイル|*.*"
				});
				openFileDialog.InitialDirectory = Global.config.lastData.ProjDirF;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					this.RestartUp(openFileDialog.FileName);
				}
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x000030AC File Offset: 0x000012AC
		private void MSave_Click(object sender, EventArgs e)
		{
			Global.cpd.project.SaveXML(Global.cpd.filename);
			Global.state.EditFlag = false;
			this.UpdateStatus("保存しました");
		}

		// Token: 0x06000182 RID: 386 RVA: 0x000227FC File Offset: 0x000209FC
		private void MSaveAs_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog saveFileDialog = new SaveFileDialog())
			{
				saveFileDialog.Filter = string.Concat(new string[]
				{
					Global.definition.AppName,
					" プロジェクト (*",
					Global.definition.ProjExt,
					")|*",
					Global.definition.ProjExt,
					"|全てのファイル|*.*"
				});
				saveFileDialog.InitialDirectory = Global.cpd.where;
				saveFileDialog.FileName = Path.GetFileName(Global.cpd.filename);
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					DialogResult dialogResult = MessageBox.Show("保存先のディレクトリに元のディレクトリのファイルをコピーしますか？" + Environment.NewLine + "移動しないとテスト実行に失敗したり、編集に失敗したりする恐れがあります。", "ファイルのコピー", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
					if (dialogResult == DialogResult.Yes)
					{
						foreach (string text in Directory.GetFiles(Global.cpd.where, "*", SearchOption.TopDirectoryOnly))
						{
							string text2 = Path.Combine(Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileName(text));
							if (!File.Exists(text2) || MessageBox.Show(string.Concat(new string[]
							{
								text2,
								Environment.NewLine,
								"はすでに存在しています。",
								Environment.NewLine,
								"上書きしてもよろしいですか？"
							}), "上書きの警告", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.No)
							{
								File.Copy(text, text2, true);
							}
						}
						File.Delete(Path.Combine(Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileName(Global.cpd.filename)));
					}
					else if (dialogResult == DialogResult.Cancel)
					{
						return;
					}
					Global.cpd.where = Path.GetDirectoryName(saveFileDialog.FileName);
					Global.cpd.filename = saveFileDialog.FileName;
				}
				this.MSave_Click(this, new EventArgs());
			}
		}

		// Token: 0x06000183 RID: 387 RVA: 0x000229D4 File Offset: 0x00020BD4
		private void MWriteHTML_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog saveFileDialog = new SaveFileDialog())
			{
				saveFileDialog.DefaultExt = Global.cpd.runtime.DefaultConfigurations.FileExt;
				saveFileDialog.AddExtension = true;
				saveFileDialog.Filter = "出力ファイル(*." + Global.cpd.runtime.DefaultConfigurations.FileExt + ")|*" + Global.cpd.runtime.DefaultConfigurations.FileExt;
				if (saveFileDialog.ShowDialog() != DialogResult.Cancel)
				{
					if (Path.GetDirectoryName(saveFileDialog.FileName) != Global.cpd.where)
					{
						using (OutputControl outputControl = new OutputControl(Path.GetDirectoryName(saveFileDialog.FileName)))
						{
							if (outputControl.ShowDialog() == DialogResult.Cancel)
							{
								return;
							}
						}
					}
					using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName, false, Global.config.localSystem.FileEncoding))
					{
						string value = Subsystem.MakeHTMLCode(0);
						streamWriter.Write(value);
						streamWriter.Close();
					}
					MessageBox.Show("出力しました。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
			}
		}

		// Token: 0x06000184 RID: 388 RVA: 0x000030DD File Offset: 0x000012DD
		private void MExit_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00022B18 File Offset: 0x00020D18
		private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing && Global.state.EditFlag && MessageBox.Show("編集データが保存されていません。" + Environment.NewLine + "終了してもよろしいですか？", Global.definition.AppName + "の終了", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
			{
				e.Cancel = true;
			}
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00022B74 File Offset: 0x00020D74
		private void ViewUnactiveLayer_Click(object sender, EventArgs e)
		{
			Global.state.DrawUnactiveLayer = !Global.state.DrawUnactiveLayer;
			this.ViewUnactiveLayer.Checked = Global.state.DrawUnactiveLayer;
			this.MDUnactiveLayer.Checked = Global.state.DrawUnactiveLayer;
			if (!this.ViewUnactiveLayer.Checked)
			{
				if (Global.state.TransparentUnactiveLayer)
				{
					this.TransparentUnactiveLayer.Checked = false;
					this.TransparentUnactiveLayer.Enabled = false;
					Global.state.TransparentUnactiveLayer = false;
					if (Global.state.EditingForeground)
					{
						this.MainDesigner.UpdateBackgroundBuffer();
					}
					else
					{
						this.MainDesigner.UpdateForegroundBuffer();
					}
				}
				this.MTUnactiveLayer.Checked = false;
				this.MTUnactiveLayer.Enabled = false;
			}
			else
			{
				this.TransparentUnactiveLayer.Enabled = true;
				this.MTUnactiveLayer.Enabled = true;
			}
			this.MainDesigner.Refresh();
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00022C60 File Offset: 0x00020E60
		private void TransparentUnactiveLayer_Click(object sender, EventArgs e)
		{
			this.UpdateStatus("描画モードを切り替えています...");
			Global.state.TransparentUnactiveLayer = !Global.state.TransparentUnactiveLayer;
			this.TransparentUnactiveLayer.Checked = Global.state.TransparentUnactiveLayer;
			this.MTUnactiveLayer.Checked = Global.state.TransparentUnactiveLayer;
			if (this.TransparentUnactiveLayer.Checked)
			{
				this.MainDesigner.InitTransparent();
			}
			else if (Global.state.EditingForeground)
			{
				this.MainDesigner.UpdateBackgroundBuffer();
			}
			else
			{
				this.MainDesigner.UpdateForegroundBuffer();
			}
			this.MainDesigner.Refresh();
			this.UpdateStatus("完了");
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00022D10 File Offset: 0x00020F10
		public void EditPatternChip_Click(object sender, EventArgs e)
		{
			if (Global.state.EditingForeground)
			{
				return;
			}
			if (!this.ChangePreCheck())
			{
				return;
			}
			Global.state.EditingForeground = true;
			this.EditPatternChip.Checked = true;
			this.PatternChipLayer.Checked = true;
			this.MainEditor.PatternChipLayer.Checked = true;
			this.EditBackground.Checked = false;
			this.BackgroundLayer.Checked = false;
			this.MainEditor.BackgroundLayer.Checked = false;
			this.UpdateLayer();
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00022D98 File Offset: 0x00020F98
		public void EditBackground_Click(object sender, EventArgs e)
		{
			if (!Global.state.EditingForeground)
			{
				return;
			}
			if (!this.ChangePreCheck())
			{
				return;
			}
			Global.state.EditingForeground = false;
			this.EditPatternChip.Checked = false;
			this.PatternChipLayer.Checked = false;
			this.MainEditor.PatternChipLayer.Checked = false;
			this.EditBackground.Checked = true;
			this.BackgroundLayer.Checked = true;
			this.MainEditor.BackgroundLayer.Checked = true;
			this.UpdateLayer();
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00022E20 File Offset: 0x00021020
		private bool ChangePreCheck()
		{
			return this.EditTab.SelectedIndex != 1 || this.MainEditor.CanConvertTextSource() || MessageBox.Show(string.Concat(new string[]
			{
				"ステージのテキストが規定の形式を満たしていないため、テキストを反映できません。",
				Environment.NewLine,
				"レイヤーを切り替えると、編集結果は失われます。",
				Environment.NewLine,
				"続行してもよろしいですか？"
			}), "コンバート失敗", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) != DialogResult.Cancel;
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00022E90 File Offset: 0x00021090
		private void StageSelectionChange(int newValue)
		{
			if (Global.state.EdittingStage == newValue)
			{
				return;
			}
			this.UpdateStatus("ステージを切り替えています...");
			this.MEditStage1.Checked = false;
			this.MEditStage2.Checked = false;
			this.MEditStage3.Checked = false;
			this.MEditStage4.Checked = false;
			this.MEditMap.Checked = false;
			Global.state.MapEditMode = false;
			switch (newValue)
			{
			case 0:
				Global.cpd.EditingMap = Global.cpd.project.StageData;
				Global.cpd.EditingLayer = Global.cpd.project.LayerData;
				this.MEditStage1.Checked = true;
				Global.state.Background = Global.cpd.project.Config.Background;
				this.LayerState(Global.cpd.UseLayer);
				break;
			case 1:
				Global.cpd.EditingMap = Global.cpd.project.StageData2;
				Global.cpd.EditingLayer = Global.cpd.project.LayerData2;
				this.MEditStage2.Checked = true;
				Global.state.Background = Global.cpd.project.Config.Background2;
				this.LayerState(Global.cpd.UseLayer);
				break;
			case 2:
				Global.cpd.EditingMap = Global.cpd.project.StageData3;
				Global.cpd.EditingLayer = Global.cpd.project.LayerData3;
				this.MEditStage3.Checked = true;
				Global.state.Background = Global.cpd.project.Config.Background3;
				this.LayerState(Global.cpd.UseLayer);
				break;
			case 3:
				Global.cpd.EditingMap = Global.cpd.project.StageData4;
				Global.cpd.EditingLayer = Global.cpd.project.LayerData4;
				this.MEditStage4.Checked = true;
				Global.state.Background = Global.cpd.project.Config.Background4;
				this.LayerState(Global.cpd.UseLayer);
				break;
			case 4:
				Global.cpd.EditingMap = Global.cpd.project.MapData;
				this.MEditMap.Checked = true;
				Global.state.MapEditMode = true;
				Global.state.Background = Global.cpd.project.Config.BackgroundM; // なぜか複数ステージでステージ開始時画面の背景色を使っている
				this.LayerState(false);
				Global.state.EditingForeground = true;
				break;
			}
			Global.state.EdittingStage = newValue;
			this.MainDesigner.ForceBufferResize();
			this.UpdateLayer();
			this.UpdateScrollbar();
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00023160 File Offset: 0x00021360
		private void UpdateStageSelector()
		{
			this.MEditStage2.Enabled = (Global.cpd.project.Config.StageNum >= 2) || Global.cpd.project.Config.UseWorldmap;
			this.MEditStage3.Enabled = (Global.cpd.project.Config.StageNum >= 3) || Global.cpd.project.Config.UseWorldmap;
			this.MEditStage4.Enabled = (Global.cpd.project.Config.StageNum >= 4) || Global.cpd.project.Config.UseWorldmap;
			this.MEditMap.Enabled = Global.cpd.project.Config.UseWorldmap;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x000231FC File Offset: 0x000213FC
		private void UpdateLayer()
		{
			if (this.EditTab.SelectedIndex == 0)
			{
				this.UpdateStatus("描画を更新しています...");
				if (Global.state.EditingForeground)
				{
					Global.state.CurrentChip = Global.cpd.Mapchip[0];
				}
				else
				{
					Global.state.CurrentChip = Global.cpd.Layerchip[0];
				}
				this.ChipItemReadyInvoke();
				this.MainDesigner.ClearBuffer();
				this.MainDesigner.UpdateForegroundBuffer();
				this.MainDesigner.UpdateBackgroundBuffer();
				if (Global.state.TransparentUnactiveLayer)
				{
					this.MainDesigner.InitTransparent();
				}
				this.MainDesigner.AddBuffer();
				this.MainDesigner.Refresh();
				this.UpdateStatus("完了");
				return;
			}
			if (this.EditTab.SelectedIndex == 1)
			{
				if (Global.state.EditingForeground)
				{
					Global.cpd.EditingLayer = (string[])this.MainEditor.StageTextEditor.Lines.Clone();
					this.MainEditor.StageTextEditor.Text = (string)string.Join(Environment.NewLine, Global.cpd.EditingMap).Clone();
					this.MainEditor.BufferClear();
					this.MainEditor.AddBuffer();
					return;
				}
				Global.cpd.EditingMap = (string[])this.MainEditor.StageTextEditor.Lines.Clone();
				this.MainEditor.StageTextEditor.Text = (string)string.Join(Environment.NewLine, Global.cpd.EditingLayer).Clone();
				this.MainEditor.BufferClear();
				this.MainEditor.AddBuffer();
			}
		}

		// Token: 0x0600018E RID: 398 RVA: 0x000030E5 File Offset: 0x000012E5
		private void DrawType_Resize(object sender, EventArgs e)
		{
			this.DrawType.Refresh();
		}

		// Token: 0x0600018F RID: 399 RVA: 0x000233B4 File Offset: 0x000215B4
		public void MSysConfig_Click(object sender, EventArgs e)
		{
			using (SideConfig sideConfig = new SideConfig())
			{
				if (sideConfig.ShowDialog() == DialogResult.OK && this.EditTab.SelectedIndex == 0)
				{
					this.UpdateStatus("描画を更新しています...");
					this.GuiChipList.Refresh();
					this.MainDesigner.UpdateBackgroundBuffer();
					this.MainDesigner.UpdateForegroundBuffer();
					this.MainDesigner.InitTransparent();
					this.MainDesigner.Refresh();
					this.MasaoConfigList.Reload();
					this.UpdateStatus("完了");
				}
			}
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00023454 File Offset: 0x00021654
		private void MVersion_Click(object sender, EventArgs e)
		{
			using (VersionInfo versionInfo = new VersionInfo())
			{
				versionInfo.ShowDialog();
			}
		}

		// Token: 0x06000191 RID: 401 RVA: 0x000030F2 File Offset: 0x000012F2
		private void SideTab_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.SideTab.SelectedIndex == 1)
			{
				this.MasaoConfigList.Prepare();
			}
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0002348C File Offset: 0x0002168C
		private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (e.CloseReason != CloseReason.ApplicationExitCall)
			{
				Global.config.lastData.SpliterDist = (double)this.MainSplit.SplitterDistance / (double)base.Width;
				Rectangle normalWindowLocation = Native.GetNormalWindowLocation(this);
				Global.config.lastData.WndSize = normalWindowLocation.Size;
				Global.config.lastData.WndPoint = normalWindowLocation.Location;
				Global.config.lastData.WndState = base.WindowState;
				Global.config.SaveXML(Path.Combine(Application.StartupPath, Global.definition.ConfigFile));
				if (Global.state.RunFile != null)
				{
					try
					{
						Process.Start(Global.state.RunFile);
					}
					catch
					{
						MessageBox.Show("アップデートの実行ができませんでした。" + Environment.NewLine + "手動更新してください。", "更新失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
				}
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00023584 File Offset: 0x00021784
		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				Keys keyCode = e.KeyCode;
				if (keyCode != Keys.ShiftKey)
				{
					if (keyCode != Keys.Space)
					{
						return;
					}
					if (e.Shift)
					{
						this.AllTestrun(this, new EventArgs());
					}
					else
					{
						this.ProjTestRun(this, new EventArgs());
					}
					e.Handled = true;
					return;
				}
				else
				{
					if (!Global.config.localSystem.ShiftFocusShift)
					{
						return;
					}
					if (this.EditTab.SelectedIndex == 0)
					{
						if (this.SideTab.SelectedIndex != 0)
						{
							return;
						}
						if (this.MainDesigner.Focused)
						{
							if (this.DrawType.SelectedIndex < 3)
							{
								this.ChipList.Focus();
							}
							else
							{
								this.GuiChipList.Focus();
							}
						}
						else
						{
							this.MainDesigner.Focus();
						}
						e.Handled = true;
					}
				}
			}
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00023650 File Offset: 0x00021850
		public void ProjectConfig_Click(object sender, EventArgs e)
		{
			using (ProjectConfig projectConfig = new ProjectConfig())
			{
				projectConfig.ShowDialog();
			}
			this.UpdateStageSelector();
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0002368C File Offset: 0x0002188C
		private void MWriteStagePicture_Click(object sender, EventArgs e)
		{
			string text = "";
			using (SaveFileDialog saveFileDialog = new SaveFileDialog())
			{
				saveFileDialog.Filter = "PNG画像(*.png)|*.png|GIF画像(*.gif)|*.gif|JPEG画像(*.jpg)|*.jpg|ビットマップ(*.bmp)|*.bmp";
				saveFileDialog.DefaultExt = ".png";
				saveFileDialog.FileName = "無題";
				if (saveFileDialog.ShowDialog() != DialogResult.OK)
				{
					return;
				}
				text = saveFileDialog.FileName;
			}
			Runtime.DefinedData.StageSizeData stageSizeData = Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize : Global.cpd.runtime.Definitions.StageSize;
			using (Bitmap bitmap = new Bitmap(stageSizeData.x * Global.cpd.runtime.Definitions.ChipSize.Width, stageSizeData.y * Global.cpd.runtime.Definitions.ChipSize.Height))
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					using (Brush brush = new SolidBrush(Global.state.Background))
					{
						graphics.FillRectangle(brush, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
					}
					Global.MainWnd.MainDesigner.PaintStage(graphics, MessageBox.Show("拡張描画画像も一緒に書き込みますか？", "拡張描画の選択", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
				}
				string a;
				if ((a = Path.GetExtension(text).ToLower()) != null)
				{
					if (a == ".jpg")
					{
						bitmap.Save(text, ImageFormat.Jpeg);
						goto IL_1DF;
					}
					if (a == ".bmp")
					{
						bitmap.Save(text, ImageFormat.Bmp);
						goto IL_1DF;
					}
					if (a == ".png")
					{
						bitmap.Save(text, ImageFormat.Png);
						goto IL_1DF;
					}
					if (a == ".gif")
					{
						bitmap.Save(text, ImageFormat.Gif);
						goto IL_1DF;
					}
				}
				bitmap.Save(text, ImageFormat.Bmp);
			}
			IL_1DF:
			MessageBox.Show("保存しました。", "ステージ画像の保存", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x000238F0 File Offset: 0x00021AF0
		private void MResetProjRuntime_Click(object sender, EventArgs e)
		{
			using (ResetRuntime resetRuntime = new ResetRuntime())
			{
				if (resetRuntime.ShowDialog() != DialogResult.OK)
				{
					return;
				}
			}
			this.MainDesigner.ClearBuffer();
			this.MainDesigner.AddBuffer();
			this.EditorSystemPanel_Resize(this, new EventArgs());
			if (Global.cpd.UseLayer)
			{
				this.LayerState(true);
				return;
			}
			this.LayerState(false);
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00023968 File Offset: 0x00021B68
		private void InstalledRuntime_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = DialogResult.Retry;
			while (dialogResult == DialogResult.Retry)
			{
				using (RuntimeManager runtimeManager = new RuntimeManager())
				{
					dialogResult = runtimeManager.ShowDialog();
				}
			}
		}

		// Token: 0x06000198 RID: 408 RVA: 0x000239A8 File Offset: 0x00021BA8
		private void MProjectInheritNew_Click(object sender, EventArgs e)
		{
			if (Global.state.EditFlag && MessageBox.Show(string.Concat(new string[]
			{
				"データが保存されていません。",
				Environment.NewLine,
				"保存していないデータは失われます。",
				Environment.NewLine,
				"続行してよろしいですか？"
			}), "保存の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
			{
				return;
			}
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = string.Concat(new string[]
				{
					Global.definition.AppName,
					" プロジェクト (*",
					Global.definition.ProjExt,
					")|*",
					Global.definition.ProjExt
				});
				openFileDialog.InitialDirectory = Global.config.lastData.ProjDirF;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					using (ProjInheritance projInheritance = new ProjInheritance(openFileDialog.FileName))
					{
						if (projInheritance.ShowDialog() == DialogResult.OK)
						{
							this.RestartUp(projInheritance.NewProjectName);
						}
					}
				}
			}
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00023AC8 File Offset: 0x00021CC8
		private void MConvertHTML_Click(object sender, EventArgs e)
		{
			if (Global.state.EditFlag && MessageBox.Show(string.Concat(new string[]
			{
				"データが保存されていません。",
				Environment.NewLine,
				"保存していないデータは失われます。",
				Environment.NewLine,
				"続行してよろしいですか？"
			}), "保存の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
			{
				return;
			}
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "HTML/XML ドキュメント(*.htm*;*.xml)|*.htm*;*.xml|全てのファイル|*.*";
				openFileDialog.InitialDirectory = Global.config.lastData.ProjDirF;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					using (HTMLInheritance htmlinheritance = new HTMLInheritance(openFileDialog.FileName))
					{
						if (htmlinheritance.ShowDialog() == DialogResult.OK)
						{
							this.RestartUp(htmlinheritance.ProjectFile);
						}
					}
				}
			}
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00023BAC File Offset: 0x00021DAC
		private void MUpdateApp_Click(object sender, EventArgs e)
		{
			using (WebUpdate webUpdate = new WebUpdate())
			{
				if (webUpdate.ShowDialog() == DialogResult.Retry)
				{
					Global.state.RunFile = (string)webUpdate.runfile.Clone();
					base.Close();
				}
			}
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00023C04 File Offset: 0x00021E04
		private void MStageRev_Click(object sender, EventArgs e)
		{
			this.UpdateStatus("ステージ反転処理中...");
			List<string> list = new List<string>();
			Runtime.DefinedData.StageSizeData stageSizeData = Global.state.MapEditMode ? Global.cpd.project.Runtime.Definitions.MapSize : Global.cpd.runtime.Definitions.StageSize;
			for (int i = 0; i < stageSizeData.y; i++)
			{
				list = new List<string>();
				string text = Global.cpd.EditingMap[i];
				for (int j = 0; j < stageSizeData.x; j++)
				{
					list.Add(text.Substring(j * stageSizeData.bytesize, stageSizeData.bytesize));
				}
				string[] array = list.ToArray();
				Array.Reverse(array);
				Global.cpd.EditingMap[i] = string.Join("", array);
			}
			if (Global.cpd.UseLayer)
			{
				for (int k = 0; k < Global.cpd.runtime.Definitions.LayerSize.y; k++)
				{
					list = new List<string>();
					string text2 = Global.cpd.EditingLayer[k];
					for (int l = 0; l < Global.cpd.runtime.Definitions.LayerSize.x; l++)
					{
						list.Add(text2.Substring(l * Global.cpd.runtime.Definitions.LayerSize.bytesize, Global.cpd.runtime.Definitions.LayerSize.bytesize));
					}
					string[] array2 = list.ToArray();
					Array.Reverse(array2);
					Global.cpd.EditingLayer[k] = string.Join("", array2);
				}
			}
			this.MainDesigner.ClearBuffer();
			this.MainDesigner.AddBuffer();
			this.MainDesigner.UpdateBackgroundBuffer();
			this.MainDesigner.UpdateForegroundBuffer();
			this.MainDesigner.Refresh();
			Global.state.EditFlag = true;
			this.UpdateStatus("完了");
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000310D File Offset: 0x0000130D
		private void MEditStage1_Click(object sender, EventArgs e)
		{
			this.StageSelectionChange(0);
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00003116 File Offset: 0x00001316
		private void MEditStage2_Click(object sender, EventArgs e)
		{
			this.StageSelectionChange(1);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000311F File Offset: 0x0000131F
		private void MEditStage3_Click(object sender, EventArgs e)
		{
			this.StageSelectionChange(2);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00003128 File Offset: 0x00001328
		private void MEditStage4_Click(object sender, EventArgs e)
		{
			this.StageSelectionChange(3);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00003131 File Offset: 0x00001331
		private void MEditMap_Click(object sender, EventArgs e)
		{
			this.StageSelectionChange(4);
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000313A File Offset: 0x0000133A
		private void AllTestrun(object sender, EventArgs e)
		{
			Global.state.TestrunAll = true;
			this.EditTab.SelectedIndex = 2;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00023E0C File Offset: 0x0002200C
		private void MReloadImage_Click(object sender, EventArgs e)
		{
			this.UpdateStatus("更新しています...");
			Global.MainWnd.MainDesigner.PrepareImages();
			Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
			Global.MainWnd.MainDesigner.UpdateBackgroundBuffer();
			this.MainDesigner.Refresh();
			this.UpdateStatus("完了");
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00023E68 File Offset: 0x00022068
		private void ShowOverView_Click(object sender, EventArgs e)
		{
			if (this.ovw == null)
			{
				this.ovw = new OverViewWindow();
				this.ovw.FormClosed += this.ovw_FormClosed;
				this.ovw.Show(this);
				this.ShowOverView.Checked = true;
				return;
			}
			this.ovw.Close();
			this.ovw = null;
			this.ShowOverView.Checked = false;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00003153 File Offset: 0x00001353
		private void ovw_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.ovw.Dispose();
			this.ovw = null;
			this.ShowOverView.Checked = false;
		}

		// Token: 0x0400023A RID: 570
		public OverViewWindow ovw;

		// Token: 0x0400023B RID: 571
		private bool StageConvert = true;

		// Token: 0x0400023C RID: 572
		private GUIDesigner.EditTool OldET;

		// Token: 0x0400023D RID: 573
		private bool Restartupping;

		// Token: 0x0200002C RID: 44
		// (Invoke) Token: 0x060001A6 RID: 422
		public delegate void MainDesignerScrollInvoke();

		// Token: 0x0200002D RID: 45
		// (Invoke) Token: 0x060001AA RID: 426
		private delegate void CRID();
	}
}
