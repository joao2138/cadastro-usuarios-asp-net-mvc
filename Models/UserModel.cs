namespace WebApplicationMVC.Models
{
   public class UserModel
   {
      public Guid Id { get; init; }

      public required string UserName { get; init; }

      public string? Email { get; init; }

      public required string Password { get; init; }

      public DateTime CreatedAt { get; init; }

      public string UserType { get; init; } = "user";
   }
}
