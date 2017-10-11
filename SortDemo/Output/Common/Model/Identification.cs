using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class Identification
    {
        public Person Person { get; set; }
        public IdentifyResult IdentifyResult { get; set; }
        public Face Face { get; set; }

        public Identification(string name, IdentifyResult identifyResult, Face face)
        {
            Person = new Person();
            Person.Name = name;
            IdentifyResult = identifyResult;
            Face = face;
        }

        public Identification(Person person, IdentifyResult identifyResult, Face face)
        {
            Person = person;
            IdentifyResult = identifyResult;
            Face = face;
        }
    }
}
