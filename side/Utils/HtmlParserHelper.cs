using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MasaoPlus.Utils
{
    /// <summary>
    /// HTMLファイルの解析と正男パラメータ処理を行うヘルパークラス
    /// </summary>
    public static partial class HtmlParserHelper
    {
        public static bool GetMapSource(ref string[] overwrite, string f, Runtime.DefinedData.StageSizeData StageSizeData, ref Dictionary<string, string> Params, ChipsData[] MapChip, int Split = 0)
        {
            int dxsize = StageSizeData.x, dysize = StageSizeData.y;
            string[] list = new string[dysize];
            int num = 0; // 0行目からスタート
            string NullChar = MapChip[0].character; // 空白文字（マップチップ0番の文字）

            while (num < dysize)
            {
                if (Split > 0)
                {
                    for (int num2 = 0; num2 <= Split; num2++)
                    {
                        if (list[num] == null && Params.ContainsKey(string.Format(f, num2, num)))
                            list[num] = Params[string.Format(f, num2, num)];
                        else if (Params.ContainsKey(string.Format(f, num2, num)))
                            list[num] += Params[string.Format(f, num2, num)];
                        else
                        {
                            string nullSegment = new(NullChar[0], dxsize / (Split + 1) * NullChar.Length);
                            list[num] += nullSegment;
                        }
                    }
                }
                else // 地図画面の時
                {
                    if (Params.ContainsKey(string.Format(f, num)))
                        list[num] = Params[string.Format(f, num)];
                    else list[num] = null;
                }
                num++;
            }

            if (num == dysize)
            {
                for (int i = 0; i < dysize; i++) // 空行はデフォルトの値を代入（実質地図画面専用化してるけど）
                    if (list[i] != null) overwrite[i] = list[i];

                // 文字数が足りない場合、必要なだけ補完
                for (int i = 0; i < dysize; i++)
                {
                    // 従来の形式の場合は文字で補完
                    int k = dxsize - overwrite[i].Length / NullChar.Length;
                    if (k > 0)
                    {
                        overwrite[i] += new string(NullChar[0], k * NullChar.Length);
                    }
                }

                return true;
            }
            return false;
        }

        public static string ConvertToValidJson(string jsCode)
        {
            // URLを一時的なプレースホルダーに置換
            var urlPlaceholders = new Dictionary<string, string>();
            int urlCounter = 0;
            
            // URLを一時的なプレースホルダーに置換
            var urlPattern = reg_url();
            jsCode = urlPattern.Replace(jsCode, match => {
                var placeholder = $"__URL_PLACEHOLDER_{urlCounter}__";
                urlPlaceholders[placeholder] = match.Value;
                urlCounter++;
                return placeholder;
            });

            // コメントと空行を削除
            jsCode = reg_comment_single().Replace(jsCode, "");
            jsCode = reg_comment_multi().Replace(jsCode, "");
            jsCode = reg_return().Replace(jsCode, "");
            jsCode = jsCode.Trim();

            // JavaScriptのブーリアン値をJSONのbooleanに変換
            jsCode = reg_bool_true().Replace(jsCode, "true");
            jsCode = reg_bool_false().Replace(jsCode, "false");

            // advanced-map内のattack_timingオブジェクトのキーを文字列化
            jsCode = reg_object_number().Replace(jsCode, "\"$1\"$2");

            // プロパティ名をクォートで囲む（既にクォートされている場合を除く）
            jsCode = reg_quotation().Replace(jsCode, "\"$1\"");

            // シングルクォートをダブルクォートに変換（エスケープされていないものだけ）
            var result = new StringBuilder();
            bool inString = false;
            char stringDelimiter = '"';
            
            for (int i = 0; i < jsCode.Length; i++)
            {
                char c = jsCode[i];
                if (c == '\\' && i + 1 < jsCode.Length)
                {
                    result.Append(c);
                    result.Append(jsCode[++i]);
                    continue;
                }
                
                if (c == '"' || c == '\'')
                {
                    if (!inString)
                    {
                        inString = true;
                        stringDelimiter = c;
                        result.Append('"'); // 常にダブルクォートを使用
                    }
                    else if (c == stringDelimiter)
                    {
                        inString = false;
                        result.Append('"');
                    }
                    else
                    {
                        result.Append(c);
                    }
                }
                else
                {
                    result.Append(c);
                }
            }
            jsCode = result.ToString();

            // 数値の前のプラス記号を削除（JSONでは不要）
            jsCode = reg_number_plus().Replace(jsCode, ": $1");

            // 末尾のカンマを削除する共通パターン
            jsCode = reg_trailing_comma().Replace(jsCode, "$1");

            // URLプレースホルダーを元のURLに戻す
            foreach (var placeholder in urlPlaceholders)
            {
                jsCode = jsCode.Replace(placeholder.Key, placeholder.Value);
            }

            return jsCode;
        }

        public static List<string> SplitJSMasaoArgs(string argsText)
        {
            var args = new List<string>();
            var currentArg = new StringBuilder();
            var braceCount = 0;
            var inString = false;
            var stringDelimiter = '"';

            for (int i = 0; i < argsText.Length; i++)
            {
                char c = argsText[i];

                // エスケープシーケンスの処理
                if (c == '\\' && i + 1 < argsText.Length)
                {
                    currentArg.Append(c);
                    currentArg.Append(argsText[++i]);
                    continue;
                }

                // 文字列の処理
                if (c == '"' || c == '\'')
                {
                    if (!inString)
                    {
                        inString = true;
                        stringDelimiter = c;
                    }
                    else if (c == stringDelimiter)
                    {
                        inString = false;
                    }
                    currentArg.Append(c);
                    continue;
                }

                // オブジェクトの深さを追跡
                if (!inString)
                {
                    if (c == '{' || c == '[')
                    {
                        braceCount++;
                    }
                    else if (c == '}' || c == ']')
                    {
                        braceCount--;
                    }
                    else if (c == ',' && braceCount == 0)
                    {
                        args.Add(currentArg.ToString().Trim());
                        currentArg.Clear();
                        continue;
                    }
                }

                currentArg.Append(c);
            }

            if (currentArg.Length > 0)
            {
                args.Add(currentArg.ToString().Trim());
            }

            return args;
        }

        // JSONパース前に関数リテラル（userJSCallbackやhighscoreCallbackなど）を除外するメソッド
        public static string CleanJSONForParsing(string jsonText)
        {
            // 関数リテラルを取り除くためのプロパティリスト
            string[] propertiesToRemove = [
                "userJSCallback",
                "highscoreCallback",
                "extensions"
            ];

            var cleanedJson = jsonText;
            
            // デリゲートタイプの特別処理 - より単純なアプローチを最初に試す
            foreach (var prop in propertiesToRemove)
            {
                string pattern = $@"[""|']{prop}[""|']\s*:\s*\([^)]*\)\s*=>\s*\{{.*?\}}\s*\(\)\s*,?";
                cleanedJson = Regex.Replace(cleanedJson, pattern, "", RegexOptions.Singleline);
                
                // 即時実行関数式（IIFE）を検出するための別パターン
                pattern = $@"[""|']{prop}[""|']\s*:\s*\(\s*\(\s*\)\s*=>\s*\{{.*?\}}\s*\)\(\)\s*,?";
                cleanedJson = Regex.Replace(cleanedJson, pattern, "", RegexOptions.Singleline);
                
                // 通常の関数リテラルパターン
                pattern = $@"[""|']{prop}[""|']\s*:\s*function\s*\(.*?\)\s*\{{.*?\}}\s*,?";
                cleanedJson = Regex.Replace(cleanedJson, pattern, "", RegexOptions.Singleline);
            }

            // まだプロパティが残っていれば、より詳細な分析を実施
            foreach (var prop in propertiesToRemove)
            {
                int propStart;
                while ((propStart = cleanedJson.IndexOf($"\"{prop}\"")) >= 0 || 
                       (propStart = cleanedJson.IndexOf($"'{prop}'")) >= 0)
                {
                    // プロパティの終わりを見つける
                    int colonPos = cleanedJson.IndexOf(':', propStart);
                    if (colonPos < 0) break;
                    
                    // プロパティ値の開始位置
                    int valueStart = colonPos + 1;
                    while (valueStart < cleanedJson.Length && char.IsWhiteSpace(cleanedJson[valueStart]))
                        valueStart++;
                    
                    if (valueStart >= cleanedJson.Length) break;
                    
                    // 関数リテラルの開始を検出
                    bool isFunction = false;
                    int endPos = valueStart;
                    int braceCount = 0;
                    int parenCount = 0;
                    
                    // 関数の開始パターンを確認
                    if (cleanedJson[valueStart] == '(' ||
                        cleanedJson[valueStart..].StartsWith("function") ||
                        (valueStart + 1 < cleanedJson.Length && cleanedJson.Substring(valueStart, 2) == "=>"))
                    {
                        isFunction = true;
                        
                        // 関数本体の終わりを探す
                        while (endPos < cleanedJson.Length)
                        {
                            char c = cleanedJson[endPos];
                            
                            if (c == '(')
                            {
                                parenCount++;
                            }
                            else if (c == ')')
                            {
                                parenCount--;
                                // 即時実行関数 (() => {})() の終わりを検出
                                if (parenCount == 0 && braceCount == 0)
                                {
                                    int nextPos = endPos + 1;
                                    while (nextPos < cleanedJson.Length && char.IsWhiteSpace(cleanedJson[nextPos]))
                                        nextPos++;
                                    
                                    if (nextPos < cleanedJson.Length && cleanedJson[nextPos] == '(')
                                    {
                                        // 即時実行関数の終わりまでスキップ
                                        endPos = FindMatchingCloseParen(cleanedJson, nextPos);
                                        if (endPos < 0) break; // 対応する括弧がない場合
                                        endPos++;
                                    }
                                }
                            }
                            else if (c == '{')
                            {
                                braceCount++;
                            }
                            else if (c == '}')
                            {
                                braceCount--;
                                if (braceCount == 0 && parenCount <= 0)
                                {
                                    endPos++;
                                    break; // 関数本体の終わりを見つけた
                                }
                            }
                            
                            endPos++;
                            
                            // すべての括弧が閉じられた後にコンマや括弧を探す
                            if (braceCount == 0 && parenCount == 0 && endPos < cleanedJson.Length)
                            {
                                // 空白をスキップ
                                while (endPos < cleanedJson.Length && char.IsWhiteSpace(cleanedJson[endPos]))
                                    endPos++;
                                
                                if (endPos < cleanedJson.Length)
                                {
                                    if (cleanedJson[endPos] == '(')
                                    {
                                        // 即時実行関数の呼び出し括弧
                                        int closePos = FindMatchingCloseParen(cleanedJson, endPos);
                                        if (closePos >= 0)
                                        {
                                            endPos = closePos + 1;
                                        }
                                        
                                        // 括弧の後に空白をスキップ
                                        while (endPos < cleanedJson.Length && char.IsWhiteSpace(cleanedJson[endPos]))
                                            endPos++;
                                    }
                                    
                                    // カンマがあれば、それも含める
                                    if (endPos < cleanedJson.Length && cleanedJson[endPos] == ',')
                                    {
                                        endPos++;
                                    }
                                    
                                    break;
                                }
                            }
                        }
                    }
                    
                    if (isFunction)
                    {
                        // プロパティ全体を削除
                        cleanedJson = cleanedJson.Remove(propStart, endPos - propStart);
                    }
                    else
                    {
                        // 関数でなければ、このプロパティはスキップ
                        break;
                    }
                }
            }
            
            // 構文の修正（連続カンマ、末尾カンマの除去など）
            cleanedJson = reg_trailing_comma().Replace(cleanedJson, "$1"); // 末尾カンマの除去（オブジェクトと配列）
            cleanedJson = reg_double_comma().Replace(cleanedJson, ","); // 連続カンマの除去
            cleanedJson = reg_object_start_comma().Replace(cleanedJson, "{"); // オブジェクト開始直後のカンマの除去
            
            return cleanedJson;
        }
        
        // 対応する閉じ括弧の位置を見つける補助メソッド
        public static int FindMatchingCloseParen(string text, int openPos)
        {
            int count = 1;
            for (int i = openPos + 1; i < text.Length; i++)
            {
                if (text[i] == '(') count++;
                else if (text[i] == ')') count--;
                
                if (count == 0) return i; // 対応する閉じ括弧を見つけた
            }
            return -1; // 見つからなかった
        }

        // Jsonパース失敗時の処理を共通関数化
        public static void ParseParamsWithRegex(string input, Dictionary<string, string> dictionary)
        {
            var regex2 = reg_script_param();
            Match match2 = regex2.Match(input);
            while (match2.Success)
            {
                dictionary[match2.Groups["name"].Value] = match2.Groups["value"].Value;
                match2 = match2.NextMatch();
            }
        }

        // JavaScript文字列からパラメータを抽出する共通処理を関数化
        public static List<string> PrepareJavaScriptCode(string jsContent)
        {
            string jsCode = jsContent.Trim();
            
            // '(' から始まる部分を検出して、それ以前の文字列を削除
            int openBracketIndex = jsCode.IndexOf('(');
            if (openBracketIndex >= 0)
            {
                jsCode = jsCode[openBracketIndex..];
                // 先頭の括弧を削除
                jsCode = jsCode.TrimStart('(');
            }
            
            // 最後の行を確認してJSMasao.pad.avoidAD = true;のような追加コードがないか確認
            int lastSemicolonPos = jsCode.LastIndexOf(';');
            int lastCloseBracePos = jsCode.LastIndexOf("});");
            
            // });の後にコードがある場合はそれを除外
            if (lastCloseBracePos > 0 && lastSemicolonPos > lastCloseBracePos)
            {
                jsCode = jsCode[..(lastCloseBracePos + 1)];
            }
            // 末尾が);なら除去
            else if (jsCode.EndsWith(");"))
            {
                jsCode = jsCode[..^2];
            }

            // 引数を分割して返す
            return SplitJSMasaoArgs(jsCode);
        }

        // 抽出したJavaScript引数からパラメータ辞書にデータを追加する共通処理
        public static void ProcessJavaScriptArgs(List<string> args, Dictionary<string, string> dictionary, string input)
        {
            if (args.Count > 0)
            {
                // 第1引数の処理（基本設定）
                string jsonText = ConvertToValidJson(args[0]);
                try
                {
                    var jsonDoc = JsonDocument.Parse(jsonText);
                    foreach (var prop in jsonDoc.RootElement.EnumerateObject())
                    {
                        dictionary[prop.Name] = prop.Value.ToString();
                    }
                }
                catch (JsonException)
                {
                    // JSON解析に失敗した場合は従来の正規表現による解析を試みる
                    ParseParamsWithRegex(input, dictionary);
                }

                // 第2引数以降の処理（advanced-map等）
                for (int argIndex = 1; argIndex < args.Count; argIndex++)
                {
                    if (args[argIndex].Trim() == "null") continue;

                    jsonText = ConvertToValidJson(args[argIndex]);
                    try
                    {
                        // JSONパースする前にuserJSCallbackなど関数リテラルを含むプロパティを除外
                        string jsonText2 = CleanJSONForParsing(jsonText);
                        var jsonDoc = JsonDocument.Parse(jsonText2);
                        foreach (var prop in jsonDoc.RootElement.EnumerateObject())
                        {
                            dictionary[prop.Name] = prop.Value.ToString();
                        }
                        // 1つでも成功したらループを抜ける
                        break;
                    }
                    catch (JsonException)
                    {
                        // JSON解析に失敗した場合は従来の正規表現による解析を試みる
                        ParseParamsWithRegex(input, dictionary);
                    }
                }
            }
        }

        [GeneratedRegex("^.*?<[ ]*?APPLET .*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline, "ja-JP")]
        public static partial Regex reg_applet_start();
        [GeneratedRegex("<[ ]*?/[ ]*?APPLET[ ]*?>.*$", RegexOptions.IgnoreCase | RegexOptions.Singleline, "ja-JP")]
        public static partial Regex reg_applet_end();
        [GeneratedRegex(@"<[ ]*PARAM[ ]+NAME=""(?<name>.*?)""[ ]+VALUE=""(?<value>.*?)"".*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline, "ja-JP")]
        public static partial Regex reg_param();
        [GeneratedRegex(@"(?:<[ ]*?script.*?>.*?)?new\s*?(JSMasao|CanvasMasao\.\s*?Game)", RegexOptions.IgnoreCase | RegexOptions.Singleline, "ja-JP")]
        public static partial Regex reg_script_start();
        [GeneratedRegex(@"<script[^>]*>(.*?)<\/script>", RegexOptions.IgnoreCase | RegexOptions.Singleline, "ja-JP")]
        public static partial Regex reg_script_tag();
        [GeneratedRegex(@"(""|')(?<name>.*?)(""|')\s*?:\s*?(""|')(?<value>.*?)(?<!\\)(""|')(,|\s*?)", RegexOptions.IgnoreCase | RegexOptions.Singleline, "ja-JP")]
        public static partial Regex reg_script_param();
        [GeneratedRegex(@"-(\d+)$")]
        public static partial Regex reg_text_name();
        [GeneratedRegex(@"(?<=[""'])(https?://[^""']+)(?=[""'])")]
        public static partial Regex reg_url();
        [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
        public static partial Regex reg_comment_single();
        [GeneratedRegex(@"//.*?$", RegexOptions.Multiline)]
        public static partial Regex reg_comment_multi();
        [GeneratedRegex(@"^\s*$[\r\n]*", RegexOptions.Multiline)]
        public static partial Regex reg_return();
        [GeneratedRegex(@"\btrue\b")]
        public static partial Regex reg_bool_true();
        [GeneratedRegex(@"\bfalse\b")]
        public static partial Regex reg_bool_false();
        [GeneratedRegex(@"(\d+)(\s*:)")]
        public static partial Regex reg_object_number();
        [GeneratedRegex(@"(?<![""|'])(\b[a-zA-Z_][a-zA-Z0-9_-]*\b)(?=\s*:)")]
        public static partial Regex reg_quotation();
        [GeneratedRegex(@":\s*\+(\d+)")]
        public static partial Regex reg_number_plus();
        [GeneratedRegex(@",(\s*[}\]])")]
        public static partial Regex reg_trailing_comma();
        [GeneratedRegex(",\\s*,")]
        public static partial Regex reg_double_comma();
        [GeneratedRegex("{\\s*,")]
        public static partial Regex reg_object_start_comma();
    }
}
