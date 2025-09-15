namespace WebApplicationMVC.data
{
   public static class PasswordHash
   {
      public static readonly int workfactor = 10; // 2 ^ (10) = 1024 iterations.

      public static string Hash(string password)
      {

         string salt = BCrypt.Net.BCrypt.GenerateSalt(workfactor, 'b');
         string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);

         return hash;
      }

      public static bool Compare(string hash, string password)
         => BCrypt.Net.BCrypt.Verify(password, hash);




      public static bool NeedsReHash(string hash)
         => BCrypt.Net.BCrypt.PasswordNeedsRehash(hash, workfactor);

   }
}
