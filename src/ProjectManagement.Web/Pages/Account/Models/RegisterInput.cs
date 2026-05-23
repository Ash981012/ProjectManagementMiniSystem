using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Web.Pages.Account.Models;

public sealed class RegisterInput
{
    [Required(ErrorMessage = "Full name is required.")]
    [Display(Name = "Full name")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters.")]
    [RegularExpression(@"^[A-Za-z][A-Za-z .'\-]*$", ErrorMessage = "Full name can contain only letters, spaces, apostrophes, dots, and hyphens.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$", ErrorMessage = "Enter a valid email address, for example name@example.com.")]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,100}$",
        ErrorMessage = "Password must include uppercase, lowercase, number, and special character.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare(nameof(Password), ErrorMessage = "Password and confirm password must match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
