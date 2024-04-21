using Caramelo.MvvmApp.Avalonia.Controls;
using Caramelo.MvvmApp.Demo.Core.ViewModels;

namespace Caramelo.MvvmApp.Avalonia.Demo.Views
{
    public partial class AvaloniaBootstrapperView : MvvmWindow<AppBootstrapperViewModel>
    {
        public AvaloniaBootstrapperView()
        {
            InitializeComponent();
        }
    }
}