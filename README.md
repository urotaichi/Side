# Side - the Supermasao Integrated Development Environment

Sideは高機能な正男エディタです。正男のステージ作成に必要な機能を統合的に提供します。

## ✨特徴

- 🖼️ グラフィカルなステージエディタ
- 🗂️ マルチレイヤー対応
- ⚡ 多彩なランタイムのサポート
- 🛠️ 豊富なプロパティ設定
- 🖌️ Canvas正男完全対応
- 📁 プロジェクトベースの管理機能
- 🕹️ テストプレイの統合サポート

## 💻動作環境

- 🪟 Windows環境
- 🟦 .NET 9.0以降
- 🌐 WebView2ランタイム

## 🛠️機能一覧

### 📝エディタ機能
- 🖼️ グラフィカルなステージ編集
- 📝 テキストベースのソース編集
- 🧩 マップチップのドラッグ＆ドロップ
- 📋 コピー＆ペースト
- ↩️ 元に戻す・やり直し
- 🟫 グリッド表示
- 🗂️ レイヤー編集
- 🗺️ オーバービュー表示

### 🧰チップ配置ツール
- ✏️ ペンツール：1マスずつ配置
- 📏 ラインツール：直線状に配置
- ▫️ 四角形塗り潰し：矩形選択して塗り潰し
- 🪣 塗り潰しツール：領域を塗り潰し
- 🗂️ マルチレイヤー対応
- 🌄 背景レイヤー編集

### 🕹️テスト機能
- 🌐 内蔵ブラウザでのテストプレイ
- ⏩ 途中位置からのテスト実行
- 🌍 外部ブラウザでのテスト

### 📁プロジェクト管理
- 📦 プロジェクトベースの管理（.spjファイル）
- 🗃️ 複数ステージの一括管理
- 🗺️ 地図画面対応
- ⚙️ プロジェクト設定の一元管理

## 📄対応ファイル形式

### 📥入力形式
- 📦 Side プロジェクトファイル (.spj)
- 🌐 HTMLファイル (.html)
- 📜 JavaScriptファイル (.js)
- 🗂️ masao-json-format (.json)
- 🖼️ 画像ファイル (PNG, GIF, JPEG)
- 🔊 音声ファイル (MP3, WAV, OGG*)

### 📤出力形式
- 🌐 HTMLファイル
- 📜 JavaScriptファイル
- 🗂️ masao-json-format
- 🖼️ ステージ画像 (PNG)

*OGGファイルの再生にはLAVFiltersのインストールが必要です。

## 🖱️基本操作方法

### 🖱️エディタの基本操作
- 🖱️ 左クリック：チップの配置
- 🖱️ 右クリック：カーソルモード時にメニュー表示
- 🖱️ ホイール：マップのスクロール
- Shift⇧+🖱️ホイール：横方向スクロール
- ⌨️ Ctrl+Z：元に戻す
- ⌨️ Ctrl+Y：やり直し
- ⌨️ Ctrl+C：コピー
- ⌨️ Ctrl+X：切り取り
- ⌨️ Ctrl+V：貼り付け
- ⌨️ Ctrl+Space：テストプレイ
- ⌨️ Ctrl+Shift⇧：グラフィカルデザイナとチップリストの切り替え

### 🕹️テストプレイ
- ⌨️ F5またはCtrl+Space：テストプレイ開始
- 🖱️ ホイールクリック：クリックした位置からテストプレイ
- 🖱️ 右クリック→「ここからテスト実行」：選択位置からテストプレイ

### 🗂️レイヤー編集
- 🗂️ ツールバーのレイヤーボタン：レイヤーの切り替え
- 👁️ レイヤーメニュー：表示/非表示の切り替え
- ⭐ アクティブレイヤーの設定
- 🌄 背景レイヤーの編集

### 💡便利な機能
- 🔗 プロジェクトの継承
- ⚙️ HTML出力時の自動ファイル設定
- 🎵 BGM・効果音のプレビュー
- 🔄 ステージの反転
- 🗺️ オーバービュー表示
- 🧩 カスタムパーツの管理

## 📜ライセンス

このプロジェクトはMITライセンスの下で提供されています：

```
MIT License

Copyright (c) 2020 urotaichi

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## 👨‍💻開発・メンテナンス

現在はurotaichiが開発・メンテナンスを行っています。

- 🌐 開発者サイト: [urotaichi corporation](https://urotaichi.com/)
- 📖 詳しい使い方: [Sideを使って正男を作成する - スーパー正男～GMS～](http://masao.kame33.com/setsumei/sakusei/side/)

## 謝辞

- オリジナル作者: Karno氏
- 過去の改造: タケヒロ氏
