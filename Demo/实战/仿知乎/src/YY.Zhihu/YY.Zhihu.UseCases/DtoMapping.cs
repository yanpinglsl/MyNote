using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Entites;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace YY.Zhihu.UseCases
{
    public record AppUserDto(int Id, string Nickname);

    public record TokenDto(string AccessToken);

    /// <summary>
    /// Profile要引入AutoMapper
    /// </summary>
    public class DtoMapping : Profile
    {
        public DtoMapping()
        {
            CreateMap<AppUser, AppUserDto>();
        }
    }
}
