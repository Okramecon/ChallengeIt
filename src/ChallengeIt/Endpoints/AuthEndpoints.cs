using ChallengeIt.API.Contracts.Auth;
using ChallengeIt.Application.Features.Auth.Commands;
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
        group.MapPost("google", SignInWithGoogle).WithSummary("Sign in with Google");
        return builder;
    }

    private static async Task<IResult> LoginWithCredentials(
        [FromServices] ISender mediator,
        [FromBody] SignInRequest.Login request,
        CancellationToken cancellationToken = default)
    {
        var command = new LoginCommand(
            request.UsernameOrEmail,
            request.Password
        );

        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            loginResult => Results.Ok(
                new LoginResponse(loginResult.AccessToken, loginResult.RefreshToken)
            ),
            errors => errors.Problem()
        );
    }

    private static async Task<IResult> LoginWithRefreshToken(
        [FromServices] ISender mediator,
        [FromBody] SignInRequest.Refresh request,
        CancellationToken cancellationToken = default)
    {
        var command = new LoginWIthRefreshTokenCommand(request.RefreshToken);

        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            loginResult => Results.Ok(
                new LoginResponse(loginResult.AccessToken, loginResult.RefreshToken)
            ),
            errors => errors.Problem()
        );
    }

    private static async Task<IResult> SignInWithGoogle(
        [FromServices] ISender mediator,
        [FromBody] SignInRequest.Google request,
        CancellationToken cancellationToken = default)
    {
        var command = new SignInWithGoogleCommand(request.IdToken);

        var result = await mediator.Send(command, cancellationToken);
        return result.Match(
            principal => Results.Ok(),
            errors => errors.Problem()
        );
    }
}