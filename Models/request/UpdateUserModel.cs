using System.ComponentModel.DataAnnotations;

namespace WebApplicationMVC.Models.request
{
   public class UpdateUserModel
   {

      public string Id { get; init; } = string.Empty;



      [MaxLength(80, ErrorMessage = "O nome de usuario deve ter no máximo 80 caracteres")]
      public string? UserName { get; init; }



      [EmailAddress(ErrorMessage = "Este não é um email valido")]
      [MaxLength(80, ErrorMessage = "O email deve ter no máximo 80 caracteres")]
      public string? Email { get; init; }



      public DateTime CreatedAt { get; init; }


      public string? UserType { get; init; }




      public static explicit operator UpdateUserModel(UserViewModel userViewModel)
      {
         return new()
         {
            Id = userViewModel.Id,
            UserName = userViewModel.UserName,
            Email = userViewModel.Email,
            UserType = userViewModel.UserType,
            CreatedAt = userViewModel.CreatedAt,

         };
      }



   }

}
