using System.ComponentModel.DataAnnotations;

namespace LumenID.Backend.Contexts.Models;

/// <summary>
/// Secrets model representing user credentials
/// </summary>
public class Secrets
{
    [Key]
    public string Id { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
}
