using ErrorOr;

namespace ChallengeIt.Endpoints;

public static class CustomResults
{
    public static int MapToHttpStatusCode(this Error error) => error.Type switch
    {
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status500InternalServerError,
    };

    public static IResult Problem(this List<Error> errors)
    {
        if (errors.All(e => e.Type == ErrorType.Validation))
        {
            var modelStateDictionary = errors
                .GroupBy(x => x.Code)
                .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());

            return Results.ValidationProblem(errors: modelStateDictionary);
        }

        if (errors.Any(e => e.Type == ErrorType.Unexpected))
        {
            return Results.Problem();
        }

        var firstError = errors[0];
        var statusCode = firstError.MapToHttpStatusCode();

        return Results.Problem(statusCode: statusCode, title: firstError.Description);
    }
}