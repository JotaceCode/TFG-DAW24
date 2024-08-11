using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.API.Security.Core.Application;
using Services.API.Security.Core.Dto;
using Services.API.Security.Core.Entities;
using Services.API.Security.Core.Persistence;
using System.Security.Claims;

// Controller for the Security API
namespace Services.API.Security.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserServiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly SQLServerContext _context;
        private readonly IMapper _mapper;

        private readonly UserManager<UserEntity> _userManager;

        private readonly PasswordHasher<IdentityUser> _passwordHasher;

        private readonly IWebHostEnvironment _environment;


        public UserServiceController(IMediator mediator, SQLServerContext context, IMapper mapper, UserManager<UserEntity> userManager, PasswordHasher<IdentityUser> passwordHasher, IWebHostEnvironment environment)
        {
            _mediator = mediator;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<List<RegisteredUserDto>>> GetAll()
        {
            var usersEntities = await _context.Users.ToListAsync();
            var registeredUsersDto = _mapper.Map<List<UserEntity>, List<RegisteredUserDto>>(usersEntities);
            return Ok(registeredUsersDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RegisteredUserDto>> GetById(string Id)
        {
            var userEntity = await _context.Users.FindAsync(Id);
            if (userEntity == null)
            {
                return NotFound("El usuario no existe.");
            }
            var registeredUserDto = _mapper.Map<UserEntity, RegisteredUserDto>(userEntity);
            return Ok(registeredUserDto);
        }

        [HttpPost]
        public async Task<ActionResult<RegisteredUserDto>> Insert(Register.UserRegisterCommand options)
        {
            return await _mediator.Send(options);
        }

        [HttpPost("login")]
        public async Task<ActionResult<RegisteredUserDto>> Login(Login.UserLoginCommand options)
        {
            return await _mediator.Send(options);
        }

        [HttpPost("loggedUser")]
        public async Task<ActionResult<RegisteredUserDto>> LoggedUser()
        {
            return await _mediator.Send(new LoggedInUser.LoggedInUserCommand());
        }

        [HttpPut("{idUser}")]
        public async Task<ActionResult<RegisteredUserDto>> Update(string idUser, Update.UserUpdateCommand options)
        {
            options.idUser = idUser;
            return await _mediator.Send(options);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound("El usuario no existe.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok("Usuario eliminado correctamente");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "No se pudo eliminar el usuario.");

        }

        [HttpPost("pagination")]
        public async Task<ActionResult<UsersFilteredDto>> Pagination([FromBody] UsersFilteredDto parameters)
        {
            IQueryable<UserEntity> query = _context.Users;

            if (!string.IsNullOrEmpty(parameters.Sort))
            {
                if (parameters.SortDirection == "desc")
                {
                    query = query.OrderByDescending((UserEntity doc) => EF.Property<object>(doc, parameters.Sort));
                }
                else
                {
                    query = query.OrderBy((UserEntity doc) => EF.Property<object>(doc, parameters.Sort));
                }
            }
            var filter = parameters.Filter;

            if (filter != null)
            {
                query = query.Where(u => EF.Property<string>(u, parameters.Filter.Property).Contains(parameters.Filter.Value));

            }

            var usersEntities = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var totalUsers = await query.CountAsync();

            parameters.TotalRows = totalUsers;

            var registeredUsersDto = _mapper.Map<List<UserEntity>, List<RegisteredUserDto>>(usersEntities);

            parameters.Data = registeredUsersDto;

            var rounded = Math.Ceiling(totalUsers / Convert.ToDecimal(parameters.PageSize));
            var totalPages = Convert.ToInt32(rounded);

            parameters.PagesQuantity = totalPages;

            return Ok(parameters);
        }

        // Logged in user (need old password)
        [HttpPost("updatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto updatePasswordModel)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, updatePasswordModel.oldPassword);
            if (!passwordCheck)
            {
                return BadRequest("La contraseña es incorrecta");
            }

            var result = await _userManager.ChangePasswordAsync(user, updatePasswordModel.oldPassword, updatePasswordModel.newPassword);
            if (!result.Succeeded)
            {
                return BadRequest("No se pudo cambiar la contraseña");
            }

            return Ok("Contraseña cambiada exitosamente");
        }

        // For the admin (no need old password)
        [HttpPost("resetPassword/{id}")]
        public async Task<IActionResult> UpdatePassword(string id, [FromBody] ResetPasswordDto resetPasswordModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }


            user.PasswordHash = _passwordHasher.HashPassword(user, resetPasswordModel.newPassword);

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al cambiar la contraseña");
            }

            return Ok("Contraseña cambiada exitosamente");
        }

        [HttpPost("uploadPhoto")]
        public async Task<IActionResult> UploadPhoto()
        {
            try
            {
                var file = Request.Form.Files[0];

                if (file != null && file.Length > 0)
                {
                    var uploads = Path.Combine(_environment.WebRootPath, "photos");
                    var filePath = Path.Combine(uploads, file.FileName);

                    if (file.FileName.Contains("cuadrante"))
                    {
                        var cacheBuster = DateTime.Now.Ticks;
                        var cacheBusterQueryString = $"cache={cacheBuster}";

                        var extension = Path.GetExtension(file.FileName);
                        var newFileName = "cuadrante" + cacheBusterQueryString + extension;

                        filePath = Path.Combine(uploads, newFileName);
                        var formatoFilePath = Path.Combine(uploads, "cuadrante.txt");

                        var existingContent = await System.IO.File.ReadAllTextAsync(formatoFilePath);
                        if (existingContent != newFileName)
                        {
                            await DeletePhotoAsync(existingContent);
                        }

                        await System.IO.File.WriteAllTextAsync(formatoFilePath, newFileName);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return Ok("Foto subida exitosamente");
                }
                else
                {
                    return BadRequest("No se envió ningún archivo");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al subir la foto: {ex.Message}");
            }
        }

        [HttpDelete("deletePhoto")]
        public async Task<IActionResult> DeletePhotoAsync(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    return BadRequest("El nombre del archivo es obligatorio");
                }

                var filePath = Path.Combine("wwwroot/photos", fileName);

                if (fileName.Contains("cuadrante"))
                {
                    var formatoFilePath = Path.Combine("wwwroot/photos", "cuadrante.txt");
                    var existingContent = await System.IO.File.ReadAllTextAsync(formatoFilePath);
                    filePath = Path.Combine("wwwroot/photos", existingContent);
                    await System.IO.File.WriteAllTextAsync(formatoFilePath, "");
                }
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("El archivo no existe");
                }

                System.IO.File.Delete(filePath);

                return Ok("Foto eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar la foto: {ex.Message}");
            }
        }

        [HttpGet("getCuadranteFileName")]
        public async Task<IActionResult> GetCuadranteFileName()
        {
            try
            {
                var formatoFilePath = Path.Combine("wwwroot/photos", "cuadrante.txt");
                if (System.IO.File.Exists(formatoFilePath))
                {
                    var contenido = await System.IO.File.ReadAllTextAsync(formatoFilePath);
                    return Ok(contenido);
                }
                else
                {
                    return NotFound("El archivo cuadrante.txt no existe");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el nombre del archivo: {ex.Message}");
            }
        }

    }
}
