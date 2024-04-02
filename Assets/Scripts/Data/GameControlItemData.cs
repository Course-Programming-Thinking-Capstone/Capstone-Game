using GameScene;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ControlItem", menuName = "GamePlay/GameControlItemData", order = -999)]
    public class GameControlItemData : ScriptableObject
    {
        [SerializeField] private SelectType itemType;
        [SerializeField] private Sprite unSelectRender;
        [SerializeField] private Sprite selectedRender;
        
        public SelectType ItemType => itemType;
        public Sprite UnSelectRender => unSelectRender;
        public Sprite SelectedRender => selectedRender;
    }
}