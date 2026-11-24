namespace HT_Web.Services.Traceability;

public class VisionReportService
{
    private static readonly Dictionary<string, string> VisionBasePaths = new(StringComparer.OrdinalIgnoreCase)
    {
        { "MOKE", @"\\mokebks001\ETArchive$" },
        { "MOAS", @"\\moasvcs001\ETArchive$" },
        { "MOMZ", @"\\mofzbks001\ETArchive$" },
        { "MOAA", @"\\moaabks001\ETArchive$" },
        { "MOSK", @"\\moskvcs001\ETArchive$" },
        { "TNMO", @"\\tnmobks001\ETArchive$" },
        { "ESPS", @"\\egpsbks001\ETArchive$" },
        { "RODV", @"\\rodvbks001\ETArchive$" },
        { "ESSO", @"\\egpsbks001\ETArchive$" },
        { "ROAI", @"\\roaibks001\ETArchive$" }
    };

    public (string? FilePath, string? ErrorMessage) FindVisionReport(string? rawHostname, DateTime visionDatetime)
    {
        if (string.IsNullOrWhiteSpace(rawHostname))
            return (null, "Hostname is missing for this Vision station.");

        var hostnames = rawHostname
            .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        string? lastError = null;

        foreach (var hostname in hostnames)
        {
            var result = FindForSingleHost(hostname, visionDatetime);
            if (!string.IsNullOrWhiteSpace(result.FilePath))
                return result;

            lastError = result.ErrorMessage;
        }

        return (null, lastError ?? "The PDF report does not exist.");
    }

    private static (string? FilePath, string? ErrorMessage) FindForSingleHost(string visionHostname, DateTime visionDatetime)
    {
        try
        {
            string? basePath = null;
            foreach (var kvp in VisionBasePaths)
            {
                if (visionHostname.StartsWith(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    basePath = kvp.Value;
                    break;
                }
            }

            if (basePath is null)
                return (null, $"Unknown Vision hostname prefix: {visionHostname}");

            if (!Directory.Exists(basePath))
                return (null, $"Archive share is not accessible: {basePath}");

            string[] matchingFolders = Directory.GetDirectories(basePath, visionHostname + "*");
            if (matchingFolders.Length == 0)
                return (null, $"No folder found starting with: {visionHostname}");

            string machineFolder = matchingFolders[0];

            string? validationPath = Directory.GetDirectories(machineFolder)
                .FirstOrDefault(d => Path.GetFileName(d).Contains("validation", StringComparison.OrdinalIgnoreCase));

            if (validationPath is null)
                return (null, $"No validation folder found in: {machineFolder}");

            string expectedYearMonth = $"{visionDatetime:yyyy_MM}";
            string exactYearMonthPath = Path.Combine(validationPath, expectedYearMonth);

            string? bestYearMonthFolder = Directory.Exists(exactYearMonthPath)
                ? exactYearMonthPath
                : Directory.GetDirectories(validationPath)
                    .OrderBy(f =>
                    {
                        string digits = Path.GetFileName(f).Replace("_", "");
                        return int.TryParse(digits, out int val)
                            ? Math.Abs(val - (visionDatetime.Year * 100 + visionDatetime.Month))
                            : int.MaxValue;
                    })
                    .FirstOrDefault();

            if (bestYearMonthFolder is null)
                return (null, "No valid Year_Month folder found.");

            string dayString = visionDatetime.Day.ToString("00");
            string? selectedDayFolder = Directory.GetDirectories(bestYearMonthFolder)
                .FirstOrDefault(f => Path.GetFileName(f).EndsWith(dayString, StringComparison.OrdinalIgnoreCase));

            if (selectedDayFolder is null)
                return (null, $"The PDF report doesn't exist for day {dayString}.");

            string[] pdfFiles = Directory.GetFiles(selectedDayFolder, "*.pdf");
            if (pdfFiles.Length == 0)
                return (null, $"No PDF report found in: {selectedDayFolder}");

            FileInfo? bestMatch = pdfFiles
                .Select(f => new FileInfo(f))
                .Where(fi => fi.LastWriteTime <= visionDatetime)
                .OrderBy(fi => visionDatetime - fi.LastWriteTime)
                .FirstOrDefault();

            bestMatch ??= pdfFiles
                .Select(f => new FileInfo(f))
                .OrderBy(fi => Math.Abs((fi.LastWriteTime - visionDatetime).Ticks))
                .FirstOrDefault();

            return bestMatch is not null
                ? (bestMatch.FullName, null)
                : (null, "No suitable PDF found.");
        }
        catch (Exception ex)
        {
            return (null, $"Vision report error: {ex.Message}");
        }
    }
}