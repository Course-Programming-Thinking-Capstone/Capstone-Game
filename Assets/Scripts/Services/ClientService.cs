using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Services.Response;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Services
{
    public class ClientService
    {
        #region Cache user data

        public string UserEmail { get; set; }
        public string UserDisplayName{ get; set; }
        public int UserId{ get; set; }
        public int Coin { get; set; }
        
        
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

        public async void LoginWithEmail(string email, string password, UnityAction onSuccess)
        {
            var api = baseApi + "games/authentication/login";
            var requestParam = new
            {
                email, password
            };
            try
            {
                var result = await Post<LoginResponse>(api, requestParam);
                if (result != null)
                {
                    onSuccess?.Invoke();
                    jwt = result.accessToken;
                    UserId = result.userId;
                    UserEmail = email;
                    UserDisplayName = result.displayName;
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
            request.SetRequestHeader("Authorization", jwt);

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
            request.SetRequestHeader("Authorization", jwt);

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
                    OnFailed?.Invoke("ProtocolError");
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
            request.SetRequestHeader("Authorization", jwt);

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

            request.SetRequestHeader("Authorization", jwt);
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