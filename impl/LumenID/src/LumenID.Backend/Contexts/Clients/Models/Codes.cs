using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LumenID.Backend.Contexts.Clients.Models;

[Table("codes")]
public class Codes {
    [Column("id"), Key]
    public string Id { get; set; } = string.Empty;
    [Column("code"), Required]
    public string Code { get; set; } = string.Empty;
    [Column("user_id"), Required]
    public string UserId { get; set; } = string.Empty;
    [Column("app_id"), Required]
    public string AppId { get; set; } = string.Empty;
    [Column("used")]
    public bool Used { get; set; } = false;
}
