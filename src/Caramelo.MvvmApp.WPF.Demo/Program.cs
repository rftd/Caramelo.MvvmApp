using System.Reflection;
using Caramelo.MvvmApp.Demo.Core.ViewModels;
using Caramelo.MvvmApp.Extensions;
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
        
        // Adiciona as views e os model
        builder.Services.AddViewAndModelFromAssembly(typeof(DemoViewModel).Assembly);
        var app = builder.Build();
        app.Run();
    }
}