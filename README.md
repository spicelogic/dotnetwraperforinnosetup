.Net Wraper For Inno Setup
========================

.NET Wrapper for Inno Setup


Welcome to the .NET Wraper For InnoSetup (http://www.jrsoftware.org/isinfo.php)

It is a .NET Class library project that gives .NET Developers the pleasure of using Inno Setup without learning any Pascal scripting language. Not only that, this wrapper .NET Interface will allow the .NET Programmers to create installer file automatically as part of Continuous Integration directly using .NET Language. The .NET Developers will be able to call a method named BuildSetupFile() in a Wrapper class named InnoSetupService. Yes, that will build the Installer exe file. There is nothing to learn actually if you want to use this class library for creating your Installer.

Yes, you do need to configure your installer and the Constructor of this InnoSetupService takes an object which contains various properties. By setting various properties, you can configure the installer. So, the very basic start up usage would be like this:

    private void btnBuildInstaller_Click(object sender, RoutedEventArgs e)
        {
            Guid productGuid = Guid.NewGuid();

            SetupSettings innoSetupSetting = new SetupSettings
            {
                CompanyName = "Your Company Name",
                // A folder with this company name will be created in the Start Menu / Program Files folder
                //and your Product short cut will be placed within this Company Name folder.

                ProductName = "your Product Name", 

                ProductId = productGuid,
                // You should keep your product Guid Unique so that you can publish update easily with same Guid.

                ProductVersion = new Version("1.0.0.0"), // Your product version


                DeployableFolderPath = "C:\\My Project\\Deployable",
                // This folder contains all of the exe, dll etc whatever you want to publish to your user's computer. 
                // You do not need to specify each file name. Whatever file placed in this deployable folder will be 
                // published to your user's computer.

                StartUpApplictionFileName = "My Exciting Calculator.exe",
                // This is the main executive file name which will be placed in the Start Menu

                ShortCutName = "Exciting Calculator", // Well, this shortcut name will be shown in the start menu,
                //if  your product name is very big, it is better to use a short cut name to identify your product 
                // quickly and this property holds that short cut name.

                CompanyUrl = "http://your-company-website.com",

                SupportUrl = "http://your-company-website.com/ContactUs",

                IconFilePath = "C:\\My Project\\Resources\\my-icon.ico", // This icon will be shown in Desktop and Start Menu.

                EULAFilePath = "C:\\My Project\\Resources\\End-User-License-Agreements.txt",
                // If you set this property, only then a End User License Agreement Screen will be shown, otherwise no EULA screen will be shown.

                Platform = PlatformNames.AnyCPU_Prefer32Bit,
                // This property is self explanatory, you can set PlatformNames.AnyCPU, PlatformNames.x64, PlatformNames.x32 etc.
                // If you set value PlatformNames.x64 then your application will be installed in "C:\Program Files" folder. 
                // If you set value PlatformNames.x32, then your application will be installed in "C:\Program Files (x86)" folder. 
                // If you set value PlatformNames.AnyCPU, then, if your user's OS is Windows 64 bit, then your application will be
                // installed in "C:\Program Files" folder otherwise it will be installed in "C:\Program Files (x86)" folder.


                UninstallerFeedbackUrl = "http://your-company-website.com/uninstall-feedback",
                // If you set this property, then after uninstall, this url will be opened where you can ask your user to provide
                // additional feedback.

                GeneratedSetupScriptFolderPath = "C:\\My Project\\Temporary Scripts",
                // This folder is the place where InnoSetup Intermediate scripts will be generated. 
                // As you understand that, your C# code will generate Pascal Scripts for InnoSetup and 
                // finally that Pascal script will be executed to build your installer file. So, this 
                // folder will be used to store the auto generated Pascal Scripts. You will never need to 
                // look insude this folder. Everytime you call the BuildSetupFile(), old scripts will be 
                // deleted and new scripts will be written.


                OutPutSetupFilePath = "C:\\My Project\\Publish\\Setup.exe",
                // This is the path where your final Installer file will be created.
                // If a file already exists in this path then the existing file will be replaced.


                Prerequisites = new List<Prerequisite> // This is a collection of prerequisites. 
                // Prerequisites can be .NET Framework, SQL Express Local Db etc.
                // Every prerequisite can be configured to either install directly from Installer 
                // or Download from Web. IF you prefer to download from Web, then you can super configure
                // to define if the required prerequisite file should be downloaded automatically from a 
                // remote URL or it will simply navigate to a Download page and user can download the necessary
                // prerequisite from that page. For example, if you want to define .NET Framework 4.5 is a 
                // prerequisite for your application, then, you can navigate your user to the download page of 
                // microsoft .net framework 4.5. 
                {
                    new DotNetFrameworkPrerequisite(DotNetVersions.V4_5)
                    // This example is a very basic common usage. Here we did not set any extra property of the DotNetFrameworkPrerequisite.
                    // Therefore, by default, the installer will check if the .NET framework version 4.5 is installed in user machine or not.
                    // If not, then, the installer will take the user to the official Microsoft .NET framework 4.5 download page.

                    // You can set many properties of this class to customize the behavior. For example, you can host the .NET Framework in your 
                    // own server and silently download and install to user's machine so that if the user wont be aware about what is .NET framework,
                    // you wont risk missing the user's installation. Please check the documentation for details.
                },

                FileExtensionAssociation = ".abc",
                // Yes, if you want to associate any file that has extension *.abc with the start up application,
                // then, you set this property. That means, if a file with extension *.abc is double clicked in user's PC,
                // then the exe file defined in the property StartUpApplictionFileName = "My Exciting Calculator.exe", will be 
                // invoked and the full file path of the double clicked file will be passed to this exe file as command argument.
                // So, you can program your application to receive this file path as command Argument.


                SignToolInfo = new SignToolInfo("C:\\Program Files (x86)\\Windows Kits\\8.0\\bin\\x64\\signtool.exe") 
                // The constructor takes the path to your SignTool.exe file.
               
                // Yes, you can program to Sign All Signable files (like exe and dll files) including the Uninstaller.
                // You read right. Yes, your uninstaller will be signed too, which is really an exciting and rare feature
                // in a installer software. If you do not want to digitally sign your files, then, do not set this SignToolInfo property.
                {
                    PfxFilePath = "C:\\your-pfx-file-path.pfx",
                    TimeStampServerUrl = "http://digicert.timestampserver.com", // a TimeStamp Server Url
                    CertificateSubject = "Certificate Subject"
                },

                ShellContextMenuItem = new WindowsShellContextMenuItem
                // Another exciting feature is Windows Context Menu Shell Integration.
                // That means, you can right click on a file or folder and invoke your application.
                // If you do not want the Shell Integration, then do not set this property.
                {
                    DisplayName = "Resize Image", // This text will be shown in the Context Menu
                    
                    // Say for example, your application is an Image Resizer application, then, you may want to 
                    // allow the user to right click on an image type file to invoke the application.
                    // If the user right click on an Image file and click the menu item 'Resize Image', 
                    // then your startup exe application defined in the property StartUpApplictionFileName = "My Exciting Calculator.exe", will be 
                    // invoked and the full file path of the double clicked file will be passed to this exe file as command argument.
                    // So, you can program your application to receive this file path as command Argument.

                    TargetType = WindowsShellContextMenuItem.TargetTypes.FileWithExtensionConstraint,
                    // This Target type can be WindowsShellContextMenuItem.TargetTypes.FileWithExtensionConstraint, 
                    // WindowsShellContextMenuItem.TargetTypes.File or WindowsShellContextMenuItem.TargetTypes.Folder.
                    // If you set WindowsShellContextMenuItem.TargetTypes.FileWithExtensionConstraint, that means,
                    // The context menu will be shown on File but if the file has extensions defined in the array type 
                    // property named ExtensionConstraints. IF you set WindowsShellContextMenuItem.TargetTypes.Folder
                    // for this TargetType, then the ContextMenu will be shown on Folders only. If you set TargetType
                    // = WindowsShellContextMenuItem.TargetTypes.File then the Context Menu will be shown on Any File type.

                    ExtensionConstraints = new[] {  ".jpg", ".png"}
                },
                
            };


            // Ok, once we have setup various properties for the object  SetupSettings innoSetupSetting,
            // we can instantiate the  InnoSetupService and pass this Settings object to the constructor.

            InnoSetupService generator = new InnoSetupService(innoSetupSetting, innoSetupCompilerExePath: "C:\\Program Files (x86)\\Inno Setup 5\\iscc.exe");
            // Yes, you need to pass your InnoSetup Compiler File Path. At the time of this writing, This wrapper library is working fine on Inno Setup 5. 
            // And it should work on future versions too. If this library stops to work on future version, then, as this wrapper library source code is open,
            // hopefully someone will come up with an upgrade which will work on the future version.

            
            // finally call the BuildSetupFile() method of InnoSetupService Class. This method will return a log string which 
            // is captured from the Console output of InnoSetup.
            string result = generator.BuildSetupFile();

            // If all goes good, then, your Setup exe file is built by this time and stored in the path you defined in the property
            // named OutPutSetupFilePath.

        }



When you build your installer using thi wrapper, your installer will create an uninstaller automatically and place the uninstaller in the Program Files / Start Menu folder. The uninstaller created by this wrapper will be smart enough to delete the User's App Data Directory created by this application. Not only that, if no other application has been installed from the same company (your company), then the Company App Data folder will be deleted too upon uninstallation.
