namespace ChallengeIt.Endpoints;

public static class ApplicationEndpoints
{
    public static IEndpointRouteBuilder UseApplicationEndpoints(this IEndpointRouteBuilder builder)
    {
        builder
            .UseUserEndpoints()
            .UseAuthEndpoints();
        
        return builder;
    }
}