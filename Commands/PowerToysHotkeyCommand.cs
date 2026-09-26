using LoupixDeck.Plugin.PowerToys.PowerToys;
using LoupixDeck.PluginSdk;
using System.Reflection;

namespace LoupixDeck.Plugin.PowerToys.Commands;

public sealed class PowerToysHotkeyCommand(PowerToysHotkeyDefinition definition) : IDisplayImageCommand
{
    public PowerToysHotkeyDefinition Definition => definition;
    private const string Clipboard = "Clipboard";
    private const string WindowLayout = "Window & Layout";
    private const string Mouse = "Mouse";
    private const string Tools = "Tools";
    private const string LaunchSearch = "Launch & Search";
    private readonly byte[] _icon = ReadIcon(definition.IconResource);

    public static IReadOnlyList<IPluginCommand> All { get; } =
    [
        Create("PowerToys.AdvancedPaste.Activate", "Advanced Paste", Clipboard, "AdvancedPaste", "\U000F0192", "clipboard", "properties", "advanced-paste-ui-hotkey"),
        Create("PowerToys.AdvancedPaste.PlainText", "Paste as Plain Text", Clipboard, "AdvancedPaste", "\U000F0192", "content-paste", "properties", "paste-as-plain-hotkey"),
        Create("PowerToys.AdvancedPaste.Markdown", "Paste as Markdown", Clipboard, "AdvancedPaste", "\U000F0192", "language-markdown", "properties", "paste-as-markdown-hotkey"),
        Create("PowerToys.AdvancedPaste.Json", "Paste as JSON", Clipboard, "AdvancedPaste", "\U000F0192", "code-json", "properties", "paste-as-json-hotkey"),
        Create("PowerToys.AlwaysOnTop.Toggle", "Always On Top", WindowLayout, "AlwaysOnTop", "\U000F0403", "pin", "properties", "hotkey", "value"),
        Create("PowerToys.AlwaysOnTop.IncreaseOpacity", "Increase Opacity", WindowLayout, "AlwaysOnTop", "\U000F0403", "opacity", "properties", "increase-opacity-hotkey", "value"),
        Create("PowerToys.AlwaysOnTop.DecreaseOpacity", "Decrease Opacity", WindowLayout, "AlwaysOnTop", "\U000F0403", "opacity", "properties", "decrease-opacity-hotkey", "value"),
        Create("PowerToys.FancyZones.Editor", "FancyZones Editor", WindowLayout, "FancyZones", "\U000F0F8D", "view-grid-plus", "properties", "fancyzones_editor_hotkey", "value"),
        Create("PowerToys.CropAndLock.Thumbnail", "Crop and Lock Thumbnail", WindowLayout, "CropAndLock", "\U000F0F8D", "crop", "properties", "thumbnail-hotkey", "value"),
        Create("PowerToys.CropAndLock.Reparent", "Crop and Lock Reparent", WindowLayout, "CropAndLock", "\U000F0F8D", "crop", "properties", "reparent-hotkey", "value"),
        Create("PowerToys.CropAndLock.Screenshot", "Crop and Lock Screenshot", WindowLayout, "CropAndLock", "\U000F0F8D", "crop", "properties", "screenshot-hotkey", "value"),
        Create("PowerToys.Workspaces.Activate", "Workspaces", WindowLayout, "Workspaces", "\U000F0F8D", "view-dashboard", "properties", "hotkey", "value"),
        Create("PowerToys.MouseHighlighter.Toggle", "Mouse Highlighter", Mouse, "MouseHighlighter", "\U000F037D", "mouse", "properties", "activation_shortcut"),
        Create("PowerToys.MouseJump.Activate", "Mouse Jump", Mouse, "MouseJump", "\U000F037D", "cursor-move", "properties", "activation_shortcut"),
        Create("PowerToys.MouseCrosshairs.Toggle", "Mouse Pointer Crosshairs", Mouse, "MousePointerCrosshairs", "\U000F037D", "crosshairs-gps", "properties", "activation_shortcut"),
        Create("PowerToys.CursorWrap.Toggle", "Cursor Wrap", Mouse, "CursorWrap", "\U000F037D", "cursor-move", "properties", "activation_shortcut"),
        Create("PowerToys.ColorPicker.Activate", "Color Picker", Tools, "ColorPicker", "\U000F020A", "eyedropper", "properties", "ActivationShortcut"),
        Create("PowerToys.TextExtractor.Activate", "Text Extractor", Tools, "TextExtractor", "\U000F113D", "text-recognition", "properties", "ActivationShortcut"),
        Create("PowerToys.MeasureTool.Activate", "Screen Ruler", Tools, "Measure Tool", "\U000F046D", "ruler", "properties", "ActivationShortcut"),
        Create("PowerToys.Peek.Activate", "Peek", Tools, "Peek", "\U000F0208", "eye", "properties", "ActivationShortcut"),
        Create("PowerToys.PowerLauncher.Activate", "PowerToys Run", LaunchSearch, "PowerToys Run", "\U000F0349", "magnify", "properties", "open_powerlauncher"),
        Create("PowerToys.ShortcutGuide.Activate", "Shortcut Guide", LaunchSearch, "Shortcut Guide", "\U000F030C", "keyboard", "properties", "open_shortcutguide")
    ];

    public TimeSpan UpdateInterval => TimeSpan.FromHours(1);

    public CommandDescriptor Descriptor { get; } = new()
    {
        CommandName = definition.CommandId,
        DisplayName = definition.DisplayName,
        Group = "PowerToys",
        Icon = definition.Icon,
        HiddenFromMenu = true
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

    public bool RenderImage(CommandContext ctx, IRenderCanvas canvas)
    {
        int size = Math.Min(canvas.Width, canvas.Height) / 2;
        if (size <= 0) return false;

        canvas.Clear(PluginColor.Black);
        int y = canvas.Height / 12;
        canvas.DrawImage(_icon, (canvas.Width - size) / 2, y, size, size);
        int labelY = y + size + 2;
        int labelHeight = canvas.Height - labelY;
        if (labelHeight > 0)
            canvas.DrawText(definition.DisplayName, 1, labelY, canvas.Width - 2, labelHeight,
                PluginColor.White, canvas.Height * 11f / 90,
                TextHAlign.Center, TextVAlign.Middle);
        return true;
    }

    private static byte[] ReadIcon(string name)
    {
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
            $"LoupixDeck.Plugin.PowerToys.Assets.Mdi.{name}.png")
            ?? throw new InvalidOperationException($"Missing embedded MDI icon: {name}.");
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return buffer.ToArray();
    }

    private static IPluginCommand Create(string id, string name, string group, string module, string icon, string iconResource,
        params string[] path) => new PowerToysHotkeyCommand(new(id, name, group, module, path, icon, iconResource));
}
