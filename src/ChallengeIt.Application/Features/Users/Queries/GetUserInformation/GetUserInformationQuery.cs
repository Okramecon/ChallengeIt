using ChallengeIt.API.Contracts.Users;
using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Users.Queries.GetUserInformation;

public record GetUserInformationQuery(long UserId) : IRequest<ErrorOr<GetUserInfoResponse>>;