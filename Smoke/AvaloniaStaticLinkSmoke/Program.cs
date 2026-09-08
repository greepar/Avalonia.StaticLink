using Avalonia;

namespace AvaloniaStaticLinkSmoke;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
    {
        var builder = AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();

        if (OperatingSystem.IsWindows())
        {
            builder = builder.With(new Win32PlatformOptions
            {
                RenderingMode =
                [
                    Win32RenderingMode.AngleEgl,
                    Win32RenderingMode.Software
                ]
            });
        }

        if (OperatingSystem.IsLinux())
        {
            builder = builder.With(new X11PlatformOptions
            {
                RenderingMode =
                [
                    X11RenderingMode.Glx,
                    X11RenderingMode.Software
                ]
            });
        }

        if (OperatingSystem.IsMacOS())
        {
            var renderer = Environment.GetEnvironmentVariable("STATICLINK_MAC_RENDERER");
            if (!string.Equals(renderer, "default", StringComparison.OrdinalIgnoreCase))
            {
                builder = builder.With(new AvaloniaNativePlatformOptions
                {
                    RenderingMode = string.Equals(renderer, "metal", StringComparison.OrdinalIgnoreCase)
                        ? [AvaloniaNativeRenderingMode.Metal]
                        : [AvaloniaNativeRenderingMode.OpenGl, AvaloniaNativeRenderingMode.Software]
                });
            }
        }

        return builder;
    }
}
