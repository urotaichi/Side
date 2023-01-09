using System;
using System.Drawing;
using System.Text;

namespace MasaoPlus
{
	public struct Colors
	{
		public Color c
		{
			get
			{
				return Color.FromArgb(this.r, this.g, this.b);
			}
			set
			{
				this.r = (int)value.R;
				this.g = (int)value.G;
				this.b = (int)value.B;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.r.ToString());
			stringBuilder.Append(",");
			stringBuilder.Append(this.g.ToString());
			stringBuilder.Append(",");
			stringBuilder.Append(this.b.ToString());
			return stringBuilder.ToString();
		}

		public Colors(string color)
		{
			string[] array = color.Split(new char[]
			{
				','
			});
			if (array.Length < 3)
			{
				throw new Exception("色の取得に失敗しました:" + color);
			}
			try
			{
				this.r = int.Parse(array[0]);
				this.g = int.Parse(array[1]);
				this.b = int.Parse(array[2]);
			}
			catch (Exception ex)
			{
				throw new Exception(string.Concat(new string[]
				{
					"色の取得に失敗しました:",
					color,
					"\n",
					ex.Message,
					"\nst:",
					ex.StackTrace
				}));
			}
		}

		public int r;

		public int g;

		public int b;
	}
}
