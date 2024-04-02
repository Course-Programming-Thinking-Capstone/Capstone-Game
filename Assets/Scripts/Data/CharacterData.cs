using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Character/CharacterData", order = -998)]

    public class CharacterDataScriptableObject : ScriptableObject
    {
        [SerializeField]
        [SerializedDictionary("Id", "Character Data")]
        private SerializedDictionary<int, CharacterData> dataValue;
        public SerializedDictionary<int, CharacterData> Data => dataValue;
    }

    [Serializable]
    public class CharacterData
    {
        public string charName;
        public string charDetail;
        public int charPrice;
        public GameObject charModelUI;
    }
}