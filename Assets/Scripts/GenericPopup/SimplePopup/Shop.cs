using AYellowpaper.SerializedCollections;
using Data;
using Services;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace GenericPopup.SimplePopup
{
    public class Shop : PopupAdditive
    {
        [SerializeField]
        [SerializedDictionary("RateType", "Sprite color")]
        private SerializedDictionary<Enums.RateType, Sprite> rateRender;
        [SerializeField] private Transform characterContainer;
        [SerializeField] private Image rateImage;
        [SerializeField] private TextMeshProUGUI rankTxt;
        [SerializeField] private TextMeshProUGUI characterNameTxt;
        [SerializeField] private TextMeshProUGUI characterDetailTxt;
        [SerializeField] private TextMeshProUGUI buyPriceTxt;
        [SerializeField] private GameObject buyButton;
        [SerializeField] private GameObject selectButton;
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI gemTxt;
        [SerializeField] private CharacterData shopData;

        private GameObject currentChar;
        private ClientService clientService;
        private int currentIndex = 0;
        private int maxIndex = 0;

        private void Awake()
        {
            clientService = GameServices.Instance.GetService<ClientService>();
        }

        private void Start()
        {
            coinTxt.text = clientService.Coin.ToString();
            gemTxt.text = clientService.Gem.ToString();
            maxIndex = shopData.Data.Count - 1;
            LoadCharacterData();
            loading.SetActive(false);
        }

        private void LoadCharacterData()
        {
            var character = shopData.Data[currentIndex];
            rateImage.sprite = rateRender[character.rateType];
            rankTxt.text = character.rateType.ToString();
            characterNameTxt.text = character.charName;
            characterDetailTxt.text = character.charDetail;
            buyPriceTxt.text = character.charPrice.ToString();
            if (currentChar != null)
            {
                SimplePool.Despawn(currentChar);
            }

            currentChar = SimplePool.Spawn(character.charModelUI, Vector3.zero, Quaternion.identity);
            currentChar.transform.SetParent(characterContainer);
            currentChar.transform.localScale = Vector3.one;
            currentChar.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "action/idle/normal", false);
        }

        public void OnClickSelect()
        {
        }

        public void OnClickBuy()
        {
        }

        public void OnClickPrevious()
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = maxIndex;
            }

            LoadCharacterData();
        }

        public void OnClickNext()
        {
            currentIndex++;
            if (currentIndex > maxIndex)
            {
                currentIndex = 0;
            }

            LoadCharacterData();
        }

        public void OnClickClose()
        {
            ClosePopup();
        }
    }
}