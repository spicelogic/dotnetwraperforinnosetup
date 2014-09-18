using System;
using System.Collections.Generic;

namespace SpiceLogic.InnoSetupWrapper.Models
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Prerequisite
    {
        /// <summary>
        /// 
        /// </summary>
        protected enum PrerequisiteInstallerSources
        {
            /// <summary>
            /// The web
            /// </summary>
            Web,
            /// <summary>
            /// The embedded
            /// </summary>
            Embedded,
            /// <summary>
            /// The web automatic
            /// </summary>
            WebAuto
        }

        /// <summary>
        /// 
        /// </summary>
        public enum Architecture
        {
            /// <summary>
            /// Any
            /// </summary>
            Any,
            /// <summary>
            /// The X64
            /// </summary>
            X64,
            /// <summary>
            /// The X86
            /// </summary>
            X86
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        public delegate string AlterScript(string script);

        private Dictionary<Architecture, string> _filePathIfInstallSourceIsEmbedded;

        private Dictionary<Architecture, string> _fileUrlIfInstallSourceIsWebAuto;

        protected const string ScriptSectionStart = "//#section PrerequisiteScripts";

        protected const string InitSectionStart = "//#section PrerequisiteInit";

        protected const string InstallSectionStart = "//#section PrerequisiteInstall";

        protected const string PrerequisiteInstallationInProgressTemplate = "WizardForm.StatusLabel.Caption := '{0}';";

        /// <summary>
        /// Gets or sets the installer source.
        /// </summary>
        /// <value>
        /// The installer source.
        /// </value>
        protected PrerequisiteInstallerSources InstallerSource { get; set; }
        
        /// <summary>
        /// Gets or sets the file path if install source is embedded.
        /// </summary>
        public Dictionary<Architecture, string> FilePathIfInstallSourceIsEmbedded
        {
            get { return _filePathIfInstallSourceIsEmbedded; }
            set
            {
                if (_filePathIfInstallSourceIsEmbedded != value)
                {
                    _filePathIfInstallSourceIsEmbedded = value;
                    InstallerSource = value == null ? PrerequisiteInstallerSources.Web : PrerequisiteInstallerSources.Embedded;
                }
            }
        }

        /// <summary>
        /// Gets or sets the file URL if install source is web automatic.
        /// </summary>
        public Dictionary<Architecture, string> FileUrlIfInstallSourceIsWebAuto
        {
            get { return _fileUrlIfInstallSourceIsWebAuto; }
            set
            {
                if (_fileUrlIfInstallSourceIsWebAuto != value)
                {
                    _fileUrlIfInstallSourceIsWebAuto = value;
                    InstallerSource = value == null ? PrerequisiteInstallerSources.Web : PrerequisiteInstallerSources.WebAuto;
                }
            }
        }

        internal abstract AlterScript ExecutionStrategy { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [alert prerequisite installation].
        /// </summary>
        /// <value>
        /// <c>true</c> if [alert prerequisite installation]; otherwise, <c>false</c>.
        /// </value>
        public bool AlertPrerequisiteInstallation { get; set; }

        /// <summary>
        /// Gets or sets the alert message for missing prerequisite.
        /// </summary>
        /// <value>
        /// The alert message for missing prerequisite.
        /// </value>
        public string AlertMessageForMissingPrerequisite { get; set; }

        /// <summary>
        /// Gets or sets the message while installation in progress.
        /// </summary>
        /// <value>
        /// The message while installation in progress.
        /// </value>
        public string MessageWhileInstallationInProgress { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Prerequisite"/> class.
        /// </summary>
        protected Prerequisite()
        {
            Id = Guid.NewGuid();
            AlertPrerequisiteInstallation = true;
        }
    }
}