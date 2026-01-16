using InputSystem;
using MessagePipe;
using VContainer;
using VContainer.Unity;
using SceneSystem;
using UnityEngine;
using SaveSystem;
public class RootLifetimeScope : LifetimeScope
{
    [SerializeField] private SceneDatabaeSO _sceneDatabaseSO;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.UseEntryPoints(entryPoints =>
        {
            entryPoints.Add<ApplicationController>().AsSelf();
            entryPoints.Add<SceneController>().AsSelf();
        });
        builder.Register<InputSystem_Actions>(Lifetime.Singleton);
        builder.Register<GameInputReader>(Lifetime.Singleton);

        builder.RegisterInstance(_sceneDatabaseSO);
        builder.Register<CloudSaveCustomClient>(Lifetime.Singleton).As<ICustomSaveClient>();
        builder.RegisterMessagePipe();
    }
}
