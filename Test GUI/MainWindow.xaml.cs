using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using SpiceLogic.InnoSetupWrapper;
using SpiceLogic.InnoSetupWrapper.Models;
using SpiceLogic.InnoSetupWrapper.PrerequisiteStrategies;
using SpiceLogic.InnoSetupWrapper.Utilities;

namespace Test_GUI
{
    /// <summary>
    /// Interaction logic for DigitalSignTabMainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            initInnoSetupTab();
        }

        /// <summary>
        /// Initializes the inno setup tab.
        /// </summary>
        private void initInnoSetupTab()
        {
            string innosetupTestSampleFolderPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)))) + @"\Contents for Tests\InnoSetup Test Sample Folders";

            textboxCompanyName.Text = "Your Company Name";
            textboxProductName.Text = "Your Product Name";
            textboxShortCutName.Text = "Product";
            textboxProductId.Text = Guid.NewGuid().ToString().ToUpper();
            textboxProductVersion.Text = "1.1.1.1";
            textboxDeployableFolderPath.Text = innosetupTestSampleFolderPath + @"\Deployable";
            textboxStartUpApplictionFileName.Text = "Desktop UI.exe";
            textboxCompanyUrl.Text = "http://www.spicelogic.com/";
            textboxSupportUrl.Text = "http://www.spicelogic.com/HelpDesk";
            textboxIconFilePath.Text = innosetupTestSampleFolderPath + @"\Resources\MainApplicationIcon.ico";
            textboxEULAFilePath.Text = innosetupTestSampleFolderPath + @"\Resources\End User License Agreement.txt";
            textboxPlatform.Text = PlatformNames.AnyCPU.ToString();
            textboxUninstallerFeedbackUrl.Text = "http://www.google.com/";
            textboxGeneratedSetupScriptFolderPath.Text = innosetupTestSampleFolderPath + @"\Generated Setup Scripts";
            textboxOutPutSetupFilePath.Text = innosetupTestSampleFolderPath + @"\8051Setup.exe";
            textboxTargetDotNetFrameworkVersion.Text = EnumUtils.StringValueOf(DotNetVersions.V1_1);
            textboxSignToolPath.Text = "C:\\Program Files (x86)\\Windows Kits\\8.0\\bin\\x64\\signtool.exe";
            textboxInnoSetupCompiler.Text = "C:\\Program Files (x86)\\Inno Setup 5\\iscc.exe";
            textboxFileExtensionAssociation.Text = ".myProj";
            textboxDisplayName.Text = "My Proj";
            textboxTargetType.Text = WindowsShellContextMenuItem.TargetTypes.Folder.ToString();
            textboxExtensionConstraints.Text = ".myProj;.myProj2";
        }

        /// <summary>
        /// Handles the Click event of the btnBuildSetup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnBuildSetup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DotNetVersions dotNetVersion = (DotNetVersions)EnumUtils.EnumValueOf(textboxTargetDotNetFrameworkVersion.Text, typeof(DotNetVersions));

                SetupSettings innoSetupSetting = new SetupSettings
                {
                    CompanyName = textboxCompanyName.Text,
                    ProductName = textboxProductName.Text,
                    ShortCutName = textboxShortCutName.Text,
                    ProductId = Guid.Parse(textboxProductId.Text),
                    ProductVersion = new Version(textboxProductVersion.Text),
                    DeployableFolderPath = textboxDeployableFolderPath.Text,
                    StartUpApplictionFileName = textboxStartUpApplictionFileName.Text,
                    CompanyUrl = textboxCompanyUrl.Text,
                    SupportUrl = textboxSupportUrl.Text,
                    IconFilePath = textboxIconFilePath.Text,
                    EULAFilePath = textboxEULAFilePath.Text,
                    Platform = (PlatformNames)Enum.Parse(typeof(PlatformNames), textboxPlatform.Text),
                    UninstallerFeedbackUrl = textboxUninstallerFeedbackUrl.Text,
                    GeneratedSetupScriptFolderPath = textboxGeneratedSetupScriptFolderPath.Text,
                    OutPutSetupFilePath = textboxOutPutSetupFilePath.Text,
                    Prerequisites = new List<Prerequisite>
                    {
                        //new DotNetFrameworkPrerequisite(dotNetVersion),
                        //new DotNetFrameworkPrerequisite(dotNetVersion) { 
                        //    FilePathIfInstallSourceIsEmbedded = new Dictionary<Prerequisite.Architecture, string> 
                        //    {
                        //        { 
                        //            Prerequisite.Architecture.Any, @"C:\dotnetfx.exe"
                        //        }
                        //    }, 
                        //    AlertPrerequisiteInstallation = false 
                        //},
                        new DotNetFrameworkPrerequisite(dotNetVersion) { 
                            FileUrlIfInstallSourceIsWebAuto = new Dictionary<Prerequisite.Architecture, string> 
                            {
                                { 
                                    Prerequisite.Architecture.Any, "http://your-repository/dotnetfx.exe"
                                }
                            }, 
                            AlertPrerequisiteInstallation = false,
                            MessageWhileInstallationInProgress = "Please wait while setup install prerequisite: .net framework..."
                        },
                       
                        //new SQLExpress2012LocalDbPrerequisite
                        //{ 
                        //    FilePathIfInstallSourceIsEmbedded = new Dictionary<Prerequisite.Architecture, string> 
                        //    {
                        //        { Prerequisite.Architecture.X64, @"C:\SqlServer\SqlLocalDB.x64.MSI" },
                        //        { Prerequisite.Architecture.X86, @"C:\SqlServer\SqlLocalDB.x86.MSI" }
                        //    }
                        //},
                        new SQLExpress2012LocalDbPrerequisite
                        { 
                            FileUrlIfInstallSourceIsWebAuto = 
                            new Dictionary<Prerequisite.Architecture, string> 
                            {
                                { Prerequisite.Architecture.X64, "http://your-repository/SqlLocalDB.x64.MSI" },
                                { Prerequisite.Architecture.X86, "http://your-repository/SqlLocaLDB.x86.MSI" }
                            },
                            MessageWhileInstallationInProgress = "Please wait while setup install prerequisite: SqlLocalDB..."
                        }
                    },
                    FileExtensionAssociation = textboxFileExtensionAssociation.Text
                };

                if (chkContextMenu.IsChecked.HasValue && chkContextMenu.IsChecked.Value)
                {
                    WindowsShellContextMenuItem windowsShellContextMenuItem = new WindowsShellContextMenuItem
                    {
                        DisplayName = textboxDisplayName.Text,
                        TargetType = (WindowsShellContextMenuItem.TargetTypes)Enum.Parse(typeof(WindowsShellContextMenuItem.TargetTypes), textboxTargetType.Text),
                        ExtensionConstraints = textboxExtensionConstraints.Text.Split(';')
                    };
                    innoSetupSetting.ShellContextMenuItem = windowsShellContextMenuItem;
                }

                if (chkSign.IsChecked.HasValue && chkSign.IsChecked.Value)
                {
                    SignToolInfo theSignToolInfo = new SignToolInfo(textboxSignToolPath.Text)
                    {
                        PfxFilePath = textboxPfxFilePath.Text,
                        TimeStampServerUrl = textboxTimeStampServerUrl.Text,
                        //CertificateSubject = "Certificate Subject"
                    };

                    innoSetupSetting.SignToolInfo = theSignToolInfo;
                }

                InnoSetupService generator = new InnoSetupService(innoSetupSetting);

                string result = generator.BuildSetupFile(textboxInnoSetupCompiler.Text);
                new MessageDisplayBox(result, "Success").ShowDialog();

                if (MessageBox.Show("Do you want to run the Installer now ?", "Run now ?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Process prc = new Process { StartInfo = { FileName = textboxOutPutSetupFilePath.Text } };
                    prc.Start();
                }
            }
            catch (Exception ex)
            {
                new MessageDisplayBox(ex.ToString(), "Error").ShowDialog();
            }
        }

    }
}
