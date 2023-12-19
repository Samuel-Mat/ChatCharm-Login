using System.Security.Claims;

namespace ChatCharm_Login.Service
{
    
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }
        public string GetId()
        {
            string result = "";

            if (_contextAccessor.HttpContext != null)
            {
                var id = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

                if (id != null)
                {
                    result = id.Value;
                }
                else
                {
                    // Log or handle the case where parsing fails
                    Console.WriteLine("Error parsing user ID claim.");
                }
            }

            return result;
        }
    }
}
