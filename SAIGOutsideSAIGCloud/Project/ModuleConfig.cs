using Microsoft.Extensions.DependencyInjection;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using SAIGOutsideSAIGCloud;

public class ModuleConfig : ICloudCodeSetup
{
    public void Setup(ICloudCodeConfig config)
    {
        config.Dependencies.AddSingleton(GameApiClient.Create());
        config.Dependencies.AddSingleton(PushClient.Create());
        config.Dependencies.AddSingleton<SceneService>();
        config.Dependencies.AddSingleton<PassCodeService>();
    }
}