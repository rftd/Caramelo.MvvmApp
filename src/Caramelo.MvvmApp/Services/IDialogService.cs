using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.ViewModel;

namespace Caramelo.MvvmApp.Services;

public interface IDialogService
{
    #region Methods

    Task ShowAsync(string title, string message);
    
    Task InfoAsync(string message);
    
    Task WarnAsync(string message);
    
    Task ErroAsync(string message);
    
    Task<string> InputAsync(string title, string message);
    
    Task<bool> ConfirmAsync(string title, string message);
    
    Task<TResult> ShowAsync<TViewModel, TResult, TParameter>(TParameter parameter)
        where TViewModel : MvvmDialogViewModel<TParameter, TResult>
        where TParameter : DialogOptions
        where TResult : notnull;

    #endregion
}