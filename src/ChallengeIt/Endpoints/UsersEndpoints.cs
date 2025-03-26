using System.Security.Claims;
using ChallengeIt.API.Contracts.Users;
using ChallengeIt.Application.Features.Users.Commands;
using ChallengeIt.Application.Features.Users.Queries.FindUserProfile;
using ChallengeIt.Application.Features.Users.Queries.GetUserInformation;
using ChallengeIt.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChallengeIt.Endpoints;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder UseUserEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/users")
            .WithOpenApi();

        group.MapPost(string.Empty, CreateUser).WithSummary("Creates a new user");
        group.MapGet("information", GetCurrentUserInformation).RequireAuthorization().WithSummary("Returns current user information");
        group.MapGet("find", FindUserProfile).RequireAuthorization().WithSummary("Finds a user by username");
        
        return builder;
    }

    private static async Task<IResult> CreateUser(
        [FromServices] ISender mediator,
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateUserCommand(
            request.Username,
            request.Password,
            request.Email, 
            request.FIrstName,
            request.LastName
        );
        
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            user => Results.Ok(user.UserId),
            errors => errors.Problem()
        ) ;
    }

    private static async Task<IResult> GetCurrentUserInformation(
        [FromServices] ISender mediator,
        [FromServices] ICurrentUserProvider currentUserProvider,
        ClaimsPrincipal user)
    {
        var userId = currentUserProvider.GetUserId();
        var result = await mediator.Send(new GetUserInformationQuery(userId));
        return result.Match(
            userInfo => Results.Ok(userInfo),
            errors => errors.Problem()
        );
    }
    
    private static async Task<IResult> FindUserProfile(
        [FromServices] ISender mediator,
        [FromQuery] string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString) || string.IsNullOrWhiteSpace(searchString))
            return Results.BadRequest("Search string is empty");
        
        var result = await mediator.Send(new FindUserProfileQuery(searchString));
        
        return result.Match(
            profiles => Results.Ok(profiles),
            CustomResults.Problem
        );
    }
}