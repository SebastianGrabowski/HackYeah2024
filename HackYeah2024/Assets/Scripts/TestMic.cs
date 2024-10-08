using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class InputMic : MonoBehaviour
{
    public static Action<string> OnRecognize;
    [SerializeField] private PlayerController _playerController;

    protected KeywordRecognizer _Recognizer;
    public string[] keywords = new string[] {"up", "down", "left", "right"};
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;

    private void Start()
    {
        _Recognizer = new KeywordRecognizer(keywords, confidence);
        _Recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
        _Recognizer.Start();
    }

    private void OnDestroy()
    {
        _Recognizer.Dispose();
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log(args.text);

        if(_playerController)
            _playerController.Move(args.text);
            
        OnRecognize?.Invoke(args.text);
    }

    private void OnApplicationQuit()
    {
        if (_Recognizer != null && _Recognizer.IsRunning)
        {
            _Recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            _Recognizer.Stop();
        }
    }
}
