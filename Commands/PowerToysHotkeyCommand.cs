using LoupixDeck.Plugin.PowerToys.PowerToys;
using LoupixDeck.PluginSdk;

namespace LoupixDeck.Plugin.PowerToys.Commands;

public sealed class PowerToysHotkeyCommand(PowerToysHotkeyDefinition definition) : IPluginCommand
{
    public PowerToysHotkeyDefinition Definition => definition;
    private const string Clipboard = "PowerToys - Clipboard";
    private const string WindowLayout = "PowerToys - Window & Layout";
    private const string Mouse = "PowerToys - Mouse";
    private const string Tools = "PowerToys - Tools";
    private const string LaunchSearch = "PowerToys - Launch & Search";

    public static IReadOnlyList<IPluginCommand> All { get; } =
    [
        Create("PowerToys.AdvancedPaste.Activate", "Advanced Paste", Clipboard, "AdvancedPaste", "\U000F0192", "properties", "advanced-paste-ui-hotkey"),
        Create("PowerToys.AdvancedPaste.PlainText", "Paste as Plain Text", Clipboard, "AdvancedPaste", "\U000F0192", "properties", "paste-as-plain-hotkey"),
        Create("PowerToys.AdvancedPaste.Markdown", "Paste as Markdown", Clipboard, "AdvancedPaste", "\U000F0192", "properties", "paste-as-markdown-hotkey"),
        Create("PowerToys.AdvancedPaste.Json", "Paste as JSON", Clipboard, "AdvancedPaste", "\U000F0192", "properties", "paste-as-json-hotkey"),
        Create("PowerToys.AlwaysOnTop.Toggle", "Always On Top", WindowLayout, "AlwaysOnTop", "\U000F0403", "properties", "hotkey", "value"),
        Create("PowerToys.AlwaysOnTop.IncreaseOpacity", "Increase Opacity", WindowLayout, "AlwaysOnTop", "\U000F0403", "properties", "increase-opacity-hotkey", "value"),
        Create("PowerToys.AlwaysOnTop.DecreaseOpacity", "Decrease Opacity", WindowLayout, "AlwaysOnTop", "\U000F0403", "properties", "decrease-opacity-hotkey", "value"),
        Create("PowerToys.FancyZones.Editor", "FancyZones Editor", WindowLayout, "FancyZones", "\U000F0F8D", "properties", "fancyzones_editor_hotkey", "value"),
        Create("PowerToys.CropAndLock.Thumbnail", "Crop and Lock Thumbnail", WindowLayout, "CropAndLock", "\U000F0F8D", "properties", "thumbnail-hotkey", "value"),
        Create("PowerToys.CropAndLock.Reparent", "Crop and Lock Reparent", WindowLayout, "CropAndLock", "\U000F0F8D", "properties", "reparent-hotkey", "value"),
        Create("PowerToys.CropAndLock.Screenshot", "Crop and Lock Screenshot", WindowLayout, "CropAndLock", "\U000F0F8D", "properties", "screenshot-hotkey", "value"),
        Create("PowerToys.Workspaces.Activate", "Workspaces", WindowLayout, "Workspaces", "\U000F0F8D", "properties", "hotkey", "value"),
        Create("PowerToys.MouseHighlighter.Toggle", "Mouse Highlighter", Mouse, "MouseHighlighter", "\U000F037D", "properties", "activation_shortcut"),
        Create("PowerToys.MouseJump.Activate", "Mouse Jump", Mouse, "MouseJump", "\U000F037D", "properties", "activation_shortcut"),
        Create("PowerToys.MouseCrosshairs.Toggle", "Mouse Pointer Crosshairs", Mouse, "MousePointerCrosshairs", "\U000F037D", "properties", "activation_shortcut"),
        Create("PowerToys.CursorWrap.Toggle", "Cursor Wrap", Mouse, "CursorWrap", "\U000F037D", "properties", "activation_shortcut"),
        Create("PowerToys.ColorPicker.Activate", "Color Picker", Tools, "ColorPicker", "\U000F020A", "properties", "ActivationShortcut"),
        Create("PowerToys.TextExtractor.Activate", "Text Extractor", Tools, "TextExtractor", "\U000F113D", "properties", "ActivationShortcut"),
        Create("PowerToys.MeasureTool.Activate", "Screen Ruler", Tools, "Measure Tool", "\U000F046D", "properties", "ActivationShortcut"),
        Create("PowerToys.Peek.Activate", "Peek", Tools, "Peek", "\U000F0208", "properties", "ActivationShortcut"),
        Create("PowerToys.PowerLauncher.Activate", "PowerToys Run", LaunchSearch, "PowerToys Run", "\U000F0349", "properties", "open_powerlauncher"),
        Create("PowerToys.ShortcutGuide.Activate", "Shortcut Guide", LaunchSearch, "Shortcut Guide", "\U000F030C", "properties", "open_shortcutguide")
    ];

    public CommandDescriptor Descriptor { get; } = new()
    {
        CommandName = definition.CommandId,
        DisplayName = definition.DisplayName,
        Group = definition.Group,
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

    private static IPluginCommand Create(string id, string name, string group, string module, string icon,
        params string[] path) => new PowerToysHotkeyCommand(new(id, name, group, module, path, icon));
}
