using System.ComponentModel.DataAnnotations;

namespace WebApplicationMVC.Models.request
{
   public class DeleteAccountModel
   {
      public string? UserName { get; set; } = null;

      public string? UserId { get; set; } = null;


      [Required(ErrorMessage = "Insira a senha")]
      public required string Password { get; init; }



      [Compare(nameof(Password), ErrorMessage = "A senha de confirmação não confere com a senha.")]
      public required string ConfirmPassword { get; init; }



   }
}
