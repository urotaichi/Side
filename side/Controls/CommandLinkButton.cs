using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Threading;
using System.Windows.Forms;

namespace MasaoPlus.Controls
{
	// Token: 0x02000035 RID: 53
	public class CommandLinkButton : Control, IButtonControl
	{
		// Token: 0x060001DE RID: 478 RVA: 0x000033FE File Offset: 0x000015FE
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x060001DF RID: 479 RVA: 0x000261A4 File Offset: 0x000243A4
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

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x0000341D File Offset: 0x0000161D
		// (set) Token: 0x060001E1 RID: 481 RVA: 0x00003425 File Offset: 0x00001625
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

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x0000342E File Offset: 0x0000162E
		// (set) Token: 0x060001E3 RID: 483 RVA: 0x00003436 File Offset: 0x00001636
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

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060001E4 RID: 484 RVA: 0x0000343F File Offset: 0x0000163F
		// (set) Token: 0x060001E5 RID: 485 RVA: 0x00003447 File Offset: 0x00001647
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

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x00003456 File Offset: 0x00001656
		// (set) Token: 0x060001E7 RID: 487 RVA: 0x0000345E File Offset: 0x0000165E
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

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060001E8 RID: 488 RVA: 0x0000346D File Offset: 0x0000166D
		// (set) Token: 0x060001E9 RID: 489 RVA: 0x00003475 File Offset: 0x00001675
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

		// Token: 0x060001EA RID: 490 RVA: 0x00003484 File Offset: 0x00001684
		public CommandLinkButton()
		{
			base.GotFocus += this.CommandLinkButton_GotFocus;
			base.LostFocus += this.CommandLinkButton_LostFocus;
			this.InitializeComponent();
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00026250 File Offset: 0x00024450
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

		// Token: 0x060001EC RID: 492 RVA: 0x000034B6 File Offset: 0x000016B6
		private void CommandLinkButton_MouseEnter(object sender, EventArgs e)
		{
			this.Entered = true;
		}

		// Token: 0x060001ED RID: 493 RVA: 0x000034BF File Offset: 0x000016BF
		private void CommandLinkButton_MouseLeave(object sender, EventArgs e)
		{
			this.Entered = false;
		}

		// Token: 0x060001EE RID: 494 RVA: 0x000034C8 File Offset: 0x000016C8
		private void CommandLinkButton_Enter(object sender, EventArgs e)
		{
			this.Focusing = true;
		}

		// Token: 0x060001EF RID: 495 RVA: 0x000034D1 File Offset: 0x000016D1
		private void CommandLinkButton_Leave(object sender, EventArgs e)
		{
			this.Focusing = false;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x000034D1 File Offset: 0x000016D1
		private void CommandLinkButton_LostFocus(object sender, EventArgs e)
		{
			this.Focusing = false;
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x000034C8 File Offset: 0x000016C8
		private void CommandLinkButton_GotFocus(object sender, EventArgs e)
		{
			this.Focusing = true;
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x000034DA File Offset: 0x000016DA
		private void CommandLinkButton_MouseDown(object sender, MouseEventArgs e)
		{
			base.Focus();
			this.Pressed = true;
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x000034EA File Offset: 0x000016EA
		private void CommandLinkButton_MouseUp(object sender, MouseEventArgs e)
		{
			this.Pressed = false;
			this.Click(this, new EventArgs());
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00003504 File Offset: 0x00001704
		private void CommandLinkButton_KeyDown(object sender, KeyEventArgs e)
		{
			base.Focus();
			if (e.Modifiers == Keys.None && e.KeyCode == Keys.Space)
			{
				this.Pressed = true;
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00003526 File Offset: 0x00001726
		private void CommandLinkButton_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.None && e.KeyCode == Keys.Space)
			{
				this.Pressed = false;
				this.Click(this, new EventArgs());
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x00003552 File Offset: 0x00001752
		// (set) Token: 0x060001F7 RID: 503 RVA: 0x0000355A File Offset: 0x0000175A
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

		// Token: 0x060001F8 RID: 504 RVA: 0x00003563 File Offset: 0x00001763
		public void NotifyDefault(bool value)
		{
			this.Defaulting = value;
			this.Refresh();
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x00003572 File Offset: 0x00001772
		public void PerformClick()
		{
			this.Pressed = true;
			Application.DoEvents();
			Thread.Sleep(1);
			this.Pressed = false;
		}

		// Token: 0x0400028A RID: 650
		private IContainer components;

		// Token: 0x0400028B RID: 651
		private string text;

		// Token: 0x0400028C RID: 652
		private string description;

		// Token: 0x0400028D RID: 653
		public new EventHandler Click;

		// Token: 0x0400028E RID: 654
		private bool entered;

		// Token: 0x0400028F RID: 655
		private bool focusing;

		// Token: 0x04000290 RID: 656
		private bool pressed;

		// Token: 0x04000291 RID: 657
		private bool Defaulting;

		// Token: 0x04000292 RID: 658
		private DialogResult dr;
	}
}
