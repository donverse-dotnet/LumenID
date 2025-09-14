using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LumenID.Backend.Contexts.Clients.Models;

[Table("metadata")]
public class Metadata {
    [Column("id"), Key]
    public string Id { get; set; }
    [Column("secret_id"), Required]
    public string SecretId { get; set; } = string.Empty;
    [Column("profile_id"), Required]
    public string ProfileId { get; set; } = string.Empty;
    
    [Column("created_at"), Required]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at"), Required]
    public DateTime UpdatedAt { get; set; }
}
