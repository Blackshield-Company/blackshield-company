using System.Text.RegularExpressions;

namespace Modkit.Mutagen;

/// <summary>
/// Minimal .psc reader used to cross-check plugin script attachments against
/// the Papyrus sources: every attached script must exist and every filled
/// property must be declared with a compatible kind (object / int / float / bool / array).
/// </summary>
public sealed class PapyrusIndex
{
    public sealed record Prop(string Type, string Name, bool IsArray, bool HasDefault);

    private readonly Dictionary<string, Dictionary<string, Prop>> _scripts = new(StringComparer.OrdinalIgnoreCase);

    private static readonly Regex ScriptName = new(@"^\s*Scriptname\s+(\w+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
    private static readonly Regex PropertyDecl = new(@"^\s*(\w+)(\[\])?\s+Property\s+(\w+)(?!.*\bAutoReadOnly\b)(\s*=)?", RegexOptions.IgnoreCase | RegexOptions.Multiline);

    public PapyrusIndex(string sourceDir)
    {
        foreach (var file in Directory.EnumerateFiles(sourceDir, "*.psc"))
        {
            var text = File.ReadAllText(file);
            var m = ScriptName.Match(text);
            if (!m.Success) continue;
            var props = new Dictionary<string, Prop>(StringComparer.OrdinalIgnoreCase);
            foreach (Match p in PropertyDecl.Matches(text))
                props[p.Groups[3].Value] = new Prop(p.Groups[1].Value, p.Groups[3].Value, p.Groups[2].Success, p.Groups[4].Success);
            _scripts[m.Groups[1].Value] = props;
        }
    }

    public bool HasScript(string name) => _scripts.ContainsKey(name);

    public Prop? GetProperty(string script, string prop)
        => _scripts.TryGetValue(script, out var props) && props.TryGetValue(prop, out var p) ? p : null;

    public IReadOnlyCollection<string> Scripts => _scripts.Keys;

    public IEnumerable<Prop> Properties(string script)
        => _scripts.TryGetValue(script, out var props) ? props.Values : Enumerable.Empty<Prop>();
}
