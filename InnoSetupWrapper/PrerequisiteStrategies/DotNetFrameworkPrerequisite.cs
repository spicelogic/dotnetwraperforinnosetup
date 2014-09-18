using System;
using System.Collections.Generic;
using SpiceLogic.InnoSetupWrapper.Models;
using SpiceLogic.InnoSetupWrapper.Utilities;

namespace SpiceLogic.InnoSetupWrapper.PrerequisiteStrategies
{
    /// <summary>
    /// 
    /// </summary>
    public class DotNetFrameworkPrerequisite : Prerequisite
    { 
        private readonly Dictionary<DotNetVersions, string> _downloads = new Dictionary<DotNetVersions, string>
        {
            {DotNetVersions.V1_1, "http://www.microsoft.com/en-us/download/details.aspx?id=26"},
            {DotNetVersions.V2_0, "http://www.microsoft.com/en-us/download/details.aspx?id=1639"},
            {DotNetVersions.V3_0, "http://www.microsoft.com/en-us/download/details.aspx?id=3005"},
            {DotNetVersions.V3_5, "http://www.microsoft.com/en-us/download/details.aspx?id=22"},
            {DotNetVersions.V4_0_Client, "http://www.microsoft.com/en-us/download/details.aspx?id=24872"},
            {DotNetVersions.V4_0_Full, "http://www.microsoft.com/en-us/download/details.aspx?id=17718"},
            {DotNetVersions.V4_5, "http://www.microsoft.com/en-us/download/details.aspx?id=30653"},
            {DotNetVersions.V4_5_1, "http://www.microsoft.com/en-us/download/details.aspx?id=40779"}
        };

        private const string InnoScriptDetectDotNetVersion = @"
function IsDotNetDetected_{0}(version: string; service: cardinal): boolean;
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
//    'v4.5.1'        .NET Framework 4.5.1
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var        
    key: string;
    install, release, serviceCount: cardinal;
    check45, check451, success: boolean;
begin
    version := 'v' + version;
    // .NET 4.5 installs as update to .NET 4.0 Full
    if version = 'v4.5' then begin
        version := 'v4\Full';
        check45 := true;
    end else
        check45 := false;

	// .NET 4.5.1 installs as update to .NET 4.0 Full
    if version = 'v4.5.1' then begin
        version := 'v4\Full';
        check451 := true;
    end else
        check451 := false;
		
    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0/4.5/4.5.1 uses value Servicing instead of SP
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
	
	// .NET 4.5.1 uses additional value Release
    if check451 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= 378675);
    end;

    result := success and (install = 1) and (serviceCount >= service);
end;
";

        private const string InnoScriptInit = @"
if (not IsDotNetDetected_{0}('{1}', 0)) then begin
    {2}      
    {3}
    {4}
end;
";

        const string MessageBox = "MsgBox('{0}', mbInformation, MB_OK);";
        const string RunTemplate = "ShellExec('open', ExpandConstant('{0}'), '', '', SW_SHOW, {1}, ErrCode);";
        const string BrowserRunTemplate = "ShellExec('open', '{0}', '', '', SW_SHOW, {1}, ErrCode);";
        const string DownloadTemplate = "itd_addfile('{0}', expandconstant('{1}'));";
        const string StopIfNotInstalled = @"
if (not IsDotNetDetected_{0}('{1}', 0)) then begin
    FinalResult := false;
end;";
        const string abortIfNotInstalled = @"
if (not IsDotNetDetected_{0}('{1}', 0)) then begin
    {2}
    Abort();
end;
WizardForm.StatusLabel.Caption := '';";

        private readonly DotNetVersions _frameworkVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetFrameworkPrerequisite"/> class.
        /// </summary>
        /// <param name="frameworkVersion">The framework version.</param>
        public DotNetFrameworkPrerequisite(DotNetVersions frameworkVersion)
        {
            _frameworkVersion = frameworkVersion;
            AlertMessageForMissingPrerequisite = "You need to download and install .NET framework version {0}";
            InstallerSource = PrerequisiteInstallerSources.Web;
        }
       
        /// <summary>
        /// Gets the installer download web URL if source is web.
        /// </summary>
        public new string InstallerDownloadWebUrlIfSourceIsWeb
        {
            get { return _downloads[_frameworkVersion]; }
            set { _downloads[_frameworkVersion] = value; }
        }

        /// <summary>
        /// Gets the execution strategy.
        /// </summary>
        internal override AlterScript ExecutionStrategy
        {
            get
            {
                return script =>
                {
                    string functionPrefix = Id.ToString().Replace('-', '_');
                    int from = script.IndexOf(ScriptSectionStart, StringComparison.Ordinal);
                    script = script.Insert(from + ScriptSectionStart.Length, string.Format(InnoScriptDetectDotNetVersion, functionPrefix));

                    string frameworkVersion = EnumUtils.StringValueOf(_frameworkVersion);
                    string initAny = "";
                    switch (InstallerSource)
                    {
                        case PrerequisiteInstallerSources.Web:
                            {
                                initAny = string.Format(BrowserRunTemplate, InstallerDownloadWebUrlIfSourceIsWeb, "ewNoWait");
                                break;
                            }
                        case PrerequisiteInstallerSources.Embedded:
                            {
                                initAny = string.Format(RunTemplate, FilePathIfInstallSourceIsEmbedded[Architecture.Any], "ewWaitUntilTerminated");
                                break;
                            }
                        case PrerequisiteInstallerSources.WebAuto:
                            {
                                initAny = string.Format(DownloadTemplate, FileUrlIfInstallSourceIsWebAuto[Architecture.Any], "{tmp}\\dotnet.exe");
                                break;
                            }
                    }
                    from = script.IndexOf(InitSectionStart, StringComparison.Ordinal);
                    script = script.Insert(from + InitSectionStart.Length,
                        string.Format(
                            InnoScriptInit,
                            functionPrefix,
                            frameworkVersion,
                            AlertPrerequisiteInstallation ? string.Format(MessageBox, string.Format(AlertMessageForMissingPrerequisite, frameworkVersion)) : "",
                            initAny,
                            InstallerSource == PrerequisiteInstallerSources.WebAuto ? "" : string.Format(StopIfNotInstalled, functionPrefix, frameworkVersion)));

                    from = script.IndexOf(InstallSectionStart, StringComparison.Ordinal);
                    if (InstallerSource == PrerequisiteInstallerSources.WebAuto)
                    {
                        script = script.Insert(from + InstallSectionStart.Length,
                            Environment.NewLine + string.Format(PrerequisiteInstallationInProgressTemplate, MessageWhileInstallationInProgress) +
                            Environment.NewLine + string.Format(RunTemplate, "{tmp}\\dotnet.exe", "ewWaitUntilTerminated") +
                            Environment.NewLine + string.Format(abortIfNotInstalled, functionPrefix, frameworkVersion, string.Format(MessageBox, "Automatic installation of .net framework failed. Please try manual installation.")));
                    }

                    return script;
                };
            }
        }
    }
}