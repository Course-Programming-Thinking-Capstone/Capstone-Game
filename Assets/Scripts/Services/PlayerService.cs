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
        private const string UserCurrentLevelKey = "uclk";
        private const string HistoryStarKey = "hsk";

        public int UserCoin { get; set; }
        public int UserDiamond { get; set; }
        public List<int> CurrentLevel { get; set; }

        public PlayerService()
        {
            LoadData();
        }

        public List<int> GetHistoryStar(int stateIndex)
        {
            return GetList(HistoryStarKey + stateIndex, new List<int>());
        }

        public void SaveHistoryStar(int stateIndex, List<int> newData)
        {
            SaveList(HistoryStarKey + stateIndex, newData);
        }

        public void SaveHistoryStar(int stateIndex, int saveIndex, int newData)
        {
            var current = GetHistoryStar(stateIndex);
            current[saveIndex] = newData;
            SaveList(HistoryStarKey + stateIndex, current);
        }

        public void AddHistoryStar(int stateIndex, int newData)
        {
            var current = GetHistoryStar(stateIndex);
            current.Add(newData);
            SaveList(HistoryStarKey + stateIndex, current);
        }

        public void LoadData()
        {
            UserCoin = PlayerPrefs.GetInt(UserCoinKey, 0);
            UserDiamond = PlayerPrefs.GetInt(UserDiamondKey, 0);
            CurrentLevel = GetList(UserCurrentLevelKey, new List<int>());
        }

        public void SaveData()
        {
            PlayerPrefs.SetInt(UserCoinKey, UserCoin);
            PlayerPrefs.SetInt(UserDiamondKey, UserDiamond);
            SaveList(UserCurrentLevelKey, CurrentLevel);
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

        #endregion
    }
}