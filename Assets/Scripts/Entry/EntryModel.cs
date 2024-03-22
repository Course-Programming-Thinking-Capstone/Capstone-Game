using System.Collections.Generic;
using UnityEngine;

namespace Entry
{
    public class EntryModel : MonoBehaviour
    {
        [Header("API")]
        [SerializeField] private string baseApiUrl = "url";
        [Header("Game Service")]
        [SerializeField] private string facebookPage = "url";
        [SerializeField] private string webPage = "url";

        [Header("Game models")]
        [SerializeField] private List<GameObject> bgModel;
        public List<GameObject> BgModel => bgModel;
        public string BaseApiUrl => baseApiUrl;

        public string FacebookPage => facebookPage;
        public string WebPage => webPage;
    }
}