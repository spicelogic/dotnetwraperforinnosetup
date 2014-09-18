; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "name", "MyApp")
#define MyAppVersion ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "version", "unknown")
#define MyAppPublisher ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "publisher", "MyCompany")
#define MyAppURL ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "url", "unknown")
#define MySupportURL ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "support_url", "unknown")
#define MyAppExeName ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "startup_filename", "unknown")
#define AppCPU ReadIni(SourcePath + "\ProjectMetaData.txt", "platform", "cpu", "unknown")
#define FeedbackUrl ReadIni(SourcePath + "\ProjectMetaData.txt", "uninstall", "feedback_url", "unknown")
#define MyAppId ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "id", "unknown")
#define DeployableFolder ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "deployable_folder", "unknown")
#define InfoBeforeFile ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "eula_file", "")
#define MyAppIconPath ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "icon_path", "")
#define MyAppIconName ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "icon_name", "")
#define MyOutputDir ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "output_dir", "\Output")
#define OutputFilename ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "output_filename", "setup")
#define ShortcutName ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "shortcut_name", "MyApp")
#define DotNetVersion ReadIni(SourcePath + "\ProjectMetaData.txt", "platform", "dot_net_version", "unknown")
#define FileExtensionAssociation ReadIni(SourcePath + "\ProjectMetaData.txt", "application", "extension", "")

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={#MyAppId}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MySupportURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={code:GetPFPath|{#AppCPU}}\{#MyAppPublisher}\{#MyAppName}
;{pf}\{#MyAppPublisher}\{#MyAppName}
DefaultGroupName={#MyAppPublisher}\{#MyAppName}
AllowNoIcons=yes
InfoBeforeFile={#InfoBeforeFile}
OutputDir={#MyOutputDir}
OutputBaseFilename={#OutputFilename}
Compression=lzma
SolidCompression=yes

;#section Association
ChangesAssociations=yes
;#end_section Association

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#DeployableFolder}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#MyAppIconPath}{#MyAppIconName}"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Registry]
;#section Association
Root: HKCR; Subkey: "{#FileExtensionAssociation}"; ValueType: string; ValueName: ""; ValueData: "{#FileExtensionAssociation}"; Flags: uninsdeletevalue 
Root: HKCR; Subkey: "{#FileExtensionAssociation}"; ValueType: string; ValueName: ""; ValueData: "{#FileExtensionAssociation}"; Flags: uninsdeletekey 
Root: HKCR; Subkey: "{#FileExtensionAssociation}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppIconName}" 
Root: HKCR; Subkey: "{#FileExtensionAssociation}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1""" 
;#end_section Association
;#section ContextMenu
Root: HKCR; Subkey: SystemFileAssociations\.spiceProj2\shell\Spice Proj; ValueType: string; Flags: uninsdeletekey deletekey; ValueName: Icon; ValueData: """{app}\{#MyAppIconName}"""
Root: HKCR; Subkey: SystemFileAssociations\.spiceProj2\shell\Spice Proj\command; ValueType: string; ValueData: """{app}\{#MyAppExeName}"" ""%1"""; Flags: uninsdeletekey deletekey
Root: HKCR; Subkey: SystemFileAssociations\.spiceProj\shell\Spice Proj; ValueType: string; Flags: uninsdeletekey deletekey; ValueName: Icon; ValueData: """{app}\{#MyAppIconName}"""
Root: HKCR; Subkey: SystemFileAssociations\.spiceProj\shell\Spice Proj\command; ValueType: string; ValueData: """{app}\{#MyAppExeName}"" ""%1"""; Flags: uninsdeletekey deletekey
;#end_section ContextMenu

[Icons]
Name: "{group}\{#ShortcutName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppIconName}"
Name: "{group}\{cm:UninstallProgram,{#ShortcutName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#ShortcutName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon; IconFilename: "{app}\{#MyAppIconName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: files; Name: "{userappdata}\{#MyAppPublisher}\{#MyAppName}\*.*"
Type: dirifempty; Name: "{userappdata}\{#MyAppPublisher}\{#MyAppName}"
Type: dirifempty; Name: "{userappdata}\{#MyAppPublisher}"
     
[Code]
function GetPFPath(appCpu : string): string;
begin   
  if (appCpu = 'AnyCPU') then
    begin
      if (IsWin64) then
        begin
          Result := ExpandConstant('{pf64}');
        end
      else
        begin
          Result := ExpandConstant('{pf32}');
        end
    end
  else
    if (appCpu = '64bit') then
      begin
        Result := ExpandConstant('{pf64}');
      end
    else
      begin
        Result := ExpandConstant('{pf32}');
      end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
    ErrCode: integer;
    Url: string;
begin
    Url := '{#FeedbackUrl}';
    if (CurUninstallStep=usDone) then
    begin
        if (Url <> '') then
          ShellExec('open', Url, '', '', SW_SHOW, ewNoWait, ErrCode);
    end;
end;

function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var        
    key: string;
    install, release, serviceCount: cardinal;
    check45, success: boolean;
begin
    version := 'v' + version;
    // .NET 4.5 installs as update to .NET 4.0 Full
    if version = 'v4.5' then begin
        version := 'v4\Full';
        check45 := true;
    end else
        check45 := false;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0/4.5 uses value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5 uses additional value Release
    if check45 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= 378389);
    end;

    result := success and (install = 1) and (serviceCount >= service);
end;


function InitializeSetup(): Boolean;
var        
    ErrCode: integer;
    DownloadUrl: string;
begin
    if ('{#DotNetVersion}' <> 'unknown') and not IsDotNetDetected('{#DotNetVersion}', 0) then begin
        MsgBox('You need to download and install .NET framework version ' + '{#DotNetVersion}', mbInformation, MB_OK);
        
        case '{#DotNetVersion}' of
          '4.5' :
            begin
              DownloadUrl := 'http://www.microsoft.com/en-us/download/details.aspx?id=30653';
            end;
          '4\Full' :
            begin
              DownloadUrl := 'http://www.microsoft.com/en-us/download/details.aspx?id=17718';
            end;      
          '4\Client' :
            begin
              DownloadUrl := 'http://www.microsoft.com/en-us/download/details.aspx?id=24872';
            end; 
          '3.5' :
            begin
              DownloadUrl := 'http://www.microsoft.com/en-us/download/details.aspx?id=22';
            end; 
          '3.0' :
            begin
              DownloadUrl := 'http://www.microsoft.com/en-us/download/details.aspx?id=3005';
            end; 
          '2.0.50727' :
            begin
              DownloadUrl := 'http://www.microsoft.com/en-us/download/details.aspx?id=1639';
            end; 
          '1.1.4322' :
            begin
              DownloadUrl := 'http://www.microsoft.com/en-us/download/details.aspx?id=26';
            end; 
          else
            begin
              DownloadUrl := 'http://www.microsoft.com/net/downloads';
            end; 
        end;
        ShellExec('open', DownloadUrl, '', '', SW_SHOW, ewNoWait, ErrCode);
        result := false;
    end else
        result := true;
end;