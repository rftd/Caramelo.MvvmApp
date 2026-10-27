using System;
using System.Threading.Tasks;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.Logging;
using ReactiveUI.SourceGenerators;

namespace Caramelo.MvvmApp.Demo.Core.ViewModels;

public partial class DemoAppViewModel : AppViewModel
{
    public DemoAppViewModel(IServiceProvider service) : base(service)
    {
    }

    [ReactiveCommand]
    private async Task OpenHomeAsync()
    {
        Log.LogInformation("Abrindo Início");
        await Navigation.GoToAsync<HomeViewModel>();
    }

    [ReactiveCommand]
    private async Task OpenNewTabAsync()
    {
        Log.LogInformation("Abrindo Nova Aba");
        await Navigation.GoToAsync<NewTabDemoViewModel>();
    }

    [ReactiveCommand]
    private async Task OpenDialogsAsync()
    {
        Log.LogInformation("Abrindo Diálogos");
        await Navigation.GoToAsync<DialogsDemoViewModel>();
    }

    [ReactiveCommand]
    private async Task OpenResultDemoAsync()
    {
        Log.LogInformation("Abrindo Retorno de Resultados");
        await Navigation.GoToAsync<ParametersDemoViewModel>();
    }

    [ReactiveCommand]
    private async Task OpenGuardedDemoAsync()
    {
        Log.LogInformation("Abrindo Navegação Protegida");
        await Navigation.GoToAsync<GuardedNavigationViewModel>();
    }
}