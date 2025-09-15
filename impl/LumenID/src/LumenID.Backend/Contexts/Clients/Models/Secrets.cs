using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LumenID.Backend.Contexts.Clients.Models;

[Table("secrets")]
public class Secrets {
  /// <summary>
  /// The uuid of OAuth client.
  /// </summary>
  [Key, Column("id")]
  public string Id { get; set; } = string.Empty;

  /// <summary>
  /// The secret key for OAuth client.
  /// <para>
  /// Generate:
  /// uuid -- SHA256 --> secret key
  /// </para>
  /// </summary>
  [Required, Column("secret_key")]
  public string SecretKey { get; set; } = string.Empty;

  /// <summary>
  /// The redirect url for OAuth client.
  /// <para>
  /// This value is the URL of the app that will be navigated to after permission is granted.
  /// </para>
  /// <para>
  /// TODO: Multiple redirect url for multiple purpose.
  /// </para>
  /// </summary>
  [Required, Column("redirect_url")]
  public string RedirectUrl { get; set; } = string.Empty;
}
