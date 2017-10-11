using Common.Model;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using static Windows.UI.Xaml.Media.Imaging.WriteableBitmapExtensions;

namespace Common
{
    public class FaceHelper
    {
        private readonly IFaceServiceClient faceServiceClient =
            new FaceServiceClient("a291f46e4c99492fbd1847d66f937c9f", "https://westus.api.cognitive.microsoft.com/face/v1.0");

        public async Task<Face[]> Detect(StorageFile file)
        {
            IEnumerable<FaceAttributeType> faceAttributes = new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Emotion, FaceAttributeType.Glasses, FaceAttributeType.Hair };

            using (var stream = await file.OpenStreamForReadAsync())
            {
                var faces = await faceServiceClient.DetectAsync(stream, returnFaceId: true, returnFaceLandmarks: false, returnFaceAttributes: faceAttributes);

                Debug.WriteLine(JsonConvert.SerializeObject(faces, Formatting.Indented));

                return faces;
            }
        }

        public async Task<WriteableBitmap> MarkFaces(StorageFile file, Face[] faces)
        {
            if (faces.Length == 0) return null;

            using (var stream = await file.OpenStreamForReadAsync())
            {
                WriteableBitmap wb = await BitmapFactory.FromStream(stream);
                using (wb.GetBitmapContext())
                {

                    for (int i = 0; i < faces.Length; ++i)
                    {
                        Face face = faces[i];

                        wb.DrawRectangle(
                            face.FaceRectangle.Left,
                            face.FaceRectangle.Top,
                            face.FaceRectangle.Left + face.FaceRectangle.Width,
                            face.FaceRectangle.Top + face.FaceRectangle.Height,
                            Colors.Red
                            );
                    }
                }

                return wb;
            }
        }

        public async Task<List<Identification>> Identify(string personGroupId, StorageFile file)
        {
            var result = new List<Identification>();

            using (var stream = await file.OpenStreamForReadAsync())
            {
                IEnumerable<FaceAttributeType> faceAttributes = new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Emotion, FaceAttributeType.Glasses, FaceAttributeType.Hair };

                var faces = await faceServiceClient.DetectAsync(stream, true, false, faceAttributes);
                var faceIds = faces.Select(face => face.FaceId).ToArray();

                if (faceIds.Length == 0)
                {
                    Debug.WriteLine("No Faces Found");
                    return result;
                }

                var identities = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);

                foreach (var identifyResult in identities)
                {
                    Debug.WriteLine("Result of face: {0}", identifyResult.FaceId);
                    if (identifyResult.Candidates.Length == 0)
                    {
                        result.Add(new Identification("UnKnown", identifyResult, faces.Where(f => f.FaceId == identifyResult.FaceId).FirstOrDefault()));
                    }
                    else
                    {
                        // Get top 1 among all candidates returned
                        var candidateId = identifyResult.Candidates[0].PersonId;

                        var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);

                        result.Add(new Identification(person, identifyResult, faces.Where(f => f.FaceId == identifyResult.FaceId).FirstOrDefault()));
                    }
                }
            }

            return result;
        }

        public async Task<PersonGroup[]> ListGroups()
        {
            PersonGroup[] groups = await faceServiceClient.ListPersonGroupsAsync();

            return groups;
        }

        public async Task CreatePersonGroup(string personGroupId, string personGroupName)
        {
            await faceServiceClient.CreatePersonGroupAsync(personGroupId, personGroupName);
        }

        public async Task DeletePersonGroup(string personGroupId)
        {
            await faceServiceClient.DeletePersonGroupAsync(personGroupId);
        }

        public async Task<Person[]> GetPersonsInGroup(string personGroupId)
        {
            Person[] persons = await faceServiceClient.ListPersonsAsync(personGroupId);

            return persons;
        }

        public async Task<CreatePersonResult> AddPerson(string personGroupId, string personName)
        {
            CreatePersonResult result = await faceServiceClient.CreatePersonAsync(personGroupId, personName);

            return result;
        }

        public async Task DeletePerson(string personGroupId, Guid personId)
        {
            await faceServiceClient.DeletePersonAsync(personGroupId, personId);
        }

        public async Task AddImageToPerson(string personGroupId, Guid personId, StorageFile file)
        {
            using (var s = await file.OpenStreamForReadAsync())
            {
                await faceServiceClient.AddPersonFaceAsync(personGroupId, personId, s);
            }
        }

        public async Task<Person> GetPerson(string personGroupId, Guid personId)
        {
            Person person = await faceServiceClient.GetPersonAsync(personGroupId, personId);

            return person;
        }

        public async Task TrainGroup(string personGroupId)
        {
            await faceServiceClient.TrainPersonGroupAsync(personGroupId);
        }

        public async Task<string> IsTrainingComplete(string personGroupId)
        {
            TrainingStatus status = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

            return status.Status.ToString();
        }

        public string DescribeImage(List<Identification> people)
        {
            if(people == null || people.Count == 0)
            {
                return "I do not see any faces.";
            }

            var unknownCount = people.Count(p => p.Person.Name.ToLower() == "unknown");
            var known = people.Where(p => p.Person.Name.ToLower() != "unknown").Select(p => p.Person.Name).ToList();

            string message = string.Empty;

            if (known.Count > 0)
            {
                if (known.Count == 1)
                {
                    message = $"I see {known[0]}.  ";
                }
                else
                {
                    message = $"I see {people.Count} faces.  ";
                    message += "I recognize: ";
                    foreach (var name in known)
                    {
                        message += name + ", ";
                    }
                }
            }

            if (unknownCount > 0)
            {
                if (known.Count > 0)
                {
                    message += " and ";
                }

                if (unknownCount == 1)
                {
                    message += $"I see 1 face I do not recognize";
                }
                else
                {
                    message += $"I see {unknownCount} faces I do not recognize";
                }
            }

            return message;
        }
    }
}
