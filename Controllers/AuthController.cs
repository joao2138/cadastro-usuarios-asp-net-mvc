using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationMVC.data;
using WebApplicationMVC.Models;
using WebApplicationMVC.Models.request;
using WebApplicationMVC.repositories;


namespace WebApplicationMVC.Controllers
{
   public class AuthController(IAuthRepository authRepository, TokenGenerator token) : Controller
   {

      private readonly IAuthRepository _authRepository = authRepository;
      private readonly TokenGenerator _tokenGenerator = token;
      private static readonly CookieOptions CookieOpts = new() { Path = Consts.CookiePath };



      [HttpPost]
      public async Task<IActionResult> Login(UserLoginModel loginModel, string? route = null)
      {
         TempData["ActiveTab"] = "login";
         ViewData["Route"] = route;

         if (!ModelState.IsValid)
         {
            return View("LoginPage",
               new LoginRegisterViewModel { Login = loginModel });
         }


         var result = await _authRepository.Login(loginModel);


         if (result.IsT0)
         {
            UserViewModel userViewModel = result.AsT0;

            if (loginModel.RememberMe)
            {
               string refreshToken = _tokenGenerator.CreateRefreshToken();

               await _authRepository.SaveRefreshToken(userViewModel.Id, refreshToken);

               await HttpContext.SetCookie(userViewModel, refreshToken);
            }
            else
            {
               await HttpContext.SetCookie(userViewModel);
            }

            if (route is not null && !route.StartsWith("/auth/login", StringComparison.OrdinalIgnoreCase))
            {
               return Redirect(route);
            }

            return RedirectToAction("Index", "Home");
         }


         AppError appError = result.AsT1;

         TempData[nameof(AppError.Errors)] = appError.Errors;


         return
            View("LoginPage", new LoginRegisterViewModel { Login = loginModel });

      }



      [Authorize, HttpGet, HttpPost]
      public async Task<IActionResult> Logout()
      {
         HttpContext.Response.Cookies.Delete(Consts.RefreshToken, CookieOpts);
         HttpContext.Response.Cookies.Delete(Consts.UserId, CookieOpts);
         HttpContext.Response.Cookies.Delete(Consts.IsAuthenticatedCookie);

         string? userId = User.Claims.FirstOrDefault(x => x.Type == Consts.UserId)?.Value;

         if (userId is null) return RedirectToAction("LoginPage", "Account");

         await _authRepository.RemoveRefreshToken(userId);

         await HttpContext.SignOutAsync();

         return RedirectToAction("LoginPage", "Account");
      }



      [HttpPost, HttpGet]
      public async Task<IActionResult> Refresh(string? route = null)
      {
         string refreshToken = Request.Cookies[Consts.RefreshToken] ?? string.Empty;
         string userId = Request.Cookies[Consts.UserId] ?? string.Empty;


         if (refreshToken.Length < 45 || userId.Length < 33)
         {
            if (User?.Identity?.IsAuthenticated ?? false) await HttpContext.SignOutAsync();

            return RedirectToAction("LoginPage", "Account");
         }


         UserViewModel? user = await _authRepository.ValidateRefreshToken(userId, refreshToken);


         if (user is null)
         {
            TempData[nameof(AppError.Errors)] = new List<string>
            {
               "Não foi possivel renovar a sessão",
            };

            HttpContext.Response.Cookies.Delete(Consts.RefreshToken, CookieOpts);
            HttpContext.Response.Cookies.Delete(Consts.UserId, CookieOpts);
            HttpContext.Response.Cookies.Delete(Consts.IsAuthenticatedCookie);

            if (User?.Identity?.IsAuthenticated ?? false) await HttpContext.SignOutAsync();

            return View("LoginPage");
         }


         string newRefreshToken = _tokenGenerator.CreateRefreshToken();

         await _authRepository.SaveRefreshToken(userId, newRefreshToken);

         await HttpContext.SetCookie(user, newRefreshToken);

         if (route is not null && !route.StartsWith("/auth/refresh", StringComparison.OrdinalIgnoreCase))
         {
            return Redirect(route);
         }

         return RedirectToAction("Index", "Home");
      }



      [HttpGet]
      public IActionResult AccessDenied() => View();



   }
}
