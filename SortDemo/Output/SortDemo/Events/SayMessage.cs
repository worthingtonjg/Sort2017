using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortDemo.Events
{
    public class SayMessage
    {
        public string Message { get; set; }

        public SayMessage(string  message)
        {
            Message = message;
        }
    }
}
