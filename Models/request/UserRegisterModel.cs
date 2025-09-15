using System.ComponentModel.DataAnnotations;

namespace WebApplicationMVC.Models.request
{
   public class UserRegisterModel
   {
      [Required(ErrorMessage = "O nome de usuario é obrigatorio")]
      [MaxLength(80, ErrorMessage = "O nome de usuario deve ter no máximo 80 caracteres")]
      public required string UserName { get; init; }



      [EmailAddress(ErrorMessage = "Este não é um email valido")]
      [MaxLength(80, ErrorMessage = "O email deve ter no máximo 80 caracteres")]
      public string? Email { get; init; }



      [MinLength(5, ErrorMessage = "A senha deve conter no mínimo 5 caracteres")]
      [MaxLength(64, ErrorMessage = "A senha deve conter no máximo 64 caracteres")]
      public required string Password { get; init; }



      [AllowedValues(["admin", "user"], ErrorMessage = "O tipo de usuario deve estar entre \"administrador\" e \"usuario\"")]
      public string UserType { get; init; } = "user";


      public bool RememberMe { get; init; } = true;


   }
}
