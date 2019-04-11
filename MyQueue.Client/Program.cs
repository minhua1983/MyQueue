using MyQueue.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyQueue.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Provider provider = new Provider("localhost", 20000);
            provider.Connect("test");

            while (true)
            {
                string message = Console.ReadLine();
                provider.Send(message);
            }

            provider.Close();
        }
    }
}
