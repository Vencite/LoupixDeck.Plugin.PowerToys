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
        new() { Group = "PowerToys", Description = "Microsoft PowerToys shortcuts", Icon = "\U000F05A9" }
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
