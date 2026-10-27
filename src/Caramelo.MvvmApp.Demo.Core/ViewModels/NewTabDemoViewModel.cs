using System;
using System.Threading;
using System.Threading.Tasks;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.Logging;
using ReactiveUI.SourceGenerators;

namespace Caramelo.MvvmApp.Demo.Core.ViewModels;

[NewTab]
public partial class NewTabDemoViewModel : MvvmViewModel
{
    private static int instanceCounter;

    [Reactive] private int tabNumber;
    [Reactive] private string editableTitle;

    public NewTabDemoViewModel(IServiceProvider service) : base(service)
    {
        TabNumber = Interlocked.Increment(ref instanceCounter);
        editableTitle = $"Nova Aba #{TabNumber}";
        Title = editableTitle;
        this.WhenAnyValue(x => x.EditableTitle)
            .Subscribe(value => Title = value ?? string.Empty);
    }

    [ReactiveCommand]
    private async Task OpenAnotherTabAsync()
    {
        Log.LogInformation("Abrindo mais uma nova aba");
        await Navigation.GoToAsync<NewTabDemoViewModel>();
    }

    [ReactiveCommand]
    private async Task OpenDetailInsideThisTabAsync()
    {
        Log.LogInformation("Navegando dentro desta aba para DetailView");
        await Navigation.GoToAsync<DetailViewModel, string>($"Vindo da Aba #{TabNumber}");
    }

    [ReactiveCommand]
    private async Task CloseTabAsync()
    {
        Log.LogInformation("Fechando esta aba");
        await Navigation.GoBackAsync(this);
    }
}
