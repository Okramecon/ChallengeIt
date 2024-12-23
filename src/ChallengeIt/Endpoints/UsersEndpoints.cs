using System.Security.Claims;
using ChallengeIt.API.Contracts.Users;
using ChallengeIt.Application.Features.Users.Commands;
using ChallengeIt.Application.Features.Users.Queries;
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
            CustomResults.Problem
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
            CustomResults.Problem
        );
    }
}