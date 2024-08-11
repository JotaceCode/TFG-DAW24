namespace Services.API.Security.Core.JwtLogic
{
    public class UserLogged : IUserLogged
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserLogged(IHttpContextAccessor contextAccessor)
        {
            _httpContextAccessor = contextAccessor;
        }

        public string GetLoggedUser()
        {
            var userName = _httpContextAccessor.HttpContext.User?.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
            return userName;
        }
    }
}
