namespace Services.API.Security.Core.Dto
{
    public class UpdatePasswordDto
    {

        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}
