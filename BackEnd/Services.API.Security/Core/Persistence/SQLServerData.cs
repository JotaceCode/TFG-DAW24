using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.API.Security.Core.Entities;

namespace Services.API.Security.Core.Persistence
{
    // Data Manager (for do DB tasks while app is starting)
    public class SQLServerData
    {
        public static async Task InsertUser(SQLServerContext context, UserManager<UserEntity> userManager)
        {
            context.Database.Migrate();
            if (!userManager.Users.Any())
            {
                var userAdminDefault = new UserEntity
                {
                    // Optional properties

                    BirthDate = DateTime.Now,
                    City = "Huelva",
                    CP = "21006",

                    // user properties
                    Name = "Administrador",
                    Surname = "Admin",
                    RoleAdmin = true,
                    DNI = "00000000X",

                    // identity properties
                    PhoneNumber = "+01 234 567 890",
                    Email = "admin@gmail.com",
                    UserName = "admin@gmail.com",
                };
                await userManager.CreateAsync(userAdminDefault, "StrongPassword1!");
            }
        }
    }
}
