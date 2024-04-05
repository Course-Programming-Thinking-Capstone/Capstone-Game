using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class PlayerService
    {
        private const char Break = '~';
        private const string LevelPlayedKey = "lpk";
        private const string CharacterOwnKey = "cok";
        private Dictionary<int, int> levelPlayer;

        public readonly List<int> characterBought;

        public PlayerService()
        {
            characterBought = GetList(CharacterOwnKey, new List<int> { 0 });
        }

        public void SaveNewCharacter(int boughtId)
        {
            if (!characterBought.Contains(boughtId))
            {
                characterBought.Add(boughtId);
                SaveList(CharacterOwnKey, characterBought);
            }
        }

        public int GetCurrentLevel(int modeId)
        {
            return PlayerPrefs.GetInt(LevelPlayedKey + modeId, 0);
        }

        public void SaveData(int modeId, int index)
        {
            PlayerPrefs.SetInt(LevelPlayedKey + modeId, index);
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