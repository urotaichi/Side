using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Threading;
using System.Windows.Forms;

namespace MasaoPlus.Controls
{
	public class CommandLinkButton : Control, IButtonControl
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
			base.SuspendLayout();
			base.MouseLeave += this.CommandLinkButton_MouseLeave;
			base.Leave += this.CommandLinkButton_Leave;
			base.KeyUp += this.CommandLinkButton_KeyUp;
			base.MouseDown += this.CommandLinkButton_MouseDown;
			base.Enter += this.CommandLinkButton_Enter;
			base.MouseUp += this.CommandLinkButton_MouseUp;
			base.MouseEnter += this.CommandLinkButton_MouseEnter;
			base.KeyDown += this.CommandLinkButton_KeyDown;
			base.ResumeLayout(false);
		}

		public override string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		[Description("説明として表示する文字列です。")]
		[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		private bool Entered
		{
			get
			{
				return this.entered;
			}
			set
			{
				this.entered = value;
				this.Refresh();
			}
		}

		private bool Focusing
		{
			get
			{
				return this.focusing;
			}
			set
			{
				this.focusing = value;
				this.Refresh();
			}
		}

		private bool Pressed
		{
			get
			{
				return this.pressed;
			}
			set
			{
				this.pressed = value;
				this.Refresh();
			}
		}

		public CommandLinkButton()
		{
			base.GotFocus += this.CommandLinkButton_GotFocus;
			base.LostFocus += this.CommandLinkButton_LostFocus;
			this.InitializeComponent();
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			if (this.Pressed)
			{
				ControlPaint.DrawBorder3D(pe.Graphics, new Rectangle(default(Point), base.Size), Border3DStyle.SunkenInner);
			}
			else if (this.Entered || this.Defaulting)
			{
				ControlPaint.DrawBorder3D(pe.Graphics, new Rectangle(default(Point), base.Size), Border3DStyle.RaisedOuter);
			}
			else
			{
				ControlPaint.DrawBorder3D(pe.Graphics, new Rectangle(default(Point), base.Size), Border3DStyle.Flat);
			}
			if (this.Focusing)
			{
				ControlPaint.DrawFocusRectangle(pe.Graphics, new Rectangle(3, 3, base.Width - 6, base.Height - 6));
			}
			FontStyle fontStyle = FontStyle.Bold;
			if (this.Entered)
			{
				fontStyle |= FontStyle.Underline;
			}
			using (Font font = new Font(this.Font, fontStyle))
			{
				TextRenderer.DrawText(pe.Graphics, this.text, font, new Point(3, 3), Color.Blue);
				Size size = TextRenderer.MeasureText(this.text, font);
				TextRenderer.DrawText(pe.Graphics, this.description, this.Font, new Point(3, 5 + size.Height), Color.Black);
			}
			base.OnPaint(pe);
		}

		private void CommandLinkButton_MouseEnter(object sender, EventArgs e)
		{
			this.Entered = true;
		}

		private void CommandLinkButton_MouseLeave(object sender, EventArgs e)
		{
			this.Entered = false;
		}

		private void CommandLinkButton_Enter(object sender, EventArgs e)
		{
			this.Focusing = true;
		}

		private void CommandLinkButton_Leave(object sender, EventArgs e)
		{
			this.Focusing = false;
		}

		private void CommandLinkButton_LostFocus(object sender, EventArgs e)
		{
			this.Focusing = false;
		}

		private void CommandLinkButton_GotFocus(object sender, EventArgs e)
		{
			this.Focusing = true;
		}

		private void CommandLinkButton_MouseDown(object sender, MouseEventArgs e)
		{
			base.Focus();
			this.Pressed = true;
		}

		private void CommandLinkButton_MouseUp(object sender, MouseEventArgs e)
		{
			this.Pressed = false;
			this.Click(this, new EventArgs());
		}

		private void CommandLinkButton_KeyDown(object sender, KeyEventArgs e)
		{
			base.Focus();
			if (e.Modifiers == Keys.None && e.KeyCode == Keys.Space)
			{
				this.Pressed = true;
			}
		}

		private void CommandLinkButton_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.None && e.KeyCode == Keys.Space)
			{
				this.Pressed = false;
				this.Click(this, new EventArgs());
			}
		}

		public DialogResult DialogResult
		{
			get
			{
				return this.dr;
			}
			set
			{
				this.dr = value;
			}
		}

		public void NotifyDefault(bool value)
		{
			this.Defaulting = value;
			this.Refresh();
		}

		public void PerformClick()
		{
			this.Pressed = true;
			Application.DoEvents();
			Thread.Sleep(1);
			this.Pressed = false;
		}

		private IContainer components;

		private string text;

		private string description;

		public new EventHandler Click;

		private bool entered;

		private bool focusing;

		private bool pressed;

		private bool Defaulting;

		private DialogResult dr;
	}
}
