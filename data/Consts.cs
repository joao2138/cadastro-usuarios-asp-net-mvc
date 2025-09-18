namespace WebApplicationMVC.data
{
   public static class Consts
   {
      public const string RefreshToken = "auth.refresh";

      public const string UserId = "auth.userId";

      public const string UserName = "auth.userName";

      public const string CookiePath = "/auth/refresh";

      public const string IsAuthenticatedCookie = "auth.isAuthenticated";

      public const string InfoMessage = "info-message";

      public static DateTime GetCookieExpires() => DateTime.UtcNow.AddDays(7);


      public static readonly List<string> UserTypes = ["admin", "user"];
   }
}
