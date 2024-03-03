using UnityEngine;

namespace Entry
{
    public class EntryModel : MonoBehaviour
    {
        [Header("API")]
        [SerializeField] private string baseApiUrl = "url";
        [Header("Game Service")]
        [SerializeField] private string tosURL = "url";
        [SerializeField] private string privacyURL = "url";
        [SerializeField] private string rateURL = "url";

        public string PrivacyURL => privacyURL;
        public string RateURL => rateURL;
        public string BaseApiUrl => baseApiUrl;
        public string TosURL => tosURL;
    }
}