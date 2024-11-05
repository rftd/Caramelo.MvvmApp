using System.Reactive;
using Caramelo.MvvmApp.Dialogs;
using ReactiveUI;

namespace Caramelo.MvvmApp.ViewModel;

public sealed class ConfirmDialogViewModel : MvvmDialogViewModel<DialogMensageOptions, bool>
{
    #region Fields

    private string message = string.Empty;

    #endregion Fields

    #region Constructors

    public ConfirmDialogViewModel(IServiceProvider service) : base(service)
    {
        YesCommand = ReactiveCommand.Create(OnYesButtonPressed);
        NoCommand = ReactiveCommand.Create(OnNoButtonPressed);
    }

    #endregion Constructors

    #region Properties

    public ReactiveCommand<Unit, Unit> YesCommand { get; }
    
    public ReactiveCommand<Unit, Unit> NoCommand { get; }
    
    public string Message
    {
        get => message;
        set => this.RaiseAndSetIfChanged(ref message, value);
    }

    #endregion Properties

    #region Methods

    public override void Initialize(DialogMensageOptions parameter)
    {
        Title = parameter.Titulo;
        Message = parameter.Mensagem;
    }

    private void OnYesButtonPressed() => SetResult(true);
    
    private void OnNoButtonPressed() => SetResult(false);

    #endregion Methods
}