using Microsoft.AspNetCore.Identity;

namespace MangoFusion_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        // using all properties of Identity user and adding new Name Property along with default value as empty string.
        public string Name { get; set; } = string.Empty;
    }
}
