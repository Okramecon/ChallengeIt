using ChallengeIt.API.Contracts.Auth;
using ChallengeIt.API.Contracts.Users;
using ChallengeIt.Application.Features.Auth.Commands;
using ChallengeIt.Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChallengeIt.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder UseAuthEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("api/auth")
            .AllowAnonymous()
            .WithOpenApi();

        group.MapPost(string.Empty, LoginWithCredentials).WithSummary("Creates a new user");
        group.MapPost("refresh", LoginWithRefreshToken).WithSummary("Refreshes the user token").RequireAuthorization();
        return builder;
    }

    private static async Task<IResult> LoginWithCredentials(
        [FromServices] ISender mediator,
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new LoginCommand(
            request.Username,
            request.Email, 
            request.Password
        );
        
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            loginResult => Results.Ok(
                new LoginResponse(loginResult.AccessToken, loginResult.RefreshToken)
            ),
            CustomResults.Problem
        ) ;
    }
    
    private static async Task<IResult> LoginWithRefreshToken(
        [FromServices] ISender mediator,
        [FromBody] LoginWithRefreshRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new LoginWIthRefreshTokenCommand(request.RefreshToken);
        
        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            loginResult => Results.Ok(
                new LoginResponse(loginResult.AccessToken, loginResult.RefreshToken)
            ),
            CustomResults.Problem
        ) ;
    }
}