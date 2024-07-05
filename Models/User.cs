using Microsoft.AspNetCore.Identity;

namespace The_Look_Lab.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
    }
}
