namespace ChallengeIt.Domain.Models.Paging;

public record Page<TEntity>(int PageNumber, int PageSize, int TotalCount, TEntity[] Items);