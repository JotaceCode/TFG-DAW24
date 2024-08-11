using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Services.API.Security.Core.Dto;
using Services.API.Security.Core.Entities;
using Services.API.Security.Core.JwtLogic;
using Services.API.Security.Core.Persistence;

namespace Services.API.Security.Core.Application
{
    public class Login
    {
        public class UserLoginCommand : IRequest<RegisteredUserDto>
        {
            public string Email { get; set; }

            public string Password { get; set; }
        }

        public class UserLoginValidation : AbstractValidator<UserLoginCommand>
        {
            public UserLoginValidation()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }

        }

        public class UserLoginHandler : IRequestHandler<UserLoginCommand, RegisteredUserDto>
        {
            private readonly SQLServerContext _context;
            private readonly UserManager<UserEntity> _userManager;
            private readonly IMapper _mapper;
            private readonly IJwtGenerator _jwtGenerator;
            private readonly SignInManager<UserEntity> _signInManager;

            public UserLoginHandler(SQLServerContext context, UserManager<UserEntity> userManager, IMapper mapper, IJwtGenerator jwtGenerator, SignInManager<UserEntity> signInManager)
            {
                _context = context;
                _userManager = userManager;
                _mapper = mapper;
                _jwtGenerator = jwtGenerator;
                _signInManager = signInManager;
            }

            public async Task<RegisteredUserDto> Handle(UserLoginCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    throw new Exception("El usuario no existe");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

                if (result.Succeeded)
                {
                    var logedUser = _mapper.Map<UserEntity, RegisteredUserDto>(user);
                    logedUser.Token = _jwtGenerator.GenerateJwt(user);
                    return logedUser;
                }

                throw new Exception("Login incorrecto");
            }
        }
    }
}
