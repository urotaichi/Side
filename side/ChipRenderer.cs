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
        /// チップを描画します
        /// </summary>
        public static void RenderChip(Graphics g, ChipData cschip, Point location, Size chipsize, Image chipImage, string character = null, bool useRotation = true)
        {
            GraphicsState transState = g.Save();
            g.TranslateTransform(location.X, location.Y);

            bool isSpecialChip = IsAthleticChip(cschip.name);
            bool isMainCharacter = character == Global.cpd.Mapchip[1].character;
            bool shouldReverseDirection = isMainCharacter && 
                (Global.state.MapEditMode && location.X > Global.cpd.runtime.Definitions.MapSize.x / 2 ||
                Global.state.ChipRegister.ContainsKey("view_move_type") && int.Parse(Global.state.ChipRegister["view_move_type"]) == 2);

            if (isSpecialChip)
            {
                // 特殊チップ（アスレチック等）の描画
                AthleticView.list[cschip.name].Min(cschip, g, chipsize);
            }
            else
            {
                // 標準チップの描画
                g.TranslateTransform(chipsize.Width / 2, chipsize.Height / 2);
                
                if (isMainCharacter)
                {
                    g.ScaleTransform(-1, 1); // 基本主人公は逆向き
                    if (shouldReverseDirection)
                    {
                        g.ScaleTransform(-1, 1); // 特殊条件下では元の向き
                    }
                }

                if (useRotation && cschip.rotate != 0)
                {
                    g.RotateTransform(cschip.rotate);
                }

                var rect = new Rectangle(new Point(-chipsize.Width / 2, -chipsize.Height / 2), chipsize);

                if (character == "4" && ShouldApplyWaterTransparency(out float waterLevel))
                {
                    // 水の半透明処理
                    ApplyWaterTransparency(g, chipImage, rect, cschip.pattern, chipsize, waterLevel);
                }
                else if (cschip.repeat != default)
                {
                    for (int j = 0; j < cschip.repeat; j++)
                    {
                        g.DrawImage(chipImage,
                            new Rectangle(new Point(-chipsize.Width / 2 + j * chipsize.Width * Math.Sign(cschip.rotate), -chipsize.Height / 2), chipsize),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                    }
                }
                else if (cschip.repeat_x != default)
                {
                    for (int j = 0; j < cschip.repeat_x; j++)
                    {
                        g.DrawImage(chipImage,
                            new Rectangle(new Point(-chipsize.Width / 2 + j * chipsize.Width, -chipsize.Height / 2), chipsize),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                    }
                }
                else if (cschip.repeat_y != default)
                {
                    for (int j = 0; j < cschip.repeat_y; j++)
                    {
                        g.DrawImage(chipImage,
                            new Rectangle(new Point(-chipsize.Width / 2, -chipsize.Height / 2 + j * chipsize.Height), chipsize),
                            new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                    }
                }
                else
                {
                    g.DrawImage(chipImage, rect, new Rectangle(cschip.pattern, chipsize), GraphicsUnit.Pixel);
                }
            }

            g.Restore(transState);
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
    }
}
