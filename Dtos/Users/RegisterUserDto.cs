using System.ComponentModel.DataAnnotations;

namespace TODOAPP.Dtos
{
  public class RegisterUserDto
  {
    [Required(ErrorMessage ="Email is required")]
    [EmailAddress(ErrorMessage ="Email is invalid")]
    public string Email {get;set;}="";
    
    [Required(ErrorMessage = "Full name is required")]
    [MinLength(3,ErrorMessage ="Full name must be at least 3 characters long")]
    public string FullName {get;set;}="";

    [Required(ErrorMessage ="Password is required")]
    [MinLength(8,ErrorMessage ="Password must be at least 8  characters long")]
    public string Password {get;set;}="";
  }
}