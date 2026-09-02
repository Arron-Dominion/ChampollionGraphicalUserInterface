# Windows Installer Maintenance

This guide explains how the Champollion Graphical User Interface Windows release is built and how to maintain its Inno Setup configuration.

## Files involved

| File | Responsibility |
| --- | --- |
| `packaging/windows/setup.iss` | Defines installer identity, destination, shortcuts, privileges, installed files, and uninstall cleanup. |
| `scripts/package-windows.ps1` | Publishes the application, creates the portable ZIP, compiles the installer, and generates SHA-256 checksums. |
| `src/ChampollionGraphicalUserInterface/Properties/PublishProfiles/win-x64.pubxml` | Defines the self-contained Windows x64 publish settings. |
| `src/ChampollionGraphicalUserInterface/ChampollionGraphicalUserInterface.csproj` | Defines product and default application version metadata. |
| `global.json` | Selects the .NET SDK used for publishing and validation. |

The PowerShell script is the normal release entry point. Avoid compiling `setup.iss` manually for a release because the packaging script supplies the version and absolute publish and output directories.

## Prerequisites

- The .NET SDK selected by `global.json`.
- Inno Setup 7 or 6 to create the installer.
- PowerShell 7 or Windows PowerShell 5.1.
- A Windows x64-compatible build environment.

The packaging script searches for `ISCC.exe` on `PATH` and in the standard Inno Setup 7 and 6 installation directories under `Program Files` and `Program Files (x86)`. If the compiler is unavailable, the script still creates the portable ZIP and prints a warning.

## Build the Windows package

Run this command from the repository root:

```powershell
.\scripts\package-windows.ps1 -Version 2.0.0
```

Use a three-part release version unless the release process deliberately requires a prerelease suffix. The supplied version controls the published assembly version, installer display version, artifact filenames, and checksum selection.

The script performs these steps:

1. Deletes and recreates `artifacts/publish/win-x64`.
2. Publishes a Release, self-contained `win-x64` application using the `win-x64` publish profile.
3. Rejects the package if a third-party `Champollion.exe` is present in the publish output.
4. Creates the portable ZIP.
5. Calls the Inno Setup command-line compiler with release-specific preprocessor values when `ISCC.exe` is available.
6. Creates one `.sha256` file beside each Windows artifact.

Expected outputs:

```text
artifacts/packages/ChampollionGraphicalUserInterface-<version>-win-x64-portable.zip
artifacts/packages/ChampollionGraphicalUserInterface-<version>-win-x64-portable.zip.sha256
artifacts/packages/ChampollionGraphicalUserInterface-<version>-win-x64-setup.exe
artifacts/packages/ChampollionGraphicalUserInterface-<version>-win-x64-setup.exe.sha256
```

The portable ZIP and installer use the same complete self-contained publish directory. The external Legacy or Current Champollion executable is not included; users obtain that tool separately.

## Preprocessor values

The top of `setup.iss` defines three overridable values:

| Value | Purpose | Release value supplied by |
| --- | --- | --- |
| `MyAppVersion` | Installer version and output filename version. | `-Version` in `package-windows.ps1` |
| `PublishDir` | Directory whose complete contents are installed. | `artifacts/publish/win-x64` |
| `OutputDir` | Directory where Inno Setup writes the installer. | `artifacts/packages` |

The defaults make `setup.iss` convenient to open and compile in the Inno Setup IDE, but release builds should use the PowerShell entry point.

## Installer identity

`AppId` is the permanent identity Windows uses to associate upgrades and uninstallation records:

```ini
AppId={{6C66A083-6E9B-4B2F-B21B-4E84DB3292A4}
```

Do not change this value for ordinary releases, product renames, or version updates. Changing it makes Windows treat the installer as a different product and can leave the previous version installed alongside it.

The product name is `Champollion Graphical User Interface`, published by `Arron Dominion`, and the installed executable is `ChampollionGraphicalUserInterface.exe`.

## Install location and privileges

The installer currently uses:

```ini
DefaultDirName={autopf}\Champollion Graphical User Interface
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
```

The normal default is therefore:

```text
C:\Program Files\Champollion Graphical User Interface
```

The installer requests elevation because the normal destination is under Program Files. `PrivilegesRequiredOverridesAllowed=dialog` permits a user to select current-user mode when needed. In current-user mode, Inno Setup resolves `{autopf}` to the user's Programs directory instead.

Keep `PrivilegesRequired=admin` if Program Files must remain the default. Setting it to `lowest` changes the default to a per-user location under the user's profile.

## Installed files and writable data

The `[Files]` section copies the complete self-contained publish directory recursively into `{app}`. This includes the application, .NET runtime, native libraries, Avalonia assemblies, and resources. Do not replace it with a hand-maintained file list.

The installer grants standard-user modify access to these application data directories while keeping application binaries protected:

```text
{app}\UserData
{app}\ChampollionGraphicalUserInterfaceOutput
{app}\ChampollionGraphicalUserInterfaceAssembly
```

The application stores current settings and diagnostic logs in `{app}\UserData`. The embedded WebView2 profile is separate and is stored for the current user at:

```text
%LOCALAPPDATA%\ChampollionGraphicalUserInterface\WebView2
```

This profile contains browser cache, cookies, login state, and related browser state. The `[UninstallDelete]` entry removes this application-specific WebView2 profile during uninstall. It does not explicitly remove adjacent `UserData` settings or logs.

## Shortcuts and post-install launch

The installer always creates a Start menu shortcut. A desktop shortcut is optional and controlled by the `desktopicon` task.

The `[Run]` entry offers to launch the application after an interactive installation. The `skipifsilent` flag prevents automated or silent builds from unexpectedly starting the UI.

When changing shortcut names or locations, verify that uninstall removes them and that an upgrade does not create duplicate shortcuts.

## Compression and architecture

`Compression=lzma2` and `SolidCompression=yes` reduce installer size at the cost of compile time. The package supports x64-compatible Windows systems and installs in 64-bit mode.

Do not add ARM64 files to this installer. Create a separate runtime publish and installer artifact when ARM64 support is introduced.

## Validation

At minimum, perform these checks after changing `setup.iss` or the Windows packaging script:

1. Run `.\scripts\package-windows.ps1 -Version <version>` and confirm Inno Setup reports `Successful compile`.
2. Verify the installer, portable ZIP, and checksum files exist in `artifacts/packages`.
3. Install over an existing version and confirm the application is upgraded in place.
4. Launch the installed executable and complete a representative GUI workflow.
5. Open the Help tab and confirm the embedded WebView2 pages work.
6. Confirm the Start menu shortcut and optional desktop shortcut work.
7. Uninstall and confirm application files and shortcuts are removed.
8. Confirm the application-specific WebView2 profile is removed from `%LOCALAPPDATA%` after uninstall.
9. Test on a clean Windows 10 or 11 x64 virtual machine before release.

For a non-elevated automated smoke test, use Inno Setup's `/CURRENTUSER` override and an isolated destination. This tests file deployment and uninstall behavior but does not prove the normal Program Files elevation flow:

```powershell
$installer = Resolve-Path .\artifacts\packages\ChampollionGraphicalUserInterface-2.0.0-win-x64-setup.exe
$destination = Join-Path (Resolve-Path .\artifacts) installer-smoke-test

$install = Start-Process $installer `
    -ArgumentList '/CURRENTUSER', '/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART', "/DIR=$destination" `
    -Wait -PassThru

if ($install.ExitCode -ne 0) {
    throw "Installer failed with exit code $($install.ExitCode)."
}

$uninstall = Start-Process (Join-Path $destination unins000.exe) `
    -ArgumentList '/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART' `
    -Wait -PassThru

if ($uninstall.ExitCode -ne 0) {
    throw "Uninstaller failed with exit code $($uninstall.ExitCode)."
}
```

Do not automate the normal administrative mode from an unelevated terminal because its UAC prompt requires interactive approval.

## Common maintenance changes

### Change the product version

Pass the release version to `package-windows.ps1`. Do not edit the fallback `MyAppVersion` for each release.

### Change the default directory

Edit `DefaultDirName`. Keep `{autopf}` unless there is a specific reason to hard-code a Windows directory.

### Rename the executable

Update the assembly name or project output first, then update the Inno Setup executable name, shortcut targets, launch entries, scripts, documentation, and CI artifact checks together.

### Change WebView2 profile handling

Keep the profile under a writable per-user location rather than beside the installed executable. If the profile path changes, update the runtime configuration, README, architecture and security diagrams, and `[UninstallDelete]` together. Verify that uninstall removes the new application-specific profile without deleting unrelated user browser data.

### Add an installer icon

Create a Windows `.ico` containing common sizes and add this under `[Setup]`:

```ini
SetupIconFile=path\to\application.ico
```

The installed executable icon is controlled by the application project, not by `SetupIconFile`.

### Add code signing

Keep signing certificates and passwords outside the repository. Configure an Inno Setup signing command or sign the completed executable in CI using secret-backed credentials. Verify both the application executable and installer signature before publishing.

## Troubleshooting

### The portable ZIP exists but the installer does not

The script could not locate `ISCC.exe`, or Inno Setup compilation failed. Review the warning or compiler output and confirm Inno Setup is installed in a standard directory or available on `PATH`.

### The installer defaults to AppData

Confirm `PrivilegesRequired=admin` is still present and the installer was not started with `/CURRENTUSER`. Per-user mode intentionally changes `{autopf}` to a user-scoped Programs directory.

### The installer contains stale files

The packaging script recreates the publish directory, so stale files usually mean an installer was compiled manually against a different `PublishDir`. Delete `artifacts/publish/win-x64` and run the PowerShell packaging command again.

### Upgrade creates a second installation

Confirm `AppId` has not changed and that the previous installer used the same architecture and privilege mode.

### WebView2 data remains after uninstall

Confirm the application was uninstalled by the expected user context. The cleanup targets the current user's `%LOCALAPPDATA%\ChampollionGraphicalUserInterface\WebView2` directory. An administrator uninstalling another user's installation may not remove that other user's per-user browser profile.

### Silent validation reports no exit code

Use `Start-Process -Wait -PassThru` and read its `ExitCode`. Direct invocation of a Windows GUI-subsystem installer may not populate PowerShell's `$LASTEXITCODE` reliably.
