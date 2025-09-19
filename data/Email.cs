using System.Text.RegularExpressions;

namespace WebApplicationMVC.data
{
   public static class Email
   {

      public static bool IsEmail(string email)
      {
         string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
         return Regex.IsMatch(email, pattern);
      }


   }
}
