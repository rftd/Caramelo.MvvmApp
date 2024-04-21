namespace Caramelo.MvvmApp.ViewModel;

public interface IMvvmViewModel
{
    public bool IsBusy { get; }

    public string Title { get; }

    public IServiceProvider Service { get; }

    public void ViewCreated();

    public void ViewAppearing();

    public void ViewAppeared();

    public void ViewDisappearing();

    public void ViewDisappeared();

    public void ViewDestroy(bool viewFinishing = true);
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