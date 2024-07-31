using YY.Zhihu.SharedLibraries.Message;
using YY.Zhihu.SharedLibraries.Result;
using YY.Zhihu.UseCases.Common.Attributes;
using YY.Zhihu.UseCases.Contracts.Interfaces;

namespace YY.Zhihu.UseCases.AppUsers.Queries
{
    [Authorize]
    public record GetUserInfoQuery(int Id) : IQuery<Result<UserInfoDto>>;

    public class GetUserInfoQueryHandler
        (IDataQueryService queryService) : IQueryHandler<GetUserInfoQuery, Result<UserInfoDto>>
    {
        public async Task<Result<UserInfoDto>> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
        {
            var queryable = queryService.AppUsers
                .Where(u => u.Id == request.Id)
                .Select(u => new UserInfoDto
                {
                    Id = u.Id,
                    Nickname = u.Nickname,
                    Avatar = u.Avatar,
                    Bio = u.Bio,
                    FolloweesCount = u.Followees.Count,
                    FollowersCount = u.Followers.Count
                });

            var appUserInfo = await queryService.FirstOrDefaultAsync(queryable);

            return appUserInfo is null ? Result.NotFound() : Result.Success(appUserInfo);
        }
    }
}