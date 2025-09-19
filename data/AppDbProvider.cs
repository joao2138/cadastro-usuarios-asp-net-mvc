using Dapper;
using Microsoft.Data.Sqlite;

namespace WebApplicationMVC.data
{
   public class AppDbProvider
   {
      private string ConnectionString { get; }



      public AppDbProvider(IHostEnvironment host)
      {
         //var dbDirectory = Directory.GetCurrentDirectory();
         var dbDirectory = host.ContentRootPath;

         var dbPath = Path.Combine(dbDirectory, "app.db");

         ConnectionString = $"Data Source={dbPath}";


         if (!File.Exists(dbPath))
         {
            if (!Directory.Exists(dbDirectory)) Directory.CreateDirectory(dbDirectory);

            Console.WriteLine($"[Info] banco de dados criado em: {dbPath}");

            Initialize();
         }

      }


      public SqliteConnection NewConnection() => new(ConnectionString);



      private void Initialize()
      {
         using var connection = NewConnection();

         connection.Open();

         var transaction = connection.BeginTransaction();

         try
         {
            connection.Execute(
               """
                  CREATE TABLE IF NOT EXISTS `users` (
                  id                CHAR(36) NOT NULL PRIMARY KEY,
                  user_name         VARCHAR(80) COLLATE NOCASE NOT NULL UNIQUE,
                  email             VARCHAR(80) COLLATE NOCASE DEFAULT NULL UNIQUE,
                  password_hash     CHAR(60) NOT NULL,
                  created_at        DATETIME NOT NULL,
                  user_type         VARCHAR(10) DEFAULT 'user'
                  );

                  CREATE TABLE IF NOT EXISTS `refresh_tokens`(
                  user_id     CHAR(36) NOT NULL PRIMARY KEY,
                  expires     DATETIME NOT NULL,
                  token       CHAR(48) NOT NULL,

                  CONSTRAINT FK_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
                  );
                """,
               transaction: transaction
            );

            transaction.Commit();

         }
         catch 
         {
            transaction.Rollback();

            Console.WriteLine("Ocorreu um erro na criação do banco de dados!");
            throw;
         }


      }



   }
}
