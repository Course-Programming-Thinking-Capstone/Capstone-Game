using UnityEngine.UI;

namespace GameScene.Component
{
    public class Arrow : Selector
    {
        private void Awake()
        {
            var button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(OnClickButton);
        }
    }
}
