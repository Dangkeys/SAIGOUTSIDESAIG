using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.CloudSave.Internal;
using UnityEngine;
using Utils;

namespace SaveSystem
{
    public class CloudSavePlayerClient : IPlayerSaveClient
    {
        private readonly IPlayerDataService _client = CloudSaveService.Instance.Data.Player;

        public async Task Save(string key, object value)
        {
            var data = new Dictionary<string, object> { { key, value } };
            await Call(_client.SaveAsync(data));
        }

        public async Task Save(params (string key, object value)[] values)
        {
            var data = values.ToDictionary(item => item.key, item => item.value);
            await Call(_client.SaveAsync(data));
        }

        public async Task<T> Load<T>(string key)
        {
            var query = await Call(_client.LoadAsync(new HashSet<string> { key }));
            if (query.TryGetValue(key, out var item))
            {
                var stringValue = item.Value?.ToString();
                return stringValue != null ? SerializeHelper.Deserialize<T>(stringValue) : default;
            }
            return default;
        }

        public async Task<IEnumerable<T>> Load<T>(params string[] keys)
        {
            var query = await Call(_client.LoadAsync(keys.ToHashSet()));

            return keys.Select(k =>
            {
                if (query.TryGetValue(k, out var item))
                {
                    var stringValue = item.Value?.ToString();
                    return stringValue != null ? SerializeHelper.Deserialize<T>(stringValue) : default;
                }

                return default;
            });
        }


        public async Task Delete(string key)
        {
            var options = new Unity.Services.CloudSave.Models.Data.Player.DeleteOptions();
            await Call(_client.DeleteAsync(key, options));
        }

        public async Task DeleteAll()
        {
            var options = new Unity.Services.CloudSave.Models.Data.Player.DeleteAllOptions();
            await Call(_client.DeleteAllAsync(options));
        }
        

        private static async Task Call(Task action)
        {
            try
            {
                await action;
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError($"CloudSave Validation Error: {e.Message}");
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError($"CloudSave Rate Limit Error: {e.Message}");
            }
            catch (CloudSaveException e)
            {
                Debug.LogError($"CloudSave Error: {e.Message}");
            }
        }

        private static async Task<T> Call<T>(Task<T> action)
        {
            try
            {
                return await action;
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError($"CloudSave Validation Error: {e.Message}");
                return default;
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError($"CloudSave Rate Limit Error: {e.Message}");
                return default;
            }
            catch (CloudSaveException e)
            {
                Debug.LogError($"CloudSave Error: {e.Message}");
                return default;
            }
        }
    }


}