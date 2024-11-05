using System.Reactive;
using Caramelo.MvvmApp.Dialogs;
using ReactiveUI;

namespace Caramelo.MvvmApp.ViewModel;

public sealed class MessageDialogViewModel : MvvmDialogViewModel<DialogMensageOptions, Unit>
{
    #region Fields

    private string message = string.Empty;

    #endregion Fields

    #region Constructors

    public MessageDialogViewModel(IServiceProvider service) : base(service)
    {
        OkCommand = ReactiveCommand.Create(OnOkButtonPressed);
    }

    #endregion Constructors

    #region Properties

    public ReactiveCommand<Unit, Unit> OkCommand { get; }
    
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

    private void OnOkButtonPressed() => SetResult(Unit.Default);

    #endregion Methods
}