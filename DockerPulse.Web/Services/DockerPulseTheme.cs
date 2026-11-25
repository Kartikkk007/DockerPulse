using MudBlazor;

namespace DockerPulse.Web.Services;

public static class DockerPulseTheme
{
    public static MudTheme DarkTheme => new MudTheme()
    {
        PaletteDark = new PaletteDark()
        {
            Primary = "#FF8C42",
            Secondary = "#FFB36B",

            Background = "#0B1120",
            Surface = "#151F32",

            AppbarBackground = "#111827",
            DrawerBackground = "#111827",

            TextPrimary = "#F8FAFC",
            TextSecondary = "#94A3B8",

            ActionDefault = "#94A3B8",
            Divider = "#263449",

            Success = "#22C55E",
            Warning = "#F59E0B",
            Error = "#EF4444",
            Info = "#38BDF8"
        }
    };
}