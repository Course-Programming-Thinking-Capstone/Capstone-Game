using UnityEngine;

namespace GameScene.GameBasic
{
    public class BasicModel : GameModel
    {
        [SerializeField] private GameObject roadToSelect;
        [SerializeField] private GameObject roadGroundPrefab;
        [SerializeField] private Sprite horizontalSprite;
        [SerializeField] private Sprite verticalSprite;
        [SerializeField] private Sprite turn1Sprite;
        [SerializeField] private Sprite turn2Sprite;
        [SerializeField] private Sprite turn3Sprite;
        [SerializeField] private Sprite turn4Sprite;
        public GameObject RoadToSelect => roadToSelect;
        public GameObject RoadGroundPrefab => roadGroundPrefab;

        // private float blockOffset;
        //
        // public override float GetBlockOffset()
        // {
        //     if (blockOffset == 0)
        //     {
        //         blockOffset = roadGroundPrefab.GetComponent<SpriteRenderer>().size.x *
        //                       roadGroundPrefab.transform.localScale.x;
        //     }
        //
        //     return blockOffset;
        // }

        public Sprite GetSprite(SelectType roadType)
        {
            switch (roadType)
            {
                case SelectType.RoadHorizontal:
                    return horizontalSprite;
                case SelectType.RoadVertical:
                    return verticalSprite;
                case SelectType.RoadTurn1:
                    return turn1Sprite;
                case SelectType.RoadTurn2:
                    return turn2Sprite;
                case SelectType.RoadTurn3:
                    return turn3Sprite;
                case SelectType.RoadTurn4:
                    return turn4Sprite;
            }

            return null;
        }
    }
}