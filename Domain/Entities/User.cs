using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(80, ErrorMessage = "Name must not exceed 80 characters")]
        public string Name { get; set; } = string.Empty;
    }
}
