namespace HT_Web.Models.Configuration;

public static class SiteConfigurations
{
    public static readonly List<SiteConfiguration> Sites =
    [
        new() { Site = "K1", Server = "MOKEMLS001" },
        new() { Site = "K2", Server = "MOASMLS001" },
        new() { Site = "MFZ", Server = "MOFZMLS001" },
        new() { Site = "AA", Server = "MOAAMLS001" },
        new() { Site = "Skirat", Server = "MOSKMLS001" },
        new() { Site = "PortSaid", Server = "EGPSMLS001" },
        new() { Site = "TenthRamadan", Server = "EGTRMLS001" },
        new() { Site = "SixthOctober", Server = "EGSOMLS001" },
        new() { Site = "Alba", Server = "ROAIMLS001" },
        new() { Site = "Deva", Server = "RODVMLS001" },
        new() { Site = "Monastir", Server = "TNMOMLS001" }
    ];
}