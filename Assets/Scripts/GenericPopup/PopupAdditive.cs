using UnityEngine;
using Utilities;

namespace GenericPopup
{
    public class PopupAdditive : MonoBehaviour
    {

        public void ClosePopup()
        {
            PopupHelpers.Close(gameObject.scene);
        }
        
    }
}