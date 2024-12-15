using ChallengeIt.API.Contracts.Users;
using ChallengeIt.Application.Features.Users.Commands;
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
        
        return group;
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
}