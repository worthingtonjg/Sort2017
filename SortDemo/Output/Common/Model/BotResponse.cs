using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class BotResponse
    {
        public List<Answer> answers { get; set; }

        public EnumBotAction? GetAction()
        {
            EnumBotAction? result = null;

            if(answers != null && answers.Count > 0)
            {
                if(answers[0].answer.Contains("\"action\""))
                {
                    var action = JsonConvert.DeserializeObject<BotAction>(answers[0].answer);

                    result = action.Action;
                }
            }

            return result;
        }
    }

    public class Answer
    {
        public string answer { get; set; }
        public List<string> questions { get; set; }
        public double score { get; set; }
    }

    public class BotAction
    {
        public EnumBotAction Action { get; set; }
    }
}
