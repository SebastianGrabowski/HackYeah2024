namespace Game.UI
{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class RightBarView : MonoBehaviour
    {
        [SerializeField]private GameObject _view;
        [SerializeField]private TMP_Text _valueLabel;

        public void Show()
        {
            _view.SetActive(true);
            PlayerController.Instance.OnTeamChanged += Refresh;
            Refresh();
        }

        public void Hide()
        {
            _view.SetActive(false);
            PlayerController.Instance.OnTeamChanged -= Refresh;
        }

        private void Refresh()
        {
            if(PlayerController.Instance == null)
                return;
            _valueLabel.text = PlayerController.Instance.TeamCount.ToString();
        }
    }
}
