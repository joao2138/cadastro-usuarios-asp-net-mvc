using System.ComponentModel.DataAnnotations;

namespace WebApplicationMVC.Models.request
{
   public class UpdateUserPasswordModel
   {

      [Required(ErrorMessage ="A nova senha é obrigatoria")]
      [MinLength(5, ErrorMessage = "A senha deve conter no mínimo 5 caracteres")]
      [MaxLength(64, ErrorMessage = "A senha deve conter no máximo 64 caracteres")]
      public required string NewPassword { get; init; }


      [Required(ErrorMessage = "A antiga senha é obrigatoria")]
      public required string OldPassword { get; init; }
   }
}
