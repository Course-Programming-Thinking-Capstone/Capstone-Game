using System.Collections.Generic;
using UnityEngine;

namespace GameScene.Component
{
    public class RandomSpriter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteHolder;

        [SerializeField] private List<Sprite> randomList;

        private void Start()
        {
            if (randomList.Count == 0)
            {
                return;
            }

            spriteHolder.sprite = randomList[Random.Range(0, randomList.Count)];
        }
    }
}