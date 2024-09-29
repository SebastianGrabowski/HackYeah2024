namespace Game.UI
{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class TopBarView : MonoBehaviour
    {
        [SerializeField]private float _delay = 0.5f;
        [SerializeField]private GameObject _view;
        [SerializeField]private TextMeshProUGUI _textMeshProUGUI;
        [SerializeField]private TextMeshProUGUI _textMeshProUGUIBg;
        [SerializeField]private TextMeshProUGUI _btextMeshProUGUI;
        [SerializeField]private TextMeshProUGUI _btextMeshProUGUIBg;

        public bool stop;

        string format = "Distance {0}m.";
        string format2 = "BEST Distance {0}m.";
        float _distance;
        float _timeToElapse;
        

        public void Show()
        {
            _view.SetActive(true);
        }

        public void Hide()
        {
            _view.SetActive(false);
        }
        
        private void Update()
        {
            if(stop)
            {
                _distance = 0f;
                return;
            }
                
            _distance += Time.deltaTime;
            if(Time.time > _timeToElapse)
            {
                
                _textMeshProUGUI.text = string.Format(format, _distance.ToString("F2"));
                _textMeshProUGUIBg.text = _textMeshProUGUI.text;
                
                var bestD = PlayerPrefs.GetFloat("BestD", 0.0f);
                if(_distance > bestD)
                {
                    bestD = _distance;
                    PlayerPrefs.SetFloat("BestD", _distance);
                }
                _btextMeshProUGUI.text = string.Format(format2, bestD.ToString("F2"));
                _btextMeshProUGUIBg.text = _btextMeshProUGUI.text;
                _btextMeshProUGUI.gameObject.SetActive(bestD > 0.0f);

                _timeToElapse = Time.time + _delay;
            }

        }

        private void Refresh()
        {
            
        }
    }
}
