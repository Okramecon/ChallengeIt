using System.ComponentModel.DataAnnotations.Schema;

namespace ChallengeIt.Domain.Entities;

public abstract class Entity<TKey> where TKey : struct
{
    [Column("id")]
    public TKey Id { get; init; }
}