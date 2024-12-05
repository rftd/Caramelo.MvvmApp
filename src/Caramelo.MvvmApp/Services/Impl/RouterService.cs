using ReactiveUI;

namespace Caramelo.MvvmApp.Services.Impl;

internal class RouterService : IRouterService
{
    #region Fields

    private readonly Dictionary<string, RoutingState> routers = new()
    {
        { "Default", new RoutingState() }
    };

    #endregion Fields

    #region Methods

    public bool RegisterRoute(string route) => routers.TryAdd(route, new RoutingState());
    
    public bool RegisterRoute(string route, RoutingState router) => routers.TryAdd(route, router);
    
    public RoutingState GetDefaultRouter() => routers["Default"];

    public RoutingState? GetRouter(string route)
    {
        routers.TryGetValue(route, out var router);
        return router;
    }
    
    #endregion Methods
}