using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace MasaoPlus
{
    /// <summary>
    /// チップの描画に関するユーティリティクラス
    /// </summary>
    public static class ChipRenderer
    {
        /// <summary>
        /// 拡張画像を描画します
        /// </summary>
        public static void DrawExtendChip(Graphics g, Rectangle rectangle, Point xdraw, Size chipsize)
        {
            // 拡張描画画像は今のところ正方形だけだからInterpolationModeは固定
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(Global.MainWnd.MainDesigner.DrawExOrig, rectangle, new Rectangle(xdraw, chipsize), GraphicsUnit.Pixel);
        }

        /// <summary>
        /// 水チップの透明度を適用して描画します
        /// </summary>
        public static void ApplyWaterTransparency(Graphics g, Image chipImage, Rectangle destRect, Point srcPoint, Size srcSize, float waterLevel)
        {
            var colorMatrix = new ColorMatrix
            {
                Matrix00 = 1f,
                Matrix11 = 1f,
                Matrix22 = 1f,
                Matrix33 = waterLevel / 255f,
                Matrix44 = 1f
            };
            using var imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix);
            g.DrawImage(chipImage, destRect, srcPoint.X, srcPoint.Y, srcSize.Width, srcSize.Height,
                        GraphicsUnit.Pixel, imageAttributes);
        }

        /// <summary>
        /// 水の透明設定が有効かどうかを確認し、透明度値を取得します
        /// </summary>
        public static bool ShouldApplyWaterTransparency(out float waterLevel)
        {
            waterLevel = 1.0f;
            return Global.state.ChipRegister.TryGetValue("water_clear_switch", out string water_clear_switch) &&
                   bool.Parse(water_clear_switch) == false &&
                   Global.state.ChipRegister.TryGetValue("water_clear_level", out string value) &&
                   float.TryParse(value, out waterLevel);
        }

        /// <summary>
        /// アスレチックチップかどうか判定します
        /// </summary>
        public static bool IsAthleticChip(string chipName)
        {
            return chipName switch
            {
                "一方通行" => true,
                "左右へ押せるドッスンスンのゴール" => true,
                "シーソー" => true,
                "ブランコ" => true,
                "スウィングバー" => true,
                "動くＴ字型" => true,
                "ロープ" => true,
                "長いロープ" => true,
                "ゆれる棒" => true,
                "人間大砲" => true,
                "曲線による上り坂" => true,
                "曲線による下り坂" => true,
                "乗れる円" => true,
                "跳ねる円" => true,
                "円" => true,
                "半円" => true,
                "ファイヤーバー" => true,
                "ファイヤーバー2本" => true,
                "ファイヤーバー3本　左回り" => true,
                "ファイヤーバー3本　右回り" => true,
                "スウィングファイヤーバー" => true,
                "人口太陽" => true,
                "ファイヤーリング" => true,
                "ファイヤーウォール" => true,
                "スイッチ式ファイヤーバー" => true,
                "スイッチ式動くＴ字型" => true,
                "スイッチ式速く動くＴ字型" => true,
                _ => false
            };
        }

        /// <summary>
        /// オリジナルボスの拡張描画のポイントを取得します
        /// </summary>
        public static Point GetOribossExtensionPoint(int oribossUgoki)
        {
            return oribossUgoki switch
            {
                1 => new Point(352, 256),
                2 => new Point(96, 0),
                3 => new Point(64, 0),
                4 => new Point(256, 0),
                5 => new Point(288, 0),
                6 => new Point(288, 448),
                7 => new Point(320, 448),
                8 => new Point(32, 32),
                9 => new Point(96, 0),
                10 => new Point(0, 32),
                11 => new Point(64, 0),
                12 => new Point(96, 32),
                13 => new Point(64, 0),
                14 => new Point(352, 448),
                15 => new Point(416, 448),
                16 => new Point(288, 448),
                17 => new Point(320, 448),
                18 => new Point(96, 0),
                19 => new Point(96, 0),
                20 => new Point(256, 0),
                21 => new Point(256, 0),
                22 => new Point(352, 448),
                23 => new Point(384, 448),
                24 => new Point(32, 32),
                25 => new Point(32, 32),
                26 => new Point(32, 128),
                27 => new Point(32, 128),
                _ => default,
            };
        }

        /// <summary>
        /// オリジナルボスの動作説明を取得します
        /// </summary>
        public static string GetOribossMovementDescription(int oribossUgoki)
        {
            return oribossUgoki switch
            {
                1 => "停止",
                2 => "左右移動",
                3 => "上下移動",
                4 => "左回り",
                5 => "右回り",
                6 => "四角形左回り",
                7 => "四角形右回り",
                8 => "HPが半分になると左へ移動",
                9 => "HPが減ると左と右へ移動",
                10 => "HPが半分になると上へ移動",
                11 => "HPが減ると上と下へ移動",
                12 => "HPが半分になると下へ移動",
                13 => "HPが減ると下と上へ移動",
                14 => "画面の端で方向転換",
                15 => "ジグザグ移動",
                16 => "画面の内側を左回り",
                17 => "画面の内側を右回り",
                18 => "HPが半分以下になると左右移動",
                19 => "HPが1/3以下になると左右移動",
                20 => "HPが半分以下になると左回り",
                21 => "HPが1/3以下になると左回り",
                22 => "斜め上へ往復",
                23 => "斜め下へ往復",
                24 => "中央で停止",
                25 => "中央で停止 主人公の方を向く",
                26 => "巨大化  中央から",
                27 => "巨大化  右から",
                _ => "",
            };
        }
    }
}
