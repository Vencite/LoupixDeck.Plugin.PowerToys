# PowerToys for LoupixDeck

Control Microsoft PowerToys from [LoupixDeck](https://github.com/RadiatorTwo/LoupixDeck) using the shortcuts configured in PowerToys.

## Actions

- Always On Top
- Color Picker
- FancyZones Editor
- PowerToys Run
- Shortcut Guide
- Text Extractor
- Screen Ruler
- Mouse Highlighter

## Requirements

- Windows with LoupixDeck and Microsoft PowerToys installed.
- A LoupixDeck Plugin SDK compatible with version 1.26.0.

## Installation

No public release is available yet. For a local installation, build the plugin and follow [DEVELOPMENT.md](DEVELOPMENT.md).

## Use

Add a PowerToys action from the LoupixDeck command picker to a button. The plugin reads that action's current shortcut from PowerToys settings each time the button is pressed and sends it through LoupixDeck. Configure shortcuts in PowerToys first. A missing or invalid shortcut is reported in the plugin log.

## Development

Build, validation and release instructions are in [DEVELOPMENT.md](DEVELOPMENT.md).

## License

Plugin code is available under the MIT License. See [LICENSE](LICENSE). MDI icon attribution is in [Assets/Mdi/NOTICE.md](Assets/Mdi/NOTICE.md).
