using ChallengeIt.Application.Features.Users.Results;
using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Errors;
using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Users.Commands;

public record CreateUserCommand(
    string Username, 
    string Password, 
    string Email, 
    string? FirstName, 
    string? LastName) : IRequest<ErrorOr<CreateUserResult>>;

public class CreateUserCommandHandler(IUsersRepository usersRepository, IPasswordHasher passwordHasher) : IRequestHandler<CreateUserCommand, ErrorOr<CreateUserResult>>
{
    public async Task<ErrorOr<CreateUserResult>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var isEmailBusy =  await usersRepository.IsUsedEmailAsync(request.Email, cancellationToken);
        if (isEmailBusy)
            return Error.Validation(code: ApplicationErrors.EmailAlreadyInUse, description: ApplicationErrors.EmailAlreadyInUse);
        
        var isUsernameBusy = await usersRepository.IsUsedUsernameAsync(request.Username, cancellationToken);
        if (isUsernameBusy)
            return Error.Validation(code: ApplicationErrors.UsernameAlreadyInUse, description: ApplicationErrors.UsernameAlreadyInUse);

        var passwordHash = passwordHasher.Hash(request.Password);
        
        var user = new User(username: request.Username, email: request.Email, passwordHash: passwordHash)
        {
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        
        await usersRepository.AddAsync(user, cancellationToken);

        return new CreateUserResult(user.Id);
    }
}