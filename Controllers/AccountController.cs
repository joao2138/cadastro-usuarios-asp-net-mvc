using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationMVC.data;
using WebApplicationMVC.data.extensions;
using WebApplicationMVC.Models;
using WebApplicationMVC.Models.request;
using WebApplicationMVC.repositories;

namespace WebApplicationMVC.Controllers
{
   public class AccountController : Controller
   {
      private readonly IAccountRepository _accountRepository;
      private readonly TokenGenerator _tokenGenerator;
      private readonly IAuthRepository _authRepository;



      public AccountController(IAuthRepository authRepository, IAccountRepository accountRepository, TokenGenerator token)
      {
         _accountRepository = accountRepository;
         _tokenGenerator = token;
         _authRepository = authRepository;
      }



      [HttpGet]
      public IActionResult LoginPage(string? route = null)
      {
         ViewData["Route"] = route;
         return View();
      }




      [HttpPost]
      public async Task<IActionResult> Register(UserRegisterModel userRegister)
      {
         TempData["ActiveTab"] = "register";

         if (!ModelState.IsValid)
         {
            return View("LoginPage",
               new LoginRegisterViewModel { Register = userRegister });
         }


         UserModel userModel = new()
         {
            Id = Guid.NewGuid(),
            UserName = userRegister.UserName,
            Email = userRegister.Email,
            Password = userRegister.Password,
            CreatedAt = DateTime.Now,
            UserType = userRegister.UserType
         };


         var result = await _accountRepository.CreateUser(userModel);


         if (result.IsT0)
         {
            if (userRegister.RememberMe)
            {
               string refreshToken = _tokenGenerator.CreateRefreshToken();

               await _authRepository.SaveRefreshToken(userModel.Id.ToString(), refreshToken);

               await HttpContext.SetCookie(userModel, refreshToken);
            }
            else
            {
               await HttpContext.SetCookie(userModel);
            }

            return RedirectToAction("Index", "Home");
         }


         AppError error = result.AsT1;

         TempData.AddInfoMessage(error.Errors);

         return
            View("LoginPage", new LoginRegisterViewModel { Register = userRegister });


      }




      [Authorize, HttpGet]
      public async Task<IActionResult> GetUsers()
      {
         var users = await _accountRepository.GetUsers();

         return View(users);

      }






      [Authorize("admin")]
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




      [Authorize("admin")]
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




      [Authorize, HttpGet]
      public async Task<IActionResult> UserProfile()
      {
         if (!User.TryGetUserId(out var userId))
         {
            TempData.AddInfoMessage("O Id de usuário não pode ser obtido");
            return View();
         }


         var user = await _accountRepository.GetUserById(userId);

         if (user is null)
         {
            TempData.AddInfoMessage("Ocorreu um erro ao retornar as informações do usuário");
            return View();
         }

         return View(user);
      }




      [Authorize, HttpGet]
      public async Task<IActionResult> UpdateUserProfile()
      {
         if (!User.TryGetUserId(out var userId))
         {
            TempData.AddInfoMessage("O Id de usuário não pode ser obtido");
            return View();
         }


         var user = await _accountRepository.GetUserById(userId);

         if (user is null)
         {
            TempData.AddInfoMessage("Usuário não encontrado");
            return View();
         }

         return View((UpdateUserModel)user);
      }




      [Authorize, HttpPost]
      public async Task<IActionResult> UpdateUserProfile(UpdateUserModel updateUser)
      {
         if (!User.TryGetUserId(out var userId))
         {
            TempData.AddInfoMessage("O Id de usuário não pode ser obtido");
            return View();
         }


         if (!ModelState.IsValid) return View(updateUser);


         UpdateUserModel userChanges = new()
         {
            Email = updateUser.Email,
            UserName = updateUser.UserName,
         };


         var result = await _accountRepository.UpdateUser(userId, userChanges);

         if (result.IsT0)
         {
            TempData.AddInfoMessage("Usuário atualizado com sucesso");

            //redirecionar para atualizar o userName do cookie de sessão
            return Redirect("/auth/refresh?route=/account/userProfile");

            //ou apenas retornar a view
            //mas o nome de usuario que aparece no header da pagina ficará desatualizado
            //return View(updateUser);
         }

         AppError error = result.AsT1;

         TempData.AddInfoMessage(error.Errors);

         return View(updateUser);
      }





      [Authorize, HttpGet]
      public IActionResult UpdatePassword()
      {
         return View();
      }



      [Authorize, HttpPost]
      public async Task<IActionResult> UpdatePassword(UpdateUserPasswordModel updateUserPassword)
      {

         if (!User.TryGetUserId(out var userId))
         {
            TempData.AddInfoMessage("O Id de usuário não pode ser obtido");
            return View(updateUserPassword);
         }

         if (!ModelState.IsValid) return View(updateUserPassword);



         var result = await _accountRepository.UpdateUserPassword(userId, updateUserPassword);

         if (result.IsT0)
         {
            TempData.AddInfoMessage("Senha atualizada com sucesso");
            return View(updateUserPassword);
         }
         

         AppError appError = result.AsT1;

         TempData.AddInfoMessage(appError.Errors);

         return View(updateUserPassword);

      }



   }
}
