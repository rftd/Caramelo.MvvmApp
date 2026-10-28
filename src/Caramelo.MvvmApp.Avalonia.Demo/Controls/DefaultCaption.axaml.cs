using System;
using Avalonia;
using Avalonia.Controls;

namespace Caramelo.MvvmApp.Avalonia.Demo.Controls;

public partial class DefaultCaption : UserControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<DefaultCaption, string>(nameof(Title), defaultValue: string.Empty);

    public DefaultCaption()
    {
        InitializeComponent();

        this.WhenAnyValue(x => x.Title)
            .Subscribe(value => CaptionHeader.Text = value ?? string.Empty);
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}
