using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MasaoPlus.Controls;
using MasaoPlus.Properties;

namespace MasaoPlus.Dialogs
{
    public partial class StartUp : Form
    {
        public StartUp()
        {
            InitializeComponent();
            DialogResult = DialogResult.None;
        }

        private void StartUp_Load(object sender, EventArgs e)
        {
            WelcomeLabel.Text = Global.definition.AppNameFull + " v" + Global.definition.Version;
        }

        private void StartUp_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            if (Global.config.localSystem.CheckAutoUpdate)
            {
                Subsystem.UpdateAutoCheck();
            }
        }

        private void StartUp_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void NewProj_Click(object sender, EventArgs e)
        {
            using NewProject newProject = new NewProject();
            if (newProject.ShowDialog() == DialogResult.OK)
            {
                ProjectPath = newProject.CreatedProject;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = $"{Global.definition.AppName}プロジェクト及びHTML/XML(*.html,*.xml,*{Global.definition.ProjExt})|*.html;*.xml;*{Global.definition.ProjExt}|{Global.definition.AppName} プロジェクト (*{Global.definition.ProjExt})|*{Global.definition.ProjExt}|HTML/XML ドキュメント(*.htm*;*.xml)|*.htm*;*.xml|全てのファイル|*.*";
            openFileDialog.InitialDirectory = Global.config.lastData.ProjDirF;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetExtension(openFileDialog.FileName) == Global.definition.ProjExt)
                {
                    ProjectPath = openFileDialog.FileName;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    using HTMLInheritance htmlinheritance = new HTMLInheritance(openFileDialog.FileName);
                    if (htmlinheritance.ShowDialog() == DialogResult.OK)
                    {
                        ProjectPath = htmlinheritance.ProjectFile;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
            }
        }

        private void ExMenu_Click(object sender, EventArgs e)
        {
            ExMenuStrip.Show(ExMenu, new Point(0, ExMenu.Height));
        }

        private void CallConfig_Click(object sender, EventArgs e)
        {
            using SideConfig sideConfig = new SideConfig();
            sideConfig.ShowDialog();
        }

        private void CallRuntimeManager_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = DialogResult.Retry;
            while (dialogResult == DialogResult.Retry)
            {
                using RuntimeManager runtimeManager = new RuntimeManager();
                dialogResult = runtimeManager.ShowDialog();
            }
        }

        private void CallSideUpdate_Click(object sender, EventArgs e)
        {
            using WebUpdate webUpdate = new WebUpdate();
            if (webUpdate.ShowDialog() == DialogResult.Retry)
            {
                Global.state.RunFile = (string)webUpdate.runfile.Clone();
                Close();
            }
        }

        private void InheritNew_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = $"{Global.definition.AppName} プロジェクト (*{Global.definition.ProjExt})|*{Global.definition.ProjExt}";
            openFileDialog.InitialDirectory = Global.config.lastData.ProjDirF;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using ProjInheritance projInheritance = new ProjInheritance(openFileDialog.FileName);
                if (projInheritance.DialogResult != DialogResult.Abort && projInheritance.ShowDialog() == DialogResult.OK)
                {
                    ProjectPath = projInheritance.NewProjectName;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        public string ProjectPath = "";
    }
}
