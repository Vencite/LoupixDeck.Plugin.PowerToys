using System.Reflection;
using LoupixDeck.Plugin.PowerToys.Commands;
using LoupixDeck.PluginSdk;

namespace LoupixDeck.Plugin.PowerToys;

public sealed class PowerToysPlugin : LoupixPlugin, IMenuContributor
{
    public override PluginMetadata Metadata { get; } = new()
    {
        Id = "powertoys",
        Name = "PowerToys",
        Version = new Version(0, 2, 1),
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

    public Task<IReadOnlyList<MenuNode>> GetMenuNodes(ButtonTargets target)
    {
        if ((target & (ButtonTargets.TouchButton | ButtonTargets.SimpleButton)) == 0)
            return Task.FromResult<IReadOnlyList<MenuNode>>([]);

        var folders = PowerToysHotkeyCommand.All.Cast<PowerToysHotkeyCommand>()
            .GroupBy(command => command.Definition.Group)
            .Select(group => new MenuNode
            {
                Name = group.Key,
                Children = group.Select(command => new MenuNode
                {
                    Name = command.Descriptor.DisplayName,
                    CommandName = command.Descriptor.CommandName
                }).ToArray()
            }).ToArray();

        return Task.FromResult<IReadOnlyList<MenuNode>>(
            [new MenuNode { Name = "PowerToys", Children = folders }]);
    }

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
