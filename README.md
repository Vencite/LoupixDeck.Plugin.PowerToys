# PowerToys for LoupixDeck

Control Microsoft PowerToys from [LoupixDeck](https://github.com/RadiatorTwo/LoupixDeck) using the shortcuts configured in PowerToys. Version 0.2.0 provides 22 actions.

## Actions

### PowerToys - Clipboard

- Advanced Paste
- Paste as Plain Text
- Paste as Markdown
- Paste as JSON

### PowerToys - Window & Layout

- Always On Top
- Increase Opacity
- Decrease Opacity
- FancyZones Editor
- Crop and Lock Thumbnail
- Crop and Lock Reparent
- Crop and Lock Screenshot
- Workspaces

### PowerToys - Mouse

- Mouse Highlighter
- Mouse Jump
- Mouse Pointer Crosshairs
- Cursor Wrap

### PowerToys - Tools

- Color Picker
- Text Extractor
- Screen Ruler
- Peek

### PowerToys - Launch & Search

- PowerToys Run
- Shortcut Guide

## Requirements

- Windows with LoupixDeck and Microsoft PowerToys installed.
- A LoupixDeck Plugin SDK compatible with version 1.26.0.

## Installation

No public release is available yet. For a local installation, build the plugin and follow [DEVELOPMENT.md](DEVELOPMENT.md).

## Use

Add a PowerToys action from the LoupixDeck command picker to a button. The plugin reads its current shortcut from PowerToys settings each time the button is pressed and sends it through LoupixDeck. You do not need to copy shortcuts into the plugin. Changes made in PowerToys take effect without restarting LoupixDeck. A missing or invalid shortcut is reported in the plugin log.

## Development

Build, validation and release instructions are in [DEVELOPMENT.md](DEVELOPMENT.md).

## License

Plugin code is available under the MIT License. See [LICENSE](LICENSE). MDI icon attribution is in [Assets/Mdi/NOTICE.md](Assets/Mdi/NOTICE.md).
