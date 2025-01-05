using ChallengeIt.Application.Features.Challenges.Commands.CheckInChallengeDay;
using ChallengeIt.Application.Features.Challenges.Commands.CreateChallenge;
using ChallengeIt.Application.Features.Challenges.Commands.UpdateChallenge;
using ChallengeIt.Application.Features.Challenges.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChallengeIt.Endpoints;

public static class ChallengesEndpoints
{
    public static IEndpointRouteBuilder UseChallengeEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/challenges")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("my", GetCurrentUserChallenges).WithSummary("Gets all challenges belonging to the current user.");
        group.MapPost(string.Empty, CreateChallenge).WithSummary("Creates a new challenge.");
        group.MapPut(string.Empty, UpdateChallenge).WithSummary("Updates existing challenge.");
        group.MapPatch("checkin", CheckInChallengeDay).WithSummary("Checkin challenge day.");
        
        return builder;
    }

    private static async Task<IResult> CreateChallenge(
        [FromServices] ISender mediator,
        [FromBody] CreateChallengeCommand request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return result.Match(
            response => Results.Ok(response),
            CustomResults.Problem
        ) ;
    }

    private static async Task<IResult> UpdateChallenge(
        [FromServices] ISender mediator,
        [FromBody] UpdateChallengeCommand request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
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
            return Results.BadRequest("Page number and/or page size must be greater than 0.");
        
        var result = await mediator.Send(new GetUserChallengesListQuery()
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        }, cancellationToken);
        
        return Results.Ok(result.Value);
    }

    private static async Task<IResult> CheckInChallengeDay(
        [FromServices] ISender mediator,
        [FromQuery] Guid? challengeId,
        [FromQuery] Guid? checkInId,
        CancellationToken cancellationToken = default)
    {
        if (!challengeId.HasValue && !checkInId.HasValue)
        {
            return Results.BadRequest("At least one of ChallengeId or CheckInId must be provided.");
        }

        var command = new CheckInChallengeDayCommand(ChallengeId: challengeId, CheckInId: checkInId);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            _ => Results.NoContent(),
            CustomResults.Problem);
    }
}