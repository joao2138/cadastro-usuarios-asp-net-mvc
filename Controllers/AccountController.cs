using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationMVC.data;
using WebApplicationMVC.Models;
using WebApplicationMVC.Models.request;
using WebApplicationMVC.repositories;

namespace WebApplicationMVC.Controllers
{
   public class AccountController : Controller
   {
      private readonly IAccountRepository _userRepository;
      private readonly TokenGenerator _tokenGenerator;
      private readonly IAuthRepository _authRepository;



      public AccountController(IAuthRepository authRepository, IAccountRepository userRepository, TokenGenerator token)
      {
         _userRepository = userRepository;
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


         var userModel = new UserModel
         {
            Id = Guid.NewGuid(),
            UserName = userRegister.UserName,
            Email = userRegister.Email,
            Password = userRegister.Password,
            CreatedAt = DateTime.Now,
            UserType = userRegister.UserType
         };

         var result = await _userRepository.CreateUser(userModel);


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

         TempData[nameof(AppError.Errors)] = error.Errors;

         return
            View("LoginPage", new LoginRegisterViewModel { Register = userRegister });


      }



 
      [Authorize, HttpGet]
      public async Task<IActionResult> GetUsers()
      {
         var users = await _userRepository.GetUsers();

         return View(users);

      }




      //[Authorize("admin"), HttpGet]
      //public async Task<IActionResult> UpdateUser()
      //{
      
      //}



   }
}
