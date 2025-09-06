using System.ComponentModel.DataAnnotations;

namespace LumenID.Backend.Contexts.Models;

/// <summary>
/// Infos model representing user profile information
/// </summary>
public class Infos
{
    /// <summary>
    /// Id of Infos (user profile) generated as UUID v4
    /// </summary>
    [Key]
    public string Id { get; set; } = null!;
    /// <summary>
    /// Username of the user (unique)
    /// </summary>
    public string Username { get; set; } = null!;
    /// <summary>
    /// Email of the user (unique)
    /// </summary>
    public string Email { get; set; } = null!;
    /// <summary>
    /// Avatar image Id of the user profile picture
    /// </summary>
    public string AvatarId { get; set; } = string.Empty;
    /// <summary>
    /// Header image Id of the user profile header picture
    /// </summary>
    public string HeaderId { get; set; } = string.Empty;
    /// <summary>
    /// Key color of the user profile (hex color code)
    /// </summary>
    public string KeyColor { get; set; } = string.Empty;
}
