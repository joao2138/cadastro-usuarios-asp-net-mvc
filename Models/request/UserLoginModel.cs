using System.ComponentModel.DataAnnotations;

namespace WebApplicationMVC.Models.request
{
   public class UserLoginModel
   {
      [Required(ErrorMessage ="O nome de usuario é obrigatorio")]
      public required string UserName { get; init; }


      [Required(ErrorMessage = "A senha é obrigatoria")]
      public required string Password { get; init; }


      public bool RememberMe { get; init; } = true;
   }
}
