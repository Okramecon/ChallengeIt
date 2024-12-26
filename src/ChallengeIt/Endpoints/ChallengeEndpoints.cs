using ChallengeIt.Application.Features.Challenges.Commands;
using ChallengeIt.Application.Features.Challenges.Commands.CreateChallenge;
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
}