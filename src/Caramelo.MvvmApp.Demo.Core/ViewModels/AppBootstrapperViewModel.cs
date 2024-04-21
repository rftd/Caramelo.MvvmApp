using System;
using Caramelo.MvvmApp.ViewModel;

namespace Caramelo.MvvmApp.Demo.Core.ViewModels;

public class AppBootstrapperViewModel : RouterViewModel
{
    public AppBootstrapperViewModel(IServiceProvider service) : base(service)
    {
    }
}