using System;
using System.Threading.Tasks;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.Logging;
using ReactiveUI.SourceGenerators;

namespace Caramelo.MvvmApp.Demo.Core.ViewModels;

[CurrentTab]
public partial class PromptResultViewModel : MvvmViewModel<string, string>
{
    [Reactive] private string parameterInfo = string.Empty;
    [Reactive] private string inputResult = "Valor de teste gerado";

    public PromptResultViewModel(IServiceProvider service) : base(service)
    {
        Title = "Fornecer Resultado";
    }

    public override void Initialize(string parameter)
    {
        ParameterInfo = parameter;
        Log.LogInformation("PromptResultViewModel inicializado com parâmetro: {Parameter}", parameter);
    }

    [ReactiveCommand]
    private async Task ConfirmResultAsync()
    {
        Log.LogInformation("Confirmando resultado: {Result}", InputResult);
        await Navigation.GoBackAsync(this, InputResult);
    }

    [ReactiveCommand]
    private async Task CancelAsync()
    {
        Log.LogInformation("Cancelando retorno de resultado");
        await Navigation.GoBackAsync(this, string.Empty);
    }
}
