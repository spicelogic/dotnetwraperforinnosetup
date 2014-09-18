using System;
using SpiceLogic.InnoSetupWrapper.Models;

namespace SpiceLogic.InnoSetupWrapper.PrerequisiteStrategies
{
    /// <summary>
    /// 
    /// </summary>
    public class SQLExpress2012LocalDbPrerequisite : Prerequisite
    {
        private const string InnoScriptDetectSqlServerExpressLocalDb = @"
function IsLocalDbDetected_{0}(): boolean;
var        
    key: string;
    parentInstance: String;
    success: boolean;
begin		
    key := 'SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions\11.0';
    success := RegQueryStringValue(HKLM, key, 'ParentInstance', parentInstance);
    result := success;
end;
";

        private const string InnoScriptInit = @"
if (not IsLocalDbDetected_{0}()) then begin
    {1}      
    if IsWin64 then begin
      {2}
    end else begin
      {3}
    end;
    {4}
end;
";

        const string MessageBox = "MsgBox('{0}', mbInformation, MB_OK);";
        const string MsiRunTemplate = "ShellExec('open', 'msiexec', '/i ' + AddQuotes(ExpandConstant('{0}')) + ' /qn IACCEPTSQLLOCALDBLICENSETERMS=YES', '', SW_SHOW, {1}, ErrCode);";
        const string BrowserRunTemplate = "ShellExec('open', '{0}', '', '', SW_SHOW, {1}, ErrCode);";
        const string DownloadTemplate = "itd_addfile('{0}', expandconstant('{1}'));";
        const string StopIfNotInstalled = @"
if (not IsLocalDbDetected_{0}()) then begin
    FinalResult := false;
end;";
        const string abortIfNotInstalled = @"
if (not IsLocalDbDetected_{0}()) then begin
    {1}
    Abort();
end;
WizardForm.StatusLabel.Caption := '';";

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLExpress2012LocalDbPrerequisite"/> class.
        /// </summary>
        public SQLExpress2012LocalDbPrerequisite()
        {
            AlertMessageForMissingPrerequisite = "You need to install Microsoft SQL Server 2012 Express LocalDB.";
        }

        /// <summary>
        /// Gets the installer download web URL if source is web.
        /// </summary>
        protected virtual string InstallerDownloadWebUrlIfSourceIsWeb
        {
            get { return "http://www.microsoft.com/en-us/download/details.aspx?id=29062"; }
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
                    script = script.Insert(from + ScriptSectionStart.Length, string.Format(InnoScriptDetectSqlServerExpressLocalDb, functionPrefix));

                    if (InstallerSource != PrerequisiteInstallerSources.Embedded)
                    {
                        string initX64 = "";
                        string initX86 = "";
                        switch (InstallerSource)
                        {
                            case PrerequisiteInstallerSources.Web:
                                {
                                    initX64 = string.Format(BrowserRunTemplate, InstallerDownloadWebUrlIfSourceIsWeb, "ewNoWait");
                                    initX86 = string.Format(BrowserRunTemplate, InstallerDownloadWebUrlIfSourceIsWeb, "ewNoWait");
                                    break;
                                }
                            case PrerequisiteInstallerSources.WebAuto:
                                {
                                    initX64 = string.Format(DownloadTemplate, FileUrlIfInstallSourceIsWebAuto[Architecture.X64], "{tmp}\\SqlLocalDB.MSI");
                                    initX86 = string.Format(DownloadTemplate, FileUrlIfInstallSourceIsWebAuto[Architecture.X86], "{tmp}\\SqlLocalDB.MSI");
                                    break;
                                }
                        }
                        from = script.IndexOf(InitSectionStart, StringComparison.Ordinal);
                        script = script.Insert(from + InitSectionStart.Length,
                            string.Format(
                                InnoScriptInit,
                                functionPrefix,
                                AlertPrerequisiteInstallation && InstallerSource != PrerequisiteInstallerSources.WebAuto ?
                                    string.Format(MessageBox, AlertMessageForMissingPrerequisite) :
                                    "",
                                initX64,
                                initX86,
                                InstallerSource == PrerequisiteInstallerSources.WebAuto ? "" : string.Format(StopIfNotInstalled, functionPrefix)));
                    }

                    from = script.IndexOf(InstallSectionStart, StringComparison.Ordinal);
                    switch (InstallerSource)
                    {
                        case PrerequisiteInstallerSources.WebAuto:
                            {
                                script = script.Insert(from + InstallSectionStart.Length,
                                    Environment.NewLine + string.Format(PrerequisiteInstallationInProgressTemplate, MessageWhileInstallationInProgress) +
                                    Environment.NewLine + string.Format(MsiRunTemplate, "{tmp}\\SqlLocalDB.MSI", "ewWaitUntilTerminated") +
                                    Environment.NewLine + string.Format(abortIfNotInstalled, functionPrefix, string.Format(MessageBox, "Automatic installation of SQL Server LocalDB failed. Please try manual installation.")));
                                break;
                            }
                        case PrerequisiteInstallerSources.Embedded:
                            {
                                string installX64 = string.Format(MsiRunTemplate, FilePathIfInstallSourceIsEmbedded[Architecture.X64], "ewWaitUntilTerminated");
                                string installX86 = string.Format(MsiRunTemplate, FilePathIfInstallSourceIsEmbedded[Architecture.X86], "ewWaitUntilTerminated");
                                script = script.Insert(from + InstallSectionStart.Length,
                                    string.Format(
                                        InnoScriptInit,
                                        functionPrefix,
                                        "",
                                        installX64,
                                        installX86,
                                        string.Format(abortIfNotInstalled, functionPrefix, string.Format(MessageBox, "Automatic installation of SQL Server LocalDB failed. Please try manual installation."))));
                                break;
                            }
                    }
                    return script;
                };
            }
        }
    }
}