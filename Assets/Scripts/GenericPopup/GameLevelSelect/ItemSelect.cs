using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainScene.Element
{
    public class ItemSelect : MonoBehaviour
    {
        
        public void Initialize(int indexParam, UnityAction<int> callback)
        {
            var btn = gameObject.AddComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                callback?.Invoke(indexParam);
            });
        }
        
        
    }
}