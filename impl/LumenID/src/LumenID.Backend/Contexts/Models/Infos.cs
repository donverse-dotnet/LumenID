using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LumenID.Backend.Contexts.Models;

/// <summary>
/// Infos model representing user profile information
/// </summary>
[Table("infos")]
public class Infos {
    /// <summary>
    /// Id of Infos (user profile) generated as UUID v4
    /// </summary>
    [Key, Column("id")]
    public string Id { get; set; } = null!;
    /// <summary>
    /// Username of the user (unique)
    /// </summary>
    [Required, Column("username")]
    public string Username { get; set; } = null!;
    /// <summary>
    /// Email of the user (unique)
    /// </summary>
    [Required, Column("email")]
    public string Email { get; set; } = null!;
    /// <summary>
    /// Avatar image Id of the user profile picture
    /// </summary>
    [Column("avatar_id")]
    public string AvatarId { get; set; } = string.Empty;
    /// <summary>
    /// Header image Id of the user profile header picture
    /// </summary>
    [Column("header_id")]
    public string HeaderId { get; set; } = string.Empty;
    /// <summary>
    /// Key color of the user profile (hex color code)
    /// </summary>
    [Column("key_color")]
    public string KeyColor { get; set; } = string.Empty;
}
