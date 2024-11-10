using Caramelo.MvvmApp.Demo.Core.ViewModels;
using Caramelo.MvvmApp.WPF.Demo.Views;
using Caramelo.MvvmApp.WPF.Extensions;

namespace Caramelo.MvvmApp.WPF.Demo;

public class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var builder = MvvmApp.CreateBuilder();
        builder.UseWpf<App, DemoAppViewModel, DemoAppView>();
        
        var app = builder.Build();
        app.Run();
    }
}