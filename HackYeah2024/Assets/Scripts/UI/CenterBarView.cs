namespace Game.UI
{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class CenterBarView : MonoBehaviour
    {
        [SerializeField]private GameObject _view;
        [SerializeField]private Image[] _formationBg;
        [SerializeField]private Color _activeFormationColor;
        [SerializeField]private Color _inactiveFormationColor;

        public void Show()
        {
            _view.SetActive(true);
            PlayerController.Instance.OnFormationChanged += RefreshFormation;
            RefreshFormation();
        }

        public void Hide()
        {
            _view.SetActive(false);
            PlayerController.Instance.OnFormationChanged -= RefreshFormation;
        }

        private void OnDestroy()
        {
            PlayerController.Instance.OnFormationChanged -= RefreshFormation;
        }

        private void RefreshFormation()
        {
            if(PlayerController.Instance == null)
                return;
            var active = PlayerController.Instance.LastFormation;
            for(var i = 0; i < _formationBg.Length; i++)
            {
                _formationBg[i].color = active == i ? _activeFormationColor : _inactiveFormationColor;
            }
        }
    }
}
