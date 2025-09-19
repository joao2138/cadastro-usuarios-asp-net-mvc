using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace WebApplicationMVC.data.extensions
{
   public static class TempDataExtensions
   {
      public static void AddInfoMessage(this ITempDataDictionary tempData, string message)
      {
         IEnumerable<string>? messages = null;

         if (tempData.TryGetValue(Consts.InfoMessage, out object? value))
         {
            if (value is IEnumerable<string> enumerable)
            {
               messages = enumerable.Append(message);
            }

         }


         messages ??= [message];

         tempData[Consts.InfoMessage] = messages;
      }


      public static void AddInfoMessage(this ITempDataDictionary tempData, IEnumerable<string> message)
      {
         IEnumerable<string> messages;

         if (tempData.TryGetValue(Consts.InfoMessage, out object? value))
         {
            if (value is IEnumerable<string> enumerable)
            {
               messages = enumerable.Concat(message);
            }
            else
            {
               messages = message;
            }

         }
         else
         {
            messages = message;
         }

         tempData[Consts.InfoMessage] = messages;
      }




   }

}
