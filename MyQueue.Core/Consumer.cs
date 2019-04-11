using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyQueue.Core
{
    public class Consumer
    {
        Socket _socket;
        int _port;
        string _hostName;
        string _channelName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="hostName">域名</param>
        /// <param name="port">端口</param>
        /// <param name="channelName">通道</param>
        public Consumer(string hostName, int port, string channelName)
        {
            _port = port;
            _hostName = hostName;
            _channelName = channelName;
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// 关闭当前client端
        /// </summary>
        public void Close()
        {
            _socket.Close();
        }

        /// <summary>
        /// 循环接收消息并调用委托来处理
        /// </summary>
        /// <param name="action"></param>
        public void Receive(Action<Message> action)
        {
            //Console.WriteLine("Receive1");
            while (true)
            {
                //Console.WriteLine("Receive2");
                int length = _socket.Available;

                byte[] data = new byte[length];
                _socket.Receive(data);

                string json = Encoding.UTF8.GetString(data);
                if (json != "")
                {
                    string[] messageContents =json.Split(new[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var messageContent in messageContents)
                    {
                        Message message = JsonConvert.DeserializeObject<Message>(messageContent);
                        action(message);
                        Console.WriteLine(messageContent);
                    }
                }
                //Console.WriteLine("Receive3");
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 将当前client端连接到server端
        /// </summary>
        public void Connect()
        {
            _socket.Connect(_hostName, _port);
            Send("cid_" + _channelName);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="content">消息内容</param>
        protected void Send(string content)
        {
            Message message = new Message()
            {
                ChannelName = _channelName,
                Content = content
            };
            string json = JsonConvert.SerializeObject(message);
            byte[] data = Encoding.UTF8.GetBytes(json);
            _socket.Send(data);
        }
    }
}
