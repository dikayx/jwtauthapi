using JAuth.Api.Data;

namespace JAuth.Api.Factories
{
    public static class LoginResultFactory
    {
        public static LoginResult Create(bool success, string message, string? token = null)
        {
            return new LoginResult
            {
                Success = success,
                Message = message,
                Token = token
            };
        }
    }
}
