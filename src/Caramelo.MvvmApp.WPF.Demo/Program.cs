using Caramelo.MvvmApp.Demo.Core.ViewModels;
using Caramelo.MvvmApp.WPF.Extensions;
using WpfAppBootstrapperView = Caramelo.MvvmApp.WPF.Demo.Views.WpfAppBootstrapperView;

namespace Caramelo.MvvmApp.WPF.Demo;

public class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var builder = MvvmApp.CreateBuilder();
        builder.UseWpf<App, AppBootstrapperViewModel, WpfAppBootstrapperView>();
        
        var app = builder.Build();
        app.Run();
    }
}