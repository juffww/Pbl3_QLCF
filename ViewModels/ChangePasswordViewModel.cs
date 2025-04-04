using System.ComponentModel.DataAnnotations;

namespace pbl3_QLCF.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string? State;
        public string? ConfirmCode;
        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        public string newPassword { get; set; }
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("newPassword", ErrorMessage = "Password does not match")]
        public string confirmPassword { get; set; }
    }
}
