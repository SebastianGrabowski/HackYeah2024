namespace Game.UI
{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class LeftBarView : MonoBehaviour
    {
        [SerializeField]private GameObject _view;

        private void Start()
        {
            Show();
        }

        public void Show()
        {
            _view.SetActive(true);
        }

        public void Hide()
        {
            _view.SetActive(false);
        }
    }
}
