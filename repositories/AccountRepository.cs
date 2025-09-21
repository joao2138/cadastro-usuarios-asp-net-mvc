using System.Data;
using System.Transactions;
using Dapper;
using Microsoft.Data.Sqlite;
using OneOf;
using WebApplicationMVC.data;
using WebApplicationMVC.Models;
using WebApplicationMVC.Models.request;

namespace WebApplicationMVC.repositories
{
   public class AccountRepository(AppDbProvider db) : IAccountRepository
   {

      private readonly AppDbProvider _db = db;



      public async Task<OneOf<int, AppError>> CreateUser(UserModel user)
      {
         using var conn = _db.NewConnection();

         await conn.OpenAsync();

         const string sql =
         $"""
         INSERT INTO users 
            (id, user_name, email, password_hash, user_type, created_at)

         VALUES 
            (@Id, @UserName, @Email, @Password, @UserType, @CreatedAt)
         """;


         var param = new
         {
            Id = user.Id.ToString(),
            user.UserName,
            user.Email,
            Password = PasswordHash.Hash(user.Password),
            user.UserType,
            user.CreatedAt
         };

         using var transaction = await conn.BeginTransactionAsync();

         try
         {
            var rows = await conn.ExecuteAsync(sql, param, transaction);

            if (rows > 0)
            {
               await transaction.CommitAsync();
               return rows;
            }

            await transaction.RollbackAsync();

            return new AppError(["Ocorreu um erro ao cadastrar usuario"]);

         }
         catch (SqliteException e) when (e.SqliteErrorCode == 19)
         {
            await transaction.RollbackAsync();

            if (e.Message.Contains("email"))
            {
               return new AppError(["Este email já está em uso"]);
            }
            else if (e.Message.Contains("user_name"))
            {
               return new AppError(["Este nome de usuario já esta em uso"]);
            }

            Console.WriteLine("Erro sqlite CreateUser\n");
            Console.WriteLine(e);

            return new AppError(["Ocorreu um erro ao cadastrar o usuario"]);
         }
         catch (Exception e)
         {
            await transaction.RollbackAsync();

            Console.WriteLine("Exception CreateUser \n");
            Console.WriteLine(e);

            return new AppError(["Ocorreu um erro ao cadastrar usuario"]);
         }



      }




      public async Task<IEnumerable<UserViewModel>> GetUsers()
      {
         using var conn = _db.NewConnection();

         await conn.OpenAsync();

         const string sql =
         """
         SELECT
            id Id,
            user_name UserName,
            email Email,
            created_at CreatedAt,
            user_type UserType
         
         FROM users;
         """;

         return await conn.QueryAsync<UserViewModel>(sql);

      }



      public async Task<UserViewModel?> GetUserById(string userId)
      {
         using var conn = _db.NewConnection();

         await conn.OpenAsync();

         const string sql =
         """
         SELECT
            id Id,
            user_name UserName,
            email Email,
            created_at CreatedAt,
            user_type UserType
         
         FROM users
         WHERE id = @userId
         """;

         return await conn.QuerySingleOrDefaultAsync<UserViewModel>(sql, new { userId });
      }




      public async Task<UserViewModel?> GetUserById(string userId, SqliteConnection conn)
      {
         const string sql =
         """
         SELECT
            id Id,
            user_name UserName,
            email Email,
            created_at CreatedAt,
            user_type UserType
         
         FROM users
         WHERE id = @userId
         """;

         return await conn.QuerySingleOrDefaultAsync<UserViewModel>(sql, new { userId });
      }




      public async Task<OneOf<int, AppError>> UpdateUser(string userId, UpdateUserModel updateUser)
      {

         List<string> updateParts = new(3);

         if (!string.IsNullOrWhiteSpace(updateUser.UserName)) updateParts.Add("user_name = @UserName");
         if (updateUser.Email is not null) updateParts.Add("email = @Email");
         if (!string.IsNullOrWhiteSpace(updateUser.UserType)) updateParts.Add("user_type = @UserType");

         if (updateParts.Count == 0) return new AppError(["Pelo menos 1 campo deve ser preenchido"]);


         string sql = $"UPDATE users SET {string.Join(',', updateParts)} WHERE id = @userId";

         var param = new
         {
            userId,
            updateUser.UserName,
            updateUser.Email,
            updateUser.UserType,
         };


         using var conn = _db.NewConnection();

         await conn.OpenAsync();

         using var transaction = await conn.BeginTransactionAsync();

         try
         {
            int rows = await conn.ExecuteAsync(sql, param, transaction);

            if (rows > 0)
            {
               await transaction.CommitAsync();
               return rows;
            }

            await transaction.RollbackAsync();
            return new AppError(["Ocorreu um erro ao atualizar os dados do usuario"]);

         }
         catch (SqliteException e) when (e.SqliteErrorCode == 19)
         {
            await transaction.RollbackAsync();

            if (e.Message.Contains("email"))
            {
               return new AppError(["Este email já está em uso"]);
            }
            else if (e.Message.Contains("user_name"))
            {
               return new AppError(["Este nome de usuario já esta em uso"]);
            }

            Console.WriteLine("Erro sqlite UpdateUser\n");
            Console.WriteLine(e);

            return new AppError(["Ocorreu um erro ao atualizar os dados do usuario"]);


         }
         catch (Exception e)
         {
            await transaction.RollbackAsync();

            Console.WriteLine("exception UpdateUser:\n");
            Console.WriteLine(e);

            return new AppError(["Ocorreu um erro ao atualizar os dados do usuario"]);

         }


      }






      public async Task<OneOf<int, AppError>> UpdateUserPassword(string userId, UpdateUserPasswordModel passwordModel)
      {
         const string sql =
         """
         UPDATE users SET 
            password_hash = @newHash
               
         WHERE id = @userId;
         """;

         using var conn = _db.NewConnection();

         await conn.OpenAsync();

         using var transaction = await conn.BeginTransactionAsync();

         try
         {
            bool passwordIsCorrect =
                  await ComparePassword(userId, passwordModel.OldPassword, conn, transaction);

            if (!passwordIsCorrect)
            {
               return new AppError(["A antiga senha não confere"]);
            }


            var param = new
            {
               newHash = PasswordHash.Hash(passwordModel.NewPassword),
               userId
            };


            int rows = await conn.ExecuteAsync(sql, param, transaction);

            if (rows > 0)
            {
               await transaction.CommitAsync();
               return rows;
            }

            await transaction.RollbackAsync();

            return new AppError(["Ocorreu um erro ao atualizar a senha"]);

         }
         catch (Exception e)
         {
            await transaction.RollbackAsync();

            Console.WriteLine("exception UpdateUserPassword:\n");
            Console.WriteLine(e);

            return new AppError(["Ocorreu um erro ao atualizar a senha do usuario"]);

         }



      }





      public async Task<OneOf<int, AppError>> AdminDeleteUserAccount(
         string idToDelete,
         DeleteAccountModel deleteUser,
         string adminId)
      {
         using var conn = _db.NewConnection();

         await conn.OpenAsync();

         using var transaction = await conn.BeginTransactionAsync();

         try
         {

            if (!await ComparePassword(adminId, deleteUser.Password, conn, transaction))
            {
               return new AppError(["A senha do administrador está incorreta"]);
            }

            int rows = await DeleteUser(idToDelete, conn, transaction);

            if (rows == 1)
            {
               await transaction.CommitAsync();
               return rows;
            }


            await transaction.RollbackAsync();
            return new AppError(["Ocorreu um erro ao deletar a conta do usuário"]);

         }
         catch (Exception e)
         {
            Console.WriteLine($"Exception {nameof(AdminDeleteUserAccount)}\n");
            Console.WriteLine(e);

            return new AppError(["Ocorreu um erro ao deletar a conta do usuário"]);
         }


      }




      public async Task<OneOf<int, AppError>> DeleteUserAccount(string idToDelete, DeleteAccountModel deleteUser)
      {
         using var conn = _db.NewConnection();

         await conn.OpenAsync();

         using var transaction = await conn.BeginTransactionAsync();

         try
         {

            if (!await ComparePassword(idToDelete, deleteUser.Password, conn, transaction))
            {
               return new AppError(["A senha está incorreta"]);
            }

            int rows = await DeleteUser(idToDelete, conn, transaction);

            if (rows == 1)
            {
               await transaction.CommitAsync();
               return rows;
            }


            await transaction.RollbackAsync();
            return new AppError(["Ocorreu um erro ao deletar a conta do usuário"]);

         }
         catch (Exception e)
         {
            Console.WriteLine($"Exception {nameof(DeleteUserAccount)}\n");
            Console.WriteLine(e);

            return new AppError(["Ocorreu um erro ao deletar a conta do usuário"]);
         }


      }




      private static async Task<int> DeleteUser(
         string userId,
         IDbConnection conn,
         IDbTransaction transaction)
      {
         const string sql =
         """
           DELETE FROM users
           WHERE id = @userId
          """;

         return await conn.ExecuteAsync(sql, new { userId }, transaction);
      }



      private static async Task<bool> ComparePassword(
         string userId,
         string userPassword,
         IDbConnection conn,
         IDbTransaction? transaction = null)
      {
         var currentHash = await conn.QueryFirstOrDefaultAsync<string>(
            """
            SELECT password_hash FROM users
            WHERE id = @userId
            """,
            new { userId },
            transaction
         );


         if (currentHash is null || !PasswordHash.Compare(currentHash, userPassword))
         {
            return false;
         }

         return true;
      }


   }
}
