using Microsoft.AspNetCore.Identity;

namespace Services.API.Security.Core.Entities
{
    // Define a 'User' Model
    public class UserEntity : IdentityUser
    {
        // No needed, identity have them
        //public string Id { get; set; }
        //public string Username { get; set; }
        //public string Email { get; set; }
        //public string? PhoneNumber { get; set; }

        // Required
        public string Name { get; set; }
        public string Surname { get; set; }
        public string DNI { get; set; }
        public bool RoleAdmin { get; set; }

        // Optional
        public DateTime? BirthDate { get; set; }
        public string? City { get; set; }
        public string? CP { get; set; }

        public string? urlImage { get; set; }

        public bool? Disabled { get; set; }
    }
}
