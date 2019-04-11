using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MyQueue.Core;

namespace MyQueue.Server
{
    class Program
    {
        public static Dictionary<string, Channel> Channels { get; set; } = new Dictionary<string, Channel>();

        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                ProviderServerListener providerServerListener = new ProviderServerListener(20000);
                providerServerListener.Start();
                providerServerListener.Listen();
                providerServerListener.Stop();
            });


            Task.Run(() =>
            {
                ConsumerServerListener consumerServerListener = new ConsumerServerListener(30000);
                consumerServerListener.Start();
                consumerServerListener.Listen();
                consumerServerListener.Stop();
            });

            Console.ReadLine();
        }
    }
}
