using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
                                RuntimeSet.Items.Add(string.Concat(new string[]
                                {
                                    runtime.Definitions.Name,
                                    " [Author:",
                                    runtime.Definitions.Author,
                                    " Layer:",
                                    (runtime.Definitions.LayerSize.bytesize != 0) ? "○" : "×",
                                    "] : ",
                                    Path.GetFileName(text)
                                }));
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
                if (project.Runtime.Definitions.LayerSize.bytesize != 0)
                {
                    void setLayerSize(ref Runtime.DefinedData.StageSizeData size)
                    {
                        size.x = 180;
                        size.y = 30;
                        size.bytesize = project.Runtime.Definitions.LayerSize.bytesize;
                    }
                    setLayerSize(ref project.Runtime.Definitions.LayerSize);
                    setLayerSize(ref project.Runtime.Definitions.LayerSize2);
                    setLayerSize(ref project.Runtime.Definitions.LayerSize3);
                    setLayerSize(ref project.Runtime.Definitions.LayerSize4);
                    project.LayerData = new string[project.Runtime.Definitions.LayerSize.y];
                    project.LayerData2 = new string[project.Runtime.Definitions.LayerSize2.y];
                    project.LayerData3 = new string[project.Runtime.Definitions.LayerSize3.y];
                    project.LayerData4 = new string[project.Runtime.Definitions.LayerSize4.y];
                }
                void setStageSize(ref Runtime.DefinedData.StageSizeData size)
                {
                    size.x = 180;
                    size.y = 30;
                    size.bytesize = project.Runtime.Definitions.StageSize.bytesize;
                }
                setStageSize(ref project.Runtime.Definitions.StageSize);
                setStageSize(ref project.Runtime.Definitions.StageSize2);
                setStageSize(ref project.Runtime.Definitions.StageSize3);
                setStageSize(ref project.Runtime.Definitions.StageSize4);
                project.StageData = new string[project.Runtime.Definitions.StageSize.y];
                project.StageData2 = new string[project.Runtime.Definitions.StageSize2.y];
                project.StageData3 = new string[project.Runtime.Definitions.StageSize3.y];
                project.StageData4 = new string[project.Runtime.Definitions.StageSize4.y];
                project.MapData = new string[project.Runtime.Definitions.MapSize.y];
                ChipDataClass chipDataClass = ChipDataClass.ParseXML(Path.Combine(text2, project.Runtime.Definitions.ChipDefinition));
                void setStageData(string[] data, int x, string character)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        StringBuilder stringBuilder = new();
                        for (int j = 0; j < x; j++)
                        {
                            stringBuilder.Append(character);
                        }
                        data[i] = stringBuilder.ToString();
                    }
                }
                string character = chipDataClass.Mapchip[0].character;
                setStageData(project.StageData, project.Runtime.Definitions.StageSize.x, character);
                setStageData(project.StageData2, project.Runtime.Definitions.StageSize2.x, character);
                setStageData(project.StageData3, project.Runtime.Definitions.StageSize3.x, character);
                setStageData(project.StageData4, project.Runtime.Definitions.StageSize4.x, character);
                character = chipDataClass.WorldChip[0].character;
                setStageData(project.MapData, project.Runtime.Definitions.MapSize.x, character);
                if (project.Runtime.Definitions.LayerSize.bytesize != 0)
                {
                    character = chipDataClass.Layerchip[0].character;
                    setStageData(project.LayerData, project.Runtime.Definitions.LayerSize.x, character);
                    setStageData(project.LayerData2, project.Runtime.Definitions.LayerSize2.x, character);
                    setStageData(project.LayerData3, project.Runtime.Definitions.LayerSize3.x, character);
                    setStageData(project.LayerData4, project.Runtime.Definitions.LayerSize4.x, character);
                }
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
                List<string> list = new();
                if (SeekHeaderFooter.Checked)
                {
                    Regex regex = new("^.*?<[ ]*?APPLET .*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    Match match = regex.Match(input);
                    if (match.Success)
                    {
                        project.Runtime.DefaultConfigurations.HeaderHTML = match.Value;
                    }
                    regex = new Regex("<[ ]*?/[ ]*?APPLET[ ]*?>.*$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    match = regex.Match(input);
                    if (match.Success)
                    {
                        project.Runtime.DefaultConfigurations.FooterHTML = match.Value;
                    }
                }
                StatusText.Text = "HTMLデータ取得中...";
                StatusText.Refresh();

                Regex regex2 = new(@"<[ ]*PARAM[ ]+NAME=""(?<name>.*?)""[ ]+VALUE=""(?<value>.*?)"".*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Regex regex_script = new(@"<[ ]*?script.*?>.*?new\s*?(JSMasao|CanvasMasao\.\s*?Game)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (regex_script.IsMatch(input))
                {
                    regex2 = new Regex(@"(""|')(?<name>.*?)(""|')\s*?:\s*?(""|')(?<value>.*?)(?<!\\)(""|')(,|\s*?)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }

                Dictionary<string, string> dictionary = new();
                Match match2 = regex2.Match(input);
                while (match2.Success)
                {
                    dictionary[match2.Groups["name"].Value] = match2.Groups["value"].Value;
                    match2 = match2.NextMatch();
                }
                StatusText.Text = "マップソース生成中...";
                StatusText.Refresh();
                GetMapSource(ref project.MapData, project.Runtime.Definitions.MapName, project.Runtime.Definitions.MapSize, ref dictionary, chipDataClass.WorldChip);
                StatusText.Text = "ステージソース生成中[1/4]...";
                StatusText.Refresh();
                GetMapSource(ref project.StageData, project.Runtime.Definitions.ParamName, project.Runtime.Definitions.StageSize, ref dictionary, chipDataClass.Mapchip, project.Runtime.Definitions.StageSplit);
                StatusText.Text = "ステージソース生成中[2/4]...";
                StatusText.Refresh();
                GetMapSource(ref project.StageData2, project.Runtime.Definitions.ParamName2, project.Runtime.Definitions.StageSize2, ref dictionary, chipDataClass.Mapchip, project.Runtime.Definitions.StageSplit);
                StatusText.Text = "ステージソース生成中[3/4]...";
                StatusText.Refresh();
                GetMapSource(ref project.StageData3, project.Runtime.Definitions.ParamName3, project.Runtime.Definitions.StageSize3, ref dictionary, chipDataClass.Mapchip, project.Runtime.Definitions.StageSplit);
                StatusText.Text = "ステージソース生成中[4/4]...";
                StatusText.Refresh();
                GetMapSource(ref project.StageData4, project.Runtime.Definitions.ParamName4, project.Runtime.Definitions.StageSize4, ref dictionary, chipDataClass.Mapchip, project.Runtime.Definitions.StageSplit);
                if (project.Runtime.Definitions.LayerSize.bytesize != 0)
                {
                    StatusText.Text = "レイヤーソース生成中[1/4]...";
                    StatusText.Refresh();
                    GetMapSource(ref project.LayerData, project.Runtime.Definitions.LayerName, project.Runtime.Definitions.LayerSize, ref dictionary, chipDataClass.Layerchip, project.Runtime.Definitions.LayerSplit);
                    StatusText.Text = "レイヤーソース生成中[2/4]...";
                    StatusText.Refresh();
                    GetMapSource(ref project.LayerData2, project.Runtime.Definitions.LayerName2, project.Runtime.Definitions.LayerSize2, ref dictionary, chipDataClass.Layerchip, project.Runtime.Definitions.LayerSplit);
                    StatusText.Text = "レイヤーソース生成中[3/4]...";
                    StatusText.Refresh();
                    GetMapSource(ref project.LayerData3, project.Runtime.Definitions.LayerName3, project.Runtime.Definitions.LayerSize3, ref dictionary, chipDataClass.Layerchip, project.Runtime.Definitions.LayerSplit);
                    StatusText.Text = "レイヤーソース生成中[4/4]...";
                    StatusText.Refresh();
                    GetMapSource(ref project.LayerData4, project.Runtime.Definitions.LayerName4, project.Runtime.Definitions.LayerSize4, ref dictionary, chipDataClass.Layerchip, project.Runtime.Definitions.LayerSplit);
                }
                if(dictionary.ContainsKey("advanced-map") || dictionary.ContainsKey("advance-map"))
                {
                    // 第3版マップデータを含む場合の処理
                }
                StatusText.Text = "パラメータ反映中...";
                StatusText.Refresh();

                var s = string.Join(string.Empty, project.MapData);
                string Mapdata = new(s.Except(s.Where(ch => s.Count(c => c == ch) > 1)).ToArray()); // 地図画面データを圧縮

                int num = 0;
                while (num < project.Config.Configurations.Length)
                {
                    switch (project.Config.Configurations[num].Type)
                    {
                        case ConfigParam.Types.b:
                        case ConfigParam.Types.b2:
                        case ConfigParam.Types.b0:
                            if (dictionary.TryGetValue(project.Config.Configurations[num].Name, out string value1))
                            {
                                if (value1 == "2" || value1 == "0")
                                {
                                    project.Config.Configurations[num].Value = "false";
                                }
                                else
                                {
                                    project.Config.Configurations[num].Value = "true";
                                }
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
                            break;
                        case ConfigParam.Types.s:
                        case ConfigParam.Types.i:
                        case ConfigParam.Types.l:
                        case ConfigParam.Types.l_a:
                            goto IL_D9E;
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

                                List<string> list2 = new();

                                int num2 = 1;

                                Regex text_name_regex = new(@"-(\d+)$");
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
                                    project.Config.Configurations[num].Value = string.Join(Environment.NewLine, [.. list2]);
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
                                list.Add(value2);
                                project.Config.Configurations[num].Value = Path.GetFileName(value2);
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

        private static bool GetMapSource(ref string[] overwrite, string f, Runtime.DefinedData.StageSizeData StageSizeData, ref Dictionary<string, string> Params, ChipsData[] MapChip, int Split = 0)
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
                            for (int j = 0; j < dxsize / (Split + 1) / NullChar.Length; j++)
                                list[num] += NullChar;

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

                // 文字数が足りない場合空白文字を追加
                int k;
                for (int i = 0; i < dysize; i++)
                {
                    k = dxsize - overwrite[i].Length / NullChar.Length;
                    for (int j = 0; j < k; j++)
                        overwrite[i] += NullChar;
                }

                return true;
            }
            return false;
        }

        public string ProjectFile = "";

        public List<string> runtimes = new();

        public List<Runtime> runtimedatas = new();

        public List<bool> runtimeuselayer = new();

        private readonly string ParseFile = "";
    }
}
