using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Services.API.Security.Core.Dto;
using Services.API.Security.Core.Entities;
using Services.API.Security.Core.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Services.API.Security.Core.Application
{
    public class Update
    {

        // Update
        public class UserUpdateCommand : IRequest<RegisteredUserDto>
        {
            // url id
            public string? idUser { get; set; }

            // Identity properties
            public string? Id { get; set; } // Exists, but is null when we create one new user
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? Password { get; set; }

            public string? PhoneNumber { get; set; }

            // User properties
            public string? Name { get; set; }
            public string? Surname { get; set; }
            public string? DNI { get; set; }

            public bool? RoleAdmin { get; set; } = false;

            // Optional User properties for register
            public DateTime? BirthDate { get; set; }
            public string? City { get; set; }
            public string? CP { get; set; }
            public string? urlImage { get; set; }
            public bool? Disabled { get; set; }

        }

        public class UserUpdateHandler : IRequestHandler<UserUpdateCommand, RegisteredUserDto>
        {
            private readonly SQLServerContext _context;
            private readonly UserManager<UserEntity> _userManager;
            private readonly IMapper _mapper;

            public UserUpdateHandler(SQLServerContext context, UserManager<UserEntity> userManager, IMapper mapper)
            {
                _context = context;
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<RegisteredUserDto> Handle(UserUpdateCommand request, CancellationToken cancellationToken)
            {
                var exists = await _context.Users.Where(x => x.Id == request.idUser).AnyAsync();
                var same = request.Id == request.idUser;

                if (!exists)
                {
                    throw new Exception("El Id del usuario no existe en la base de datos.");
                }

                if (!same)
                {
                    throw new Exception("El Id del usuario y el de la URL no coinciden.");
                }

                var registeredUser = await _context.Users.Where(x => x.Id == request.Id).FirstAsync();

                registeredUser.Id = request.Id;
                registeredUser.UserName = request.Email;
                registeredUser.Email = request.Email;
                registeredUser.PhoneNumber = request.PhoneNumber;
                registeredUser.Name = request.Name;
                registeredUser.Surname = request.Surname;
                registeredUser.DNI = request.DNI;
                registeredUser.RoleAdmin = (bool)request.RoleAdmin;
                registeredUser.Disabled = (bool)request.Disabled;
                registeredUser.BirthDate = request.BirthDate;
                registeredUser.City = request.City;
                registeredUser.CP = request.CP;
                registeredUser.urlImage = request.urlImage;

                var result = await _userManager.UpdateAsync(registeredUser);

                if (result.Succeeded)
                {
                    var registeredUserDto = _mapper.Map<UserEntity, RegisteredUserDto>(registeredUser);
                    return registeredUserDto;
                }
                throw new Exception("No se pudo actualizar el usuario");
            }
        }

    }
}
