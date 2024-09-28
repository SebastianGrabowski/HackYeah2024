namespace Game.UI
{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class PanelsController : MonoBehaviour
    {
        [SerializeField]private RightBarView _rightBarView;
        [SerializeField]private LeftBarView _leftBarView;
        [SerializeField]private CenterBarView _centerBarView;

        private IEnumerator Start()
        {
            while(PlayerController.Instance == null)
                yield return null;
            _rightBarView.Show();
            _leftBarView.Show();
            _centerBarView.Show();
        }
    }
}
