using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.CloudSave.Internal;
using Unity.Services.CloudSave.Models;
using UnityEngine;
using Utils;

namespace SaveSystem
{
    public class CloudSaveCustomClient : ICustomSaveClient
    {
        private ICustomDataService _client => CloudSaveService.Instance.Data.Custom;

        public async Task<T> Load<T>(string customItemID, params string[] keys)
        {
            var keySet = new HashSet<string>(keys);
            var query = await Call(_client.LoadAsync(customItemID, keySet));

            if (query == null || query.Count == 0)
                return default;

            // If single key requested, return that value deserialized
            if (keys.Length == 1)
            {
                var key = keys[0];
                if (query.TryGetValue(key, out var item))
                {
                    // item.Value is a JsonObject - serialize it to string
                    var stringValue = JsonConvert.SerializeObject(item.Value);
                    return stringValue != null ? SerializeHelper.Deserialize<T>(stringValue) : default;
                }
            }

            return default;
        }

        public async Task<List<T>> Query<T>(List<FieldFilter> filters, HashSet<string> returnKeys = null) where T : EntityData
        {
            var query = new Query(filters, returnKeys);
            var results = await Call(_client.QueryAsync(query));

            if (results == null || results.Count == 0)
                return new List<T>();

            // Deserialize each EntityData result into type T
            var deserializedResults = new List<T>();
            foreach (var entityData in results)
            {
                try
                {
                    // Build a dictionary from the Data list (Key-Value pairs)
                    var dataDict = new Dictionary<string, object>();
                    if (entityData.Data != null)
                    {
                        foreach (var item in entityData.Data)
                        {
                            var value = item.Value.GetAsString();
                            dataDict[item.Key] = value;
                        }
                    }

                    // Add the ID to the dictionary (extract clean ID without type prefix)
                    var cleanId = ItemTypeHelper.ExtractId(entityData.Id);
                    dataDict["id"] = cleanId;

                    // Serialize the dictionary and deserialize into type T
                    var json = JsonConvert.SerializeObject(dataDict);
                    var deserialized = SerializeHelper.Deserialize<T>(json);
                    if (deserialized != null)
                        deserializedResults.Add(deserialized);
                    else
                        Debug.LogWarning($"[Query] Deserialization returned null for entity {entityData.Id}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[Query] Exception deserializing entity {entityData.Id}: {ex.Message}\n{ex.StackTrace}");
                }
            }

            return deserializedResults;
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