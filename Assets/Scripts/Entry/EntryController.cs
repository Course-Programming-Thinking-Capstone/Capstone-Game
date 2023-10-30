using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Services;
using Audio;

namespace Entry
{
	public class EntryController : MonoBehaviour
	{
		private const string soundObjectName = "Sound";
		private const string UnUsed = "unused";
		[SerializeField] private EntryModel model;

		[SerializeField] private List<Sound> sounds;
		[SerializeField] private Music music;
		[SerializeField] private GameObject musicObject;

		[Space(8.0f)]
		[SerializeField] private float loadingTime = 3f;

		private bool isReady = false;
		private GameServices gameServices = null;

		void Awake()
		{
			if (GameObject.FindGameObjectWithTag(Constants.ServicesTag) == null)
			{
				GameObject gameServiceObject = new(nameof(GameServices))
				{
					tag = Constants.ServicesTag
				};
				gameServices = gameServiceObject.AddComponent<GameServices>();

				// Instantie Audio
				DontDestroyOnLoad(musicObject);

				GameObject soundObject = new(soundObjectName);
				DontDestroyOnLoad(soundObject);
				// Add Services
				gameServices.AddService(new AudioService(music, sounds, soundObject));
				gameServices.AddService(new APIService());
				gameServices.AddService(new DisplayService());
				gameServices.AddService(new InputService());
				gameServices.AddService(new PlayerService());
				gameServices.AddService(new GameService(model.TOSURL, model.PrivacyURL, model.RateURL));
	//			gameServices.AddService(new FirebaseService(OnFetchSuccess));
#if UNITY_ANDROID
				gameServices.AddService(new AdsService(model.BannerIdAndroid, model.IntersIdAndroid, model.RewardedIdAndroid, model.RewardedInterstitialIdAndroid, model.AOAIdAndroid));
				gameServices.AddService(new AppsFlyerService(model.AppsFlyerDevKey, model.AppFlyerAppIdAndroid));
#elif UNITY_IPHONE || UNITY_IOS
				gameServices.AddService(new AdsService(model.BannerIdIOS,model.IntersIdIOS,model.RewardedIdIOS,model.RewardedInterstitialIdIOS,model.AOAIdIOS));
				gameServices.AddService(new AppsFlyerService(model.AppsFlyerDevKey, model.AppsFlyerAppIdIos));
#else
			//	gameServices.AddService(new AdsService(UnUsed,UnUsed,UnUsed,UnUsed,UnUsed));
			//	gameServices.AddService(new AppsFlyerService(model.AppsFlyerDevKey, UnUsed));
#endif

				// Get services
				var displayService = gameServices.GetService<DisplayService>();
				var audioService = gameServices.GetService<AudioService>();
				var playerService = gameServices.GetService<PlayerService>();
			
				// --------------------------- Ads ---------------------------------
				// Setting ads from firebase
				//firebaseServices.OnLimitTimeAdsChanged = adsServices.SetLimitTimeShowAds;
				//firebaseServices.OnShowAppOpenAdChange = adsServices.OnShowAppOpenAdChange;
				// ------------------------------------------------------------------

				// --------------------------- Audio ---------------------------------
				// Set Volume
				playerService.OnMusicVolumeChange = audioService.SetMusicVolume;
				playerService.OnSoundVolumeChange = audioService.SetSoundVolume;

				playerService.OnVibrateChange = audioService.SetVibrate;

				audioService.MusicVolume = playerService.GetMusicVolume();
				audioService.SoundVolume = playerService.GetSoundVolume();

				audioService.VibrateOn = playerService.GetVibrate();

				audioService.MusicOn = true;
				audioService.SoundOn = true;

				audioService.StopMusic();
				playerService.LoadData();
				// ------------------------------------------------------------------
			}
		}
		private void Start()
		{
			Loading();
		}
		private void Loading()
		{
			StartCoroutine(Wait());
		}
		private IEnumerator Wait()
		{
			float timer = loadingTime * 0.1f;
			while (timer < loadingTime * 1.1f)
			{
				timer += Time.deltaTime;
				yield return null;
			}
			timer = 0;
			while (!isReady && timer <= loadingTime)
			{
				yield return null;
				timer += Time.deltaTime;
			}
			SceneManager.LoadScene(Constants.MainMenu);
		}
		private void OnFetchSuccess()
		{
			Time.timeScale = 1.0f;
			//gameServices.GetService<AdsService>().LoadAppOpenAd();
			isReady = true;
		}
	}
}
