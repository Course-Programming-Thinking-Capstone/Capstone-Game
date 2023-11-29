using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class PlayerService
    {
        private const char Break = '~';
        private const string UserCoinKey = "uck";
        private const string UserDiamondKey = "udk";

        public int UserCoin { get; set; }
        public int UserDiamond { get; set; }

        public void LoadData()
        {
            UserCoin = PlayerPrefs.GetInt(UserCoinKey, 0);
            UserDiamond = PlayerPrefs.GetInt(UserDiamondKey, 0);
        }

        public void SaveData()
        {
            PlayerPrefs.SetInt(UserCoinKey, UserCoin);
            PlayerPrefs.SetInt(UserDiamondKey, UserDiamond);
        }

        //       // All keys for save data in PlayerPrefs
        //       private const string HighScoreKey = "hsk";
        //       private const string PlayerItemsKey = "ptk";
        //       private const string PlayerPlayCount = "ppc";
        //       private const string PlayerWatchAdCount = "pwc";
        //       private const string PlayerSkinCount = "psc";
        //       private const string PlayerReviseCount = "prc";
        //
        //       // Player Selected
        //       private const string PlayerSelectedBall = "psb";
        //       private const string PlayerSelectedWing = "psw";
        //       private const string PlayerSelectedHoop = "psh";
        // private const string ReplayCountKey = "rpk";
        // private const string MaxScoreKey = "msk";
        //       private const string PlayerSelectedFlame = "psf";

        // Properties
        // public int LastScore { get; set; }
        // public int HighScore { get; set; }
        // public int PlayCount { get; set; }
        // public int WatchAdCount { get; set; }
        // public int SkinCount { get; set; }
        // public int ResCount { get; set; }

        // public void UpdateSelectedSkin(int id, ItemType itemType)
        // {
        //     switch (itemType)
        //     {
        //         case ItemType.Ball:
        //             SelectedBall = id;
        //             PlayerPrefs.SetInt(PlayerSelectedBall, id);
        //             break;
        //         case ItemType.Wing:
        //             SelectedWing = id;
        //             PlayerPrefs.SetInt(PlayerSelectedWing, id);
        //             break;
        //         case ItemType.Hoop:
        //             SelectedHoop = id;
        //             PlayerPrefs.SetInt(PlayerSelectedHoop, id);
        //             break;
        //         case ItemType.Flame:
        //             SelectedFlame = id;
        //             PlayerPrefs.SetInt(PlayerSelectedFlame, id);
        //             break;
        //     }
        // }
        //
        // public void LoadData()
        // {
        //     LastScore = 0;
        //     PlayerOwnedItemsList = GetList(PlayerItemsKey, new List<int>() { 0, 50, 100, 200 });
        //     HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        //     PlayCount = PlayerPrefs.GetInt(PlayerPlayCount, 0);
        //     WatchAdCount = PlayerPrefs.GetInt(PlayerWatchAdCount, 0);
        //     SkinCount = PlayerPrefs.GetInt(PlayerSkinCount, 0);
        //     ResCount = PlayerPrefs.GetInt(PlayerReviseCount, 0);
        //     SelectedBall = PlayerPrefs.GetInt(PlayerSelectedBall, 0);
        //     SelectedWing = PlayerPrefs.GetInt(PlayerSelectedWing, 50);
        //     SelectedHoop = PlayerPrefs.GetInt(PlayerSelectedHoop, 100);
        //     SelectedFlame = PlayerPrefs.GetInt(PlayerSelectedFlame, 200);
        //     NewItemList = new();
        // }
        //
        // public void AddNewItem(int id)
        // {
        //     PlayerOwnedItemsList.Add(id);
        //     SaveList(PlayerItemsKey, PlayerOwnedItemsList);
        // }
        // public void UpdateData(AwardedTriggerType updateName, int data = 0)
        // {
        //     switch (updateName)
        //     {
        //         case AwardedTriggerType.PlayCount:
        //             if (data != 0)
        //             {
        //                 PlayCount += data;
        //                 PlayerPrefs.SetInt(PlayerPlayCount, PlayCount);
        //                 break;
        //             }
        //             PlayCount++;
        //             PlayerPrefs.SetInt(PlayerPlayCount, PlayCount);
        //             break;
        //         case AwardedTriggerType.MaxScore:
        //             if (data > HighScore)
        //             {
        //                 HighScore = data;
        //                 PlayerPrefs.SetInt(HighScoreKey, HighScore);
        //             }
        //
        //             break;
        //         case AwardedTriggerType.AdsCount:
        //             if (data != 0)
        //             {
        //                 WatchAdCount += data;
        //                 PlayerPrefs.SetInt(PlayerPlayCount, WatchAdCount);
        //                 break;
        //             }
        //             WatchAdCount++;
        //             PlayerPrefs.SetInt(PlayerWatchAdCount, WatchAdCount);
        //             break;
        //         case AwardedTriggerType.SkinCount:
        //             SkinCount = PlayerOwnedItemsList.Count;
        //             PlayerPrefs.SetInt(PlayerSkinCount, SkinCount);
        //             break;
        //         case AwardedTriggerType.Revise:
        //             if (data != 0)
        //             {
        //                 ResCount += data;
        //                 PlayerPrefs.SetInt(PlayerPlayCount, ResCount);
        //                 break;
        //             }
        //             ResCount++;
        //             PlayerPrefs.SetInt(PlayerReviseCount, ResCount);
        //             break;
        //     }
        // }
        // public void UpdateLastScore(int lastScore)
        // {
        //     LastScore = lastScore;
        // }

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

        #endregion
    }
}