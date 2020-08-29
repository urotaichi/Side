using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MasaoPlus.Dialogs;

namespace MasaoPlus.Controls
{
	// Token: 0x02000008 RID: 8
	public class ConfigList : UserControl
	{
		// Token: 0x06000042 RID: 66 RVA: 0x000023D6 File Offset: 0x000005D6
		public ConfigList()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00009D48 File Offset: 0x00007F48
		public void Prepare()
		{
			this.ConfigSelector.Items.Clear();
			this.ConfigSelector.Items.Add("全部");
			foreach (string text in Global.cpd.project.Config.Categories)
			{
				if (text != null)
				{
					this.ConfigSelector.Items.Add(text);
				}
			}
			this.ConfigSelector.SelectedIndex = 0;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000023EF File Offset: 0x000005EF
		public void Reload()
		{
			this.ConfigSelector_SelectedIndexChanged(this, new EventArgs());
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00009DEC File Offset: 0x00007FEC

		// 表示を変える
		private void ConfigSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.ConfigSelector.Items.Count < 1)
			{
				return;
			}
			this.OrigIdx.Clear();
			this.ConfView.Rows.Clear();
			for (int i = 0; i < Global.cpd.project.Config.Configurations.Length; i++)
			{
				ConfigParam configParam = Global.cpd.project.Config.Configurations[i];
				if ((this.ConfigSelector.SelectedIndex == 0 || configParam.Category == Global.cpd.project.Config.Categories[this.ConfigSelector.SelectedIndex - 1]) && !(configParam.Relation == "STAGENUM") && !(configParam.Relation == "STAGESTART") && !(configParam.Relation == "STAGESELECT"))
				{
					if (configParam.Name == "width") this.width_index = i;
					else if (configParam.Name == "height") this.height_index = i;

					this.OrigIdx.Add(i);
					this.ConfView.Rows.Add(new string[]
					{
						configParam.Description,
						configParam.Value
					});
					switch (configParam.Type)
					{
					case ConfigParam.Types.b:
					case ConfigParam.Types.b0:
					{
						DataGridViewCheckBoxCell dataGridViewCheckBoxCell = new DataGridViewCheckBoxCell();
						dataGridViewCheckBoxCell.TrueValue = "true";
						dataGridViewCheckBoxCell.FalseValue = "false";
						bool flag;
						if (bool.TryParse(configParam.Value, out flag))
						{
							dataGridViewCheckBoxCell.Value = flag.ToString();
						}
						else
						{
							dataGridViewCheckBoxCell.Value = false;
						}
						this.ConfView[1, this.OrigIdx.Count - 1] = dataGridViewCheckBoxCell;
						break;
					}
					case ConfigParam.Types.b2:
					{
						DataGridViewCheckBoxCell dataGridViewCheckBoxCell = new DataGridViewCheckBoxCell();
						dataGridViewCheckBoxCell.TrueValue = "false";
						dataGridViewCheckBoxCell.FalseValue = "true";
						bool flag;
						if (bool.TryParse(configParam.Value, out flag))
						{
							dataGridViewCheckBoxCell.Value = (!flag).ToString();
						}
						else
						{
							dataGridViewCheckBoxCell.Value = false;
						}
						this.ConfView[1, this.OrigIdx.Count - 1] = dataGridViewCheckBoxCell;
						break;
					}
					case ConfigParam.Types.i:
					{
						DataGridViewNumericUpdownCell dataGridViewNumericUpdownCell = new DataGridViewNumericUpdownCell();
						dataGridViewNumericUpdownCell.Value = configParam.Value;
						this.ConfView[1, this.OrigIdx.Count - 1] = dataGridViewNumericUpdownCell;
						break;
					}
					case ConfigParam.Types.t:
						if (Global.config.localSystem.UsePropExTextEditor)
						{
							DataGridViewButtonCell dataGridViewButtonCell = new DataGridViewButtonCell();
							dataGridViewButtonCell.Value = configParam.Value;
							dataGridViewButtonCell.FlatStyle = FlatStyle.Popup;
							if (Global.config.localSystem.WrapPropText)
							{
								dataGridViewButtonCell.Style.WrapMode = DataGridViewTriState.True;
							}
							this.ConfView[1, this.OrigIdx.Count - 1] = dataGridViewButtonCell;
						}
						else
						{
							this.ConfView[1, this.OrigIdx.Count - 1].Value = configParam.Value;
							this.ConfView[1, this.OrigIdx.Count - 1].Style.WrapMode = DataGridViewTriState.True;
							this.ConfView[1, this.OrigIdx.Count - 1].Style.Alignment = DataGridViewContentAlignment.TopLeft;
							this.ConfView[1, this.OrigIdx.Count - 1].ToolTipText = "Shift+Enterで改行できます。";
						}
						break;
					case ConfigParam.Types.f:
					case ConfigParam.Types.f_i:
					case ConfigParam.Types.f_a:
					{
						DataGridViewButtonCell dataGridViewButtonCell2 = new DataGridViewButtonCell();
						dataGridViewButtonCell2.Value = configParam.Value + "...";
						dataGridViewButtonCell2.FlatStyle = FlatStyle.Popup;
						this.ConfView[1, this.OrigIdx.Count - 1] = dataGridViewButtonCell2;
						break;
					}
					case ConfigParam.Types.l:
					{
						DataGridViewComboBoxCell dataGridViewComboBoxCell = new DataGridViewComboBoxCell();
						dataGridViewComboBoxCell.Items.AddRange(configParam.ListItems);
						dataGridViewComboBoxCell.FlatStyle = FlatStyle.Popup;
						int num;
						if (int.TryParse(configParam.Value, out num) && num <= dataGridViewComboBoxCell.Items.Count && num > 0)
						{
							dataGridViewComboBoxCell.Value = configParam.ListItems[num - 1];
						}
						else
						{
							dataGridViewComboBoxCell.Value = configParam.ListItems[0];
						}
						this.ConfView[1, this.OrigIdx.Count - 1] = dataGridViewComboBoxCell;
						break;
					}
					case ConfigParam.Types.l_a:
					{
						DataGridViewComboBoxCell dataGridViewComboBoxCell = new DataGridViewComboBoxCell();
						dataGridViewComboBoxCell.Items.AddRange(configParam.ListItems);
						dataGridViewComboBoxCell.FlatStyle = FlatStyle.Popup;
						int num, MaxAthleticNumber = Global.cpd.runtime.Definitions.MaxAthleticNumber;
						if (int.TryParse(configParam.Value, out num) && ( num > 0 && num <= MaxAthleticNumber || num >= 1001 && num <= 1249) )
						{
								if (num <= MaxAthleticNumber)
								{
									dataGridViewComboBoxCell.Value = configParam.ListItems[num - 1];
								}
								else
								{
									dataGridViewComboBoxCell.Value = configParam.ListItems[num  - (1001 - MaxAthleticNumber)];
								}
						}
						else
						{
							dataGridViewComboBoxCell.Value = configParam.ListItems[0];
						}
						this.ConfView[1, this.OrigIdx.Count - 1] = dataGridViewComboBoxCell;
						break;
					}
					case ConfigParam.Types.c:
					{
						DataGridViewButtonCell dataGridViewButtonCell3 = new DataGridViewButtonCell();
						dataGridViewButtonCell3.Value = configParam.Value;
						Colors colors = new Colors(configParam.Value);
						dataGridViewButtonCell3.Style.BackColor = colors.c;
						dataGridViewButtonCell3.Style.SelectionBackColor = colors.c;
						dataGridViewButtonCell3.FlatStyle = FlatStyle.Popup;
						this.ConfView[1, this.OrigIdx.Count - 1] = dataGridViewButtonCell3;
						break;
					}
					}
				}
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000023FD File Offset: 0x000005FD
		private void ConfigSelector_Resize(object sender, EventArgs e)
		{
			this.ConfigSelector.Refresh();
		}

		// Token: 0x06000047 RID: 71 RVA: 0x0000A22C File Offset: 0x0000842C
		private void ConfView_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != 1)
			{
				return;
			}
			if (e.RowIndex >= this.OrigIdx.Count || e.RowIndex < 0)
			{
				return;
			}
			int num = this.OrigIdx[e.RowIndex];
			ConfigParam configParam = Global.cpd.project.Config.Configurations[num];
			switch (configParam.Type)
			{
			case ConfigParam.Types.t:
				if (Global.config.localSystem.UsePropExTextEditor)
				{
					using (PropertyTextInputDialog propertyTextInputDialog = new PropertyTextInputDialog())
					{
						propertyTextInputDialog.InputStr = configParam.Value;
						if (propertyTextInputDialog.ShowDialog() != DialogResult.Cancel)
						{
							this.ConfView[e.ColumnIndex, e.RowIndex].Value = propertyTextInputDialog.InputStr;
							Global.cpd.project.Config.Configurations[num].Value = propertyTextInputDialog.InputStr;
						}
						break;
					}
				}
				return;
			case ConfigParam.Types.f:
			case ConfigParam.Types.f_i:
			case ConfigParam.Types.f_a:
				using (OpenFileDialog openFileDialog = new OpenFileDialog())
				{
					openFileDialog.InitialDirectory = Global.cpd.where;
					openFileDialog.FileName = configParam.Value;
					if(configParam.Type == ConfigParam.Types.f_i)
						openFileDialog.Filter = "画像(*.gif;*.png;*.jpg;*.webp;*.bmp)|*.gif;*.png;*.jpg;*.webp;*.bmp|全てのファイル (*.*)|*.*";
					else if(configParam.Type == ConfigParam.Types.f_a)
						openFileDialog.Filter = "音声(*.wav;*.mp3;*.ogg)|*.wav;*.mp3;*.ogg|全てのファイル (*.*)|*.*";
					if (openFileDialog.ShowDialog() != DialogResult.OK)
					{
						return;
					}
					string fileName = openFileDialog.FileName;
					string text = Path.Combine(Global.cpd.where, Path.GetFileName(openFileDialog.FileName));
					if (Path.GetDirectoryName(openFileDialog.FileName) != Global.cpd.where)
					{
						if (MessageBox.Show(string.Concat(new string[]
						{
							"ファイルはプロジェクトディレクトリに含まれていません。",
							Environment.NewLine,
							"プロジェクトディレクトリへコピーします。",
							Environment.NewLine,
							"また、同じ名前のファイルがある場合は上書きされます。",
							Environment.NewLine,
							"よろしいですか？"
						}), "ファイルの追加", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
						{
							return;
						}
						try
						{
							File.Copy(fileName, text, true);
						}
						catch (IOException)
						{
							MessageBox.Show("ファイルをコピーできませんでした。" + Environment.NewLine + "ファイルのコピー先を再指定してください。", "コピー失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							bool flag = false;
							while (!flag)
							{
								using (SaveFileDialog saveFileDialog = new SaveFileDialog())
								{
									saveFileDialog.InitialDirectory = Path.GetDirectoryName(text);
									saveFileDialog.FileName = Path.GetFileName(text);
									saveFileDialog.Filter = "保存ファイル|*" + Path.GetExtension(fileName);
									saveFileDialog.AddExtension = true;
									saveFileDialog.DefaultExt = Path.GetExtension(fileName);
									if (saveFileDialog.ShowDialog() != DialogResult.OK)
									{
										return;
									}
									if (File.Exists(saveFileDialog.FileName))
									{
										MessageBox.Show("上書きはできません。" + Environment.NewLine + "元のファイルを消すか、別のファイルを指定してください。", "選択エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
									}
									else if (Path.GetDirectoryName(saveFileDialog.FileName) != Global.cpd.where)
									{
										MessageBox.Show("プロジェクトディレクトリ内に保存してください。", "選択エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
									}
									else
									{
										text = saveFileDialog.FileName;
										flag = true;
									}
								}
							}
						}
					}
					this.ConfView[e.ColumnIndex, e.RowIndex].Value = Path.GetFileName(text) + "...";
					Global.cpd.project.Config.Configurations[num].Value = Path.GetFileName(text);
					string relation;
					if (Global.cpd.project.Config.Configurations[num].Relation != null && Global.cpd.project.Config.Configurations[num].Relation != "" && (relation = Global.cpd.project.Config.Configurations[num].Relation) != null)
					{// バグ　編集していない方のレイヤーを半透明表示にしている際にそのレイヤーの画像を変更すると半透明表示が解除される
						if (!(relation == "PATTERN"))
						{
							if (relation == "LAYERCHIP")
							{
								Global.MainWnd.MainDesigner.PrepareImages();
								Global.MainWnd.MainDesigner.UpdateBackgroundBuffer();
								Global.MainWnd.MainDesigner.Refresh();
							}
						}
						else
						{
							Global.MainWnd.MainDesigner.PrepareImages();
							Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
							Global.MainWnd.MainDesigner.Refresh();
						}
					}
					if(Global.cpd.project.Config.Configurations[num].Name == "filename_oriboss_left1")
					{
						Global.MainWnd.MainDesigner.PrepareImages();
						Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
						Global.MainWnd.MainDesigner.Refresh();
					}
					if(Regex.IsMatch(Global.cpd.project.Config.Configurations[num].Name, "^(filename_haikei|filename_second_haikei|filename_chizu)"))
					{
						Global.MainWnd.MainDesigner.PrepareImages();
						Global.MainWnd.MainDesigner.UpdateForegroundBuffer();
						Global.MainWnd.MainDesigner.UpdateBackgroundBuffer();
						Global.MainWnd.MainDesigner.Refresh();
					}
					break;
				}
			case ConfigParam.Types.l:
			case ConfigParam.Types.l_a:
				return;
			case ConfigParam.Types.c:
				using (ColorDialog colorDialog = new ColorDialog())
				{
					Colors colors = new Colors(configParam.Value);
					colorDialog.Color = colors.c;
					if (colorDialog.ShowDialog() == DialogResult.OK)
					{
						colors.c = colorDialog.Color;
						this.ConfView[e.ColumnIndex, e.RowIndex].Value = colors.ToString();
						this.ConfView[e.ColumnIndex, e.RowIndex].Style.BackColor = colors.c;
						Global.cpd.project.Config.Configurations[num].Value = colors.ToString();
						if (Global.cpd.project.Config.Configurations[num].Relation == "BACKGROUND" && Global.state.EdittingStage == 0)
						{
							Global.state.Background = colors.c;
							Global.MainWnd.MainDesigner.Refresh();
						}
						else if (Global.cpd.project.Config.Configurations[num].Relation == "BACKGROUND2" && Global.state.EdittingStage == 1)
						{
							Global.state.Background = colors.c;
							Global.MainWnd.MainDesigner.Refresh();
						}
						else if (Global.cpd.project.Config.Configurations[num].Relation == "BACKGROUND3" && Global.state.EdittingStage == 2)
						{
							Global.state.Background = colors.c;
							Global.MainWnd.MainDesigner.Refresh();
						}
						else if (Global.cpd.project.Config.Configurations[num].Relation == "BACKGROUND4" && Global.state.EdittingStage == 3)
						{
							Global.state.Background = colors.c;
							Global.MainWnd.MainDesigner.Refresh();
						}
						else if (Global.cpd.project.Config.Configurations[num].Relation == "BACKGROUNDM" && Global.state.EdittingStage == 4)
						{
							Global.state.Background = colors.c;
							Global.MainWnd.MainDesigner.Refresh();
						}
					}
					break;
				}
			default:
				return;
			}
			Global.state.EditFlag = true;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x0000A9A8 File Offset: 0x00008BA8
		private void ConfView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			if (e.Control is DataGridViewTextBoxEditingControl)
			{
				DataGridViewTextBoxEditingControl dataGridViewTextBoxEditingControl = (DataGridViewTextBoxEditingControl)e.Control;
				dataGridViewTextBoxEditingControl.PreviewKeyDown += this.ct_PreviewKeyDown;
				dataGridViewTextBoxEditingControl.ScrollBars = ScrollBars.Both;
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000A9E8 File Offset: 0x00008BE8
		private void ct_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			Keys keyCode = e.KeyCode;
			if (keyCode != Keys.Return)
			{
				return;
			}
			e.IsInputKey = true;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000AA0C File Offset: 0x00008C0C
		private void ConfView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (this.ConfView.CurrentCellAddress.X == 1 && this.ConfView.IsCurrentCellDirty)
			{
				this.ConfView.CommitEdit(DataGridViewDataErrorContexts.Commit);
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x0000AA50 File Offset: 0x00008C50
		private void ConfView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != 1)
			{
				return;
			}
			if (e.RowIndex >= this.OrigIdx.Count || e.RowIndex < 0)
			{
				return;
			}
			int num = this.OrigIdx[e.RowIndex];
			ConfigParam configParam = Global.cpd.project.Config.Configurations[num];
			switch (configParam.Type)
			{
			case ConfigParam.Types.b:
			case ConfigParam.Types.b2:
			case ConfigParam.Types.b0:
					if (configParam.Value == this.ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
				{
					return;
				}
				Global.cpd.project.Config.Configurations[num].Value = this.ConfView[e.ColumnIndex, e.RowIndex].Value.ToString();
				break;
			case ConfigParam.Types.s:
				if (this.ConfView[e.ColumnIndex, e.RowIndex].Value == null)
				{
					this.ConfView[e.ColumnIndex, e.RowIndex].Value = "";
				}
				if (configParam.Value == this.ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
				{
					return;
				}
				Global.cpd.project.Config.Configurations[num].Value = this.ConfView[e.ColumnIndex, e.RowIndex].Value.ToString();
				break;
			case ConfigParam.Types.i:
			{
				int num2;
				if (this.ConfView[e.ColumnIndex, e.RowIndex].Value == null)
				{
					this.ConfView[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
					num2 = 0;
				}
				if (!int.TryParse(this.ConfView[e.ColumnIndex, e.RowIndex].Value.ToString(), out num2))
				{
					MessageBox.Show("有効な設定値ではありません。", "設定エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					if (configParam.Value.ToString() == this.ConfView[e.ColumnIndex, e.RowIndex].Value.ToString())
					{
						return;
					}
					this.ConfView[e.ColumnIndex, e.RowIndex].Value = configParam.Value.ToString();
					return;
				}
				else
				{
					Global.cpd.project.Config.Configurations[num].Value = num2.ToString();
				}
				break;
			}
			case ConfigParam.Types.t:
			{
				if (this.ConfView[e.ColumnIndex, e.RowIndex].Value == null)
				{
					this.ConfView[e.ColumnIndex, e.RowIndex].Value = "";
				}
				string text = this.ConfView[e.ColumnIndex, e.RowIndex].Value.ToString();
				string[] array = text.Split(new string[]
				{
					Environment.NewLine
				}, StringSplitOptions.None);
				if (configParam.Rows > 0)
				{
					List<string> list = new List<string>(array);
					if (list.Count > configParam.Rows)
					{
						MessageBox.Show("行数が最大値を超えています。" + Environment.NewLine + "超えた行は削除されます。", "行の超過", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						list.RemoveRange(configParam.Rows, list.Count - configParam.Rows);
					}
					else if (list.Count < configParam.Rows)
					{
						while (list.Count < configParam.Rows)
						{
							list.Add("0");
						}
					}
					array = list.ToArray();
				}
				text = string.Join(Environment.NewLine, array);
				if (configParam.Value == text)
				{
					return;
				}
				Global.cpd.project.Config.Configurations[num].Value = text;
				this.ConfView[e.ColumnIndex, e.RowIndex].Value = text;
				break;
			}
			case ConfigParam.Types.f:
			case ConfigParam.Types.f_i:
			case ConfigParam.Types.f_a:
				return;
			case ConfigParam.Types.l:
				if (configParam.Value == (((DataGridViewComboBoxCell)this.ConfView[e.ColumnIndex, e.RowIndex]).Items.IndexOf(this.ConfView[e.ColumnIndex, e.RowIndex].Value) + 1).ToString())
				{
					return;
				}
				Global.cpd.project.Config.Configurations[num].Value = (((DataGridViewComboBoxCell)this.ConfView[e.ColumnIndex, e.RowIndex]).Items.IndexOf(this.ConfView[e.ColumnIndex, e.RowIndex].Value) + 1).ToString();
				if (Global.cpd.project.Config.Configurations[num].Name == "mcs_screen_size"){
					if(Global.cpd.project.Config.Configurations[num].Value == "1") {
							Global.cpd.project.Config.Configurations[this.width_index].Value = "640";
							Global.cpd.project.Config.Configurations[this.height_index].Value = "480";
							int tmp_y = Global.cpd.runtime.Definitions.MapSize.y;
							Global.cpd.runtime.Definitions.MapSize.x = 19;
							Global.cpd.runtime.Definitions.MapSize.y = 14;

							// 予め地図画面の配列数を多めに取っておけば↓は不要
							
							Array.Resize(ref Global.cpd.project.MapData, Global.cpd.runtime.Definitions.MapSize.y);
							for (int i = 0; i < Global.cpd.runtime.Definitions.MapSize.y; i++)
                            {
								int k;
								if (i < tmp_y) k = Global.cpd.runtime.Definitions.MapSize.x - Global.cpd.project.MapData[i].Length;
								else k = Global.cpd.runtime.Definitions.MapSize.x;
								for (int j = 0; j < k; j++)
									Global.cpd.project.MapData[i] += ".";
							}
					}
					else if(Global.cpd.project.Config.Configurations[num].Value == "2") {
							Global.cpd.project.Config.Configurations[this.width_index].Value = "512";
							Global.cpd.project.Config.Configurations[this.height_index].Value = "320";
							Global.cpd.runtime.Definitions.MapSize.x = 15;
							Global.cpd.runtime.Definitions.MapSize.y = 9;
						}
				}
				break;
			case ConfigParam.Types.l_a:
			{
				int configParam_num = ((DataGridViewComboBoxCell)this.ConfView[e.ColumnIndex, e.RowIndex]).Items.IndexOf(this.ConfView[e.ColumnIndex, e.RowIndex].Value) + 1;
				int MaxAthleticNumber = Global.cpd.runtime.Definitions.MaxAthleticNumber;
				if (configParam_num <= MaxAthleticNumber && configParam.Value == configParam_num.ToString() || configParam_num > MaxAthleticNumber && configParam.Value == (configParam_num - 1 - MaxAthleticNumber + 1001).ToString())
				{
					return;
				}
				Global.cpd.project.Config.Configurations[num].Value = ((configParam_num <= MaxAthleticNumber)?configParam_num:configParam_num - 1 - MaxAthleticNumber + 1001).ToString();
				break;
			}
			default:
				return;
			}
			if (configParam.ChipRelation != "" && configParam.ChipRelation != null)
			{
				Global.state.ChipRegister[configParam.ChipRelation] = Global.cpd.project.Config.Configurations[num].Value;
				Global.MainWnd.RefreshAll();
			}
			Global.state.EditFlag = true;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x0000240A File Offset: 0x0000060A
		private void ConfView_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 1)
			{
				this.ConfView.BeginEdit(true);
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002422 File Offset: 0x00000622
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x0000AFC0 File Offset: 0x000091C0
		private void InitializeComponent()
		{
			this.ConfigSelector = new ComboBox();
			this.ConfView = new DataGridView();
			this.CNames = new DataGridViewTextBoxColumn();
			this.CValues = new DataGridViewTextBoxColumn();
			((ISupportInitialize)this.ConfView).BeginInit();
			base.SuspendLayout();
			this.ConfigSelector.Dock = DockStyle.Top;
			this.ConfigSelector.DropDownStyle = ComboBoxStyle.DropDownList;
			this.ConfigSelector.FlatStyle = FlatStyle.System;
			this.ConfigSelector.FormattingEnabled = true;
			this.ConfigSelector.Location = new Point(0, 0);
			this.ConfigSelector.Name = "ConfigSelector";
			this.ConfigSelector.Size = new Size(298, 20);
			this.ConfigSelector.TabIndex = 3;
			this.ConfigSelector.Resize += this.ConfigSelector_Resize;
			this.ConfigSelector.SelectedIndexChanged += this.ConfigSelector_SelectedIndexChanged;
			this.ConfView.AllowUserToAddRows = false;
			this.ConfView.AllowUserToDeleteRows = false;
			this.ConfView.AllowUserToResizeRows = false;
			this.ConfView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			this.ConfView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			this.ConfView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.ConfView.Columns.AddRange(new DataGridViewColumn[]
			{
				this.CNames,
				this.CValues
			});
			this.ConfView.Dock = DockStyle.Fill;
			this.ConfView.EditMode = DataGridViewEditMode.EditOnEnter;
			this.ConfView.Location = new Point(0, 20);
			this.ConfView.MultiSelect = false;
			this.ConfView.Name = "ConfView";
			this.ConfView.RowHeadersVisible = false;
			this.ConfView.RowTemplate.Height = 21;
			this.ConfView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.ConfView.Size = new Size(298, 338);
			this.ConfView.TabIndex = 4;
			this.ConfView.CellValueChanged += this.ConfView_CellValueChanged;
			this.ConfView.CellClick += this.ConfView_CellClick;
			this.ConfView.EditingControlShowing += this.ConfView_EditingControlShowing;
			this.ConfView.CurrentCellDirtyStateChanged += this.ConfView_CurrentCellDirtyStateChanged;
			this.ConfView.CellContentClick += this.ConfView_CellContentClick;
			this.CNames.HeaderText = "項目名";
			this.CNames.Name = "CNames";
			this.CNames.ReadOnly = true;
			this.CNames.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.CValues.FillWeight = 50f;
			this.CValues.HeaderText = "値";
			this.CValues.Name = "CValues";
			this.CValues.SortMode = DataGridViewColumnSortMode.NotSortable;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.ConfView);
			base.Controls.Add(this.ConfigSelector);
			base.Name = "ConfigList";
			base.Size = new Size(298, 358);
			((ISupportInitialize)this.ConfView).EndInit();
			base.ResumeLayout(false);
		}

		// Token: 0x0400006E RID: 110
		private List<int> OrigIdx = new List<int>();

		// Token: 0x0400006F RID: 111
		private IContainer components;

		// Token: 0x04000070 RID: 112
		private ComboBox ConfigSelector;

		// Token: 0x04000071 RID: 113
		private DataGridView ConfView;

		// Token: 0x04000072 RID: 114
		private DataGridViewTextBoxColumn CNames;

		// Token: 0x04000073 RID: 115
		private DataGridViewTextBoxColumn CValues;

		private int width_index, height_index;
	}
}
