using System;
using System.Collections.Generic;
using System.Drawing;

namespace MasaoPlus
{
    /// <summary>
    /// 塗りつぶしツールのヘルパークラス
    /// </summary>
    public class FillToolHelper
    {
        private readonly List<char[]> repls = [];
        private readonly List<string[]> replsCode = [];

        private struct BufStr(int left, int right, int y)
        {
            public int left = left;
            public int right = right;
            public int y = y;
        }

        // 塗りつぶすチップデータ、塗りつぶし開始座標
        public void FillStart(ChipsData repl, Point pt)
        {
            // 画面外なら終了
            if (GUIDesigner.StageText.IsOverflow(pt))
            {
                return;
            }

            repls.Clear();
            replsCode.Clear();

            if (Global.state.EditingForeground)
            {
                int num = 0;
                while (Global.state.MapEditMode ? (num < Global.cpd.project.Runtime.Definitions.MapSize.y) : (num < GUIDesigner.CurrentStageSize.y))
                {
                    if (Global.state.Use3rdMapDataCurrently)
                    {
                        string[] array = GUIDesigner.PutItemTextCodeStart(num);
                        if (array != null)
                        {
                            replsCode.Add(array);
                        }
                    }
                    else
                    {
                        char[] array = GUIDesigner.PutItemTextStart(num);
                        if (array != null)
                        {
                            repls.Add(array);
                        }
                    }
                    num++;
                }
            }
            else
            {
                for (int i = 0; i < GUIDesigner.CurrentLayerSize.y; i++)
                {
                    if (Global.cpd.project.Use3rdMapData)
                    {
                        string[] array2 = GUIDesigner.PutItemTextCodeStart(i);
                        if (array2 != null)
                        {
                            replsCode.Add(array2);
                        }
                    }
                    else
                    {
                        char[] array2 = GUIDesigner.PutItemTextStart(i);
                        if (array2 != null)
                        {
                            repls.Add(array2);
                        }
                    }
                }
            }

            string stageChar = GUIDesigner.StageText.GetStageChar(pt);
            // 塗りつぶしたマスと同じなら終了
            if (Global.state.MapEditMode && stageChar == repl.character
                || !Global.state.MapEditMode && (Global.cpd.project.Use3rdMapData && stageChar == repl.code
                    || !Global.cpd.project.Use3rdMapData && stageChar == repl.character))
            {
                return;
            }

            if (Global.state.MapEditMode)
            {
                if (Global.MainWnd.MainDesigner.DrawWorldRef.TryGetValue(stageChar, out ChipsData value) && CheckChar(pt, value))
                {
                    FillThis(value, repl, pt);
                }
            }
            else if (Global.state.EditingForeground)
            {
                if (Global.cpd.project.Use3rdMapData && Global.MainWnd.MainDesigner.DrawItemCodeRef.TryGetValue(stageChar, out ChipsData value1) && CheckChar(pt, value1))
                {
                    FillThisCode(value1, repl, pt);
                }
                else if (!Global.cpd.project.Use3rdMapData && Global.MainWnd.MainDesigner.DrawItemRef.TryGetValue(stageChar, out ChipsData value2) && CheckChar(pt, value2))
                {
                    FillThis(value2, repl, pt);
                }
            }
            else
            {
                if (Global.cpd.project.Use3rdMapData && Global.MainWnd.MainDesigner.DrawLayerCodeRef.TryGetValue(stageChar, out ChipsData value1) && CheckChar(pt, value1))
                {
                    FillThisCode(value1, repl, pt);
                }
                else if (!Global.cpd.project.Use3rdMapData && Global.MainWnd.MainDesigner.DrawLayerRef.TryGetValue(stageChar, out ChipsData value2) && CheckChar(pt, value2))
                {
                    FillThis(value2, repl, pt);
                }
            }

            for (int j = 0; j < Global.state.GetCSSize.y; j++)
            {
                if (Global.state.Use3rdMapDataCurrently)
                {
                    GUIDesigner.PutItemTextEnd(replsCode[j], j);
                }
                else
                {
                    GUIDesigner.PutItemTextEnd(repls[j], j);
                }
            }
        }

        // 塗りつぶす前のチップデータ、塗りつぶすチップデータ、塗りつぶし開始座標
        private void FillThisCode(ChipsData old, ChipsData repl, Point pt)
        {
            var queue = new Queue<BufStr>();

            queue.Enqueue(new BufStr(
                ScanLeft(pt, replsCode, old),
                ScanRight(pt, replsCode, old),
                pt.Y
            ));

            var newmap = new List<string[]>(replsCode);

            while (queue.Count > 0)
            {
                int left = queue.Peek().left;
                int right = queue.Peek().right;
                int y = queue.Peek().y;
                queue.Dequeue();

                UpdateLine(left, right, y, repl);

                // 上下を探索
                if (0 < y)
                {
                    SearchLine(left, right, y - 1, newmap, old, queue);
                }
                if (y < Global.state.GetCSSize.y - 1)
                {
                    SearchLine(left, right, y + 1, newmap, old, queue);
                }
            }
        }

        // 塗りつぶす前のチップデータ、塗りつぶすチップデータ、塗りつぶし開始座標
        private void FillThis(ChipsData old, ChipsData repl, Point pt)
        {
            var queue = new Queue<BufStr>();

            queue.Enqueue(new BufStr(
                ScanLeft(pt, repls, old),
                ScanRight(pt, repls, old),
                pt.Y
            ));

            var newmap = new List<char[]>(repls);

            while (queue.Count > 0)
            {
                int left = queue.Peek().left;
                int right = queue.Peek().right;
                int y = queue.Peek().y;
                queue.Dequeue();

                UpdateLine(left, right, y, repl);

                // 上下を探索
                if (0 < y)
                {
                    SearchLine(left, right, y - 1, newmap, old, queue);
                }
                if (y < Global.state.GetCSSize.y - 1)
                {
                    SearchLine(left, right, y + 1, newmap, old, queue);
                }
            }
        }

        private static int ScanLeft(Point pt, List<string[]> newmap, ChipsData old)
        {
            int result = pt.X;

            while (0 < result && newmap[pt.Y][result - 1].Equals(old.code))
            {
                result--;
            }
            return result;
        }

        private static int ScanLeft(Point pt, List<char[]> newmap, ChipsData old)
        {
            int result = pt.X;

            while (0 < result && GetMapChipString(result - 1, pt.Y, newmap).Equals(old.character))
            {
                result--;
            }
            return result;
        }

        private static int ScanRight(Point pt, List<string[]> newmap, ChipsData old)
        {
            int result = pt.X;

            while (result < Global.state.GetCSSize.x - 1 && newmap[pt.Y][result + 1].Equals(old.code))
            {
                result++;
            }
            return result;
        }

        private static int ScanRight(Point pt, List<char[]> newmap, ChipsData old)
        {
            int result = pt.X;

            while (result < Global.state.GetCSSize.x - 1 && GetMapChipString(result + 1, pt.Y, newmap).Equals(old.character))
            {
                result++;
            }
            return result;
        }

        private void UpdateLine(int left, int right, int y, ChipsData repl)
        {
            if (Global.state.Use3rdMapDataCurrently)
            {
                //マップの文字を書き換える
                for (int x = left; x <= right; x++)
                {
                    replsCode[y][x] = repl.code;
                }
            }
            else
            {
                //マップの文字を書き換える
                for (int x = left; x <= right; x++)
                {
                    for (int i = 0; i < Global.state.GetCByte; i++)
                    {
                        repls[y][x * Global.state.GetCByte + i] = repl.character[i];
                    }
                }
            }
        }

        private static void SearchLine(int left, int right, int y, List<string[]> newmap, ChipsData old, Queue<BufStr> queue)
        {
            int l = -1;
            for (int x = left; x <= right; x++)
            {
                var c = newmap[y][x];

                if (c.Equals(old.code) && l == -1)
                {
                    if (x == left)
                    {
                        l = ScanLeft(new Point(x, y), newmap, old);
                    }
                    else
                    {
                        l = x;
                    }
                }
                else if (!c.Equals(old.code) && l != -1)
                {
                    int r = x - 1;
                    queue.Enqueue(new BufStr(l, r, y));
                    l = -1;
                }
            }
            if (l != -1)
            {
                int r = ScanRight(new Point(right, y), newmap, old);
                queue.Enqueue(new BufStr(l, r, y));
            }
        }

        private static void SearchLine(int left, int right, int y, List<char[]> newmap, ChipsData old, Queue<BufStr> queue)
        {
            int l = -1;
            for (int x = left; x <= right; x++)
            {
                var c = GetMapChipString(x, y, newmap);

                if (c.Equals(old.character) && l == -1)
                {
                    if (x == left)
                    {
                        l = ScanLeft(new Point(x, y), newmap, old);
                    }
                    else
                    {
                        l = x;
                    }
                }
                else if (!c.Equals(old.character) && l != -1)
                {
                    int r = x - 1;
                    queue.Enqueue(new BufStr(l, r, y));
                    l = -1;
                }
            }
            if (l != -1)
            {
                int r = ScanRight(new Point(right, y), newmap, old);
                queue.Enqueue(new BufStr(l, r, y));
            }
        }

        private bool CheckChar(Point pt, ChipsData cd)
        {
            if (GUIDesigner.StageText.IsOverflow(pt))
            {
                return false;
            }
            if (Global.state.Use3rdMapDataCurrently)
            {
                if (replsCode[pt.Y][pt.X] != cd.code)
                {
                    return false;
                }
            }
            else
            {
                for (int i = 0; i < Global.state.GetCByte; i++)
                {
                    if (repls[pt.Y][pt.X * Global.state.GetCByte + i] != cd.character[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // マップ文字列から特定の座標の文字(String型)を取り出す。通常は1文字。レイヤーは2文字。
        private static string GetMapChipString(int x, int y, List<char[]> newmap)
        {
            var c = new char[Global.state.GetCByte];

            for (int i = 0; i < Global.state.GetCByte; i++)
            {
                c[i] = newmap[y][x * Global.state.GetCByte + i];
            }

            return new string(c);
        }
    }
}
