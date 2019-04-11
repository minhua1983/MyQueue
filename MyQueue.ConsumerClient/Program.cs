using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyQueue.Core;

namespace MyQueue.ConsumerClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Consumer consumer = new Consumer("localhost", 30000, "test");
            consumer.Connect();
            consumer.Receive(message =>
            {
                Console.WriteLine("接收到消息：" + message.Content);
            });
            consumer.Close();
        }
    }
}
