using LoupixDeck.Plugin.PowerToys.PowerToys;
using LoupixDeck.PluginSdk;

namespace LoupixDeck.Plugin.PowerToys.Commands;

public sealed class PowerToysHotkeyCommand(PowerToysHotkeyDefinition definition) : IPluginCommand
{
    public static IReadOnlyList<IPluginCommand> All { get; } =
    [
        Create("PowerToys.AlwaysOnTop.Toggle", "Always On Top", "AlwaysOnTop", "properties", "hotkey", "value"),
        Create("PowerToys.ColorPicker.Activate", "Color Picker", "ColorPicker", "properties", "ActivationShortcut"),
        Create("PowerToys.FancyZones.Editor", "FancyZones Editor", "FancyZones", "properties", "fancyzones_editor_hotkey", "value"),
        Create("PowerToys.PowerLauncher.Activate", "PowerToys Run", "PowerToys Run", "properties", "open_powerlauncher"),
        Create("PowerToys.ShortcutGuide.Activate", "Shortcut Guide", "Shortcut Guide", "properties", "open_shortcutguide"),
        Create("PowerToys.TextExtractor.Activate", "Text Extractor", "TextExtractor", "properties", "ActivationShortcut"),
        Create("PowerToys.MeasureTool.Activate", "Screen Ruler", "Measure Tool", "properties", "ActivationShortcut"),
        Create("PowerToys.MouseHighlighter.Toggle", "Mouse Highlighter", "MouseHighlighter", "properties", "activation_shortcut")
    ];

    public CommandDescriptor Descriptor { get; } = new()
    {
        CommandName = definition.CommandId,
        DisplayName = definition.DisplayName,
        Group = "PowerToys"
    };

    public ButtonTargets SupportedTargets => ButtonTargets.TouchButton | ButtonTargets.SimpleButton;

    public Task Execute(CommandContext ctx)
    {
        if (!PowerToysSettingsReader.TryRead(definition, out var hotkey))
        {
            ctx.Host.Logger.Warn($"PowerToys {definition.DisplayName} shortcut is missing or invalid.");
            return Task.CompletedTask;
        }

        try
        {
            ctx.Host.ExecuteCommand($"System.KeyCombination({hotkey!.KeyCombination})");
        }
        catch (Exception exception)
        {
            ctx.Host.Logger.Error($"Failed to execute PowerToys {definition.DisplayName} shortcut.", exception);
        }
        return Task.CompletedTask;
    }

    private static IPluginCommand Create(string id, string name, string module, params string[] path) =>
        new PowerToysHotkeyCommand(new(id, name, module, path));
}
