using Caramelo.MvvmApp.Demo.Core.ViewModels;
using Caramelo.MvvmApp.WPF.Controls;

namespace Caramelo.MvvmApp.WPF.Demo.Views;

public partial class WpfAppBootstrapperView : MvvmWindow<AppBootstrapperViewModel>
{
    public WpfAppBootstrapperView()
    {
        InitializeComponent();
    }
}