$ErrorActionPreference = "Stop"

if ( -not (Test-Path env:BuildVersion)) {
  $env:BuildVersion = "1.0.0.0"
}

$root = "$pwd"

# Make sure root is in root folder and not scripts folder
if ((Get-Item $root).Name.ToLowerInvariant() -eq "scripts") {
  $root = "$((Get-Item $pwd).Parent.FullName)"
}

$scriptPath = "$root/scripts"
$buildPath = "$root/build"
$binPath = "$buildPath/ffxivapp"
$updaterPath = "$buildPath/updater"
$updateExec = "FFXIVAPP.Updater"
$runtime = "linux-x64"
$warpRuntime = "linux-x64"

$warp = "warp-packer";
If ( -not (Test-Path -LiteralPath $buildPath)) {
  New-Item -Path $buildPath -ItemType Directory
}
If ( -not (Test-Path -LiteralPath $binPath)) {
  New-Item -Path $binPath -ItemType Directory
}
Else {
  Get-ChildItem -Path $binPath -Include * -Recurse | ForEach-Object { $_.Delete()}
}

If ($IsLinux) {
  $warp = "warp-packer";
  $runtime = "linux-x64"
  $warpRuntime = "linux-x64"
  $updateExec = "FFXIVAPP.Updater"

  if ( -not (Test-Path -LiteralPath "$scriptPath/$warp")) {
    curl -Lo $scriptPath/$warp https://github.com/dgiagio/warp/releases/download/v0.3.0/linux-x64.warp-packer
    if ($LastExitCode -ne 0) {
      exit 1
    }
  }

  chmod +x "$scriptPath/$warp"
  if ($LastExitCode -ne 0) {
    exit 1
  }
}
If ($IsWindows) {
  $warp = "warp-packer.exe";
  $runtime = "win-x64"
  $warpRuntime = "windows-x64"
  $updateExec = "FFXIVAPP.Updater.exe"

  if ( -not (Test-Path -LiteralPath "$scriptPath/$warp")) {
    curl -Lo $scriptPath/$warp https://github.com/dgiagio/warp/releases/download/v0.3.0/windows-x64.warp-packer.exe
    if ($LastExitCode -ne 0) {
      exit 1
    }
  }
}
If ($IsMacOS) {
  $warp = "warp-packer";
  $runtime = "osx-x64"
  $warpRuntime = "macos-x64"
  $updateExec = "FFXIVAPP.Updater"

  if ( -not (Test-Path -LiteralPath "$scriptPath/$warp")) {
    curl -Lo $scriptPath/$warp https://github.com/dgiagio/warp/releases/download/v0.3.0/macos-x64.warp-packer
    if ($LastExitCode -ne 0) {
      exit 1
    }
  }

  chmod +x "$scriptPath/$warp"
  if ($LastExitCode -ne 0) {
    exit 1
  }
}

# Build FFXIVAPP.Updater
dotnet publish FFXIVAPP.Updater/FFXIVAPP.Updater.csproj -r $runtime -c Release --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:Version=$env:BuildVersion /p:FileVersion=$env:BuildVersion /p:AssemblyVersion=$env:BuildVersion -o $updaterPath
if ($LastExitCode -ne 0) {
  exit 1
}

$warpCommand = "$scriptPath/$warp --arch $warpRuntime --input_dir $updaterPath --exec $updateExec --output $binPath/$updateExec"
Invoke-Expression $warpCommand
if ($LastExitCode -ne 0) {
  exit 1
}

# Build FFXIVAPP.Client
dotnet publish FFXIVAPP.Client/FFXIVAPP.Client.csproj -r $runtime -c Release --self-contained true /p:Version=$env:BuildVersion /p:FileVersion=$env:BuildVersion /p:AssemblyVersion=$env:BuildVersion -o $binPath
if ($LastExitCode -ne 0) {
  exit 1
}
