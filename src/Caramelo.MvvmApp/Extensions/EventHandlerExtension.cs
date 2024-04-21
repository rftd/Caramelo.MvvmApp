using System.Collections.Specialized;
using System.ComponentModel;

namespace Caramelo.MvvmApp.Extensions;

/// <summary>
///     Classe EventHandlerExtension.
/// </summary>
public static class EventHandlerExtension
{
    /// <summary>
    ///     Chama o evento.
    /// </summary>
    /// <param name="eventHandler">The event handler.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    public static void Raise(this EventHandler? eventHandler, object sender, EventArgs e)
    {
        if (eventHandler == null)
            return;

        if (eventHandler.Target is ISynchronizeInvoke {InvokeRequired: true} synchronizeInvoke)
            synchronizeInvoke.Invoke(eventHandler, new[] {sender, e});
        else
            eventHandler.DynamicInvoke(sender, e);
    }

    /// <summary>
    ///     Chama o evento.
    /// </summary>
    /// <param name="eventHandler">The event handler.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    public static void Raise(this NotifyCollectionChangedEventHandler? eventHandler, object sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (eventHandler == null)
            return;

        if (eventHandler.Target is ISynchronizeInvoke {InvokeRequired: true} synchronizeInvoke)
            synchronizeInvoke.Invoke(eventHandler, new[] {sender, e});
        else
            eventHandler.DynamicInvoke(sender, e);
    }

    /// <summary>
    ///     Chama o evento.
    /// </summary>
    /// <param name="eventHandler">The event handler.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    public static void Raise(this PropertyChangedEventHandler? eventHandler, object sender, PropertyChangedEventArgs e)
    {
        if (eventHandler == null)
            return;

        if (eventHandler.Target is ISynchronizeInvoke {InvokeRequired: true} synchronizeInvoke)
            synchronizeInvoke.Invoke(eventHandler, new[] {sender, e});
        else
            eventHandler.DynamicInvoke(sender, e);
    }

    /// <summary>
    ///     Chama o evento.
    /// </summary>
    /// <param name="eventHandler">The event handler.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    public static void Raise(this PropertyChangingEventHandler? eventHandler, object sender,
        PropertyChangingEventArgs e)
    {
        if (eventHandler == null)
            return;

        if (eventHandler.Target is ISynchronizeInvoke {InvokeRequired: true} synchronizeInvoke)
            synchronizeInvoke.Invoke(eventHandler, new[] {sender, e});
        else
            eventHandler.DynamicInvoke(sender, e);
    }

    /// <summary>
    ///     Chama o evento.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventHandler">The event handler.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    public static void Raise<T>(this EventHandler<T>? eventHandler, object? sender, T? e) where T : EventArgs
    {
        if (eventHandler == null)
            return;

        if (eventHandler.Target is ISynchronizeInvoke {InvokeRequired: true} synchronizeInvoke)
            synchronizeInvoke.Invoke(eventHandler, new[] {sender, e});
        else
            eventHandler.DynamicInvoke(sender, e);
    }

    /// <summary>
    ///     Chama o evento.
    /// </summary>
    /// <param name="eventHandler">The event handler.</param>
    /// <param name="sender">The sender.</param>
    public static void Raise(this EventHandler<EventArgs>? eventHandler, object sender)
    {
        if (eventHandler == null)
            return;

        var e = EventArgs.Empty;
        if (eventHandler.Target is ISynchronizeInvoke {InvokeRequired: true} synchronizeInvoke)
            synchronizeInvoke.Invoke(eventHandler, new[] {sender, e});
        else
            eventHandler.DynamicInvoke(sender, e);
    }

    /// <summary>
    ///     Chama o evento.
    /// </summary>
    /// <param name="eventHandler">The event handler.</param>
    /// <param name="sender">The sender.</param>
    public static void Raise(this EventHandler? eventHandler, object sender)
    {
        if (eventHandler == null)
            return;

        var e = EventArgs.Empty;
        if (eventHandler.Target is ISynchronizeInvoke {InvokeRequired: true} synchronizeInvoke)
            synchronizeInvoke.Invoke(eventHandler, new[] {sender, e});
        else
            eventHandler.DynamicInvoke(sender, e);
    }
}