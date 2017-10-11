using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class Voice
    {
        public string Name { get; set; }
        public string Locale { get; set; }

        public EnumGender Gender { get; set; }

        public string ServiceName { get; set; }

        public Voice(string name, string locale, EnumGender gender, string serviceName)
        {
            Name = name;
            Locale = locale;
            Gender = gender;
            ServiceName = serviceName;
        }

        public static List<Voice> GetVoices()
        {
            var voices = new List<Voice>();

            voices.Add(new Voice("Hayley", "en-AU", EnumGender.Female, "Microsoft Server Speech Text to Speech Voice (en-AU, HayleyRUS)"));
            voices.Add(new Voice("Linda", "en-CA", EnumGender.Female, "Microsoft Server Speech Text to Speech Voice (en-CA, Linda)"));
            voices.Add(new Voice("Heather", "en-CA", EnumGender.Female, "Microsoft Server Speech Text to Speech Voice (en-CA, HeatherRUS)"));
            voices.Add(new Voice("Susan", "en-GB", EnumGender.Female, "Microsoft Server Speech Text to Speech Voice (en-GB, Susan, Apollo)"));
            voices.Add(new Voice("Hazel", "en-GB", EnumGender.Female, "Microsoft Server Speech Text to Speech Voice (en-GB, HazelRUS)"));
            voices.Add(new Voice("George", "en-GB", EnumGender.Male, "Microsoft Server Speech Text to Speech Voice (en-GB, George, Apollo)"));
            voices.Add(new Voice("Shaun", "en-IE", EnumGender.Male, "Microsoft Server Speech Text to Speech Voice (en-IE, Shaun)"));
            voices.Add(new Voice("Heera", "en-IN", EnumGender.Female, "Microsoft Server Speech Text to Speech Voice (en-IN, Heera, Apollo)"));
            voices.Add(new Voice("Priya", "en-IN", EnumGender.Female, "Microsoft Server Speech Text to Speech Voice (en-IN, PriyaRUS)"));
            voices.Add(new Voice("Ravi", "en-IN", EnumGender.Male, "Microsoft Server Speech Text to Speech Voice (en-IN, Ravi, Apollo)"));
            voices.Add(new Voice("Zira", "en-US", EnumGender.Female, "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)"));
            voices.Add(new Voice("Jessa", "en-US", EnumGender.Female, "Microsoft Server Speech Text to Speech Voice (en-US, JessaRUS)"));
            voices.Add(new Voice("Benjamin", "en-US", EnumGender.Male, "Microsoft Server Speech Text to Speech Voice (en-US, BenjaminRUS)"));

            return voices;
        }
    }
}
