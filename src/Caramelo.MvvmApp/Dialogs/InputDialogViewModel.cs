using System.Reactive;
using Caramelo.MvvmApp.ViewModel;
using ReactiveUI;

namespace Caramelo.MvvmApp.Dialogs;

public sealed class InputDialogViewModel : MvvmDialogViewModel<DialogMensageOptions, string>
{
    #region Fields

    private string message = string.Empty;
    private string input = string.Empty;

    #endregion Fields

    #region Constructors

    public InputDialogViewModel(IServiceProvider service) : base(service)
    {
        ConfirmCommand = ReactiveCommand.Create(OnConfirmButtonPressed);
        CancelCommand = ReactiveCommand.Create(OnCancelButtonPressed);
    }

    #endregion Constructors

    #region Properties

    public ReactiveCommand<Unit, Unit> ConfirmCommand { get; }
    
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    
    public string Message
    {
        get => message;
        set => this.RaiseAndSetIfChanged(ref message, value);
    }

    public string Input
    {
        get => input;
        set => this.RaiseAndSetIfChanged(ref input, value);
    }

    #endregion Properties

    #region Methods

    public override void Initialize(DialogMensageOptions parameter)
    {
        Title = parameter.Titulo;
        Message = parameter.Mensagem;
    }

    private void OnConfirmButtonPressed() => SetResult(Input);
    
    private void OnCancelButtonPressed() => SetResult(string.Empty);

    #endregion Methods
}