using OneOf;
using WebApplicationMVC.Models;
using WebApplicationMVC.Models.request;

namespace WebApplicationMVC.repositories
{
   public interface IAccountRepository
   {
      public Task<OneOf<int, AppError>> CreateUser(UserModel user);



      public Task<IEnumerable<UserViewModel>> GetUsers();



      public Task<UserViewModel?> GetUserById(string userId);



      public Task<OneOf<int, AppError>> UpdateUser(string userId, UpdateUserModel updateUser);



      public Task<OneOf<int, AppError>> UpdateUserPassword(string userId, UpdateUserPasswordModel passwordModel);



      public Task<OneOf<int, AppError>> AdminDeleteUserAccount(
       string idToDelete,
       DeleteAccountModel deleteUser,
       string adminId);



      public Task<OneOf<int, AppError>> DeleteUserAccount(string idToDelete, DeleteAccountModel deleteUser);

   }
}
