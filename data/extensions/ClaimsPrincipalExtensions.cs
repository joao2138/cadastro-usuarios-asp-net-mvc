using System.Security.Claims;

namespace WebApplicationMVC.data.extensions
{
   public static class ClaimsPrincipalExtensions
   {
      public static bool TryGetUserId(this ClaimsPrincipal user, out string userId)
      {

         userId = user?.Claims.FirstOrDefault(x => x.Type == Consts.UserId)?.Value ?? string.Empty;

         return userId != string.Empty;
      }
   }

}
