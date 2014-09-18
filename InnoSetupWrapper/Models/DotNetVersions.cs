using System.ComponentModel;

namespace SpiceLogic.InnoSetupWrapper.Models
{
    public enum DotNetVersions
    {
        [Description("none")]
        VNone,
        [Description("unknown")]
        VUnknown,
        [Description("1.1.4322")]
        V1_1,
        [Description("2.0.50727")]
        V2_0,
        [Description("3.0")]
        V3_0,
        [Description("3.5")]
        V3_5,
        [Description("4\\Client")]
        V4_0_Client,
        [Description("4\\Full")]
        V4_0_Full,
        [Description("4.5")]
        V4_5,
        [Description("4.5.1")]
        V4_5_1,
    }
}