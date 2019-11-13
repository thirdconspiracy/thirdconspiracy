using System;

namespace tc.Base64.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("Please provide string");
            }

            try
            {
                var encodedResult = Encode.HexToBase64(args[0]);
                System.Console.WriteLine(encodedResult);
            }
            catch (ArgumentException e)
            {
                System.Console.WriteLine("String length is odd");
                
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
        }
    }
}
