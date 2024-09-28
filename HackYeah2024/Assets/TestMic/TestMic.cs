using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class InputMic : MonoBehaviour
{
    protected DictationRecognizer _Recognizer;

    void Start()
    {
        StartDictationEngine();
    }

    private void DictationRecognizer_OnDictationHypothesis(string text)
    {
        Debug.Log("Dictation hypothesis: " + text);
    }

    private void DictationRecognizer_OnDictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log("Dictation result: " + text);
    }

    private void DictationRecognizer_OnDictationError(string error, int hresult)
    {
        Debug.Log("Dictation error: " + error);
    }


    private void DictationRecognizer_OnDictationComplete(DictationCompletionCause completionCause)
    {
        switch (completionCause)
        {
            case DictationCompletionCause.TimeoutExceeded:
            case DictationCompletionCause.PauseLimitExceeded:
            case DictationCompletionCause.Canceled:
            case DictationCompletionCause.Complete:
                CloseDictationEngine();
                StartDictationEngine();
                break;
            case DictationCompletionCause.UnknownError:
            case DictationCompletionCause.AudioQualityFailure:
            case DictationCompletionCause.MicrophoneUnavailable:
            case DictationCompletionCause.NetworkFailure:
                CloseDictationEngine();
                break;
        }
    }

    private void OnApplicationQuit()
    {
        CloseDictationEngine();
    }

    private void StartDictationEngine()
    {
        _Recognizer = new DictationRecognizer();
        _Recognizer.DictationHypothesis += DictationRecognizer_OnDictationHypothesis;
        _Recognizer.DictationResult += DictationRecognizer_OnDictationResult;
        _Recognizer.DictationComplete += DictationRecognizer_OnDictationComplete;
        _Recognizer.DictationError += DictationRecognizer_OnDictationError;
        _Recognizer.Start();
    }

    private void CloseDictationEngine()
    {
        if (_Recognizer != null)
        {
            _Recognizer.DictationHypothesis -= DictationRecognizer_OnDictationHypothesis;
            _Recognizer.DictationComplete -= DictationRecognizer_OnDictationComplete;
            _Recognizer.DictationResult -= DictationRecognizer_OnDictationResult;
            _Recognizer.DictationError -= DictationRecognizer_OnDictationError;
            if (_Recognizer.Status == SpeechSystemStatus.Running)
            {
                _Recognizer.Stop();
            }
            _Recognizer.Dispose();
        }
    }
}
