using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Character/CharacterData", order = -998)]

    public class CharacterData : ScriptableObject
    {
        [SerializeField]
        [SerializedDictionary("Id", "Character Data")]
        private SerializedDictionary<int, CharacterDataDetail> dataValue;
        public SerializedDictionary<int, CharacterDataDetail> Data => dataValue;
    }

    [Serializable]
    public class CharacterDataDetail
    {
        public string charName;
        public Enums.RateType rateType;
        [TextArea(5,25)]
        public string charDetail;
        public int charPrice;
        public GameObject charModelUI;
    }
}