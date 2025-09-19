using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationMVC.data;
using WebApplicationMVC.data.extensions;
using WebApplicationMVC.Models;
using WebApplicationMVC.Models.request;
using WebApplicationMVC.repositories;

namespace WebApplicationMVC.Controllers
{

   [Authorize("admin")]
   public class AdminController(IAccountRepository accountRepository) : Controller
   {
      private readonly IAccountRepository _accountRepository = accountRepository;




      [HttpGet("/account/admin/updateUser")]
      public async Task<IActionResult> UpdateUser(string? userId)
      {
         if (string.IsNullOrEmpty(userId))
         {
            TempData.AddInfoMessage("O Id de usuario deve ser informado");
            return View();
         }


         var user = await _accountRepository.GetUserById(userId);

         if (user is null)
         {
            TempData.AddInfoMessage("Usuário não encotrado");
            return View();
         }


         return View((UpdateUserModel)user);
      }




      [HttpPost("/account/admin/updateUser")]
      public async Task<IActionResult> UpdateUser(string? userId, UpdateUserModel updateUser)
      {

         if (string.IsNullOrEmpty(userId))
         {
            TempData.AddInfoMessage("O Id de usuario deve ser informado");
            return View();
         }

         if (!Consts.UserTypes.Any(type => type == updateUser.UserType))
         {
            TempData.AddInfoMessage($"O tipo de usuario deve estar entre: {string.Join(", ", Consts.UserTypes)}");

            return View(updateUser);
         }


         if (!ModelState.IsValid) return View(updateUser);


         var result = await _accountRepository.UpdateUser(userId, updateUser);


         if (result.IsT0)
         {
            TempData.AddInfoMessage($"Usuário {updateUser.UserName} atualizado com sucesso");

            return View(updateUser);
         }


         AppError error = result.AsT1;

         TempData.AddInfoMessage(error.Errors);

         return View(updateUser);
      }





      [HttpGet("/account/admin/delete")]
      public async Task<IActionResult> DeleteAccount(string? userId)
      {
         if (string.IsNullOrEmpty(userId))
         {
            TempData.AddInfoMessage("O Id de usuario deve ser informado");
            return View();
         }


         var user = await _accountRepository.GetUserById(userId);

         if (user is null)
         {
            TempData.AddInfoMessage("Usuário não encotrado");
            return View();
         }

         DeleteAccountModel deleteAccountModel = new()
         {
            ConfirmPassword = "",
            Password = "",
            UserId = user.Id,
            UserName = user.UserName,
         };

         return View(deleteAccountModel);

      }




      [HttpPost("/account/admin/delete")]
      public async Task<IActionResult> DeleteAccount(string? userId, DeleteAccountModel deleteAccountModel)
      {
         if (!ModelState.IsValid) return View(deleteAccountModel);

         if (string.IsNullOrEmpty(userId))
         {
            TempData.AddInfoMessage("O Id de usuário deve ser informado");
            return View(deleteAccountModel);
         }

         if (!User.TryGetUserId(out var adminId))
         {
            TempData.AddInfoMessage("Não foi possivel obter o Id do administrador");
            return View(deleteAccountModel);
         }

         if (userId == adminId)
         {
            TempData.AddInfoMessage("O administrador não deveria excluir a própria conta com este recurso");
            return View(deleteAccountModel);
         }


         var result = await _accountRepository.AdminDeleteUserAccount(userId, deleteAccountModel, adminId);

         if (result.IsT0)
         {
            ViewData["redirect"] = "/account/getusers";
            return View("Views/Account/AccountDeleted.cshtml");
         }


         AppError appError = result.AsT1;

         TempData.AddInfoMessage(appError.Errors);

         return View(deleteAccountModel);

      }





   }
}
