using MenuSystem;
using SceneSystem;
using Unity.Services.CloudCode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class MenuLifetimeScope : LifetimeScope
{

    protected override void Configure(IContainerBuilder builder)
    {
        builder.UseEntryPoints(entryPoints =>
        {
            entryPoints.Add<MenuController>().AsSelf();
        });
    }
}
