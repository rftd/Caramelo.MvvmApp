using Caramelo.MvvmApp.ViewModel;

namespace Caramelo.MvvmApp.Dialogs;

public class DialogOptions
{
    public IMvvmViewModel? Owner { get; set; }
    
    public string Titulo { get; set; } = string.Empty;
    
    public bool CanMinimize { get; set; }

    public bool CanMaximize { get; set; }

}