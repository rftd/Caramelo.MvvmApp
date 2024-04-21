using Caramelo.MvvmApp.ViewModel;

namespace Caramelo.MvvmApp.Services;

public interface INavigationService
{
    #region Methods

    Task<bool> CanGoBack(IMvvmViewModel? viewModel = null);
    
    Task<bool> CanGoTo(IMvvmViewModel? viewModel = null);
    
    Task GoBackAsync<TModel>(TModel viewModel) where TModel : MvvmViewModel;

    Task GoBackAsync<TParameter, TResult>(MvvmViewModel<TParameter, TResult> viewModel, TResult result)
        where TParameter : notnull
        where TResult : notnull;

    Task GoBackAsync<TResult>(MvvmResultViewModel<TResult> viewModel, TResult result) where TResult : notnull;

    Task<TResult?> GoToAsync<TModel, TParameter, TResult>(TParameter parameter)
        where TModel : MvvmViewModel<TParameter, TResult>
        where TParameter : notnull
        where TResult : notnull;

    Task GoToAsync<TModel, TParameter>(TParameter parameter)
        where TModel : MvvmViewModel<TParameter>
        where TParameter : notnull;

    Task<TResult?> GoToAsync<TModel, TResult>()
        where TModel : MvvmResultViewModel<TResult>
        where TResult : notnull;

    Task GoToAsync<TModel>() where TModel : MvvmViewModel;

    #endregion Methods
}