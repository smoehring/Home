namespace Smoehring.Home.Domain.Common;

public abstract class Entity
{
    public int Id { get; set; }
}

public abstract class AuditableEntity : Entity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public bool IsDeleted => DeletedAt is not null;
}

public sealed class DomainException(string message) : Exception(message);
