namespace ChallengeIt.Application.Features.Common;

public class PageQuery
{
    public int PageNumber { get; init; } 
    public int PageSize { get; init; } 
    public string? OrderB { get; init; }
    public bool Ascending { get; init; }
    public string? Filter { get; init; }
}