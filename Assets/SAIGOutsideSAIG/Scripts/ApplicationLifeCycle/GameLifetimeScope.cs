using Core.Player;
using SceneSystem;
using UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private SceneSO _sceneSO;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_sceneSO);
        builder.RegisterComponentInHierarchy<PassCodeUI>();
        builder.RegisterComponentInHierarchy<PlayerMovement>();
    }
}
