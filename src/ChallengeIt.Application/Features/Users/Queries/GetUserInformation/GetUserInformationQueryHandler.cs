using ChallengeIt.API.Contracts.Users;
using ChallengeIt.Application.Persistence;
using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Users.Queries.GetUserInformation;

public class GetUserInformationQueryHandler(IUsersRepository usersRepository) : IRequestHandler<GetUserInformationQuery, ErrorOr<GetUserInfoResponse>>
{
    public async Task<ErrorOr<GetUserInfoResponse>> Handle(GetUserInformationQuery request, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Error.NotFound($"User with id: {request.UserId} does not exist");

        return new GetUserInfoResponse(
            user.Id,
            user.Username,
            user.Email,
            user.FirstName,
            user.LastName);
    }
}