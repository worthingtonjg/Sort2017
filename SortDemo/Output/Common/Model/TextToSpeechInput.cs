using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class TextToSpeechInput
    {
        public TextToSpeechInput()
        {
            Voice = Voice.GetVoices().FirstOrDefault(v => v.Name == "Hazel");
            OutputFormat = EnumAudioOutputFormat.Riff16Khz16BitMonoPcm;
        }

        public Voice Voice { get; set; }

        public EnumAudioOutputFormat OutputFormat { get; set; }

        public async Task<IEnumerable<KeyValuePair<string, string>>> GetHeaders()
        {
            var auth = await SpeechAuth.GetInstance();

            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("Content-Type", "application/ssml+xml"));

            string outputFormat;

            switch (this.OutputFormat)
            {
                case EnumAudioOutputFormat.Raw16Khz16BitMonoPcm:
                    outputFormat = "raw-16khz-16bit-mono-pcm";
                    break;
                case EnumAudioOutputFormat.Raw8Khz8BitMonoMULaw:
                    outputFormat = "raw-8khz-8bit-mono-mulaw";
                    break;
                case EnumAudioOutputFormat.Riff16Khz16BitMonoPcm:
                    outputFormat = "riff-16khz-16bit-mono-pcm";
                    break;
                case EnumAudioOutputFormat.Riff8Khz8BitMonoMULaw:
                    outputFormat = "riff-8khz-8bit-mono-mulaw";
                    break;
                case EnumAudioOutputFormat.Ssml16Khz16BitMonoSilk:
                    outputFormat = "ssml-16khz-16bit-mono-silk";
                    break;
                case EnumAudioOutputFormat.Ssml16Khz16BitMonoTts:
                    outputFormat = "ssml-16khz-16bit-mono-tts";
                    break;
                default:
                    outputFormat = "riff-16khz-16bit-mono-pcm";
                    break;
            }

            headers.Add(new KeyValuePair<string, string>("X-Microsoft-OutputFormat", outputFormat));
            headers.Add(new KeyValuePair<string, string>("Authorization", auth.GetAccessToken()));
            headers.Add(new KeyValuePair<string, string>("X-Search-AppId", "07D3234E49CE426DAA29772419F436CA"));
            headers.Add(new KeyValuePair<string, string>("X-Search-ClientID", "1ECFAE91408841A480F00935DC390960"));
            headers.Add(new KeyValuePair<string, string>("User-Agent", "TTSClient"));

            return headers;
        }
    }
}
