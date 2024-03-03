using MainScene.Data;
using UnityEngine;

namespace Entry
{
    public class EntryModel : MonoBehaviour
    {

        [Header("Game Service")]
        [SerializeField] private string tosURL = "url";
        [SerializeField] private string privacyURL = "url";
        [SerializeField] private string rateURL = "url";
        
        public string TOSURL => tosURL;
        public string PrivacyURL => privacyURL;
        public string RateURL => rateURL;
        
    }
}