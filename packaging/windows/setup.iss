#ifndef MyAppVersion
  #define MyAppVersion "2.0.0"
#endif
#ifndef PublishDir
  #define PublishDir "..\..\artifacts\publish\win-x64"
#endif
#ifndef OutputDir
  #define OutputDir "..\..\artifacts\packages"
#endif

#define MyAppName "Champollion Graphical User Interface"
#define MyAppExeName "ChampollionGraphicalUserInterface.exe"

[Setup]
AppId={{6C66A083-6E9B-4B2F-B21B-4E84DB3292A4}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=Arron Dominion
DefaultDirName={autopf}\Champollion Graphical User Interface
DefaultGroupName={#MyAppName}
OutputDir={#OutputDir}
OutputBaseFilename=ChampollionGraphicalUserInterface-{#MyAppVersion}-win-x64-setup
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
WizardStyle=modern

[Dirs]
Name: "{app}\UserData"; Permissions: users-modify

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent
