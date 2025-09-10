using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LumenID.Backend.Contexts.Clients.Models;

[Table("profiles")]
public class Profiles {
    [Column("id"), Key]
    public string Id { get; set; } = string.Empty;
    [Column("name"), Required]
    public string Name { get; set; } = string.Empty;
    [Column("description"), Required]
    public string Description { get; set; } = string.Empty;
    [Column("icon_id"), Required]
    public string IconId { get; set; } = string.Empty;
    [Column("terms"), Required]
    public string Terms { get; set; } = string.Empty;
    [Column("privacy"), Required]
    public string Privacy { get; set; } = string.Empty;
}
