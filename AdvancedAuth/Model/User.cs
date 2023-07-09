using Microsoft.AspNetCore.Identity;

namespace AdvancedAuth.Model
{
    public class User : IdentityUser<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
