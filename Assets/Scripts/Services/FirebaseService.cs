/*
using AppsFlyerSDK;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseService
{
	private const string Break = "~";

	// Key save data to player prefs
	private const string showAppOpenAdKey = "saoa";
	private const string limitedTimeAdsKey = "lta";

	// Name key from firebase
	private const string nameShowAppOpenAd = "show_app_open_ad";
	private const string nameLimitedTimeAds = "limited_time_ads";

	private const string nameLevelStart = "level_start";
	private const string nameLevelEnd = "level_end";
	private const string nameSkin = "skin";
	private const string nameCoin = "coin";
	private const string nameBooster = "booster";
	private const string nameTransactionComplete = "transaction_complete";

	// Key
	private const string user_idKey = "user_id";
	private const string max_stageKey = "max_stage";
	private const string user_dayKey = "user_day";

	private const string replayKey = "replay";

	private const string revivalKey = "revival";
	private const string successKey = "success";
	private const string resultKey = "result";
	private const string play_timeKey = "play_time";
	private const string max_streak_longKey = "max_streak_long";

	private const string stageKey = "stage";
	private const string amountKey = "amount";
	private const string typeKey = "type";
	private const string reasonKey = "reason";
	private const string play_orderKey = "play_order";

	private const string offer_nameKey = "offer_name";
	private const string offer_priceKey = "offer_price";
	private const string transaction_placementKey = "transaction_placement";
	private const string transaction_orderKey = "transaction_order";
	// Action
	public Action OnFetchSuccess;
	public Action<bool> OnShowAppOpenAdChange;
	public Action<int> OnLimitTimeAdsChanged;

	private FirebaseApp firebaseApp;

	// Cache
	private bool isShowAppOpenAd = false;
	private int lmtTimeAds = 70;
	public FirebaseService(Action onFetchSuccess)
	{
		OnFetchSuccess = onFetchSuccess;

		_ = InitFirebaseAsync();
	}
	private async Task InitFirebaseAsync()
	{
		await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
		{
			var dependencyStatus = task.Result;
			if (dependencyStatus == DependencyStatus.Available)
			{
#if UNITY_EDITOR
				firebaseApp = FirebaseApp.Create();
#else
				firebaseApp = FirebaseApp.DefaultInstance;
#endif
				InitRemoteConfig();
			}
			else
			{
				Logger.Error(
				  "Could not resolve all Firebase dependencies: " + dependencyStatus);
			}
		});

	}
	public void InitRemoteConfig()
	{
		// Set default values
		Dictionary<string, object> defaults = new()
		{
			{ nameShowAppOpenAd, false },
			{ nameLimitedTimeAds, 70 }
		};

		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			GetData();
			OnFetchSuccess?.Invoke();
			Logger.Debug("System is not connected to internet");
			return;
		}

		FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task =>
		{
			_ = FetchDataAsync();
		});

	}
	private void GetData()
	{
		// AOA
		int tempAOA = PlayerPrefs.GetInt(showAppOpenAdKey, 0);
		isShowAppOpenAd = tempAOA != 0;
		OnShowAppOpenAdChange?.Invoke(isShowAppOpenAd);

		// limit time ad
		lmtTimeAds = PlayerPrefs.GetInt(limitedTimeAdsKey, 70);
		OnLimitTimeAdsChanged?.Invoke(lmtTimeAds);
	}
	public Task FetchDataAsync()
	{
		Logger.Debug("Fetching data...");
#if UNITY_DEBUG
		Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
#else
		Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.FromHours(12.0));
#endif
		return fetchTask.ContinueWithOnMainThread(FetchComplete);
	}
	private void FetchComplete(Task fetchTask)
	{
		if (fetchTask.IsCanceled)
		{
			GetData();
			OnFetchSuccess?.Invoke();
			Logger.Debug("Fetching Remote Config values was cancelled.");
			return;
		}
		if (fetchTask.IsFaulted)
		{
			GetData();
			OnFetchSuccess?.Invoke();
			Logger.Debug("Fetching Remote Config values encountered an error: " + fetchTask.Exception);
			return;
		}
		if(fetchTask.IsCompletedSuccessfully == false)
		{
			GetData();
			OnFetchSuccess?.Invoke();
			Logger.Debug("Fetching Failed.");
			return;
		}

		var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
		var info = remoteConfig.Info;
		if (info.LastFetchStatus != LastFetchStatus.Success)
		{
			GetData();
			OnFetchSuccess?.Invoke();
			Logger.Error($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
			return;
		}

		remoteConfig.ActivateAsync()
		  .ContinueWithOnMainThread(
			task =>
			{
				Logger.Debug($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");

				// limit time ad
				lmtTimeAds = (int)remoteConfig.GetValue(nameLimitedTimeAds).LongValue;
				PlayerPrefs.SetInt(limitedTimeAdsKey, lmtTimeAds);
				OnLimitTimeAdsChanged?.Invoke(lmtTimeAds);

				// AOA
				isShowAppOpenAd = remoteConfig.GetValue(nameShowAppOpenAd).BooleanValue;
				PlayerPrefs.SetInt(showAppOpenAdKey, isShowAppOpenAd == true ? 1 : 0);
				OnShowAppOpenAdChange?.Invoke(isShowAppOpenAd);

				OnFetchSuccess?.Invoke();
			});
	}
	private List<T> GetListValue<T>(string value)
	{
		if (value == string.Empty)
			return new List<T>();

		string[] list = value.Split(Break);
		List<T> result = new();

		foreach (string s in list)
		{
			T item = (T)Convert.ChangeType(s, typeof(T));
			result.Add(item);
		}
		return result;
	}

	public void LevelStart(int score, int day, long replay)
	{
		Logger.Debug("Log event: "+ nameLevelStart);
		string id = SystemInfo.deviceUniqueIdentifier;
		var levelStart = new[]
			{
				new Parameter(user_idKey, id),
				new Parameter(max_stageKey, score),
				new Parameter(user_dayKey, day),
				new Parameter(replayKey, replay),
			};
		FirebaseAnalytics.LogEvent(nameLevelStart, levelStart);

		// Appsflyer
		Dictionary<string, string> levelStartAppsflyer = new Dictionary<string, string>();
		levelStartAppsflyer.Add(user_idKey, id);
		levelStartAppsflyer.Add(max_stageKey, score.ToString());
		levelStartAppsflyer.Add(user_dayKey, day.ToString());
		levelStartAppsflyer.Add(replayKey, replay.ToString());
		AppsFlyer.sendEvent(nameLevelStart, levelStartAppsflyer);
	}
	public void LevelEnd(int maxScore, int day, bool isRevival,long play_time, int score)
	{
		Logger.Debug("Log event: " + nameLevelEnd);

		string id = SystemInfo.deviceUniqueIdentifier;
		var levelStart = new[]
		{
			new Parameter(user_idKey, id),
			new Parameter(user_dayKey, maxScore),
			new Parameter(revivalKey, isRevival.ToString()),
			new Parameter(play_timeKey, play_time),
			new Parameter(user_dayKey, score)
		};
		FirebaseAnalytics.LogEvent(nameLevelEnd, levelStart);

		// Appsflyer
		Dictionary<string, string> levelEndAppsflyer = new Dictionary<string, string>();
		levelEndAppsflyer.Add(user_idKey, id);
		levelEndAppsflyer.Add(max_stageKey, maxScore.ToString());
		levelEndAppsflyer.Add(user_dayKey, day.ToString());
		levelEndAppsflyer.Add(revivalKey, isRevival.ToString());
		levelEndAppsflyer.Add(play_timeKey, play_time.ToString());
		levelEndAppsflyer.Add(max_streak_longKey, score.ToString());
		AppsFlyer.sendEvent(nameLevelEnd, levelEndAppsflyer);
	}
	public void ClaimSkin(long maxScore, int user_day, int type, int reason, long play_order)
	{
		Logger.Debug("Log event: " + nameSkin);

		string id = SystemInfo.deviceUniqueIdentifier;
		var skin = new[]
		{
			new Parameter(user_idKey, id),
			new Parameter(max_stageKey, maxScore),
			new Parameter(user_dayKey, user_day),
			new Parameter(typeKey, type),
			new Parameter(reasonKey, reason),
			new Parameter(play_orderKey, play_order)
		};
		FirebaseAnalytics.LogEvent(nameSkin, skin);

		// Appsflyer
		Dictionary<string, string> skinAppsflyer = new Dictionary<string, string>();
		skinAppsflyer.Add(user_idKey, id);
		skinAppsflyer.Add(max_stageKey, maxScore.ToString());
		skinAppsflyer.Add(user_dayKey, user_day.ToString());
		skinAppsflyer.Add(typeKey, type.ToString());
		skinAppsflyer.Add(reasonKey, reason.ToString());
		skinAppsflyer.Add(play_orderKey, play_order.ToString());
		AppsFlyer.sendEvent(nameSkin, skinAppsflyer);
	}
	public void Booster(int maxScore, int day, int type ,int amount,int reason )
	{
		Logger.Debug("Log event: " + nameBooster);

		string id = SystemInfo.deviceUniqueIdentifier;
		var booster = new[]
		{
			new Parameter(user_idKey, id),
			new Parameter(max_stageKey, maxScore),
			new Parameter(user_dayKey, day),
			new Parameter(amountKey, amount),
			new Parameter(typeKey, type),
			new Parameter(reasonKey, reason)
		};
		FirebaseAnalytics.LogEvent(nameBooster, booster);

		// Appsflyer
		Dictionary<string, string> boosterflyer = new Dictionary<string, string>();
		boosterflyer.Add(user_idKey, id);
		boosterflyer.Add(max_stageKey, maxScore.ToString());
		boosterflyer.Add(user_dayKey, day.ToString());
		boosterflyer.Add(amountKey, amount.ToString());
		boosterflyer.Add(typeKey, type.ToString());
		boosterflyer.Add(reasonKey, reason.ToString());
		AppsFlyer.sendEvent(nameBooster, boosterflyer);
	}
	public void TransactionComplete(long max_stage, int user_day, string offer_name, string offer_price, string transaction_placement, long transaction_order)
	{
		Logger.Debug("Log event: " + nameTransactionComplete);

		string id = SystemInfo.deviceUniqueIdentifier;
		var transaction_complete = new[]
		{
			new Parameter(user_idKey, id),
			new Parameter(max_stageKey, max_stage),
			new Parameter(user_dayKey, user_day),
			new Parameter(offer_nameKey, offer_name),
			new Parameter(offer_priceKey, offer_price),
			new Parameter(transaction_placementKey, transaction_placement),
			new Parameter(transaction_orderKey, transaction_order),
		};
		FirebaseAnalytics.LogEvent(nameTransactionComplete, transaction_complete);

		// Appsflyer
		Dictionary<string, string> transactionAppsflyer = new Dictionary<string, string>();
		transactionAppsflyer.Add(user_idKey, id);
		transactionAppsflyer.Add(max_stageKey, max_stage.ToString());
		transactionAppsflyer.Add(user_dayKey, user_day.ToString());
		transactionAppsflyer.Add(offer_nameKey, offer_name.ToString());
		transactionAppsflyer.Add(offer_priceKey, offer_price.ToString());
		transactionAppsflyer.Add(transaction_placementKey, transaction_placement.ToString());
		transactionAppsflyer.Add(transaction_orderKey, transaction_order.ToString());
		AppsFlyer.sendEvent(nameTransactionComplete, transactionAppsflyer);
	}
}
*/

