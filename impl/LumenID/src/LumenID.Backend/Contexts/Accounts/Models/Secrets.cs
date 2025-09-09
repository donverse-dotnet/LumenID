using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LumenID.Backend.Contexts.Accounts.Models;

/// <summary>
/// Secrets model representing user credentials
/// </summary>
[Table("secrets")]
public class Secrets {
    [Key, Column("id")]
    public string Id { get; set; } = null!;
    [Column("password")]
    public string Password { get; set; } = null!;
    [Column("secret_key")]
    public string SecretKey { get; set; } = null!;
}
