using System.ComponentModel.DataAnnotations;

namespace Login.Web.Models
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "The {0} is required!")]
        [StringLength(40, ErrorMessage = "The {0} mus have {2} and {1} characters!", MinimumLength = 3)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The {0} is required!")]
        [StringLength(40, ErrorMessage = "The {0} mus have {2} and {1} characters!", MinimumLength = 3)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The {0} is required!")]
        [StringLength(16, ErrorMessage = "The {0} mus have {2} and {1} characters!", MinimumLength = 6)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The {0} is required!")]
        [EmailAddress(ErrorMessage = "The {0} is invalid!")]
        public string Email { get; set; }
        
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "The {0} is required!")]
        [StringLength(16, ErrorMessage = "The {0} mus have {2} and {1} characters!", MinimumLength = 6)]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords does not match!")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginUserViewModel
    {
        [Required(ErrorMessage = "The {0} is required!")]
        [StringLength(16, ErrorMessage = "The {0} mus have {2} and {1} characters!", MinimumLength = 6)]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "The {0} is required!")]
        [StringLength(16, ErrorMessage = "The {0} mus have {2} and {1} characters!", MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class LoggedUserViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}