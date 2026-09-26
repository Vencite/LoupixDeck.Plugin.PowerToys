using System.Text.Json;

namespace LoupixDeck.Plugin.PowerToys.PowerToys;

public sealed record PowerToysHotkey(string KeyCombination)
{
    public string HostCommand => $"System.KeyCombination({KeyCombination})";

    public static bool TryParse(JsonElement value, out PowerToysHotkey? hotkey)
    {
        hotkey = null;
        if (value.ValueKind != JsonValueKind.Object ||
            !TryBool(value, "win", out var win) || !TryBool(value, "ctrl", out var ctrl) ||
            !TryBool(value, "alt", out var alt) || !TryBool(value, "shift", out var shift) ||
            !value.TryGetProperty("code", out var codeValue) || codeValue.ValueKind != JsonValueKind.Number ||
            !codeValue.TryGetInt32(out var code) || code == 0 ||
            !value.TryGetProperty("key", out var keyValue) || keyValue.ValueKind != JsonValueKind.String)
            return false;

        var key = keyValue.GetString();
        if (string.IsNullOrWhiteSpace(key))
            key = KeyFromCode(code);
        if (key is null || !IsHostKeyName(key) || !(win || ctrl || alt || shift))
            return false;

        var parts = new List<string>(5);
        if (win) parts.Add("Win");
        if (ctrl) parts.Add("Ctrl");
        if (alt) parts.Add("Alt");
        if (shift) parts.Add("Shift");
        parts.Add(key);
        hotkey = new PowerToysHotkey(string.Join('+', parts));
        return true;
    }

    private static bool TryBool(JsonElement value, string name, out bool result)
    {
        result = false;
        if (!value.TryGetProperty(name, out var property) ||
            property.ValueKind is not (JsonValueKind.True or JsonValueKind.False))
            return false;
        result = property.GetBoolean();
        return true;
    }

    private static string? KeyFromCode(int code) => code switch
    {
        >= 0x30 and <= 0x39 => ((char)code).ToString(),
        >= 0x41 and <= 0x5A => ((char)code).ToString(),
        0x08 => "Backspace", 0x09 => "Tab", 0x0D => "Enter", 0x1B => "Esc", 0x20 => "Space",
        0x21 => "PageUp", 0x22 => "PageDown", 0x23 => "End", 0x24 => "Home",
        0x25 => "Left", 0x26 => "Up", 0x27 => "Right", 0x28 => "Down",
        0x2D => "Ins", 0x2E => "Del",
        0xBB => "equals", 0xBD => "minus", 0xBF => "slash", 0xC0 => "grave",
        >= 0x70 and <= 0x7B => $"F{code - 0x6F}",
        _ => null
    };

    private static bool IsHostKeyName(string key) =>
        key.Length == 1 && char.IsAsciiLetterOrDigit(key[0]) ||
        key is "Space" or "Enter" or "Tab" or "Esc" or "Backspace" or "PageUp" or "PageDown" or
            "End" or "Home" or "Left" or "Up" or "Right" or "Down" or "Ins" or "Del" or
            "equals" or "minus" or "slash" or "grave" ||
        key.Length is 2 or 3 && key[0] == 'F' && int.TryParse(key.AsSpan(1), out var f) && f is >= 1 and <= 12;
}
