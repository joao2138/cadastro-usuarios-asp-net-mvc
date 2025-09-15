namespace WebApplicationMVC.data
{
   public static class Consts
   {
      public const string RefreshToken = "auth.refresh";

      public const string UserId = "auth.userId";

      public const string UserName = "auth.userName";

      public const string CookiePath = "/auth/refresh";

      public const string IsAuthenticatedCookie = "auth.isAuthenticated";

      public static DateTime GetCookieExpires() => DateTime.UtcNow.AddDays(7);
   }
}
