using System;
using System.Threading.Tasks;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.Logging;
using ReactiveUI.SourceGenerators;

namespace Caramelo.MvvmApp.Demo.Core.ViewModels;

[CurrentTab]
public partial class GuardedNavigationViewModel : MvvmViewModel
{
    [Reactive] private bool hasUnsavedChanges = true;

    public GuardedNavigationViewModel(IServiceProvider service) : base(service)
    {
        Title = "Navegação Protegida";
    }

    public override async Task<bool> CanNavigateAsync(NavigationMode mode, IMvvmViewModel? viewModel)
    {
        if (!HasUnsavedChanges)
            return true;

        var confirmed = await Dialogs.ConfirmAsync(
            "Alterações Pendentes",
            "Existem alterações não salvas nesta tela. Tem certeza de que deseja sair?");

        Log.LogInformation("CanNavigateAsync resultado para modo {Mode}: {Confirmed}", mode, confirmed);
        return confirmed;
    }

    [ReactiveCommand]
    private async Task GoBackAsync()
    {
        await Navigation.GoBackAsync(this);
    }
}
