namespace Game.UI
{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class RightBarView : MonoBehaviour
    {
        [SerializeField] public float scaleMultiplier = 1.5f;
        [SerializeField] private float duration = 0.5f;
        [SerializeField]private GameObject _view;
        [SerializeField]private TMP_Text _valueLabel;

        string valueFormat = "x{0}";
        int prevAmount = 0;

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
            
            var teamCount = PlayerController.Instance.TeamCount;
            if(teamCount > prevAmount) StartCoroutine(OnCollectEffect());

            
            _valueLabel.text = string.Format(valueFormat, teamCount);
            prevAmount = teamCount;
        }

        private IEnumerator OnCollectEffect()
        {
            Vector3 originalScale = _valueLabel.transform.localScale;
            Vector3 targetScale = originalScale * scaleMultiplier;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                _valueLabel.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                _valueLabel.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _valueLabel.transform.localScale = originalScale;
        }

    }
}
