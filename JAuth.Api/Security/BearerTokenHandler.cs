using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JAuth.Api.Security
{
    /// <summary>
    /// This class is a custom token handler that will be used to validate the token.
    /// It is necessary due to a bug in the JwtBearerHandler that which throws an
    /// exception saying "IDX14100: JWT is not well formed, there are no dots (.)"
    /// See https://github.com/dotnet/aspnetcore/issues/52286 for more information.
    /// 
    /// Usage: Just add it to your TokenHandlers collection in the JwtBearerOptions.
    /// Example: options.TokenHandlers.Add(new BearerTokenHandler());
    /// </summary>
    public class BearerTokenHandler : TokenHandler
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = new();

        /// <summary>
        /// Override the built-in ValidateTokenAsync method to bypass a bug in the JwtBearerHandler
        /// saying the JWT is not well formed (IDX14100). See this class's summary for more information.
        /// </summary>
        /// <param name="token">The JWT token as a string</param>
        /// <param name="validationParameters">The validation parameters to check against</param>
        /// <returns>A TokenValidationResult object with the result of the validation</returns>
        public override Task<TokenValidationResult> ValidateTokenAsync(string token, TokenValidationParameters validationParameters)
        {
            try
            {
                _tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwtSecurityToken)
                    return Task.FromResult(new TokenValidationResult() { IsValid = false });

                return Task.FromResult(new TokenValidationResult
                {
                    IsValid = true,
                    ClaimsIdentity = new ClaimsIdentity(jwtSecurityToken.Claims, JwtBearerDefaults.AuthenticationScheme),

                    // If you do not add SecurityToken to the result, then our validator will fire, return a positive result, 
                    // but the authentication, in general, will fail.
                    SecurityToken = jwtSecurityToken,
                });
            }

            catch (Exception e)
            {
                return Task.FromResult(new TokenValidationResult
                {
                    IsValid = false,
                    Exception = e,
                });
            }
        }
    }
}
