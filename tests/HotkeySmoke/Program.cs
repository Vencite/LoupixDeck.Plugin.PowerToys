using System.Text.Json;
using LoupixDeck.Plugin.PowerToys;
using LoupixDeck.Plugin.PowerToys.Commands;
using LoupixDeck.Plugin.PowerToys.PowerToys;

Check("Win+Ctrl+C", """{"win":true,"ctrl":true,"alt":false,"shift":false,"code":67,"key":""}""");
Check("Win+Shift+T", """{"win":true,"ctrl":false,"alt":false,"shift":true,"code":84,"key":""}""");
Check("Alt+Space", """{"win":false,"ctrl":false,"alt":true,"shift":false,"code":32,"key":""}""");
Check("Win+Ctrl+Alt+Shift+F", """{"win":true,"ctrl":true,"alt":true,"shift":true,"code":70,"key":"F"}""");
Check("Win+Shift+grave", """{"win":true,"ctrl":false,"alt":false,"shift":true,"code":192,"key":""}""");
Check("Win+Shift+slash", """{"win":true,"ctrl":false,"alt":false,"shift":true,"code":191,"key":""}""");
Check("Win+Ctrl+equals", """{"win":true,"ctrl":true,"alt":false,"shift":false,"code":187,"key":""}""");
Check("Win+Ctrl+minus", """{"win":true,"ctrl":true,"alt":false,"shift":false,"code":189,"key":""}""");
if (PowerToysHotkey.TryParse(JsonDocument.Parse("{}").RootElement, out _))
    throw new Exception("An incomplete hotkey must be rejected.");
if (PowerToysHotkey.TryParse(JsonDocument.Parse("""{"win":true,"ctrl":false,"alt":false,"shift":false,"code":0,"key":"X"}""").RootElement, out _))
    throw new Exception("A hotkey with no virtual key code must be rejected.");
if (PowerToysHotkey.TryParse(JsonDocument.Parse("""{"win":true,"ctrl":false,"alt":false,"shift":false,"code":"X","key":"X"}""").RootElement, out _))
    throw new Exception("A hotkey with a nonnumeric code must be rejected.");

var commands = PowerToysHotkeyCommand.All.Cast<PowerToysHotkeyCommand>().ToArray();
var groups = new PowerToysPlugin().GetCommandGroups().Select(group => group.Group).ToHashSet();
var legacyIds = new[]
{
    "PowerToys.AlwaysOnTop.Toggle", "PowerToys.ColorPicker.Activate", "PowerToys.FancyZones.Editor",
    "PowerToys.PowerLauncher.Activate", "PowerToys.ShortcutGuide.Activate", "PowerToys.TextExtractor.Activate",
    "PowerToys.MeasureTool.Activate", "PowerToys.MouseHighlighter.Toggle"
};
if (commands.Length != 22 || commands.Select(command => command.Descriptor.CommandName).Distinct().Count() != 22 ||
    legacyIds.Any(id => commands.All(command => command.Descriptor.CommandName != id)) ||
    commands.Any(command => !groups.Contains(command.Descriptor.Group)))
    throw new Exception("Command count, IDs or group descriptors are invalid.");

foreach (var icon in commands.Select(command => command.Definition.IconResource).Distinct())
{
    var resource = $"LoupixDeck.Plugin.PowerToys.Assets.Mdi.{icon}.png";
    using var stream = typeof(PowerToysHotkey).Assembly.GetManifestResourceStream(resource);
    if (stream is null || stream.Length == 0)
        throw new Exception($"Missing embedded icon: {icon}.");
}

var root = Path.Combine(Path.GetTempPath(), "powertoys-smoke-" + Guid.NewGuid());
Directory.CreateDirectory(root);
try
{
    const string direct = """{"win":true,"ctrl":true,"alt":false,"shift":false,"code":67,"key":""}""";
    foreach (var command in commands)
    {
        var definition = command.Definition;
        var file = Path.Combine(root, definition.Module, "settings.json");
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        if (PowerToysSettingsReader.TryRead(definition, out _, root))
            throw new Exception($"Missing file accepted: {definition.CommandId}");

        File.WriteAllText(file, "{}");
        if (PowerToysSettingsReader.TryRead(definition, out _, root))
            throw new Exception($"Missing property accepted: {definition.CommandId}");

        File.WriteAllText(file, "{");
        if (PowerToysSettingsReader.TryRead(definition, out _, root))
            throw new Exception($"Malformed JSON accepted: {definition.CommandId}");

        File.WriteAllText(file, Nest(definition.PropertyPath, "{}"));
        if (PowerToysSettingsReader.TryRead(definition, out _, root))
            throw new Exception($"Incomplete hotkey accepted: {definition.CommandId}");

        File.WriteAllText(file, Nest(definition.PropertyPath, direct));
        if (!PowerToysSettingsReader.TryRead(definition, out var hotkey, root) ||
            hotkey?.HostCommand != "System.KeyCombination(Win+Ctrl+C)")
            throw new Exception($"Shortcut read failed: {definition.CommandId}");
        File.Delete(file);
    }
}
finally
{
    Directory.Delete(root, recursive: true);
}

static string Nest(string[] path, string value)
{
    for (var i = path.Length - 1; i >= 0; i--)
        value = "{\"" + path[i] + "\":" + value + "}";
    return value;
}

static void Check(string expected, string json)
{
    using var document = JsonDocument.Parse(json);
    if (!PowerToysHotkey.TryParse(document.RootElement, out var hotkey) || hotkey!.KeyCombination != expected)
        throw new Exception($"Expected {expected}, got {hotkey?.KeyCombination ?? "invalid"}.");
}
