using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudSave.Model;
namespace SAIGOutsideSAIGCloud
{
    public enum SceneStatus
    {
        Available,
        Solved,
        Archived
    }
    public class SceneItem
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public SceneStatus? status { get; set; }
    }

    public class SceneService
    {

        private readonly ILogger<SceneService> _logger;
        public SceneService(ILogger<SceneService> logger)
        {
            _logger = logger;
        }

        private async Task<List<SceneItem>> GetSceneItemList(IExecutionContext context, IGameApiClient gameApiClient)
        {
            var results = await gameApiClient.CloudSaveData.GetCustomItemsAsync(context, context.AccessToken, context.ProjectId, "CONFIG_SCENES", new List<string> { "data" });
            _logger.LogDebug("CloudSaveData.GetCustomItemsAsync results: {Results}", JsonConvert.SerializeObject(results));
            if (results.Data.Results == null || !results.Data.Results.Any())
            {
                return null;
            }
            var value = results.Data.Results.First().Value;

            List<SceneItem>? sceneItemList = null;
            try
            {
                var valueString = value?.ToString();
                if (string.IsNullOrEmpty(valueString))
                {
                    return null;
                }
                sceneItemList = JsonConvert.DeserializeObject<List<SceneItem>>(valueString);
                return sceneItemList;
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> SolveScene(IExecutionContext context, IGameApiClient gameApiClient, string sceneID, string sceneName)
        {
            var sceneItemList = await GetSceneItemList(context, gameApiClient);
            if (sceneItemList == null || sceneItemList.Count == 0)
            {
                _logger.LogWarning("[SolveScene] No scene items found to update.");
                return false;
            }

            var target = sceneItemList.FirstOrDefault(s => s.id == sceneID)
                         ?? sceneItemList.FirstOrDefault(s => s.name == sceneName);

            if (target == null)
            {
                _logger.LogWarning("[SolveScene] Scene not found. id={SceneID}, name={SceneName}", sceneID, sceneName);
                return false;
            }

            target.status = SceneStatus.Solved;

            try
            {
                var body = new SetItemBody("data", JsonConvert.SerializeObject(sceneItemList));
                _logger.LogDebug("Body: {Body}", body);
                var setResult = await gameApiClient.CloudSaveData.SetCustomItemAsync(
                    context,
                    context.ServiceToken,
                    context.ProjectId,
                    "CONFIG_SCENES",
                    body);

                _logger.LogDebug("[SolveScene] SetPrivateCustomItemAsync response: {Response}", JsonConvert.SerializeObject(setResult));
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "[SolveScene] Failed to persist scene status update.");
                return false;
            }
        }
    }
}