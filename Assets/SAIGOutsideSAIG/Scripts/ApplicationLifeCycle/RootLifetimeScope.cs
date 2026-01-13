using InputSystem;
using MessagePipe;
using VContainer;
using VContainer.Unity;

public class RootLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {

        builder.Register<InputSystem_Actions>(Lifetime.Singleton);
        builder.Register<GameInputReader>(Lifetime.Singleton);
        builder.RegisterMessagePipe();
    }
}
