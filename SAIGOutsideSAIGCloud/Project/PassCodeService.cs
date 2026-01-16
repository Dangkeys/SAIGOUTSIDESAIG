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
        public async Task<bool> VerifyPassword(IExecutionContext context, IGameApiClient gameApiClient, string passCodeID, string inputPassword)
        {
            var results = await gameApiClient.CloudSaveData.GetPrivateCustomItemsAsync(context, context.ServiceToken, context.ProjectId, $"PASSCODE_{passCodeID}", new List<string> { "password" });
            _logger.LogDebug("CloudSaveData.GetPrivateCustomItemsAsync results: {Results}", results);
            if (results.Data.Results == null)
            {
                return false;
            }
            var value = results.Data.Results.First().Value;
            _logger.LogDebug("Value: {Value}", value);
            return string.Equals(inputPassword, (string?)value, System.StringComparison.OrdinalIgnoreCase);

        }
        [CloudCodeFunction("VerifyPasswordAndSolveScene")]
        public async Task<bool> VerifyPasswordAndSolveScene(IExecutionContext context, PushClient pushClient, IGameApiClient gameApiClient, string passCodeID, string inputPassword, string sceneID)
        {
            var isCorrect = await VerifyPassword(context, gameApiClient, passCodeID, inputPassword);
            if (isCorrect)
            {
                await _sceneService.SolveScene(context, gameApiClient, sceneID);
                await SendProjectMessage(context, pushClient, $"{sceneID}", "GlobalSceneAlert");
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