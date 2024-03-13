using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Services;
using Audio;

namespace Entry
{
    public class EntryController : MonoBehaviour
    {
        private const string SoundObjectName = "Sound";
        [SerializeField] private EntryModel model;
        [SerializeField] private EntryView view;

        [SerializeField] private List<Sound> sounds;
        [SerializeField] private Music music;
        [SerializeField] private GameObject musicObject;

        [Space(8.0f)]
        [SerializeField] private float loadingTime = 3f;

        private GameServices gameServices = null;

        void Awake()
        {
            // Gen model
            view.CreateBg(model.BgModel[Random.Range(0, model.BgModel.Count)]);
            if (GameObject.FindGameObjectWithTag(Constants.ServicesTag) == null)
            {
                GameObject gameServiceObject = new(nameof(GameServices))
                {
                    tag = Constants.ServicesTag
                };
                gameServices = gameServiceObject.AddComponent<GameServices>();

                // Instantie Audio
                DontDestroyOnLoad(gameServiceObject);
                DontDestroyOnLoad(musicObject);

                GameObject soundObject = new(SoundObjectName);
                DontDestroyOnLoad(soundObject);
                // Add Services
                gameServices.AddService(new AudioService(music, sounds, soundObject));
                gameServices.AddService(new ServerSideService(model.BaseApiUrl));
                gameServices.AddService(new DisplayService());
                gameServices.AddService(new PlayerService());
                gameServices.AddService(new GameService(model.TosURL, model.PrivacyURL, model.RateURL));
            }
        }

        private void Start()
        {
            Loading();
        }

        private void Loading()
        {
            view.PlayLoading(loadingTime, () => SceneManager.LoadScene(Constants.MainMenu));
        }
    }
}