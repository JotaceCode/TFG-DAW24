using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Services.API.Security.Core.Dto;
using Services.API.Security.Core.Entities;
using Services.API.Security.Core.JwtLogic;

namespace Services.API.Security.Core.Application
{
    public class LoggedInUser
    {
        public class LoggedInUserCommand : IRequest<RegisteredUserDto> { }

        public class LoggedInUserHandler : IRequestHandler<LoggedInUserCommand, RegisteredUserDto>
        {
            private readonly UserManager<UserEntity> _userManager;
            private readonly IUserLogged _userLogged;
            private readonly IJwtGenerator _jwtGenerator;
            private readonly IMapper _mapper;

            public LoggedInUserHandler(UserManager<UserEntity> userManager, IUserLogged userLogged, IJwtGenerator jwtGenerator, IMapper mapper)
            {
                _userManager = userManager;
                _userLogged = userLogged;
                _jwtGenerator = jwtGenerator;
                _mapper = mapper;
            }

            public async Task<RegisteredUserDto> Handle(LoggedInUserCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userLogged.GetLoggedUser());
                if (user != null)
                {
                    var loggedInUser = _mapper.Map<UserEntity, RegisteredUserDto>(user);
                    loggedInUser.Token = _jwtGenerator.GenerateJwt(user);
                    return loggedInUser;
                }

                throw new Exception("No se encontró al usuario");
            }
        }
    }
}
