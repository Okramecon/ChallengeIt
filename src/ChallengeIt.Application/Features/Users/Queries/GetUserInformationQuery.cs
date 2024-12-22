using ChallengeIt.API.Contracts.Users;
using ChallengeIt.Application.Persistence;
using MediatR;
using ErrorOr;

namespace ChallengeIt.Application.Features.Users.Queries;

public record GetUserInformationQuery(long UserId) : IRequest<ErrorOr<GetUserInfoResponse>>;

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