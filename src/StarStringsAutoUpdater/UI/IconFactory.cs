using System.Drawing;
using System.Reflection;

namespace StarStringsAutoUpdater.UI;

/// <summary>Loads the app's icon from embedded multi-resolution .ico resources.</summary>
public static class IconFactory
{
    private const string ResourcePrefix = "StarStringsAutoUpdater.Assets.";

    private static readonly Icon RunningIcon = LoadEmbeddedIcon("AppIcon.ico");
    private static readonly Icon PausedIcon = LoadEmbeddedIcon("AppIconPaused.ico");

    public static Icon CreateTrayIcon(bool paused = false, int size = 32)
    {
        var source = paused ? PausedIcon : RunningIcon;
        return new Icon(source, size, size);
    }

    private static Icon LoadEmbeddedIcon(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = ResourcePrefix + fileName;
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
        return new Icon(stream);
    }
}
