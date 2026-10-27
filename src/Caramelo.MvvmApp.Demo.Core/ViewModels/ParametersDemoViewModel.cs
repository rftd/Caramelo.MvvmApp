using System;
using System.Threading.Tasks;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.Logging;
using ReactiveUI.SourceGenerators;

namespace Caramelo.MvvmApp.Demo.Core.ViewModels;

[CurrentTab]
public partial class ParametersDemoViewModel : MvvmViewModel
{
    [Reactive] private string returnedResult = "Nenhum resultado recebido ainda.";

    public ParametersDemoViewModel(IServiceProvider service) : base(service)
    {
        Title = "Navegação com Resultados";
    }

    [ReactiveCommand]
    private async Task RequestResultAsync()
    {
        Log.LogInformation("Navegando aguardando retorno de PromptResultViewModel");
        var result = await Navigation.GoToAsync<PromptResultViewModel, string, string>("Configuração inicial");
        ReturnedResult = string.IsNullOrEmpty(result)
            ? "Operação cancelada ou retorno nulo."
            : $"Resultado retornado com sucesso: \"{result}\"";
    }

    [ReactiveCommand]
    private async Task GoBackAsync()
    {
        await Navigation.GoBackAsync(this);
    }
}
