using System.Text.Json;

namespace LoupixDeck.Plugin.PowerToys.PowerToys;

public sealed record PowerToysHotkeyDefinition(
    string CommandId, string DisplayName, string Group, string Module, string[] PropertyPath, string Icon, string IconResource);

public static class PowerToysSettingsReader
{
    public static bool TryRead(PowerToysHotkeyDefinition definition, out PowerToysHotkey? hotkey, string? settingsRoot = null)
    {
        hotkey = null;
        var root = settingsRoot ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Microsoft", "PowerToys");
        var path = Path.Combine(root, definition.Module, "settings.json");
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete);
            using var document = JsonDocument.Parse(stream);
            var current = document.RootElement;
            foreach (var property in definition.PropertyPath)
            {
                if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(property, out current))
                    return false;
            }
            return PowerToysHotkey.TryParse(current, out hotkey);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException)
        {
            return false;
        }
    }
}
