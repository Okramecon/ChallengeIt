namespace ChallengeIt.Application.Features.Common;

public sealed class CommandResult<TResult>
{
    public bool IsSuccessResult { get; init; }
    
    public TResult? Result { get; init; }
};