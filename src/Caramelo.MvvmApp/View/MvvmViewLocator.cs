using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

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

    public IViewFor? ResolveView<T>(T? viewModel, string? contract) where T : class
    {
        return viewModel == null ? null : ResolveView(typeof(IViewFor<T>), contract);
    }

    public IViewFor? ResolveView(object? viewModel, string? contract)
    {
        return null;
    }

    [RequiresDynamicCode("Resolves IViewFor<T> using the view model's runtime type.")]
    public IViewFor? ResolveViewUnsafe(object? viewModel, string? contract)
    {
        if (viewModel == null) return null;

        var type = typeof(IViewFor<>).MakeGenericType(viewModel.GetType());
        return ResolveView(type, contract);
    }

    private IViewFor? ResolveView(Type viewType, string? contract)
    {
        if (contract is not null && service is IKeyedServiceProvider keyedServiceProvider)
            return (IViewFor?)keyedServiceProvider.GetKeyedService(viewType, contract);

        return (IViewFor?)service.GetService(viewType);
    }

    #endregion Methods
}