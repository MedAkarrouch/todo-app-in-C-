using System.ComponentModel.DataAnnotations;

namespace TODOAPP.Dtos
{
  public class LoginUserDto
  {
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = "";
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = "";
  }
}