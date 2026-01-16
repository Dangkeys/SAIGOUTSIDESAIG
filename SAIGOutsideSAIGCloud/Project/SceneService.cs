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
        public async Task<bool> SolveScene(IExecutionContext context, IGameApiClient gameApiClient, string sceneID)
        {

            try
            {
                var body = new SetItemBody("status", "Solved");
                _logger.LogDebug("Body: {Body}", body);
                var setResult = await gameApiClient.CloudSaveData.SetCustomItemAsync(
                    context,
                    context.ServiceToken,
                    context.ProjectId,
                    $"SCENE_{sceneID}",
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