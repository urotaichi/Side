# Microsoft Store 用 MSIX パッケージガイド

Side を Microsoft Store に提出するためのビルド・署名・テスト手順です。

## 全体の流れ

```
1. Build-Msix.ps1     → ビルド＆MSIX生成
2. Sign-And-Install-MSIX.ps1 → 署名＆ローカルテスト
3. Partner Center にアップロード
```

## 必要な環境

- Visual Studio 2022（MSBuild）
- Windows SDK（MakeAppx.exe、SignTool.exe）
- .NET 10 SDK
- PowerShell 7 以上

## ステップ 1: MSIX パッケージのビルド

### コマンド

```powershell
# 通常のビルド（Store構成ビルド → MSIX生成 → .msixupload生成）
pwsh -File .\Build-Msix.ps1

# プラットフォーム指定
pwsh -File .\Build-Msix.ps1 -Platform x64

# ビルド済みの場合はスキップ
pwsh -File .\Build-Msix.ps1 -SkipBuild
```

### パラメータ

| パラメータ | デフォルト | 説明 |
|-----------|-----------|------|
| `-Configuration` | `Store` | ビルド構成 |
| `-Platform` | `x64` | ターゲット (`x64`, `x86`, `arm64`) |
| `-SkipBuild` | なし | ビルド済みの場合にビルドをスキップ |

### 出力ファイル

`side/AppPackages/` に以下が生成されます：

- `Side_X.X.X.X_x64.msix` — MSIX パッケージ
- `Side_X.X.X.X.msixupload` — Partner Center アップロード用

### パッケージに含まれるフォルダ

| フォルダ/ファイル | 説明 | 必須 |
|-----------------|------|------|
| `runtime/` | ランタイム定義ファイル（XML）とリソース | ✅ |
| `pictures/` | デフォルト画像リソース | ✅ |
| `projects/` | サンプルプロジェクト | ✅ |
| `readme.txt` | リリースノート | ✅ |
| `side.exe` | アプリ本体 | ✅ |
| `使い方.txt` | 使用方法の説明 | 推奨 |
| `勝手にFx14説明書.txt` | 機能説明 | 推奨 |

> **重要**: `runtime/` フォルダが含まれないと起動時に「ランタイムフォルダが見つかりません」エラーが発生します。`side.csproj` の Content 設定を確認してください。

## ステップ 2: 署名とローカルテスト

MSIX パッケージのインストールにはデジタル署名が必須です。

### 方法 A: Visual Studio でデバッグ実行（署名不要・最も簡単）

1. Visual Studio で `side.sln` を開く
2. Configuration を `Store`、Platform を `x64` に設定
3. F5 キーでデバッグ実行

### 方法 B: Sign-And-Install-MSIX.ps1 で署名＆インストール

```powershell
# 管理者権限の PowerShell で実行
.\Sign-And-Install-MSIX.ps1
```

このスクリプトは以下を自動実行します：

1. 自己署名証明書の作成（Code Signing EKU + Basic Constraints）
2. PFX・CER ファイルのエクスポート
3. 証明書を Trusted People ストアにインストール（管理者権限が必要）
4. SignTool で MSIX に署名
5. 署名済み MSIX をインストール

### 証明書について

- **Subject**: `CN=2086D10A-4660-4D0C-AAB2-595399EE7B10`（MSIX 内の Publisher と一致）
- **配置場所**:
  - `Cert:\CurrentUser\My` — 署名用（秘密鍵あり）
  - `Cert:\LocalMachine\TrustedPeople` — インストール用（公開証明書のみ）
- **有効期限**: デフォルト 1 年（`Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert` で確認）
- **ファイル**:
  - `SideTestCert.pfx` — 秘密鍵含む（署名用、Git にコミットしない）
  - `SideTestCert.cer` — 公開証明書のみ（テストマシンに配布）

### 手動で証明書をインストールする場合

```powershell
# 管理者権限で実行
Import-Certificate -FilePath ".\SideTestCert.cer" -CertStoreLocation "Cert:\LocalMachine\TrustedPeople"
```

### 手動でパッケージをインストールする場合

```powershell
Add-AppxPackage -Path '.\side\AppPackages\Side_4.8.0.0_x64_Signed.msix'
```

## ステップ 3: Partner Center へのアップロード

1. [Partner Center](https://partner.microsoft.com) にログイン
2. Side アプリを選択
3. 「パッケージ」セクションで `.msixupload` ファイルをアップロード

> Store 提出時、Microsoft が独自の証明書でパッケージに再署名します。自己署名証明書は不要です。

## テスト確認項目

- [ ] アプリが正常に起動する
- [ ] 「ランタイムフォルダが見つかりません」エラーが出ない
- [ ] 新規プロジェクト作成が動作する
- [ ] サンプルプロジェクトが開ける

## GitHub Actions

`.github/workflows/update-version-and-release.yml` で自動ビルド：

1. `readme.txt` を更新してバージョンを変更
2. master にプッシュ
3. ワークフローが自動実行 → Windows 通常版（ZIP）+ Store 版（.msixupload）を生成
4. GitHub リリースに添付

## トラブルシューティング

### 「ランタイムフォルダが見つかりません」

`side.csproj` の Content セクションに `runtime/` のコピー設定があるか確認。

### 「サブジェクトに署名がありませんでした」(0x800B0100)

MSIX が未署名。`Sign-And-Install-MSIX.ps1` を管理者権限で実行。

### SignerSign() failed (0x8007000b)

証明書の CN と MSIX 内の Publisher が不一致。イベントビューアの `AppxPackagingOM > Operational` で Event ID 150 を確認。

### 「MakeAppx.exe not found」

Windows SDK をインストール（Visual Studio Installer から）。

## スクリプト一覧

| スクリプト | 役割 |
|-----------|------|
| `Build-Msix.ps1` | Store ビルド → ファイル検証 → MSIX 生成 → .msixupload 生成 |
| `Sign-And-Install-MSIX.ps1` | 自己署名証明書の作成 → MSIX 署名 → ローカルインストール |
