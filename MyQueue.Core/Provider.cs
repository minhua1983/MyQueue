using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace MyQueue.Core
{
    public class Provider
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
        public Provider(string hostName, int port,string channelName)
        {
            _port = port;
            _hostName = hostName;
            _channelName = channelName;
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// 关闭client端
        /// </summary>
        public void Close()
        {
            _socket.Close();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="content"></param>
        public void Send(string content)
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

        /// <summary>
        /// 将当前client端连接到server端
        /// </summary>
        public void Connect()
        {
            _socket.Connect(_hostName, _port);
        }
    }
}
