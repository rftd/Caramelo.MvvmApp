using System;
using System.Threading.Tasks;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.Logging;
using ReactiveUI.SourceGenerators;

namespace Caramelo.MvvmApp.Demo.Core.ViewModels;

[CurrentTab]
[CanClose(false)]
public partial class HomeViewModel : MvvmViewModel
{
    [Reactive] private string tabTitle = "Início";

    public HomeViewModel(IServiceProvider service) : base(service)
    {
        Title = tabTitle;
        this.WhenAnyValue(x => x.TabTitle)
            .Subscribe(value => Title = value ?? string.Empty);
    }

    [ReactiveCommand]
    private async Task OpenDetailAsync()
    {
        Log.LogInformation("Navegando na aba atual para DetailView");
        await Navigation.GoToAsync<DetailViewModel, string>("Parâmetro enviado pela HomeViewModel");
    }

    [ReactiveCommand]
    private async Task OpenNewTabAsync()
    {
        Log.LogInformation("Navegando para Nova Aba");
        await Navigation.GoToAsync<NewTabDemoViewModel>();
    }

    [ReactiveCommand]
    private async Task OpenDialogsAsync()
    {
        Log.LogInformation("Abrindo demonstração de Diálogos na aba atual");
        await Navigation.GoToAsync<DialogsDemoViewModel>();
    }

    [ReactiveCommand]
    private async Task OpenResultDemoAsync()
    {
        Log.LogInformation("Abrindo demonstração de Retorno de Resultados");
        await Navigation.GoToAsync<ParametersDemoViewModel>();
    }

    [ReactiveCommand]
    private async Task OpenGuardedDemoAsync()
    {
        Log.LogInformation("Abrindo demonstração de Navegação Protegida");
        await Navigation.GoToAsync<GuardedNavigationViewModel>();
    }
}
