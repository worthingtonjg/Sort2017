using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class CategoryDetail
    {
        public List<Landmark> Landmarks { get; set; }

        public List<Celebrity> Celebrities { get; set; }

        public List<Caption> LandsmarksToCaptions()
        {
            var result = new List<Caption>();

            if (Landmarks != null)
            {
                result = Landmarks.Select(l => new Caption
                {
                    Text = l.Name,
                    Confidence = l.Confidence
                }).ToList();
            }

            return result;
        }

        public List<Caption> CelebritiesToCaptions()
        {
            var result = new List<Caption>();

            if (Celebrities != null)
            {
                result = Celebrities.Select(l => new Caption
                {
                    Text = l.Name,
                    Confidence = l.Confidence
                }).ToList();
            }

            return result;
        }
    }

    public class Celebrity
    {
        public string Name { get; set; }
        public FaceRectangle FaceRectangle { get; set; }
        public double Confidence { get; set; }
    }

    public class Landmark
    {
        public string Name { get; set; }
        
        public double Confidence { get; set; }

    }
}
