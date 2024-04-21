using ReactiveUI;

namespace Caramelo.MvvmApp.View;

public interface IViewDialog : IViewFor
{
    void SetOwner(IViewDialog viewDialog);
    
    bool? ShowDialog();
}