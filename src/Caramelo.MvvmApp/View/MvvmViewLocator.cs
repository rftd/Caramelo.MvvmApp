using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Caramelo.MvvmApp.View;

internal sealed class MvvmViewLocator : IViewLocator
{
    #region Fields

    private readonly IServiceProvider service;

    #endregion Fields

    #region Constructors

    public MvvmViewLocator(IServiceProvider service)
    {
        this.service = service;
    }

    #endregion Constructors

    #region Methods

    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null)
    {
        if (viewModel == null) return null;

        IViewFor? view;
        var type = typeof(IViewFor<>).MakeGenericType(viewModel.GetType());
        
        if(contract is not null && service is IKeyedServiceProvider keyedServiceProvider)
            view = (IViewFor?)keyedServiceProvider.GetKeyedService(type, contract);
        else
            view = (IViewFor?)service.GetService(type);
        
        return view;
    }

    #endregion Methods
}