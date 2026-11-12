using MudBlazor;

namespace HT_Web.Theme;

public static class SEWSTheme
{
    public static MudTheme Theme = new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = "#0A2540", // Bleu nuit moderne Sumitomo/SEWS
            PrimaryContrastText = "#FFFFFF",
            Secondary = "#0066CC", // Bleu cobalt vif
            SecondaryContrastText = "#FFFFFF",
            Tertiary = "#20C997", // Vert émeraude industriel
            Success = "#10B981",
            Warning = "#F59E0B",
            Error = "#EF4444",
            Info = "#3B82F6",
            Background = "#F8FAFC", // Gris très doux / ardoise
            Surface = "#FFFFFF",
            AppbarBackground = "#0A2540",
            AppbarText = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#1E293B",
            DrawerIcon = "#64748B",
            TextPrimary = "#0F172A",
            TextSecondary = "#475569",
            Divider = "#E2E8F0",
            ActionDefault = "#64748B"
        },
        PaletteDark = new PaletteDark()
        {
            Primary = "#38BDF8",
            Secondary = "#818CF8",
            Tertiary = "#34D399",
            Success = "#34D399",
            Warning = "#FBBF24",
            Error = "#F87171",
            Info = "#60A5FA",
            Background = "#0B0F19",
            Surface = "#111827",
            AppbarBackground = "#111827",
            AppbarText = "#F9FAFB",
            DrawerBackground = "#0F172A",
            DrawerText = "#F1F5F9",
            DrawerIcon = "#94A3B8",
            TextPrimary = "#F9FAFB",
            TextSecondary = "#94A3B8",
            Divider = "#1F2937",
            ActionDefault = "#94A3B8"
        },
        LayoutProperties = new LayoutProperties()
        {
            DefaultBorderRadius = "10px",
            AppbarHeight = "64px"
        },
        Typography = new Typography()
        {
            Default = new DefaultTypography()
            {
                FontFamily = new[] { "Segoe UI", "-apple-system", "BlinkMacSystemFont", "Roboto", "sans-serif" },
                FontSize = "0.925rem"
            },
            H5 = new H5Typography() { FontWeight = "700", LetterSpacing = "-0.02em" },
            H6 = new H6Typography() { FontWeight = "600", LetterSpacing = "-0.01em" },
            Button = new ButtonTypography() { TextTransform = "none", FontWeight = "600" }
        }
    };
}
