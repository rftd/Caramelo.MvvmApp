using Caramelo.MvvmApp.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Caramelo.MvvmApp.ViewModel;

public abstract class RoutableViewModel : MvvmViewModel, IScreen
{
    #region Constructors

    protected RoutableViewModel(IServiceProvider service) : base(service)
    {
        Router = Service.GetRequiredService<IRouterService>().GetDefaultRouter();
    }

    #endregion Constructors

    #region Properties

    public RoutingState Router { get; }
    
    #endregion Properties
}