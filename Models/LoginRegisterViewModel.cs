using WebApplicationMVC.Models.request;

namespace WebApplicationMVC.Models
{
   public class LoginRegisterViewModel
   {
      public UserLoginModel? Login { get; set; } 
      public UserRegisterModel? Register { get; set; }
   }
}
