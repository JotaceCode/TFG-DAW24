namespace Services.API.Security.Core.Dto
{
    // Dto for not return all data from user when the account is created (basically for not return user password and sensible data)
    public class RegisteredUserDto
    {
        // Identity properties
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        // User properties
        public string Name { get; set; }
        public string Surname { get; set; }
        public string DNI { get; set; }
        public bool RoleAdmin { get; set; }

        // Optional User properties for register
        public DateTime? BirthDate { get; set; }
        public string? City { get; set; }
        public string? CP { get; set; }

        public string? Token { get; set; }

        public string? urlImage { get; set; }

        public bool? Disabled { get; set; }

    }
}
