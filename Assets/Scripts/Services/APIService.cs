using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine.Networking;

namespace Services
{
    public class APIService
    {
        /// <summary>
        /// Get an api
        /// </summary>
        /// <param name="url">api url</param>
        /// <typeparam name="TResultType">return type</typeparam>
        /// <returns></returns>
        public async Task<TResultType> Get<TResultType>(string url)
        {
            using UnityWebRequest request = UnityWebRequest.Get(url);
            var operation = request.SendWebRequest();
            while (!operation.isDone) // wait for operation
            {
                await Task.Yield();
            }
                
            if (request.result == UnityWebRequest.Result.Success)
            {  
                var jsonResult = request.downloadHandler.text;
                var result =  JsonConvert.DeserializeObject<TResultType>(jsonResult);
                return result;
            }
            
            Logger.Debug(request.result);
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
            var operation = request.SendWebRequest();
            while (!operation.isDone) // wait for operation
            {
                await Task.Yield();
            }
                
            if (request.result == UnityWebRequest.Result.Success)
            {  
                var jsonResult = request.downloadHandler.text;
                var result =  JsonConvert.DeserializeObject<TResultType>(jsonResult);
                return result;
            }
            
            Logger.Debug(request.result);
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
            var operation = request.SendWebRequest();
            while (!operation.isDone) // wait for operation
            {
                await Task.Yield();
            }
                
            if (request.result == UnityWebRequest.Result.Success)
            {  
                var jsonResult = request.downloadHandler.text;
                var result =  JsonConvert.DeserializeObject<TResultType>(jsonResult);
                return result;
            }
            
            Logger.Debug(request.result);
            return default;
        }
        
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="url">API Url</param>
        /// <returns></returns>
        public async Task<TResultType> Put<TResultType>(string url)
        {
            using UnityWebRequest request = UnityWebRequest.Delete(url);
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }
                
            if (request.result == UnityWebRequest.Result.Success)
            {  
                var jsonResult = request.downloadHandler.text;
                var result =  JsonConvert.DeserializeObject<TResultType>(jsonResult);
                return result;
            }
            
            Logger.Debug(request.result);
            return default;
        }
    }
}