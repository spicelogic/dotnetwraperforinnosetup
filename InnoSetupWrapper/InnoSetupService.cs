using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using SpiceLogic.InnoSetupWrapper.Models;
using SpiceLogic.InnoSetupWrapper.Properties;
using SpiceLogic.InnoSetupWrapper.Utilities;

namespace SpiceLogic.InnoSetupWrapper
{
    /// <summary>
    /// 
    /// </summary>
    public class InnoSetupService
    {
        private readonly SetupSettings _settings;

        private readonly string _innoScriptFilePath;
        private readonly string _generatedSetupScriptFolderPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="InnoSetupService" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.Exception"></exception>
        public InnoSetupService(SetupSettings settings)
        {
            IEnumerable<PropertyInfo> properties = settings.GetType().GetProperties();
            foreach (PropertyInfo p in properties)
            {
                RequiredAttribute[] attr = (RequiredAttribute[])p.GetCustomAttributes(typeof(RequiredAttribute), false);
                if (attr.Length > 0)
                {
                    // check if there is a value for the property.
                    object theValue = p.GetValue(settings, null);
                    if (theValue == null || theValue.ToString() == string.Empty)
                        throw new Exception(string.Format("The property {0} cannot be null or empty!", p.Name));
                }
            }

            _settings = settings;
            _innoScriptFilePath = string.Format("{0}\\Script.iss", settings.GeneratedSetupScriptFolderPath);
            _generatedSetupScriptFolderPath = settings.GeneratedSetupScriptFolderPath;
        }

        /// <summary>
        /// Builds the contex menu item.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="displayName">The display name.</param>
        /// <returns></returns>
        private static string buildContexMenuItem(string extension, string displayName)
        {
            string menuItem = string.Format("{0}Root: HKCR; Subkey: {1}\\shell\\{2}; ValueType: string; Flags: uninsdeletekey deletekey; ValueName: Icon; ValueData: \"\"\"{{app}}\\{{#MyAppIconName}}\"\"\"{0}Root: HKCR; Subkey: {1}\\shell\\{2}\\command; ValueType: string; ValueData: \"\"\"{{app}}\\{{#MyAppExeName}}\"\" \"\"%1\"\"\"; Flags: uninsdeletekey deletekey",
                Environment.NewLine, extension, displayName);
            return menuItem;
        }

        /// <summary>
        /// Generates the scripts.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void GenerateScripts()
        {
            try
            {
                if (Directory.Exists(_settings.GeneratedSetupScriptFolderPath))
                    DirectoryUtils.Delete(_settings.GeneratedSetupScriptFolderPath, CancellationToken.None);
            }
            catch (Exception err)
            {
                throw new Exception(string.Format("Error deleting existing Setup Script folder at {0}", _settings.GeneratedSetupScriptFolderPath), err);
            }

            try
            {
                Directory.CreateDirectory(_settings.GeneratedSetupScriptFolderPath);
            }
            catch (Exception err)
            {
                throw new Exception(string.Format("Error creating Script folder path at {0}", _settings.GeneratedSetupScriptFolderPath), err);
            }

            try
            {
                string script = Resources.InnoSetupScript;
               
                if (_settings.SignToolInfo == null)
                {
                    const string sectionStart = ";#section Signtool";
                    const string sectionEnd = ";#end_section Signtool";
                    int from = script.IndexOf(sectionStart, StringComparison.Ordinal);
                    int to = script.IndexOf(sectionEnd, StringComparison.Ordinal) + sectionEnd.Length;
                    script = script.Remove(from, to - from);
                }
              
                if (string.IsNullOrEmpty(_settings.FileExtensionAssociation))
                {
                    const string sectionStart = ";#section Association";
                    const string sectionEnd = ";#end_section Association";
                    int from = script.IndexOf(sectionStart, StringComparison.Ordinal);
                    int to = script.IndexOf(sectionEnd, StringComparison.Ordinal) + sectionEnd.Length;
                    script = script.Remove(from, to - from);
                    // this section contains two parts
                    from = script.IndexOf(sectionStart, StringComparison.Ordinal);
                    to = script.IndexOf(sectionEnd, StringComparison.Ordinal) + sectionEnd.Length;
                    script = script.Remove(from, to - from);
                }
               
                if (_settings.ShellContextMenuItem != null)
                {
                    const string sectionStart = ";#section ContextMenu";
                    //const string sectionEnd = ";#end_section ContextMenu";
                    int from = script.IndexOf(sectionStart, StringComparison.Ordinal);
                    //int to = script.IndexOf(sectionEnd, StringComparison.Ordinal) + sectionEnd.Length;
                    switch (_settings.ShellContextMenuItem.TargetType)
                    {
                        case WindowsShellContextMenuItem.TargetTypes.Folder:
                        {
                            string menuItem = buildContexMenuItem("Folder", _settings.ShellContextMenuItem.DisplayName);
                            script = script.Insert(from + sectionStart.Length, menuItem);
                        }
                            break;
                        case WindowsShellContextMenuItem.TargetTypes.File:
                        {
                            string menuItem = buildContexMenuItem("*", _settings.ShellContextMenuItem.DisplayName);
                            script = script.Insert(from + sectionStart.Length, menuItem);
                        }
                            break;
                        case WindowsShellContextMenuItem.TargetTypes.FileWithExtensionConstraint:
                        {
                            script = _settings.ShellContextMenuItem.ExtensionConstraints
                                .Select(extension => buildContexMenuItem(string.Format("SystemFileAssociations\\{0}", extension), _settings.ShellContextMenuItem.DisplayName))
                                .Aggregate(script, (current, menuItem) => current.Insert(@from + sectionStart.Length, menuItem));
                        }
                            break;
                    }
                }

                script = _settings.Prerequisites.Aggregate(script, (current, prerequisite) => prerequisite.ExecutionStrategy(current));

                File.WriteAllText(_innoScriptFilePath, script);
                File.WriteAllBytes(string.Format("{0}\\it_download.iss", _generatedSetupScriptFolderPath), Resources.Downloader_iss);
                File.WriteAllBytes(string.Format("{0}\\itdownload.dll", _generatedSetupScriptFolderPath), Resources.Downloader_dll);
            }
            catch (Exception err)
            {
                throw new Exception(string.Format("Error copying the InnoSetupScript at {0}", _innoScriptFilePath), err);
            }

            StringBuilder metaDataBuilder = new StringBuilder();
            metaDataBuilder.Append("[application]");
            metaDataBuilder.AppendLine();
            metaDataBuilder.AppendLine(string.Format("name={0}", _settings.ProductName));
            metaDataBuilder.AppendLine(string.Format("shortcut_name={0}", _settings.ShortCutName));
            metaDataBuilder.AppendLine(string.Format("version={0}", _settings.ProductVersion));
            metaDataBuilder.AppendLine(string.Format("publisher={0}", _settings.CompanyName));
            metaDataBuilder.AppendLine(string.Format("url={0}", _settings.CompanyUrl));
            metaDataBuilder.AppendLine(string.Format("support_url={0}", _settings.SupportUrl));
            metaDataBuilder.AppendLine(string.Format("startup_filename={0}", _settings.StartUpApplictionFileName));
            metaDataBuilder.AppendLine(string.Format("id={{{{{0}}}", _settings.ProductId));
            metaDataBuilder.AppendLine(string.Format("deployable_folder={0}", _settings.DeployableFolderPath));
            if (!string.IsNullOrEmpty(_settings.EULAFilePath))
                metaDataBuilder.AppendLine(string.Format("eula_file={0}", _settings.EULAFilePath));
            metaDataBuilder.AppendLine(string.Format("icon_path={0}", Path.GetDirectoryName(_settings.IconFilePath) + "\\"));
            metaDataBuilder.AppendLine(string.Format("icon_name={0}", Path.GetFileName(_settings.IconFilePath)));
            metaDataBuilder.AppendLine(string.Format("output_dir={0}", Path.GetDirectoryName(_settings.OutPutSetupFilePath) + "\\"));
            metaDataBuilder.AppendLine(string.Format("output_filename={0}", Path.GetFileNameWithoutExtension(_settings.OutPutSetupFilePath)));
            if (!string.IsNullOrEmpty(_settings.FileExtensionAssociation))
                metaDataBuilder.AppendLine(string.Format("extension={0}", _settings.FileExtensionAssociation));
            metaDataBuilder.AppendLine();
            metaDataBuilder.AppendLine();
            metaDataBuilder.AppendLine("[platform]");
            metaDataBuilder.AppendLine();
            switch (_settings.Platform)
            {
                case PlatformNames.AnyCPU_Prefer32Bit:
                    metaDataBuilder.AppendLine("cpu=AnyCPU_Prefer32Bit");
                    break;
                case PlatformNames.x86:
                    metaDataBuilder.AppendLine("cpu=32bit");
                    break;
                case PlatformNames.x64:
                    metaDataBuilder.AppendLine("cpu=64bit");
                    break;
                default:
                    metaDataBuilder.AppendLine("cpu=AnyCPU");
                    break;
            }

            metaDataBuilder.AppendLine();
            metaDataBuilder.AppendLine();
            metaDataBuilder.AppendLine("[uninstall]");
            metaDataBuilder.AppendLine();
            if (!string.IsNullOrEmpty(_settings.UninstallerFeedbackUrl))
                metaDataBuilder.AppendLine(string.Format("feedback_url={0}", _settings.UninstallerFeedbackUrl));

            string finalMetaDataFileContent = metaDataBuilder.ToString();
            string projectMetaDataFilePath = string.Format("{0}\\ProjectMetaData.txt", _settings.GeneratedSetupScriptFolderPath);
            
            try
            {
                File.WriteAllText(projectMetaDataFilePath, finalMetaDataFileContent);
            }
            catch (Exception err)
            {
                throw new Exception(string.Format("Error copying the Project Metadata File at {0}", projectMetaDataFilePath), err);
            }
        }

        /// <summary>
        /// Builds the setup file silently. Throws Exception if Build Fails.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string BuildSetupFile(string innoSetupCompilerExePath)
        {
            GenerateScripts();

            if (!File.Exists(innoSetupCompilerExePath))
                throw new Exception(string.Format("InnoSetup was not found at {0}", innoSetupCompilerExePath));
            if (!File.Exists(_innoScriptFilePath))
                throw new Exception(string.Format("Inno Setup Script file was not found at {0}", _innoScriptFilePath));

            string argument;
            if (_settings.SignToolInfo != null)
            {
                SignToolCommand theSignCommand = new SignToolCommand(_settings.SignToolInfo) { SignableFilePath = "$f" };
                if (string.IsNullOrWhiteSpace(theSignCommand.ToolInfo.SignToolPath))
                    throw new Exception("The SignToolPath cannot be null or empty!");
                if (!File.Exists(theSignCommand.ToolInfo.SignToolPath))
                    throw new Exception(string.Format("The SignToolPath was not found at {0}", theSignCommand.ToolInfo.SignToolPath));
                argument = string.Format("\"/ssigntool={0}\" \"{1}\"", theSignCommand, _innoScriptFilePath);
            }
            else
                argument = string.Format("\"{0}\"", _innoScriptFilePath);

            ProcessStartInfo psi = new ProcessStartInfo(innoSetupCompilerExePath, argument)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            using (Process process = Process.Start(psi))
            {
                string output = null;
                if (process != null)
                {
                    using (StreamReader myOutput = process.StandardOutput)
                    {
                        output = myOutput.ReadToEnd();
                    }
                    using (StreamReader myError = process.StandardError)
                    {
                        output += String.Format("\n{0}", myError.ReadToEnd());
                    }

                    process.WaitForExit();

                    if (process.ExitCode != 0)
                        throw new Exception(string.Format("Error Compiling InnoSetup script. {0}{1}", Environment.NewLine, output));

                    process.Close();
                }

                return output;
            }
        }
    }
}
