using OneOf;
using WebApplicationMVC.Models;
using WebApplicationMVC.Models.request;

namespace WebApplicationMVC.repositories
{
   public interface IAccountRepository
   {
      public Task<OneOf<int, AppError>> CreateUser(UserModel user);



      public Task<IEnumerable<UserViewModel>> GetUsers();



      public Task<UserViewModel?> GetUser(string userId);



   }
}
