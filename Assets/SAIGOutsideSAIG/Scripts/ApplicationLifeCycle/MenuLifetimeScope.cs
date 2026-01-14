using SceneController;
using Unity.Services.CloudCode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class MenuLifetimeScope : LifetimeScope
{
    [SerializeField] private SceneDatabaeSO _sceneDatabaseSO;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_sceneDatabaseSO);
    }
}
