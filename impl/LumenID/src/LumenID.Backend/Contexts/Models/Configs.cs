using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace LumenID.Backend.Contexts.Models;

/// <summary>
/// Configs model representing user settings
/// </summary>
[Table("configs")]
public class Configs
{
    [Key, Column("id")]
    public string Id { get; set; } = null!;

    [Column(name: "notify", TypeName = "json")]
    public string Notify { get; set; } = string.Empty;
    [Column(name: "theme", TypeName = "json")]
    public string Theme { get; set; } = string.Empty;

    public NotifyConfig GetNotifyConfig()
    {
        var deserialized = JsonSerializer.Deserialize<NotifyConfig>(Notify);
        return deserialized ?? new NotifyConfig();
    }
    public ThemeConfig GetThemeConfig()
    {
        var deserialized = JsonSerializer.Deserialize<ThemeConfig>(Theme);
        return deserialized ?? new ThemeConfig();
    }

    public void SetNotifyConfig(NotifyConfig config)
    {
        Notify = JsonSerializer.Serialize(config);
    }
    public void SetThemeConfig(ThemeConfig config)
    {
        Theme = JsonSerializer.Serialize(config);
    }
}

public class NotifyConfig
{
    public bool Email { get; set; } = true;
    public bool Push { get; set; } = true;
}

public class ThemeConfig
{
    public string Mode { get; set; } = "light"; // light, dark, system
}
