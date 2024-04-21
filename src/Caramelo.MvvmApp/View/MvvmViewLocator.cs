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
        var view = (IViewFor?)service.GetService(typeof(IViewFor<>).MakeGenericType(viewModel!.GetType()));
        return view;
    }

    #endregion Methods
}