using LoupixDeck.Plugin.PowerToys.PowerToys;
using LoupixDeck.PluginSdk;

namespace LoupixDeck.Plugin.PowerToys.Commands;

public sealed class PowerToysHotkeyCommand(PowerToysHotkeyDefinition definition) : IPluginCommand
{
    public PowerToysHotkeyDefinition Definition => definition;

    public static IReadOnlyList<IPluginCommand> All { get; } =
    [
        Create("PowerToys.AdvancedPaste.Activate", "Advanced Paste", "AdvancedPaste", "\U000F0192", "properties", "advanced-paste-ui-hotkey"),
        Create("PowerToys.AdvancedPaste.PlainText", "Paste as Plain Text", "AdvancedPaste", "\U000F0192", "properties", "paste-as-plain-hotkey"),
        Create("PowerToys.AdvancedPaste.Markdown", "Paste as Markdown", "AdvancedPaste", "\U000F0192", "properties", "paste-as-markdown-hotkey"),
        Create("PowerToys.AdvancedPaste.Json", "Paste as JSON", "AdvancedPaste", "\U000F0192", "properties", "paste-as-json-hotkey"),
        Create("PowerToys.AlwaysOnTop.Toggle", "Always On Top", "AlwaysOnTop", "\U000F033E", "properties", "hotkey", "value"),
        Create("PowerToys.AlwaysOnTop.IncreaseOpacity", "Increase Opacity", "AlwaysOnTop", "\U000F00DF", "properties", "increase-opacity-hotkey", "value"),
        Create("PowerToys.AlwaysOnTop.DecreaseOpacity", "Decrease Opacity", "AlwaysOnTop", "\U000F00DF", "properties", "decrease-opacity-hotkey", "value"),
        Create("PowerToys.FancyZones.Editor", "FancyZones Editor", "FancyZones", "\U000F07C0", "properties", "fancyzones_editor_hotkey", "value"),
        Create("PowerToys.CropAndLock.Thumbnail", "Crop and Lock Thumbnail", "CropAndLock", "\U000F0E51", "properties", "thumbnail-hotkey", "value"),
        Create("PowerToys.CropAndLock.Reparent", "Crop and Lock Reparent", "CropAndLock", "\U000F0E51", "properties", "reparent-hotkey", "value"),
        Create("PowerToys.CropAndLock.Screenshot", "Crop and Lock Screenshot", "CropAndLock", "\U000F0E51", "properties", "screenshot-hotkey", "value"),
        Create("PowerToys.Workspaces.Activate", "Workspaces", "Workspaces", "\U000F0379", "properties", "hotkey", "value"),
        Create("PowerToys.MouseHighlighter.Toggle", "Mouse Highlighter", "MouseHighlighter", "\U000F037D", "properties", "activation_shortcut"),
        Create("PowerToys.MouseJump.Activate", "Mouse Jump", "MouseJump", "\U000F037D", "properties", "activation_shortcut"),
        Create("PowerToys.MouseCrosshairs.Toggle", "Mouse Pointer Crosshairs", "MousePointerCrosshairs", "\U000F037D", "properties", "activation_shortcut"),
        Create("PowerToys.CursorWrap.Toggle", "Cursor Wrap", "CursorWrap", "\U000F037D", "properties", "activation_shortcut"),
        Create("PowerToys.ColorPicker.Activate", "Color Picker", "ColorPicker", "\U000F03D8", "properties", "ActivationShortcut"),
        Create("PowerToys.TextExtractor.Activate", "Text Extractor", "TextExtractor", "\U000F018F", "properties", "ActivationShortcut"),
        Create("PowerToys.MeasureTool.Activate", "Screen Ruler", "Measure Tool", "\U000F03EB", "properties", "ActivationShortcut"),
        Create("PowerToys.Peek.Activate", "Peek", "Peek", "\U000F0208", "properties", "ActivationShortcut"),
        Create("PowerToys.PowerLauncher.Activate", "PowerToys Run", "PowerToys Run", "\U000F0349", "properties", "open_powerlauncher"),
        Create("PowerToys.ShortcutGuide.Activate", "Shortcut Guide", "Shortcut Guide", "\U000F030C", "properties", "open_shortcutguide")
    ];

    public CommandDescriptor Descriptor { get; } = new()
    {
        CommandName = definition.CommandId,
        DisplayName = definition.DisplayName,
        Group = "PowerToys",
        Icon = definition.Icon
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
            var command = hotkey!.HostCommand;
            ctx.Host.Logger.Info($"PowerToys {definition.DisplayName}: executing {command}.");
            ctx.Host.ExecuteCommand(command);
        }
        catch (Exception exception)
        {
            ctx.Host.Logger.Error($"Failed to execute PowerToys {definition.DisplayName} shortcut.", exception);
        }
        return Task.CompletedTask;
    }

    private static IPluginCommand Create(string id, string name, string module, string icon,
        params string[] path) => new PowerToysHotkeyCommand(new(id, name, module, path, icon));
}
