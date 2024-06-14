using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Entites;

namespace YY.Zhihu.UseCases.AppUsers
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, AppUserDto>();
        }
    }

    public record AppUserDto(int Id, string Nickname);
    public record UserInfoDto
    {
        public int Id { get; set; }

        public string? Nickname { get; set; }

        public string? Avatar { get; set; }

        public string? Bio { get; set; }

        public int FolloweesCount { get; set; }

        public int FollowersCount { get; set; }
    }
}
