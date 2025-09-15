namespace WebApplicationMVC.Models
{
   public class AppError(IEnumerable<string> erros)
   {
      public IEnumerable<string> Errors { get; } = erros;

   }
}
