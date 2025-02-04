using System.ComponentModel.DataAnnotations;

namespace FlightApp.Data
{
    public class User
    {
        public User() { }

        public User(string firstname, string lastname, string email, string password, string role)
        {
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            Password = password;
            Role = role;
        }

        [Key]
        public int UserId { get; set; }

        [Required]
        public string Firstname { get; set; } = string.Empty;

        [Required]
        public string Lastname { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";
    }
}
