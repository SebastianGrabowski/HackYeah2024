namespace Game.UI
{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using UnityEngine.UI;

    public class LeftBarView : MonoBehaviour
    {
        [SerializeField]private GameObject _view;
        [SerializeField]private TMP_Text _valueLabel;
        [SerializeField]private RectTransform[] _rects;

        public void Show()
        {
            _view.SetActive(true);
            InputMic.OnRecognize += DisplayCommand;
        }

        public void Hide()
        {
            _view.SetActive(false);
            InputMic.OnRecognize -= DisplayCommand;
        }

        public void DisplayCommand(string value)
        {
            StopAllCoroutines();
            StartCoroutine(DisplayCommandUpdate(value));
        }

        private IEnumerator DisplayCommandUpdate(string value)
        {
            _valueLabel.transform.parent.gameObject.SetActive(true);
            _valueLabel.text = value;
            foreach(var r in _rects)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(r);
            }
            yield return new WaitForSeconds(2.0f);
            _valueLabel.transform.parent.gameObject.SetActive(false);
        }
    }
}
