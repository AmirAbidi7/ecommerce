using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using main.Entity;

namespace main.Entity;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }
    public string Token { get; set; } = default!;

    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; } = default!;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;
}
