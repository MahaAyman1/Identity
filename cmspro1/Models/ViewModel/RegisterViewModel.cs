using System.ComponentModel.DataAnnotations;

namespace cmspro1.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Enter Your Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter Confirm Email")]
        [EmailAddress]
        [Compare("Email" , ErrorMessage =("Not match"))]
        public string ConfirmEmail{ get; set; }

        [Required(ErrorMessage = "Enter pass")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Enter  Confirm pass")]
        [DataType(DataType.Password)]
        [Compare("Password" , ErrorMessage =("Pass Not match"))]
        public string ConfirmPassword { get; set; }

        public string Mobile { get; set; } 

    }
}
