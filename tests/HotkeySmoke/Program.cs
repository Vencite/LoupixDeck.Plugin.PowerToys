using System.Globalization;
using System.Text.Json;
using LoupixDeck.Plugin.PowerToys;
using LoupixDeck.Plugin.PowerToys.Commands;
using LoupixDeck.Plugin.PowerToys.PowerToys;
using LoupixDeck.PluginSdk;

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
var plugin = new PowerToysPlugin();
var groups = plugin.GetCommandGroups();
var legacyIds = new[]
{
    "PowerToys.AlwaysOnTop.Toggle", "PowerToys.ColorPicker.Activate", "PowerToys.FancyZones.Editor",
    "PowerToys.PowerLauncher.Activate", "PowerToys.ShortcutGuide.Activate", "PowerToys.TextExtractor.Activate",
    "PowerToys.MeasureTool.Activate", "PowerToys.MouseHighlighter.Toggle"
};
if (commands.Length != 22 || commands.Select(command => command.Descriptor.CommandName).Distinct().Count() != 22 ||
    legacyIds.Any(id => commands.All(command => command.Descriptor.CommandName != id)) ||
    groups.Count != 1 || groups[0].Group != "PowerToys" ||
    commands.Any(command => command.Descriptor.Group != "PowerToys" || command.Descriptor.HiddenFromMenu))
    throw new Exception("Command count, IDs or group descriptors are invalid.");

if (((object)plugin).GetType().GetInterfaces().Any(type => type == typeof(IMenuContributor)))
    throw new Exception("PowerToys must not use a dynamic menu; plain leaves carry Descriptor.Icon.");

foreach (var icon in commands.Select(command => command.Descriptor.Icon).Distinct())
{
    if (string.IsNullOrEmpty(icon) || StringInfo.GetTextElementEnumerator(icon).MoveNext() is false)
        throw new Exception("Missing command icon.");
}
if (commands.Cast<IPluginCommand>().Any(command => command is IDisplayImageCommand))
    throw new Exception("PowerToys commands must not implement IDisplayImageCommand.");
if (commands.Any(command =>
        command is not PowerToysHotkeyCommand ||
        (command.SupportedTargets & ButtonTargets.TouchButton) == 0))
    throw new Exception("PowerToys commands must be plain actions supporting touch buttons.");

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
