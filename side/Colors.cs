using System;
using System.Drawing;
using System.Text;

namespace MasaoPlus
{
	// Token: 0x02000028 RID: 40
	public struct Colors
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000121 RID: 289 RVA: 0x00002B98 File Offset: 0x00000D98
		// (set) Token: 0x06000122 RID: 290 RVA: 0x00002BB1 File Offset: 0x00000DB1
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

		// Token: 0x06000123 RID: 291 RVA: 0x00018030 File Offset: 0x00016230
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

		// Token: 0x06000124 RID: 292 RVA: 0x00018098 File Offset: 0x00016298
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

		// Token: 0x04000161 RID: 353
		public int r;

		// Token: 0x04000162 RID: 354
		public int g;

		// Token: 0x04000163 RID: 355
		public int b;
	}
}
