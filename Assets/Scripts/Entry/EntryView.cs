using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Entry
{
    public class EntryView : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI loadTxt;
        [SerializeField] private Transform bgContainer;

        public void CreateBg(GameObject model)
        {
            var obj = Instantiate(model, bgContainer);
            obj.transform.localScale = Vector3.one;
        }

        public void PlayLoading(float loadTime, UnityAction callBack)
        {
            slider.value = 0f;
            slider.DOValue(1f, loadTime).OnComplete(() => { callBack?.Invoke(); });
            StartCoroutine(CountUpToTarget());
        }

        private IEnumerator CountUpToTarget()
        {
            var value = 0;
            SetTextLoading(value);
            while (slider.value < 1f)
            {
                value = (int)(slider.value * 100);
                SetTextLoading(value);
                yield return null;
            }
        }

        private void SetTextLoading(float value)
        {
            loadTxt.text = "Loading... " + value + "%";
        }
    }
}