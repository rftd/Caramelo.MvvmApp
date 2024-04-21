namespace Caramelo.MvvmApp;

/// <summary>
///     Base class for members implementing <see cref="IDisposable" />.
/// </summary>
public abstract class MvvmDisposable : IDisposable
{
    #region Properties

    /// <summary>
    ///     Gets a value indicating whether this <see cref="MvvmDisposable" /> is disposed.
    /// </summary>
    /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
    public bool IsDisposed { get; private set; }

    #endregion Properties

    #region Public Methods

    /// <inheritdoc />
    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        // Dispose all managed and unmanaged resources.
        Dispose(true);

        // Take this object off the finalization queue and prevent finalization code for this
        // object from executing a second time.
        GC.SuppressFinalize(this);
    }

    #endregion Public Methods

    #region Desctructors

    /// <summary>
    ///     Finalizes an instance of the <see cref="MvvmDisposable" /> class. Releases unmanaged
    ///     resources and performs other cleanup operations before the <see cref="MvvmDisposable" />
    ///     is reclaimed by garbage collection. Will run only if the
    ///     Dispose method does not get called.
    /// </summary>
    ~MvvmDisposable()
    {
        Dispose(false);
    }

    #endregion Desctructors

    #region Private Methods

    /// <summary>
    ///     Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    ///     <c>true</c> to release both managed and unmanaged resources;
    ///     <c>false</c> to release only unmanaged resources, called from the finalizer only.
    /// </param>
    private void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (IsDisposed)
            return;

        // If disposing managed and unmanaged resources.
        if (disposing) DisposeManaged();

        DisposeUnmanaged();

        IsDisposed = true;
    }

    #endregion Private Methods

    #region Protected Methods

    /// <summary>
    ///     Disposes the managed resources implementing <see cref="IDisposable" />.
    /// </summary>
    protected virtual void DisposeManaged()
    {
    }

    /// <summary>
    ///     Disposes the unmanaged resources implementing <see cref="IDisposable" />.
    /// </summary>
    protected virtual void DisposeUnmanaged()
    {
    }

    #endregion Protected Methods
}