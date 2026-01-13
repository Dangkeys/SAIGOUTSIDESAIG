using Core.Player;
using UI;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<PassCodeUI>();
        builder.RegisterComponentInHierarchy<PlayerMovement>();
    }
}
