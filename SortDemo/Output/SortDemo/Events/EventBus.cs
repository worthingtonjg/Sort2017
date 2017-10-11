using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SortDemo.Events
{
    public class EventBus
    {
        private static EventAggregator _instance;

        public static EventAggregator Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new EventAggregator();
                }

                return _instance;
            }
        }
    }
}
