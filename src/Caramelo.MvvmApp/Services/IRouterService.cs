using ReactiveUI;

namespace Caramelo.MvvmApp.Services;

public interface IRouterService
{
    bool RegisterRoute(string route);
    bool RegisterRoute(string route, RoutingState router);
    RoutingState GetDefaultRouter();
    RoutingState? GetRouter(string route);
}