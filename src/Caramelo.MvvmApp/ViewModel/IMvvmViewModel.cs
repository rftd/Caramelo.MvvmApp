namespace Caramelo.MvvmApp.ViewModel;

public interface IMvvmViewModel
{
    bool IsBusy { get; }

    string Title { get; }

    IServiceProvider Service { get; }

    void ViewCreated();

    void ViewAppearing();

    void ViewAppeared();

    void ViewDisappearing();

    void ViewDisappeared();

    void ViewDestroy(bool viewFinishing = true);
}

public interface IMvvmViewModelParameter : IMvvmViewModel
{
    void Initialize(object parameter);
}

public interface IMvvmViewModelResult : IMvvmViewModel
{
}

public interface IMvvmViewModelParameterResult : IMvvmViewModelParameter, IMvvmViewModelResult
{
}