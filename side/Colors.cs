using System;
using System.Drawing;
using System.Text;

namespace MasaoPlus
{
    public struct Colors
    {
        public Color c
        {
            readonly get
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

        public override readonly string ToString()
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

        /// <summary>
        /// 指定された色の反転色を計算して返します
        /// </summary>
        /// <param name="color">反転する色</param>
        /// <returns>反転色</returns>
        public static Color GetInvertedColor(Color color)
        {
            return Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
        }

        public int r;

        public int g;

        public int b;
    }
}
