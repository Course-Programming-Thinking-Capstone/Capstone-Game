using UnityEngine;
using UnityEngine.Events;

namespace GenericPopup.Inventory
{
    public class InventItem : MonoBehaviour
    {
        public UnityAction onClick;


        public void OnClickThisItem()
        {
            onClick?.Invoke();
        }
    }
}