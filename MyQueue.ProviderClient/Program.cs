using MyQueue.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyQueue.ProviderClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Provider provider = new Provider("localhost", 20000, "test");
            provider.Connect();

            while (true)
            {
                string message = Console.ReadLine();
                provider.Send(message);
            }

            provider.Close();
        }
    }
}
