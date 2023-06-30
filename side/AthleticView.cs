using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace MasaoPlus
{
    readonly struct Athletic
    {
        public Athletic(MainFunc Main, MaxFunc Max, MainFunc Min, SmallFunc Small, MainFunc Large)
        {
            this.Main = Main;
            this.Max = Max;
            this.Min = Min;
            this.Small = Small;
            this.Large = Large;
        }
        public delegate void MainFunc(ChipData cschip, Graphics g, Size chipsize);
        public delegate void MaxFunc(ChipData cschip, Graphics g);
        public delegate void SmallFunc(ChipData cschip, Graphics g, int height);
        public MainFunc Main { get; } // チップ - クラシック、チップリストの左上 
        public MaxFunc Max { get; } // グラフィカルデザイナ
        public MainFunc Min { get; } // ステータスバーっぽいところに表示される小さいアイコン
        public SmallFunc Small { get; } // チップ - サムネイル
        public MainFunc Large { get; } // チップ - チップ
    }
    internal static class AthleticView
    {
        public static Dictionary<string, Athletic> list = new Dictionary<string, Athletic>(){
            {"一方通行", new Athletic(
                (cschip, g, chipsize) => {
                    if (!cschip.description.Contains("表示なし")){
                        Pen pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
                        if (cschip.description.Contains("右"))
                            g.DrawLine(pen, chipsize.Width - 1, 0, chipsize.Width - 1, chipsize.Height);
                        else if (cschip.description.Contains("左"))
                            g.DrawLine(pen, 1, 0, 1, chipsize.Height);
                        else if (cschip.description.Contains("上"))
                            g.DrawLine(pen, 0, 1, chipsize.Width, 1);
                        else if (cschip.description.Contains("下"))
                            g.DrawLine(pen, 0, chipsize.Height - 1, chipsize.Width, chipsize.Height - 1);
                        pen.Dispose();
                    }
                },
                (cschip, g) =>
                {
                    if (!cschip.description.Contains("表示なし")){
                        Pen pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
                        if (cschip.description.Contains("右"))
                            g.DrawLine(pen, cschip.view_size.Width - 1, 0, cschip.view_size.Width - 1, cschip.view_size.Height);
                        else if (cschip.description.Contains("左"))
                            g.DrawLine(pen, 1, 0, 1, cschip.view_size.Height);
                        else if (cschip.description.Contains("上"))
                            g.DrawLine(pen, 0, 1, cschip.view_size.Width, 1);
                        else if (cschip.description.Contains("下"))
                            g.DrawLine(pen, 0, cschip.view_size.Height - 1, cschip.view_size.Width, cschip.view_size.Height - 1);
                        pen.Dispose();
                    }
                },
                (cschip, g, chipsize) => {
                    if (!cschip.description.Contains("表示なし")){
                        Pen pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
                        if (cschip.description.Contains("右"))
                            g.DrawLine(pen, chipsize.Width - 1, 0, chipsize.Width - 1, chipsize.Width);
                        else if (cschip.description.Contains("左"))
                            g.DrawLine(pen, 0, 0, 0, chipsize.Width);
                        else if (cschip.description.Contains("上"))
                            g.DrawLine(pen, 0, 0, chipsize.Width, 0);
                        else if (cschip.description.Contains("下"))
                            g.DrawLine(pen, 0, chipsize.Width - 1, chipsize.Width, chipsize.Width - 1);
                        pen.Dispose();
                    }
                },
                (cschip, g, height) =>
                {
                    if (!cschip.description.Contains("表示なし")){
                        Pen pen = new Pen(Global.cpd.project.Config.Firebar2, 2 / height);
                        if (cschip.description.Contains("右"))
                            g.DrawLine(pen, height - 1, 0, height - 1, height);
                        else if (cschip.description.Contains("左"))
                            g.DrawLine(pen, 1, 0, 1, height);
                        else if (cschip.description.Contains("上"))
                            g.DrawLine(pen, 0, 1, height, 1);
                        else if (cschip.description.Contains("下"))
                            g.DrawLine(pen, 0, height - 1, height, height - 1);
                        pen.Dispose();
                    }
                },
                (cschip, g, chipsize) => {
                    if (!cschip.description.Contains("表示なし")){
                        Pen pen = new Pen(Global.cpd.project.Config.Firebar2, 2);
                        if (cschip.description.Contains("右"))
                            g.DrawLine(pen, chipsize.Width - 1, 0, chipsize.Width - 1, chipsize.Height);
                        else if (cschip.description.Contains("左"))
                            g.DrawLine(pen, 1, 0, 1, chipsize.Height);
                        else if (cschip.description.Contains("上"))
                            g.DrawLine(pen, 0, 1, chipsize.Width, 1);
                        else if (cschip.description.Contains("下"))
                            g.DrawLine(pen, 0, chipsize.Height - 1, chipsize.Width, chipsize.Height - 1);
                        pen.Dispose();
                    }
                })},
        };
    }
}
