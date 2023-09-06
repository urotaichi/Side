using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MasaoPlus.Properties;

namespace MasaoPlus.Dialogs
{
    public partial class ProjectConfig : Form
    {
        public ProjectConfig()
        {
            InitializeComponent();
            OutHeader.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
            OutMiddle.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
            OutFooter.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
        }

        private void ProjectConfig_Load(object sender, EventArgs e)
        {
            ProjectName.Text = Global.cpd.project.Name;
            OutDir.Text = Global.cpd.runtime.DefaultConfigurations.OutputDir;
            OutExt.Text = Global.cpd.runtime.DefaultConfigurations.FileExt;
            StageF.Text = Global.cpd.runtime.DefaultConfigurations.StageParam;
            LayerF.Text = Global.cpd.runtime.DefaultConfigurations.LayerParam;
            StageF2.Text = Global.cpd.runtime.DefaultConfigurations.StageParam2;
            LayerF2.Text = Global.cpd.runtime.DefaultConfigurations.LayerParam2;
            StageF3.Text = Global.cpd.runtime.DefaultConfigurations.StageParam3;
            LayerF3.Text = Global.cpd.runtime.DefaultConfigurations.LayerParam3;
            StageF4.Text = Global.cpd.runtime.DefaultConfigurations.StageParam4;
            LayerF4.Text = Global.cpd.runtime.DefaultConfigurations.LayerParam4;
            MapF.Text = Global.cpd.runtime.DefaultConfigurations.MapParam;
            ProjNum.Value = Global.cpd.project.Config.StageNum;
            UseWorldmap.Checked = Global.cpd.project.Config.UseWorldmap;
            if (!Global.cpd.runtime.Definitions.Package.Contains("28"))
            {
                bool state = Global.cpd.project.Use3rdMapData;
                StageF.Enabled = !state;
                LayerF.Enabled = !state;
                StageF2.Enabled = !state;
                LayerF2.Enabled = !state;
                StageF3.Enabled = !state;
                LayerF3.Enabled = !state;
                StageF4.Enabled = !state;
                LayerF4.Enabled = !state;
                Use3rdMapData.Checked = state;
            }

            OutHeader.Text = Subsystem.DecodeBase64(Global.cpd.runtime.DefaultConfigurations.HeaderHTML);
            OutMiddle.Text = Subsystem.DecodeBase64(Global.cpd.runtime.DefaultConfigurations.MiddleHTML);
            OutFooter.Text = Subsystem.DecodeBase64(Global.cpd.runtime.DefaultConfigurations.FooterHTML);

            if (Global.cpd.runtime.DefaultConfigurations.OutputReplace.Length > 0)
            {
                foreach (HTMLReplaceData htmlreplaceData in Global.cpd.runtime.DefaultConfigurations.OutputReplace)
                {
                    OutputReplaceView.Rows.Add(new string[]
                    {
                        htmlreplaceData.Name,
                        htmlreplaceData.Value
                    });
                }
            }
        }
        private void Use3rdMapData_CheckedChanged(object sender = default, EventArgs e = default)
        {
            label18.Enabled = Use3rdMapData.Checked;
            label19.Enabled = Use3rdMapData.Checked;
            label20.Enabled = Use3rdMapData.Checked;
            label21.Enabled = Use3rdMapData.Checked;
            MapSizeWidth.Enabled = Use3rdMapData.Checked;
            MapSizeHeight.Enabled = Use3rdMapData.Checked;
            MapSize2Width.Enabled = Use3rdMapData.Checked;
            MapSize2Height.Enabled = Use3rdMapData.Checked;
            MapSize3Width.Enabled = Use3rdMapData.Checked;
            MapSize3Height.Enabled = Use3rdMapData.Checked;
            MapSize4Width.Enabled = Use3rdMapData.Checked;
            MapSize4Height.Enabled = Use3rdMapData.Checked;
            cross.Enabled = Use3rdMapData.Checked;
            cross2.Enabled = Use3rdMapData.Checked;
            cross3.Enabled = Use3rdMapData.Checked;
            cross4.Enabled = Use3rdMapData.Checked;
        }

        private void Accept_Click(object sender, EventArgs e)
        {
            Global.cpd.project.Name = ProjectName.Text;
            Global.cpd.runtime.DefaultConfigurations.OutputDir = OutDir.Text;
            Global.cpd.runtime.DefaultConfigurations.FileExt = OutExt.Text;
            Global.cpd.runtime.DefaultConfigurations.StageParam = StageF.Text;
            Global.cpd.runtime.DefaultConfigurations.LayerParam = LayerF.Text;
            Global.cpd.runtime.DefaultConfigurations.StageParam2 = StageF2.Text;
            Global.cpd.runtime.DefaultConfigurations.LayerParam2 = LayerF2.Text;
            Global.cpd.runtime.DefaultConfigurations.StageParam3 = StageF3.Text;
            Global.cpd.runtime.DefaultConfigurations.LayerParam3 = LayerF3.Text;
            Global.cpd.runtime.DefaultConfigurations.StageParam4 = StageF4.Text;
            Global.cpd.runtime.DefaultConfigurations.LayerParam4 = LayerF4.Text;
            Global.cpd.runtime.DefaultConfigurations.MapParam = MapF.Text;
            Global.cpd.project.Config.StageNum = (int)ProjNum.Value;

            Global.cpd.runtime.DefaultConfigurations.HeaderHTML = Subsystem.EncodeBase64(OutHeader.Text);
            Global.cpd.runtime.DefaultConfigurations.MiddleHTML = Subsystem.EncodeBase64(OutMiddle.Text);
            Global.cpd.runtime.DefaultConfigurations.FooterHTML = Subsystem.EncodeBase64(OutFooter.Text);

            Global.cpd.project.Config.UseWorldmap = UseWorldmap.Checked;
            if (!Global.cpd.runtime.Definitions.Package.Contains("28") && (Global.cpd.project.Use3rdMapData || Global.cpd.project.Use3rdMapData != Use3rdMapData.Checked))
            {
                if (Global.cpd.project.Use3rdMapData)
                {
                    Global.cpd.runtime.Definitions.StageSize.x = (int)MapSizeWidth.Value;
                    Global.cpd.runtime.Definitions.StageSize.y = (int)MapSizeHeight.Value;
                    Global.cpd.runtime.Definitions.StageSize2.x = (int)MapSize2Width.Value;
                    Global.cpd.runtime.Definitions.StageSize2.y = (int)MapSize2Height.Value;
                    Global.cpd.runtime.Definitions.StageSize3.x = (int)MapSize3Width.Value;
                    Global.cpd.runtime.Definitions.StageSize3.y = (int)MapSize3Height.Value;
                    Global.cpd.runtime.Definitions.StageSize4.x = (int)MapSize4Width.Value;
                    Global.cpd.runtime.Definitions.StageSize4.y = (int)MapSize4Height.Value;
                    if (Global.cpd.project.Runtime.Definitions.LayerSize.bytesize != 0)
                    {
                        Global.cpd.runtime.Definitions.LayerSize.x = (int)MapSizeWidth.Value;
                        Global.cpd.runtime.Definitions.LayerSize.y = (int)MapSizeHeight.Value;
                        Global.cpd.runtime.Definitions.LayerSize2.x = (int)MapSize2Width.Value;
                        Global.cpd.runtime.Definitions.LayerSize2.y = (int)MapSize2Height.Value;
                        Global.cpd.runtime.Definitions.LayerSize3.x = (int)MapSize3Width.Value;
                        Global.cpd.runtime.Definitions.LayerSize3.y = (int)MapSize3Height.Value;
                        Global.cpd.runtime.Definitions.LayerSize4.x = (int)MapSize4Width.Value;
                        Global.cpd.runtime.Definitions.LayerSize4.y = (int)MapSize4Height.Value;
                    }
                    Global.MainWnd.MainDesigner.ForceBufferResize();
                    Global.MainWnd.UpdateLayer();
                    Global.MainWnd.UpdateScrollbar();
                    Global.state.StageSizeChanged = true;
                }
                if(Global.cpd.project.Use3rdMapData != Use3rdMapData.Checked)
                {
                    if (Use3rdMapData.Checked)
                    {
                        Global.MainWnd.MainDesigner.CreateDrawItemCodeReference();
                        Project.Convert3rdMapData(Global.cpd.project.StageData, Global.cpd.runtime.Definitions.StageSize.bytesize);
                        Project.Convert3rdMapData(Global.cpd.project.StageData2, Global.cpd.runtime.Definitions.StageSize.bytesize);
                        Project.Convert3rdMapData(Global.cpd.project.StageData3, Global.cpd.runtime.Definitions.StageSize.bytesize);
                        Project.Convert3rdMapData(Global.cpd.project.StageData4, Global.cpd.runtime.Definitions.StageSize.bytesize);
                        if (Global.cpd.project.Runtime.Definitions.LayerSize.bytesize != 0)
                        {
                            Project.Convert3rdMapData(Global.cpd.project.LayerData, Global.cpd.runtime.Definitions.LayerSize.bytesize);
                            Project.Convert3rdMapData(Global.cpd.project.LayerData2, Global.cpd.runtime.Definitions.LayerSize.bytesize);
                            Project.Convert3rdMapData(Global.cpd.project.LayerData3, Global.cpd.runtime.Definitions.LayerSize.bytesize);
                            Project.Convert3rdMapData(Global.cpd.project.LayerData4, Global.cpd.runtime.Definitions.LayerSize.bytesize);
                        }
                    }
                    else
                    {
                        if (MessageBox.Show($"第3版マップデータから第2版マップデータに移行します。{Environment.NewLine}移行すると設置済みの変種パーツやカスタムパーツが削除されます。{Environment.NewLine}本当に移行してもよろしいですか？", "移行の警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                        {
                            return;
                        }
                        Project.Convert2ndMapData(Global.cpd.project.StageData, Global.cpd.runtime.Definitions.StageSize.bytesize);
                        Project.Convert2ndMapData(Global.cpd.project.StageData2, Global.cpd.runtime.Definitions.StageSize.bytesize);
                        Project.Convert2ndMapData(Global.cpd.project.StageData3, Global.cpd.runtime.Definitions.StageSize.bytesize);
                        Project.Convert2ndMapData(Global.cpd.project.StageData4, Global.cpd.runtime.Definitions.StageSize.bytesize);
                        Global.cpd.runtime.Definitions.StageSize.x = 180;
                        Global.cpd.runtime.Definitions.StageSize.y = 30;
                        Global.cpd.runtime.Definitions.StageSize2.x = 180;
                        Global.cpd.runtime.Definitions.StageSize2.y = 30;
                        Global.cpd.runtime.Definitions.StageSize3.x = 180;
                        Global.cpd.runtime.Definitions.StageSize3.y = 30;
                        Global.cpd.runtime.Definitions.StageSize4.x = 180;
                        Global.cpd.runtime.Definitions.StageSize4.y = 30;
                        if (Global.cpd.project.Runtime.Definitions.LayerSize.bytesize != 0)
                        {
                            Project.Convert2ndMapData(Global.cpd.project.LayerData, Global.cpd.runtime.Definitions.LayerSize.bytesize);
                            Project.Convert2ndMapData(Global.cpd.project.LayerData2, Global.cpd.runtime.Definitions.LayerSize.bytesize);
                            Project.Convert2ndMapData(Global.cpd.project.LayerData3, Global.cpd.runtime.Definitions.LayerSize.bytesize);
                            Project.Convert2ndMapData(Global.cpd.project.LayerData4, Global.cpd.runtime.Definitions.LayerSize.bytesize);
                            Global.cpd.runtime.Definitions.LayerSize.x = 180;
                            Global.cpd.runtime.Definitions.LayerSize.y = 30;
                            Global.cpd.runtime.Definitions.LayerSize2.x = 180;
                            Global.cpd.runtime.Definitions.LayerSize2.y = 30;
                            Global.cpd.runtime.Definitions.LayerSize3.x = 180;
                            Global.cpd.runtime.Definitions.LayerSize3.y = 30;
                            Global.cpd.runtime.Definitions.LayerSize4.x = 180;
                            Global.cpd.runtime.Definitions.LayerSize4.y = 30;
                        }
                    }
                    Global.cpd.project.Use3rdMapData = Use3rdMapData.Checked;
                }
                Global.MainWnd.RefreshAll();
                Global.MainWnd.MainDesigner.ClearBuffer();
                Global.MainWnd.MainDesigner.AddBuffer();
            }
            Global.state.EditFlag = true;
            List<HTMLReplaceData> list = new List<HTMLReplaceData>();
            foreach (object obj in OutputReplaceView.Rows)
            {
                DataGridViewRow dataGridViewRow = (DataGridViewRow)obj;
                if (dataGridViewRow.Cells[0].Value != null)
                {
                    list.Add(new HTMLReplaceData(dataGridViewRow.Cells[0].Value.ToString(), (dataGridViewRow.Cells[1].Value == null) ? "" : dataGridViewRow.Cells[1].Value.ToString()));
                }
            }
            Global.cpd.runtime.DefaultConfigurations.OutputReplace = list.ToArray();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void TextUndo_Click(object sender, EventArgs e)
        {
            ((RichTextBox)EditContext.SourceControl).Undo();
        }

        private void TextCut_Click(object sender, EventArgs e)
        {
            ((RichTextBox)EditContext.SourceControl).Cut();
        }

        private void TextCopy_Click(object sender, EventArgs e)
        {
            ((RichTextBox)EditContext.SourceControl).Copy();
        }

        private void TextPaste_Click(object sender, EventArgs e)
        {
            ((RichTextBox)EditContext.SourceControl).Paste();
        }
    }
}
