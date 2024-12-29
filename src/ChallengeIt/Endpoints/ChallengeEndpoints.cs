using ChallengeIt.Application.Features.Challenges.Commands.CreateChallenge;
using ChallengeIt.Application.Features.Challenges.Commands.UpdateChallenge;
using ChallengeIt.Application.Features.Challenges.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChallengeIt.Endpoints;

public static class ChallengeEndpoints
{
    public static IEndpointRouteBuilder UseChallengeEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/challenges")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("my", GetCurrentUserChallenges).WithSummary("Gets all challenges belonging to the current user.");
        group.MapPost(string.Empty, CreateChallenge).WithSummary("Creates a new challenge.");
        group.MapPut(string.Empty, UpdateChallenge).WithSummary("Updates existing challenge.");
        
        return builder;
    }

    private static async Task<IResult> CreateChallenge(
        [FromServices] ISender mediator,
        [FromBody] CreateChallengeCommand request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return result.Match(
            id => Results.Ok(id),
            CustomResults.Problem
        ) ;
    }

    private static async Task<IResult> UpdateChallenge(
        [FromServices] ISender mediator,
        [FromBody] UpdateChallengeCommand request)
    {
        var result = await mediator.Send(request);
        return result.Match(
            _ => Results.NoContent(),
            CustomResults.Problem);
    }

    private static async Task<IResult> GetCurrentUserChallenges(
        [FromServices] ISender mediator,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 1000,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
            Results.BadRequest("Page number and/or page size must be greater than 0.");
        
        var result = await mediator.Send(new GetUserChallengesListQuery()
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        
        return Results.Ok(result.Value);
    }
}