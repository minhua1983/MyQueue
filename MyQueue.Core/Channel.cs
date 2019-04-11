using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyQueue.Core
{
    public class Channel
    {
        /// <summary>
        /// 队列字典
        /// </summary>
        public static Dictionary<string, Channel> Dictionary { get; set; } = new Dictionary<string, Channel>();

        ConcurrentQueue<Message> concurrentQueue;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Channel()
        {
            //持久化队列
            concurrentQueue = new ConcurrentQueue<Message>();
        }

        /// <summary>
        /// 将消息放入队列最底部
        /// </summary>
        /// <param name="message">入列的消息</param>
        public void Enqueue(Message message)
        {
            concurrentQueue.Enqueue(message);
        }

        /// <summary>
        /// 将顶部的消息从队列中去除
        /// </summary>
        /// <returns>出列的消息</returns>
        public Message Dequeue()
        {
            Message message = null;
            concurrentQueue.TryDequeue(out message);
            return message;
        }

        /// <summary>
        /// 获取队列中最顶层的消息
        /// </summary>
        /// <returns>最顶层的消息</returns>
        public Message Peek()
        {
            Message message = null;
            concurrentQueue.TryPeek(out message);
            return message;
        }
    }
}
