using System;
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
	public partial class MainWindow : Form
	{
		public event MainWindow.MainDesignerScrollInvoke MainDesignerScroll;

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

		public void Testrun()
		{
			this.EditTab.SelectedIndex = 2;
		}

		private void MainDesigner_ChangeBufferInvoke()
		{
			this.ItemUndo.Enabled = (this.MainDesigner.BufferCurrent != 0);
			this.GMUndo.Enabled = this.ItemUndo.Enabled;
			this.ItemRedo.Enabled = (this.MainDesigner.BufferCurrent != this.MainDesigner.StageBuffer.Count - 1);
			this.GMRedo.Enabled = this.ItemRedo.Enabled;
		}

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

		// ステータスバーっぽいところに表示される小さいアイコンや文字
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
				if (Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 && chipsData.character == "Z")
					size = this.MainDesigner.DrawOribossOrig.Size;
				if (this.ChipNavigator.Image != null)
				{
					this.ChipNavigator.Image.Dispose();
				}
				Bitmap bitmap;
				if (Math.Abs(cschip.rotate) % 180 == 90 && cschip.size.Width > cschip.size.Height)
				{
					bitmap = new Bitmap(size.Height, size.Width);
				}
				else bitmap = new Bitmap(size.Width, size.Height);
				using Graphics graphics = Graphics.FromImage(bitmap);

				using Brush brush = new SolidBrush(Global.state.Background);
				graphics.FillRectangle(brush, new Rectangle(new Point(0, 0), size));

				if (Global.state.EditingForeground)
				{
					graphics.PixelOffsetMode = PixelOffsetMode.Half;
					if (Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 && chipsData.character == "Z")
					{
						graphics.DrawImage(this.MainDesigner.DrawOribossOrig, 0, 0, this.MainDesigner.DrawOribossOrig.Size.Width, this.MainDesigner.DrawOribossOrig.Size.Height);
					}
					else
					{
						Pen pen;
						SolidBrush brush2;
						PointF[] vo_pa;
						double rad = 0;
						switch (cschip.name)
						{
							case "一方通行":
								if (cschip.description.Contains("表示なし")) break;
								pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
								if (cschip.description.Contains("右"))
									graphics.DrawLine(pen, size.Width - 1, 0, size.Width - 1, size.Width);
								else if (cschip.description.Contains("左"))
									graphics.DrawLine(pen, 0, 0, 0, size.Width);
								else if (cschip.description.Contains("上"))
									graphics.DrawLine(pen, 0, 0, size.Width, 0);
								else if (cschip.description.Contains("下"))
									graphics.DrawLine(pen, 0, size.Width - 1, size.Width, size.Width - 1);
								pen.Dispose();
								break;
							case "左右へ押せるドッスンスンのゴール":
								pen = new Pen(Global.cpd.project.Config.Firebar1, 2);
								graphics.TranslateTransform(1, 1);
								graphics.DrawRectangle(pen, 0, 11, size.Width - 2, size.Width - 2 - 11);
								graphics.TranslateTransform(-1, 0);
								graphics.DrawLine(pen, 0, 11, size.Width, size.Width);
								graphics.DrawLine(pen, 0, size.Width, size.Width, 11);
								pen.Dispose();
								break;
							case "シーソー":
								graphics.TranslateTransform(16, 17);
								vo_pa = new PointF[4];
								if (cschip.description.Contains("左")) rad = -56 * Math.PI / 180;
								else if (cschip.description.Contains("右")) rad = 56 * Math.PI / 180;
								vo_pa[0].X = (float)Math.Cos(rad + Math.PI) * size.Width / 2;
								vo_pa[0].Y = (float)Math.Sin(rad + Math.PI) * size.Width / 2;
								vo_pa[1].X = (float)Math.Cos(rad) * size.Width / 2;
								vo_pa[1].Y = (float)Math.Sin(rad) * size.Width / 2;
								vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad - Math.PI / 2) * 5;
								vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad - Math.PI / 2) * 5;
								vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad - Math.PI / 2) * 5;
								vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad - Math.PI / 2) * 5;
								brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
								graphics.FillPolygon(brush2, vo_pa);
								vo_pa = new PointF[3];
								vo_pa[0].X = 0;
								vo_pa[0].Y = -2;
								vo_pa[1].X = -4;
								vo_pa[1].Y = 15;
								vo_pa[2].X = 4;
								vo_pa[2].Y = 15;
								brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
								graphics.FillPolygon(brush2, vo_pa);
								brush2.Dispose();
								break;
							case "ブランコ":
								graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
									new Rectangle(0, 0, size.Width, size.Height),
									new Rectangle(cschip.pattern, size), GraphicsUnit.Pixel);
								graphics.TranslateTransform(16, -9);
								rad = 90 * Math.PI / 180;
								vo_pa = new PointF[4];
								vo_pa[0].X = (float)Math.Cos(rad + Math.PI / 9) * size.Width * (float)1.15;
								vo_pa[0].Y = (float)Math.Sin(rad + Math.PI / 9) * size.Width * (float)1.15;
								vo_pa[1].X = (float)Math.Cos(rad - Math.PI / 9) * size.Width * (float)1.15;
								vo_pa[1].Y = (float)Math.Sin(rad - Math.PI / 9) * size.Width * (float)1.15;
								vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad) * 5;
								vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad) * 5;
								vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad) * 5;
								vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad) * 5;
								double dx = Math.Cos(rad) * 21;
								double dy = Math.Sin(rad) * 21;
								pen = new Pen(Global.cpd.project.Config.Firebar1, 2);
								graphics.DrawLine(pen, (float)Math.Cos(rad) * 10, (float)Math.Sin(rad) * 10, (float)dx, (float)dy);
								graphics.DrawLine(pen, vo_pa[0].X, vo_pa[0].Y, (float)dx, (float)dy);
								graphics.DrawLine(pen, vo_pa[1].X, vo_pa[1].Y, (float)dx, (float)dy);
								brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
								graphics.FillPolygon(brush2, vo_pa);
								pen.Dispose();
								brush2.Dispose();
								break;
							case "スウィングバー":
								graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
									new Rectangle(0, 0, size.Width, size.Height),
									new Rectangle(cschip.pattern, size), GraphicsUnit.Pixel);
								graphics.TranslateTransform(16, 14);
								if (cschip.description.Contains("左"))
								{
									graphics.TranslateTransform(32, 0);
									rad = 180 + Math.Floor((double)(-26 - 5) / 10);
								}
								else if (cschip.description.Contains("右"))
								{
									graphics.TranslateTransform(-32, 0);
									rad = 360 + Math.Floor((double)(26 + 5) / 10);
								}
								rad = (rad * Math.PI) / 180;
								vo_pa = new PointF[4];
								vo_pa[0].X = (float)(Math.Cos(rad) * 40 + Math.Cos(rad + Math.PI / 2) * 3);
								vo_pa[0].Y = (float)(Math.Sin(rad) * 40 + Math.Sin(rad + Math.PI / 2) * 3);
								vo_pa[1].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad + Math.PI / 2) * 3);
								vo_pa[1].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad + Math.PI / 2) * 3);
								vo_pa[2].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad - Math.PI / 2) * 3);
								vo_pa[2].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad - Math.PI / 2) * 3);
								vo_pa[3].X = (float)(Math.Cos(rad) * 40 + Math.Cos(rad - Math.PI / 2) * 3);
								vo_pa[3].Y = (float)(Math.Sin(rad) * 40 + Math.Sin(rad - Math.PI / 2) * 3);
								brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
								graphics.FillPolygon(brush2, vo_pa);
								brush2.Dispose();
								break;
							case "動くＴ字型":
								graphics.TranslateTransform(16, 37);
								rad = 270;
								vo_pa = new PointF[3];
								vo_pa[0].X = (float)Math.Cos(((rad + 6) * Math.PI) / 180) * size.Width;
								vo_pa[0].Y = (float)Math.Sin(((rad + 6) * Math.PI) / 180) * size.Width;
								vo_pa[1].X = (float)Math.Cos(((rad - 6) * Math.PI) / 180) * size.Width;
								vo_pa[1].Y = (float)Math.Sin(((rad - 6) * Math.PI) / 180) * size.Width;
								vo_pa[2].X = 0;
								vo_pa[2].Y = 0;
								brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
								graphics.FillPolygon(brush2, vo_pa);
								vo_pa = new PointF[4];
								vo_pa[0].X = (float)Math.Cos(((rad + 20) * Math.PI) / 180) * size.Width * (float)1.3;
								vo_pa[0].Y = (float)Math.Sin(((rad + 20) * Math.PI) / 180) * size.Width;
								vo_pa[1].X = (float)Math.Cos(((rad - 20) * Math.PI) / 180) * size.Width * (float)1.3;
								vo_pa[1].Y = (float)Math.Sin(((rad - 20) * Math.PI) / 180) * size.Width;
								vo_pa[2].X = vo_pa[1].X + (float)Math.Cos((rad * Math.PI) / 180) * 5;
								vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin((rad * Math.PI) / 180) * 5;
								vo_pa[3].X = vo_pa[0].X + (float)Math.Cos((rad * Math.PI) / 180) * 5;
								vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin((rad * Math.PI) / 180) * 5;
								brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
								graphics.FillPolygon(brush2, vo_pa);
								brush2.Dispose();
								break;
							case "ロープ":
							case "長いロープ":
							case "ゆれる棒":
								graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
									new Rectangle(0, 0, size.Width, size.Height),
									new Rectangle(cschip.pattern, size), GraphicsUnit.Pixel);
								int length;
								if (cschip.name == "ロープ") length = 2;
								else length = 1;
								if (cschip.description == "つかまると左から動く")
								{
									graphics.TranslateTransform(39, 5);
									rad = 168;
								}
								else
								{
									graphics.TranslateTransform(16, -9);
									rad = 90;
								}
								vo_pa = new PointF[4];
								vo_pa[0].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
								vo_pa[0].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
								vo_pa[1].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
								vo_pa[1].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
								vo_pa[2].X = (float)(Math.Cos((rad * Math.PI) / 180) * size.Width * 1.2 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
								vo_pa[2].Y = (float)(Math.Sin((rad * Math.PI) / 180) * size.Width * 1.2 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
								vo_pa[3].X = (float)(Math.Cos((rad * Math.PI) / 180) * size.Width * 1.2 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
								vo_pa[3].Y = (float)(Math.Sin((rad * Math.PI) / 180) * size.Width * 1.2 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
								brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
								graphics.FillPolygon(brush2, vo_pa);
								brush2.Dispose();
								break;
							case "人間大砲":
								if (cschip.description == "右向き") { rad = 330; graphics.TranslateTransform(-9, 3); }
								else if (cschip.description == "左向き") { rad = 225; graphics.TranslateTransform(9, 3); }
								else if (cschip.description == "天井") { rad = 30; graphics.TranslateTransform(-9, 0); }
								else if (cschip.description == "右の壁") { rad = 270; graphics.TranslateTransform(0, 9); }
								else if (cschip.description == "左の壁") { rad = 300; graphics.TranslateTransform(0, 9); }
								brush2 = new SolidBrush(Global.cpd.project.Config.Mizunohadou);
								graphics.FillEllipse(brush2, 16 - 7, 16 - 7, 14, 14);
								vo_pa = new PointF[4];
								vo_pa[0].X = 16 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 7;
								vo_pa[0].Y = 16 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 7;
								vo_pa[1].X = 16 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 7;
								vo_pa[1].Y = 16 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 7;
								vo_pa[2].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 20 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 7;
								vo_pa[2].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 20 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 7;
								vo_pa[3].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 20 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 7;
								vo_pa[3].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 20 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 7;
								graphics.FillPolygon(brush2, vo_pa);
								brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
								if (cschip.description == "天井")
								{
									vo_pa[0].X = 16 - 2;
									vo_pa[0].Y = 16 + 1;
									vo_pa[1].X = 16 + 2;
									vo_pa[1].Y = 16 + 1;
									vo_pa[2].X = 16 + 5;
									vo_pa[2].Y = 0;
									vo_pa[3].X = 16 - 5;
									vo_pa[3].Y = 0;
								}
								else if (cschip.description == "右の壁")
								{
									vo_pa[0].X = 16 - 1;
									vo_pa[0].Y = 16 - 2;
									vo_pa[1].X = 16 - 1;
									vo_pa[1].Y = 16 + 2;
									vo_pa[2].X = size.Width;
									vo_pa[2].Y = 16 + 5;
									vo_pa[3].X = size.Width;
									vo_pa[3].Y = 16 - 5;
								}
								else if (cschip.description == "左の壁")
								{
									vo_pa[0].X = 16 + 1;
									vo_pa[0].Y = 16 - 2;
									vo_pa[1].X = 16 + 1;
									vo_pa[1].Y = 16 + 2;
									vo_pa[2].X = 0;
									vo_pa[2].Y = 16 + 5;
									vo_pa[3].X = 0;
									vo_pa[3].Y = 16 - 5;
								}
								else
								{
									vo_pa[0].X = 16 - 2;
									vo_pa[0].Y = 16 - 1;
									vo_pa[1].X = 16 + 2;
									vo_pa[1].Y = 16 - 1;
									vo_pa[2].X = 16 + 5;
									vo_pa[2].Y = size.Width - 3;
									vo_pa[3].X = 16 - 5;
									vo_pa[3].Y = size.Width - 3;
								}
								graphics.FillPolygon(brush2, vo_pa);
								brush2.Dispose();
								break;
							case "曲線による上り坂":
							case "曲線による下り坂":
								var k21 = 0; float j20 = default, k20 = default, l20 = default, i21 = default;
								if (cschip.description.Contains("線のみ"))
								{
									vo_pa = new PointF[11];
									pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
									for (var i1 = 0; i1 <= 50; i1 += 5)
									{
										if (cschip.name.Contains("上"))
											vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * 3.1415926535897931) / 180) * size.Width * 5 / 8);
										else if (cschip.name.Contains("下"))
											vo_pa[k21].X = (float)Math.Floor(size.Width - Math.Sin((i1 * 3.1415926535897931) / 180) * size.Width * 5 / 8);
										vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * 3.1415926535897931) / 180) * size.Width * 5 / 8) - 1;
										if (i1 == 50)
										{
											j20 = vo_pa[k21].X;
											k20 = vo_pa[k21].Y;
										}
										k21++;
									}

									graphics.DrawLines(pen, vo_pa);
									k21 = 0;
									for (var i1 = 0; i1 <= 50; i1 += 5)
									{
										if (cschip.name.Contains("上"))
											vo_pa[k21].X = (float)Math.Floor(size.Width - Math.Sin((i1 * 3.1415926535897931) / 180) * size.Width * 5 / 8);
										else if (cschip.name.Contains("下"))
											vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * 3.1415926535897931) / 180) * size.Width * 5 / 8);
										vo_pa[k21].Y = (float)Math.Floor(size.Width * 5 / 8 - Math.Cos((i1 * 3.1415926535897931) / 180) * size.Width * 5 / 8) + 1;
										if (i1 == 50)
										{
											l20 = vo_pa[k21].X;
											i21 = vo_pa[k21].Y;
										}
										k21++;
									}
									graphics.DrawLines(pen, vo_pa);
									vo_pa = new PointF[2];
									vo_pa[0].X = j20;
									vo_pa[0].Y = k20;
									vo_pa[1].X = l20;
									vo_pa[1].Y = i21;
									graphics.DrawLines(pen, vo_pa);
									pen.Dispose();
								}
								else
								{
									vo_pa = new PointF[13];
									brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
									for (var i1 = 0; i1 <= 50; i1 += 5)
									{
										if (cschip.name.Contains("上"))
											vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * 3.1415926535897931) / 180) * size.Width * 5 / 8);
										else if (cschip.name.Contains("下"))
											vo_pa[k21].X = (float)Math.Floor(size.Width - Math.Sin((i1 * 3.1415926535897931) / 180) * size.Width * 5 / 8);
										vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * 3.1415926535897931) / 180) * size.Width * 5 / 8);
										if (i1 == 50)
										{
											j20 = vo_pa[k21].X;
											k20 = vo_pa[k21].Y;
										}
										k21++;
									}

									vo_pa[k21].X = j20;
									vo_pa[k21].Y = size.Width;
									k21++;
									if (cschip.name.Contains("上")) vo_pa[k21].X = 0;
									else if (cschip.name.Contains("下")) vo_pa[k21].X = size.Width;
									vo_pa[k21].Y = size.Width;
									graphics.FillPolygon(brush2, vo_pa);
									vo_pa = new PointF[13];
									k21 = 0;
									for (var i1 = 0; i1 <= 50; i1 += 5)
									{
										if (cschip.name.Contains("上"))
											vo_pa[k21].X = (float)Math.Floor(size.Width - Math.Sin((i1 * 3.1415926535897931) / 180) * size.Width * 5 / 8);
										else if (cschip.name.Contains("下"))
											vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * 3.1415926535897931) / 180) * size.Width * 5 / 8);
										vo_pa[k21].Y = (float)Math.Floor(size.Width * 5 / 8 - Math.Cos((i1 * 3.1415926535897931) / 180) * size.Width * 5 / 8);
										if (i1 == 50)
										{
											l20 = vo_pa[k21].X;
											i21 = vo_pa[k21].Y;
										}
										k21++;
									}

									vo_pa[k21].X = l20;
									vo_pa[k21].Y = size.Width;
									k21++;
									if (cschip.name.Contains("上")) vo_pa[k21].X = size.Width;
									else if (cschip.name.Contains("下")) vo_pa[k21].X = 0;
									vo_pa[k21].Y = size.Width;
									graphics.FillPolygon(brush2, vo_pa);
									vo_pa = new PointF[4];
									vo_pa[0].X = j20;
									vo_pa[0].Y = k20;
									vo_pa[1].X = l20;
									vo_pa[1].Y = i21;
									vo_pa[2].X = l20;
									vo_pa[2].Y = size.Width;
									vo_pa[3].X = j20;
									vo_pa[3].Y = size.Width;
									graphics.FillPolygon(brush2, vo_pa);
									brush2.Dispose();
								}
								break;
							case "乗れる円":
							case "跳ねる円":
							case "円":
								int radius = default;
								graphics.TranslateTransform(size.Width / 2, size.Width / 2);
								if (cschip.name == "円")
								{
									brush2 = new SolidBrush(Color.FromArgb(176, Global.cpd.project.Config.Mizunohadou));
									if (cschip.description.Contains("乗ると下がる"))
									{
										radius = 80 / 10;
									}
									else
									{
										radius = 112 / 10;
									}
								}
								else
								{
									brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
									if (cschip.description.Contains("大"))
									{
										if (cschip.name == "乗れる円")
										{
											radius = 144 / 10;
										}
										else if (cschip.name == "跳ねる円")
										{
											radius = 128 / 10;
										}
									}
									else
									{
										radius = 96 / 10;
									}
								}
								graphics.FillEllipse(brush2, -radius, -radius, radius * 2, radius * 2);
								brush2.Dispose();
								break;
							case "半円":
								graphics.TranslateTransform(1, 2);
								if (cschip.description.Contains("乗れる"))
								{
									if (cschip.description.Contains("線のみ"))
									{
										pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
										vo_pa = new PointF[12];
										var j21 = 0;
										vo_pa[j21].X = 1 / 8;
										vo_pa[j21].Y = 63 / 8;
										j21++;
										for (var j = 140; j >= 90; j -= 5)
										{
											vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * 3.1415926535897931) / 180) * 144) / 8;
											vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((j * 3.1415926535897931) / 180) * 144) / 8;
											j21++;
										}
										graphics.DrawLines(pen, vo_pa);
										j21 = 0;
										for (var k2 = 90; k2 >= 40; k2 -= 5)
										{
											vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * 3.1415926535897931) / 180) * 144) / 8;
											vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((k2 * 3.1415926535897931) / 180) * 144) / 8;
											j21++;
										}
										vo_pa[j21].X = 240 / 8;
										vo_pa[j21].Y = 63 / 8;
										graphics.DrawLines(pen, vo_pa);
										pen.Dispose();
									}
									else
									{
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										vo_pa = new PointF[13];
										var j21 = 0;
										vo_pa[j21].X = 0;
										vo_pa[j21].Y = 64 / 8;
										j21++;
										for (var j = 140; j >= 90; j -= 5)
										{
											vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * 3.1415926535897931) / 180) * 144) / 8;
											vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * 3.1415926535897931) / 180) * 144) / 8;
											j21++;
										}

										vo_pa[j21].X = 120 / 8;
										vo_pa[j21].Y = 64 / 8;
										graphics.FillPolygon(brush2, vo_pa);
										j21 = 0;
										for (var k2 = 90; k2 >= 40; k2 -= 5)
										{
											vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * 3.1415926535897931) / 180) * 144) / 8;
											vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * 3.1415926535897931) / 180) * 144) / 8;
											j21++;
										}

										vo_pa[j21].X = 240 / 8;
										vo_pa[j21].Y = 64 / 8;
										j21++;
										vo_pa[j21].X = 120 / 8;
										vo_pa[j21].Y = 64 / 8;
										graphics.FillPolygon(brush2, vo_pa);
										brush2.Dispose();
									}
								}
								else
								{
									brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
									graphics.FillRectangle(brush2, (120 - 20) / 8, 64 / 8 - 1, 40 / 8, 22);
									brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
									vo_pa = new PointF[13];
									var j21 = 0;
									vo_pa[j21].X = 0;
									vo_pa[j21].Y = 64 / 8;
									j21++;
									for (var j = 140; j >= 90; j -= 5)
									{
										vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * 3.1415926535897931) / 180) * 144) / 8;
										vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * 3.1415926535897931) / 180) * 144) / 8;
										j21++;
									}

									vo_pa[j21].X = 120 / 8;
									vo_pa[j21].Y = 64 / 8;
									graphics.FillPolygon(brush2, vo_pa);
									j21 = 0;
									for (var k2 = 90; k2 >= 40; k2 -= 5)
									{
										vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * 3.1415926535897931) / 180) * 144) / 8;
										vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * 3.1415926535897931) / 180) * 144) / 8;
										j21++;
									}

									vo_pa[j21].X = 240 / 8;
									vo_pa[j21].Y = 64 / 8;
									j21++;
									vo_pa[j21].X = 120 / 8;
									vo_pa[j21].Y = 64 / 8;
									graphics.FillPolygon(brush2, vo_pa);
									brush2.Dispose();
								}
								break;
							case "ファイヤーバー":
							case "スウィングファイヤーバー":
								{
									graphics.TranslateTransform(size.Width / 2, size.Width / 2);
									graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
										new Rectangle(0, 0, size.Width / 2, size.Width / 2),
										new Rectangle(cschip.pattern, new Size(size.Width / 2, size.Width / 2)), GraphicsUnit.Pixel);
									int v = 225;
									graphics.TranslateTransform(size.Width / 2, size.Width / 2);
									rad = ((v + 90) * Math.PI) / 180;
									const double d = 0.017453292519943295;
									vo_pa = new PointF[4];
									vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 25) + Math.Cos(rad) * 4);
									vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) + Math.Sin(rad) * 4);
									vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 25) - Math.Cos(rad) * 4);
									vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) - Math.Sin(rad) * 4);
									vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 42) - Math.Cos(rad) * 4);
									vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 42) - Math.Sin(rad) * 4);
									vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 42) + Math.Cos(rad) * 4);
									vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 42) + Math.Sin(rad) * 4);
									brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
									graphics.FillPolygon(brush2, vo_pa);
									// 内側の色を描画
									brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
									vo_pa[0].X = (float)(Math.Cos(v * d) * 28 + Math.Cos(rad) * 2);
									vo_pa[0].Y = (float)(Math.Sin(v * d) * 28 + Math.Sin(rad) * 2);
									vo_pa[1].X = (float)(Math.Cos(v * d) * 28 - Math.Cos(rad) * 2);
									vo_pa[1].Y = (float)(Math.Sin(v * d) * 28 - Math.Sin(rad) * 2);
									vo_pa[2].X = (float)(Math.Cos(v * d) * 40 - Math.Cos(rad) * 2);
									vo_pa[2].Y = (float)(Math.Sin(v * d) * 40 - Math.Sin(rad) * 2);
									vo_pa[3].X = (float)(Math.Cos(v * d) * 40 + Math.Cos(rad) * 2);
									vo_pa[3].Y = (float)(Math.Sin(v * d) * 40 + Math.Sin(rad) * 2);
									graphics.FillPolygon(brush2, vo_pa);
									brush2.Dispose();
								}
								break;
							case "人口太陽":
								{
									graphics.TranslateTransform(size.Width / 2, size.Width / 2);

									int v = default, n = default;
									const double d = 0.017453292519943295;
									vo_pa = new PointF[4];
									if (cschip.description.Contains("棒５本")) n = 5;
									else n = 3;
									if (cschip.description.Contains("左")) v = 360 - 2;
									else v = 2;

									for (var ii = 0; ii < n; ii++)
									{
										v += 360 / n;
										rad = ((v + 90) * Math.PI) / 180;
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
										vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 4) + Math.Cos(rad) * 3);
										vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 4) + Math.Sin(rad) * 3);
										vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 4) - Math.Cos(rad) * 3);
										vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 4) - Math.Sin(rad) * 3);
										vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 15) - Math.Cos(rad) * 3);
										vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) - Math.Sin(rad) * 3);
										vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 15) + Math.Cos(rad) * 3);
										vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) + Math.Sin(rad) * 3);
										graphics.FillPolygon(brush2, vo_pa);

										// 内側の色を描画
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										vo_pa[0].X = (float)(Math.Cos(v * d) * 6 + Math.Cos(rad) * 1);
										vo_pa[0].Y = (float)(Math.Sin(v * d) * 6 + Math.Sin(rad) * 1);
										vo_pa[1].X = (float)(Math.Cos(v * d) * 6 - Math.Cos(rad) * 1);
										vo_pa[1].Y = (float)(Math.Sin(v * d) * 6 - Math.Sin(rad) * 1);
										vo_pa[2].X = (float)(Math.Cos(v * d) * 13 - Math.Cos(rad) * 1);
										vo_pa[2].Y = (float)(Math.Sin(v * d) * 13 - Math.Sin(rad) * 1);
										vo_pa[3].X = (float)(Math.Cos(v * d) * 13 + Math.Cos(rad) * 1);
										vo_pa[3].Y = (float)(Math.Sin(v * d) * 13 + Math.Sin(rad) * 1);
										graphics.FillPolygon(brush2, vo_pa);
									}
									brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
									graphics.FillEllipse(brush2, -6, -6, 12, 12);
									brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
									graphics.FillEllipse(brush2, -2, -2, 4, 4);

									brush2.Dispose();
								}
								break;
							case "ファイヤーリング":
								{
									graphics.TranslateTransform(size.Width / 2, size.Width / 2);

									int v = default, n = default;
									if (cschip.description.Contains("2本")) n = 2;
									else n = 3;
									if (cschip.description.Contains("左回り"))
									{
										if (cschip.description.Contains("高速")) v = -4 + 360;
										else v = -2 + 360;
									}
									else
									{
										if (cschip.description.Contains("高速")) v = 4;
										else v = 2;
									}

									brush2 = default;

									for (var ii = 0; ii < n; ii++)
									{
										var k6 = 0;
										if (cschip.description.Contains("2本"))
										{
											vo_pa = new PointF[26];
											for (var i4 = 0; i4 >= -120; i4 -= 10)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * size.Width / 2);
												vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * size.Width / 2);
												k6++;
											}

											for (var j4 = -120; j4 <= 0; j4 += 10)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * size.Width / 2 * 0.3);
												vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * size.Width / 2 * 0.3);
												k6++;
											}
										}
										else
										{
											vo_pa = new PointF[12];
											for (var i4 = 0; i4 >= -50; i4 -= 10)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * size.Width / 2);
												vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * size.Width / 2);
												k6++;
											}

											for (var j4 = -50; j4 <= 0; j4 += 10)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * size.Width / 2 * 0.3);
												vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * size.Width / 2 * 0.3);
												k6++;
											}
										}

										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
										graphics.FillPolygon(brush2, vo_pa);

										// 内側の色を描画
										k6 = 0;
										if (cschip.description.Contains("2本"))
										{
											vo_pa = new PointF[24];
											for (var k4 = -5; k4 >= -115; k4 -= 10)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * size.Width / 2 * 0.925);
												vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * size.Width / 2 * 0.925);
												k6++;
											}

											for (var l4 = -115; l4 <= -5; l4 += 10)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * size.Width / 2 * 0.5);
												vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * size.Width / 2 * 0.5);
												k6++;
											}
										}
										else
										{
											for (var k4 = -5; k4 >= -45; k4 -= 8)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * size.Width / 2 * 0.925);
												vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * size.Width / 2 * 0.925);
												k6++;
											}

											for (var l4 = -45; l4 <= -5; l4 += 8)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * size.Width / 2 * 0.5);
												vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * size.Width / 2 * 0.5);
												k6++;
											}
										}

										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										graphics.FillPolygon(brush2, vo_pa);

										v += 360 / n;
									}

									brush2.Dispose();
								}
								break;
							default:
								int rotate_o = default;
								if (Math.Abs(cschip.rotate) % 180 == 90 && cschip.size.Width > cschip.size.Height)
								{
									rotate_o = (cschip.size.Width - cschip.size.Height) / (cschip.size.Width / cschip.size.Height) * Math.Sign(cschip.rotate);
								}
								graphics.TranslateTransform(size.Width / 2, size.Height / 2);
								graphics.RotateTransform(cschip.rotate);

								// 水の半透明処理
								if (Global.state.ChipRegister.ContainsKey("water_clear_switch") && bool.Parse(Global.state.ChipRegister["water_clear_switch"]) == false && chipsData.character == "4" && Global.state.ChipRegister.ContainsKey("water_clear_level"))
								{
									float water_clear_level = float.Parse(Global.state.ChipRegister["water_clear_level"]);
                                    var colorMatrix = new ColorMatrix
                                    {
                                        Matrix00 = 1f,
                                        Matrix11 = 1f,
                                        Matrix22 = 1f,
                                        Matrix33 = water_clear_level / 255f,
                                        Matrix44 = 1f
                                    };
                                    using var imageAttributes = new ImageAttributes();
                                    imageAttributes.SetColorMatrix(colorMatrix);
                                    graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(new Point(-size.Width / 2 + rotate_o, -size.Height / 2 + rotate_o), size), cschip.pattern.X, cschip.pattern.Y, size.Width, size.Height, GraphicsUnit.Pixel, imageAttributes);
                                }
								else graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(new Point(-size.Width / 2 + rotate_o, -size.Height / 2 + rotate_o), size), new Rectangle(cschip.pattern, size), GraphicsUnit.Pixel);
								break;
						}
					}
				}
				else
				{
					graphics.DrawImage(this.MainDesigner.DrawLayerOrig, new Rectangle(new Point(0, 0), size), new Rectangle(cschip.pattern, size), GraphicsUnit.Pixel);
				}
				this.ChipNavigator.Image = (Image)bitmap.Clone();
				bitmap.Dispose();
				string name, description;

				if (Global.state.EditingForeground && Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 && chipsData.character == "Z")
				{
					name = "オリジナルボス";
                    if (Global.state.ChipRegister.ContainsKey("oriboss_ugoki"))
                    {
                        switch (int.Parse(Global.state.ChipRegister["oriboss_ugoki"]))
                        {
							case 1:
								description = "停止";
								break;
							case 2:
								description = "左右移動";
								break;
							case 3:
								description = "上下移動";
								break;
							case 4:
								description = "左回り";
								break;
							case 5:
								description = "右回り";
								break;
							case 6:
								description = "四角形左回り";
								break;
							case 7:
								description = "四角形右回り";
								break;
							case 8:
								description = "HPが半分になると左へ移動";
								break;
							case 9:
								description = "HPが減ると左と右へ移動";
								break;
							case 10:
								description = "HPが半分になると上へ移動";
								break;
							case 11:
								description = "HPが減ると上と下へ移動";
								break;
							case 12:
								description = "HPが半分になると下へ移動";
								break;
							case 13:
								description = "HPが減ると下と上へ移動";
								break;
							case 14:
								description = "画面の端で方向転換";
								break;
							case 15:
								description = "ジグザグ移動";
								break;
							case 16:
								description = "画面の内側を左回り";
								break;
							case 17:
								description = "画面の内側を右回り";
								break;
							case 18:
								description = "HPが半分以下になると左右移動";
								break;
							case 19:
								description = "HPが1/3以下になると左右移動";
								break;
							case 20:
								description = "HPが半分以下になると左回り";
								break;
							case 21:
								description = "HPが1/3以下になると左回り";
								break;
							case 22:
								description = "斜め上へ往復";
								break;
							case 23:
								description = "斜め下へ往復";
								break;
							case 24:
								description = "中央で停止";
								break;
							case 25:
								description = "中央で停止 主人公の方を向く";
								break;
							case 26:
								description = "巨大化  中央から";
								break;
							case 27:
								description = "巨大化  右から";
								break;
							default:
								description = "";
								break;
						}
                    }
					else description = "";

				}
				else {
					name = cschip.name;
					description = cschip.description;
				}

				this.ChipNavigator.Text = string.Concat(new string[]
				{
					"[",
					chipsData.character,
					"]",
					name,
					"/",
					description
				});
				this.ChipNavigator.Visible = true;
			}
		}

		public void CommitScrollbar()
		{
			this.GHorzScroll.Value = Global.state.MapPoint.X;
			this.GVirtScroll.Value = Global.state.MapPoint.Y;
			if (this.MainDesignerScroll != null)
			{
				this.MainDesignerScroll();
			}
		}

		public void UpdateScrollbar()
		{
			this.UpdateScrollbar(Global.config.draw.ZoomIndex);
		}

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

            if (e.TabPageIndex == 2) this.IntegrateBrowser.Navigate("about:blank");
        }

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

		public void ChipItemReady()
		{
			MainWindow.CRID method = new MainWindow.CRID(this.ChipItemReadyInvoke);
			base.Invoke(method);
		}

		// クラシックチップリスト以外
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
				else if(Global.state.EditingForeground && chipsData.character == "Z" && Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3)
				{
					string description;
					if (Global.state.ChipRegister.ContainsKey("oriboss_ugoki"))
					{
						switch (int.Parse(Global.state.ChipRegister["oriboss_ugoki"]))
						{
							case 1:
								description = "停止";
								break;
							case 2:
								description = "左右移動";
								break;
							case 3:
								description = "上下移動";
								break;
							case 4:
								description = "左回り";
								break;
							case 5:
								description = "右回り";
								break;
							case 6:
								description = "四角形左回り";
								break;
							case 7:
								description = "四角形右回り";
								break;
							case 8:
								description = "HPが半分になると左へ移動";
								break;
							case 9:
								description = "HPが減ると左と右へ移動";
								break;
							case 10:
								description = "HPが半分になると上へ移動";
								break;
							case 11:
								description = "HPが減ると上と下へ移動";
								break;
							case 12:
								description = "HPが半分になると下へ移動";
								break;
							case 13:
								description = "HPが減ると下と上へ移動";
								break;
							case 14:
								description = "画面の端で方向転換";
								break;
							case 15:
								description = "ジグザグ移動";
								break;
							case 16:
								description = "画面の内側を左回り";
								break;
							case 17:
								description = "画面の内側を右回り";
								break;
							case 18:
								description = "HPが半分以下になると左右移動";
								break;
							case 19:
								description = "HPが1/3以下になると左右移動";
								break;
							case 20:
								description = "HPが半分以下になると左回り";
								break;
							case 21:
								description = "HPが1/3以下になると左回り";
								break;
							case 22:
								description = "斜め上へ往復";
								break;
							case 23:
								description = "斜め下へ往復";
								break;
							case 24:
								description = "中央で停止";
								break;
							case 25:
								description = "中央で停止 主人公の方を向く";
								break;
							case 26:
								description = "巨大化  中央から";
								break;
							case 27:
								description = "巨大化  右から";
								break;
							default:
								description = "";
								break;
						}
					}
					else description = "";

					this.ChipList.Items.Add("オリジナルボス" + "/" + description);

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

		private void GVirtScroll_Scroll(object sender, ScrollEventArgs e)
		{
			Global.state.MapPoint.Y = this.GVirtScroll.Value;
			this.MainDesigner.Refresh();
			if (this.MainDesignerScroll != null)
			{
				this.MainDesignerScroll();
			}
		}

		private void GHorzScroll_Scroll(object sender, ScrollEventArgs e)
		{
			Global.state.MapPoint.X = this.GHorzScroll.Value;
			this.MainDesigner.Refresh();
			if (this.MainDesignerScroll != null)
			{
				this.MainDesignerScroll();
			}
		}

		private void ShowGrid_Click(object sender, EventArgs e)
		{
			this.GMShowGrid.Checked = this.ShowGrid.Checked;
			Global.config.draw.DrawGrid = this.ShowGrid.Checked;
			this.MainDesigner.Refresh();
		}

		private void GMShowGrid_Click(object sender, EventArgs e)
		{
			this.ShowGrid.Checked = this.GMShowGrid.Checked;
			Global.config.draw.DrawGrid = this.GMShowGrid.Checked;
			this.MainDesigner.Refresh();
		}

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

		private void StageZoom25_Click(object sender, EventArgs e)
		{
			this.ZoomInit();
			this.StageZoom25.Checked = true;
			this.GMZoom25.Checked = true;
			double zoomIndex = Global.config.draw.ZoomIndex;
			Global.config.draw.ZoomIndex = 0.25;
			this.UpdateScrollbar(zoomIndex);
		}

		private void StageZoom50_Click(object sender, EventArgs e)
		{
			this.ZoomInit();
			this.StageZoom50.Checked = true;
			this.GMZoom50.Checked = true;
			double zoomIndex = Global.config.draw.ZoomIndex;
			Global.config.draw.ZoomIndex = 0.5;
			this.UpdateScrollbar(zoomIndex);
		}

		private void StageZoom100_Click(object sender, EventArgs e)
		{
			this.ZoomInit();
			this.StageZoom100.Checked = true;
			this.GMZoom100.Checked = true;
			double zoomIndex = Global.config.draw.ZoomIndex;
			Global.config.draw.ZoomIndex = 1.0;
			this.UpdateScrollbar(zoomIndex);
		}

		private void StageZoom200_Click(object sender, EventArgs e)
		{
			this.ZoomInit();
			this.StageZoom200.Checked = true;
			this.GMZoom200.Checked = true;
			double zoomIndex = Global.config.draw.ZoomIndex;
			Global.config.draw.ZoomIndex = 2.0;
			this.UpdateScrollbar(zoomIndex);
		}

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

		// チップリストの左上
		private void state_UpdateCurrentChipInvoke()
		{
			this.ChipImage.Refresh();
			if (Global.state.CurrentChip.character == "Z" && Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3)
				this.ChipDescription.Text = "オリジナルボス";
			else this.ChipDescription.Text = Global.state.CurrentChip.GetCSChip().name;
			if (Global.state.CurrentChip.GetCSChip().description != "")
			{
				if (Global.state.EditingForeground && Global.state.CurrentChip.character == "Z" && Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3)
				{
					string description;
					if (Global.state.ChipRegister.ContainsKey("oriboss_ugoki"))
					{
						switch (int.Parse(Global.state.ChipRegister["oriboss_ugoki"]))
						{
							case 1:
								description = "停止";
								break;
							case 2:
								description = "左右移動";
								break;
							case 3:
								description = "上下移動";
								break;
							case 4:
								description = "左回り";
								break;
							case 5:
								description = "右回り";
								break;
							case 6:
								description = "四角形左回り";
								break;
							case 7:
								description = "四角形右回り";
								break;
							case 8:
								description = "HPが半分になると左へ移動";
								break;
							case 9:
								description = "HPが減ると左と右へ移動";
								break;
							case 10:
								description = "HPが半分になると上へ移動";
								break;
							case 11:
								description = "HPが減ると上と下へ移動";
								break;
							case 12:
								description = "HPが半分になると下へ移動";
								break;
							case 13:
								description = "HPが減ると下と上へ移動";
								break;
							case 14:
								description = "画面の端で方向転換";
								break;
							case 15:
								description = "ジグザグ移動";
								break;
							case 16:
								description = "画面の内側を左回り";
								break;
							case 17:
								description = "画面の内側を右回り";
								break;
							case 18:
								description = "HPが半分以下になると左右移動";
								break;
							case 19:
								description = "HPが1/3以下になると左右移動";
								break;
							case 20:
								description = "HPが半分以下になると左回り";
								break;
							case 21:
								description = "HPが1/3以下になると左回り";
								break;
							case 22:
								description = "斜め上へ往復";
								break;
							case 23:
								description = "斜め下へ往復";
								break;
							case 24:
								description = "中央で停止";
								break;
							case 25:
								description = "中央で停止 主人公の方を向く";
								break;
							case 26:
								description = "巨大化  中央から";
								break;
							case 27:
								description = "巨大化  右から";
								break;
							default:
								description = "";
								break;
						}
					}
					else description = "";

					this.ChipChar.Text = "[Z]" + description;

				}
				else this.ChipChar.Text = "[" + Global.state.CurrentChip.character + "]" + Global.state.CurrentChip.GetCSChip().description;
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

		// チップリストの左上
		private void ChipImage_Paint(object sender, PaintEventArgs e)
		{
			if (this.MainDesigner.DrawChipOrig != null)
			{
				e.Graphics.InterpolationMode = InterpolationMode.High;
				ChipData cschip = Global.state.CurrentChip.GetCSChip();
				if (!Global.cpd.UseLayer || Global.state.EditingForeground)
				{
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					if (Global.state.EditingForeground && Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 && Global.state.CurrentChip.character == "Z")
					{
						e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawOribossOrig, 0, 0, this.ChipImage.Size.Width, this.ChipImage.Size.Height);
					}
					else
					{
						Pen pen;
						SolidBrush brush;
						PointF[] vo_pa;
						double rad = 0;
						const double math_pi = 3.1415926535897931;
						switch (cschip.name)
						{
							case "一方通行":
								if (cschip.description.Contains("表示なし")) break;
								pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
								if (cschip.description.Contains("右"))
									e.Graphics.DrawLine(pen, this.ChipImage.Width - 1, 0, this.ChipImage.Width - 1, this.ChipImage.Height);
								else if (cschip.description.Contains("左"))
									e.Graphics.DrawLine(pen, 1, 0, 1, this.ChipImage.Height);
								else if (cschip.description.Contains("上"))
									e.Graphics.DrawLine(pen, 0, 1, this.ChipImage.Width, 1);
								else if (cschip.description.Contains("下"))
									e.Graphics.DrawLine(pen, 0, this.ChipImage.Height - 1, this.ChipImage.Width, this.ChipImage.Height - 1);
								pen.Dispose();
								break;
							case "左右へ押せるドッスンスンのゴール":
								pen = new Pen(Global.cpd.project.Config.Firebar1, 1);
								e.Graphics.TranslateTransform(1, 1);
								e.Graphics.DrawRectangle(pen, 0, 11, this.ChipImage.Width - 1, this.ChipImage.Height - 1 - 11);
								e.Graphics.TranslateTransform(-1, -1);
								e.Graphics.DrawLine(pen, 0, 11, this.ChipImage.Width, this.ChipImage.Height);
								e.Graphics.DrawLine(pen, 0, this.ChipImage.Height, this.ChipImage.Width, 11);
								pen.Dispose();
								break;
							case "シーソー":
								e.Graphics.TranslateTransform(16, 17);
								vo_pa = new PointF[4];
								if (cschip.description.Contains("左")) rad = -56 * Math.PI / 180;
								else if (cschip.description.Contains("右")) rad = 56 * Math.PI / 180;
								vo_pa[0].X = (float)Math.Cos(rad + Math.PI) * this.ChipImage.Width / 2;
								vo_pa[0].Y = (float)Math.Sin(rad + Math.PI) * this.ChipImage.Width / 2;
								vo_pa[1].X = (float)Math.Cos(rad) * this.ChipImage.Width / 2;
								vo_pa[1].Y = (float)Math.Sin(rad) * this.ChipImage.Width / 2;
								vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad - Math.PI / 2) * 5;
								vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad - Math.PI / 2) * 5;
								vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad - Math.PI / 2) * 5;
								vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad - Math.PI / 2) * 5;
								brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
								e.Graphics.FillPolygon(brush, vo_pa);
								vo_pa = new PointF[3];
								vo_pa[0].X = 0;
								vo_pa[0].Y = -2;
								vo_pa[1].X = -4;
								vo_pa[1].Y = 15;
								vo_pa[2].X = 4;
								vo_pa[2].Y = 15;
								brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
								e.Graphics.FillPolygon(brush, vo_pa);
								brush.Dispose();
								break;
							case "ブランコ":
								e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
									new Rectangle(0, 0, this.ChipImage.Width, this.ChipImage.Height),
									new Rectangle(cschip.pattern, this.ChipImage.Size), GraphicsUnit.Pixel);
								e.Graphics.TranslateTransform(16, -9);
								rad = 90 * Math.PI / 180;
								vo_pa = new PointF[4];
								vo_pa[0].X = (float)Math.Cos(rad + Math.PI / 9) * this.ChipImage.Width * (float)1.15;
								vo_pa[0].Y = (float)Math.Sin(rad + Math.PI / 9) * this.ChipImage.Width * (float)1.15;
								vo_pa[1].X = (float)Math.Cos(rad - Math.PI / 9) * this.ChipImage.Width * (float)1.15;
								vo_pa[1].Y = (float)Math.Sin(rad - Math.PI / 9) * this.ChipImage.Width * (float)1.15;
								vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad) * 5;
								vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad) * 5;
								vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad) * 5;
								vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad) * 5;
								double dx = Math.Cos(rad) * 21;
								double dy = Math.Sin(rad) * 21;
								pen = new Pen(Global.cpd.project.Config.Firebar1, 2);
								e.Graphics.DrawLine(pen, (float)Math.Cos(rad) * 10, (float)Math.Sin(rad) * 10, (float)dx, (float)dy);
								e.Graphics.DrawLine(pen, vo_pa[0].X, vo_pa[0].Y, (float)dx, (float)dy);
								e.Graphics.DrawLine(pen, vo_pa[1].X, vo_pa[1].Y, (float)dx, (float)dy);
								brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
								e.Graphics.FillPolygon(brush, vo_pa);
								pen.Dispose();
								brush.Dispose();
								break;
							case "スウィングバー":
								e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
									new Rectangle(0, 0, this.ChipImage.Width, this.ChipImage.Height),
									new Rectangle(cschip.pattern, this.ChipImage.Size), GraphicsUnit.Pixel);
								e.Graphics.TranslateTransform(16, 14);
								if (cschip.description.Contains("左"))
								{
									e.Graphics.TranslateTransform(32, 0);
									rad = 180 + Math.Floor((double)(-26 - 5) / 10);
								}
								else if (cschip.description.Contains("右"))
								{
									e.Graphics.TranslateTransform(-32, 0);
									rad = 360 + Math.Floor((double)(26 + 5) / 10);
								}
								rad = (rad * Math.PI) / 180;
								vo_pa = new PointF[4];
								vo_pa[0].X = (float)(Math.Cos(rad) * 40 + Math.Cos(rad + Math.PI / 2) * 3);
								vo_pa[0].Y = (float)(Math.Sin(rad) * 40 + Math.Sin(rad + Math.PI / 2) * 3);
								vo_pa[1].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad + Math.PI / 2) * 3);
								vo_pa[1].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad + Math.PI / 2) * 3);
								vo_pa[2].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad - Math.PI / 2) * 3);
								vo_pa[2].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad - Math.PI / 2) * 3);
								vo_pa[3].X = (float)(Math.Cos(rad) * 40 + Math.Cos(rad - Math.PI / 2) * 3);
								vo_pa[3].Y = (float)(Math.Sin(rad) * 40 + Math.Sin(rad - Math.PI / 2) * 3);
								brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
								e.Graphics.FillPolygon(brush, vo_pa);
								brush.Dispose();
								break;
							case "動くＴ字型":
								e.Graphics.TranslateTransform(16, 37);
								rad = 270;
								vo_pa = new PointF[3];
								vo_pa[0].X = (float)Math.Cos(((rad + 6) * Math.PI) / 180) * this.ChipImage.Width;
								vo_pa[0].Y = (float)Math.Sin(((rad + 6) * Math.PI) / 180) * this.ChipImage.Width;
								vo_pa[1].X = (float)Math.Cos(((rad - 6) * Math.PI) / 180) * this.ChipImage.Width;
								vo_pa[1].Y = (float)Math.Sin(((rad - 6) * Math.PI) / 180) * this.ChipImage.Width;
								vo_pa[2].X = 0;
								vo_pa[2].Y = 0;
								brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
								e.Graphics.FillPolygon(brush, vo_pa);
								vo_pa = new PointF[4];
								vo_pa[0].X = (float)Math.Cos(((rad + 20) * Math.PI) / 180) * this.ChipImage.Width * (float)1.3;
								vo_pa[0].Y = (float)Math.Sin(((rad + 20) * Math.PI) / 180) * this.ChipImage.Width;
								vo_pa[1].X = (float)Math.Cos(((rad - 20) * Math.PI) / 180) * this.ChipImage.Width * (float)1.3;
								vo_pa[1].Y = (float)Math.Sin(((rad - 20) * Math.PI) / 180) * this.ChipImage.Width;
								vo_pa[2].X = vo_pa[1].X + (float)Math.Cos((rad * Math.PI) / 180) * 5;
								vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin((rad * Math.PI) / 180) * 5;
								vo_pa[3].X = vo_pa[0].X + (float)Math.Cos((rad * Math.PI) / 180) * 5;
								vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin((rad * Math.PI) / 180) * 5;
								brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
								e.Graphics.FillPolygon(brush, vo_pa);
								brush.Dispose();
								break;
							case "ロープ":
							case "長いロープ":
							case "ゆれる棒":
								e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
									new Rectangle(0, 0, this.ChipImage.Width, this.ChipImage.Height),
									new Rectangle(cschip.pattern, this.ChipImage.Size), GraphicsUnit.Pixel);
								int length;
								if (cschip.name == "ロープ") length = 2;
								else length = 1;
								if (cschip.description == "つかまると左から動く")
								{
									e.Graphics.TranslateTransform(39, 5);
									rad = 168;
								}
								else
								{
									e.Graphics.TranslateTransform(16, -9);
									rad = 90;
								}
								vo_pa = new PointF[4];
								vo_pa[0].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
								vo_pa[0].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
								vo_pa[1].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
								vo_pa[1].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
								vo_pa[2].X = (float)(Math.Cos((rad * Math.PI) / 180) * this.ChipImage.Width * 1.2 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
								vo_pa[2].Y = (float)(Math.Sin((rad * Math.PI) / 180) * this.ChipImage.Width * 1.2 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
								vo_pa[3].X = (float)(Math.Cos((rad * Math.PI) / 180) * this.ChipImage.Width * 1.2 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
								vo_pa[3].Y = (float)(Math.Sin((rad * Math.PI) / 180) * this.ChipImage.Width * 1.2 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
								brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
								e.Graphics.FillPolygon(brush, vo_pa);
								brush.Dispose();
								break;
							case "人間大砲":
								if (cschip.description == "右向き") { rad = 330; e.Graphics.TranslateTransform(-9, 3); }
								else if (cschip.description == "左向き") { rad = 225; e.Graphics.TranslateTransform(9, 3); }
								else if (cschip.description == "天井") { rad = 30; e.Graphics.TranslateTransform(-9, 0); }
								else if (cschip.description == "右の壁") { rad = 270; e.Graphics.TranslateTransform(0, 9); }
								else if (cschip.description == "左の壁") { rad = 300; e.Graphics.TranslateTransform(0, 9); }
								brush = new SolidBrush(Global.cpd.project.Config.Mizunohadou);
								e.Graphics.FillEllipse(brush, 16 - 7, 16 - 7, 14, 14);
								vo_pa = new PointF[4];
								vo_pa[0].X = 16 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 7;
								vo_pa[0].Y = 16 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 7;
								vo_pa[1].X = 16 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 7;
								vo_pa[1].Y = 16 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 7;
								vo_pa[2].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 20 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 7;
								vo_pa[2].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 20 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 7;
								vo_pa[3].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 20 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 7;
								vo_pa[3].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 20 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 7;
								e.Graphics.FillPolygon(brush, vo_pa);
								brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
								if (cschip.description == "天井")
								{
									vo_pa[0].X = 16 - 2;
									vo_pa[0].Y = 16 + 1;
									vo_pa[1].X = 16 + 2;
									vo_pa[1].Y = 16 + 1;
									vo_pa[2].X = 16 + 5;
									vo_pa[2].Y = 0;
									vo_pa[3].X = 16 - 5;
									vo_pa[3].Y = 0;
								}
								else if (cschip.description == "右の壁")
								{
									vo_pa[0].X = 16 - 1;
									vo_pa[0].Y = 16 - 2;
									vo_pa[1].X = 16 - 1;
									vo_pa[1].Y = 16 + 2;
									vo_pa[2].X = this.ChipImage.Width;
									vo_pa[2].Y = 16 + 5;
									vo_pa[3].X = this.ChipImage.Width;
									vo_pa[3].Y = 16 - 5;
								}
								else if (cschip.description == "左の壁")
								{
									vo_pa[0].X = 16 + 1;
									vo_pa[0].Y = 16 - 2;
									vo_pa[1].X = 16 + 1;
									vo_pa[1].Y = 16 + 2;
									vo_pa[2].X = 0;
									vo_pa[2].Y = 16 + 5;
									vo_pa[3].X = 0;
									vo_pa[3].Y = 16 - 5;
								}
								else
								{
									vo_pa[0].X = 16 - 2;
									vo_pa[0].Y = 16 - 1;
									vo_pa[1].X = 16 + 2;
									vo_pa[1].Y = 16 - 1;
									vo_pa[2].X = 16 + 5;
									vo_pa[2].Y = this.ChipImage.Width - 3;
									vo_pa[3].X = 16 - 5;
									vo_pa[3].Y = this.ChipImage.Width - 3;
								}
								e.Graphics.FillPolygon(brush, vo_pa);
								brush.Dispose();
								break;
							case "曲線による上り坂":
							case "曲線による下り坂":
								var k21 = 0; float j20 = default, k20 = default, l20 = default, i21 = default;
								if (cschip.description.Contains("線のみ"))
								{
									vo_pa = new PointF[11];
									pen = new Pen(Global.cpd.project.Config.Firebar2, 1);
									for (var i1 = 0; i1 <= 50; i1 += 5)
									{
										if (cschip.name.Contains("上"))
											vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * this.ChipImage.Width * 5 / 8);
										else if (cschip.name.Contains("下"))
											vo_pa[k21].X = (float)Math.Floor(this.ChipImage.Width - Math.Sin((i1 * math_pi) / 180) * this.ChipImage.Width * 5 / 8);
										vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * math_pi) / 180) * this.ChipImage.Height * 5 / 8) - 1;
										if (i1 == 50)
										{
											j20 = vo_pa[k21].X;
											k20 = vo_pa[k21].Y;
										}
										k21++;
									}

									e.Graphics.DrawLines(pen, vo_pa);
									k21 = 0;
									for (var i1 = 0; i1 <= 50; i1 += 5)
									{
										if (cschip.name.Contains("上"))
											vo_pa[k21].X = (float)Math.Floor(this.ChipImage.Width - Math.Sin((i1 * math_pi) / 180) * this.ChipImage.Width * 5 / 8);
										else if (cschip.name.Contains("下"))
											vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * this.ChipImage.Width * 5 / 8);
										vo_pa[k21].Y = (float)Math.Floor(this.ChipImage.Height * 5 / 8 - Math.Cos((i1 * math_pi) / 180) * this.ChipImage.Height * 5 / 8) + 1;
										if (i1 == 50)
										{
											l20 = vo_pa[k21].X;
											i21 = vo_pa[k21].Y;
										}
										k21++;
									}
									e.Graphics.DrawLines(pen, vo_pa);
									vo_pa = new PointF[2];
									vo_pa[0].X = j20;
									vo_pa[0].Y = k20;
									vo_pa[1].X = l20;
									vo_pa[1].Y = i21;
									e.Graphics.DrawLines(pen, vo_pa);
									pen.Dispose();
								}
								else
								{
									vo_pa = new PointF[13];
									brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
									for (var i1 = 0; i1 <= 50; i1 += 5)
									{
										if (cschip.name.Contains("上"))
											vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * this.ChipImage.Width * 5 / 8);
										else if (cschip.name.Contains("下"))
											vo_pa[k21].X = (float)Math.Floor(this.ChipImage.Width - Math.Sin((i1 * math_pi) / 180) * this.ChipImage.Width * 5 / 8);
										vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * math_pi) / 180) * this.ChipImage.Height * 5 / 8);
										if (i1 == 50)
										{
											j20 = vo_pa[k21].X;
											k20 = vo_pa[k21].Y;
										}
										k21++;
									}

									vo_pa[k21].X = j20;
									vo_pa[k21].Y = this.ChipImage.Height;
									k21++;
									if (cschip.name.Contains("上")) vo_pa[k21].X = 0;
									else if (cschip.name.Contains("下")) vo_pa[k21].X = this.ChipImage.Width;
									vo_pa[k21].Y = this.ChipImage.Height;
									e.Graphics.FillPolygon(brush, vo_pa);
									vo_pa = new PointF[13];
									k21 = 0;
									for (var i1 = 0; i1 <= 50; i1 += 5)
									{
										if (cschip.name.Contains("上"))
											vo_pa[k21].X = (float)Math.Floor(this.ChipImage.Width - Math.Sin((i1 * math_pi) / 180) * this.ChipImage.Width * 5 / 8);
										else if (cschip.name.Contains("下"))
											vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * this.ChipImage.Width * 5 / 8);
										vo_pa[k21].Y = (float)Math.Floor(this.ChipImage.Height * 5 / 8 - Math.Cos((i1 * math_pi) / 180) * this.ChipImage.Height * 5 / 8);
										if (i1 == 50)
										{
											l20 = vo_pa[k21].X;
											i21 = vo_pa[k21].Y;
										}
										k21++;
									}

									vo_pa[k21].X = l20;
									vo_pa[k21].Y = this.ChipImage.Height;
									k21++;
									if (cschip.name.Contains("上")) vo_pa[k21].X = this.ChipImage.Width;
									else if (cschip.name.Contains("下")) vo_pa[k21].X = 0;
									vo_pa[k21].Y = this.ChipImage.Height;
									e.Graphics.FillPolygon(brush, vo_pa);
									vo_pa = new PointF[4];
									vo_pa[0].X = j20;
									vo_pa[0].Y = k20;
									vo_pa[1].X = l20;
									vo_pa[1].Y = i21;
									vo_pa[2].X = l20;
									vo_pa[2].Y = this.ChipImage.Height;
									vo_pa[3].X = j20;
									vo_pa[3].Y = this.ChipImage.Height;
									e.Graphics.FillPolygon(brush, vo_pa);
									brush.Dispose();
								}
								break;
							case "乗れる円":
							case "跳ねる円":
							case "円":
								int radius = default;
								e.Graphics.TranslateTransform(this.ChipImage.Width / 2, this.ChipImage.Width / 2);
								if (cschip.name == "円")
								{
									brush = new SolidBrush(Color.FromArgb(176, Global.cpd.project.Config.Mizunohadou));
									if (cschip.description.Contains("乗ると下がる"))
									{
										radius = 80 / 10;
									}
									else
									{
										radius = 112 / 10;
									}
								}
								else
								{
									brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
									if (cschip.description.Contains("大"))
									{
										if (cschip.name == "乗れる円")
										{
											radius = 144 / 10;
										}
										else if (cschip.name == "跳ねる円")
										{
											radius = 128 / 10;
										}
									}
									else
									{
										radius = 96 / 10;
									}
								}
								e.Graphics.FillEllipse(brush, -radius, -radius, radius * 2, radius * 2);
								brush.Dispose();
								break;
							case "半円":
								e.Graphics.TranslateTransform(1, 2);
								if (cschip.description.Contains("乗れる"))
								{
									if (cschip.description.Contains("線のみ"))
									{
										pen = new Pen(Global.cpd.project.Config.Firebar2, 1);
										vo_pa = new PointF[12];
										var j21 = 0;
										vo_pa[j21].X = 1 / 8;
										vo_pa[j21].Y = 63 / 8;
										j21++;
										for (var j = 140; j >= 90; j -= 5)
										{
											vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
											vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((j * math_pi) / 180) * 144) / 8;
											j21++;
										}
										e.Graphics.DrawLines(pen, vo_pa);
										j21 = 0;
										for (var k2 = 90; k2 >= 40; k2 -= 5)
										{
											vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
											vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
											j21++;
										}
										vo_pa[j21].X = 240 / 8;
										vo_pa[j21].Y = 63 / 8;
										e.Graphics.DrawLines(pen, vo_pa);
										pen.Dispose();
									}
									else
									{
										brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
										vo_pa = new PointF[13];
										var j21 = 0;
										vo_pa[j21].X = 0;
										vo_pa[j21].Y = 64 / 8;
										j21++;
										for (var j = 140; j >= 90; j -= 5)
										{
											vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
											vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144) / 8;
											j21++;
										}

										vo_pa[j21].X = 120 / 8;
										vo_pa[j21].Y = 64 / 8;
										e.Graphics.FillPolygon(brush, vo_pa);
										j21 = 0;
										for (var k2 = 90; k2 >= 40; k2 -= 5)
										{
											vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
											vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
											j21++;
										}

										vo_pa[j21].X = 240 / 8;
										vo_pa[j21].Y = 64 / 8;
										j21++;
										vo_pa[j21].X = 120 / 8;
										vo_pa[j21].Y = 64 / 8;
										e.Graphics.FillPolygon(brush, vo_pa);
										brush.Dispose();
									}
								}
								else
								{
									brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
									e.Graphics.FillRectangle(brush, (120 - 20) / 8, 64 / 8 - 1, 40 / 8, 22);
									brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
									vo_pa = new PointF[13];
									var j21 = 0;
									vo_pa[j21].X = 0;
									vo_pa[j21].Y = 64 / 8;
									j21++;
									for (var j = 140; j >= 90; j -= 5)
									{
										vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
										vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144) / 8;
										j21++;
									}

									vo_pa[j21].X = 120 / 8;
									vo_pa[j21].Y = 64 / 8;
									e.Graphics.FillPolygon(brush, vo_pa);
									j21 = 0;
									for (var k2 = 90; k2 >= 40; k2 -= 5)
									{
										vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
										vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
										j21++;
									}

									vo_pa[j21].X = 240 / 8;
									vo_pa[j21].Y = 64 / 8;
									j21++;
									vo_pa[j21].X = 120 / 8;
									vo_pa[j21].Y = 64 / 8;
									e.Graphics.FillPolygon(brush, vo_pa);
									brush.Dispose();
								}
								break;
							case "ファイヤーバー":
							case "スウィングファイヤーバー":
								{
									e.Graphics.TranslateTransform(this.ChipImage.Width / 2, this.ChipImage.Width / 2);
									e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
										new Rectangle(0, 0, this.ChipImage.Width / 2, this.ChipImage.Width / 2),
										new Rectangle(cschip.pattern, new Size(this.ChipImage.Width / 2, this.ChipImage.Width / 2)), GraphicsUnit.Pixel);
									int v = 225;
									e.Graphics.TranslateTransform(this.ChipImage.Width / 2, this.ChipImage.Width / 2);
									rad = ((v + 90) * Math.PI) / 180;
									const double d = 0.017453292519943295;
									vo_pa = new PointF[4];
									vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 25) + Math.Cos(rad) * 4);
									vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) + Math.Sin(rad) * 4);
									vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 25) - Math.Cos(rad) * 4);
									vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) - Math.Sin(rad) * 4);
									vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 42) - Math.Cos(rad) * 4);
									vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 42) - Math.Sin(rad) * 4);
									vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 42) + Math.Cos(rad) * 4);
									vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 42) + Math.Sin(rad) * 4);
									brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
									e.Graphics.FillPolygon(brush, vo_pa);
									// 内側の色を描画
									brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
									vo_pa[0].X = (float)(Math.Cos(v * d) * 28 + Math.Cos(rad) * 2);
									vo_pa[0].Y = (float)(Math.Sin(v * d) * 28 + Math.Sin(rad) * 2);
									vo_pa[1].X = (float)(Math.Cos(v * d) * 28 - Math.Cos(rad) * 2);
									vo_pa[1].Y = (float)(Math.Sin(v * d) * 28 - Math.Sin(rad) * 2);
									vo_pa[2].X = (float)(Math.Cos(v * d) * 40 - Math.Cos(rad) * 2);
									vo_pa[2].Y = (float)(Math.Sin(v * d) * 40 - Math.Sin(rad) * 2);
									vo_pa[3].X = (float)(Math.Cos(v * d) * 40 + Math.Cos(rad) * 2);
									vo_pa[3].Y = (float)(Math.Sin(v * d) * 40 + Math.Sin(rad) * 2);
									e.Graphics.FillPolygon(brush, vo_pa);
									brush.Dispose();
								}
								break;
							case "人口太陽":
								{
									e.Graphics.TranslateTransform(this.ChipImage.Width / 2, this.ChipImage.Width / 2);

									int v = default, n = default;
									const double d = 0.017453292519943295;
									vo_pa = new PointF[4];
									if (cschip.description.Contains("棒５本")) n = 5;
									else n = 3;
									if (cschip.description.Contains("左")) v = 360 - 2;
									else v = 2;

									for (var ii = 0; ii < n; ii++)
									{
										v += 360 / n;
										rad = ((v + 90) * Math.PI) / 180;
										brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
										vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 4) + Math.Cos(rad) * 3);
										vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 4) + Math.Sin(rad) * 3);
										vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 4) - Math.Cos(rad) * 3);
										vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 4) - Math.Sin(rad) * 3);
										vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 15) - Math.Cos(rad) * 3);
										vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) - Math.Sin(rad) * 3);
										vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 15) + Math.Cos(rad) * 3);
										vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) + Math.Sin(rad) * 3);
										e.Graphics.FillPolygon(brush, vo_pa);

										// 内側の色を描画
										brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
										vo_pa[0].X = (float)(Math.Cos(v * d) * 6 + Math.Cos(rad) * 1);
										vo_pa[0].Y = (float)(Math.Sin(v * d) * 6 + Math.Sin(rad) * 1);
										vo_pa[1].X = (float)(Math.Cos(v * d) * 6 - Math.Cos(rad) * 1);
										vo_pa[1].Y = (float)(Math.Sin(v * d) * 6 - Math.Sin(rad) * 1);
										vo_pa[2].X = (float)(Math.Cos(v * d) * 13 - Math.Cos(rad) * 1);
										vo_pa[2].Y = (float)(Math.Sin(v * d) * 13 - Math.Sin(rad) * 1);
										vo_pa[3].X = (float)(Math.Cos(v * d) * 13 + Math.Cos(rad) * 1);
										vo_pa[3].Y = (float)(Math.Sin(v * d) * 13 + Math.Sin(rad) * 1);
										e.Graphics.FillPolygon(brush, vo_pa);
									}
									brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
									e.Graphics.FillEllipse(brush, -6, -6, 12, 12);
									brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
									e.Graphics.FillEllipse(brush, -2, -2, 4, 4);

									brush.Dispose();
								}
								break;
							case "ファイヤーリング":
								{
									e.Graphics.TranslateTransform(this.ChipImage.Width / 2, this.ChipImage.Width / 2);

									int v = default, n = default;
									if (cschip.description.Contains("2本")) n = 2;
									else n = 3;
									if (cschip.description.Contains("左回り"))
									{
										if (cschip.description.Contains("高速")) v = -4 + 360;
										else v = -2 + 360;
									}
									else
									{
										if (cschip.description.Contains("高速")) v = 4;
										else v = 2;
									}

									brush = default;

									for (var ii = 0; ii < n; ii++)
									{
										var k6 = 0;
										if (cschip.description.Contains("2本"))
										{
											vo_pa = new PointF[26];
											for (var i4 = 0; i4 >= -120; i4 -= 10)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * this.ChipImage.Width / 2);
												vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * this.ChipImage.Width / 2);
												k6++;
											}

											for (var j4 = -120; j4 <= 0; j4 += 10)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * this.ChipImage.Width / 2 * 0.3);
												vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * this.ChipImage.Width / 2 * 0.3);
												k6++;
											}
										}
										else
										{
											vo_pa = new PointF[12];
											for (var i4 = 0; i4 >= -50; i4 -= 10)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * this.ChipImage.Width / 2);
												vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * this.ChipImage.Width / 2);
												k6++;
											}

											for (var j4 = -50; j4 <= 0; j4 += 10)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * this.ChipImage.Width / 2 * 0.3);
												vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * this.ChipImage.Width / 2 * 0.3);
												k6++;
											}
										}

										brush = new SolidBrush(Global.cpd.project.Config.Firebar1);
										e.Graphics.FillPolygon(brush, vo_pa);

										// 内側の色を描画
										k6 = 0;
										if (cschip.description.Contains("2本"))
										{
											vo_pa = new PointF[24];
											for (var k4 = -5; k4 >= -115; k4 -= 10)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * this.ChipImage.Width / 2 * 0.925);
												vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * this.ChipImage.Width / 2 * 0.925);
												k6++;
											}

											for (var l4 = -115; l4 <= -5; l4 += 10)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * this.ChipImage.Width / 2 * 0.5);
												vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * this.ChipImage.Width / 2 * 0.5);
												k6++;
											}
										}
										else
										{
											for (var k4 = -5; k4 >= -45; k4 -= 8)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * this.ChipImage.Width / 2 * 0.925);
												vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * this.ChipImage.Width / 2 * 0.925);
												k6++;
											}

											for (var l4 = -45; l4 <= -5; l4 += 8)
											{
												vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * this.ChipImage.Width / 2 * 0.5);
												vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * this.ChipImage.Width / 2 * 0.5);
												k6++;
											}
										}

										brush = new SolidBrush(Global.cpd.project.Config.Firebar2);
										e.Graphics.FillPolygon(brush, vo_pa);

										v += 360 / n;
									}

									brush.Dispose();
								}
								break;
							default:
								e.Graphics.TranslateTransform(this.ChipImage.Width / 2, this.ChipImage.Height / 2);
								if (Math.Abs(cschip.rotate) % 90 == 0) e.Graphics.RotateTransform(cschip.rotate);

								// 水の半透明処理
								if (Global.state.ChipRegister.ContainsKey("water_clear_switch") && bool.Parse(Global.state.ChipRegister["water_clear_switch"]) == false && Global.state.CurrentChip.character == "4" && Global.state.ChipRegister.ContainsKey("water_clear_level"))
								{
									float water_clear_level = float.Parse(Global.state.ChipRegister["water_clear_level"]);
                                    var colorMatrix = new ColorMatrix
                                    {
                                        Matrix00 = 1f,
                                        Matrix11 = 1f,
                                        Matrix22 = 1f,
                                        Matrix33 = water_clear_level / 255f,
                                        Matrix44 = 1f
                                    };
                                    using var imageAttributes = new ImageAttributes();
									imageAttributes.SetColorMatrix(colorMatrix);
									e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(new Point(-this.ChipImage.Width / 2, -this.ChipImage.Height / 2), this.ChipImage.Size), cschip.pattern.X, cschip.pattern.Y, Global.cpd.runtime.Definitions.ChipSize.Width, Global.cpd.runtime.Definitions.ChipSize.Height, GraphicsUnit.Pixel, imageAttributes);
								}
								else e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(new Point(-this.ChipImage.Width / 2, -this.ChipImage.Height / 2), this.ChipImage.Size), new Rectangle(cschip.pattern, (cschip.size == default(Size)) ? Global.cpd.runtime.Definitions.ChipSize : cschip.size), GraphicsUnit.Pixel);
								break;
						}
					}
				}
				else
				{
					e.Graphics.DrawImage(this.MainDesigner.DrawLayerOrig, new Rectangle(new Point(0, 0), this.ChipImage.Size), new Rectangle(cschip.pattern, (cschip.size == default(Size)) ? Global.cpd.runtime.Definitions.ChipSize : cschip.size), GraphicsUnit.Pixel);
				}
			}
		}

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

		public void UpdateStatus(string t)
		{
			this.CurrentStatus.Text = t;
			this.GUIEditorStatus.Refresh();
		}

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

		private void ItemCursor_Click(object sender, EventArgs e)
		{
			this.ItemUncheck();
			this.ItemCursor.Checked = true;
			this.GMCursor.Checked = true;
			this.MainDesigner.CurrentTool = GUIDesigner.EditTool.Cursor;
		}

		private void ItemPen_Click(object sender, EventArgs e)
		{
			this.ItemUncheck();
			this.ItemPen.Checked = true;
			this.GMPen.Checked = true;
			this.MainDesigner.CurrentTool = GUIDesigner.EditTool.Pen;
		}

		private void ItemLine_Click(object sender, EventArgs e)
		{
			this.ItemUncheck();
			this.ItemLine.Checked = true;
			this.GMLine.Checked = true;
			this.MainDesigner.CurrentTool = GUIDesigner.EditTool.Line;
		}

		private void ItemRect_Click(object sender, EventArgs e)
		{
			this.ItemUncheck();
			this.ItemRect.Checked = true;
			this.GMRect.Checked = true;
			this.MainDesigner.CurrentTool = GUIDesigner.EditTool.Rect;
		}

		private void ItemFill_Click(object sender, EventArgs e)
		{
			this.ItemUncheck();
			this.ItemFill.Checked = true;
			this.GMFill.Checked = true;
			this.MainDesigner.CurrentTool = GUIDesigner.EditTool.Fill;
		}

		private void ItemUndo_Click(object sender, EventArgs e)
		{
			this.MainDesigner.Undo();
		}

		private void GMUndo_Click(object sender, EventArgs e)
		{
			this.ItemUndo_Click(this, new EventArgs());
		}

		private void ItemRedo_Click(object sender, EventArgs e)
		{
			this.MainDesigner.Redo();
		}

		private void GMRedo_Click(object sender, EventArgs e)
		{
			this.ItemRedo_Click(this, new EventArgs());
		}

		private void GMCut_Click(object sender, EventArgs e)
		{
			this.GuiCut.Checked = !this.GuiCut.Checked;
			this.GuiCut_Click(sender, e);
		}

		private void GMCopy_Click(object sender, EventArgs e)
		{
			this.GuiCopy.Checked = !this.GuiCopy.Checked;
			this.GuiCopy_Click(sender, e);
		}

		private void GMPaste_Click(object sender, EventArgs e)
		{
			this.GuiPaste.Checked = !this.GuiPaste.Checked;
			this.GuiPaste_Click(sender, e);
		}

		private void MainDesigner_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			Keys keyCode = e.KeyCode;
			if (keyCode - Keys.Left <= 3)
			{
				e.IsInputKey = true;
				return;
			}
		}

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

		public void CopyPasteInit()
		{
			this.GuiCopy.Checked = false;
			this.GuiCut.Checked = false;
			this.GuiPaste.Checked = false;
			this.MainDesigner.CopyPaste = GUIDesigner.CopyPasteTool.None;
			this.MainDesigner.Refresh();
		}

		private void GUIProjConfig_Click(object sender, EventArgs e)
		{
			this.ProjectConfig_Click(this, new EventArgs());
		}

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

		private void ChipList_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index == -1)
			{
				return;
			}
			e.DrawBackground();
			try
			{
				ChipData cschip;
				using Brush brush = new SolidBrush(e.ForeColor);
				int width = 0;
				GraphicsState transState;
				Size chipsize = Global.cpd.runtime.Definitions.ChipSize;
				Pen pen;
				SolidBrush brush2;
				PointF[] vo_pa;
				double rad = 0;
				const double math_pi = 3.1415926535897931;

				switch (this.DrawType.SelectedIndex)
				{
					case 0: // サムネイル
						if (!Global.cpd.UseLayer || Global.state.EditingForeground)
						{
							if (Global.state.MapEditMode)
							{
								cschip = Global.cpd.Worldchip[e.Index].GetCSChip();
							}
							else
							{
								cschip = Global.cpd.Mapchip[e.Index].GetCSChip();
							}

							e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
							transState = e.Graphics.Save();
							e.Graphics.TranslateTransform(e.Bounds.X, e.Bounds.Y);
							if (Global.state.EditingForeground && Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 && Global.cpd.Mapchip[e.Index].character == "Z")
							{
								e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawOribossOrig, 0, 0, e.Bounds.Height, e.Bounds.Height);
							}
							else
							{
								switch (cschip.name)
								{
									case "一方通行":
										if (cschip.description.Contains("表示なし")) break;
										pen = new Pen(Global.cpd.project.Config.Firebar2, 2 / e.Bounds.Height);
										if (cschip.description.Contains("右"))
											e.Graphics.DrawLine(pen, e.Bounds.Height - 1, 0, e.Bounds.Height - 1, e.Bounds.Height);
										else if (cschip.description.Contains("左"))
											e.Graphics.DrawLine(pen, 1, 0, 1, e.Bounds.Height);
										else if (cschip.description.Contains("上"))
											e.Graphics.DrawLine(pen, 0, 1, e.Bounds.Height, 1);
										else if (cschip.description.Contains("下"))
											e.Graphics.DrawLine(pen, 0, e.Bounds.Height - 1, e.Bounds.Height, e.Bounds.Height - 1);
										pen.Dispose();
										break;
									case "左右へ押せるドッスンスンのゴール":
										pen = new Pen(Global.cpd.project.Config.Firebar1, 1);
										e.Graphics.TranslateTransform(1, 1);
										e.Graphics.DrawRectangle(pen, 0, 5, e.Bounds.Height - 1, e.Bounds.Height - 1 - 5);
										e.Graphics.TranslateTransform(-1, -1);
										e.Graphics.DrawLine(pen, 0, 5, e.Bounds.Height, e.Bounds.Height);
										e.Graphics.DrawLine(pen, 0, e.Bounds.Height, e.Bounds.Height, 5);
										pen.Dispose();
										break;
									case "シーソー":
										e.Graphics.TranslateTransform(e.Bounds.Height / 2, e.Bounds.Height / 2 + 1);
										vo_pa = new PointF[4];
										if (cschip.description.Contains("左")) rad = -56 * Math.PI / 180;
										else if (cschip.description.Contains("右")) rad = 56 * Math.PI / 180;
										vo_pa[0].X = (float)Math.Cos(rad + Math.PI) * e.Bounds.Height / 2;
										vo_pa[0].Y = (float)Math.Sin(rad + Math.PI) * e.Bounds.Height / 2;
										vo_pa[1].X = (float)Math.Cos(rad) * e.Bounds.Height / 2;
										vo_pa[1].Y = (float)Math.Sin(rad) * e.Bounds.Height / 2;
										vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad - Math.PI / 2) * e.Bounds.Height / 6;
										vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad - Math.PI / 2) * e.Bounds.Height / 6;
										vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad - Math.PI / 2) * e.Bounds.Height / 6;
										vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad - Math.PI / 2) * e.Bounds.Height / 6;
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										e.Graphics.FillPolygon(brush2, vo_pa);
										vo_pa = new PointF[3];
										vo_pa[0].X = 0;
										vo_pa[0].Y = 0;
										vo_pa[1].X = -2;
										vo_pa[1].Y = e.Bounds.Height / 2;
										vo_pa[2].X = 2;
										vo_pa[2].Y = e.Bounds.Height / 2;
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
										e.Graphics.FillPolygon(brush2, vo_pa);
										brush2.Dispose();
										break;
									case "ブランコ":
										e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
											new Rectangle(0, 0, e.Bounds.Height, e.Bounds.Height),
											new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
										e.Graphics.TranslateTransform(e.Bounds.Height / 2, -e.Bounds.Height / 2 + 3);
										rad = 90 * Math.PI / 180;
										vo_pa = new PointF[4];
										vo_pa[0].X = (float)Math.Cos(rad + Math.PI / 8) * e.Bounds.Height * (float)1.1;
										vo_pa[0].Y = (float)Math.Sin(rad + Math.PI / 8) * e.Bounds.Height * (float)1.1;
										vo_pa[1].X = (float)Math.Cos(rad - Math.PI / 8) * e.Bounds.Height * (float)1.1;
										vo_pa[1].Y = (float)Math.Sin(rad - Math.PI / 8) * e.Bounds.Height * (float)1.1;
										vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad) * 2;
										vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad) * 2;
										vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad) * 2;
										vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad) * 2;
										pen = new Pen(Global.cpd.project.Config.Firebar1, 1);
										e.Graphics.DrawLine(pen, 0, 5, 0, 8);
										e.Graphics.DrawLine(pen, vo_pa[0].X, vo_pa[0].Y, 0, 8);
										e.Graphics.DrawLine(pen, vo_pa[1].X, vo_pa[1].Y, 0, 8);
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										e.Graphics.FillPolygon(brush2, vo_pa);
										pen.Dispose();
										brush2.Dispose();
										break;
									case "スウィングバー":
										e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
											new Rectangle(0, 0, e.Bounds.Height, e.Bounds.Height),
											new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
										e.Graphics.TranslateTransform(0, 5);
										if (cschip.description.Contains("左"))
										{
											e.Graphics.TranslateTransform(20, 0);
											rad = 180 + Math.Floor((double)(-26 - 5) / 10);
										}
										else if (cschip.description.Contains("右"))
										{
											e.Graphics.TranslateTransform(-10, 0);
											rad = 360 + Math.Floor((double)(26 + 5) / 10);
										}
										rad = (rad * Math.PI) / 180;
										vo_pa = new PointF[4];
										vo_pa[0].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad + Math.PI / 2) * 1);
										vo_pa[0].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad + Math.PI / 2) * 1);
										vo_pa[1].X = (float)(Math.Cos(rad) * 10 + Math.Cos(rad + Math.PI / 2) * 1);
										vo_pa[1].Y = (float)(Math.Sin(rad) * 10 + Math.Sin(rad + Math.PI / 2) * 1);
										vo_pa[2].X = (float)(Math.Cos(rad) * 10 + Math.Cos(rad - Math.PI / 2) * 1);
										vo_pa[2].Y = (float)(Math.Sin(rad) * 10 + Math.Sin(rad - Math.PI / 2) * 1);
										vo_pa[3].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad - Math.PI / 2) * 1);
										vo_pa[3].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad - Math.PI / 2) * 1);
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										e.Graphics.FillPolygon(brush2, vo_pa);
										brush2.Dispose();
										break;
									case "動くＴ字型":
										e.Graphics.TranslateTransform(e.Bounds.Height / 2, e.Bounds.Height + 2);
										rad = 270;
										vo_pa = new PointF[3];
										vo_pa[0].X = (float)Math.Cos(((rad + 6) * Math.PI) / 180) * e.Bounds.Height;
										vo_pa[0].Y = (float)Math.Sin(((rad + 6) * Math.PI) / 180) * e.Bounds.Height;
										vo_pa[1].X = (float)Math.Cos(((rad - 6) * Math.PI) / 180) * e.Bounds.Height;
										vo_pa[1].Y = (float)Math.Sin(((rad - 6) * Math.PI) / 180) * e.Bounds.Height;
										vo_pa[2].X = 0;
										vo_pa[2].Y = 0;
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
										e.Graphics.FillPolygon(brush2, vo_pa);
										vo_pa = new PointF[4];
										vo_pa[0].X = (float)Math.Cos(((rad + 20) * Math.PI) / 180) * e.Bounds.Height * (float)1.3;
										vo_pa[0].Y = (float)Math.Sin(((rad + 20) * Math.PI) / 180) * e.Bounds.Height;
										vo_pa[1].X = (float)Math.Cos(((rad - 20) * Math.PI) / 180) * e.Bounds.Height * (float)1.3;
										vo_pa[1].Y = (float)Math.Sin(((rad - 20) * Math.PI) / 180) * e.Bounds.Height;
										vo_pa[2].X = vo_pa[1].X + (float)Math.Cos((rad * Math.PI) / 180) * 2;
										vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin((rad * Math.PI) / 180) * 2;
										vo_pa[3].X = vo_pa[0].X + (float)Math.Cos((rad * Math.PI) / 180) * 2;
										vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin((rad * Math.PI) / 180) * 2;
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										e.Graphics.FillPolygon(brush2, vo_pa);
										brush2.Dispose();
										break;
									case "ロープ":
									case "長いロープ":
									case "ゆれる棒":
										e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
											new Rectangle(0, 0, e.Bounds.Height, e.Bounds.Height),
											new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
										double length;
										if (cschip.name == "ロープ") length = 1;
										else length = 0.5;
										if (cschip.description == "つかまると左から動く")
										{
											e.Graphics.TranslateTransform(e.Bounds.Height * 2 - 3, 0);
											rad = 168;
										}
										else
										{
											e.Graphics.TranslateTransform(e.Bounds.Height / 2, -e.Bounds.Height);
											rad = 90;
										}
										vo_pa = new PointF[4];
										vo_pa[0].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
										vo_pa[0].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
										vo_pa[1].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
										vo_pa[1].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
										vo_pa[2].X = (float)(Math.Cos((rad * Math.PI) / 180) * e.Bounds.Height * 1.8 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
										vo_pa[2].Y = (float)(Math.Sin((rad * Math.PI) / 180) * e.Bounds.Height * 1.8 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
										vo_pa[3].X = (float)(Math.Cos((rad * Math.PI) / 180) * e.Bounds.Height * 1.8 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
										vo_pa[3].Y = (float)(Math.Sin((rad * Math.PI) / 180) * e.Bounds.Height * 1.8 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										e.Graphics.FillPolygon(brush2, vo_pa);
										brush2.Dispose();
										break;
									case "人間大砲":
										if (cschip.description == "右向き") { rad = 330; e.Graphics.TranslateTransform(0, 3); }
										else if (cschip.description == "左向き") { rad = 225; e.Graphics.TranslateTransform(0, 3); }
										else if (cschip.description == "天井") { rad = 30; e.Graphics.TranslateTransform(0, 0); }
										else if (cschip.description == "右の壁") { rad = 270; e.Graphics.TranslateTransform(0, 0); }
										else if (cschip.description == "左の壁") { rad = 300; e.Graphics.TranslateTransform(0, 0); }
										brush2 = new SolidBrush(Global.cpd.project.Config.Mizunohadou);
										e.Graphics.FillEllipse(brush2, e.Bounds.Height / 2 - 3, e.Bounds.Height / 2 - 3, 6, 6);
										vo_pa = new PointF[4];
										vo_pa[0].X = e.Bounds.Height / 2 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 3;
										vo_pa[0].Y = e.Bounds.Height / 2 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 3;
										vo_pa[1].X = e.Bounds.Height / 2 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 3;
										vo_pa[1].Y = e.Bounds.Height / 2 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 3;
										vo_pa[2].X = e.Bounds.Height / 2 + (float)Math.Cos((rad * Math.PI) / 180) * 6 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 3;
										vo_pa[2].Y = e.Bounds.Height / 2 + (float)Math.Sin((rad * Math.PI) / 180) * 6 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 3;
										vo_pa[3].X = e.Bounds.Height / 2 + (float)Math.Cos((rad * Math.PI) / 180) * 6 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 3;
										vo_pa[3].Y = e.Bounds.Height / 2 + (float)Math.Sin((rad * Math.PI) / 180) * 6 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 3;
										e.Graphics.FillPolygon(brush2, vo_pa);
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										if (cschip.description == "天井")
										{
											vo_pa[0].X = e.Bounds.Height / 2 - 1;
											vo_pa[0].Y = e.Bounds.Height / 2;
											vo_pa[1].X = e.Bounds.Height / 2 + 1;
											vo_pa[1].Y = e.Bounds.Height / 2;
											vo_pa[2].X = e.Bounds.Height / 2 + 2;
											vo_pa[2].Y = 0;
											vo_pa[3].X = e.Bounds.Height / 2 - 2;
											vo_pa[3].Y = 0;
										}
										else if (cschip.description == "右の壁")
										{
											vo_pa[0].X = e.Bounds.Height / 2;
											vo_pa[0].Y = e.Bounds.Height / 2 - 1;
											vo_pa[1].X = e.Bounds.Height / 2;
											vo_pa[1].Y = e.Bounds.Height / 2 + 1;
											vo_pa[2].X = e.Bounds.Height;
											vo_pa[2].Y = e.Bounds.Height / 2 + 2;
											vo_pa[3].X = e.Bounds.Height;
											vo_pa[3].Y = e.Bounds.Height / 2 - 2;
										}
										else if (cschip.description == "左の壁")
										{
											vo_pa[0].X = e.Bounds.Height / 2;
											vo_pa[0].Y = e.Bounds.Height / 2 - 1;
											vo_pa[1].X = e.Bounds.Height / 2;
											vo_pa[1].Y = e.Bounds.Height / 2 + 1;
											vo_pa[2].X = 0;
											vo_pa[2].Y = e.Bounds.Height / 2 + 2;
											vo_pa[3].X = 0;
											vo_pa[3].Y = e.Bounds.Height / 2 - 2;
										}
										else
										{
											vo_pa[0].X = e.Bounds.Height / 2 - 1;
											vo_pa[0].Y = e.Bounds.Height / 2;
											vo_pa[1].X = e.Bounds.Height / 2 + 1;
											vo_pa[1].Y = e.Bounds.Height / 2;
											vo_pa[2].X = e.Bounds.Height / 2 + 2;
											vo_pa[2].Y = e.Bounds.Height - 3;
											vo_pa[3].X = e.Bounds.Height / 2 - 2;
											vo_pa[3].Y = e.Bounds.Height - 3;
										}
										e.Graphics.FillPolygon(brush2, vo_pa);
										brush2.Dispose();
										break;
									case "曲線による上り坂":
									case "曲線による下り坂":
										var k21 = 0; float j20 = default, k20 = default, l20 = default, i21 = default;
										if (cschip.description.Contains("線のみ"))
										{
											vo_pa = new PointF[11];
											pen = new Pen(Global.cpd.project.Config.Firebar2, 1);
											for (var i1 = 0; i1 <= 50; i1 += 5)
											{
												if (cschip.name.Contains("上"))
													vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * e.Bounds.Height * 5 / 8);
												else if (cschip.name.Contains("下"))
													vo_pa[k21].X = (float)Math.Floor(e.Bounds.Height - Math.Sin((i1 * math_pi) / 180) * e.Bounds.Height * 5 / 8);
												vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * math_pi) / 180) * e.Bounds.Height * 5 / 8) - 1;
												if (i1 == 50)
												{
													j20 = vo_pa[k21].X;
													k20 = vo_pa[k21].Y;
												}
												k21++;
											}

											e.Graphics.DrawLines(pen, vo_pa);
											k21 = 0;
											for (var i1 = 0; i1 <= 50; i1 += 5)
											{
												if (cschip.name.Contains("上"))
													vo_pa[k21].X = (float)Math.Floor(e.Bounds.Height - Math.Sin((i1 * math_pi) / 180) * e.Bounds.Height * 5 / 8);
												else if (cschip.name.Contains("下"))
													vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * e.Bounds.Height * 5 / 8);
												vo_pa[k21].Y = (float)Math.Floor(e.Bounds.Height * 5 / 8 - Math.Cos((i1 * math_pi) / 180) * e.Bounds.Height * 5 / 8) + 1;
												if (i1 == 50)
												{
													l20 = vo_pa[k21].X;
													i21 = vo_pa[k21].Y;
												}
												k21++;
											}
											e.Graphics.DrawLines(pen, vo_pa);
											vo_pa = new PointF[2];
											vo_pa[0].X = j20;
											vo_pa[0].Y = k20;
											vo_pa[1].X = l20;
											vo_pa[1].Y = i21;
											e.Graphics.DrawLines(pen, vo_pa);
											pen.Dispose();
										}
										else
										{
											vo_pa = new PointF[13];
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
											for (var i1 = 0; i1 <= 50; i1 += 5)
											{
												if (cschip.name.Contains("上"))
													vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * e.Bounds.Height * 5 / 8);
												else if (cschip.name.Contains("下"))
													vo_pa[k21].X = (float)Math.Floor(e.Bounds.Height - Math.Sin((i1 * math_pi) / 180) * e.Bounds.Height * 5 / 8);
												vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * math_pi) / 180) * e.Bounds.Height * 5 / 8);
												if (i1 == 50)
												{
													j20 = vo_pa[k21].X;
													k20 = vo_pa[k21].Y;
												}
												k21++;
											}

											vo_pa[k21].X = j20;
											vo_pa[k21].Y = e.Bounds.Height;
											k21++;
											if (cschip.name.Contains("上")) vo_pa[k21].X = 0;
											else if (cschip.name.Contains("下")) vo_pa[k21].X = e.Bounds.Height;
											vo_pa[k21].Y = e.Bounds.Height;
											e.Graphics.FillPolygon(brush2, vo_pa);
											vo_pa = new PointF[13];
											k21 = 0;
											for (var i1 = 0; i1 <= 50; i1 += 5)
											{
												if (cschip.name.Contains("上"))
													vo_pa[k21].X = (float)Math.Floor(e.Bounds.Height - Math.Sin((i1 * math_pi) / 180) * e.Bounds.Height * 5 / 8);
												else if (cschip.name.Contains("下"))
													vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * e.Bounds.Height * 5 / 8);
												vo_pa[k21].Y = (float)Math.Floor(e.Bounds.Height * 5 / 8 - Math.Cos((i1 * math_pi) / 180) * e.Bounds.Height * 5 / 8);
												if (i1 == 50)
												{
													l20 = vo_pa[k21].X;
													i21 = vo_pa[k21].Y;
												}
												k21++;
											}

											vo_pa[k21].X = l20;
											vo_pa[k21].Y = e.Bounds.Height;
											k21++;
											if (cschip.name.Contains("上")) vo_pa[k21].X = e.Bounds.Height;
											else if (cschip.name.Contains("下")) vo_pa[k21].X = 0;
											vo_pa[k21].Y = e.Bounds.Height;
											e.Graphics.FillPolygon(brush2, vo_pa);
											vo_pa = new PointF[4];
											vo_pa[0].X = j20;
											vo_pa[0].Y = k20;
											vo_pa[1].X = l20;
											vo_pa[1].Y = i21;
											vo_pa[2].X = l20;
											vo_pa[2].Y = e.Bounds.Height;
											vo_pa[3].X = j20;
											vo_pa[3].Y = e.Bounds.Height;
											e.Graphics.FillPolygon(brush2, vo_pa);
											brush2.Dispose();
										}
										break;
									case "乗れる円":
									case "跳ねる円":
									case "円":
										int radius = default;
										e.Graphics.TranslateTransform(e.Bounds.Height / 2, e.Bounds.Height / 2);
										if (cschip.name == "円")
										{
											brush2 = new SolidBrush(Color.FromArgb(176, Global.cpd.project.Config.Mizunohadou));
											if (cschip.description.Contains("乗ると下がる"))
											{
												radius = 80 / 25;
											}
											else
											{
												radius = 112 / 25;
											}
										}
										else
										{
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
											if (cschip.description.Contains("大"))
											{
												if (cschip.name == "乗れる円")
												{
													radius = 144 / 25;
												}
												else if (cschip.name == "跳ねる円")
												{
													radius = 128 / 25;
												}
											}
											else
											{
												radius = 96 / 25;
											}
										}
										e.Graphics.FillEllipse(brush2, -radius, -radius, radius * 2, radius * 2);
										brush2.Dispose();
										break;
									case "半円":
										e.Graphics.TranslateTransform(1, 2);
										if (cschip.description.Contains("乗れる"))
										{
											if (cschip.description.Contains("線のみ"))
											{
												pen = new Pen(Global.cpd.project.Config.Firebar2, 1);
												vo_pa = new PointF[12];
												var j21 = 0;
												vo_pa[j21].X = 1 / 20;
												vo_pa[j21].Y = 63 / 20;
												j21++;
												for (var j = 140; j >= 90; j -= 5)
												{
													vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 20;
													vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((j * math_pi) / 180) * 144) / 20;
													j21++;
												}
												e.Graphics.DrawLines(pen, vo_pa);
												j21 = 0;
												for (var k2 = 90; k2 >= 40; k2 -= 5)
												{
													vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 20;
													vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((k2 * math_pi) / 180) * 144) / 20;
													j21++;
												}
												vo_pa[j21].X = 240 / 20;
												vo_pa[j21].Y = 63 / 20;
												e.Graphics.DrawLines(pen, vo_pa);
												pen.Dispose();
											}
											else
											{
												brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
												vo_pa = new PointF[13];
												var j21 = 0;
												vo_pa[j21].X = 0;
												vo_pa[j21].Y = 64 / 20;
												j21++;
												for (var j = 140; j >= 90; j -= 5)
												{
													vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 20;
													vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144) / 20;
													j21++;
												}

												vo_pa[j21].X = 120 / 20;
												vo_pa[j21].Y = 64 / 20;
												e.Graphics.FillPolygon(brush2, vo_pa);
												j21 = 0;
												for (var k2 = 90; k2 >= 40; k2 -= 5)
												{
													vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 20;
													vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144) / 20;
													j21++;
												}

												vo_pa[j21].X = 240 / 20;
												vo_pa[j21].Y = 64 / 20;
												j21++;
												vo_pa[j21].X = 120 / 20;
												vo_pa[j21].Y = 64 / 20;
												e.Graphics.FillPolygon(brush2, vo_pa);
												brush2.Dispose();
											}
										}
										else
										{
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
											e.Graphics.FillRectangle(brush2, (120 - 20) / 20, 64 / 20 - 1, 40 / 20, 7);
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
											vo_pa = new PointF[13];
											var j21 = 0;
											vo_pa[j21].X = 0;
											vo_pa[j21].Y = 64 / 20;
											j21++;
											for (var j = 140; j >= 90; j -= 5)
											{
												vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 20;
												vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144) / 20;
												j21++;
											}

											vo_pa[j21].X = 120 / 20;
											vo_pa[j21].Y = 64 / 20;
											e.Graphics.FillPolygon(brush2, vo_pa);
											j21 = 0;
											for (var k2 = 90; k2 >= 40; k2 -= 5)
											{
												vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 20;
												vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144) / 20;
												j21++;
											}

											vo_pa[j21].X = 240 / 20;
											vo_pa[j21].Y = 64 / 20;
											j21++;
											vo_pa[j21].X = 120 / 20;
											vo_pa[j21].Y = 64 / 20;
											e.Graphics.FillPolygon(brush2, vo_pa);
											brush2.Dispose();
										}
										break;
									case "ファイヤーバー":
									case "スウィングファイヤーバー":
										{
											e.Graphics.TranslateTransform(e.Bounds.Height / 2, e.Bounds.Height / 2);
											e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
												new Rectangle(0, 0, e.Bounds.Height / 2, e.Bounds.Height / 2),
												new Rectangle(cschip.pattern, new Size(chipsize.Width / 2, chipsize.Height / 2)), GraphicsUnit.Pixel);
											int v = 225;
											e.Graphics.TranslateTransform(e.Bounds.Height / 2, e.Bounds.Height / 2);
											rad = ((v + 90) * Math.PI) / 180;
											const double d = 0.017453292519943295;
											vo_pa = new PointF[4];
											vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 9) + Math.Cos(rad) * 2);
											vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 9) + Math.Sin(rad) * 2);
											vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 9) - Math.Cos(rad) * 2);
											vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 9) - Math.Sin(rad) * 2);
											vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 15) - Math.Cos(rad) * 2);
											vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) - Math.Sin(rad) * 2);
											vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 15) + Math.Cos(rad) * 2);
											vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) + Math.Sin(rad) * 2);
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
											e.Graphics.FillPolygon(brush2, vo_pa);
											// 内側の色を描画
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
											vo_pa[0].X = (float)(Math.Cos(v * d) * 11 + Math.Cos(rad) * 1);
											vo_pa[0].Y = (float)(Math.Sin(v * d) * 11 + Math.Sin(rad) * 1);
											vo_pa[1].X = (float)(Math.Cos(v * d) * 11 - Math.Cos(rad) * 1);
											vo_pa[1].Y = (float)(Math.Sin(v * d) * 11 - Math.Sin(rad) * 1);
											vo_pa[2].X = (float)(Math.Cos(v * d) * 14 - Math.Cos(rad) * 1);
											vo_pa[2].Y = (float)(Math.Sin(v * d) * 14 - Math.Sin(rad) * 1);
											vo_pa[3].X = (float)(Math.Cos(v * d) * 14 + Math.Cos(rad) * 1);
											vo_pa[3].Y = (float)(Math.Sin(v * d) * 14 + Math.Sin(rad) * 1);
											e.Graphics.FillPolygon(brush2, vo_pa);
											brush2.Dispose();
										}
										break;
									case "人口太陽":
										{
											e.Graphics.TranslateTransform(e.Bounds.Height / 2, e.Bounds.Height / 2);

											int v = default, n = default;
											const double d = 0.017453292519943295;
											vo_pa = new PointF[4];
											if (cschip.description.Contains("棒５本")) n = 5;
											else n = 3;
											if (cschip.description.Contains("左")) v = 360 - 2;
											else v = 2;

											for (var ii = 0; ii < n; ii++)
											{
												v += 360 / n;
												rad = ((v + 90) * Math.PI) / 180;
												brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
												vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 1) + Math.Cos(rad) * 1);
												vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 1) + Math.Sin(rad) * 1);
												vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 1) - Math.Cos(rad) * 1);
												vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 1) - Math.Sin(rad) * 1);
												vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 6) - Math.Cos(rad) * 1);
												vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 6) - Math.Sin(rad) * 1);
												vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 6) + Math.Cos(rad) * 1);
												vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 6) + Math.Sin(rad) * 1);
												e.Graphics.FillPolygon(brush2, vo_pa);

												// 内側の色を描画
												brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
												vo_pa[0].X = (float)(Math.Cos(v * d) * 2 + Math.Cos(rad) * 0.5);
												vo_pa[0].Y = (float)(Math.Sin(v * d) * 2 + Math.Sin(rad) * 0.5);
												vo_pa[1].X = (float)(Math.Cos(v * d) * 2 - Math.Cos(rad) * 0.5);
												vo_pa[1].Y = (float)(Math.Sin(v * d) * 2 - Math.Sin(rad) * 0.5);
												vo_pa[2].X = (float)(Math.Cos(v * d) * 5 - Math.Cos(rad) * 0.5);
												vo_pa[2].Y = (float)(Math.Sin(v * d) * 5 - Math.Sin(rad) * 0.5);
												vo_pa[3].X = (float)(Math.Cos(v * d) * 5 + Math.Cos(rad) * 0.5);
												vo_pa[3].Y = (float)(Math.Sin(v * d) * 5 + Math.Sin(rad) * 0.5);
												e.Graphics.FillPolygon(brush2, vo_pa);
											}
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
											e.Graphics.FillEllipse(brush2, -2, -2, 4, 4);
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
											e.Graphics.FillEllipse(brush2, -1, -1, 2, 2);

											brush2.Dispose();
										}
										break;
									case "ファイヤーリング":
										{
											e.Graphics.TranslateTransform(e.Bounds.Height / 2, e.Bounds.Height / 2);

											int v = default, n = default;
											if (cschip.description.Contains("2本")) n = 2;
											else n = 3;
											if (cschip.description.Contains("左回り"))
											{
												if (cschip.description.Contains("高速")) v = -4 + 360;
												else v = -2 + 360;
											}
											else
											{
												if (cschip.description.Contains("高速")) v = 4;
												else v = 2;
											}

											brush2 = default;

											for (var ii = 0; ii < n; ii++)
											{
												var k6 = 0;
												if (cschip.description.Contains("2本"))
												{
													vo_pa = new PointF[26];
													for (var i4 = 0; i4 >= -120; i4 -= 10)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * e.Bounds.Height / 2);
														vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * e.Bounds.Height / 2);
														k6++;
													}

													for (var j4 = -120; j4 <= 0; j4 += 10)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * e.Bounds.Height / 2 * 0.3);
														vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * e.Bounds.Height / 2 * 0.3);
														k6++;
													}
												}
												else
												{
													vo_pa = new PointF[12];
													for (var i4 = 0; i4 >= -50; i4 -= 10)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * e.Bounds.Height / 2);
														vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * e.Bounds.Height / 2);
														k6++;
													}

													for (var j4 = -50; j4 <= 0; j4 += 10)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * e.Bounds.Height / 2 * 0.3);
														vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * e.Bounds.Height / 2 * 0.3);
														k6++;
													}
												}

												brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
												e.Graphics.FillPolygon(brush2, vo_pa);

												// 内側の色を描画
												k6 = 0;
												if (cschip.description.Contains("2本"))
												{
													vo_pa = new PointF[24];
													for (var k4 = -5; k4 >= -115; k4 -= 10)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * e.Bounds.Height / 2 * 0.925);
														vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * e.Bounds.Height / 2 * 0.925);
														k6++;
													}

													for (var l4 = -115; l4 <= -5; l4 += 10)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * e.Bounds.Height / 2 * 0.5);
														vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * e.Bounds.Height / 2 * 0.5);
														k6++;
													}
												}
												else
												{
													for (var k4 = -5; k4 >= -45; k4 -= 8)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * e.Bounds.Height / 2 * 0.925);
														vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * e.Bounds.Height / 2 * 0.925);
														k6++;
													}

													for (var l4 = -45; l4 <= -5; l4 += 8)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * e.Bounds.Height / 2 * 0.5);
														vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * e.Bounds.Height / 2 * 0.5);
														k6++;
													}
												}

												brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
												e.Graphics.FillPolygon(brush2, vo_pa);

												v += 360 / n;
											}

											brush2.Dispose();
										}
										break;
									default:
										e.Graphics.TranslateTransform(e.Bounds.Height / 2, e.Bounds.Height / 2);
										if (Math.Abs(cschip.rotate) % 90 == 0) e.Graphics.RotateTransform(cschip.rotate);
										// 水の半透明処理
										if (Global.state.ChipRegister.ContainsKey("water_clear_switch") && bool.Parse(Global.state.ChipRegister["water_clear_switch"]) == false && Global.cpd.Mapchip[e.Index].character == "4" && Global.state.ChipRegister.ContainsKey("water_clear_level"))
											{
											float water_clear_level = float.Parse(Global.state.ChipRegister["water_clear_level"]);
											var colorMatrix = new ColorMatrix
											{
												Matrix00 = 1f,
												Matrix11 = 1f,
												Matrix22 = 1f,
												Matrix33 = water_clear_level / 255f,
												Matrix44 = 1f
											};
											using var imageAttributes = new ImageAttributes();
											imageAttributes.SetColorMatrix(colorMatrix);
											e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(-e.Bounds.Height / 2, -e.Bounds.Height / 2, e.Bounds.Height, e.Bounds.Height), cschip.pattern.X, cschip.pattern.Y, chipsize.Width, chipsize.Height, GraphicsUnit.Pixel, imageAttributes);
										}
										else e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(-e.Bounds.Height / 2, -e.Bounds.Height / 2, e.Bounds.Height, e.Bounds.Height), new Rectangle(cschip.pattern, (cschip.size == default(Size)) ? chipsize : cschip.size), GraphicsUnit.Pixel);
										break;
								}
							}
							e.Graphics.Restore(transState);
						}
						else
						{
							cschip = Global.cpd.Layerchip[e.Index].GetCSChip();
							e.Graphics.DrawImage(this.MainDesigner.DrawLayerOrig, new Rectangle(e.Bounds.Location, new Size(e.Bounds.Height, e.Bounds.Height)), new Rectangle(cschip.pattern, (cschip.size == default(Size)) ? chipsize : cschip.size), GraphicsUnit.Pixel);
						}
						width = e.Bounds.Height;
					break;
					case 2: // チップ
						if (!Global.cpd.UseLayer || Global.state.EditingForeground)
						{
							if (Global.state.MapEditMode)
							{
								cschip = Global.cpd.Worldchip[e.Index].GetCSChip();
							}
							else
							{
								cschip = Global.cpd.Mapchip[e.Index].GetCSChip();
							}

							e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
							transState = e.Graphics.Save();
							
							e.Graphics.TranslateTransform(e.Bounds.X, e.Bounds.Y);
							if (cschip.size == default(Size))
							{
								switch (cschip.name)
								{
									case "一方通行":
										if (cschip.description.Contains("表示なし")) break;
										pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
										if (cschip.description.Contains("右"))
											e.Graphics.DrawLine(pen, chipsize.Width - 1, 0, chipsize.Width - 1, chipsize.Height);
										else if (cschip.description.Contains("左"))
											e.Graphics.DrawLine(pen, 1, 0, 1, chipsize.Height);
										else if (cschip.description.Contains("上"))
											e.Graphics.DrawLine(pen, 0, 1, chipsize.Width, 1);
										else if (cschip.description.Contains("下"))
											e.Graphics.DrawLine(pen, 0, chipsize.Height - 1, chipsize.Width, chipsize.Height - 1);
										pen.Dispose();
										break;
									case "左右へ押せるドッスンスンのゴール":
										pen = new Pen(Global.cpd.project.Config.Firebar1, 1);
										e.Graphics.TranslateTransform(1, 1);
										e.Graphics.DrawRectangle(pen, 0, 11, chipsize.Width - 1, chipsize.Height - 1 - 11);
										e.Graphics.TranslateTransform(-1, -1);
										e.Graphics.DrawLine(pen, 0, 11, chipsize.Width, chipsize.Height);
										e.Graphics.DrawLine(pen, 0, chipsize.Height, chipsize.Width, 11);
										pen.Dispose();
										break;
									case "シーソー":
										e.Graphics.TranslateTransform(16, 17);
										vo_pa = new PointF[4];
										if (cschip.description.Contains("左")) rad = -56 * Math.PI / 180;
										else if (cschip.description.Contains("右")) rad = 56 * Math.PI / 180;
										vo_pa[0].X = (float)Math.Cos(rad + Math.PI) * chipsize.Width / 2;
										vo_pa[0].Y = (float)Math.Sin(rad + Math.PI) * chipsize.Width / 2;
										vo_pa[1].X = (float)Math.Cos(rad) * chipsize.Width / 2;
										vo_pa[1].Y = (float)Math.Sin(rad) * chipsize.Width / 2;
										vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad - Math.PI / 2) * 5;
										vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad - Math.PI / 2) * 5;
										vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad - Math.PI / 2) * 5;
										vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad - Math.PI / 2) * 5;
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										e.Graphics.FillPolygon(brush2, vo_pa);
										vo_pa = new PointF[3];
										vo_pa[0].X = 0;
										vo_pa[0].Y = -2;
										vo_pa[1].X = -4;
										vo_pa[1].Y = 15;
										vo_pa[2].X = 4;
										vo_pa[2].Y = 15;
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
										e.Graphics.FillPolygon(brush2, vo_pa);
										brush2.Dispose();
										break;
									case "ブランコ":
										e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
											new Rectangle(0, 0, chipsize.Width, chipsize.Height),
											new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
										e.Graphics.TranslateTransform(16, -9);
										rad = 90 * Math.PI / 180;
										vo_pa = new PointF[4];
										vo_pa[0].X = (float)Math.Cos(rad + Math.PI / 9) * chipsize.Width * (float)1.15;
										vo_pa[0].Y = (float)Math.Sin(rad + Math.PI / 9) * chipsize.Width * (float)1.15;
										vo_pa[1].X = (float)Math.Cos(rad - Math.PI / 9) * chipsize.Width * (float)1.15;
										vo_pa[1].Y = (float)Math.Sin(rad - Math.PI / 9) * chipsize.Width * (float)1.15;
										vo_pa[2].X = vo_pa[1].X + (float)Math.Cos(rad) * 5;
										vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin(rad) * 5;
										vo_pa[3].X = vo_pa[0].X + (float)Math.Cos(rad) * 5;
										vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin(rad) * 5;
										double dx = Math.Cos(rad) * 21;
										double dy = Math.Sin(rad) * 21;
										pen = new Pen(Global.cpd.project.Config.Firebar1, 2);
										e.Graphics.DrawLine(pen, (float)Math.Cos(rad) * 10, (float)Math.Sin(rad) * 10, (float)dx, (float)dy);
										e.Graphics.DrawLine(pen, vo_pa[0].X, vo_pa[0].Y, (float)dx, (float)dy);
										e.Graphics.DrawLine(pen, vo_pa[1].X, vo_pa[1].Y, (float)dx, (float)dy);
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										e.Graphics.FillPolygon(brush2, vo_pa);
										pen.Dispose();
										brush2.Dispose();
										break;
									case "スウィングバー":
										e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
											new Rectangle(0, 0, chipsize.Width, chipsize.Height),
											new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
										e.Graphics.TranslateTransform(16, 14);
										if (cschip.description.Contains("左"))
										{
											e.Graphics.TranslateTransform(32, 0);
											rad = 180 + Math.Floor((double)(-26 - 5) / 10);
										}
										else if (cschip.description.Contains("右"))
										{
											e.Graphics.TranslateTransform(-32, 0);
											rad = 360 + Math.Floor((double)(26 + 5) / 10);
										}
										rad = (rad * Math.PI) / 180;
										vo_pa = new PointF[4];
										vo_pa[0].X = (float)(Math.Cos(rad) * 40 + Math.Cos(rad + Math.PI / 2) * 3);
										vo_pa[0].Y = (float)(Math.Sin(rad) * 40 + Math.Sin(rad + Math.PI / 2) * 3);
										vo_pa[1].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad + Math.PI / 2) * 3);
										vo_pa[1].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad + Math.PI / 2) * 3);
										vo_pa[2].X = (float)(Math.Cos(rad) * 20 + Math.Cos(rad - Math.PI / 2) * 3);
										vo_pa[2].Y = (float)(Math.Sin(rad) * 20 + Math.Sin(rad - Math.PI / 2) * 3);
										vo_pa[3].X = (float)(Math.Cos(rad) * 40 + Math.Cos(rad - Math.PI / 2) * 3);
										vo_pa[3].Y = (float)(Math.Sin(rad) * 40 + Math.Sin(rad - Math.PI / 2) * 3);
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										e.Graphics.FillPolygon(brush2, vo_pa);
										brush2.Dispose();
										break;
									case "動くＴ字型":
										e.Graphics.TranslateTransform(16, 37);
										rad = 270;
										vo_pa = new PointF[3];
										vo_pa[0].X = (float)Math.Cos(((rad + 6) * Math.PI) / 180) * chipsize.Width;
										vo_pa[0].Y = (float)Math.Sin(((rad + 6) * Math.PI) / 180) * chipsize.Width;
										vo_pa[1].X = (float)Math.Cos(((rad - 6) * Math.PI) / 180) * chipsize.Width;
										vo_pa[1].Y = (float)Math.Sin(((rad - 6) * Math.PI) / 180) * chipsize.Width;
										vo_pa[2].X = 0;
										vo_pa[2].Y = 0;
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
										e.Graphics.FillPolygon(brush2, vo_pa);
										vo_pa = new PointF[4];
										vo_pa[0].X = (float)Math.Cos(((rad + 20) * Math.PI) / 180) * chipsize.Width * (float)1.3;
										vo_pa[0].Y = (float)Math.Sin(((rad + 20) * Math.PI) / 180) * chipsize.Width;
										vo_pa[1].X = (float)Math.Cos(((rad - 20) * Math.PI) / 180) * chipsize.Width * (float)1.3;
										vo_pa[1].Y = (float)Math.Sin(((rad - 20) * Math.PI) / 180) * chipsize.Width;
										vo_pa[2].X = vo_pa[1].X + (float)Math.Cos((rad * Math.PI) / 180) * 5;
										vo_pa[2].Y = vo_pa[1].Y + (float)Math.Sin((rad * Math.PI) / 180) * 5;
										vo_pa[3].X = vo_pa[0].X + (float)Math.Cos((rad * Math.PI) / 180) * 5;
										vo_pa[3].Y = vo_pa[0].Y + (float)Math.Sin((rad * Math.PI) / 180) * 5; 
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										e.Graphics.FillPolygon(brush2, vo_pa);
										brush2.Dispose();
										break;
									case "ロープ":
									case "長いロープ":
									case "ゆれる棒":
										e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
											new Rectangle(0, 0, chipsize.Width, chipsize.Height),
											new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
										int length;
										if (cschip.name == "ロープ") length = 2;
										else length = 1;
										if (cschip.description == "つかまると左から動く")
										{
											e.Graphics.TranslateTransform(39, 5);
											rad = 168;
										}
										else
										{
											e.Graphics.TranslateTransform(16, -9);
											rad = 90;
										}
										vo_pa = new PointF[4];
										vo_pa[0].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
										vo_pa[0].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
										vo_pa[1].X = (float)(Math.Cos((rad * Math.PI) / 180) * 12 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
										vo_pa[1].Y = (float)(Math.Sin((rad * Math.PI) / 180) * 12 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
										vo_pa[2].X = (float)(Math.Cos((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Cos(((rad - 90) * Math.PI) / 180) * length);
										vo_pa[2].Y = (float)(Math.Sin((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Sin(((rad - 90) * Math.PI) / 180) * length);
										vo_pa[3].X = (float)(Math.Cos((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Cos(((rad + 90) * Math.PI) / 180) * length);
										vo_pa[3].Y = (float)(Math.Sin((rad * Math.PI) / 180) * chipsize.Width * 1.2 + Math.Sin(((rad + 90) * Math.PI) / 180) * length);
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										e.Graphics.FillPolygon(brush2, vo_pa);
										brush2.Dispose();
										break;
									case "人間大砲":
										if (cschip.description == "右向き") { rad = 330; e.Graphics.TranslateTransform(-9, 3); }
										else if (cschip.description == "左向き") { rad = 225; e.Graphics.TranslateTransform(9, 3); }
										else if (cschip.description == "天井") { rad = 30; e.Graphics.TranslateTransform(-9, 0); }
										else if (cschip.description == "右の壁") { rad = 270; e.Graphics.TranslateTransform(0, 9); }
										else if (cschip.description == "左の壁") { rad = 300; e.Graphics.TranslateTransform(0, 9); }
										brush2 = new SolidBrush(Global.cpd.project.Config.Mizunohadou);
										e.Graphics.FillEllipse(brush2, 16 - 7, 16 - 7, 14, 14);
										vo_pa = new PointF[4];
										vo_pa[0].X = 16 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 7;
										vo_pa[0].Y = 16 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 7;
										vo_pa[1].X = 16 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 7;
										vo_pa[1].Y = 16 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 7;
										vo_pa[2].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 20 + (float)Math.Cos(((rad - 90) * Math.PI) / 180) * 7;
										vo_pa[2].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 20 + (float)Math.Sin(((rad - 90) * Math.PI) / 180) * 7;
										vo_pa[3].X = 16 + (float)Math.Cos((rad * Math.PI) / 180) * 20 + (float)Math.Cos(((rad + 90) * Math.PI) / 180) * 7;
										vo_pa[3].Y = 16 + (float)Math.Sin((rad * Math.PI) / 180) * 20 + (float)Math.Sin(((rad + 90) * Math.PI) / 180) * 7;
										e.Graphics.FillPolygon(brush2, vo_pa);
										brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
										if (cschip.description == "天井")
										{
											vo_pa[0].X = 16 - 2;
											vo_pa[0].Y = 16 + 1;
											vo_pa[1].X = 16 + 2;
											vo_pa[1].Y = 16 + 1;
											vo_pa[2].X = 16 + 5;
											vo_pa[2].Y = 0;
											vo_pa[3].X = 16 - 5;
											vo_pa[3].Y = 0;
										}
										else if (cschip.description == "右の壁")
										{
											vo_pa[0].X = 16 - 1;
											vo_pa[0].Y = 16 - 2;
											vo_pa[1].X = 16 - 1;
											vo_pa[1].Y = 16 + 2;
											vo_pa[2].X = chipsize.Width;
											vo_pa[2].Y = 16 + 5;
											vo_pa[3].X = chipsize.Width;
											vo_pa[3].Y = 16 - 5;
										}
										else if (cschip.description == "左の壁")
										{
											vo_pa[0].X = 16 + 1;
											vo_pa[0].Y = 16 - 2;
											vo_pa[1].X = 16 + 1;
											vo_pa[1].Y = 16 + 2;
											vo_pa[2].X = 0;
											vo_pa[2].Y = 16 + 5;
											vo_pa[3].X = 0;
											vo_pa[3].Y = 16 - 5;
										}
										else
										{
											vo_pa[0].X = 16 - 2;
											vo_pa[0].Y = 16 - 1;
											vo_pa[1].X = 16 + 2;
											vo_pa[1].Y = 16 - 1;
											vo_pa[2].X = 16 + 5;
											vo_pa[2].Y = chipsize.Width - 3;
											vo_pa[3].X = 16 - 5;
											vo_pa[3].Y = chipsize.Width - 3;
										}
										e.Graphics.FillPolygon(brush2, vo_pa);
										brush2.Dispose();
										break;
									case "曲線による上り坂":
									case "曲線による下り坂":
										var k21 = 0; float j20 = default, k20 = default, l20 = default, i21 = default;
										if (cschip.description.Contains("線のみ"))
										{
											vo_pa = new PointF[11];
											pen = new Pen(Global.cpd.project.Config.Firebar2, 1);
											for (var i1 = 0; i1 <= 50; i1 += 5)
											{
												if (cschip.name.Contains("上"))
													vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
												else if (cschip.name.Contains("下"))
													vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
												vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * math_pi) / 180) * chipsize.Height * 5 / 8) - 1;
												if (i1 == 50)
												{
													j20 = vo_pa[k21].X;
													k20 = vo_pa[k21].Y;
												}
												k21++;
											}

											e.Graphics.DrawLines(pen, vo_pa);
											k21 = 0;
											for (var i1 = 0; i1 <= 50; i1 += 5)
											{
												if (cschip.name.Contains("上"))
													vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
												else if (cschip.name.Contains("下"))
													vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
												vo_pa[k21].Y = (float)Math.Floor(chipsize.Height * 5 / 8 - Math.Cos((i1 * math_pi) / 180) * chipsize.Height * 5 / 8) + 1;
												if (i1 == 50)
												{
													l20 = vo_pa[k21].X;
													i21 = vo_pa[k21].Y;
												}
												k21++;
											}
											e.Graphics.DrawLines(pen, vo_pa);
											vo_pa = new PointF[2];
											vo_pa[0].X = j20;
											vo_pa[0].Y = k20;
											vo_pa[1].X = l20;
											vo_pa[1].Y = i21;
											e.Graphics.DrawLines(pen, vo_pa);
											pen.Dispose();
										}
										else
										{
											vo_pa = new PointF[13];
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
											for (var i1 = 0; i1 <= 50; i1 += 5)
											{
												if (cschip.name.Contains("上"))
													vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
												else if (cschip.name.Contains("下"))
													vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
												vo_pa[k21].Y = (float)Math.Floor(Math.Cos((i1 * math_pi) / 180) * chipsize.Height * 5 / 8);
												if (i1 == 50)
												{
													j20 = vo_pa[k21].X;
													k20 = vo_pa[k21].Y;
												}
												k21++;
											}

											vo_pa[k21].X = j20;
											vo_pa[k21].Y = chipsize.Height;
											k21++;
											if (cschip.name.Contains("上")) vo_pa[k21].X = 0;
											else if (cschip.name.Contains("下")) vo_pa[k21].X = chipsize.Width;
											vo_pa[k21].Y = chipsize.Height;
											e.Graphics.FillPolygon(brush2, vo_pa);
											vo_pa = new PointF[13];
											k21 = 0;
											for (var i1 = 0; i1 <= 50; i1 += 5)
											{
												if (cschip.name.Contains("上"))
													vo_pa[k21].X = (float)Math.Floor(chipsize.Width - Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
												else if (cschip.name.Contains("下"))
													vo_pa[k21].X = (float)Math.Floor(Math.Sin((i1 * math_pi) / 180) * chipsize.Width * 5 / 8);
												vo_pa[k21].Y = (float)Math.Floor(chipsize.Height * 5 / 8 - Math.Cos((i1 * math_pi) / 180) * chipsize.Height * 5 / 8);
												if (i1 == 50)
												{
													l20 = vo_pa[k21].X;
													i21 = vo_pa[k21].Y;
												}
												k21++;
											}

											vo_pa[k21].X = l20;
											vo_pa[k21].Y = chipsize.Height;
											k21++;
											if (cschip.name.Contains("上")) vo_pa[k21].X = chipsize.Width;
											else if (cschip.name.Contains("下")) vo_pa[k21].X = 0;
											vo_pa[k21].Y = chipsize.Height;
											e.Graphics.FillPolygon(brush2, vo_pa);
											vo_pa = new PointF[4];
											vo_pa[0].X = j20;
											vo_pa[0].Y = k20;
											vo_pa[1].X = l20;
											vo_pa[1].Y = i21;
											vo_pa[2].X = l20;
											vo_pa[2].Y = chipsize.Height;
											vo_pa[3].X = j20;
											vo_pa[3].Y = chipsize.Height;
											e.Graphics.FillPolygon(brush2, vo_pa);
											brush2.Dispose();
										}
										break;
									case "乗れる円":
									case "跳ねる円":
									case "円":
										int radius = default;
										e.Graphics.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);
										if (cschip.name == "円")
										{
											brush2 = new SolidBrush(Color.FromArgb(176, Global.cpd.project.Config.Mizunohadou));
											if (cschip.description.Contains("乗ると下がる"))
											{
												radius = 80 / 10;
											}
											else
											{
												radius = 112 / 10;
											}
										}
										else
										{
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
											if (cschip.description.Contains("大"))
											{
												if (cschip.name == "乗れる円")
												{
													radius = 144 / 10;
												}
												else if (cschip.name == "跳ねる円")
												{
													radius = 128 / 10;
												}
											}
											else
											{
												radius = 96 / 10;
											}
										}
										e.Graphics.FillEllipse(brush2, -radius, -radius, radius * 2, radius * 2);
										brush2.Dispose();
										break;
									case "半円":
										e.Graphics.TranslateTransform(1, 2);
										if (cschip.description.Contains("乗れる"))
										{
											if (cschip.description.Contains("線のみ"))
											{
												pen = new Pen(Global.cpd.project.Config.Firebar2, 1);
												vo_pa = new PointF[12];
												var j21 = 0;
												vo_pa[j21].X = 1 / 8;
												vo_pa[j21].Y = 63 / 8;
												j21++;
												for (var j = 140; j >= 90; j -= 5)
												{
													vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
													vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((j * math_pi) / 180) * 144) / 8;
													j21++;
												}
												e.Graphics.DrawLines(pen, vo_pa);
												j21 = 0;
												for (var k2 = 90; k2 >= 40; k2 -= 5)
												{
													vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
													vo_pa[j21].Y = (float)Math.Floor(145 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
													j21++;
												}
												vo_pa[j21].X = 240 / 8;
												vo_pa[j21].Y = 63 / 8;
												e.Graphics.DrawLines(pen, vo_pa);
												pen.Dispose();
											}
											else
											{
												brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
												vo_pa = new PointF[13];
												var j21 = 0;
												vo_pa[j21].X = 0;
												vo_pa[j21].Y = 64 / 8;
												j21++;
												for (var j = 140; j >= 90; j -= 5)
												{
													vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
													vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144) / 8;
													j21++;
												}

												vo_pa[j21].X = 120 / 8;
												vo_pa[j21].Y = 64 / 8;
												e.Graphics.FillPolygon(brush2, vo_pa);
												j21 = 0;
												for (var k2 = 90; k2 >= 40; k2 -= 5)
												{
													vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
													vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
													j21++;
												}

												vo_pa[j21].X = 240 / 8;
												vo_pa[j21].Y = 64 / 8;
												j21++;
												vo_pa[j21].X = 120 / 8;
												vo_pa[j21].Y = 64 / 8;
												e.Graphics.FillPolygon(brush2, vo_pa);
												brush2.Dispose();
											}
										}
										else
										{
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
											e.Graphics.FillRectangle(brush2, (120 - 20) / 8, 64 / 8 - 1, 40 / 8, 22);
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
											vo_pa = new PointF[13];
											var j21 = 0;
											vo_pa[j21].X = 0;
											vo_pa[j21].Y = 64 / 8;
											j21++;
											for (var j = 140; j >= 90; j -= 5)
											{
												vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((j * math_pi) / 180) * 144) / 8;
												vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((j * math_pi) / 180) * 144) / 8;
												j21++;
											}

											vo_pa[j21].X = 120 / 8;
											vo_pa[j21].Y = 64 / 8;
											e.Graphics.FillPolygon(brush2, vo_pa);
											j21 = 0;
											for (var k2 = 90; k2 >= 40; k2 -= 5)
											{
												vo_pa[j21].X = (float)Math.Floor(120 + Math.Cos((k2 * math_pi) / 180) * 144) / 8;
												vo_pa[j21].Y = (float)Math.Floor(144 - Math.Sin((k2 * math_pi) / 180) * 144) / 8;
												j21++;
											}

											vo_pa[j21].X = 240 / 8;
											vo_pa[j21].Y = 64 / 8;
											j21++;
											vo_pa[j21].X = 120 / 8;
											vo_pa[j21].Y = 64 / 8;
											e.Graphics.FillPolygon(brush2, vo_pa);
											brush2.Dispose();
										}
										break;
									case "ファイヤーバー":
									case "スウィングファイヤーバー":
										{
											e.Graphics.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);
											e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawChipOrig,
												new Rectangle(0, 0, chipsize.Width / 2, chipsize.Height / 2),
												new Rectangle(cschip.pattern, new Size(chipsize.Width / 2, chipsize.Height / 2)), GraphicsUnit.Pixel);
											int v = 225;
											e.Graphics.TranslateTransform(chipsize.Width / 2 + 1, chipsize.Width / 2 + 1);
											rad = ((v + 90) * Math.PI) / 180;
											const double d = 0.017453292519943295;
											vo_pa = new PointF[4];
											vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 25) + Math.Cos(rad) * 4);
											vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) + Math.Sin(rad) * 4);
											vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 25) - Math.Cos(rad) * 4);
											vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 25) - Math.Sin(rad) * 4);
											vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 42) - Math.Cos(rad) * 4);
											vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 42) - Math.Sin(rad) * 4);
											vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 42) + Math.Cos(rad) * 4);
											vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 42) + Math.Sin(rad) * 4);
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
											e.Graphics.FillPolygon(brush2, vo_pa);
											// 内側の色を描画
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
											vo_pa[0].X = (float)(Math.Cos(v * d) * 28 + Math.Cos(rad) * 2);
											vo_pa[0].Y = (float)(Math.Sin(v * d) * 28 + Math.Sin(rad) * 2);
											vo_pa[1].X = (float)(Math.Cos(v * d) * 28 - Math.Cos(rad) * 2);
											vo_pa[1].Y = (float)(Math.Sin(v * d) * 28 - Math.Sin(rad) * 2);
											vo_pa[2].X = (float)(Math.Cos(v * d) * 40 - Math.Cos(rad) * 2);
											vo_pa[2].Y = (float)(Math.Sin(v * d) * 40 - Math.Sin(rad) * 2);
											vo_pa[3].X = (float)(Math.Cos(v * d) * 40 + Math.Cos(rad) * 2);
											vo_pa[3].Y = (float)(Math.Sin(v * d) * 40 + Math.Sin(rad) * 2);
											e.Graphics.FillPolygon(brush2, vo_pa);
											brush2.Dispose();
										}
										break;
									case "人口太陽":
										{
											e.Graphics.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);

											int v = default, n = default;
											const double d = 0.017453292519943295;
											vo_pa = new PointF[4];
											if (cschip.description.Contains("棒５本")) n = 5;
											else n = 3;
											if (cschip.description.Contains("左")) v = 360 - 2;
											else v = 2;

											for (var ii = 0; ii < n; ii++)
											{
												v += 360 / n;
												rad = ((v + 90) * Math.PI) / 180;
												brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
												vo_pa[0].X = (float)(Math.Floor(Math.Cos(v * d) * 4) + Math.Cos(rad) * 3);
												vo_pa[0].Y = (float)(Math.Floor(Math.Sin(v * d) * 4) + Math.Sin(rad) * 3);
												vo_pa[1].X = (float)(Math.Floor(Math.Cos(v * d) * 4) - Math.Cos(rad) * 3);
												vo_pa[1].Y = (float)(Math.Floor(Math.Sin(v * d) * 4) - Math.Sin(rad) * 3);
												vo_pa[2].X = (float)(Math.Floor(Math.Cos(v * d) * 15) - Math.Cos(rad) * 3);
												vo_pa[2].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) - Math.Sin(rad) * 3);
												vo_pa[3].X = (float)(Math.Floor(Math.Cos(v * d) * 15) + Math.Cos(rad) * 3);
												vo_pa[3].Y = (float)(Math.Floor(Math.Sin(v * d) * 15) + Math.Sin(rad) * 3);
												e.Graphics.FillPolygon(brush2, vo_pa);

												// 内側の色を描画
												brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
												vo_pa[0].X = (float)(Math.Cos(v * d) * 6 + Math.Cos(rad) * 1);
												vo_pa[0].Y = (float)(Math.Sin(v * d) * 6 + Math.Sin(rad) * 1);
												vo_pa[1].X = (float)(Math.Cos(v * d) * 6 - Math.Cos(rad) * 1);
												vo_pa[1].Y = (float)(Math.Sin(v * d) * 6 - Math.Sin(rad) * 1);
												vo_pa[2].X = (float)(Math.Cos(v * d) * 13 - Math.Cos(rad) * 1);
												vo_pa[2].Y = (float)(Math.Sin(v * d) * 13 - Math.Sin(rad) * 1);
												vo_pa[3].X = (float)(Math.Cos(v * d) * 13 + Math.Cos(rad) * 1);
												vo_pa[3].Y = (float)(Math.Sin(v * d) * 13 + Math.Sin(rad) * 1);
												e.Graphics.FillPolygon(brush2, vo_pa);
											}
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
											e.Graphics.FillEllipse(brush2, -6, -6, 12, 12);
											brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
											e.Graphics.FillEllipse(brush2, -2, -2, 4, 4);

											brush2.Dispose();
										}
										break;
									case "ファイヤーリング":
										{
											e.Graphics.TranslateTransform(chipsize.Width / 2, chipsize.Width / 2);

											int v = default, n = default;
											if (cschip.description.Contains("2本")) n = 2;
											else n = 3;
											if (cschip.description.Contains("左回り"))
											{
												if (cschip.description.Contains("高速")) v = -4 + 360;
												else v = -2 + 360;
											}
											else
											{
												if (cschip.description.Contains("高速")) v = 4;
												else v = 2;
											}

											brush2 = default;

											for (var ii = 0; ii < n; ii++)
											{
												var k6 = 0;
												if (cschip.description.Contains("2本"))
												{
													vo_pa = new PointF[26];
													for (var i4 = 0; i4 >= -120; i4 -= 10)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
														vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
														k6++;
													}

													for (var j4 = -120; j4 <= 0; j4 += 10)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
														vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
														k6++;
													}
												}
												else
												{
													vo_pa = new PointF[12];
													for (var i4 = 0; i4 >= -50; i4 -= 10)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
														vo_pa[k6].Y = (float)(Math.Sin(((v + i4) * Math.PI) / 180) * chipsize.Width / 2);
														k6++;
													}

													for (var j4 = -50; j4 <= 0; j4 += 10)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
														vo_pa[k6].Y = (float)(Math.Sin(((v + j4) * Math.PI) / 180) * chipsize.Width / 2 * 0.3);
														k6++;
													}
												}

												brush2 = new SolidBrush(Global.cpd.project.Config.Firebar1);
												e.Graphics.FillPolygon(brush2, vo_pa);

												// 内側の色を描画
												k6 = 0;
												if (cschip.description.Contains("2本"))
												{
													vo_pa = new PointF[24];
													for (var k4 = -5; k4 >= -115; k4 -= 10)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
														vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
														k6++;
													}

													for (var l4 = -115; l4 <= -5; l4 += 10)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
														vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
														k6++;
													}
												}
												else
												{
													for (var k4 = -5; k4 >= -45; k4 -= 8)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
														vo_pa[k6].Y = (float)(Math.Sin(((v + k4) * Math.PI) / 180) * chipsize.Width / 2 * 0.925);
														k6++;
													}

													for (var l4 = -45; l4 <= -5; l4 += 8)
													{
														vo_pa[k6].X = (float)(Math.Cos(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
														vo_pa[k6].Y = (float)(Math.Sin(((v + l4) * Math.PI) / 180) * chipsize.Width / 2 * 0.5);
														k6++;
													}
												}

												brush2 = new SolidBrush(Global.cpd.project.Config.Firebar2);
												e.Graphics.FillPolygon(brush2, vo_pa);

												v += 360 / n;
											}

											brush2.Dispose();
										}
										break;
									default:
										e.Graphics.TranslateTransform(chipsize.Width / 2, chipsize.Height / 2);
										e.Graphics.RotateTransform(cschip.rotate);
										// 水の半透明処理
										if (Global.state.ChipRegister.ContainsKey("water_clear_switch") && bool.Parse(Global.state.ChipRegister["water_clear_switch"]) == false && Global.cpd.Mapchip[e.Index].character == "4" && Global.state.ChipRegister.ContainsKey("water_clear_level"))
										{
											float water_clear_level = float.Parse(Global.state.ChipRegister["water_clear_level"]);
											var colorMatrix = new ColorMatrix
											{
												Matrix00 = 1f,
												Matrix11 = 1f,
												Matrix22 = 1f,
												Matrix33 = water_clear_level / 255f,
												Matrix44 = 1f
											};
											using var imageAttributes = new ImageAttributes();
											imageAttributes.SetColorMatrix(colorMatrix);
											e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(new Point(-chipsize.Width / 2, -chipsize.Height / 2), chipsize), cschip.pattern.X, cschip.pattern.Y, chipsize.Width, chipsize.Height, GraphicsUnit.Pixel, imageAttributes);
										}
										else e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(new Point(-chipsize.Width / 2, -chipsize.Height / 2), chipsize), new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
										break;
								}
								width = chipsize.Width;
							}
							else
							{
								if (Global.state.EditingForeground && Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 && Global.cpd.Mapchip[e.Index].character == "Z")
								{
									e.Graphics.DrawImage(Global.MainWnd.MainDesigner.DrawOribossOrig, 0, 0);
									width = Global.MainWnd.MainDesigner.DrawOribossOrig.Width;
								}
								else
								{
									int rotate_o = default;
									if (Math.Abs(cschip.rotate) % 180 == 90 && cschip.size.Width > cschip.size.Height)
										{
											rotate_o = (cschip.size.Width - cschip.size.Height) / (cschip.size.Width / cschip.size.Height) * Math.Sign(cschip.rotate);
											width = cschip.size.Height;
										}
									else
										{
											width = cschip.size.Width;
										}
									e.Graphics.TranslateTransform(cschip.size.Width / 2, cschip.size.Height / 2);
									if (Math.Abs(cschip.rotate) % 90 == 0) e.Graphics.RotateTransform(cschip.rotate);
									e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig,
										new Rectangle(new Point(-cschip.size.Width / 2 + rotate_o, -cschip.size.Height / 2 + rotate_o), cschip.size),
										new Rectangle(cschip.pattern, cschip.size), GraphicsUnit.Pixel);
								}
							}

							e.Graphics.Restore(transState);
						}
						else
						{
							cschip = Global.cpd.Layerchip[e.Index].GetCSChip();
							if (cschip.size == default(Size))
							{
								e.Graphics.DrawImage(this.MainDesigner.DrawLayerOrig, new Rectangle(e.Bounds.Location, chipsize), new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
								width = chipsize.Width;
							}
							else
							{
								e.Graphics.DrawImage(this.MainDesigner.DrawChipOrig, new Rectangle(e.Bounds.Location, cschip.size), new Rectangle(cschip.pattern, cschip.size), GraphicsUnit.Pixel);
								width = cschip.size.Width;
							}
						}
						break;
				}
				e.Graphics.PixelOffsetMode = default;
				e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(), e.Font, brush, new Rectangle(e.Bounds.X + width, e.Bounds.Y, e.Bounds.Width - width, e.Bounds.Height));

				if (!this.ChipList.Enabled) e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(160, Color.White)), e.Bounds); // 無効時に白くする
			}
			catch
			{
			}
			e.DrawFocusRectangle();
		}

		private void ChipList_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			int chip_hight = Global.cpd.runtime.Definitions.ChipSize.Height;

			switch (this.DrawType.SelectedIndex)
			{
			case 0: // サムネイル
			case 1: // テキスト
				e.ItemHeight = this.ChipList.ItemHeight;
				return;
			case 2: // チップ
				int height;
				if (Global.state.MapEditMode)
				{
					if (Global.cpd.Worldchip[e.Index].GetCSChip().size.Height == 0)
					{
						height = chip_hight;
					}
					else
					{
						height = Global.cpd.Worldchip[e.Index].GetCSChip().size.Height;
					}
					e.ItemHeight = height;
					return;
				}
				if (!Global.cpd.UseLayer || Global.state.EditingForeground)
				{
					ChipData cschip = Global.cpd.Mapchip[e.Index].GetCSChip();
						if (cschip.size.Height == 0)
						{
							height = chip_hight;
						}
						else
						{
							if (Math.Abs(cschip.rotate) % 180 == 90 && cschip.size.Width > cschip.size.Height)
							{
								height = cschip.size.Width;
							}
							else
							{
								if (Global.state.ChipRegister.ContainsKey("oriboss_v") && int.Parse(Global.state.ChipRegister["oriboss_v"]) == 3 && Global.cpd.Mapchip[e.Index].character == "Z")
								{
									height = Global.MainWnd.MainDesigner.DrawOribossOrig.Height;
								}
								else height = cschip.size.Height;
							}
						}
					e.ItemHeight = height;
					return;
				}
				if (Global.cpd.Layerchip[e.Index].GetCSChip().size.Height == 0)
				{
					height = chip_hight;
				}
				else
				{
					height = Global.cpd.Layerchip[e.Index].GetCSChip().size.Height;
				}
				e.ItemHeight = height;
				return;
			}
		}

		private void TMUndo_Click(object sender, EventArgs e)
		{
			this.MainEditor.Undo();
		}

		private void TMRedo_Click(object sender, EventArgs e)
		{
			this.MainEditor.Redo();
		}

		private void TextCut_Click(object sender, EventArgs e)
		{
			this.MainEditor.StageTextEditor.Cut();
		}

		private void TextCopy_Click(object sender, EventArgs e)
		{
			this.MainEditor.StageTextEditor.Copy();
		}

		private void TextPaste_Click(object sender, EventArgs e)
		{
			this.MainEditor.StageTextEditor.Paste();
		}

		private void ProjTestRun(object sender, EventArgs e)
		{
			this.EditTab.SelectedIndex = 2;
		}

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

		private void MainDesigner_Enter(object sender, EventArgs e)
		{
			if (this.OldET != GUIDesigner.EditTool.Cursor && this.MainDesigner.CurrentTool == GUIDesigner.EditTool.Cursor)
			{
				this.MainDesigner.CurrentTool = this.OldET;
				this.OldET = GUIDesigner.EditTool.Cursor;
			}
		}

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

		private void LayerState(bool enabled)
		{
			this.ViewUnactiveLayer.Enabled = enabled;
			this.LayerMenu.Visible = enabled;
			this.StageLayer.Visible = enabled;
			this.MainEditor.StageLayer.Visible = enabled;
		}

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

		private void MSave_Click(object sender, EventArgs e)
		{
			Global.cpd.project.SaveXML(Global.cpd.filename);
			Global.state.EditFlag = false;
			this.UpdateStatus("保存しました");
		}

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

		private void MExit_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing && Global.state.EditFlag && MessageBox.Show("編集データが保存されていません。" + Environment.NewLine + "終了してもよろしいですか？", Global.definition.AppName + "の終了", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
			{
				e.Cancel = true;
			}
		}

		private void ViewUnactiveLayer_Click(object sender, EventArgs e)
		{
			Global.state.DrawUnactiveLayer = !Global.state.DrawUnactiveLayer;
			this.ViewUnactiveLayer.Checked = Global.state.DrawUnactiveLayer;
			this.MDUnactiveLayer.Checked = Global.state.DrawUnactiveLayer;
			if (!this.ViewUnactiveLayer.Checked)
			{
				if (Global.state.TransparentUnactiveLayer)
				{
					this.TransparentUnactiveLayer.Enabled = false;
					if (Global.state.EditingForeground)
					{
						this.MainDesigner.UpdateBackgroundBuffer();
					}
					else
					{
						this.MainDesigner.UpdateForegroundBuffer();
					}
				}
				this.MTUnactiveLayer.Enabled = false;
			}
			else
			{
				this.MainDesigner.InitTransparent();
				this.TransparentUnactiveLayer.Enabled = true;
				this.MTUnactiveLayer.Enabled = true;
			}
			this.MainDesigner.Refresh();
		}

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

		private void UpdateStageSelector()
		{
			this.MEditStage2.Enabled = (Global.cpd.project.Config.StageNum >= 2) || Global.cpd.project.Config.UseWorldmap;
			this.MEditStage3.Enabled = (Global.cpd.project.Config.StageNum >= 3) || Global.cpd.project.Config.UseWorldmap;
			this.MEditStage4.Enabled = (Global.cpd.project.Config.StageNum >= 4) || Global.cpd.project.Config.UseWorldmap;
			this.MEditMap.Enabled = Global.cpd.project.Config.UseWorldmap;
		}

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

		private void DrawType_Resize(object sender, EventArgs e)
		{
			this.DrawType.Refresh();
		}

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

		private void MVersion_Click(object sender, EventArgs e)
		{
			using (VersionInfo versionInfo = new VersionInfo())
			{
				versionInfo.ShowDialog();
			}
		}

		private void SideTab_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.SideTab.SelectedIndex == 1)
			{
				this.MasaoConfigList.Prepare();
			}
		}

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

		public void ProjectConfig_Click(object sender, EventArgs e)
		{
			using (ProjectConfig projectConfig = new ProjectConfig())
			{
				projectConfig.ShowDialog();
			}
			this.UpdateStageSelector();
		}

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
			if (Global.state.TransparentUnactiveLayer)
			{
				this.MainDesigner.InitTransparent();
			}
			this.MainDesigner.Refresh();
			Global.state.EditFlag = true;
			this.UpdateStatus("完了");
		}

		private void MEditStage1_Click(object sender, EventArgs e)
		{
			this.StageSelectionChange(0);
		}

		private void MEditStage2_Click(object sender, EventArgs e)
		{
			this.StageSelectionChange(1);
		}

		private void MEditStage3_Click(object sender, EventArgs e)
		{
			this.StageSelectionChange(2);
		}

		private void MEditStage4_Click(object sender, EventArgs e)
		{
			this.StageSelectionChange(3);
		}

		private void MEditMap_Click(object sender, EventArgs e)
		{
			this.StageSelectionChange(4);
		}

		private void AllTestrun(object sender, EventArgs e)
		{
			Global.state.TestrunAll = true;
			this.EditTab.SelectedIndex = 2;
		}

		private void MReloadImage_Click(object sender, EventArgs e)
		{
			this.UpdateStatus("更新しています...");
			Global.MainWnd.MainDesigner.PrepareImages();
			Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
			Global.MainWnd.MainDesigner.UpdateBackgroundBuffer();
			this.MainDesigner.Refresh();
			this.UpdateStatus("完了");
		}

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

		private void ovw_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.ovw.Dispose();
			this.ovw = null;
			this.ShowOverView.Checked = false;
		}

		public OverViewWindow ovw;

		private bool StageConvert = true;

		private GUIDesigner.EditTool OldET;

		private bool Restartupping;

		public delegate void MainDesignerScrollInvoke();

		private delegate void CRID();
	}
}
