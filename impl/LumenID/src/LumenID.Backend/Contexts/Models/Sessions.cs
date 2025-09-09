using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LumenID.Backend.Contexts.Models;

/// <summary>
/// Sessions model representing user sessions
/// </summary>
[Table("sessions")]
public class Sessions {
    [Key, Column("id")]
    public string Id { get; set; } = null!;
    [Column("meta_id")]
    public string MetaId { get; set; } = null!;
    [Column("token")]
    public string Token { get; set; } = null!;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }
}
