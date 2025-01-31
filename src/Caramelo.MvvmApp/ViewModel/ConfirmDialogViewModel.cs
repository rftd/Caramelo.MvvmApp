using Caramelo.MvvmApp.Dialogs;
using ReactiveUI.SourceGenerators;

namespace Caramelo.MvvmApp.ViewModel;

public sealed partial class ConfirmDialogViewModel : MvvmDialogViewModel<DialogMensageOptions, bool>
{
    #region Constructors

    public ConfirmDialogViewModel(IServiceProvider service) : base(service)
    {
        Message = "Confirm";
    }

    #endregion Constructors

    #region Properties

    [Reactive] 
    public partial string Message { get; set; }

    #endregion Properties

    #region Methods

    public override void Initialize(DialogMensageOptions parameter)
    {
        Title = parameter.Titulo;
        Message = parameter.Mensagem;
    }

    
    [ReactiveCommand]
    private void Yes() => SetResult(true);
    
    [ReactiveCommand]
    private void No() => SetResult(false);

    #endregion Methods
}