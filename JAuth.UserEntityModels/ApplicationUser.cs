using Microsoft.AspNetCore.Identity;

namespace JAuth.UserEntityModels
{
    // The same model is used for both the Sqlite and SqlServer databases
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
    }
}
