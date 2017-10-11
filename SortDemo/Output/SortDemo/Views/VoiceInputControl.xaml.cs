using Caliburn.Micro;
using Common;
using Common.Model;
using Newtonsoft.Json;
using SortDemo.Events;
using SortDemo.UnityInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SortDemo.Views
{
    public sealed partial class VoiceInputControl : UserControl, IHandle<SayMessage>
    {
        private bool _isBusy;
        
        private MediaCapture _captureMedia = new MediaCapture();
        private InMemoryRandomAccessStream _audioStream = new InMemoryRandomAccessStream();

        public VoiceInputControl()
        {
            this.InitializeComponent();
            EventBus.Instance.Subscribe(this);
        }

        private async void Image_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_isBusy) return;

            await StartRecording();
        }

        private async void Image_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isBusy) return;

            await StopRecordingAndSend();
        }

        private async Task StartRecording()
        {
            Question.Text = "";
            Answer.Text = "";
            Cleanup();

            // Initialize the media capture device to record audio
            _audioStream = new InMemoryRandomAccessStream();
            _captureMedia = new MediaCapture();
            var captureInitSettings = new MediaCaptureInitializationSettings();
            captureInitSettings.StreamingCaptureMode = StreamingCaptureMode.Audio;
            await _captureMedia.InitializeAsync(captureInitSettings);
            MediaEncodingProfile encodingProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Medium);

            // Start recording
            await _captureMedia.StartRecordToStreamAsync(encodingProfile, _audioStream);

            MicOn.Visibility = Visibility.Visible;
            MicOff.Visibility = Visibility.Collapsed;
        }

        private void Cleanup()
        {
            if (_audioStream != null)
            {
                _audioStream.Dispose();
                _audioStream = null;
            }

            if (_captureMedia != null)
            {
                _captureMedia.Dispose();
                _captureMedia = null;
            }
        }

        private async Task StopRecordingAndSend()
        {
            _isBusy = true;

            await _captureMedia.StopRecordAsync();

            try
            {
                MicOff.Visibility = Visibility.Collapsed;
                MicOn.Visibility = Visibility.Collapsed;
                Thinking.Visibility = Visibility.Visible;

                // Convert speecch to text
                SpeechToTextHelper _speechToTextHelper = new SpeechToTextHelper();
                byte[] bytes = await _speechToTextHelper.GetBytes(_audioStream);
                SpeechToTextResponse question = await _speechToTextHelper.Convert(bytes);

                Debug.WriteLine(JsonConvert.SerializeObject(question, Formatting.Indented));

                if(string.IsNullOrEmpty(question.DisplayText))
                {
                    await DoSayMessage(BotConstants.TroubleHearing);
                    return;
                }

                #region
                if (question.DisplayText.Trim() == BotConstants.WhatDoYouSay || question.DisplayText.Trim() == BotConstants.WhatDidYouSay || question.DisplayText.Trim() == BotConstants.WhatDoYouThink || question.DisplayText == BotConstants.WhatDidYouThink)
                {
                    question.DisplayText = BotConstants.WhatDoYouSee;
                }

                question.DisplayText = question.DisplayText.Replace(BotConstants.Say, BotConstants.See);
                #endregion

                Question.Text = question.DisplayText;

                // First Ask the Navigation bot
                // Ask the bot the question
                var botHelper = new BotHelper(EnumBot.NavigationAI);
                BotResponse response = await botHelper.Ask(question.DisplayText);
                EnumBotAction? action = response.GetAction();
                if(action != null)
                {
                    ProcessCommand(action.Value);
                }
                else
                {
                    if (response.answers.Count > 0 && response.answers[0].answer.Trim() != BotConstants.NoGoodMatch)
                    {
                        await DoSayMessage(response.answers[0].answer);
                    }
                    else
                    {
                        await GetAnswerFromCurrentContext(question);
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(BotConstants.StopRecordingAndSend + ex.Message);
                await dialog.ShowAsync();
            }
            finally
            {
                Thinking.Visibility = Visibility.Collapsed;
                MicOff.Visibility = Visibility.Visible;
                _isBusy = false;
            }
        }

        private async void ProcessCommand(EnumBotAction action)
        {
            var actions = new Dictionary<EnumBotAction, IBotAction>();

            actions[EnumBotAction.Clipboard] = new ImageFromClipboardAction();
            actions[EnumBotAction.AnalyzeImage] = new AnalyzeImageAction();
            actions[EnumBotAction.GotoPhotoUI] = new GotoTakePhotoPageAction();
            actions[EnumBotAction.GotoPersonIdentifierUI] = new GotoPersonIdentifierPageAction();
            actions[EnumBotAction.GotoAnalyzeImageUI] = new GotoAnalyzeImagePageAction();
            actions[EnumBotAction.GotoIdentifyCelebrityUI] = new GotoCelebrityIdentifierPageAction();
            actions[EnumBotAction.GotoIdentifyChurchLeaderUI] = new GotoChurchLeaderIdentifierPageAction();
            actions[EnumBotAction.GotoFaceRecognitionUI] = new GotoFaceRecognitionPageAction();

            if (actions.ContainsKey(action))
            {
                EventBus.Instance.PublishOnUIThread(actions[action]);
            }
            else
            {
                await DoSayMessage(BotConstants.InvalidAction);
            }
        }

        private async Task GetAnswerFromCurrentContext(SpeechToTextResponse question)
        {
            // Ask the bot the question
            var botHelper = new BotHelper(EnumBot.ArticlesOfFaith);
            BotResponse response = await botHelper.Ask(question.DisplayText);

            if (response.answers.Count > 0)
            {
                await DoSayMessage(response.answers[0].answer);
            }
        }

        public async void Handle(SayMessage message)
        {
            await DoSayMessage(message.Message);
        }

        private async Task DoSayMessage(string message)
        {
            #region
            if(message.Trim() == BotConstants.NoGoodMatch)
            {
                message = BotConstants.ResponsesLimited;
            }
            #endregion

            Answer.Text = message;

            // Convert the answer to speech
            TextToSpeechHelper textToSpeechHelper = new TextToSpeechHelper();
            var audioBytes = await textToSpeechHelper.Convert(message);

            // Tell unity to play the audio response
            UnityHelper unityHelper = new UnityHelper();
            unityHelper.PlayAudio(audioBytes);
        }
    }
}
