using System.Collections;
using System.Collections.Generic;
using Game.UI.Fade;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string _startValue;
    [SerializeField] private int _gameplaySceneIndex;
    [SerializeField] private FadeController _fadeController;

    private bool started;
    private void Start()
    {
        InputMic.OnRecognize += OnRecognize;
    }

    private void Update()
    {
        if(started)
            return;

        if(Input.GetKeyDown(KeyCode.Return))
        {
            OnStartGame();
            started = true;
        }
    }

    private void OnRecognize(string value)
    {
        if(value == _startValue)
            OnStartGame();
    }

    public void OnStartGame()
    {
        _fadeController.FadeIn(()=>{ 
            UnityEngine.SceneManagement.SceneManager.LoadScene(_gameplaySceneIndex);
        });
    }

    private void OnDestroy()
    {
        InputMic.OnRecognize -= OnRecognize;
    }
}
