# Microsoft Store用 MSIX パッケージ ビルドスクリプト
# Store構成でビルド → 必要ファイル検証 → MSIX生成 → .msixupload生成
#
# 使い方:
#   pwsh -File .\Build-Msix.ps1
#   pwsh -File .\Build-Msix.ps1 -Platform x64
#   pwsh -File .\Build-Msix.ps1 -SkipBuild     # ビルド済みの場合

param(
	[string]$Configuration = "Store",
	[string]$Platform = "x64",
	[switch]$SkipBuild
)

Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║        Microsoft Store用 MSIX パッケージ ビルド               ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

$ProjectRoot = $PSScriptRoot
$ProjectPath = Join-Path $ProjectRoot "side\side.csproj"
$ManifestPath = Join-Path $ProjectRoot "side\Package.appxmanifest"
$AppPackagesDir = Join-Path $ProjectRoot "side\AppPackages"

# バージョン取得
[xml]$Manifest = Get-Content $ManifestPath
$Version = $Manifest.Package.Identity.Version

Write-Host "  バージョン: $Version" -ForegroundColor Cyan
Write-Host "  Configuration: $Configuration" -ForegroundColor Cyan
Write-Host "  Platform: $Platform" -ForegroundColor Cyan
Write-Host ""

# ============================================================================
# ステップ1: プロジェクトのビルド
# ============================================================================

if ($SkipBuild) {
	Write-Host "[1/5] ビルドをスキップ (-SkipBuild)" -ForegroundColor Yellow
} else {
	Write-Host "[1/5] Microsoft Store版をビルド中..." -ForegroundColor Yellow

	try {
		# MSBuildのパスを探す
		$msbuildPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" `
			-latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe `
			-prerelease | Select-Object -First 1

		if (-not $msbuildPath) {
			Write-Host "❌ MSBuild が見つかりません。Visual Studio 2022 をインストールしてください。" -ForegroundColor Red
			exit 1
		}

		Write-Host "  MSBuild: $msbuildPath" -ForegroundColor Cyan

		# ビルド実行
		& $msbuildPath /p:Configuration=$Configuration /p:Platform=$Platform `
			/p:AppxPackageDir=AppPackages\ `
			/p:GenerateAppxPackageOnBuild=true `
			/t:Restore,Build `
			$ProjectPath

		if ($LASTEXITCODE -ne 0) {
			Write-Host "❌ ビルドに失敗しました (exit code: $LASTEXITCODE)" -ForegroundColor Red
			exit $LASTEXITCODE
		}

		Write-Host "  ✓ ビルド成功" -ForegroundColor Green
	}
	catch {
		Write-Host "❌ ビルドエラー: $_" -ForegroundColor Red
		exit 1
	}
}

# ============================================================================
# ステップ2: 出力ディレクトリの確認
# ============================================================================

Write-Host "`n[2/5] 出力ディレクトリを確認中..." -ForegroundColor Yellow

$outputDir = Join-Path $ProjectRoot "side\bin\$Platform\$Configuration"
$publishDir = Get-ChildItem -Path $outputDir -Directory -Recurse -ErrorAction SilentlyContinue |
	Where-Object { $_.Name -eq "win-$Platform" } |
	Select-Object -First 1 -ExpandProperty FullName

if (-not $publishDir) {
	# publish サブディレクトリも検索
	$publishDir = Get-ChildItem -Path $outputDir -Directory -Recurse -ErrorAction SilentlyContinue |
		Where-Object { $_.Name -eq "publish" } |
		Select-Object -First 1 -ExpandProperty FullName
}

if (-not $publishDir) {
	Write-Host "❌ 出力ディレクトリが見つかりません: $outputDir" -ForegroundColor Red
	exit 1
}

Write-Host "  出力先: $publishDir" -ForegroundColor Cyan

# ============================================================================
# ステップ3: 必要なフォルダとファイルの確認
# ============================================================================

Write-Host "`n[3/5] 必要なフォルダとファイルを確認中..." -ForegroundColor Yellow

$requiredItems = @(
	@{ Name = "runtime"; Type = "Directory"; Critical = $true },
	@{ Name = "pictures"; Type = "Directory"; Critical = $true },
	@{ Name = "projects"; Type = "Directory"; Critical = $true },
	@{ Name = "readme.txt"; Type = "File"; Critical = $true },
	@{ Name = "使い方.txt"; Type = "File"; Critical = $false },
	@{ Name = "勝手にFx14説明書.txt"; Type = "File"; Critical = $false },
	@{ Name = "side.exe"; Type = "File"; Critical = $true }
)

$allOk = $true
foreach ($item in $requiredItems) {
	$itemPath = Join-Path $publishDir $item.Name
	$exists = if ($item.Type -eq "Directory") {
		Test-Path $itemPath -PathType Container
	} else {
		Test-Path $itemPath -PathType Leaf
	}

	if ($exists) {
		$icon = "✓"
		$color = "Green"
		if ($item.Type -eq "Directory") {
			$count = (Get-ChildItem -Path $itemPath -Recurse -File).Count
			$displayText = "$($item.Name) ($count files)"
		} else {
			$size = [math]::Round((Get-Item $itemPath).Length / 1KB, 2)
			$displayText = "$($item.Name) ($size KB)"
		}
	} else {
		$icon = "✗"
		$color = if ($item.Critical) { "Red" } else { "Yellow" }
		$displayText = "$($item.Name) (missing)"
		if ($item.Critical) {
			$allOk = $false
		}
	}

	Write-Host "  $icon $displayText" -ForegroundColor $color
}

if (-not $allOk) {
	Write-Host "`n  ❌ 必須ファイル/フォルダが不足しています" -ForegroundColor Red
	Write-Host "     side.csproj の Content 設定を確認してください" -ForegroundColor Yellow
	exit 1
}

Write-Host "  ✓ すべての必要なファイルが含まれています" -ForegroundColor Green

# ============================================================================
# ステップ4: MSIX パッケージを生成 (MakeAppx.exe)
# ============================================================================

Write-Host "`n[4/5] MSIX パッケージを生成中..." -ForegroundColor Yellow

# MakeAppx.exe を探す
$MakeAppxPath = Get-ChildItem -Path "C:\Program Files (x86)\Windows Kits\10\bin" -Filter "MakeAppx.exe" -Recurse -ErrorAction SilentlyContinue |
	Where-Object { $_.FullName -like "*\x64\*" } |
	Sort-Object { [version]($_.FullName -replace '.*\\(\d+\.\d+\.\d+\.\d+)\\.*', '$1') } -Descending |
	Select-Object -First 1 -ExpandProperty FullName

if (-not $MakeAppxPath) {
	Write-Host "❌ MakeAppx.exe が見つかりません" -ForegroundColor Red
	Write-Host "   Windows SDK をインストールしてください" -ForegroundColor Yellow
	exit 1
}

Write-Host "  MakeAppx: $MakeAppxPath" -ForegroundColor Cyan

if (-not (Test-Path $AppPackagesDir)) {
	New-Item -ItemType Directory -Path $AppPackagesDir -Force | Out-Null
}

$MsixPath = Join-Path $AppPackagesDir "Side_${Version}_${Platform}.msix"

# 一時ディレクトリを準備
$TempPackDir = Join-Path $env:TEMP "msix_pack_$([DateTime]::Now.Ticks)"
New-Item -ItemType Directory -Path $TempPackDir -Force | Out-Null

try {
	# AppxManifest.xml をコピー（Package.appxmanifest → AppxManifest.xml）
	Copy-Item -Path $ManifestPath -Destination (Join-Path $TempPackDir "AppxManifest.xml") -Force

	# Assets をコピー
	$AssetsDir = Join-Path $ProjectRoot "side\Assets"
	if (Test-Path $AssetsDir) {
		Copy-Item -Path $AssetsDir -Destination (Join-Path $TempPackDir "Assets") -Recurse -Force
	}

	# Publish ファイルをコピー
	Get-ChildItem -Path $publishDir | Copy-Item -Destination $TempPackDir -Recurse -Force

	# 既存の MSIX を削除
	if (Test-Path $MsixPath) {
		Remove-Item $MsixPath -Force
	}

	# MakeAppx.exe でパッキング
	Write-Host "  パッキング中..." -ForegroundColor Cyan
	& $MakeAppxPath pack /d $TempPackDir /p $MsixPath /o

	if ($LASTEXITCODE -ne 0) {
		Write-Host "❌ MakeAppx が失敗しました" -ForegroundColor Red
		exit 1
	}

	$MsixSizeMB = [math]::Round((Get-Item $MsixPath).Length / 1MB, 2)
	Write-Host "  ✓ MSIX パッケージを生成: $(Split-Path -Leaf $MsixPath) ($MsixSizeMB MB)" -ForegroundColor Green
}
finally {
	# 一時ディレクトリをクリーンアップ
	Remove-Item -Path $TempPackDir -Recurse -Force -ErrorAction SilentlyContinue
}

# ============================================================================
# ステップ5: .msixupload ファイルを生成
# ============================================================================

Write-Host "`n[5/5] .msixupload ファイルを生成中..." -ForegroundColor Yellow

$MsixUploadPath = Join-Path $AppPackagesDir "Side_${Version}.msixupload"

if (Test-Path $MsixUploadPath) {
	Remove-Item $MsixUploadPath -Force
}

# .msixupload は MSIX を含む ZIP ファイル
Add-Type -AssemblyName System.IO.Compression.FileSystem

$TempUploadDir = Join-Path $env:TEMP "msixupload_$([DateTime]::Now.Ticks)"
New-Item -ItemType Directory -Path $TempUploadDir -Force | Out-Null

try {
	# MSIX ファイルをコピー
	Copy-Item -Path $MsixPath -Destination $TempUploadDir -Force

	# ZIP として作成してリネーム
	$TempZipPath = $MsixUploadPath -replace '\.msixupload$', '.zip'

	[System.IO.Compression.ZipFile]::CreateFromDirectory(
		$TempUploadDir,
		$TempZipPath,
		[System.IO.Compression.CompressionLevel]::Optimal,
		$false
	)

	Rename-Item -Path $TempZipPath -NewName (Split-Path -Leaf $MsixUploadPath)

	$UploadSizeMB = [math]::Round((Get-Item $MsixUploadPath).Length / 1MB, 2)
	Write-Host "  ✓ .msixupload を生成: $(Split-Path -Leaf $MsixUploadPath) ($UploadSizeMB MB)" -ForegroundColor Green

	# 検証: ファイル数チェック
	$zip = [System.IO.Compression.ZipFile]::OpenRead($MsixUploadPath)
	$fileCount = $zip.Entries.Count
	$zip.Dispose()

	if ($fileCount -le 16) {
		Write-Host "  ✓ ファイル数: $fileCount / 16 (Partner Center 要件 OK)" -ForegroundColor Green
	} else {
		Write-Host "  ⚠ ファイル数: $fileCount / 16 (多すぎます)" -ForegroundColor Yellow
	}
}
catch {
	Write-Host "❌ .msixupload の生成に失敗しました: $_" -ForegroundColor Red
	exit 1
}
finally {
	Remove-Item -Path $TempUploadDir -Recurse -Force -ErrorAction SilentlyContinue
}

# ============================================================================
# 完了
# ============================================================================

Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║                    ✓ ビルド完了しました！                     ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""

Write-Host "📦 生成されたファイル:" -ForegroundColor Cyan
Write-Host "  MSIX:       $(Split-Path -Leaf $MsixPath) ($MsixSizeMB MB)" -ForegroundColor White
Write-Host "  msixupload: $(Split-Path -Leaf $MsixUploadPath) ($UploadSizeMB MB)" -ForegroundColor White
Write-Host ""

Write-Host "🔧 次のステップ:" -ForegroundColor Cyan
Write-Host "  ローカルテスト:" -ForegroundColor White
Write-Host "    .\Sign-And-Install-MSIX.ps1" -ForegroundColor Yellow
Write-Host "  Store アップロード:" -ForegroundColor White
Write-Host "    Partner Center に $MsixUploadPath をアップロード" -ForegroundColor Yellow
Write-Host ""
