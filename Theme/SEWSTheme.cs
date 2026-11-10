using MudBlazor;

namespace HT_Web.Theme;

public static class SEWSTheme
{
    public static MudTheme Theme = new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = "#003366",
            Secondary = "#1976D2",
            Success = "#2E7D32",
            Error = "#D32F2F",
            Background = "#F5F7FA",
            Surface = "#FFFFFF"
        }
    };
}