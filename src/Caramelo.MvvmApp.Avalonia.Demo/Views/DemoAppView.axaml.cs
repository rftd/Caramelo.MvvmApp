using System;
using Caramelo.MvvmApp.Demo.Core.ViewModels;
using ReactiveUI;

namespace Caramelo.MvvmApp.Avalonia.Demo.Views
{
    public partial class DemoAppView : MvvmWindow<DemoAppViewModel>
    {
        public DemoAppView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                ViewModel.OpenDemoViewCommand.Execute().Subscribe();
            });
        }
    }
}