namespace MasaoPlus.Dialogs
{
	public partial class RuntimeManager : global::System.Windows.Forms.Form
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
			this.RuntimeViewer = new global::System.Windows.Forms.ListView();
			this.RuntimeName = new global::System.Windows.Forms.ColumnHeader();
			this.Author = new global::System.Windows.Forms.ColumnHeader();
			this.Archive = new global::System.Windows.Forms.ColumnHeader();
			this.Layer = new global::System.Windows.Forms.ColumnHeader();
			this.Version = new global::System.Windows.Forms.ColumnHeader();
			this.button1 = new global::System.Windows.Forms.Button();
			this.NetworkUpdate = new global::System.Windows.Forms.Button();
			this.NewRuntimeInstall = new global::System.Windows.Forms.Button();
			this.DownProgress = new global::System.Windows.Forms.ProgressBar();
			this.Uninstall = new global::System.Windows.Forms.Button();
			base.SuspendLayout();
			this.RuntimeViewer.Columns.AddRange(new global::System.Windows.Forms.ColumnHeader[]
			{
				this.RuntimeName,
				this.Author,
				this.Archive,
				this.Layer,
				this.Version
			});
			this.RuntimeViewer.FullRowSelect = true;
			this.RuntimeViewer.GridLines = true;
			this.RuntimeViewer.HeaderStyle = global::System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.RuntimeViewer.HideSelection = false;
			this.RuntimeViewer.Location = new global::System.Drawing.Point(12, 12);
			this.RuntimeViewer.MultiSelect = false;
			this.RuntimeViewer.Name = "RuntimeViewer";
			this.RuntimeViewer.Size = new global::System.Drawing.Size(568, 201);
			this.RuntimeViewer.TabIndex = 0;
			this.RuntimeViewer.UseCompatibleStateImageBehavior = false;
			this.RuntimeViewer.View = global::System.Windows.Forms.View.Details;
			this.RuntimeViewer.SelectedIndexChanged += new global::System.EventHandler(this.RuntimeViewer_SelectedIndexChanged);
			this.RuntimeName.Text = "ランタイム名";
			this.RuntimeName.Width = 200;
			this.Author.Text = "作者";
			this.Author.Width = 70;
			this.Archive.Text = "実行アーカイブ";
			this.Archive.Width = 120;
			this.Layer.Text = "レイヤー";
			this.Version.Text = "バージョン";
			this.Version.Width = 60;
			this.button1.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new global::System.Drawing.Point(486, 219);
			this.button1.Name = "button1";
			this.button1.Size = new global::System.Drawing.Size(94, 25);
			this.button1.TabIndex = 1;
			this.button1.Text = "閉じる";
			this.button1.UseVisualStyleBackColor = true;
			this.NetworkUpdate.Enabled = false;
			this.NetworkUpdate.Location = new global::System.Drawing.Point(12, 219);
			this.NetworkUpdate.Name = "NetworkUpdate";
			this.NetworkUpdate.Size = new global::System.Drawing.Size(145, 25);
			this.NetworkUpdate.TabIndex = 2;
			this.NetworkUpdate.Text = "ネットワーク アップデート(&N)";
			this.NetworkUpdate.UseVisualStyleBackColor = true;
			this.NetworkUpdate.Click += new global::System.EventHandler(this.NetworkUpdate_Click);
			this.NewRuntimeInstall.Location = new global::System.Drawing.Point(289, 219);
			this.NewRuntimeInstall.Name = "NewRuntimeInstall";
			this.NewRuntimeInstall.Size = new global::System.Drawing.Size(172, 25);
			this.NewRuntimeInstall.TabIndex = 3;
			this.NewRuntimeInstall.Text = "新しいランタイムのインストール(&I)";
			this.NewRuntimeInstall.UseVisualStyleBackColor = true;
			this.NewRuntimeInstall.Click += new global::System.EventHandler(this.NewRuntimeInstall_Click);
			this.DownProgress.Location = new global::System.Drawing.Point(50, 98);
			this.DownProgress.Name = "DownProgress";
			this.DownProgress.Size = new global::System.Drawing.Size(488, 28);
			this.DownProgress.TabIndex = 4;
			this.DownProgress.Visible = false;
			this.Uninstall.Enabled = false;
			this.Uninstall.Location = new global::System.Drawing.Point(163, 219);
			this.Uninstall.Name = "Uninstall";
			this.Uninstall.Size = new global::System.Drawing.Size(120, 25);
			this.Uninstall.TabIndex = 5;
			this.Uninstall.Text = "アンインストール(&U)";
			this.Uninstall.UseVisualStyleBackColor = true;
			this.Uninstall.Click += new global::System.EventHandler(this.Uninstall_Click);
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 12f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(592, 256);
			base.ControlBox = false;
			base.Controls.Add(this.Uninstall);
			base.Controls.Add(this.DownProgress);
			base.Controls.Add(this.NewRuntimeInstall);
			base.Controls.Add(this.NetworkUpdate);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.RuntimeViewer);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "RuntimeManager";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ランタイム マネージャ";
			base.Shown += new global::System.EventHandler(this.InstalledRuntime_Shown);
			base.ResumeLayout(false);
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.ListView RuntimeViewer;

		private global::System.Windows.Forms.Button button1;

		private global::System.Windows.Forms.Button NetworkUpdate;

		private global::System.Windows.Forms.Button NewRuntimeInstall;

		private global::System.Windows.Forms.ColumnHeader RuntimeName;

		private global::System.Windows.Forms.ColumnHeader Author;

		private global::System.Windows.Forms.ColumnHeader Archive;

		private global::System.Windows.Forms.ColumnHeader Version;

		private global::System.Windows.Forms.ColumnHeader Layer;

		private global::System.Windows.Forms.ProgressBar DownProgress;

		private global::System.Windows.Forms.Button Uninstall;
	}
}
