using UnityEngine;

namespace GameScene
{
    public class GameView  : MonoBehaviour
    {
        [SerializeField] private Transform selectorContainer;
        [SerializeField] private Transform selectedContainer;

        private void SetParentSelector(Transform child)
        {
            child.SetParent(selectorContainer);
        }
        
        private void SetParentSelected(Transform child)
        {
            child.SetParent(selectedContainer);
        }
    }
}