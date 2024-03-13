using System.Collections.Generic;
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

        [Header("Game models")]
        [SerializeField] private List<GameObject> bgModel;
        public List<GameObject> BgModel => bgModel;
        public string PrivacyURL => privacyURL;
        public string RateURL => rateURL;
        public string BaseApiUrl => baseApiUrl;
        public string TosURL => tosURL;
    }
}