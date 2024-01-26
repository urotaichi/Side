using System.Drawing.Drawing2D;
using System.Drawing;

namespace MasaoPlus
{
    internal class IconImageView
    {
        internal IconImageView(int dpi, Image image, int width = 16, int height = 16)
        {
            this.dpi = dpi;
            this.image = image;
            this.width = width * dpi / 96;
            this.height = height * dpi / 96;
        }

        internal Bitmap View()
        {
            Bitmap img = new(width, height);
            Graphics g = Graphics.FromImage(img);
            g.PixelOffsetMode = PixelOffsetMode.Half;
            if (dpi / 96 >= 2)
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
            }
            else g.InterpolationMode = InterpolationMode.High;
            g.DrawImage(image, 0, 0, width, height);
            g.Dispose();
            return img;
        }

        private readonly int dpi;

        private readonly Image image;

        private readonly int width;

        private readonly int height;
    }
}
