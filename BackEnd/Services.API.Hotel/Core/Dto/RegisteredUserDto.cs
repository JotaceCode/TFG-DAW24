using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace Services.API.Hotel.Core.Dto
{
    // Dto for not return all data from user when the account is created (basically for not return user password and sensible data)
    public class RegisteredUserDto
    {
        // Identity properties
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        // User properties
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("surname")]
        public string? Surname { get; set; }

        [JsonPropertyName("dni")]
        public string? DNI { get; set; }

        [JsonPropertyName("roleAdmin")]
        public bool? RoleAdmin { get; set; }

        // Optional User properties for register

        [JsonPropertyName("birthDate")]
        public DateTime? BirthDate { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("cp")]
        public string? CP { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("urlImage")]
        public string? urlImage { get; set; }
    }
}
