using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MasaoPlus.Utils;

namespace MasaoPlus.Dialogs
{
    public partial class HTMLInheritance : Form
    {
        public HTMLInheritance(string pf)
        {
            InitializeComponent();
            ParseFile = pf;
        }

        private void Inheritance_Load(object sender, EventArgs e)
        {
            TargetFile.Text = ParseFile;
            ProjectName.Text = Path.GetFileNameWithoutExtension(ParseFile);
            RootDir.Text = Global.config.lastData.ProjDirF;
            OK.Text = "お待ちください...";
            OK.Enabled = false;
        }

        private void Inheritance_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            if (!Directory.Exists(Path.Combine(Application.StartupPath, Global.definition.RuntimeDir)))
            {
                MessageBox.Show($"ランタイムフォルダが見つかりません。{Environment.NewLine}Sideを再インストールしてください。", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }
            string[] files = Directory.GetFiles(Path.Combine(Application.StartupPath, Global.definition.RuntimeDir), "*.xml", SearchOption.TopDirectoryOnly);
            foreach (string text in files)
            {
                try
                {
                    string text2 = Path.Combine(Path.GetDirectoryName(text), Path.GetFileNameWithoutExtension(text));
                    if (Directory.Exists(text2))
                    {
                        Runtime runtime = Runtime.ParseXML(text);
                        if (runtime != null)
                        {
                            string[] array2 = Runtime.CheckFiles(text2, runtime);
                            if (array2.Length == 0)
                            {
                                runtimes.Add(text);
                                runtimedatas.Add(runtime);
                                runtimeuselayer.Add(runtime.Definitions.LayerSize.bytesize != 0);
                                RuntimeSet.Items.Add(string.Concat(
                                    runtime.Definitions.Name,
                                    " [Author:",
                                    runtime.Definitions.Author,
                                    " Layer:",
                                    (runtime.Definitions.LayerSize.bytesize != 0) ? "○" : "×",
                                    "] : ",
                                    Path.GetFileName(text)
                                ));
                            }
                            else
                            {
                                MessageBox.Show($"必須ファイルが欠落しています。{Environment.NewLine}{Path.GetFileName(text)} : {string.Join(",", array2)}", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show($"ランタイムフォルダが見つかりません:{Path.GetFileName(text)}", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"読み込めませんでした:{Path.GetFileName(text)}{Environment.NewLine}{ex.Message}", "ランタイム定義エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            if (RuntimeSet.Items.Count == 0)
            {
                MessageBox.Show("利用可能なランタイムがありません。", "ランタイムロードエラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                DialogResult = DialogResult.Cancel;
                Close();
            }
            else
            {
                RuntimeSet.SelectedIndex = 0;
            }
            OK.Enabled = true;
            OK.Text = "OK";
        }

        private void RootDirBrowse_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.Description = "プロジェクトのルートディレクトリを選択してください。";
            folderBrowserDialog.SelectedPath = RootDir.Text;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                RootDir.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void ProjectName_TextChanged(object sender, EventArgs e)
        {
            ValidationText();
        }

        private void RootDir_TextChanged(object sender, EventArgs e)
        {
            ValidationText();
        }

        private void ValidationText()
        {
            if (ProjectName.Text == "")
            {
                OK.Enabled = false;
                return;
            }
            if (RootDir.Text == "" || !Directory.Exists(RootDir.Text))
            {
                OK.Enabled = false;
                return;
            }
            OK.Enabled = true;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            try
            {
                Enabled = false;
                OK.Text = "生成中...";
                OK.Refresh();
                StatusText.Text = "プロジェクト生成準備中...";
                StatusText.Refresh();
                Global.config.lastData.ProjDir = RootDir.Text;
                string text = Path.Combine(RootDir.Text, ProjectName.Text);
                if (Directory.Exists(text) && MessageBox.Show($"ディレクトリ{Environment.NewLine}\"{text}\"{Environment.NewLine}はすでに存在します。{Environment.NewLine}中に含まれるファイルは上書きされてしまう事があります。{Environment.NewLine}続行しますか？", "プロジェクト生成警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    return;
                }
                ProjectFile = Path.Combine(text, ProjectName.Text + Global.definition.ProjExt);
                string text2 = Path.Combine(Path.GetDirectoryName(runtimes[RuntimeSet.SelectedIndex]), Path.GetFileNameWithoutExtension(runtimes[RuntimeSet.SelectedIndex]));
                Project project = new()
                {
                    Name = ProjectName.Text,
                    Runtime = runtimedatas[RuntimeSet.SelectedIndex],
                    Config = ConfigurationOwner.LoadXML(Path.Combine(text2, runtimedatas[RuntimeSet.SelectedIndex].Definitions.Configurations))
                };
                ChipDataClass chipDataClass = Project.SetAllStageData(project, text2);
                StatusText.Text = "HTMLデータ取得準備中...";
                StatusText.Refresh();
                string input = "";
                try
                {
                    input = Subsystem.LoadUnknownTextFile(ParseFile);
                }
                catch
                {
                    MessageBox.Show("ファイルをロードできませんでした。", "コンバート失敗", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
                List<string> list = [];
                if (SeekHeaderFooter.Checked)
                {
                    Regex regex = HtmlParserHelper.reg_applet_start();
                    Match match = regex.Match(input);
                    if (match.Success)
                    {
                        project.Runtime.DefaultConfigurations.HeaderHTML = match.Value;
                    }
                    regex = HtmlParserHelper.reg_applet_end();
                    match = regex.Match(input);
                    if (match.Success)
                    {
                        project.Runtime.DefaultConfigurations.FooterHTML = match.Value;
                    }
                }
                StatusText.Text = "HTMLデータ取得中...";
                StatusText.Refresh();

                Dictionary<string, string> dictionary = [];

                // JavaScriptのパラメータを解析
                Regex regex_js = HtmlParserHelper.reg_script_start();
                Match js_match = regex_js.Match(input);
                if (js_match.Success)
                {
                    // JavaScriptのコードを取得
                    int jsStart = js_match.Index + js_match.Length;
                    int jsEnd = input.IndexOf("</script>", jsStart);
                    if (jsEnd == -1) // scriptタグが見つからない場合（外部JSファイル）は最後まで読む
                    {
                        jsEnd = input.Length;
                        StatusText.Text = "JSデータ取得中...";
                        StatusText.Refresh();
                    }
                    if (jsEnd > jsStart)
                    {
                        string jsCode = input[jsStart..jsEnd].Trim();
                        // 共通関数を使用してJavaScriptコードを処理
                        var args = HtmlParserHelper.PrepareJavaScriptCode(jsCode);
                        HtmlParserHelper.ProcessJavaScriptArgs(args, dictionary, input);
                    }
                }
                else
                {
                    // スクリプトタグ内容全体から正男パラメータを探す
                    Regex regexScriptTag = HtmlParserHelper.reg_script_tag();
                    foreach (Match scriptMatch in regexScriptTag.Matches(input))
                    {
                        string scriptContent = scriptMatch.Groups[1].Value;

                        foreach (var param in HtmlParserHelper.CommonParams)
                        {
                            if (scriptContent.Contains("\"" + param) || scriptContent.Contains("'" + param))
                            {
                                // 正男パラメータを含むスクリプトを発見したので解析
                                StatusText.Text = "スクリプト内のパラメータ解析中...";
                                StatusText.Refresh();

                                // 共通関数を使用してJavaScriptコードを処理
                                var args = HtmlParserHelper.PrepareJavaScriptCode(scriptContent);
                                HtmlParserHelper.ProcessJavaScriptArgs(args, dictionary, input);

                                break; // 正男パラメータを含むスクリプトが見つかったらループを抜ける
                            }
                        }
                    }

                    // JSONファイル形式の検出と解析
                    try
                    {
                        // BOMを削除
                        if (input.StartsWith('\ufeff'))
                        {
                            input = input[1..];
                        }
                        rootJsonElement = JsonDocument.Parse(input).RootElement;

                        // フォーマットバージョンの確認
                        if (rootJsonElement.TryGetProperty("masao-json-format-version", out var formatVersion))
                        {
                            StatusText.Text = "JSONデータ取得中...";
                            StatusText.Refresh();
                            // パラメータの読み込み
                            if (rootJsonElement.TryGetProperty("params", out var parameters))
                            {
                                foreach (var param in parameters.EnumerateObject())
                                {
                                    dictionary[param.Name] = param.Value.ToString();
                                }
                            }

                            // メタデータの読み込み
                            if (rootJsonElement.TryGetProperty("metadata", out var metadata))
                            {
                                if (metadata.TryGetProperty("title", out var title))
                                {
                                    if (project.Runtime.DefaultConfigurations.OutputReplace?.Any(r => r.Name == "タイトル") == true)
                                    {
                                        var titleReplace = project.Runtime.DefaultConfigurations.OutputReplace.First(r => r.Name == "タイトル");
                                        titleReplace.Value = title.ToString();
                                    }
                                }
                            }

                            // advanced-mapの読み込み
                            if (rootJsonElement.TryGetProperty("advanced-map", out var advancedMap) && advancedMap.ValueKind != JsonValueKind.Null)
                            {
                                dictionary["advanced-map"] = advancedMap.ToString();
                            }

                            // スクリプトの読み込み
                            if (rootJsonElement.TryGetProperty("script", out var script) && script.ValueKind != JsonValueKind.Null)
                            {
                                var scriptContent = $@"<script>
{script}
</script>";

                                // HeaderHTMLからhead要素を探す
                                var headerHtml = project.Runtime.DefaultConfigurations.HeaderHTML;
                                if (headerHtml.Contains("<head>"))
                                {
                                    // head要素が存在する場合、</head>の直前にスクリプトを挿入
                                    int headEndIndex = headerHtml.IndexOf("</head>");
                                    if (headEndIndex != -1)
                                    {
                                        project.Runtime.DefaultConfigurations.HeaderHTML =
                                            headerHtml.Insert(headEndIndex, scriptContent);
                                    }
                                }
                                else
                                {
                                    // head要素がない場合、HTMLの先頭にhead要素を作成してスクリプトを追加
                                    var headElement = $@"<head>
<meta charset=""UTF-8"">
{scriptContent}
</head>

";
                                    // <!DOCTYPE>がある場合はその後に挿入
                                    if (headerHtml.Contains("<!DOCTYPE"))
                                    {
                                        int doctypeEndIndex = headerHtml.IndexOf('>', headerHtml.IndexOf("<!DOCTYPE")) + 1;
                                        project.Runtime.DefaultConfigurations.HeaderHTML =
                                            headerHtml.Insert(doctypeEndIndex, Environment.NewLine + headElement);
                                    }
                                    else
                                    {
                                        project.Runtime.DefaultConfigurations.HeaderHTML = headElement + headerHtml;
                                    }
                                }
                            }
                        }
                    }
                    catch (JsonException)
                    {
                        // JSONとして解析できない場合は従来のHTMLパラメータ解析を試みる
                        HtmlParserHelper.ParseParamsFromHTML(input, dictionary);
                    }
                }

                StatusText.Text = "マップソース生成中...";
                StatusText.Refresh();
                HtmlParserHelper.GetMapSource(ref project.MapData.Strings, project.Runtime.Definitions.MapName, project.Runtime.Definitions.MapSize, ref dictionary, chipDataClass.WorldChip);

                if (dictionary.ContainsKey("advanced-map") || dictionary.ContainsKey("advance-map"))
                {
                    string mapData = dictionary.TryGetValue("advanced-map", out string value) ? value : dictionary["advance-map"];

                    try
                    {
                        StatusText.Text = "カスタムパーツ解析中...";
                        StatusText.Refresh();
                        // JavaScriptの文字列をJSONに変換
                        mapData = mapData.Trim();
                        if (mapData.StartsWith('\'') || mapData.StartsWith('\"'))
                        {
                            mapData = mapData[1..^1]; // 最初と最後のクォートを削除
                            mapData = mapData.Replace(@"\""", @"""").Replace(@"\\", @"\").Replace("¥", "\\");
                        }

                        var jsonElement = JsonDocument.Parse(mapData).RootElement;

                        // カスタムパーツ定義の読み込み
                        if (jsonElement.TryGetProperty("customParts", out var customParts))
                        {
                            var customPartsArray = customParts.EnumerateObject().ToArray();
                            var customPartsList = new List<ChipsData>();
                            for (int i = 0; i < customPartsArray.Length; i++)
                            {
                                var part = customPartsArray[i];
                                var chipData = new ChipsData
                                {
                                    code = part.Name,  // カスタムパーツのID
                                    basecode = part.Value.GetProperty("extends").ToString() // 継承元の基本パーツID
                                };

                                // baseCodeに対応するチップ定義を探してプロパティをコピー
                                var baseChip = chipDataClass.VarietyChip?.FirstOrDefault(x => x.code == chipData.basecode);
                                if (baseChip != null)
                                {
                                    chipData.color = baseChip?.color;
                                    chipData.relation = baseChip?.relation;
                                    chipData.idColor = $"#{Guid.NewGuid().ToString("N")[..6]}";

                                    // _meme_coreからカスタムパーツ名を取得
                                    var customPartName = string.Empty;
                                    try
                                    {
                                        if (rootJsonElement.TryGetProperty("_meme_core", out var memeCore) &&
                                            memeCore.TryGetProperty("customParts", out var memeCustomParts) &&
                                            memeCustomParts.TryGetProperty(part.Name, out var memePart) &&
                                            memePart.TryGetProperty("name", out var memeName))
                                        {
                                            customPartName = memeName.GetString();
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        // エラーが発生した場合は無視する
                                    }
                                    // Chipsプロパティを新しい配列として複製
                                    chipData.Chips = new ChipData[(int)(baseChip?.Chips.Length)];
                                    for (int j = 0; j < baseChip?.Chips.Length; j++)
                                    {
                                        chipData.Chips[j] = (ChipData)(baseChip?.Chips[j]);
                                        chipData.Chips[j].name = customPartName.Length > 0 ? customPartName : $"カスタムパーツ{i + 1}";
                                    }
                                }

                                // propertiesを持つ場合の処理
                                if (part.Value.TryGetProperty("properties", out var properties))
                                {
                                    chipData.Properties = new CustomPartsProperties();
                                    foreach (var prop in properties.EnumerateObject())
                                    {
                                        switch (prop.Name)
                                        {
                                            case "walk_speed":
                                                chipData.Properties.walk_speed = prop.Value.GetInt32();
                                                break;
                                            case "fall_speed":
                                                chipData.Properties.fall_speed = prop.Value.GetInt32();
                                                break;
                                            case "jump_vy":
                                                chipData.Properties.jump_vy = prop.Value.GetInt32();
                                                break;
                                            case "search_range":
                                                chipData.Properties.search_range = prop.Value.GetInt32();
                                                break;
                                            case "interval":
                                                chipData.Properties.interval = prop.Value.GetInt32();
                                                break;
                                            case "period":
                                                chipData.Properties.period = prop.Value.GetInt32();
                                                break;
                                            case "attack_timing":
                                                chipData.Properties.attack_timing = [];
                                                foreach (var timing in prop.Value.EnumerateObject())
                                                {
                                                    chipData.Properties.attack_timing.Add(new attack_timing
                                                    {
                                                        AttackFrame = int.Parse(timing.Name),
                                                        IsPlaySoundFrame = timing.Value.GetInt32() == 2
                                                    });
                                                }
                                                break;
                                            case "speed":
                                                chipData.Properties.speed = prop.Value.GetInt32();
                                                break;
                                            case "accel":
                                                chipData.Properties.accel = prop.Value.GetInt32();
                                                break;
                                            case "distance":
                                                chipData.Properties.distance = prop.Value.GetInt32();
                                                break;
                                            case "attack_speed":
                                                chipData.Properties.attack_speed = prop.Value.GetInt32();
                                                break;
                                            case "return_speed":
                                                chipData.Properties.return_speed = prop.Value.GetInt32();
                                                break;
                                            case "speed_x":
                                                chipData.Properties.speed_x = prop.Value.GetInt32();
                                                break;
                                            case "speed_y":
                                                chipData.Properties.speed_y = prop.Value.GetInt32();
                                                break;
                                            case "radius":
                                                chipData.Properties.radius = prop.Value.GetInt32();
                                                break;
                                            case "init_vy":
                                                chipData.Properties.init_vy = prop.Value.GetInt32();
                                                break;
                                        }
                                    }
                                }
                                customPartsList.Add(chipData);
                            }
                            project.CustomPartsDefinition = [.. customPartsList];
                        }

                        StatusText.Text = "ステージデータ変換中...";
                        StatusText.Refresh();
                        // ステージデータの読み込み
                        project.Use3rdMapData = true;

                        // ステージデータの初期化処理
                        static void InitializeEmptyStageData(LayerObject data, int width, ChipsData[] chips)
                        {
                            var defaultCode = ChipDataClass.CharToCode(chips[0].character);
                            for (int y = 0; y < data.Length; y++)
                            {
                                var row = new int[width];
                                for (int x = 0; x < width; x++)
                                {
                                    row[x] = int.Parse(defaultCode);
                                }
                                data[y] = string.Join(",", row);
                            }
                        }

                        // ステージデータの読み込みとサイズの設定を一元化
                        var stages = jsonElement.TryGetProperty("stages", out var stagesElement) ?
                            [.. stagesElement.EnumerateArray()] :
                            new List<JsonElement>();

                        // ステージ配列の長さを4にする処理
                        if (stages.Count < 4)
                        {
                            stages.AddRange(Enumerable.Repeat(default(JsonElement), 4 - stages.Count));
                        }

                        StatusText.Text = "ステージサイズ設定中...";
                        StatusText.Refresh();
                        for (int stageIndex = 0; stageIndex < stages.Count; stageIndex++)
                        {
                            var stage = stages[stageIndex];
                            int layerCount = 0;
                            JsonElement layers = default;
                            try
                            {
                                if (stage.TryGetProperty("size", out var size))
                                {
                                    int sizeX = size.GetProperty("x").GetInt32();
                                    int sizeY = size.GetProperty("y").GetInt32();
                                    int mainOrder = 0;
                                    string mainSrc = null;
                                    string[] layerSrc = null;

                                    // マップサイズが500x500を超える場合は警告して500に丸める
                                    if (sizeX > Global.state.MaximumStageSize.Width || sizeY > Global.state.MaximumStageSize.Height)
                                    {
                                        if (sizeX > Global.state.MaximumStageSize.Width)
                                        {
                                            sizeX = Global.state.MaximumStageSize.Width;
                                        }
                                        if (sizeY > Global.state.MaximumStageSize.Height)
                                        {
                                            sizeY = Global.state.MaximumStageSize.Height;
                                        }
                                        MessageBox.Show($"マップサイズがSideで扱える最大値({Global.state.MaximumStageSize.Width}×{Global.state.MaximumStageSize.Height})を超えています。\nサイズを{Global.state.MaximumStageSize.Width}×{Global.state.MaximumStageSize.Height}に制限します。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    try
                                    {
                                        layerCount = stage.TryGetProperty("layers", out layers) ? layers.GetArrayLength() : 0;
                                        if (layerCount > 1) layerSrc = new string[layerCount - 1];
                                        if (stage.TryGetProperty("layers", out layers))
                                        {
                                            for (int i = 0, j = 0; i < layerCount; i++)
                                            {
                                                var layer = layers[i];
                                                var type = layer.GetProperty("type").GetString();
                                                var src = layer.GetProperty("src").GetString();

                                                if (type == "main")
                                                {
                                                    mainOrder = i;
                                                    mainSrc = src;
                                                }
                                                else
                                                {
                                                    layerSrc[j++] = src;
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    // ステージ番号に応じてサイズとデータ配列を更新
                                    switch (stageIndex)
                                    {
                                        case 0:
                                            project.Runtime.Definitions.StageSize.x = sizeX;
                                            project.Runtime.Definitions.StageSize.y = sizeY;
                                            project.Runtime.Definitions.StageSize.mainPattern.Value = mainSrc;
                                            project.StageData = [.. new string[sizeY]];
                                            if (project.Runtime.Definitions.LayerSize.bytesize != 0)
                                            {
                                                project.Runtime.Definitions.LayerSize.x = sizeX;
                                                project.Runtime.Definitions.LayerSize.y = sizeY;
                                                project.Runtime.Definitions.LayerSize.mainOrder = mainOrder;
                                                if (layerCount > 1)
                                                {
                                                    project.LayerData.Clear();
                                                    for (int i = 0; i < layerCount - 1; i++)
                                                    {
                                                        project.Runtime.Definitions.LayerSize.mapchips.Add(new Runtime.DefinedData.StageSizeData.LayerObject { Value = layerSrc[i] });
                                                        project.LayerData.Add([.. new string[sizeY]]);
                                                    }
                                                }
                                            }
                                            break;
                                        case 1:
                                            project.Runtime.Definitions.StageSize2.x = sizeX;
                                            project.Runtime.Definitions.StageSize2.y = sizeY;
                                            project.Runtime.Definitions.StageSize2.mainPattern.Value = mainSrc;
                                            project.StageData2 = [.. new string[sizeY]];
                                            if (project.Runtime.Definitions.LayerSize.bytesize != 0)
                                            {
                                                project.Runtime.Definitions.LayerSize2.x = sizeX;
                                                project.Runtime.Definitions.LayerSize2.y = sizeY;
                                                project.Runtime.Definitions.LayerSize2.mainOrder = mainOrder;
                                                if (layerCount > 1)
                                                {
                                                    project.LayerData2.Clear();
                                                    for (int i = 0; i < layerCount - 1; i++)
                                                    {
                                                        project.Runtime.Definitions.LayerSize2.mapchips.Add(new Runtime.DefinedData.StageSizeData.LayerObject { Value = layerSrc[i] });
                                                        project.LayerData2.Add([.. new string[sizeY]]);
                                                    }
                                                }
                                            }
                                            break;
                                        case 2:
                                            project.Runtime.Definitions.StageSize3.x = sizeX;
                                            project.Runtime.Definitions.StageSize3.y = sizeY;
                                            project.Runtime.Definitions.StageSize3.mainPattern.Value = mainSrc;
                                            project.StageData3 = [.. new string[sizeY]];
                                            if (project.Runtime.Definitions.LayerSize.bytesize != 0)
                                            {
                                                project.Runtime.Definitions.LayerSize3.x = sizeX;
                                                project.Runtime.Definitions.LayerSize3.y = sizeY;
                                                project.Runtime.Definitions.LayerSize3.mainOrder = mainOrder;
                                                if (layerCount > 1)
                                                {
                                                    project.LayerData3.Clear();
                                                    for (int i = 0; i < layerCount - 1; i++)
                                                    {
                                                        project.Runtime.Definitions.LayerSize3.mapchips.Add(new Runtime.DefinedData.StageSizeData.LayerObject { Value = layerSrc[i] });
                                                        project.LayerData3.Add([.. new string[sizeY]]);
                                                    }
                                                }
                                            }
                                            break;
                                        case 3:
                                            project.Runtime.Definitions.StageSize4.x = sizeX;
                                            project.Runtime.Definitions.StageSize4.y = sizeY;
                                            project.Runtime.Definitions.StageSize4.mainPattern.Value = mainSrc;
                                            project.StageData4 = [.. new string[sizeY]];
                                            if (project.Runtime.Definitions.LayerSize.bytesize != 0)
                                            {
                                                project.Runtime.Definitions.LayerSize4.x = sizeX;
                                                project.Runtime.Definitions.LayerSize4.y = sizeY;
                                                project.Runtime.Definitions.LayerSize4.mainOrder = mainOrder;
                                                if (layerCount > 1)
                                                {
                                                    project.LayerData4.Clear();
                                                    for (int i = 0; i < layerCount - 1; i++)
                                                    {
                                                        project.Runtime.Definitions.LayerSize4.mapchips.Add(new Runtime.DefinedData.StageSizeData.LayerObject { Value = layerSrc[i] });
                                                        project.LayerData4.Add([.. new string[sizeY]]);
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }

                            // 指定サイズでデフォルト値を設定
                            LayerObject targetStageData = null;
                            LayerObject[] targetLayerData = null;
                            ChipsData[] targetChips = chipDataClass.Mapchip;
                            ChipsData[] targetLayerChips = chipDataClass.Layerchip;
                            int width = default;

                            switch (stageIndex)
                            {
                                case 0:
                                    targetStageData = project.StageData;
                                    if (layerCount > 1)
                                    {
                                        targetLayerData = new LayerObject[layerCount - 1];
                                        for (int i = 0; i < layerCount - 1; i++)
                                        {
                                            targetLayerData[i] = project.LayerData[i];
                                        }
                                    }
                                    width = project.Runtime.Definitions.StageSize.x;
                                    break;
                                case 1:
                                    targetStageData = project.StageData2;
                                    if (layerCount > 1)
                                    {
                                        targetLayerData = new LayerObject[layerCount - 1];
                                        for (int i = 0; i < layerCount - 1; i++)
                                        {
                                            targetLayerData[i] = project.LayerData2[i];
                                        }
                                    }
                                    width = project.Runtime.Definitions.StageSize2.x;
                                    break;
                                case 2:
                                    targetStageData = project.StageData3;
                                    if (layerCount > 1)
                                    {
                                        targetLayerData = new LayerObject[layerCount - 1];
                                        for (int i = 0; i < layerCount - 1; i++)
                                        {
                                            targetLayerData[i] = project.LayerData3[i];
                                        }
                                    }
                                    width = project.Runtime.Definitions.StageSize3.x;
                                    break;
                                case 3:
                                    targetStageData = project.StageData4;
                                    if (layerCount > 1)
                                    {
                                        targetLayerData = new LayerObject[layerCount - 1];
                                        for (int i = 0; i < layerCount - 1; i++)
                                        {
                                            targetLayerData[i] = project.LayerData4[i];
                                        }
                                    }
                                    width = project.Runtime.Definitions.StageSize4.x;
                                    break;
                            }

                            // メインステージのデフォルト値設定
                            InitializeEmptyStageData(targetStageData, width, targetChips);

                            // レイヤーのデフォルト値設定
                            if (project.Runtime.Definitions.LayerSize.bytesize != 0 && targetLayerData != null)
                            {
                                for (int i = 0; i < layerCount - 1; i++)
                                {
                                    InitializeEmptyStageData(targetLayerData[i], width, targetLayerChips);
                                }
                            }

                            StatusText.Text = $"ステージソース生成中[{stageIndex + 1}/4]...";
                            StatusText.Refresh();
                            // レイヤーデータの読み込み
                            try
                            {
                                int layerIndex = 0;
                                if (stage.TryGetProperty("layers", out layers))
                                {
                                    foreach (var layer in layers.EnumerateArray())
                                    {
                                        var type = layer.GetProperty("type").GetString();
                                        var map = layer.GetProperty("map");

                                        // レイヤーの種類に応じてターゲットデータを選択
                                        LayerObject targetData = type == "main" ? targetStageData : targetLayerData[layerIndex++];
                                        var chips = type == "main" ? targetChips : targetLayerChips;
                                        var defaultCode = ChipDataClass.CharToCode(chips[0].character);

                                        if (targetData != null)
                                        {
                                            for (int y = 0; y < map.GetArrayLength() && y < targetData.Length; y++)
                                            {
                                                var row = map[y];
                                                var rowValues = new List<object>();

                                                // マップデータの各セルを処理
                                                foreach (var cell in row.EnumerateArray())
                                                {
                                                    if (cell.ValueKind == JsonValueKind.Number && cell.TryGetInt32(out int cellvalue))
                                                    {
                                                        rowValues.Add(cellvalue);
                                                    }
                                                    else if (cell.ValueKind == JsonValueKind.String)
                                                    {
                                                        // 文字列の場合はそのまま追加
                                                        rowValues.Add(cell.GetString());
                                                    }
                                                    else
                                                    {
                                                        // それ以外はデフォルト値
                                                        rowValues.Add(int.Parse(defaultCode));
                                                    }
                                                }

                                                // 幅が足りない場合はデフォルト値で補完
                                                while (rowValues.Count < width)
                                                {
                                                    rowValues.Add(int.Parse(defaultCode));
                                                }

                                                targetData[y] = string.Join(",", rowValues);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"マップデータの解析に失敗しました。{Environment.NewLine}{ex.Message}",
                            "マップデータ解析エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    StatusText.Text = "ステージソース生成中[1/4]...";
                    StatusText.Refresh();
                    HtmlParserHelper.GetMapSource(ref project.StageData.Strings, project.Runtime.Definitions.ParamName, project.Runtime.Definitions.StageSize, ref dictionary, chipDataClass.Mapchip, project.Runtime.Definitions.StageSplit);
                    StatusText.Text = "ステージソース生成中[2/4]...";
                    StatusText.Refresh();
                    HtmlParserHelper.GetMapSource(ref project.StageData2.Strings, project.Runtime.Definitions.ParamName2, project.Runtime.Definitions.StageSize2, ref dictionary, chipDataClass.Mapchip, project.Runtime.Definitions.StageSplit);
                    StatusText.Text = "ステージソース生成中[3/4]...";
                    StatusText.Refresh();
                    HtmlParserHelper.GetMapSource(ref project.StageData3.Strings, project.Runtime.Definitions.ParamName3, project.Runtime.Definitions.StageSize3, ref dictionary, chipDataClass.Mapchip, project.Runtime.Definitions.StageSplit);
                    StatusText.Text = "ステージソース生成中[4/4]...";
                    StatusText.Refresh();
                    HtmlParserHelper.GetMapSource(ref project.StageData4.Strings, project.Runtime.Definitions.ParamName4, project.Runtime.Definitions.StageSize4, ref dictionary, chipDataClass.Mapchip, project.Runtime.Definitions.StageSplit);
                    if (project.Runtime.Definitions.LayerSize.bytesize != 0)
                    {
                        StatusText.Text = "レイヤーソース生成中[1/4]...";
                        StatusText.Refresh();
                        HtmlParserHelper.GetMapSource(ref project.LayerData[0].Strings, project.Runtime.Definitions.LayerName, project.Runtime.Definitions.LayerSize, ref dictionary, chipDataClass.Layerchip, project.Runtime.Definitions.LayerSplit);
                        StatusText.Text = "レイヤーソース生成中[2/4]...";
                        StatusText.Refresh();
                        HtmlParserHelper.GetMapSource(ref project.LayerData2[0].Strings, project.Runtime.Definitions.LayerName2, project.Runtime.Definitions.LayerSize2, ref dictionary, chipDataClass.Layerchip, project.Runtime.Definitions.LayerSplit);
                        StatusText.Text = "レイヤーソース生成中[3/4]...";
                        StatusText.Refresh();
                        HtmlParserHelper.GetMapSource(ref project.LayerData3[0].Strings, project.Runtime.Definitions.LayerName3, project.Runtime.Definitions.LayerSize3, ref dictionary, chipDataClass.Layerchip, project.Runtime.Definitions.LayerSplit);
                        StatusText.Text = "レイヤーソース生成中[4/4]...";
                        StatusText.Refresh();
                        HtmlParserHelper.GetMapSource(ref project.LayerData4[0].Strings, project.Runtime.Definitions.LayerName4, project.Runtime.Definitions.LayerSize4, ref dictionary, chipDataClass.Layerchip, project.Runtime.Definitions.LayerSplit);
                    }
                }

                StatusText.Text = "パラメータ反映中...";
                StatusText.Refresh();

                var s = string.Join(string.Empty, project.MapData);
                string Mapdata = new([.. s.Except(s.Where(ch => s.Count(c => c == ch) > 1))]); // 地図画面データを圧縮

                int num = 0;
                int mcs_screen_size = 2;
                while (num < project.Config.Configurations.Length)
                {
                    switch (project.Config.Configurations[num].Type)
                    {
                        case ConfigParam.Types.b:
                        case ConfigParam.Types.b2:
                        case ConfigParam.Types.b0:
                            // シングルクォートなし・あり両方でdictionaryを探す
                            {
                                string configName = project.Config.Configurations[num].Name;
                                string configNameNoQuote = configName.Trim('\'');
                                if (dictionary.TryGetValue(configName, out string value1) || dictionary.TryGetValue(configNameNoQuote, out value1))
                                {
                                    project.Config.Configurations[num].Value = (value1 == "2" || value1 == "0" || value1 == "false") ? "false" : "true";
                                }
                                else
                                {
                                    switch (project.Config.Configurations[num].Name) // 個別に初期値を設定
                                    {
                                        case "se_switch":
                                        case "mcs_haikei_visible":
                                        case "fx_bgm_loop":
                                        case "se_filename":
                                            project.Config.Configurations[num].Value =
                                                "false";
                                            break;
                                        case "pause_switch":
                                        case "j_fire_mkf":
                                            project.Config.Configurations[num].Value =
                                                "true";
                                            break;
                                    }
                                }
                            }
                            break;
                        case ConfigParam.Types.s:
                        case ConfigParam.Types.i:
                        case ConfigParam.Types.l_a:
                            goto IL_D9E;
                        case ConfigParam.Types.l:
                            {
                                string configName = project.Config.Configurations[num].Name;
                                string configNameNoQuote = configName.Trim('\'');
                                if (configName == "mcs_screen_size"){
                                    if (dictionary.TryGetValue(configName, out string value1) || dictionary.TryGetValue(configNameNoQuote, out value1))
                                    {
                                        if(value1 == "1")
                                        {
                                            mcs_screen_size = 1;
                                        }
                                    }
                                    break;
                                }
                                goto IL_D9E;
                            }
                        case ConfigParam.Types.t:
                            {
                                string name = project.Config.Configurations[num].Name;
                                project.Config.Configurations[num].Value = name switch // 個別に初期値を設定
                                {
                                    "serifu1" => "人の命は、お金では買えないと言われています。\r\nしかし、お店へ行けば、ＳＣＯＲＥで買えます。\r\n0",
                                    "serifu2" => "時は金なりと、言われています。しかし、\r\nお店なら、時間も買えます。\r\n店員さんて、グレートですね。",
                                    "serifu3" => "おはようございます。星と数字が付いた扉が、\r\nありますよね。あれは、ですねえ、その数だけ\r\n人面星を取ると、開くので、ございます。",
                                    "serifu4" => "LAST STAGEというのは、最終面の事ですわ。\r\nこれをクリアーすると、エンディングに、\r\n行けますのよ。がんばって下さいね。",
                                    "serifu_key2_on" => "３つのＫＥＹ２がないと、\r\nここから先へは進めないぜ。\r\nどこかで見つ付けてくれ。",
                                    "hitokoto1" => "今日は、いい天気だね。\r\n0\r\n0",
                                    "hitokoto2" => "ついに、ここまで来ましたね。\r\n0\r\n0",
                                    "hitokoto3" => "オレは、世界一になる男だ。\r\n0\r\n0",
                                    "hitokoto4" => "んちゃ！\r\n0\r\n0",
                                    _ => ""
                                };

                                List<string> list2 = [];

                                int num2 = 1;

                                Regex text_name_regex = HtmlParserHelper.reg_text_name();
                                Match text_name_match = text_name_regex.Match(name);
                                if (text_name_match.Success)
                                {
                                    num2 = int.Parse(text_name_match.Groups[0].Value);
                                    name = text_name_regex.Replace(name, string.Empty);
                                }

                                while (dictionary.ContainsKey($"{name}-{num2}"))
                                {
                                    list2.Add(dictionary[$"{name}-{num2}"]);
                                    num2++;
                                }
                                if (list2.Count > 0)
                                {
                                    if (project.Config.Configurations[num].Rows > list2.Count)
                                    {
                                        while (project.Config.Configurations[num].Rows > list2.Count)
                                        {
                                            list2.Add("0");
                                        }
                                    }
                                    else if (project.Config.Configurations[num].Rows < list2.Count)
                                    {
                                        list2.RemoveRange(project.Config.Configurations[num].Rows, list2.Count - project.Config.Configurations[num].Rows);
                                    }
                                    project.Config.Configurations[num].Value = string.Join(Environment.NewLine, list2);
                                    // 文字列に\"が含まれていた場合エスケープを戻す
                                    project.Config.Configurations[num].Value = project.Config.Configurations[num].Value.Replace(@"\""", @"""");
                                    project.Config.Configurations[num].Value = project.Config.Configurations[num].Value.Replace(@"\\", @"\");
                                }
                                break;
                            }
                        case ConfigParam.Types.f:
                        case ConfigParam.Types.f_i:
                        case ConfigParam.Types.f_a:
                            if (dictionary.TryGetValue(project.Config.Configurations[num].Name, out string value2))
                            {
                                if (!string.IsNullOrEmpty(value2))
                                {
                                    list.Add(value2);
                                    project.Config.Configurations[num].Value = Path.GetFileName(value2);
                                }
                            }
                            break;
                        case ConfigParam.Types.c:
                            {
                                string[] array =
                                [
                                    "red",
                                    "green",
                                    "blue"
                                ];
                                int[] array2 = new int[3];
                                string name = project.Config.Configurations[num].Name, param_name;
                                for (int num3 = 0; num3 < 3; num3++)
                                {
                                    param_name = name.Replace("@", array[num3]);

                                    // パラメータが存在しない または 数値に変換できない
                                    if (!dictionary.TryGetValue(param_name, out string value3) || !int.TryParse(value3, out array2[num3]))
                                    {
                                        // デフォルト値を代入
                                        switch (param_name)
                                        {
                                            case "backcolor_red":
                                            case "scorecolor_red":
                                            case "scorecolor_green":
                                            case "grenade_blue2":
                                            case "mizunohadou_red":
                                            case "firebar_green1":
                                            case "firebar_blue1":
                                            case "firebar_blue2":
                                            case "kaishi_red":
                                            case "kaishi_blue":
                                            case "kaishi_green":
                                            case "backcolor_red_s":
                                            case "backcolor_blue_s":
                                            case "backcolor_green_s":
                                            case "backcolor_red_t":
                                            case "message_back_red":
                                            case "message_back_green":
                                            case "message_back_blue":
                                            case "message_name_red":
                                            case "gauge_back_blue1":
                                            case "gauge_back_blue2":
                                                array2[num3] = 0;
                                                break;
                                            case "backcolor_green":
                                            case "backcolor_blue":
                                            case "scorecolor_blue":
                                            case "grenade_red1":
                                            case "grenade_green1":
                                            case "grenade_blue1":
                                            case "grenade_red2":
                                            case "grenade_green2":
                                            case "mizunohadou_blue":
                                            case "firebar_red1":
                                            case "firebar_red2":
                                            case "backcolor_green_t":
                                            case "backcolor_blue_t":
                                            case "message_border_red":
                                            case "message_border_green":
                                            case "message_border_blue":
                                            case "message_name_green":
                                            case "message_name_blue":
                                            case "message_text_red":
                                            case "message_text_green":
                                            case "message_text_blue":
                                            case "gauge_border_red":
                                            case "gauge_border_green":
                                            case "gauge_border_blue":
                                            case "gauge_back_red1":
                                            case "gauge_back_green1":
                                            case "gauge_back_red2":
                                                array2[num3] = 255;
                                                break;
                                            case "mizunohadou_green":
                                                array2[num3] = 32;
                                                break;
                                            case "firebar_green2":
                                            case "backcolor_red_f":
                                                array2[num3] = 192;
                                                break;
                                            case "backcolor_green_f":
                                            case "backcolor_blue_f":
                                                array2[num3] = 48;
                                                break;
                                        }
                                    }
                                }

                                Colors colors = default;
                                colors.r = array2[0];
                                colors.g = array2[1];
                                colors.b = array2[2];
                                project.Config.Configurations[num].Value = colors.ToString(); // 配列を文字列に変換　[r,g,b] => "r,g,b"

                                break;
                            }
                        default:
                            goto IL_D9E;
                    }
                IL_DF3:
                    num++;
                    continue;
                IL_D9E:
                    if (dictionary.TryGetValue(project.Config.Configurations[num].Name, out string value))
                    {
                        project.Config.Configurations[num].Value = value;
                        if (project.Config.Configurations[num].Type == ConfigParam.Types.s) // 文字列に\"が含まれていた場合エスケープを戻す
                        {
                            project.Config.Configurations[num].Value = project.Config.Configurations[num].Value.Replace(@"\""", @"""");
                            project.Config.Configurations[num].Value = project.Config.Configurations[num].Value.Replace(@"\\", @"\");
                        }
                        goto IL_DF3;
                    }
                    else if (project.Config.Configurations[num].Relation == "STAGENUM")
                    {
                        // アスキーコードで98('b')が地図画面に含まれている場合、ステージ2を選択できるように
                        // ステージ3、4も同様
                        int map_code_ASCII;
                        for (int i = 2; i <= 4; i++)
                        {
                            map_code_ASCII = i + 96;
                            if (Mapdata.Contains(((char)map_code_ASCII).ToString()))
                                project.Config.Configurations[num].Value = i.ToString();
                        }
                    }
                    else
                    {
                        switch (project.Config.Configurations[num].Name) // 個別に初期値を設定
                        {
                            case "j_hp_name":
                            case "now_loading":
                            case "oriboss_name":
                                project.Config.Configurations[num].Value =
                                    "";
                                break;
                            case "time_max":
                            case "shop_item_teika7":
                                project.Config.Configurations[num].Value =
                                    "300";
                                break;
                            case "gazou_scroll_speed_x":
                            case "gazou_scroll_speed_y":
                            case "second_gazou_scroll_speed_x":
                            case "second_gazou_scroll_speed_y":
                            case "oriboss_x":
                            case "oriboss_y":
                                project.Config.Configurations[num].Value =
                                    "0";
                                break;
                            case "water_visible":
                            case "j_tail_type":
                            case "oriboss_hp":
                            case "oriboss_speed":
                            case "dokan_mode":
                                project.Config.Configurations[num].Value =
                                    "1";
                                break;
                            case "boss_name":
                            case "boss2_name":
                            case "boss3_name":
                                project.Config.Configurations[num].Value =
                                    "BOSS";
                                break;
                            case "mes1_name":
                                project.Config.Configurations[num].Value =
                                    "ダケシ";
                                break;
                            case "mes2_name":
                                project.Config.Configurations[num].Value =
                                    "エリコ";
                                break;
                            case "oriboss_width":
                            case "oriboss_height":
                                project.Config.Configurations[num].Value =
                                    "32";
                                break;
                            case "door_score":
                                project.Config.Configurations[num].Value =
                                    "800";
                                break;
                            case "url1":
                            case "url2":
                            case "url3":
                                project.Config.Configurations[num].Value =
                                    "http://www.yahoo.co.jp/";
                                break;
                            case "url4":
                                project.Config.Configurations[num].Value =
                                    "http://www.t3.rim.or.jp/~naoto/naoto.html";
                                break;
                            case "width":
                                if(mcs_screen_size == 1)
                                {
                                    project.Config.Configurations[num].Value = "640";
                                }
                                break;
                            case "height":
                                if(mcs_screen_size == 1)
                                {
                                    project.Config.Configurations[num].Value = "480";
                                }
                                break;
                        }
                    }
                    goto IL_DF3;
                }
                Directory.CreateDirectory(text);
                foreach (string text3 in Directory.GetFiles(text2, "*", SearchOption.TopDirectoryOnly))
                {
                    if (!(Path.GetFileName(text3) == project.Runtime.Definitions.Configurations))
                    {
                        string text4 = Path.Combine(text, Path.GetFileName(text3));
                        if (!File.Exists(text4) || MessageBox.Show($"{text4}{Environment.NewLine}はすでに存在しています。{Environment.NewLine}上書きしてもよろしいですか？", "上書きの警告", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.No)
                        {
                            File.Copy(text3, text4, true);
                        }
                    }
                }
                foreach (string text5 in list)
                {
                    if (File.Exists(Path.Combine(Path.GetDirectoryName(ParseFile), text5)))
                    {
                        File.Copy(Path.Combine(Path.GetDirectoryName(ParseFile), text5), Path.Combine(text, Path.GetFileName(text5)), true);
                    }
                }
                project.SaveXML(ProjectFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"プロジェクト生成に失敗しました。{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}", "プロジェクト生成エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            finally
            {
                OK.Text = "OK";
                Enabled = true;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        public string ProjectFile = "";

        public List<string> runtimes = [];

        public List<Runtime> runtimedatas = [];

        public List<bool> runtimeuselayer = [];

        private readonly string ParseFile = "";

        private static JsonElement rootJsonElement;
    }
}
