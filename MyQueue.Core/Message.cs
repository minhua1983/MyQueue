using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyQueue.Core
{
    public class Message
    {
        public string ChannelName { get; set; } = "";
        public string Content { get; set; } = "";
    }
}
