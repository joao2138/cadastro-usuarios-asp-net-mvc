using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using OneOf;
using WebApplicationMVC.Models;
using WebApplicationMVC.Models.request;

namespace WebApplicationMVC.repositories
{
   public interface IAuthRepository
   {

      public Task<OneOf<UserViewModel, AppError>> Login(UserLoginModel userLogin);

      public Task<UserViewModel?> GetUser(string userId, SqliteConnection conn);


      public Task SaveRefreshToken(
         string userId,
         string refreshtoken,
         DateTime? expires = null);


      public Task RemoveRefreshToken(string userId);


      public Task<UserViewModel?> ValidateRefreshToken(string userId, string refreshToken);
   }
}
