using Avalonia.Markup.Xaml;
using Caramelo.MvvmApp.Demo.Core.ViewModels;

namespace Caramelo.MvvmApp.Avalonia.Demo;

public partial class App : DemoApp
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

public class DemoApp : MvvmApplication<AppBootstrapperViewModel>
{
    
}