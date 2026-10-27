using System;
using System.Threading.Tasks;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.Logging;
using ReactiveUI.SourceGenerators;

namespace Caramelo.MvvmApp.Demo.Core.ViewModels;

[CurrentTab]
public partial class DetailViewModel : MvvmViewModel<string>
{
    [Reactive] private string parameterReceived = string.Empty;

    public DetailViewModel(IServiceProvider service) : base(service)
    {
        Title = "Detalhe (Nível 2)";
    }

    public override void Initialize(string parameter)
    {
        ParameterReceived = parameter;
        Log.LogInformation("DetailViewModel inicializado com parâmetro: {Parameter}", parameter);
    }

    [ReactiveCommand]
    private async Task OpenSubDetailAsync()
    {
        Log.LogInformation("Navegando ainda mais fundo (Nível 3) na aba atual");
        await Navigation.GoToAsync<SubDetailViewModel>();
    }

    [ReactiveCommand]
    private async Task GoBackAsync()
    {
        Log.LogInformation("Voltando para a tela anterior da aba");
        await Navigation.GoBackAsync(this);
    }
}
