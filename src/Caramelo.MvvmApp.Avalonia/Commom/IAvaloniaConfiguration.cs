using Avalonia;

namespace Caramelo.MvvmApp.Avalonia;

internal interface IAvaloniaConfiguration
{
    void Configure(AppBuilder builder);
}