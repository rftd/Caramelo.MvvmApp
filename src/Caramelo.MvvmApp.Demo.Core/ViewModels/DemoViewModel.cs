using System;
using Caramelo.MvvmApp.ViewModel;

namespace Caramelo.MvvmApp.Demo.Core.ViewModels;

public class DemoViewModel : MvvmViewModel
{
    public DemoViewModel(IServiceProvider service) : base(service)
    {
        Title = "Demo View 1";
    }
}