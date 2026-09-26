using System.Reflection;
using LoupixDeck.Plugin.PowerToys.Commands;
using LoupixDeck.PluginSdk;

namespace LoupixDeck.Plugin.PowerToys;

public sealed class PowerToysPlugin : LoupixPlugin
{
    public override PluginMetadata Metadata { get; } = new()
    {
        Id = "powertoys",
        Name = "PowerToys",
        Version = new Version(0, 2, 0),
        SdkVersion = SdkInfo.Version,
        Author = "Vencite",
        Description = "Control Microsoft PowerToys directly from LoupixDeck.",
        Icon = LoadEmbeddedIcon()
    };

    public override IEnumerable<IPluginCommand> GetCommands() => PowerToysHotkeyCommand.All;

    public override IReadOnlyList<CommandGroupDescriptor> GetCommandGroups() =>
    [
        new() { Group = "PowerToys - Clipboard", Description = "Paste and transform clipboard content", Icon = "\U000F0192" },
        new() { Group = "PowerToys - Window & Layout", Description = "Arrange and capture windows", Icon = "\U000F0F8D" },
        new() { Group = "PowerToys - Mouse", Description = "Mouse and pointer tools", Icon = "\U000F037D" },
        new() { Group = "PowerToys - Tools", Description = "Screen and text tools", Icon = "\U000F020A" },
        new() { Group = "PowerToys - Launch & Search", Description = "Launch utilities and find commands", Icon = "\U000F0349" }
    ];

    private static byte[] LoadEmbeddedIcon()
    {
        using Stream stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("LoupixDeck.Plugin.PowerToys.icon.png")
            ?? throw new InvalidOperationException("Embedded plugin icon was not found.");
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }
}
