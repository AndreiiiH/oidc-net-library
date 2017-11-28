using ChaoticPixel.OIDC;
using System;

namespace DebugConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            OpenIDConnect oidc = new OpenIDConnect("chaotic-pixel.com");

            Console.ReadKey();
        }
    }
}
