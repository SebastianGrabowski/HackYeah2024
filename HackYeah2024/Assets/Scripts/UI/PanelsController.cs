namespace Game.UI
{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class PanelsController : MonoBehaviour
    {
        [SerializeField]private TopBarView _topBarView;
        [SerializeField]private RightBarView _rightBarView;
        [SerializeField]private LeftBarView _leftBarView;
        [SerializeField]private CenterBarView _centerBarView;
        [SerializeField]private GameOverView _gameOverView;
        [SerializeField]private LemursFreeView _lemursFreeView;

        private float _gameOverDelay = 1f;

        private IEnumerator Start()
        {
            while(PlayerController.Instance == null)
                yield return null;

            _topBarView.Show();
            _rightBarView.Show();
            _leftBarView.Show();
            _centerBarView.Show();
        }

        public IEnumerator SetGameOver()
        {
            yield return new WaitForSeconds(_gameOverDelay);
            
            _topBarView.stop = true;
            _gameOverView.Show();
        }

        public void SetFreeView(bool active)
        {
            if(active) _lemursFreeView.Show();
            else _lemursFreeView.Hide();
        }
    }
}
