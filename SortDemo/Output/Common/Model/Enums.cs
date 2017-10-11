using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public enum EnumGender
    {
        Female,
        Male
    }

    public enum EnumAudioOutputFormat
    {
        Raw8Khz8BitMonoMULaw,
        Raw16Khz16BitMonoPcm,
        Riff8Khz8BitMonoMULaw,
        Riff16Khz16BitMonoPcm,
        Ssml16Khz16BitMonoSilk,
        Ssml16Khz16BitMonoTts
    }

    public enum EnumBotAction
    {
        Clipboard = 1,
        GotoPhotoUI = 2,
        AnalyzeImage = 3,
        GotoPersonIdentifierUI = 4,
        GotoAnalyzeImageUI = 5,
        GotoIdentifyCelebrityUI = 6,
        GotoIdentifyChurchLeaderUI = 7,
        GotoFaceRecognitionUI = 8
    }
}
