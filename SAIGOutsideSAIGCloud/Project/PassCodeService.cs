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
        public PassCodeService(ILogger<PassCodeService> logger)
        {
            _logger = logger;
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

            var foundItem = passcodeList.FirstOrDefault(item => item.id == passCodeID);

            if (foundItem == null)
            {
                foundItem = passcodeList.FirstOrDefault(item => item.name == passCodeName);
            }

            if (foundItem != null)
            {
                return inputPassword == foundItem.password;
            }

            return false;
        }
    }
}