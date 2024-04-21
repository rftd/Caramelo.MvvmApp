using Caramelo.MvvmApp.Dialogs;

namespace Caramelo.MvvmApp.ViewModel;

public interface IMvvmDialogViewModel<TParameter, TResult> where TParameter : DialogOptions where TResult : notnull
{
    bool IsBusy { get; set; }
    
    string Title { get; set; }
    
    bool CanClose { get; set; }
    
    IObservable<TResult> DialogResult { get; }
    
    IServiceProvider Service { get; }
    
    void Initialize(TParameter parameter);
    
    void ViewCreated();
    
    void ViewAppearing();
    
    void ViewAppeared();
    
    void ViewDisappearing();
    
    void ViewDisappeared();
    
    void ViewDestroy(bool viewFinishing = true);
}