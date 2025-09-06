using System.ComponentModel.DataAnnotations;

namespace LumenID.Backend.Contexts.Models;

/// <summary>
/// Sessions model representing user sessions
/// </summary>
public class Sessions
{
    [Key]
    public string Id { get; set; } = null!;
    public string MetaId { get; set; } = null!;
    public string Token { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
