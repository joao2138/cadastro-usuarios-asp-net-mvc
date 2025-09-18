using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApplicationMVC.data.extensions
{
   public static class AddServiceAuthenticatorExtension
   {

      public static void AddServiceAuthenticator(this WebApplicationBuilder builder)
      {
         builder.Services.AddAuthentication(opt =>
         {
            opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

         })
         .AddCookie(options =>
         {
            options.LoginPath = "/account/loginpage";
            options.LogoutPath = "/auth/logout";
            options.AccessDeniedPath = "/auth/accessDenied";
            options.SlidingExpiration = false;


            options.Events = new CookieAuthenticationEvents
            {
               OnRedirectToLogin = context =>
               {
                  if (context.Request.Cookies[Consts.IsAuthenticatedCookie] == "true")
                  {
                     context.Response.Redirect($"/auth/refresh?route={context.Request.Path.Value ?? ""}");
                  }
                  else
                  {
                     context.Response.Redirect($"/account/loginpage?route={context.Request.Path.Value ?? ""}");
                  }

                  return Task.CompletedTask;

               },
               //OnRedirectToAccessDenied = context =>
               //{

               //   context.Response.Redirect("/user/accessDenied");
               //   return Task.CompletedTask;
               //},



            };

         });


         //var Key = Encoding.ASCII.GetBytes(builder.Configuration["Auth:key"]!);

         //.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, JwtBearerConfig.DefautOptions(Key))


      }
   }
}
