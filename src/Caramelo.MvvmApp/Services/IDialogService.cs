using System.Reactive;
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

    Task<TResult> ShowAsync<TViewModel, TResult>(DialogOptions options)
        where TViewModel : MvvmDialogViewModel<DialogOptions, TResult>
        where TResult : notnull;
    
    
    Task<TResult> ShowAsync<TViewModel, TResult>()
        where TViewModel : MvvmDialogViewModel<DialogOptions, TResult>
        where TResult : notnull;
    
    Task ShowAsync<TViewModel>(DialogOptions options)
        where TViewModel : MvvmDialogViewModel<DialogOptions, Unit>;
    
    Task ShowAsync<TViewModel>()
        where TViewModel : MvvmDialogViewModel<DialogOptions, Unit>;

    #endregion
}