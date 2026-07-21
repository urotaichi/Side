# MSIX パッケージに自己署名証明書で署名してインストールするスクリプト
# ローカル開発環境でのテスト用

param(
    [string]$MsixPath = "",
    [string]$CertPassword = "TestPassword123"
)

Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║       MSIX パッケージ署名 & インストール スクリプト            ║" -ForegroundColor Cyan
Write-Host "║              (ローカルテスト用)                               ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# ============================================================================
# ステップ1: MSIX ファイルを探す
# ============================================================================

Write-Host "[1/6] MSIX ファイルを検索中..." -ForegroundColor Yellow

if (-not $MsixPath) {
    # 最新の MSIX ファイルを検索
    $msixFiles = Get-ChildItem -Path "side\AppPackages" -Filter "*.msix" -Recurse -ErrorAction SilentlyContinue |
        Sort-Object -Property LastWriteTime -Descending
    
    if ($msixFiles.Count -eq 0) {
        Write-Host "❌ MSIX ファイルが見つかりません" -ForegroundColor Red
        Write-Host "   まず Build-Store-Package.ps1 を実行してください" -ForegroundColor Yellow
        exit 1
    }
    
    $MsixPath = $msixFiles[0].FullName
}

if (-not (Test-Path $MsixPath)) {
    Write-Host "❌ MSIX ファイルが見つかりません: $MsixPath" -ForegroundColor Red
    exit 1
}

Write-Host "  ✓ MSIX ファイル: $(Split-Path -Leaf $MsixPath)" -ForegroundColor Green
Write-Host "    サイズ: $([math]::Round((Get-Item $MsixPath).Length / 1MB, 2)) MB" -ForegroundColor Cyan

# ============================================================================
# ステップ2: 自己署名証明書を作成（Code Signing EKU + Basic Constraints）
# ============================================================================

Write-Host "`n[2/6] 自己署名証明書を作成中..." -ForegroundColor Yellow

# Package.appxmanifest の Publisher と完全一致させる必要がある
# Package.appxmanifest: Publisher="CN=2086D10A-4660-4D0C-AAB2-595399EE7B10"
$certSubject = "CN=2086D10A-4660-4D0C-AAB2-595399EE7B10"
$certName = "SideStoreTestCert"
$certPath = Join-Path $PSScriptRoot "SideStoreTestCert.pfx"
$cerPath = Join-Path $PSScriptRoot "SideStoreTestCert.cer"

# 既存の証明書を確認（Code Signing EKU付き）
$existingCert = Get-ChildItem -Path Cert:\CurrentUser\My -CodeSigningCert | 
    Where-Object { $_.Subject -eq $certSubject } | 
    Select-Object -First 1

if ($existingCert) {
    Write-Host "  ✓ 既存の証明書を使用: $certSubject" -ForegroundColor Green
    Write-Host "    拇印: $($existingCert.Thumbprint)" -ForegroundColor Cyan
    Write-Host "    有効期限: $($existingCert.NotAfter)" -ForegroundColor Cyan
    $cert = $existingCert
} else {
    Write-Host "  新しい証明書を作成中..." -ForegroundColor Cyan
    
    # 自己署名証明書を作成
    # -TextExtension で Code Signing EKU (1.3.6.1.5.5.7.3.3) と Basic Constraints を設定
    $cert = New-SelfSignedCertificate `
        -Type Custom `
        -Subject $certSubject `
        -KeyUsage DigitalSignature `
        -FriendlyName "Side MSIX Signing Certificate" `
        -CertStoreLocation "Cert:\CurrentUser\My" `
        -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}")
    
    Write-Host "  ✓ 証明書を作成しました" -ForegroundColor Green
    Write-Host "    拇印: $($cert.Thumbprint)" -ForegroundColor Cyan
    Write-Host "    有効期限: $($cert.NotAfter)" -ForegroundColor Cyan
}

# 証明書を PFX ファイルにエクスポート（秘密鍵を含む - 署名用）
$securePwd = ConvertTo-SecureString -String $CertPassword -Force -AsPlainText
Export-PfxCertificate -Cert $cert -FilePath $certPath -Password $securePwd -Force | Out-Null
Write-Host "  ✓ PFX をエクスポート: $certPath" -ForegroundColor Green

# 証明書を CER ファイルにエクスポート（公開証明書のみ - 配布用）
Export-Certificate -Cert $cert -FilePath $cerPath -Force | Out-Null
Write-Host "  ✓ CER をエクスポート: $cerPath" -ForegroundColor Green

# ============================================================================
# ステップ3: 証明書を信頼されたユーザー (Trusted People) / ルート (Trusted Root) にインストール
# ============================================================================

Write-Host "`n[3/6] 証明書を信頼されたユーザーにインストール中..." -ForegroundColor Yellow

# Trusted People ストアを確認（コード署名の検証に利用）
$trustedPeopleCert = Get-ChildItem -Path Cert:\LocalMachine\TrustedPeople -ErrorAction SilentlyContinue | 
    Where-Object { $_.Subject -eq $certSubject }

# ルートストアを確認（自己署名証明書の信頼チェーンに必要）
$trustedRootCert = Get-ChildItem -Path Cert:\LocalMachine\Root -ErrorAction SilentlyContinue |
    Where-Object { $_.Subject -eq $certSubject }

if ($trustedPeopleCert -and $trustedRootCert) {
    Write-Host "  ✓ 証明書は既に Trusted People / Trusted Root にインストールされています" -ForegroundColor Green
} else {
    Write-Host "  証明書を Trusted People / Trusted Root にインストール中..." -ForegroundColor Cyan
    Write-Host "  ⚠ 管理者権限が必要です。管理者権限で実行していない場合は手動でインストールしてください。" -ForegroundColor Yellow

    try {
        # CER ファイルを LocalMachine\TrustedPeople にインポート
        if (-not $trustedPeopleCert) {
            Import-Certificate -FilePath $cerPath -CertStoreLocation "Cert:\LocalMachine\TrustedPeople" -ErrorAction Stop | Out-Null
            Write-Host "  ✓ 証明書を Trusted People にインストールしました" -ForegroundColor Green
        }

        # CER ファイルを LocalMachine\Root にインポート
        if (-not $trustedRootCert) {
            Import-Certificate -FilePath $cerPath -CertStoreLocation "Cert:\LocalMachine\Root" -ErrorAction Stop | Out-Null
            Write-Host "  ✓ 証明書を Trusted Root Certification Authorities にインストールしました" -ForegroundColor Green
        }
    } catch {
        Write-Host "  ⚠ 証明書のインストールに失敗しました（管理者権限が必要）" -ForegroundColor Yellow
        Write-Host "    手動でインストールしてください:" -ForegroundColor Cyan
        Write-Host "    Import-Certificate -FilePath '$cerPath' -CertStoreLocation 'Cert:\LocalMachine\TrustedPeople'" -ForegroundColor White
        Write-Host "    Import-Certificate -FilePath '$cerPath' -CertStoreLocation 'Cert:\LocalMachine\Root'" -ForegroundColor White
        Write-Host "    または:" -ForegroundColor Cyan
        Write-Host "    certutil -addstore TrustedPeople '$cerPath'" -ForegroundColor White
        Write-Host "    certutil -addstore Root '$cerPath'" -ForegroundColor White
        Write-Host ""
        Write-Host "  続行しますか？ (証明書を手動でインストール済みの場合は Y)" -ForegroundColor Yellow
        $continue = Read-Host "  [Y/N]"
        if ($continue -ne "Y" -and $continue -ne "y") {
            exit 1
        }
    }
}

# ============================================================================
# ステップ4: MSIX パッケージに署名
# ============================================================================

Write-Host "`n[4/6] MSIX パッケージに署名中..." -ForegroundColor Yellow

# SignTool.exe のパスを探す (x64版を優先)
$signToolPath = Get-ChildItem -Path "C:\Program Files (x86)\Windows Kits\10\bin" -Filter "signtool.exe" -Recurse -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -like "*\x64\*" } |
    Select-Object -First 1 -ExpandProperty FullName

if (-not $signToolPath) {
    # x64が見つからない場合は他のアーキテクチャを試す（ARMは除外）
    $signToolPath = Get-ChildItem -Path "C:\Program Files (x86)\Windows Kits\10\bin" -Filter "signtool.exe" -Recurse -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -notlike "*\arm*" } |
        Select-Object -First 1 -ExpandProperty FullName
}

if (-not $signToolPath) {
    Write-Host "❌ SignTool.exe が見つかりません" -ForegroundColor Red
    Write-Host "   Windows SDK をインストールしてください: https://developer.microsoft.com/windows/downloads/windows-sdk/" -ForegroundColor Yellow
    exit 1
}

Write-Host "  SignTool: $signToolPath" -ForegroundColor Cyan

# 署名済みMSIXの出力パス
$signedMsixPath = $MsixPath -replace '\.msix$', '_Signed.msix'

# 元のファイルをコピー
Copy-Item -Path $MsixPath -Destination $signedMsixPath -Force

# 証明書の拇印 (thumbprint) を使用して署名
# PFXファイルではなく、証明書ストアから直接参照する方が推奨される
$signArgs = @(
    "sign",
    "/sha1", $cert.Thumbprint,
    "/fd", "SHA256",
    "/v",
    $signedMsixPath
)

Write-Host "  証明書拇印: $($cert.Thumbprint)" -ForegroundColor Cyan
Write-Host "  署名中..." -ForegroundColor Cyan
& $signToolPath $signArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ 署名に失敗しました (拇印方式)" -ForegroundColor Red
    Write-Host "  PFX ファイルを使用して再試行中..." -ForegroundColor Yellow
    
    # フォールバック: PFX ファイルを使用
    $signArgs = @(
        "sign",
        "/f", $certPath,
        "/p", $CertPassword,
        "/fd", "SHA256",
        "/v",
        $signedMsixPath
    )
    
    & $signToolPath $signArgs
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ 署名に失敗しました (PFX方式)" -ForegroundColor Red
        Write-Host "  SignTool エラーコード: $LASTEXITCODE" -ForegroundColor Red
        exit 1
    }
}

Write-Host "  ✓ MSIX パッケージに署名しました" -ForegroundColor Green
Write-Host "    署名済みファイル: $(Split-Path -Leaf $signedMsixPath)" -ForegroundColor Cyan

# ============================================================================
# ステップ5: 既存のアプリをアンインストール
# ============================================================================

Write-Host "`n[5/6] 既存のアプリをアンインストール中..." -ForegroundColor Yellow

$appName = "urotaichicorporation.Side-theSupermasaoIntegratedD"
$existingApp = Get-AppxPackage | Where-Object { $_.Name -eq $appName }

if ($existingApp) {
    Write-Host "  既存のアプリを削除中..." -ForegroundColor Cyan
    Remove-AppxPackage -Package $existingApp.PackageFullName
    Write-Host "  ✓ 既存のアプリをアンインストールしました" -ForegroundColor Green
} else {
    Write-Host "  既存のアプリは見つかりませんでした" -ForegroundColor Cyan
}

# ============================================================================
# ステップ6: MSIX パッケージをインストール
# ============================================================================

Write-Host "`n[6/6] MSIX パッケージをインストール中..." -ForegroundColor Yellow

try {
    Add-AppxPackage -Path $signedMsixPath
    Write-Host "  ✓ インストールに成功しました！" -ForegroundColor Green
} catch {
    Write-Host "❌ インストールに失敗しました" -ForegroundColor Red
    Write-Host "  エラー: $_" -ForegroundColor Red
    
    Write-Host "`n  トラブルシューティング:" -ForegroundColor Yellow
    Write-Host "  1. 証明書が Trusted People ストアにインストールされているか確認:" -ForegroundColor Cyan
    Write-Host "     Get-ChildItem Cert:\LocalMachine\TrustedPeople | Where-Object { `$_.Subject -like '*urotaichi*' }" -ForegroundColor White
    Write-Host "  2. 開発者モードが有効か確認:" -ForegroundColor Cyan
    Write-Host "     設定 > 更新とセキュリティ > 開発者向け > 開発者モード" -ForegroundColor White
    Write-Host "  3. 証明書を手動でインストール:" -ForegroundColor Cyan
    Write-Host "     Import-Certificate -FilePath '$cerPath' -CertStoreLocation 'Cert:\LocalMachine\TrustedPeople'" -ForegroundColor White
    
    exit 1
}

# ============================================================================
# 完了
# ============================================================================

Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║                ✓ インストール完了しました！                   ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""

Write-Host "🎉 次のステップ:" -ForegroundColor Cyan
Write-Host "  1. スタートメニューを開く" -ForegroundColor White
Write-Host "  2. 'Side' または 'Supermasao' を検索" -ForegroundColor White
Write-Host "  3. アプリを起動" -ForegroundColor White
Write-Host "  4. 新規プロジェクトを作成してランタイムフォルダが読み込まれるか確認" -ForegroundColor White
Write-Host ""

Write-Host "📋 確認事項:" -ForegroundColor Cyan
Write-Host "  ✓ ランタイムフォルダのエラーが出ないこと" -ForegroundColor Green
Write-Host "  ✓ 新規プロジェクト作成が正常に動作すること" -ForegroundColor Green
Write-Host "  ✓ サンプルプロジェクトが開けること" -ForegroundColor Green
Write-Host ""

# クリーンアップの案内
Write-Host "🧹 クリーンアップ (オプション):" -ForegroundColor Cyan
Write-Host "  テスト証明書を削除する場合:" -ForegroundColor White
Write-Host "  # 個人ストアから削除" -ForegroundColor Gray
Write-Host "  Get-ChildItem Cert:\CurrentUser\My | Where-Object { `$_.Subject -eq 'CN=urotaichi Test Certificate' } | Remove-Item" -ForegroundColor Yellow
Write-Host "  # Trusted People から削除（管理者権限が必要）" -ForegroundColor Gray
Write-Host "  Get-ChildItem Cert:\LocalMachine\TrustedPeople | Where-Object { `$_.Subject -eq 'CN=urotaichi Test Certificate' } | Remove-Item" -ForegroundColor Yellow
Write-Host ""
Write-Host "📚 参考情報:" -ForegroundColor Cyan
Write-Host "  証明書の配置場所:" -ForegroundColor White
Write-Host "  - Cert:\CurrentUser\My          : 署名用の証明書（秘密鍵あり）" -ForegroundColor Gray
Write-Host "  - Cert:\LocalMachine\TrustedPeople : MSIX インストール用（公開証明書のみ）" -ForegroundColor Gray
Write-Host ""
