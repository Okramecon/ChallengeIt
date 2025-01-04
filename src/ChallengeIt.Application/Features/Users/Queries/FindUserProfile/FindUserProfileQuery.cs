using ChallengeIt.Domain.Models.User;
using MediatR;
using ErrorOr;

namespace ChallengeIt.Application.Features.Users.Queries.FindUserProfile;

public record FindUserProfileQuery(string SearchString) : IRequest<ErrorOr<List<SearchUserProfileModel>>>;