namespace SpiceLogic.InnoSetupWrapper.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class WindowsShellContextMenuItem
    {
        /// <summary>
        /// 
        /// </summary>
        public enum TargetTypes
        {
            /// <summary>
            /// The file
            /// </summary>
            File,
            /// <summary>
            /// The file with extension constraint
            /// </summary>
            FileWithExtensionConstraint,
            /// <summary>
            /// The folder
            /// </summary>
            Folder
        }

        /// <summary>
        /// This will be shown in the Context Menu when right click on a file or folder in Windows Explorer
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// This will determine if the context menu will be shown on Files or Folders
        /// </summary>
        public TargetTypes TargetType { get; set; }

        /// <summary>
        /// if the Target Type is FileWithExtensionConstraint, then, the extensions that will be provided in this property will be 
        /// used to determine if the context menu will be shown or not. Example values : array of strings : ".jpg", ".exe" etc
        /// </summary>
        public string[] ExtensionConstraints { get; set; }
    }
}