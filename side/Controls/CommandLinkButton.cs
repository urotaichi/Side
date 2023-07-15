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
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.MouseLeave += CommandLinkButton_MouseLeave;
			base.Leave += CommandLinkButton_Leave;
			base.KeyUp += CommandLinkButton_KeyUp;
			base.MouseDown += CommandLinkButton_MouseDown;
			base.Enter += CommandLinkButton_Enter;
			base.MouseUp += CommandLinkButton_MouseUp;
			base.MouseEnter += CommandLinkButton_MouseEnter;
			base.KeyDown += CommandLinkButton_KeyDown;
			base.ResumeLayout(false);
		}

		public override string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		[Description("説明として表示する文字列です。")]
		[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		private bool Entered
		{
			get
			{
				return entered;
			}
			set
			{
				entered = value;
				Refresh();
			}
		}

		private bool Focusing
		{
			get
			{
				return focusing;
			}
			set
			{
				focusing = value;
				Refresh();
			}
		}

		private bool Pressed
		{
			get
			{
				return pressed;
			}
			set
			{
				pressed = value;
				Refresh();
			}
		}

		public CommandLinkButton()
		{
			base.GotFocus += CommandLinkButton_GotFocus;
			base.LostFocus += CommandLinkButton_LostFocus;
			InitializeComponent();
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			if (Pressed)
			{
				ControlPaint.DrawBorder3D(pe.Graphics, new Rectangle(default, base.Size), Border3DStyle.SunkenInner);
			}
			else if (Entered || Defaulting)
			{
				ControlPaint.DrawBorder3D(pe.Graphics, new Rectangle(default, base.Size), Border3DStyle.RaisedOuter);
			}
			else
			{
				ControlPaint.DrawBorder3D(pe.Graphics, new Rectangle(default, base.Size), Border3DStyle.Flat);
			}
			if (Focusing)
			{
				ControlPaint.DrawFocusRectangle(pe.Graphics, new Rectangle(3, 3, base.Width - 6, base.Height - 6));
			}
			FontStyle fontStyle = FontStyle.Bold;
			if (Entered)
			{
				fontStyle |= FontStyle.Underline;
			}
			using (Font font = new Font(Font, fontStyle))
			{
				TextRenderer.DrawText(pe.Graphics, text, font, new Point(3, 3), Color.Blue);
				Size size = TextRenderer.MeasureText(text, font);
				TextRenderer.DrawText(pe.Graphics, description, Font, new Point(3, 5 + size.Height), Color.Black);
			}
			base.OnPaint(pe);
		}

		private void CommandLinkButton_MouseEnter(object sender, EventArgs e)
		{
			Entered = true;
		}

		private void CommandLinkButton_MouseLeave(object sender, EventArgs e)
		{
			Entered = false;
		}

		private void CommandLinkButton_Enter(object sender, EventArgs e)
		{
			Focusing = true;
		}

		private void CommandLinkButton_Leave(object sender, EventArgs e)
		{
			Focusing = false;
		}

		private void CommandLinkButton_LostFocus(object sender, EventArgs e)
		{
			Focusing = false;
		}

		private void CommandLinkButton_GotFocus(object sender, EventArgs e)
		{
			Focusing = true;
		}

		private void CommandLinkButton_MouseDown(object sender, MouseEventArgs e)
		{
			base.Focus();
			Pressed = true;
		}

		private void CommandLinkButton_MouseUp(object sender, MouseEventArgs e)
		{
			Pressed = false;
			Click(this, new EventArgs());
		}

		private void CommandLinkButton_KeyDown(object sender, KeyEventArgs e)
		{
			base.Focus();
			if (e.Modifiers == Keys.None && e.KeyCode == Keys.Space)
			{
				Pressed = true;
			}
		}

		private void CommandLinkButton_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.None && e.KeyCode == Keys.Space)
			{
				Pressed = false;
				Click(this, new EventArgs());
			}
		}

		public DialogResult DialogResult
		{
			get
			{
				return dr;
			}
			set
			{
				dr = value;
			}
		}

		public void NotifyDefault(bool value)
		{
			Defaulting = value;
			Refresh();
		}

		public void PerformClick()
		{
			Pressed = true;
			Application.DoEvents();
			Thread.Sleep(1);
			Pressed = false;
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
