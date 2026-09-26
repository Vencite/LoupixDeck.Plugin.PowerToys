# PowerToys for LoupixDeck

Control Microsoft PowerToys with shortcuts configured in PowerToys. This early Windows plugin requires Windows, Microsoft PowerToys and LoupixDeck.

v0.1 provides these actions:

- Always On Top
- Color Picker
- FancyZones Editor
- PowerToys Run
- Shortcut Guide
- Text Extractor
- Screen Ruler
- Mouse Highlighter

The plugin reads each current shortcut from PowerToys settings when its action runs, then sends it through LoupixDeck's keyboard command pipeline.

## Build and install

Build with `dotnet build -c Release`. Copy `plugin.json`, `icon.png` and the output files from `bin/Release/` to LoupixDeck's `plugins/powertoys` directory, then restart or reload plugins. Do not copy `LoupixDeck.PluginSdk.dll`; the host supplies it.

This is an early version and has no published release yet.
