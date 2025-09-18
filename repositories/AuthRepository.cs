using System.Transactions;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using OneOf;
using WebApplicationMVC.data;
using WebApplicationMVC.Models;
using WebApplicationMVC.Models.request;

namespace WebApplicationMVC.repositories
{
   public class AuthRepository(AppDbProvider db) : IAuthRepository
   {
      private readonly AppDbProvider _db = db;



      public async Task<OneOf<UserViewModel, AppError>> Login(UserLoginModel userLogin)
      {
         using var conn = _db.NewConnection();

         await conn.OpenAsync();

         var isEmail = Email.IsEmail(userLogin.UserName);

         var column = isEmail ? "email" : "user_name";

         var sql =
         $"""
         SELECT id, password_hash
         FROM users
         WHERE {column} = @UserName
         """;


         var (id, passwordHash) =
            await conn.QueryFirstOrDefaultAsync<(string?, string?)>(sql, userLogin);


         if (passwordHash != null && id != null && PasswordHash.Compare(passwordHash, userLogin.Password))
         {
            var user = await GetUserById(id, conn);

            if (user is not null)
            {
               return user;
            }
         }

         return new AppError(["Usuário ou senha incorreto"]);


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




      public async Task SaveRefreshToken(
         string userId,
         string refreshtoken,
         DateTime? expires = null)
      {
         const string sql =
           """
            INSERT INTO refresh_tokens (user_id, expires, token)
            VALUES (@userId, @expires, @token)

            ON CONFLICT (user_id)
            DO UPDATE SET
                expires = excluded.expires,
                token = excluded.token;
         """;


         using var conn = _db.NewConnection();

         await conn.OpenAsync();

         var transaction = await conn.BeginTransactionAsync();


         var param = new
         {
            userId,
            expires = expires ?? Consts.GetCookieExpires(),
            token = refreshtoken,
         };

         int rows = 0;
         try
         {
            rows = await conn.ExecuteAsync(sql, param, transaction);

            if (rows == 1)
            {
               await transaction.CommitAsync();
               return;
            }

            await transaction.RollbackAsync();

         }
         catch (Exception e)
         {
            await transaction.RollbackAsync();

            Console.WriteLine("Exception SaveRefreshToken:\n");
            Console.WriteLine(e);
            return;
         }


         throw new InvalidOperationException($"{nameof(SaveRefreshToken)} retornou {rows} linhas afetadas, 1 linha era esperado");
      }



      public async Task RemoveRefreshToken(string userId)
      {
         const string sql =
            """
               DELETE FROM refresh_tokens
               WHERE user_id = @userId
         """;


         using var conn = _db.NewConnection();

         await conn.OpenAsync();

         var transaction = await conn.BeginTransactionAsync();

         try
         {
            await conn.ExecuteAsync(sql, new { userId }, transaction);

            await transaction.CommitAsync();
         }
         catch (Exception e)
         {
            await transaction.RollbackAsync();

            Console.WriteLine("Exception RemoveRefreshToken:");
            Console.WriteLine(e);
         }


         return;
      }




      public async Task<UserViewModel?> ValidateRefreshToken(string userId, string refreshToken)
      {
         const string sql =
         """
            SELECT 1 FROM refresh_tokens
            WHERE user_id = @userId 
            AND token = @refreshToken 
            AND expires > CURRENT_TIMESTAMP;
         """;


         using var conn = _db.NewConnection();

         await conn.OpenAsync();

         var isValid = await conn.QueryFirstOrDefaultAsync<bool>(sql, new { userId, refreshToken });

         if (isValid)
         {
            return await GetUserById(userId, conn);
         }

         return null;
      }











      //private async Task UpdateHash(string userId, string newHash, SqliteConnection conn)
      //{
      //   const string sql =
      //   """
      //   UPDATE users 
      //   SET password_hash = @newHash
      //   WHERE id = @userId
      //   """;

      //   var transaction = await conn.BeginTransactionAsync();

      //   var rows = await conn.ExecuteAsync(sql, new { newHash, userId }, transaction);
      //}




   }
}
