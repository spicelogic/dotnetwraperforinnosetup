using System.Text;

namespace SpiceLogic.InnoSetupWrapper.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SignToolCommand  
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignToolCommand" /> class.
        /// </summary>
        /// <param name="toolInfo">The tool info.</param>
        public SignToolCommand(SignToolInfo toolInfo)
        {
            ToolInfo = toolInfo;
        }

        /// <summary>
        /// Gets the tool info.
        /// </summary>
        public SignToolInfo ToolInfo { get; private set; }

        /// <summary>
        /// Gets or sets the signable file path.
        /// </summary>
        public string SignableFilePath { private get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", ToolInfo.SignToolPath, getSignToolCommandArgument());
        }

        /// <summary>
        /// Gets the sign tool command argument.
        /// </summary>
        /// <returns></returns>
        private string getSignToolCommandArgument()
        {
            StringBuilder commandBuilder = new StringBuilder();
            commandBuilder.AppendFormat("sign");
            if(!string.IsNullOrWhiteSpace(ToolInfo.CertificateSubject))
                commandBuilder.AppendFormat(" /n \"{0}\"", ToolInfo.CertificateSubject.Trim());
            if (!string.IsNullOrWhiteSpace(ToolInfo.TimeStampServerUrl))
                commandBuilder.AppendFormat(" /t {0}", ToolInfo.TimeStampServerUrl.Trim());
            if (!string.IsNullOrWhiteSpace(ToolInfo.PfxFilePath))
                commandBuilder.AppendFormat(" /f {0}", ToolInfo.PfxFilePath.Trim());
            if (!string.IsNullOrWhiteSpace(SignableFilePath))
                commandBuilder.AppendFormat(" /a \"{0}\"", SignableFilePath.Trim());
            return commandBuilder.ToString();
        }
    }
}