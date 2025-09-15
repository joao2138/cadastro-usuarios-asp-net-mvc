namespace WebApplicationMVC.Models
{
   public class UserViewModel
   {
      public required string Id { get; init; }

      public required string UserName { get; init; }

      public string? Email { get; init; }

      public DateTime CreatedAt { get; init; }

      public required string UserType { get; init; } 
   }
}
