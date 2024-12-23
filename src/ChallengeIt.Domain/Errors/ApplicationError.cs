namespace ChallengeIt.Domain.Errors;

public static class ApplicationErrors
{
    public static ApplicationError UserNotFound = new (1001, "User Not Found");
    public static ApplicationError UserAlreadyExists = new (1002, "User already exists");
    public static ApplicationError EmailAlreadyInUse = new (1003, "Email is already in use");
    public static ApplicationError UsernameAlreadyInUse = new (1004, "Username is already in use");
    public static ApplicationError MissedRequiredClaims = new (1005, "Required claims are missed");
}

public record struct ApplicationError(int Code, string Message)
{
    public static implicit operator string(ApplicationError error) => error.Message;
    public static implicit operator int(ApplicationError error) => error.Code;
}