using LoupixDeck.Plugin.PowerToys.PowerToys;
using LoupixDeck.PluginSdk;
using System.Reflection;

namespace LoupixDeck.Plugin.PowerToys.Commands;

public sealed class PowerToysHotkeyCommand(PowerToysHotkeyDefinition definition) : IDisplayImageCommand
{
    private readonly byte[] _icon = ReadIcon(definition.IconResource);

    public static IReadOnlyList<IPluginCommand> All { get; } =
    [
        Create("PowerToys.AlwaysOnTop.Toggle", "Always On Top", "AlwaysOnTop", "\U000F0403", "pin", "properties", "hotkey", "value"),
        Create("PowerToys.ColorPicker.Activate", "Color Picker", "ColorPicker", "\U000F020A", "eyedropper", "properties", "ActivationShortcut"),
        Create("PowerToys.FancyZones.Editor", "FancyZones Editor", "FancyZones", "\U000F0F8D", "view-grid-plus", "properties", "fancyzones_editor_hotkey", "value"),
        Create("PowerToys.PowerLauncher.Activate", "PowerToys Run", "PowerToys Run", "\U000F0349", "magnify", "properties", "open_powerlauncher"),
        Create("PowerToys.ShortcutGuide.Activate", "Shortcut Guide", "Shortcut Guide", "\U000F030C", "keyboard", "properties", "open_shortcutguide"),
        Create("PowerToys.TextExtractor.Activate", "Text Extractor", "TextExtractor", "\U000F113D", "text-recognition", "properties", "ActivationShortcut"),
        Create("PowerToys.MeasureTool.Activate", "Screen Ruler", "Measure Tool", "\U000F046D", "ruler", "properties", "ActivationShortcut"),
        Create("PowerToys.MouseHighlighter.Toggle", "Mouse Highlighter", "MouseHighlighter", "\U000F037D", "mouse", "properties", "activation_shortcut")
    ];

    public TimeSpan UpdateInterval => TimeSpan.FromHours(1);

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
            var command = $"System.KeyCombination({hotkey!.KeyCombination})";
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

    private static IPluginCommand Create(string id, string name, string module, string icon, string iconResource,
        params string[] path) => new PowerToysHotkeyCommand(new(id, name, module, path, icon, iconResource));
}
