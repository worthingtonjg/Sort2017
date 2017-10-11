using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class Bot
    {
        public Bot(string key, string uri)
        {
            Key = key;
            Uri = uri;
        }

        public string Key { get; set; }

        public string Uri { get; set; }
    }
}
