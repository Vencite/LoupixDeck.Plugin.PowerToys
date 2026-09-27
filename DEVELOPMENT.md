# Development

This repository contains the Windows-only PowerToys plugin for LoupixDeck.

## Requirements

- .NET 10 SDK in WSL or Linux for repository work
- LoupixDeck.PluginSdk 1.26.0
- Windows with LoupixDeck and Microsoft PowerToys for runtime checks

The plugin targets `net10.0`.

## Build and smoke check

```bash
dotnet restore
dotnet build -c Release --no-restore -m:1
dotnet run --project tests/HotkeySmoke/HotkeySmoke.csproj -c Release
git diff --check
```

The smoke check covers PowerToys hotkey parsing, key conversion, command/menu descriptors and the plain-action shape (no custom rendering). Runtime behavior requires Windows, LoupixDeck and PowerToys.

LoupixDeck provides `LoupixDeck.PluginSdk.dll` at runtime. Do not package a private copy.

## Local installation

Copy `plugin.json`, `icon.png` and the plugin build output from `bin/Release/` to LoupixDeck's `plugins/powertoys` directory on Windows. Exclude `LoupixDeck.PluginSdk.dll`. Restart LoupixDeck or reload plugins.

## Compatibility

Treat the `powertoys` plugin ID, `LoupixDeck.Plugin.PowerToys.dll` assembly name and `PowerToys.*` command IDs as saved configuration compatibility surface. Do not rename them without a migration plan.

Keep `plugin.json` `version` equal to `PluginMetadata.Version`. Keep the NuGet SDK version and manifest `sdkVersion` aligned; code uses `SdkInfo.Version`.

PowerToys shortcuts are read from the user's current settings when a command runs. Check local `docs/` notes and the current Microsoft PowerToys source before changing a settings file path or JSON property path. The local notes are ignored by Git and may be absent in a fresh checkout.

## Release workflow

The repository uses the reusable release workflow from `RadiatorTwo/LoupixDeck.PluginSdk`. Before release, run the Release build and smoke check, verify the manifest and command IDs, and inspect the workflow against current upstream packaging rules. Confirm that the package excludes `LoupixDeck.PluginSdk.dll`.

A manual `workflow_dispatch` run can validate packaging before publication. Publish a GitHub Release with tag `v<plugin.json version>` only after the package is checked on Windows. The release workflow produces the plugin ZIP, manifest, checksums and a `store-entry.json` artifact for a later Plugin Store update. Use that artifact's actual metadata rather than guessing a URL or checksum.

Publishing a release, pushing its tag or updating the Plugin Store requires an explicit request.

## Sources of truth

For PowerToys settings, use current Microsoft PowerToys source. For SDK usage, manifests and release packaging, use current `RadiatorTwo/LoupixDeck.PluginSdk` and `RadiatorTwo/LoupixDeck`. The Home Assistant plugin is a workflow example, not a source for PowerToys behavior.
