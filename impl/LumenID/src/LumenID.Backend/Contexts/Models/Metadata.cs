using System.ComponentModel.DataAnnotations;

namespace LumenID.Backend.Contexts.Models;

/// <summary>
/// Metadata model representing user account metadata
/// </summary>
public class Metadata
{
    /// <summary>
    /// Id of metadata (user id) generated as UUID v4
    /// </summary>
    [Key]
    public string Id { get; set; } = string.Empty;
    /// <summary>
    /// Id of Infos (user profile) associated with this metadata
    /// </summary>
    public string InfoId { get; set; } = string.Empty;
    /// <summary>
    /// Id of Configs (user settings) associated with this metadata
    /// </summary>
    public string ConfigId { get; set; } = string.Empty;
    /// <summary>
    /// Id of Secrets (user credentials) associated with this metadata
    /// </summary>
    public string SecretId { get; set; } = string.Empty;
    /// <summary>
    /// DateTime when this account was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// DateTime when this account was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    /// <summary>
    /// DateTime when this account was deactivated (soft deleted)
    /// </summary>
    public DateTime? DeactivatedAt { get; set; }
}
