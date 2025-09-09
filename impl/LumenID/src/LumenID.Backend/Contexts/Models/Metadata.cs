using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LumenID.Backend.Contexts.Models;

/// <summary>
/// Metadata model representing user account metadata
/// </summary>
[Table("metadata")]
public class Metadata {
    /// <summary>
    /// Id of metadata (user id) generated as UUID v4
    /// </summary>
    [Key, Column("id")]
    public string Id { get; set; } = string.Empty;
    /// <summary>
    /// Id of Infos (user profile) associated with this metadata
    /// </summary>
    [Column("info_id")]
    public string InfoId { get; set; } = string.Empty;
    /// <summary>
    /// Id of Configs (user settings) associated with this metadata
    /// </summary>
    [Column("config_id")]
    public string ConfigId { get; set; } = string.Empty;
    /// <summary>
    /// Id of Secrets (user credentials) associated with this metadata
    /// </summary>
    [Column("secret_id")]
    public string SecretId { get; set; } = string.Empty;
    /// <summary>
    /// DateTime when this account was created
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// DateTime when this account was last updated
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    /// <summary>
    /// DateTime when this account was deactivated (soft deleted)
    /// </summary>
    [Column("deactivated_at")]
    public DateTime? DeactivatedAt { get; set; }
}
