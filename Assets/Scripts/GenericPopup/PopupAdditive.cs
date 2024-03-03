using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace GenericPopup
{
    public class PopupAdditive : MonoBehaviour
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected GameObject loading;

        protected readonly int exit = Animator.StringToHash("Exit");

        protected virtual void ClosePopup()
        {
            if (animator != null)
            {
                animator.SetBool(exit, true);
                StartCoroutine(CloseDelay(0.5f));
            }
            else
            {
                PopupHelpers.Close(gameObject.scene);
            }
        }

        protected virtual IEnumerator CloseDelay(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            PopupHelpers.Close(gameObject.scene);
        }

        protected virtual IEnumerator CloseDelay(float delayTime, UnityAction onClose)
        {
            yield return new WaitForSeconds(delayTime);
            onClose?.Invoke();
            PopupHelpers.Close(gameObject.scene);
        }
    }
}