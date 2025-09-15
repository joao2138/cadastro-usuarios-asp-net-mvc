using Dapper;
using Microsoft.Data.Sqlite;
using OneOf;
using WebApplicationMVC.data;
using WebApplicationMVC.Models;

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

         var transaction = await conn.BeginTransactionAsync();

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


   }
}
