namespace Game.UI
{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class LeftBarView : MonoBehaviour
    {
        [SerializeField]private GameObject _view;
        [SerializeField]private TMP_Text _valueLabel;

        public void Show()
        {
            _view.SetActive(true);
        }

        public void Hide()
        {
            _view.SetActive(false);
        }

        private IEnumerator DisplayCommand(string value)
        {
            _valueLabel.transform.parent.gameObject.SetActive(true);
            _valueLabel.text = value;
            yield return new WaitForSeconds(2.0f);
            _valueLabel.transform.parent.gameObject.SetActive(false);
        }
    }
}
