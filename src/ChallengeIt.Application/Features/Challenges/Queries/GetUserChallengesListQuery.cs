using ChallengeIt.Application.Features.Common;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Models.Paging;
using MediatR;
using ErrorOr;

namespace ChallengeIt.Application.Features.Challenges.Queries;

public class GetUserChallengesListQuery : PageQuery, IRequest<ErrorOr<Page<Challenge>>>;