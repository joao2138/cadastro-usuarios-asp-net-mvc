using WebApplicationMVC.data;
using WebApplicationMVC.data.extensions;
using WebApplicationMVC.repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<AppDbProvider>();
builder.Services.AddSingleton<TokenGenerator>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddAuthorizationPolicies();
builder.AddServiceAuthenticator();

builder.Services.Configure<RouteOptions>(opt =>
{
   opt.LowercaseUrls = true;
});


var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
   app.UseExceptionHandler("/Home/Error");
}

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
