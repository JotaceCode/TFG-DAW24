using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.API.Security.Core.Dto;
using Services.API.Security.Core.Entities;
using Services.API.Security.Core.JwtLogic;
using Services.API.Security.Core.Persistence;

namespace Services.API.Security.Core.Application
{
    // Business logic
    // Insert User
    public class Register
    {
        public class UserRegisterCommand : IRequest<RegisteredUserDto>
        {
            // Identity properties
            public string? Id { get; set; } // Exists, but is null when we create one new user
            public string? Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }

            public string PhoneNumber { get; set; }

            // User properties
            public string Name { get; set; }
            public string Surname { get; set; }
            public string DNI { get; set; }

            public bool RoleAdmin { get; set; }

            // Optional User properties for register
            public DateTime? BirthDate { get; set; }
            public string? City { get; set; }
            public string? CP { get; set; }
            public string? urlImage { get; set; }

        }

        public class UserRegisterHandler : IRequestHandler<UserRegisterCommand, RegisteredUserDto>
        {
            private readonly SQLServerContext _context;
            private readonly UserManager<UserEntity> _userManager;
            private readonly IMapper _mapper;
            private readonly IJwtGenerator _jwtGenerator;

            public UserRegisterHandler(SQLServerContext context, UserManager<UserEntity> userManager, IMapper mapper, IJwtGenerator jwtGenerator)
            {
                _context = context;
                _userManager = userManager;
                _mapper = mapper;
                _jwtGenerator = jwtGenerator;
            }

            public async Task<RegisteredUserDto> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
            {
                var exists = await _context.Users.Where(x => x.Email == request.Email).AnyAsync();
                if (exists)
                {
                    throw new Exception("El Email del usuario ya existe en la base de datos");
                }

                exists = await _context.Users.Where(x => x.UserName == request.Username).AnyAsync();
                if (exists)
                {
                    throw new Exception("El UserName del usuario ya existe en la base de datos");
                }

                var user = new UserEntity
                {
                    UserName = request.Email, // In this case, Username will be the email too
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Name = request.Name,
                    Surname = request.Surname,
                    DNI = request.DNI,
                    RoleAdmin = request.RoleAdmin,
                    BirthDate = request.BirthDate,
                    City = request.City,
                    CP = request.CP,
                    urlImage = request.urlImage
                };

                var result = await _userManager.CreateAsync(user, request.Password);


                if (result.Succeeded)
                {
                    var registeredUserDto = _mapper.Map<UserEntity, RegisteredUserDto>(user);
                    registeredUserDto.Token = _jwtGenerator.GenerateJwt(user);
                    return registeredUserDto;
                }
                throw new Exception("No se pudo registrar el usuario");

            }
        }
    }
}
