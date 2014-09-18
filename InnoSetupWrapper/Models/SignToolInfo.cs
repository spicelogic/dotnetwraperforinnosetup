namespace SpiceLogic.InnoSetupWrapper.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SignToolInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignToolInfo"/> class.
        /// </summary>
        /// <param name="signToolPath">The sign tool path.</param>
        public SignToolInfo(string signToolPath)
        {
            SignToolPath = signToolPath;
        }

        /// <summary>
        /// Gets or sets the sign tool path.
        /// </summary>
        public string SignToolPath { get; private set; }

        /// <summary>
        /// Gets or sets the PFX file path.
        /// </summary>
        public string PfxFilePath { get; set; }

        /// <summary>
        /// Gets or sets the time stamp server URL.
        /// </summary>
        public string TimeStampServerUrl { get; set; }

        /// <summary>
        /// Gets or sets the name of the certificate subject.
        /// </summary>
        public string CertificateSubject { get; set; }
    }
}