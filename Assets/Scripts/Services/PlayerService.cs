using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class PlayerService
    {
        private const char Break = '~';

        private const string MusicVolumeKey = "mvl";
        private const string SoundVolumeKey = "svl";
        private const string VibrateKey = "vbr";

        // All keys for save data in PlayerPrefs
        private const string HighScoreKey = "hsk";
        private const string PlayerItemsKey = "ptk";
        private const string PlayerPlayCount = "ppc";
        private const string PlayerWatchAdCount = "pwc";
        private const string PlayerSkinCount = "psc";
        private const string PlayerReviseCount = "prc";

        // Player Selected
        private const string PlayerSelectedBall = "psb";
        private const string PlayerSelectedWing = "psw";
        private const string PlayerSelectedHoop = "psh";
		private const string ReplayCountKey = "rpk";
		private const string MaxScoreKey = "msk";
        private const string PlayerSelectedFlame = "psf";

        // Properties
        public int LastScore { get; set; }
        public int HighScore { get; set; }
        public int PlayCount { get; set; }
        public int WatchAdCount { get; set; }
        public int SkinCount { get; set; }
        public int ResCount { get; set; }
        public List<int> PlayerOwnedItemsList { get; set; }
        public List<int> NewItemList { get; set; }

        public int SelectedBall { get; set; }
        public int SelectedWing { get; set; }
        public int SelectedHoop { get; set; }
        public int SelectedFlame { get; set; }

        public bool AdsAvailable => PlayCount > 2;

        /// <summary>
		/// Action for catch event when music volume change
		/// </summary>
		public Action<float> OnMusicVolumeChange;
        /// <summary>
        /// Action for catch event when sound volume change
        /// </summary>
        public Action<float> OnSoundVolumeChange;
        /// <summary>
        /// Action for catch event when vibrate change
        /// </summary>
        public Action<bool> OnVibrateChange;
        /// <summary>
        /// Get the music volume of game
        /// </summary>
        /// <returns>Music volume</returns>
        public float GetMusicVolume()
        {
            return PlayerPrefs.GetFloat(MusicVolumeKey, 1.0f);
        }
        /// <summary>
        /// Set the music volume of game
        /// </summary>
        /// <param name="volume"></param>
        public void SetMusicVolume(float volume)
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, volume);
            OnMusicVolumeChange?.Invoke(volume);
        }
        /// <summary>
        /// Get the sound volume of game
        /// </summary>
        /// <returns>Sound volume</returns>
        public float GetSoundVolume()
        {
            return PlayerPrefs.GetFloat(SoundVolumeKey, 1.0f);
        }
        /// <summary>
        /// Set the sound volume of game
        /// </summary>
        /// <param name="volume"></param>
        public void SetSoundVolume(float volume)
        {
            PlayerPrefs.SetFloat(SoundVolumeKey, volume);
            OnSoundVolumeChange?.Invoke(volume);
        }
        /// <summary>
        /// Get vibrate of game
        /// </summary>
        /// <returns>is vibrate or not</returns>
        public bool GetVibrate()
        {
            return PlayerPrefs.GetInt(VibrateKey, 1) == 0 ? false : true;
        }
        /// <summary>
        /// Set vibrate of game
        /// </summary>
        /// <param name="isVibrate"></param>
        public void SetVibrate(bool isVibrate)
        {
            OnVibrateChange?.Invoke(isVibrate);
            if (isVibrate == true)
            {
                PlayerPrefs.SetInt(VibrateKey, 1);
            }
            else
            {
                PlayerPrefs.SetInt(VibrateKey, 0);
            }
        }
        public int GetData(AwardedTriggerType awardedTriggerType)
        {
            switch (awardedTriggerType)
            {
                case AwardedTriggerType.PlayCount:
                    return PlayCount;
                case AwardedTriggerType.MaxScore:
                    return HighScore;
                case AwardedTriggerType.AdsCount:
                    return WatchAdCount;
                case AwardedTriggerType.SkinCount:
                    return SkinCount;
                case AwardedTriggerType.Revise:
                    return ResCount;
            }

            return 0;
        }
        public void UpdateSelectedSkin(int id, ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Ball:
                    SelectedBall = id;
                    PlayerPrefs.SetInt(PlayerSelectedBall, id);
                    break;
                case ItemType.Wing:
                    SelectedWing = id;
                    PlayerPrefs.SetInt(PlayerSelectedWing, id);
                    break;
                case ItemType.Hoop:
                    SelectedHoop = id;
                    PlayerPrefs.SetInt(PlayerSelectedHoop, id);
                    break;
                case ItemType.Flame:
                    SelectedFlame = id;
                    PlayerPrefs.SetInt(PlayerSelectedFlame, id);
                    break;
            }
        }

        public void LoadData()
        {
            LastScore = 0;
            PlayerOwnedItemsList = GetList(PlayerItemsKey, new List<int>() { 0, 50, 100, 200 });
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
            PlayCount = PlayerPrefs.GetInt(PlayerPlayCount, 0);
            WatchAdCount = PlayerPrefs.GetInt(PlayerWatchAdCount, 0);
            SkinCount = PlayerPrefs.GetInt(PlayerSkinCount, 0);
            ResCount = PlayerPrefs.GetInt(PlayerReviseCount, 0);
            SelectedBall = PlayerPrefs.GetInt(PlayerSelectedBall, 0);
            SelectedWing = PlayerPrefs.GetInt(PlayerSelectedWing, 50);
            SelectedHoop = PlayerPrefs.GetInt(PlayerSelectedHoop, 100);
            SelectedFlame = PlayerPrefs.GetInt(PlayerSelectedFlame, 200);
            NewItemList = new();
        }

        public void AddNewItem(int id)
        {
            PlayerOwnedItemsList.Add(id);
            SaveList(PlayerItemsKey, PlayerOwnedItemsList);
        }
        public void UpdateData(AwardedTriggerType updateName, int data = 0)
        {
            switch (updateName)
            {
                case AwardedTriggerType.PlayCount:
                    if (data != 0)
                    {
                        PlayCount += data;
                        PlayerPrefs.SetInt(PlayerPlayCount, PlayCount);
                        break;
                    }
                    PlayCount++;
                    PlayerPrefs.SetInt(PlayerPlayCount, PlayCount);
                    break;
                case AwardedTriggerType.MaxScore:
                    if (data > HighScore)
                    {
                        HighScore = data;
                        PlayerPrefs.SetInt(HighScoreKey, HighScore);
                    }

                    break;
                case AwardedTriggerType.AdsCount:
                    if (data != 0)
                    {
                        WatchAdCount += data;
                        PlayerPrefs.SetInt(PlayerPlayCount, WatchAdCount);
                        break;
                    }
                    WatchAdCount++;
                    PlayerPrefs.SetInt(PlayerWatchAdCount, WatchAdCount);
                    break;
                case AwardedTriggerType.SkinCount:
                    SkinCount = PlayerOwnedItemsList.Count;
                    PlayerPrefs.SetInt(PlayerSkinCount, SkinCount);
                    break;
                case AwardedTriggerType.Revise:
                    if (data != 0)
                    {
                        ResCount += data;
                        PlayerPrefs.SetInt(PlayerPlayCount, ResCount);
                        break;
                    }
                    ResCount++;
                    PlayerPrefs.SetInt(PlayerReviseCount, ResCount);
                    break;
            }
        }
        public void UpdateLastScore(int lastScore)
        {
            LastScore = lastScore;
        }

        #region Ultils method

        private void SaveList<T>(string key, List<T> value)
        {
            if (value == null)
            {
                Logger.Warning("Input list null");
                value = new List<T>();
            }

            if (value.Count == 0)
            {
                PlayerPrefs.SetString(key, string.Empty);
                return;
            }

            if (typeof(T) == typeof(string))
            {
                foreach (var item in value)
                {
                    string tempCompare = item.ToString();
                    if (tempCompare.Contains(Break))
                    {
                        throw new Exception("Invalid input. Input contain '~'.");
                    }
                }
            }

            PlayerPrefs.SetString(key, string.Join(Break, value));
        }


        /// <summary>
        /// Get list of value that saved
        /// </summary>
        /// <typeparam name="T">type of value</typeparam>
        /// <param name="key">name key of list value</param>
        /// <param name="defaultValue">default value if playerprefs doesn't have value</param>
        /// <returns></returns>
        private List<T> GetList<T>(string key, List<T> defaultValue)
        {
            if (PlayerPrefs.HasKey(key) == false)
            {
                return defaultValue;
            }

            if (PlayerPrefs.GetString(key) == string.Empty)
            {
                return new List<T>();
            }

            string temp = PlayerPrefs.GetString(key);
            string[] listTemp = temp.Split(Break);
            List<T> list = new List<T>();

            foreach (string s in listTemp)
            {
                list.Add((T)Convert.ChangeType(s, typeof(T)));
            }

            return list;
        }
        
        public int GetReplayCount()
        {
            PlayerPrefs.SetInt(ReplayCountKey, PlayerPrefs.GetInt(ReplayCountKey, -1) + 1);
            return PlayerPrefs.GetInt(ReplayCountKey);
        }
        public int GetReplayNotCount()
        {
            return PlayerPrefs.GetInt(ReplayCountKey,0);
        }
        public void CheckAndSetMaxScore(int score)
        {
            if (score > PlayerPrefs.GetInt(MaxScoreKey,0))
            {
                PlayerPrefs.SetInt(MaxScoreKey,score);
            }
        }
        public int GetMaxScore()
        {
            return PlayerPrefs.GetInt(MaxScoreKey, 0);
        }
        #endregion
    }

    [System.Serializable]
    public enum ItemType
    {
        Ball,
        Wing,
        Hoop,
        Flame
    }

    [System.Serializable]
    public enum AwardedTriggerType
    {
        PlayCount,
        MaxScore,
        AdsCount,
        SkinCount,
        Revise
    }
}