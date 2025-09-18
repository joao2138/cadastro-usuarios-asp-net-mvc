using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApplicationMVC.Models;

namespace WebApplicationMVC.data.extensions
{
   public static class SetCookieExtension
   {
      private static CookieOptions Options => new()
      {
         HttpOnly = true,
         Secure = false, //apenas para teste
         Path = Consts.CookiePath,
         Expires = Consts.GetCookieExpires(),
         SameSite = SameSiteMode.Strict
      };


      private static CookieOptions IsAuthenticatedCookie => new()
      {
         HttpOnly = false,
         Secure = false,
         Expires = Consts.GetCookieExpires(),
      };



      public static async Task SetCookie(
            this HttpContext context,
            UserModel user,
            string? refreshToken = null)
      {

         IEnumerable<Claim> claims = 
         [
            new(Consts.UserId ,user.Id.ToString()),
            new(Consts.UserName ,user.UserName),
            new(ClaimTypes.Role, user.UserType),
         ];

         var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
         var principal = new ClaimsPrincipal(identity);


         if (refreshToken != null)
         {
            context.Response.Cookies.Append(Consts.RefreshToken, refreshToken, Options);
            context.Response.Cookies.Append(Consts.UserId, user.Id.ToString(), Options);
            context.Response.Cookies.Append(Consts.IsAuthenticatedCookie, "true", IsAuthenticatedCookie);
         }

         await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
      }




      public static async Task SetCookie(
         this HttpContext context,
         UserViewModel user,
         string? refreshToken = null)
      {
         IEnumerable<Claim> claims = 
         [
            new(Consts.UserId ,user.Id),
            new(Consts.UserName ,user.UserName),
            new(ClaimTypes.Role, user.UserType),
         ];

         var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
         var principal = new ClaimsPrincipal(identity);


         if (refreshToken != null)
         {
            context.Response.Cookies.Append(Consts.RefreshToken, refreshToken, Options);
            context.Response.Cookies.Append(Consts.UserId, user.Id.ToString(), Options);
            context.Response.Cookies.Append(Consts.IsAuthenticatedCookie, "true", IsAuthenticatedCookie);
         }

         await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

      }




   }
}
