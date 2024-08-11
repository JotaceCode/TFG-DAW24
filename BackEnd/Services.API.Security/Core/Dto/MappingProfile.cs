using AutoMapper;
using Services.API.Security.Core.Entities;

namespace Services.API.Security.Core.Dto
{
    // Class for mapping other classes
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserEntity, RegisteredUserDto>();
        }
    }
}
