using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class Photo
    {
        public string PhotoName { get; set; }

        public string PersonName { get; set; }

        public Photo(string photo, string person)
        {
            PhotoName = photo;
            PersonName = person;
        }
    }
}
