using System;
using Avalonia;

namespace Caramelo.MvvmApp.Avalonia;

internal class AvaloniaConfiguration : IAvaloniaConfiguration
{
    #region Fields

    private Action<AppBuilder> configure;

    #endregion Fields

    #region Constructors

    public AvaloniaConfiguration(Action<AppBuilder> configure)
    {
        this.configure = configure;
    }

    #endregion Constructors

    #region Methods

    public void Configure(AppBuilder builder) => configure(builder);

    #endregion Methods
}