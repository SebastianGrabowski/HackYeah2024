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
        [SerializeField]private PunchData[] _punchData;
        
        [System.Serializable]
        public class PunchData
        {
            public string Key;
            public Transform T;
        }

        public void Show()
        {
            _view.SetActive(true);
            PlayerController.Instance.OnFormationChanged += RefreshFormation;
            InputMic.OnRecognize += DisplayCommand;
            RefreshFormation();
        }

        public void Hide()
        {
            _view.SetActive(false);
            PlayerController.Instance.OnFormationChanged -= RefreshFormation;
            InputMic.OnRecognize -= DisplayCommand;
        }

        private void OnDestroy()
        {
            PlayerController.Instance.OnFormationChanged -= RefreshFormation;
            InputMic.OnRecognize -= DisplayCommand;
        }

        private void DisplayCommand(string value)
        {
            for(var i = 0; i < _punchData.Length; i++)
            {
                if(_punchData[i].Key == value)
                {
                    StartCoroutine(Punch(_punchData[i].T));
                    break;
                }
            }
        }

        private IEnumerator Punch(Transform tr)
        {
            var t = 0.0f;
            var maxt = 0.3f;
            while(t < maxt)
            {
                t += Time.deltaTime;
                tr.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.3f, t/maxt);
                yield return null;
            }
            t = 0.0f;
            maxt = 0.15f;
            while(t < maxt)
            {
                t += Time.deltaTime;
                tr.transform.localScale = Vector3.Lerp(Vector3.one * 1.3f, Vector3.one, t/maxt);
                yield return null;
            }

            tr.transform.localScale = Vector3.one;
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
