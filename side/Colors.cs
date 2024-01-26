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
                return Color.FromArgb(r, g, b);
            }
            set
            {
                r = value.R;
                g = value.G;
                b = value.B;
            }
        }

        public override string ToString()
        {
            return $"{r},{g},{b}";
        }

        public Colors(string color)
        {
            string[] array = color.Split(
            [
                ','
            ]);
            if (array.Length < 3)
            {
                throw new Exception($"色の取得に失敗しました:{color}");
            }
            try
            {
                r = int.Parse(array[0]);
                g = int.Parse(array[1]);
                b = int.Parse(array[2]);
            }
            catch (Exception ex)
            {
                throw new Exception($"色の取得に失敗しました:{color}\n{ex.Message}\nst:{ex.StackTrace}");
            }
        }

        public int r;

        public int g;

        public int b;
    }
}
