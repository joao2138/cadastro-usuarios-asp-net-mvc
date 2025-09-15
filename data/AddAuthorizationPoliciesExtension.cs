namespace WebApplicationMVC.data
{
   public static class AddAuthorizationPoliciesExtension
   {

      public static void AddAuthorizationPolicies(this IServiceCollection services)
      {
         services.AddAuthorizationBuilder()
            .AddPolicy("admin", policy => policy.RequireRole("admin"))

            .AddPolicy("user", policy => policy.RequireRole("user"))

            .AddPolicy("teste", policy => policy.RequireRole("teste"))
            ;
      }
   }
}
