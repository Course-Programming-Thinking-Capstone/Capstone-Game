using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Services.Request;
using Services.Response;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Services
{
    public class ClientService
    {
        #region Cache user data

        public string UserEmail { get; set; }
        public string UserDisplayName { get; set; }
        public int UserId { get; set; }
        public int Coin { get; set; }
        public int Gem { get; set; }
        public List<GameItemResponse> CacheShopData { get; set; }
        public List<int> UserOwnedShopItem { get; set; }
        public bool IsLogin => UserId != -1;

        #endregion

        private readonly string baseApi;

        private string jwt;
        public Dictionary<int, GameModeResponse> GameModes { get; set; } = new();
        public UnityAction<string> OnFailed { get; set; }

        public ClientService(string baseApi)
        {
            this.baseApi = baseApi;
            jwt = "";
            UserId = -1;
        }

        public async Task LoadData()
        {
            await GetShopOwnedData();
            await GetShopData();
        }

        #region INVENTORY

        public async Task<List<UserInventoryResponse>> GetUserOwnedItems()
        {
            if (!IsLogin)
            {
                return null;
            }

            var api = baseApi + "games/game-drop-item-owned/" + UserId;
            try
            {
                var result = await Get<List<UserInventoryResponse>>(api);

                return result;
            }
            catch (Exception e)
            {
                OnFailed.Invoke(e.Message);
            }

            return null;
        }

        public async void SellItem(int itemId, int sellCount, UnityAction<BuyResponse> onSuccess,
            UnityAction<string> onFail)
        {
            if (!IsLogin)
            {
                return;
            }

            OnFailed = onFail;
            var api = baseApi + "games/game-drop-item-owned";
            var requestBody = new
            {
                userId = UserId,
                soldItemId = itemId,
                quantity = sellCount,
            };
            try
            {
                var result = await Post<BuyResponse>(api, requestBody);
                if (result != null)
                {
                    Coin = result.CurrentCoin;
                    Gem = result.CurrentGem;
                    onSuccess?.Invoke(result);
                }
                else
                {
                    OnFailed.Invoke("Sold fail");
                }
            }
            catch (Exception e)
            {
                OnFailed.Invoke(e.Message);
            }
        }

        #endregion

        #region SHOP

        public async void BuyVoucher(int voucherId, int cost, UnityAction<BuyResponse> onSuccess,
            UnityAction<string> onFail)
        {
            if (!IsLogin)
            {
                return;
            }

            OnFailed = onFail;
            var api = baseApi + "games/game-voucher";
            var requestParam = new
            {
                UserId = UserId,
                Cost = cost,
                VoucherType = voucherId
            };
            try
            {
                var response = await Post<BuyResponse>(api, requestParam);
                if (response != null)
                {
                    Coin = response.CurrentCoin;
                    Gem = response.CurrentGem;
                    onSuccess?.Invoke(response);
                }
            }
            catch (Exception e)
            {
                OnFailed.Invoke(e.Message);
            }
        }

        public async Task<BuyResponse> BuyItem(int itemId)
        {
            var api = baseApi + "games/game-shop-item-owned?itemId=" + itemId + "&userId=" + UserId;
            try
            {
                var response = await Post<BuyResponse>(api, null);
                if (response != null)
                {
                    Coin = response.CurrentCoin;
                    Gem = response.CurrentGem;
                    UserOwnedShopItem = response.OwnedItem;
                }

                return response;
            }
            catch (Exception e)
            {
                OnFailed.Invoke(e.Message);
            }

            return null;
        }

        public async Task GetShopOwnedData()
        {
            if (!IsLogin)
            {
                return;
            }

            var api = baseApi + "games/game-shop-item-owned?userId=" + UserId;
            try
            {
                var response = await Get<List<int>>(api);
                UserOwnedShopItem = response ?? new List<int>();
            }
            catch (Exception e)
            {
                OnFailed.Invoke(e.Message);
            }
        }

        public async Task<List<GameItemResponse>> GetShopData()
        {
            var api = baseApi + "games/shop-item";
            try
            {
                CacheShopData = await Get<List<GameItemResponse>>(api);
                return CacheShopData;
            }
            catch (Exception e)
            {
                OnFailed.Invoke(e.Message);
            }

            return null;
        }

        #endregion

        public async Task<UserDataResponse> FinishLevel(int mode, int levelIndex, DateTime startTime)
        {
            if (!IsLogin)
            {
                return null;
            }

            var api = baseApi + "games/game-play-history";
            try
            {
                var requestParam = new FinishGameRequest
                {
                    UserID = UserId,
                    ModeId = mode,
                    LevelIndex = levelIndex,
                    StartTime = startTime
                };
                var result = await Post<UserDataResponse>(api, requestParam);

                return result;
            }
            catch (Exception e)
            {
                OnFailed.Invoke(e.Message);
            }

            return null;
        }

        public async Task<List<UserProcessResponse>> GetUserProcess()
        {
            if (!IsLogin)
            {
                return new List<UserProcessResponse>();
            }

            var api = baseApi + "games/user-process/" + UserId;
            try
            {
                var result = await Get<List<UserProcessResponse>>(api);
                if (result == null || result.Count <= 0) return result;

                return result;
            }
            catch (Exception e)
            {
                OnFailed.Invoke(e.Message);
            }

            return null;
        }

        public async Task<List<GameModeResponse>> GetGameMode()
        {
            var api = baseApi + "games/game-mode";
            try
            {
                var result = await Get<List<GameModeResponse>>(api);
                if (result == null || result.Count <= 0) return result;
                GameModes.Clear();
                foreach (var mode in result)
                {
                    GameModes.Add(mode.idMode, mode);
                }

                return result;
            }
            catch (Exception e)
            {
                OnFailed.Invoke(e.Message);
            }

            return null;
        }

        public async Task<LevelInformationResponse> GetLevelData(int modeId, int levelIndex)
        {
            //  http://localhost:5082/api/v1/games/level-data?id=3&index=2
            var api = baseApi + "games/level-data?id=" + modeId + "&index=" + levelIndex;
            try
            {
                var result = await Get<LevelInformationResponse>(api);
                return result;
            }
            catch (Exception e)
            {
                OnFailed.Invoke(e.Message);
            }

            return null;
        }

        public void LogOut()
        {
            jwt = "";
            UserId = -1;
            UserEmail = "";
            UserDisplayName = "";
            Coin = 0;
            Gem = 0;
            UserOwnedShopItem = new List<int>();
        }

        public async void LoginWithEmail(string account, string password, UnityAction onSuccess)
        {
            var api = baseApi + "games/authentication/login";
            var requestParam = new
            {
                account, password
            };
            try
            {
                var result = await Post<LoginResponse>(api, requestParam);
                if (result != null)
                {
                    jwt = result.accessToken;
                    UserId = result.userId;
                    UserEmail = account;
                    UserDisplayName = result.displayName;
                    Coin = result.userCoin;
                    Gem = result.userGem;
                    onSuccess?.Invoke();
                }
            }
            catch (Exception e)
            {
                OnFailed.Invoke(e.Message);
            }
        }

        #region Generic API

        /// <summary>
        /// Get an api
        /// </summary>
        /// <param name="url">api url</param>
        /// <typeparam name="TResultType">return type</typeparam>
        /// <returns></returns>
        public async Task<TResultType> Get<TResultType>(string url)
        {
            using UnityWebRequest request = UnityWebRequest.Get(url);

            if (!string.IsNullOrEmpty(jwt))
            {
                request.SetRequestHeader("Authorization", "Bearer " + jwt);
            }

            var operation = request.SendWebRequest();
            while (!operation.isDone) // wait for operation
            {
                await Task.Yield();
            }

            switch (request.result)
            {
                case UnityWebRequest.Result.Success:
                    var jsonResult = request.downloadHandler.text;
                    var result = JsonConvert.DeserializeObject<TResultType>(jsonResult);
                    return result;
                case UnityWebRequest.Result.ConnectionError:
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    var errorResult = JsonConvert.DeserializeObject<ErrorResponse>(request.downloadHandler.text);
                    OnFailed?.Invoke(errorResult.Message);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    break;
            }

            return default;
        }

        /// <summary>
        /// Post an api
        /// </summary>
        /// <param name="url">API Url</param>
        /// <param name="requestData">Object Data</param>
        /// <typeparam name="TResultType">Return Data</typeparam>
        /// <returns></returns>
        public async Task<TResultType> Post<TResultType>(string url, object requestData)
        {
            var payload = JsonConvert.SerializeObject(requestData);
            using UnityWebRequest request = UnityWebRequest.Post(url, payload, "application/json");

            if (!string.IsNullOrEmpty(jwt))
            {
                request.SetRequestHeader("Authorization", "Bearer " + jwt);
            }

            var operation = request.SendWebRequest();
            while (!operation.isDone) // wait for operation
            {
                await Task.Yield();
            }

            switch (request.result)
            {
                case UnityWebRequest.Result.Success:
                    var jsonResult = request.downloadHandler.text;
                    var result = JsonConvert.DeserializeObject<TResultType>(jsonResult);
                    return result;
                case UnityWebRequest.Result.ConnectionError:
                    OnFailed?.Invoke("ConnectionError");
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    var errorResult = JsonConvert.DeserializeObject<ErrorResponse>(request.downloadHandler.text);
                    OnFailed?.Invoke(errorResult.Message);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    OnFailed?.Invoke("DataProcessingError");
                    break;
            }

            return default;
        }

        /// <summary>
        /// Put an api
        /// </summary>
        /// <param name="url">API Url</param>
        /// <param name="requestData">Object Data</param>
        /// <typeparam name="TResultType">Return Data</typeparam>
        /// <returns></returns>
        public async Task<TResultType> Put<TResultType>(string url, object requestData)
        {
            var payload = JsonConvert.SerializeObject(requestData);
            using UnityWebRequest request = UnityWebRequest.Put(url, payload);

            if (!string.IsNullOrEmpty(jwt))
            {
                request.SetRequestHeader("Authorization", "Bearer " + jwt);
            }

            var operation = request.SendWebRequest();
            while (!operation.isDone) // wait for operation
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                var jsonResult = request.downloadHandler.text;
                var result = JsonConvert.DeserializeObject<TResultType>(jsonResult);
                return result;
            }

            return default;
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="url">API Url</param>
        /// <returns></returns>
        public async Task<TResultType> Delete<TResultType>(string url)
        {
            using UnityWebRequest request = UnityWebRequest.Delete(url);
            var operation = request.SendWebRequest();

            if (!string.IsNullOrEmpty(jwt))
            {
                request.SetRequestHeader("Authorization", "Bearer " + jwt);
            }

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                var jsonResult = request.downloadHandler.text;
                var result = JsonConvert.DeserializeObject<TResultType>(jsonResult);
                return result;
            }

            return default;
        }

        #endregion
    }
}