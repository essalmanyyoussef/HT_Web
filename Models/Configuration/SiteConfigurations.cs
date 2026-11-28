namespace HT_Web.Models.Configuration;

public static class SiteConfigurations
{
    public static readonly List<SiteConfiguration> Sites =
    [
        new() { Site = "K1", Server = "MOKEMLS001.SEWS-E.COM" },
        new() { Site = "K2", Server = "MOASMLS001.SEWS-E.COM" },
        new() { Site = "MFZ", Server = "MOFZMLS001.SEWS-E.COM" },
        new() { Site = "AA", Server = "MOAAMLS001.SEWS-E.COM" },
        new() { Site = "Skirat", Server = "MOSKMLS001.SEWS-E.COM" },
        new() { Site = "PortSaid", Server = "EGPSMLS001.SEWS-E.COM" },
        new() { Site = "TenthRamadan", Server = "EGTRMLS001.SEWS-E.COM" },
        new() { Site = "SixthOctober", Server = "EGSOMLS001.SEWS-E.COM" },
        new() { Site = "Alba", Server = "ROAIMLS001.SEWS-E.COM" },
        new() { Site = "Deva", Server = "RODVMLS001.SEWS-E.COM" },
        new() { Site = "Monastir", Server = "TNMOMLS001.SEWS-E.COM" }
    ];
}