using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace MyQueue.Core
{
    /// <summary>
    /// 消息发送的服务监听类
    /// </summary>
    public class ProviderServerListener
    {
        TcpListener _tcpLisenter;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port">监听本机的端口号</param>
        public ProviderServerListener(int port)
        {
            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            //持久化TcpListener实例
            _tcpLisenter = new TcpListener(iPAddress, port);
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            _tcpLisenter.Start();
            Console.WriteLine("ProviderServerListener已经启动...");
        }

        /// <summary>
        /// 监听
        /// </summary>
        public void Listen()
        {
            Console.WriteLine("ProviderServerListener开始监听...");

            while (true)
            {
                //获取连接过来的tcp请求，这里的AcceptSocket方法是阻塞式的，必须用线程异步去处理，否则就没办法处理其他连接过来的tcp请求了。
                Socket providerSocket = (Socket)_tcpLisenter.AcceptSocket();
                Console.WriteLine("ProviderServerListener监听到有ProviderClient连接...");
                //将请求交给线程池来处理
                ThreadPool.QueueUserWorkItem(o =>
                {
                    //下面是处理当前tcp请求的逻辑，由于是tcp长连接请求，需要死循环不断监听推送过来的数据
                    Socket socket = (Socket)o;
                    while (true)
                    {
                        try
                        {
                            int length = socket.Available;

                            byte[] data = new byte[length];
                            //开始接收数据，这里的Receive方法是阻塞式的
                            socket.Receive(data);
                            if (data.Length > 0)
                            {
                                string json = Encoding.UTF8.GetString(data);
                                //将消息内容转Message
                                Message message = JsonConvert.DeserializeObject<Message>(json);
                                Channel channel = null;
                                if (!Channel.Dictionary.ContainsKey(message.ChannelName))
                                {
                                    //字典中不包含channelName则新建channel实例
                                    channel = new Channel();
                                    Channel.Dictionary.Add(message.ChannelName, channel);
                                }
                                else
                                {
                                    //字典中包含channelName则获取channel实例
                                    channel = Channel.Dictionary[message.ChannelName];
                                }
                                //将Message加入队列底部
                                channel.Enqueue(message);

                                Console.WriteLine(json);
                            }
                            Thread.Sleep(100);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            break;
                        }
                    }
                    socket.Close();
                    Console.WriteLine("ProviderServerListener监听到ProviderClient已经关闭连接，因此ProviderServerListener正在关闭当前Socket实例...");
                }, providerSocket);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Stop()
        {
            _tcpLisenter.Stop();
            Console.WriteLine("ProviderServerListener已经关闭...");
        }
    }
}
