using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using MasaoPlus.Dialogs;
using System.Text.RegularExpressions;

namespace MasaoPlus
{
	public static class Subsystem
	{
		public static void MakeTestrun(int startup)
		{
			using (StreamWriter streamWriter = new StreamWriter(Subsystem.GetTempFileWhere(), false, Global.config.localSystem.FileEncoding))
			{
				string value = Subsystem.MakeHTMLCode(startup);
				streamWriter.Write(value);
				streamWriter.Close();
			}
		}

		public static void MakeTestrun(int startup, int replace, string[] sts)
		{
			using (StreamWriter streamWriter = new StreamWriter(Subsystem.GetTempFileWhere(), false, Global.config.localSystem.FileEncoding))
			{
				string value = Subsystem.MakeHTMLCode(startup, replace, sts);
				streamWriter.Write(value);
				streamWriter.Close();
			}
		}

		public static string GetTempFileWhere()
		{
			return Path.Combine(Global.cpd.where, Global.config.testRun.TempFile + "." + Global.cpd.runtime.DefaultConfigurations.FileExt);
		}

		public static string MakeHTMLCode(int StartStage)
		{
			return Subsystem.MakeHTMLCode(StartStage, -1, null);
		}

		public static string MakeHTMLCode(int StartStage, int ReplaceStage, string[] sts)
		{
			Global.cpd.project.Config.StageStart = StartStage + 1;
			StringBuilder stringBuilder = new StringBuilder();

			// ヘッダーを出力
			string text = Subsystem.DecodeBase64(Global.cpd.runtime.DefaultConfigurations.HeaderHTML);
			if (Global.cpd.runtime.DefaultConfigurations.OutputReplace.Length > 0)
			{
				foreach (HTMLReplaceData htmlreplaceData in Global.cpd.runtime.DefaultConfigurations.OutputReplace)
				{
					text = text.Replace("<?" + htmlreplaceData.Name + ">", htmlreplaceData.Value);
				}
			}
			foreach (string value in text.Split(new string[]
			{
				Environment.NewLine,
				"\r",
				"\n"
			}, StringSplitOptions.None))
			{
				stringBuilder.AppendLine(value);
			}

			//エデイタ識別コードを出力
			if (Global.config.localSystem.IntegrateEditorId)
			{
				stringBuilder.AppendLine(Global.definition.EditorIdStr);
			}

			//パラメータを出力
			stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.StageParam, Global.cpd.runtime.Definitions.StageSplit, (ReplaceStage == 0) ? sts : Global.cpd.project.StageData, Global.cpd.runtime.Definitions.StageSize, true));
			if (Global.cpd.project.Config.StageNum >= 2)
			{
				stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.StageParam2, Global.cpd.runtime.Definitions.StageSplit, (ReplaceStage == 1) ? sts : Global.cpd.project.StageData2, Global.cpd.runtime.Definitions.StageSize));
			}
			if (Global.cpd.project.Config.StageNum >= 3)
			{
				stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.StageParam3, Global.cpd.runtime.Definitions.StageSplit, (ReplaceStage == 2) ? sts : Global.cpd.project.StageData3, Global.cpd.runtime.Definitions.StageSize));
			}
			if (Global.cpd.project.Config.StageNum >= 4)
			{
				stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.StageParam4, Global.cpd.runtime.Definitions.StageSplit, (ReplaceStage == 3) ? sts : Global.cpd.project.StageData4, Global.cpd.runtime.Definitions.StageSize));
			}

			if (Global.cpd.runtime.Definitions.LayerSize.bytesize != 0)
			{
				stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.LayerParam, Global.cpd.runtime.Definitions.LayerSplit, Global.cpd.project.LayerData, Global.cpd.runtime.Definitions.LayerSize));
				if (Global.cpd.project.Config.StageNum >= 2)
				{
					stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.LayerParam2, Global.cpd.runtime.Definitions.LayerSplit, Global.cpd.project.LayerData2, Global.cpd.runtime.Definitions.LayerSize));
				}
				if (Global.cpd.project.Config.StageNum >= 3)
				{
					stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.LayerParam3, Global.cpd.runtime.Definitions.LayerSplit, Global.cpd.project.LayerData3, Global.cpd.runtime.Definitions.LayerSize));
				}
				if (Global.cpd.project.Config.StageNum >= 4)
				{
					stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.LayerParam4, Global.cpd.runtime.Definitions.LayerSplit, Global.cpd.project.LayerData4, Global.cpd.runtime.Definitions.LayerSize));
				}
			}

			if (Global.cpd.project.Config.UseWorldmap)
			{
				stringBuilder.AppendLine(Subsystem.MakeStageParameter(Global.cpd.runtime.DefaultConfigurations.MapParam, 0, Global.cpd.project.MapData, Global.cpd.runtime.Definitions.MapSize));
			}

			string parameter = Global.cpd.runtime.DefaultConfigurations.Parameter;
			ConfigParam[] configurations = Global.cpd.project.Config.Configurations;
			int k = 0;//現在読み込まれている行
			int mcs_screen_size = 2;
			while (k < configurations.Length)
			{
				ConfigParam configParam = configurations[k];

				if (configParam.Name == "mcs_screen_size") mcs_screen_size = int.Parse(configParam.Value);

				if (configParam.Category == "オプション") goto IL_718;

				if (configParam.RequireStages > 1 && configParam.RequireStages < 5)
				{
					if (configParam.RequireStages <= Global.cpd.project.Config.StageNum)
					{
						goto IL_4DB;
					}
				}
				else if (configParam.RequireStages != 5 || Global.cpd.project.Config.UseWorldmap)
				{
					goto IL_4DB;
				}

				IL_718:
				k++;
				continue;

				IL_4DB:
                if (!Global.config.localSystem.OutPutInititalSourceCode && !Global.cpd.runtime.Definitions.Package.Contains("28")) {
					// 値を調べ、初期値だったら出力しない（参考：Canvas正男のTagDataBase.js）
					switch (configParam.Name)
					{
						case "mes1_name":
							if (configParam.Value == "ダケシ") goto IL_718;
							else break;
						case "serifu1":
							if (configParam.Value ==
								"人の命は、お金では買えないと言われています。\r\nしかし、お店へ行けば、ＳＣＯＲＥで買えます。\r\n0") goto IL_718;
							else break;
						case "serifu2":
							if (configParam.Value ==
								"時は金なりと、言われています。しかし、\r\nお店なら、時間も買えます。\r\n店員さんて、グレートですね。") goto IL_718;
							else break;
						case "mes2_name":
							if (configParam.Value == "エリコ") goto IL_718;
							else break;
						case "serifu3":
							if (configParam.Value ==
								"おはようございます。星と数字が付いた扉が、\r\nありますよね。あれは、ですねえ、その数だけ\r\n人面星を取ると、開くので、ございます。") goto IL_718;
							else break;
						case "serifu4":
							if (configParam.Value ==
								"LAST STAGEというのは、最終面の事ですわ。\r\nこれをクリアーすると、エンディングに、\r\n行けますのよ。がんばって下さいね。") goto IL_718;
							else break;
						case "shop_name":
							if (configParam.Value == "店員さん") goto IL_718;
							else break;
						case "serifu5":
							if (configParam.Value ==
								"いらっしゃいませ。\r\n当店では、ＳＣＯＲＥと、アイテムを、\r\n交換いたします。") goto IL_718;
							else break;
						case "serifu8":
							if (configParam.Value ==
								"本日の営業は、終了いたしました。\r\nまたのご来店を、\r\nこころより、お待ちしております。") goto IL_718;
							else break;
						case "shop_serifu1":
							if (configParam.Value == "どれになさいますか？") goto IL_718;
							else break;
						case "shop_serifu2":
							if (configParam.Value == "で、よろしいですか？") goto IL_718;
							else break;
						case "shop_serifu3":
						case "serifu_key1_on-5":
						case "serifu_key2_on-5":
							if (configParam.Value == "はい") goto IL_718;
							else break;
						case "shop_serifu4":
						case "serifu_key1_on-6":
						case "serifu_key2_on-6":
							if (configParam.Value == "いいえ") goto IL_718;
							else break;
						case "shop_serifu5":
							if (configParam.Value == "を、装備した。") goto IL_718;
							else break;
						case "shop_serifu6":
							if (configParam.Value == "ＳＣＯＲＥが、足りません。") goto IL_718;
							else break;
						case "shop_item_name1":
							if (configParam.Value == "グレネード３発") goto IL_718;
							else break;
						case "shop_item_name2":
						case "setumei_menu4":
							if (configParam.Value == "ジェット") goto IL_718;
							else break;
						case "shop_item_name3":
							if (configParam.Value == "ドリル") goto IL_718;
							else break;
						case "shop_item_name4":
							if (configParam.Value == "ヘルメット") goto IL_718;
							else break;
						case "shop_item_name5":
							if (configParam.Value == "しっぽ") goto IL_718;
							else break;
						case "shop_item_name6":
						case "setumei_menu3":
							if (configParam.Value == "バリア") goto IL_718;
							else break;
						case "shop_item_name7":
						case "setumei_menu2":
							if (configParam.Value == "ファイヤーボール") goto IL_718;
							else break;
						case "shop_item_name8":
							if (configParam.Value == "１ｕｐ") goto IL_718;
							else break;
						case "shop_item_name9":
							if (configParam.Value == "制限時間増加") goto IL_718;
							else break;
						case "shop_item_teika1":
							if (configParam.Value == "200") goto IL_718;
							else break;
						case "shop_item_teika2":
							if (configParam.Value == "150") goto IL_718;
							else break;
						case "shop_item_teika3":
						case "shop_item_teika4":
							if (configParam.Value == "100") goto IL_718;
							else break;
						case "shop_item_teika5":
							if (configParam.Value == "250") goto IL_718;
							else break;
						case "shop_item_teika6":
							if (configParam.Value == "80") goto IL_718;
							else break;
						case "shop_item_teika7":
						case "time_max":
							if (configParam.Value == "300") goto IL_718;
							else break;
						case "shop_item_teika8":
							if (configParam.Value == "980") goto IL_718;
							else break;
						case "shop_item_teika9":
						case "easy_mode":
						case "scroll_mode":
						case "scroll_mode_s":
						case "scroll_mode_t":
						case "scroll_mode_f":
						case "stage_max":
						case "stage_kaishi":
						case "jibun_left_shoki":
						case "stage_select":
						case "j_tail_type":
						case "grenade_type":
						case "dengeki_mkf":
						case "yachamo_kf":
						case "airms_kf":
						case "ugokuyuka1_type":
						case "ugokuyuka2_type":
						case "ugokuyuka3_type":
						case "boss_type":
						case "boss2_type":
						case "boss3_type":
						case "dokan_mode":
						case "j_tokugi":
						case "scroll_area":
						case "clear_type":
						case "firebar1_type":
						case "firebar2_type":
						case "dossunsun_type":
						case "mizutaro_attack":
						case "poppie_attack":
						case "mariri_attack":
						case "chikorin_attack":
						case "taiking_attack":
						case "kuragesso_attack":
						case "coin1_type":
						case "coin3_type":
						case "dokan1_type":
						case "dokan2_type":
						case "dokan3_type":
						case "dokan4_type":
						case "view_move_type":
						case "j_fire_type":
						case "j_enemy_press":
						case "boss_destroy_type":
						case "j_add_tokugi":
						case "j_add_tokugi2":
						case "j_add_tokugi3":
						case "j_add_tokugi4":
						case "second_gazou_scroll":
						case "second_gazou_priority":
						case "water_visible":
						case "j_hp":
						case "oriboss_v":
						case "oriboss_anime_type":
						case "oriboss_hp":
						case "oriboss_speed":
						case "oriboss_ugoki":
						case "oriboss_waza_select":
						case "oriboss_waza1":
						case "oriboss_waza2":
						case "oriboss_waza3":
						case "oriboss_waza1_wait":
						case "oriboss_waza2_wait":
						case "oriboss_waza3_wait":
						case "oriboss_fumeru_f":
						case "oriboss_destroy":
							if (configParam.Value == "1") goto IL_718;
							else break;
						case "setumei_name":
							if (configParam.Value == "キドはかせ") goto IL_718;
							else break;
						case "serifu9":
							if (configParam.Value ==
								"よく来た。わしは、キドはかせ。\r\nアイテムの研究をしており、みんなから、\r\nアイテムはかせと呼ばれて、したわれておるよ。") goto IL_718;
							else break;
						case "setumei_menu1":
							if (configParam.Value == "なんでも、質問してくれたまえよ。") goto IL_718;
							else break;
						case "serifu10":
							if (configParam.Value ==
								"黄色いチューリップのアイテムと言えば、\r\nそう、ファイヤーボールじゃな。はなれた\r\n敵を攻撃できるという、大変便利なものじゃ。") goto IL_718;
							else break;
						case "serifu11":
							if (configParam.Value ==
								"ピンクのキノコのアイテムと言えば、そう、\r\nバリアじゃな。体当たりで敵を倒せるが、うっかり\r\nして、時間切れを忘れぬよう、注意が必要じゃ。") goto IL_718;
							else break;
						case "serifu12":
							if (configParam.Value ==
								"ロケットの形のアイテムと言えば、そう、ジェット\r\nじゃな。空中で、スペースキーを押せば、さらに\r\n上昇できる。燃料切れには、気を付けるのじゃぞ。") goto IL_718;
							else break;
						case "door_score":
							if (configParam.Value == "800") goto IL_718;
							else break;
						case "layer_mode":
						case "score_v":
						case "j_tail_hf":
						case "j_fire_mkf":
						case "suberuyuka_hkf":
						case "variable_sleep_time":
						case "pause_switch":
						case "control_parts_visible":
						case "j_fire_equip":
						case "second_gazou_visible":
						case "water_clear_switch":
						case "audio_se_switch_wave":
						case "audio_se_switch_mp3":
						case "audio_se_switch_ogg":
						case "audio_bgm_switch_mp3":
						case "audio_bgm_switch_ogg":
						case "oriboss_tail_f":
							if (configParam.Value == "true") goto IL_718;
							else break;
						case "filename_mapchip":
							if (configParam.Value == "mapchip.gif") goto IL_718;
							else break;
						case "filename_haikei":
						case "filename_haikei2":
						case "filename_haikei3":
						case "filename_haikei4":
							if (configParam.Value == "haikei.gif") goto IL_718;
							else break;
						case "gazou_scroll":
						case "mcs_screen_size":
							if (configParam.Value == "2") goto IL_718;
							else break;
						case "now_loading":
						case "j_hp_name":
						case "oriboss_name":
						case "filename_oriboss_left1":
						case "filename_oriboss_right1":
						case "filename_oriboss_tubure_left":
						case "filename_oriboss_tubure_right":
						case "filename_oriboss_left2":
						case "filename_oriboss_right2":
						case "filename_ximage1":
						case "filename_ximage2":
						case "filename_ximage3":
						case "filename_ximage4":
						case "x_backimage1_filename":
						case "x_backimage2_filename":
						case "x_backimage3_filename":
						case "x_backimage4_filename":
							if (configParam.Value == string.Empty) goto IL_718;
							else break;
						case "score_1up_1":
							if (configParam.Value == "500") goto IL_718;
							else break;
						case "score_1up_2":
							if (configParam.Value == "1000") goto IL_718;
							else break;
						case "url1":
						case "url2":
						case "url3":
							if (configParam.Value == "http://www.yahoo.co.jp/") goto IL_718;
							else break;
						case "url4":
							if (configParam.Value == "http://www.t3.rim.or.jp/~naoto/naoto.html") goto IL_718;
							else break;
						case "hitokoto1_name":
							if (configParam.Value == "浩二") goto IL_718;
							else break;
						case "hitokoto1":
							if (configParam.Value == "今日は、いい天気だね。\r\n0\r\n0") goto IL_718;
							else break;
						case "hitokoto2_name":
						case "serifu_key1_on_name":
							if (configParam.Value == "お姫様") goto IL_718;
							else break;
						case "hitokoto2":
							if (configParam.Value == "ついに、ここまで来ましたね。\r\n0\r\n0") goto IL_718;
							else break;
						case "hitokoto3_name":
						case "serifu_key2_on_name":
							if (configParam.Value == "ザトシ") goto IL_718;
							else break;
						case "hitokoto3":
							if (configParam.Value == "オレは、世界一になる男だ。\r\n0\r\n0") goto IL_718;
							else break;
						case "hitokoto4_name":
						case "serifu_grenade_shop_name":
							if (configParam.Value == "クリス") goto IL_718;
							else break;
						case "hitokoto4":
							if (configParam.Value == "んちゃ！\r\n0\r\n0") goto IL_718;
							else break;
						case "backcolor_@":
						case "backcolor_@_s":
						case "backcolor_@_t":
						case "backcolor_@_f":
							if (configParam.Value == "0,255,255") goto IL_718;
							else break;
						case "kaishi_@":
							if (configParam.Value == "0,0,0") goto IL_718;
							else break;
						case "scorecolor_@":
							if (configParam.Value == "0,0,255") goto IL_718;
							else break;
						case "grenade_@1":
							if (configParam.Value == "255,255,255") goto IL_718;
							else break;
						case "grenade_@2":
							if (configParam.Value == "255,255,0") goto IL_718;
							else break;
						case "mizunohadou_@":
							if (configParam.Value == "0,32,255") goto IL_718;
							else break;
						case "firebar_@1":
							if (configParam.Value == "255,0,0") goto IL_718;
							else break;
						case "firebar_@2":
							if (configParam.Value == "255,192,0") goto IL_718;
							else break;
						case "moji_score":
							if (configParam.Value == "SCORE") goto IL_718;
							else break;
						case "moji_highscore":
							if (configParam.Value == "HIGHSCORE") goto IL_718;
							else break;
						case "moji_time":
							if (configParam.Value == "TIME") goto IL_718;
							else break;
						case "moji_jet":
							if (configParam.Value == "JET") goto IL_718;
							else break;
						case "moji_grenade":
							if (configParam.Value == "GRENADE") goto IL_718;
							else break;
						case "moji_left":
							if (configParam.Value == "LEFT") goto IL_718;
							else break;
						case "moji_size":
							if (configParam.Value == "14") goto IL_718;
							else break;
						case "filename_title":
							if (configParam.Value == "title.gif") goto IL_718;
							else break;
						case "filename_ending":
							if (configParam.Value == "ending.gif") goto IL_718;
							else break;
						case "filename_gameover":
							if (configParam.Value == "gameover.gif") goto IL_718;
							else break;
						case "filename_pattern":
							if (configParam.Value == "pattern.gif") goto IL_718;
							else break;
						case "filename_chizu":
							if (configParam.Value == "chizu.gif") goto IL_718;
							else break;
						case "game_speed":
							if (configParam.Value == "70") goto IL_718;
							else break;
						case "se_switch":
						case "se_filename":
						case "fx_bgm_switch":
						case "fx_bgm_loop":
						case "sleep_time_visible":
						case "mcs_haikei_visible":
						case "audio_bgm_switch_wave":
							if (configParam.Value == "false") goto IL_718;
							else break;
						case "filename_se_start":
						case "filename_se_item":
							if (new Regex(@"^item\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_gameover":
							if (new Regex(@"^gameover\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_clear":
							if (new Regex(@"^clear\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_coin":
							if (new Regex(@"^coin\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_get":
						case "filename_se_dokan":
						case "filename_se_chizugamen":
							if (new Regex(@"^get\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_jump":
							if (new Regex(@"^jump\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_sjump":
							if (new Regex(@"^sjump\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_kiki":
							if (new Regex(@"^kiki\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_fumu":
							if (new Regex(@"^fumu\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_tobasu":
							if (new Regex(@"^tobasu\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_fireball":
						case "filename_se_bomb":
						case "filename_se_senkuuza":
							if (new Regex(@"^shot\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_jet":
						case "filename_se_dengeki":
						case "filename_se_hinoko":
						case "filename_se_grounder":
							if (new Regex(@"^mgan\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_miss":
						case "filename_se_dosun":
							if (new Regex(@"^dosun\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_block":
							if (new Regex(@"^bakuhatu\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_mizu":
							if (new Regex(@"^mizu\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_se_happa":
						case "filename_se_mizudeppo":
						case "filename_se_kaiole":
							if (new Regex(@"^happa\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_fx_bgm_stage1":
							if (new Regex(@"^stage1\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_fx_bgm_stage2":
							if (new Regex(@"^stage2\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_fx_bgm_stage3":
							if (new Regex(@"^stage3\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_fx_bgm_stage4":
							if (new Regex(@"^stage4\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_fx_bgm_boss":
							if (new Regex(@"^boss\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_fx_bgm_title":
							if (new Regex(@"^title\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_fx_bgm_ending":
							if (new Regex(@"^ending\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "filename_fx_bgm_chizu":
							if (new Regex(@"^chizu\.[0-9a-zA-Z]+$").IsMatch(configParam.Value)) goto IL_718;
							else break;
						case "gazou_scroll_speed_x":
						case "gazou_scroll_speed_y":
						case "second_gazou_scroll_speed_x":
						case "second_gazou_scroll_speed_y":
						case "second_gazou_scroll_x":
						case "second_gazou_scroll_y":
						case "gazou_scroll_x":
						case "gazou_scroll_y":
						case "j_equip_grenade":
						case "oriboss_x":
						case "oriboss_y":
						case "ximage1_view_x":
						case "ximage2_view_x":
						case "ximage3_view_x":
						case "ximage4_view_x":
						case "ximage1_x":
						case "ximage1_y":
						case "ximage2_x":
						case "ximage2_y":
						case "ximage3_x":
						case "ximage3_y":
						case "ximage4_x":
						case "ximage4_y":
						case "x_backimage1_view_x":
						case "x_backimage2_view_x":
						case "x_backimage3_view_x":
						case "x_backimage4_view_x":
							if (configParam.Value == "0") goto IL_718;
							else break;
						case "boss_hp_max":
						case "grenade_shop_score":
							if (configParam.Value == "20") goto IL_718;
							else break;
						case "j_tail_ap_boss":
							if (configParam.Value == "4") goto IL_718;
							else break;
						case "boss_name":
						case "boss2_name":
						case "boss3_name":
							if (configParam.Value == "BOSS") goto IL_718;
							else break;
						case "fs_name":
							if (configParam.Value == "ファイヤーボールセレクトの人") goto IL_718;
							else break;
						case "serifu7":
							if (configParam.Value ==
								"好きなファイヤーボールを、３種類から\r\n選んで下さい。私はサービスが良いので、\r\n何度でも選べますよ。") goto IL_718;
							else break;
						case "fs_serifu1":
							if (configParam.Value == "どのファイヤーボールにしますか？") goto IL_718;
							else break;
						case "fs_item_name1":
							if (configParam.Value == "バウンド") goto IL_718;
							else break;
						case "fs_item_name2":
							if (configParam.Value == "ストレート") goto IL_718;
							else break;
						case "fs_item_name3":
							if (configParam.Value == "ダブル") goto IL_718;
							else break;
						case "fs_serifu2":
							if (configParam.Value == "を装備しました。") goto IL_718;
							else break;
						case "filename_second_haikei":
						case "filename_second_haikei2":
						case "filename_second_haikei3":
						case "filename_second_haikei4":
							if (configParam.Value == "haikei_second.gif") goto IL_718;
							else break;
						case "serifu_key1_on":
							if (configParam.Value ==
								"ここから先へ進むには、\r\n３つのＫＥＹ１が必要です。\r\nこの世界のどこかに、あるはず。") goto IL_718;
							else break;
						case "serifu_key1_on-4":
							if (configParam.Value == "ＫＥＹ１を３つ、わたしますか？") goto IL_718;
							else break;
						case "serifu_key1_on-7":
							if (configParam.Value == "ＫＥＹ１を３つ、持っていません。") goto IL_718;
							else break;
						case "serifu_key1_on-8":
							if (configParam.Value ==
								"先へ進む道が、開けました。\r\n勇者殿、\r\nお気を付けて。") goto IL_718;
							else break;
						case "key1_on_count":
						case "key2_on_count":
						case "oriboss_waza_select_option":
							if (configParam.Value == "3") goto IL_718;
							else break;
						case "serifu_key2_on":
							if (configParam.Value ==
								"３つのＫＥＹ２がないと、\r\nここから先へは進めないぜ。\r\nどこかで見つ付けてくれ。") goto IL_718;
							else break;
						case "serifu_key2_on-4":
							if (configParam.Value == "ＫＥＹ２を３つ、わたしますか？") goto IL_718;
							else break;
						case "serifu_key2_on-7":
							if (configParam.Value == "ＫＥＹ２を３つ、持っていません。") goto IL_718;
							else break;
						case "serifu_key2_on-8":
							if (configParam.Value ==
								"３つのＫＥＹ２、受け取ったぜ。\r\nこれで、先へ進めるようになったな。\r\n0") goto IL_718;
							else break;
						case "water_clear_level":
							if (configParam.Value == "128") goto IL_718;
							else break;
						case "serifu_grenade_shop":
							if (configParam.Value ==
								"グレネード１発を、\r\n２０点で売りますよ。\r\n0") goto IL_718;
							else break;
						case "serifu_grenade_shop-4":
							if (configParam.Value == "何発にしますか？") goto IL_718;
							else break;
						case "serifu_grenade_shop-5":
							if (configParam.Value == "得点が、足りません。") goto IL_718;
							else break;
						case "serifu_grenade_shop-6":
							if (configParam.Value == "グレネードを手に入れた。") goto IL_718;
							else break;
						case "font_score":
						case "font_message":
							if (configParam.Value == "Helvetica,Arial,ＭＳ ゴシック,HG ゴシックB Sun,HG ゴシックB,monospace") goto IL_718;
							else break;
						case "oriboss_width":
						case "oriboss_height":
							if (configParam.Value == "32") goto IL_718;
							else break;
					}
				}

				if (configParam.Typestr == "text" || configParam.Typestr == "string") // 文字列に"または\が含まれていた場合エスケーピング
				{
					configParam.Value = configParam.Value.Replace(@"\", @"\\");
					configParam.Value = configParam.Value.Replace(@"""", @"\""");
				}

				string typestr;
				switch (typestr = configParam.Typestr)
				{
				case "bool":
				case "bool21":
					stringBuilder.AppendLine(string.Format(parameter, configParam.Name, (configParam.Value == "true") ? "1" : "2"));
					goto IL_718;
				case "bool10":
					stringBuilder.AppendLine(string.Format(parameter, configParam.Name, (configParam.Value == "true") ? "1" : "0"));
					goto IL_718;
				case "int":
				case "list":
				case "list_athletic":
				case "string":
				case "file":
				case "file_img":
				case "file_audio":
					stringBuilder.AppendLine(string.Format(parameter, configParam.Name, configParam.Value));
					goto IL_718;
				case "text":
				{
					string[] array2 = configParam.Value.Split(new string[]
					{
						Environment.NewLine
					}, StringSplitOptions.None);
					int num2 = 1;

					Regex text_name_regx = new Regex(@"-(\d+)$");
					Match text_name_match = text_name_regx.Match(configParam.Name);
					if(text_name_match.Success){
						num2 = int.Parse(text_name_match.Groups[1].Value);
						configParam.Name = text_name_regx.Replace(configParam.Name, string.Empty);
					}

					foreach (string arg in array2)
					{
						stringBuilder.AppendLine(string.Format(parameter, configParam.Name + "-" + num2.ToString(), arg));
						num2++;
					}
					goto IL_718;
				}
				case "color":
				{
					Colors colors = new Colors(configParam.Value);
					string param  = configParam.Name;

					if (Global.config.localSystem.OutPutInititalSourceCode || Global.cpd.runtime.Definitions.Package.Contains("28")
					   || !((param == "backcolor_@" || param == "scorecolor_@" || param == "mizunohadou_@" || param == "kaishi_@" || param == "backcolor_@_s" || param == "backcolor_@_t" || param == "message_back_@" || param == "message_name_@") && colors.r == 0
					   || (param == "grenade_@1" || param == "grenade_@2" || param == "firebar_@1" || param == "firebar_@2" || param == "message_border_@" || param == "message_text_@" || param == "gauge_border_@" || param == "gauge_back_@1" || param == "gauge_back_@2") && colors.r == 255
					   || param == "backcolor_@_f" && colors.r == 192
					))
					stringBuilder.AppendLine(string.Format(parameter, configParam.Name.Replace("@", "red"), colors.r.ToString()));
					
					if (Global.config.localSystem.OutPutInititalSourceCode || Global.cpd.runtime.Definitions.Package.Contains("28")
					   || !((param == "scorecolor_@" || param == "kaishi_@" || param == "firebar_@1" || param == "backcolor_@_s" || param == "message_back_@" || param == "gauge_back_@2") && colors.g == 0
					   || (param == "backcolor_@" || param == "grenade_@1" || param == "grenade_@2" || param == "backcolor_@_t" || param == "message_border_@" || param == "message_name_@" || param == "message_text_@" || param == "gauge_border_@" || param == "gauge_back_@1") && colors.g == 255
					   || param == "mizunohadou_@" && colors.g == 32
					   || param == "firebar_@2" && colors.g == 192
					   || param == "backcolor_@_f" && colors.g == 48
					))
					stringBuilder.AppendLine(string.Format(parameter, configParam.Name.Replace("@", "green"), colors.g.ToString()));
					
					if (Global.config.localSystem.OutPutInititalSourceCode || Global.cpd.runtime.Definitions.Package.Contains("28")
					   || !((param == "grenade_@2" || param == "firebar_@1" || param == "firebar_@2" || param == "kaishi_@" || param == "backcolor_@_s" || param == "message_back_@" || param == "gauge_back_@1" || param == "gauge_back_@2") && colors.b == 0
					   || (param == "backcolor_@" || param == "scorecolor_@" || param == "grenade_@1" || param == "mizunohadou_@" || param == "backcolor_@_t" || param == "message_border_@" || param == "message_name_@" || param == "message_text_@" || param == "gauge_border_@") && colors.b == 255
					   || param == "backcolor_@_f" && colors.b == 48
					))
					stringBuilder.AppendLine(string.Format(parameter, configParam.Name.Replace("@", "blue"), colors.b.ToString()));

					goto IL_718;
				}
				}
				throw new Exception("不明な型が含まれています:" + configParam.Typestr);
			}

			// 中間を出力
			text = Subsystem.DecodeBase64(Global.cpd.runtime.DefaultConfigurations.MiddleHTML);
			if (Global.cpd.runtime.DefaultConfigurations.OutputReplace.Length > 0)
			{
				foreach (HTMLReplaceData htmlreplaceData in Global.cpd.runtime.DefaultConfigurations.OutputReplace)
				{
					text = text.Replace("<?" + htmlreplaceData.Name + ">", htmlreplaceData.Value);
				}
			}
			foreach (string value in text.Split(new string[]
			{
				Environment.NewLine,
				"\r",
				"\n"
			}, StringSplitOptions.None))
			{
				stringBuilder.AppendLine(value);
			}

			// オプションを出力
			parameter = "\t{0}: {1},";
			for (k = 0; k < configurations.Length; k++)
			{
				ConfigParam configParam = configurations[k];

				if (configParam.Category != "オプション" ||
					!Global.config.localSystem.OutPutInititalSourceCode && ( // 初期値を出力しない(2.8含む)
						configParam.Value == "false" || // 値が "false" である
						(mcs_screen_size == 1 && // スクリーンサイズが640×480である
							(
								(configParam.Name == "width" && configParam.Value == "640") || (configParam.Name == "height" && configParam.Value == "480")
							)
						) ||
						(mcs_screen_size == 2 && // スクリーンサイズが512×320である
							(
								(configParam.Name == "width" && configParam.Value == "512") || (configParam.Name == "height" && configParam.Value == "320")
							)
						)
					)) continue;

				stringBuilder.AppendLine(string.Format(parameter, configParam.Name, configParam.Value));
			}

			//フッターを出力
			text = Subsystem.DecodeBase64(Global.cpd.runtime.DefaultConfigurations.FooterHTML);
			if (Global.cpd.runtime.DefaultConfigurations.OutputReplace.Length > 0)
			{
				foreach (HTMLReplaceData htmlreplaceData2 in Global.cpd.runtime.DefaultConfigurations.OutputReplace)
				{
					text = text.Replace("<?" + htmlreplaceData2.Name + ">", htmlreplaceData2.Value);
				}
			}
			foreach (string value2 in text.Split(new string[]
			{
				Environment.NewLine,
				"\r",
				"\n"
			}, StringSplitOptions.None))
			{
				stringBuilder.AppendLine(value2);
			}

			//末尾の,を除去（文法的にはセーフだが）
			string result = new Regex(@",(\s*?)}").Replace(stringBuilder.ToString(), "$1}");

			//3連続以上の改行を2連続改行に
			result = new Regex(@"(\r\n){3,}").Replace(result, "\r\n\r\n");

			return result;
		}

		public static string DecodeBase64(string s)
		{
			return s;
		}

		public static string EncodeBase64(string str)
		{
			return str;
		}

		public static string MakeStageParameter(string Parameter, int StageSplit, string[] StageText, Runtime.DefinedData.StageSizeData StageSizeData, bool notdefaultparam = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder[] array = new StringBuilder[StageSplit + 1];

			StringBuilder null_string = new StringBuilder(), null_string_all = new StringBuilder();
			for (int j = 0; j < StageSizeData.x / (StageSplit + 1); j++)
				for(int i = 0; i < StageSizeData.bytesize; i++)
					null_string.Append("."); // 空白文字をベタ書きしてるので後で直す？
			for (int i = 0; i <= StageSplit; i++)
				null_string_all.Append(null_string);
			int num = 0; // 何行目か
			foreach (string text in StageText)
			{
				if (StageSplit != 0 && text.Length % (StageSplit + 1) != 0)
				{
					throw new Exception("分割数の設定が異常です。");
				}
				if (StageSplit != 0 && !Global.config.localSystem.OutPutInititalSourceCode && !Global.cpd.runtime.Definitions.Package.Contains("28")
					   && text == null_string_all.ToString()) goto SKIP1; // 全部空白の行を省略
				int num2 = 0; // 何文字目から切り取るか
				for (int j = 0; j <= StageSplit; j++)
				{
					if (array[j] == null)
					{
						array[j] = new StringBuilder();
					}
					if (StageSplit == 0) // 地図画面
					{
						array[j].AppendLine(string.Format(Parameter, new object[]
						{
							num,
							text.Substring(num2, Global.cpd.runtime.Definitions.MapSize.x / (StageSplit + 1)) // 定義されたマップ幅まで
						}));
					}
					else
					{
						array[j].AppendLine(string.Format(Parameter, new object[]
						{
							j,
							num,
							text.Substring(num2, text.Length / (StageSplit + 1))
						}));
					}
					num2 += text.Length / (StageSplit + 1);
				}
				if (!Global.config.localSystem.OutPutInititalSourceCode && !Global.cpd.runtime.Definitions.Package.Contains("28"))  // 省略
				{// マップデータを二次元配列に入れて管理した方がいいかも　要改善
					array[StageSplit].Replace(string.Format(Parameter, new object[]
							   {
								StageSplit,
								num,
								null_string.ToString()
							   }) + "\r\n", string.Empty);
					for (int j = StageSplit - 1; j > 0; j--)
					{
						if (!array[j + 1].ToString().Contains((j + 1).ToString() + "-" + num.ToString()))
						{
							array[j].Replace(string.Format(Parameter, new object[]
							{
								j,
								num,
								null_string.ToString()
							}) + "\r\n", string.Empty);
							break;
						}
					}
				}
						
				SKIP1:
				num++;
				if (StageSplit == 0 && num == Global.cpd.runtime.Definitions.MapSize.y) break; // 定義されたマップ高さから下は省略
			}
			
			foreach (StringBuilder stringBuilder2 in array)
			{
				if(stringBuilder2 != null) stringBuilder.AppendLine(stringBuilder2.ToString());
			}

			if(notdefaultparam && new Regex(@"^\s*?$").Match(stringBuilder.ToString()).Success) { // 出力結果が空白のみの場合
				stringBuilder.AppendLine(string.Format(Parameter, new object[]{0,0,".."})); //
			}

			return stringBuilder.ToString();
		}

		public static string LoadUnknownTextFile(string path)
		{
			if (!File.Exists(path))
			{
				throw new FileNotFoundException("ファイルをロードできませんでした。");
			}
			byte[] array = new byte[0];
			using (FileStream fileStream = new FileStream(path, FileMode.Open))
			{
				array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
			}
			return Subsystem.GetCode(array).GetString(array);
		}

		public static Encoding GetCode(byte[] byts)
		{
			int num = byts.Length;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			for (int i = 0; i < num; i++)
			{
				if (byts[i] <= 6 || byts[i] == 127 || byts[i] == 255)
				{
					num2++;
					if (num - 1 > i && byts[i] == 0 && i > 0 && byts[i - 1] <= 127)
					{
						num3++;
					}
				}
			}
			if (num2 > 0)
			{
				if (num3 > 0)
				{
					return Encoding.Unicode;
				}
				return null;
			}
			else
			{
				for (int j = 0; j < num - 1; j++)
				{
					byte b = byts[j];
					byte b2 = byts[j + 1];
					if (b == 27)
					{
						if (b2 >= 128)
						{
							return Encoding.ASCII;
						}
						if (num - 2 > j && b2 == 36 && byts[j + 2] == 64)
						{
							return Encoding.GetEncoding(50220);
						}
						if (num - 2 > j && b2 == 36 && byts[j + 2] == 66)
						{
							return Encoding.GetEncoding(50220);
						}
						if (num - 5 > j && b2 == 38 && byts[j + 2] == 64 && byts[j + 3] == 27 && byts[j + 4] == 36 && byts[j + 5] == 66)
						{
							return Encoding.GetEncoding(50220);
						}
						if (num - 3 > j && b2 == 36 && byts[j + 2] == 40 && byts[j + 3] == 68)
						{
							return Encoding.GetEncoding(50220);
						}
						if (num - 2 > j && b2 == 40 && (byts[j + 2] == 66 || byts[j + 2] == 74))
						{
							return Encoding.GetEncoding(50220);
						}
						if (num - 2 > j && b2 == 40 && byts[j + 2] == 73)
						{
							return Encoding.GetEncoding(50220);
						}
					}
				}
				for (int k = 0; k < num - 1; k++)
				{
					byte b = byts[k];
					byte b2 = byts[k + 1];
					if (((b >= 129 && b <= 159) || (b >= 224 && b <= 252)) && ((b2 >= 64 && b2 <= 126) || (b2 >= 128 && b2 <= 252)))
					{
						num4 += 2;
						k++;
					}
				}
				for (int l = 0; l < num - 1; l++)
				{
					byte b = byts[l];
					byte b2 = byts[l + 1];
					if ((b >= 161 && b <= 254 && b2 >= 161 && b2 <= 254) || (b == 142 && b2 >= 161 && b2 <= 223))
					{
						num5 += 2;
						l++;
					}
					else if (num - 2 > l && b == 143 && b2 >= 161 && b2 <= 254 && byts[l + 2] >= 161 && byts[l + 2] <= 254)
					{
						num5 += 3;
						l += 2;
					}
				}
				for (int m = 0; m < num - 1; m++)
				{
					byte b = byts[m];
					byte b2 = byts[m + 1];
					if (b >= 192 && b <= 223 && b2 >= 128 && b2 <= 191)
					{
						num6 += 2;
						m++;
					}
					else if (num - 2 > m && b >= 224 && b <= 239 && b2 >= 128 && b2 <= 191 && byts[m + 2] >= 128 && byts[m + 2] <= 191)
					{
						num6 += 3;
						m += 2;
					}
				}
				if (num5 > num4 && num5 > num6)
				{
					return Encoding.GetEncoding(51932);
				}
				if (num4 > num5 && num4 > num6)
				{
					return Encoding.GetEncoding(932);
				}
				if (num6 > num5 && num6 > num4)
				{
					return Encoding.UTF8;
				}
				return Encoding.Default;
			}
		}

		public static string ExtractZipArchive(string InputArchive)
		{
			string text = "";
			while (text == "" || Directory.Exists(text))
			{
				text = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			}
			try
			{
				Directory.CreateDirectory(text);
			}
			catch
			{
				return null;
			}
			using (FileStream fileStream = new FileStream(InputArchive, FileMode.Open))
			{
				using (ZipInputStream zipInputStream = new ZipInputStream(fileStream))
				{
					ZipEntry nextEntry;
					while ((nextEntry = zipInputStream.GetNextEntry()) != null)
					{
						if (!nextEntry.IsDirectory)
						{
							string fileName = Path.GetFileName(nextEntry.Name);
							string text2 = Path.Combine(text, Path.GetDirectoryName(nextEntry.Name));
							Directory.CreateDirectory(text2);
							string path = Path.Combine(text2, fileName);
							using (FileStream fileStream2 = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
							{
								byte[] array = new byte[Global.definition.ZipExtractBufferLength];
								int count;
								while ((count = zipInputStream.Read(array, 0, array.Length)) > 0)
								{
									fileStream2.Write(array, 0, count);
								}
							}
						}
					}
				}
			}
			return text;
		}

		public static bool InstallRuntime(string Source)
		{
			string text = Subsystem.ExtractZipArchive(Source);
			string[] files = Directory.GetFiles(text, "*.xml", SearchOption.TopDirectoryOnly);
			if (files.Length != 1)
			{
				MessageBox.Show("ランタイムを特定できません。" + Environment.NewLine + "インストールに失敗しました。", "ランタイム インストール エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			string fileName = Path.GetFileName(files[0]);
			if (!File.Exists(Path.Combine(text, fileName)))
			{
				MessageBox.Show("定義ファイルの展開に失敗しました。" + Environment.NewLine + "インストールに失敗しました。", "ランタイム インストール エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			Runtime runtime = Runtime.ParseXML(Path.Combine(text, fileName));
			if (Global.definition.CheckVersion < runtime.Definitions.RequireLower)
			{
				MessageBox.Show(string.Concat(new string[]
				{
					"定義ファイルはこのバージョンの",
					Global.definition.AppName,
					"には対応していません。",
					Environment.NewLine,
					Global.definition.AppName,
					"を最新のバージョンへ更新してください。"
				}), "インストール拒否", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			string text2 = Path.Combine(Application.StartupPath, Global.definition.RuntimeDir);
			string text3 = Path.Combine(text2, fileName);
			if (File.Exists(text3))
			{
				if (MessageBox.Show(fileName + "はすでに存在します。" + Environment.NewLine + "上書きインストールして更新しますか?", "ランタイムパッケージの更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					using (SaveFileDialog saveFileDialog = new SaveFileDialog())
					{
						saveFileDialog.InitialDirectory = text2;
						saveFileDialog.FileName = fileName;
						saveFileDialog.DefaultExt = ".xml";
						if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
						{
							return false;
						}
						text3 = saveFileDialog.FileName;
						goto IL_20A;
					}
				}
				Runtime runtime2 = Runtime.ParseXML(text3);
				if (runtime2.Definitions.DefVersion >= runtime.Definitions.DefVersion && MessageBox.Show(string.Concat(new string[]
				{
					"上書きインストールしようとしているランタイムは、",
					Environment.NewLine,
					"現在のランタイムと同等か、それより過去のバージョンです。",
					Environment.NewLine,
					"上書きしてよろしいですか？"
				}), "ダウングレード警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
				{
					return false;
				}
			}
			IL_20A:
			File.Copy(Path.Combine(text, fileName), text3, true);
			text3 = Path.Combine(Path.GetDirectoryName(text3), Path.GetFileNameWithoutExtension(text3));
			Directory.CreateDirectory(text3);
			foreach (string text4 in Directory.GetFiles(Path.Combine(text, Path.GetFileNameWithoutExtension(fileName)), "*", SearchOption.TopDirectoryOnly))
			{
				string text5 = Path.Combine(text3, Path.GetFileName(text4));
				if (File.Exists(text5))
				{
					File.Delete(text5);
				}
				File.Move(text4, text5);
			}
			Directory.Delete(text, true);
			return true;
		}

		public static void UpdateAutoCheck()
		{
			if (!Global.definition.IsAutoUpdateEnabled)
			{
				return;
			}
			if (Global.config.localSystem.UpdateServer != Global.definition.BaseUpdateServer && MessageBox.Show("アップデート接続先サーバーが変更されています。" + Environment.NewLine + "既定のサーバーを利用しますか？", "更新先の変更の検知", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Global.config.localSystem.UpdateServer = Global.definition.BaseUpdateServer;
			}
			Subsystem.dlClient = new WebClient();
			Subsystem.dlClient.Headers.Add("User-Agent", string.Concat(new string[]
            {
                Global.definition.AppName,
                " - ",
                Global.definition.AppNameFull,
                "/",
                Global.definition.Version,
                "(Windows NT 10.0; Win64; x64)"
            }));
			Subsystem.dlClient.DownloadFileCompleted += Subsystem.dlClient_DownloadFileCompleted;
			Subsystem.tempfile = Path.GetTempFileName();
			Uri address = new Uri(Global.config.localSystem.UpdateServer);
			try
			{
				Subsystem.dlClient.DownloadFileAsync(address, Subsystem.tempfile);
			}
			catch
			{
			}
		}

		private static void dlClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			Subsystem.dlClient.Dispose();
			if (e.Error != null)
			{
				return;
			}
			UpdateData updateData = UpdateData.ParseXML(Subsystem.tempfile);
			File.Delete(Subsystem.tempfile);
			if (updateData.DefVersion <= Global.definition.CheckVersion)
			{
				return;
			}
			if (MessageBox.Show(string.Concat(new string[]
			{
				"Sideの新しいバージョンが公開されています。",
				Environment.NewLine,
				"(バージョン ",
				updateData.Name.ToString(),
				")",
				Environment.NewLine,
				"更新しますか？"
			}), "Sideの更新", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.No)
			{
				MessageBox.Show("自動更新チェックはオフになります。" + Environment.NewLine + "再度有効にする場合はエディタオプションより設定してください。", "更新の中止", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				Global.config.localSystem.CheckAutoUpdate = false;
				return;
			}
			using (WebUpdate webUpdate = new WebUpdate())
			{
				if (webUpdate.ShowDialog() == DialogResult.Retry)
				{
					Global.state.RunFile = (string)webUpdate.runfile.Clone();
					Global.MainWnd.Close();
				}
			}
		}

		private static WebClient dlClient;

		private static string tempfile;
	}
}
