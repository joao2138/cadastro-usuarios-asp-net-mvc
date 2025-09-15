using System.Security.Cryptography;

namespace WebApplicationMVC.data
{
   public class TokenGenerator
   {
      private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();


      public string CreateRefreshToken()
      {
         var randomNumber = new byte[36];
         _rng.GetBytes(randomNumber);

         return Convert.ToBase64String(randomNumber);
      }









   }
}
