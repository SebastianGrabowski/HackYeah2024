using System.Collections;
using System.Collections.Generic;
using Game.UI.Fade;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private int _gameplaySceneIndex;
    [SerializeField] private FadeController _fadeController;

    public void OnStartGame()
    {
        _fadeController.FadeIn(()=>{ 
            UnityEngine.SceneManagement.SceneManager.LoadScene(_gameplaySceneIndex);
        });
    }
}
