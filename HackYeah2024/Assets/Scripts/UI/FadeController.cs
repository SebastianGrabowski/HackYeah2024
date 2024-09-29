namespace Game.UI.Fade
{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
   // using Game.Audio;
   // using Game.Settings;

    public class FadeController : MonoBehaviour
    {
        [SerializeField]private UnityEngine.UI.RawImage _RI;
       // [SerializeField]private Data.SoundItemData _FadeSound;


        private void Start()
        {
            _RI.material.SetFloat("_Value", 1.0f);
            FadeOut(null);
        }

        public void FadeIn(UnityEngine.Events.UnityAction onFinish)
        {
            StopAllCoroutines();
            StartCoroutine(UpdateFade(1.0f, onFinish));
           // ServiceLocator.Container.Instance.Resolve<Audio.AudioProvider>().Sound2D.Play(_FadeSound);
        }

        public void FadeOut(UnityEngine.Events.UnityAction onFinish)
        {
            StopAllCoroutines();
            StartCoroutine(UpdateFade(0.0f, onFinish));
        }

        private IEnumerator UpdateFade(float targetValue, UnityEngine.Events.UnityAction onFinish)
        {
            _RI.enabled = true;
            var t = 0.0f;
            var maxt = 0.2f;
            var startValue = _RI.material.GetFloat("_Value");
            while(t < maxt)
            {
                t += Time.deltaTime;
                var v = Mathf.SmoothStep(startValue, targetValue, t/maxt);
                _RI.material.SetFloat("_Value", v);
                yield return null;
            }
            _RI.material.SetFloat("_Value", targetValue); 
            onFinish?.Invoke();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                FadeIn(()=>{ 
                    UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                    });
            }
        }
    }
}