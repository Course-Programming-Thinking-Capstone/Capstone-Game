using MainScene.Data;
using UnityEngine;

namespace MainScene
{
    public class MainSceneModel : MonoBehaviour
    {
        [SerializeField] private StageData stageData;

        public StageData StageData => stageData;
    }
}