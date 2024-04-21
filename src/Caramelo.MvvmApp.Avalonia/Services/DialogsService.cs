using System;
using System.Reactive;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Caramelo.MvvmApp.Dialogs;
using Caramelo.MvvmApp.Services;
using Caramelo.MvvmApp.ViewModel;
using DynamicData.Binding;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Caramelo.MvvmApp.Avalonia.Services;

public sealed class DialogsService : IDialogService
{
    #region Fields

    private readonly IServiceProvider service;

    #endregion Fields

    #region Constructors

    public DialogsService(IServiceProvider service)
    {
        this.service = service;
    }

    #endregion Constructors

    #region Properties

    private static IClassicDesktopStyleApplicationLifetime DesktopApp =>
        (IClassicDesktopStyleApplicationLifetime) Application.Current?.ApplicationLifetime! ?? throw new InvalidOperationException();

    #endregion Properties
    
    #region Methods

    public Task ShowAsync(string title, string message)
    {
        return ShowAsync<MessageDialogViewModel, Unit, DialogMensageOptions>(new DialogMensageOptions { Titulo = title, Mensagem = message });
    }

    public Task InfoAsync(string message)
    {
        return ShowAsync<MessageDialogViewModel, Unit, DialogMensageOptions>(new DialogMensageOptions { Titulo = "Informação", Mensagem = message });
    }

    public Task WarnAsync(string message)
    {
        return ShowAsync<MessageDialogViewModel, Unit, DialogMensageOptions>(new DialogMensageOptions { Titulo = "Aviso", Mensagem = message });
    }

    public Task ErroAsync(string message)
    {
        return ShowAsync<MessageDialogViewModel, Unit, DialogMensageOptions>(new DialogMensageOptions { Titulo = "Erro", Mensagem = message });
    }

    public Task<string> InputAsync(string title, string message)
    {
        return ShowAsync<InputDialogViewModel, string, DialogMensageOptions>(new DialogMensageOptions { Titulo = title, Mensagem = message });
    }

    public Task<bool> ConfirmAsync(string title, string message)
    {
        return ShowAsync<ConfirmDialogViewModel, bool, DialogMensageOptions>(new DialogMensageOptions { Titulo = title, Mensagem = message });
    }
    
    public Task ShowAsync<TViewModel>(DialogOptions options) where TViewModel : MvvmDialogViewModel<DialogOptions, Unit>
    {
        return ShowAsync<TViewModel, Unit, DialogOptions>(options);
    }

    public Task ShowAsync<TViewModel>() where TViewModel : MvvmDialogViewModel<DialogOptions, Unit>
    {
        return ShowAsync<TViewModel, Unit, DialogOptions>(new DialogOptions());
    }

    public async Task<TResult> ShowAsync<TViewModel, TResult, TParameter>(TParameter parameter)
        where TViewModel : MvvmDialogViewModel<TParameter, TResult>
        where TParameter : DialogOptions
        where TResult : notnull
    {

        var model = service.GetRequiredService<TViewModel>();
        model.Initialize(parameter);
        
        var viewLocator = service.GetRequiredService<IViewLocator>();
        var viewFor = viewLocator.ResolveView(model);
        if (viewFor == null) throw new ApplicationException("View Não cadastrada.");
        viewFor.ViewModel = model;

        var window = viewFor switch
        {
            UserControl mvvmUserControl => CreateWindow(mvvmUserControl),
            Window view => view,
            _ => throw new ApplicationException("View Não Suportada.")
        };
        
        ConfigureWindow<TViewModel, TParameter, TResult>(window, model, parameter);
        await window.ShowDialog(DesktopApp.MainWindow!);

        try
        {
            return await model.DialogResult.ToTask();
        }
        catch (Exception)
        {
            return default!;
        }
    }

    private static Window CreateWindow(Control dialog)
    {
        Grid.SetRow(dialog, 0);
        Grid.SetColumn(dialog, 0);
        
        var window = new Window
        {
            Content = new DockPanel
            {
                LastChildFill = true,
                Children = { dialog }
            },
            SizeToContent = SizeToContent.WidthAndHeight
        };

        return window;
    }

    private static void ConfigureWindow<TViewModel, TParameter, TResult>(Window window, TViewModel viewModel, TParameter parameter)
        where TViewModel : MvvmDialogViewModel<TParameter, TResult>
        where TParameter : DialogOptions
        where TResult : notnull
    {
        window.Bind(Window.TitleProperty, NotifyPropertyChangedEx.WhenPropertyChanged<TViewModel, string>(viewModel, x => x.Title));
        window.Closing += (_, args) => args.Cancel = !viewModel.CanClose;
        NotifyPropertyChangedEx.WhenPropertyChanged<TViewModel, bool>(viewModel, x => x.CanClose)
            .Subscribe(x =>
            {
                if(!x.Value) return;
                
                window.Close();
            });

        try
        {
            window.Icon = DesktopApp.MainWindow?.Icon;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        catch (Exception)
        {
            //
        }
    }    
    
    #endregion Methods
}