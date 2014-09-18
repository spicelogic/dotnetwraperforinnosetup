using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpiceLogic.InnoSetupWrapper.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SetupSettings
    {
        /// <summary>
        /// Gets or sets the name of the company. The installer will create CompanyName/ProductName/ShortCutName structure in Program Menu
        /// </summary>
        [Required]
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the product. The installer will create CompanyName/ProductName/ShortCutName structure in Program Menu.
        /// </summary>
        [Required]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the short name of the product. Moreover a Desktop icon will be created with the ShortCutName. PLEASE NOTE, 
        /// dont use StartUpApplictionFileName to be the name of the Shortcut icon and Start Menu short cut. The shortcut names will 
        /// be always this ProductName. An uninstaller will be created with this same shortcut name as 'Uninstall ' + ShortCutName
        /// </summary>
        [Required]
        public string ShortCutName { get; set; }

        /// <summary>
        /// Gets or sets the product id.
        /// </summary>
        [Required]
        public Guid ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product version.
        /// </summary>
        [Required]
        public Version ProductVersion { get; set; }

        /// <summary>
        /// Gets or sets the deployable folder path. A sample value can be C:\my projects\8051 App\Publish\Deployables"
        /// </summary>
        [Required]
        public string DeployableFolderPath { get; set; }

        /// <summary>
        /// Gets or sets the start name of up appliction file. A Sample value can be "Desktop UI.exe" That means, this file name will be searched within
        /// the DeployableFolderPath
        /// </summary>
        [Required]
        public string StartUpApplictionFileName { get; set; }

        /// <summary>
        /// Gets or sets the company URL.
        /// </summary>
        [Required]
        public string CompanyUrl { get; set; }

        /// <summary>
        /// Gets or sets the support URL.
        /// </summary>
        [Required]
        public string SupportUrl { get; set; }

        /// <summary>
        /// Gets or sets the icon file path. IF Set to null, use a default Icon. A sample value can be 
        /// C:\my projects\8051 App\Publish\Resources\MyIcon.Ico"
        /// </summary>
        [Required]
        public string IconFilePath { get; set; }

        /// <summary>
        /// Gets or sets the EULA file path. If set to null, then dont show EULA in the Install process at all.
        /// A sample value can be "C:\my projects\8051 App\Publish\Resources\EULA.rtf"
        /// </summary>
        public string EULAFilePath { get; set; }

        /// <summary>
        /// Gets or sets the platform.
        /// </summary>
        [Required]
        public PlatformNames Platform { get; set; }

        /// <summary>
        /// Gets or sets the sign tool command. If set to NULL, then dont sign.
        /// A value can be like this : "C:\Program Files (x86)\Windows Kits\8.0\bin\x64\signtool.exe sign /t http://timestamp.digicert.com /f D:\Work\Cert\MySPC.pfx $f"
        /// OR "C:\Program Files (x86)\Windows Kits\8.0\bin\x64\signtool.exe sign /t http://timestamp.digicert.com $f"
        /// </summary>
        public SignToolInfo SignToolInfo { get; set; }

        /// <summary>
        /// Gets or sets the uninstaller feedback URL. If set to NULL, then, DONT OPEN any URL after uninstall.
        /// </summary>
        public string UninstallerFeedbackUrl { get; set; }

        /// <summary>
        /// Gets or sets the generated setup script folder path. This is the folder where all Setup Script, BAT file, Project Metadata text file etc will be generated.
        /// A sample value can be "C:\my projects\8051 App\Publish\Scripts"
        /// </summary>
        [Required]
        public string GeneratedSetupScriptFolderPath { get; set; }

        /// <summary>
        /// Gets or sets the out put setup file path. This can be any absolute path like "C:\my projects\Publish\"
        /// </summary>
        [Required]
        public string OutPutSetupFilePath { get; set; }
        
        /// <summary>
        /// If I set this value to a file extension (for example ".spiceProj" then, InnoSetup will register .spiceProj 
        /// extension to the MainStartUp exe file and if I double click on a file with exntesion .spiceProj, the Main Startup 
        /// Exe file will be called and the File path of that .spiceProj will be sent to the Main Startup Exe file as argument. 
        /// Please make sure, that you need to UNREGISTER the extension when Uninstall.
        /// </summary>
        public string FileExtensionAssociation { get; set; }

        /// <summary>
        /// Optional. If this property is not null, then the installer will register necessary registry settings so that in Windows 
        /// Desktop (Chrome), if I right click on a File or Folder, I should be able to display a menu item and if I click that menu
        /// item, the StartUpApplictionFileName will be invoked with the argument where the argument is the file or folder FULL path 
        /// where the user invoked the context menu. Of course, when Uninstall, this registry settings will be REVERTED.
        /// </summary>
        public WindowsShellContextMenuItem ShellContextMenuItem { get; set; }

        /// <summary>
        /// All prerequisites will be installed according to the order registered in this list.
        /// </summary>
        public List<Prerequisite> Prerequisites { get; set; } 

    }
}