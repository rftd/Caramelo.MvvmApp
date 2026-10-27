using System;
using System.Threading.Tasks;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.Logging;
using ReactiveUI.SourceGenerators;

namespace Caramelo.MvvmApp.Demo.Core.ViewModels;

[CurrentTab]
public partial class DialogsDemoViewModel : MvvmViewModel
{
    [Reactive] private string lastResult = "Nenhum diálogo executado ainda.";

    public DialogsDemoViewModel(IServiceProvider service) : base(service)
    {
        Title = "Diálogos";
    }

    [ReactiveCommand]
    private async Task ShowInfoAsync()
    {
        Log.LogInformation("Exibindo diálogo Info");
        await Dialogs.InfoAsync("Esta é uma mensagem de informação padrão do Caramelo.MvvmApp.");
        LastResult = "Info exibido com sucesso.";
    }

    [ReactiveCommand]
    private async Task ShowWarningAsync()
    {
        Log.LogInformation("Exibindo diálogo Warning");
        await Dialogs.WarnAsync("Atenção: Esta é uma mensagem de aviso.");
        LastResult = "Warning exibido com sucesso.";
    }

    [ReactiveCommand]
    private async Task ShowErrorAsync()
    {
        Log.LogInformation("Exibindo diálogo Error");
        await Dialogs.ErroAsync("Ocorreu um erro simulado para demonstração.");
        LastResult = "Erro exibido com sucesso.";
    }

    [ReactiveCommand]
    private async Task ShowConfirmAsync()
    {
        Log.LogInformation("Exibindo diálogo de Confirmação");
        var confirmed = await Dialogs.ConfirmAsync("Confirmação", "Você confirma esta operação?");
        LastResult = confirmed ? "Usuário confirmou (Sim)." : "Usuário cancelou (Não).";
    }

    [ReactiveCommand]
    private async Task ShowInputAsync()
    {
        Log.LogInformation("Exibindo diálogo de Entrada de Texto");
        var text = await Dialogs.InputAsync("Entrada", "Digite o seu nome:");
        LastResult = string.IsNullOrWhiteSpace(text)
            ? "Nenhum texto informado ou diálogo cancelado."
            : $"Texto digitado: \"{text}\"";
    }

    [ReactiveCommand]
    private async Task GoBackAsync()
    {
        await Navigation.GoBackAsync(this);
    }
}
