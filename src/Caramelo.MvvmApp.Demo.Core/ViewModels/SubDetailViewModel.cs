using System;
using System.Threading.Tasks;
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.Logging;
using ReactiveUI.SourceGenerators;

namespace Caramelo.MvvmApp.Demo.Core.ViewModels;

[CurrentTab]
public partial class SubDetailViewModel : MvvmViewModel
{
    public SubDetailViewModel(IServiceProvider service) : base(service)
    {
        Title = "Sub-Detalhe (Nível 3)";
    }

    [ReactiveCommand]
    private async Task GoBackAsync()
    {
        Log.LogInformation("Voltando do Nível 3 para o Nível 2");
        await Navigation.GoBackAsync(this);
    }
}
