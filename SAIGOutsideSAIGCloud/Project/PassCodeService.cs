using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace SAIGOutsideSAIGCloud
{
    public class PasscodeItem
    {
        public string? id { get; set; }
        public string? password { get; set; }
        public string? name { get; set; }
    }

    public class PassCodeService
    {

        private readonly ILogger<PassCodeService> _logger;
        private readonly SceneService _sceneService;
        public PassCodeService(ILogger<PassCodeService> logger, SceneService sceneService)
        {
            _logger = logger;
            _sceneService = sceneService;
        }
        [CloudCodeFunction("VerifyPassword")]
        public async Task<bool> VerifyPassword(IExecutionContext context, IGameApiClient gameApiClient, string passCodeID, string passCodeName, string inputPassword)
        {
            var results = await gameApiClient.CloudSaveData.GetPrivateCustomItemsAsync(context, context.ServiceToken, context.ProjectId, "CONFIG_PASSCODES", new List<string> { "data" });
            _logger.LogDebug("CloudSaveData.GetPrivateCustomItemsAsync results: {Results}", JsonConvert.SerializeObject(results));
            if (results.Data.Results == null)
            {
                return false;
            }
            var value = results.Data.Results.First().Value;

            List<PasscodeItem>? passcodeList = null;
            try
            {
                var valueString = value?.ToString();
                if (string.IsNullOrEmpty(valueString))
                {
                    return false;
                }
                passcodeList = JsonConvert.DeserializeObject<List<PasscodeItem>>(valueString);
            }
            catch
            {
                return false;
            }

            if (passcodeList == null) return false;

            var foundItem = passcodeList.FirstOrDefault(item => item.id == passCodeID) ?? passcodeList.FirstOrDefault(item => item.name == passCodeName);

            if (foundItem != null)
            {
                return inputPassword == foundItem.password;
            }

            return false;
        }
        [CloudCodeFunction("VerifyPasswordAndSolveScene")]
        public async Task<bool> VerifyPasswordAndSolveScene(IExecutionContext context, PushClient pushClient, IGameApiClient gameApiClient, string passCodeID, string passCodeName, string inputPassword, string sceneID, string sceneName)
        {
            var isCorrect = await VerifyPassword(context, gameApiClient, passCodeID, passCodeName, inputPassword);
            if (isCorrect)
            {
                await _sceneService.SolveScene(context, gameApiClient, sceneID, sceneName);
                await SendProjectMessage(context, pushClient, $"{sceneID}:{sceneName}", "GlobalSceneAlert");
            }
            return isCorrect;
        }
        private async Task<string> SendProjectMessage(IExecutionContext context, PushClient pushClient, string message, string messageType)
        {
            var response = await pushClient.SendProjectMessageAsync(context, message, messageType);
            return "Project message sent";
        }
    }
}