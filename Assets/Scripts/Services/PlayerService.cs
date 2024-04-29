using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class PlayerService
    {
        private const char Break = '~';
        private const string LevelPlayedKey = "lpk";
        private const string SelectCharacterKey = "sck";
        private const string VoucherBuyKey = "vbk";
        private Dictionary<int, int> levelPlayer;

        public int SelectedCharacter { get; set; } = PlayerPrefs.GetInt(SelectCharacterKey, -1);

        public PlayerService()
        {
        }

        public void ClearData()
        {
            for (int i = 0; i < 20; i++)
            {
                PlayerPrefs.DeleteKey(LevelPlayedKey + i);
            }
        }

        public void SaveSelectedCharacter(int selectedId)
        {
            SelectedCharacter = selectedId;
            PlayerPrefs.SetInt(SelectCharacterKey, SelectedCharacter);
        }

        public int GetCurrentLevel(int modeId)
        {
            return PlayerPrefs.GetInt(LevelPlayedKey + modeId, 0);
        }

        public void SaveData(int modeId, int currentLevel)
        {
            var current = GetCurrentLevel(modeId);
            if (current < currentLevel)
            {
                PlayerPrefs.SetInt(LevelPlayedKey + modeId, currentLevel);
            }
        }

        private Dictionary<int, int> cacheVoucherBuyLeft;

        public int LoadVoucherBoughtLeft(int userId, int voucherId)
        {
            var result = 0;
            switch (voucherId)
            {
                case 1:
                {
                    result = PlayerPrefs.GetInt(VoucherBuyKey + userId + voucherId, 3);
                    break;
                }
                case 2:
                {
                    result = PlayerPrefs.GetInt(VoucherBuyKey + userId + voucherId, 2);
                    break;
                }
                case 3:
                {
                    result = PlayerPrefs.GetInt(VoucherBuyKey + userId + voucherId, 1);
                    break;
                }
                default:
                {
                    return 0;
                }
            }

            return result;
        }

        public void SaveVoucherBought(int userId, int voucherId)
        {
            var current = LoadVoucherBoughtLeft(userId, voucherId);
            current--;
            PlayerPrefs.SetInt(VoucherBuyKey + userId + voucherId, current);
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