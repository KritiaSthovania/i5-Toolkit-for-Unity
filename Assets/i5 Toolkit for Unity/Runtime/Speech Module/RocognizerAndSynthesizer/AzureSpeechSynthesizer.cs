using System.Threading.Tasks;
using UnityEngine;

#if I5_TOOLKIT_USE_AZURE_SPEECH_SYNTHESIZER
using Microsoft.CognitiveServices.Speech;
#endif

namespace i5.Toolkit.Core.SpeechModule
{
    /// <summary>
    /// A speech synthesizer (Text-To-Speech) using Azure Congitive Service. See https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/index-text-to-speech.
    /// Need subscribtion key and service region.
    /// </summary>
    public class AzureSpeechSynthesizer : MonoBehaviour, ISpeechSynthesizer
    {
        [Tooltip("You can find your subscription key on Azure Portal.")]
        [SerializeField] private string subscriptionKey;
        [Tooltip("You can find your service region on Azure Portal.")]
        [SerializeField] private string serviceRegion;
        [Tooltip("Specify the output form of the speech.")]
        [SerializeField] private AudioDataOutputForm outputForm;
#if I5_TOOLKIT_USE_AZURE_SPEECH_SYNTHESIZER
        private SpeechSynthesizer speechSynthesizer;
        private SpeechConfig speechConfig;
#endif

        /// <summary>
        /// Fires when the synthesis is complete.
        /// </summary>
        public event System.Action<SynthesisResult> OnSynthesisResultReceived;

        public Language Language { get; set; }

        /// <summary>
        /// Applicable if the component is enabled and there is an internet connection.
        /// </summary>
        public bool IsApplicable => enabled && Application.internetReachability != NetworkReachability.NotReachable;

        /// <summary>
        /// Whether directly output to speaker or as byte stream. Speech provider will convert a byte stream to an audio clip and play it via an audio source as spaital sound.
        /// </summary>
        public AudioDataOutputForm OutputForm { get => outputForm; set => outputForm = value; }

        void Start() {
#if I5_TOOLKIT_USE_AZURE_SPEECH_SYNTHESIZER
            speechConfig = SpeechConfig.FromSubscription(subscriptionKey, serviceRegion);
#else  
            Debug.LogError("The required Speech SDK for AzureSpeechRecognizer cannot be found, or you didn't define the I5_TOOLKIT_USE_AZURE_SPEECH_SYNTHESIZER directive on your platform.");
#endif
        }

        public async Task<SynthesisResult> StartSynthesizingAndSpeakingAsync(string text) {
#if I5_TOOLKIT_USE_AZURE_SPEECH_SYNTHESIZER
            // The language of the voice that speaks.
            switch (Language) {
                case Language.en_US:
                    speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";
                    break;
                case Language.de_DE:
                    speechConfig.SpeechSynthesisVoiceName = "de-DE-ConradNeural";
                    break;
                default:
                    speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";
                    break;
            }
            if (outputForm == AudioDataOutputForm.ToSpeaker) {
                //Directly speaking the synthesized audio
                speechSynthesizer = new SpeechSynthesizer(speechConfig);
            }
            else {
                //"Speak" the audio to a memory stream
                speechSynthesizer = new SpeechSynthesizer(speechConfig, null);
            }
            var speechSynthesisResult = await speechSynthesizer.SpeakTextAsync(text);
            SynthesisResult result =  ParseAzureSynthesisResult(speechSynthesisResult, text);
            OnSynthesisResultReceived?.Invoke(result);
            return result;
#else
            await Task.Run(() => Debug.LogError("The required Speech SDK for AzureSpeechRecognizer cannot be found, or the I5_TOOLKIT_USE_AZURE_SPEECH_SYNTHESIZER directive is not defined on current platform."));
            return SynthesisResult.RequiredModulesNotFoundResult;
#endif
        }
#if I5_TOOLKIT_USE_AZURE_SPEECH_SYNTHESIZER
        //Parse the SpeechSynthesisResult of Azure to our SynthesisResult.
        private SynthesisResult ParseAzureSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string inputText) {

            SynthesisResult result = new SynthesisResult();
            switch (speechSynthesisResult.Reason) {
                case ResultReason.SynthesizingAudioCompleted:
                    result.State = ResultState.Succeeded;
                    result.AudioData = speechSynthesisResult.AudioData;
                    result.Message = $"Speech synthesized for text: {inputText}";
                    break;
                case ResultReason.Canceled:
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                    result.State = ResultState.Failed;
                    result.Message = $"Failed: Reason: {cancellation.Reason}";
                    if (cancellation.Reason == CancellationReason.Error) {
                        result.Message += $" ErrorCode: {cancellation.ErrorCode}. Did you set the speech resource key and region values?";
                    }
                    break;
                default:
                    break;
            }
            Debug.Log(result.Message);
            return result;

        }
#endif
    }
}
