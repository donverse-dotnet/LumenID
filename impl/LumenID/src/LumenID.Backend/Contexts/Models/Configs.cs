using System.ComponentModel.DataAnnotations;

namespace LumenID.Backend.Contexts.Models;

/// <summary>
/// Configs model representing user settings
/// </summary>
public class Configs
{
    [Key]
    public string Id { get; set; } = null!;
    public NotifyConfig Notify { get; set; } = new NotifyConfig();
    public ThemeConfig Theme { get; set; } = new ThemeConfig();
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
