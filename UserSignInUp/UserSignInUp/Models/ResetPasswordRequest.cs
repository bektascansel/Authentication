using System.ComponentModel.DataAnnotations;

namespace UserSignInUp.Models
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; }=string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
}
