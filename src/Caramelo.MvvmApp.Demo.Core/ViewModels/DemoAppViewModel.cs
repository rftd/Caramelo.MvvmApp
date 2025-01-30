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
    private async Task OpenDemoViewAsync()
    {
        Log.LogInformation("Opening Demo View");
        await Navigation.GoToAsync<DemoViewModel>();
    }
}