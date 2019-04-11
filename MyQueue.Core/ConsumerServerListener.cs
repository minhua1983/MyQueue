using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyQueue.Core
{
    /// <summary>
    /// 消息接收的服务监听类
    /// </summary>
    public class ConsumerServerListener
    {
        TcpListener _tcpLisenter;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port">监听本机的端口号</param>
        public ConsumerServerListener(int port)
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
            Console.WriteLine("ConsumerServerListener已经启动...");
        }

        /// <summary>
        /// 监听
        /// </summary>
        public void Listen()
        {
            Console.WriteLine("ConsumerServerListener开始监听...");

            while (true)
            {
                //获取连接过来的tcp请求，这里的AcceptSocket方法是阻塞式的，必须用线程异步去处理，否则就没办法处理其他连接过来的tcp请求了。
                Socket consumerSocket = _tcpLisenter.AcceptSocket();
                Console.WriteLine("ConsumerServerListener监听到有ConsumerClient连接...");
                //将请求交给线程池来处理
                ThreadPool.QueueUserWorkItem(o =>
                {
                    //下面是处理当前tcp请求的逻辑，由于是tcp长连接请求，需要死循环不断监听推送过来的数据
                    Socket socket = (Socket)o;
                    string channelName = null;
                    while (true)
                    {
                        try
                        {
                            Channel channel = null;
                            //_channelName = "test";

                            //接收逻辑
                            int length = socket.Available;
                            if (channelName == null)
                            {
                                byte[] receiveData = new byte[length];
                                //开始接收数据，这里的Receive方法是阻塞式的
                                socket.Receive(receiveData);
                                if (receiveData.Length > 0)
                                {
                                    string json = Encoding.UTF8.GetString(receiveData);
                                    //将消息内容转Message
                                    Message receiveMessage = JsonConvert.DeserializeObject<Message>(json);
                                    channelName = receiveMessage.ChannelName;
                                    Console.WriteLine(json);
                                }
                            }


                            if (channelName != null)
                            {
                                //发送逻辑
                                if (!Channel.Dictionary.ContainsKey(channelName))
                                {
                                    //字典中不包含channelName则新建channel实例
                                    channel = new Channel();
                                    Channel.Dictionary.Add(channelName, channel);
                                }
                                else
                                {
                                    //字典中包含channelName则获取channel实例
                                    channel = Channel.Dictionary[channelName];
                                }
                                //从队列取出顶部Message，暂时不移除，因为可能会因客户端已断开连接而导致发送失败
                                Message sendMessage = channel.Peek();
                                if (sendMessage != null)
                                {
                                    StringBuilder stringBuilder = new StringBuilder();
                                    stringBuilder.Append(JsonConvert.SerializeObject(sendMessage));
                                    //添加消息分隔符，这样下游接收到多条记录可以用分隔符分割
                                    stringBuilder.Append("$$$");
                                    byte[] sendData = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                                    //发送消息（如果尝试发送给一个已经关闭的客户端，那么会报错）
                                    try
                                    {
                                        socket.Send(sendData);
                                        channel.Dequeue();
                                    }
                                    catch(Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                        break;
                                    }
                                

                                    //Console.WriteLine(stringBuilder.ToString());
                                }
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
                    Console.WriteLine("ConsumerServerListener监听到ConsumerClient已经关闭连接，因此ConsumerServerListener正在关闭当前Socket实例...");
                }, consumerSocket);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Stop()
        {
            _tcpLisenter.Stop();
            Console.WriteLine("ConsumerServerProvider已经关闭...");
        }
    }
}
