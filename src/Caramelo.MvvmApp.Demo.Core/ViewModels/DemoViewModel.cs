using System;
using Caramelo.MvvmApp.ViewModel;
using Microsoft.Extensions.Logging;

namespace Caramelo.MvvmApp.Demo.Core.ViewModels;

public class DemoViewModel : MvvmViewModel
{
    public DemoViewModel(IServiceProvider service) : base(service)
    {
        Title = "Demo View 1";
        Log.LogInformation("Demo View Created");
    }
}