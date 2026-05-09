using System.ComponentModel.DataAnnotations;

namespace  CanvasBid.DTOS.UserDTOS
{
    public class RegisterUserDto
    {
      [Required]
      public string Username { get; set; } = string.Empty;

      [Required]
      [EmailAddress]
      public string Email { get; set; } = string.Empty;

      [Required]
      [MinLength(8)]
      public string Password { get; set; } = string.Empty;

      [Required]
      [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
      public string ConfirmPassword { get; set; } = string.Empty;

      public string Role { get; set; } = string.Empty; // "Admin", "Artist", "Buyer"
    }
}