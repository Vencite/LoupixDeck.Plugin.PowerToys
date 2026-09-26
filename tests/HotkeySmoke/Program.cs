using System.Text.Json;
using LoupixDeck.Plugin.PowerToys.PowerToys;

Check("Win+Ctrl+C", """{"win":true,"ctrl":true,"alt":false,"shift":false,"code":67,"key":""}""");
Check("Win+Shift+T", """{"win":true,"ctrl":false,"alt":false,"shift":true,"code":84,"key":""}""");
Check("Alt+Space", """{"win":false,"ctrl":false,"alt":true,"shift":false,"code":32,"key":""}""");
Check("Win+Ctrl+Alt+Shift+F", """{"win":true,"ctrl":true,"alt":true,"shift":true,"code":70,"key":"F"}""");
if (PowerToysHotkey.TryParse(JsonDocument.Parse("{}").RootElement, out _))
    throw new Exception("An incomplete hotkey must be rejected.");

static void Check(string expected, string json)
{
    using var document = JsonDocument.Parse(json);
    if (!PowerToysHotkey.TryParse(document.RootElement, out var hotkey) || hotkey!.KeyCombination != expected)
        throw new Exception($"Expected {expected}, got {hotkey?.KeyCombination ?? "invalid"}.");
}
