﻿using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Caramelo.MvvmApp.WPF.Extensions;

internal static class WindowExtensions
{
    #region Fields

    private const int GWL_STYLE = -16;
    private const int WS_MAXIMIZEBOX = 0x10000;
    private const int WS_MINIMIZEBOX = 0x20000;

    #endregion Fields

    #region Imports

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hwnd, int index, int value);

    #endregion Imports

    #region Methods

    public static void HideMinimize(this Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        var currentStyle = GetWindowLong(hwnd, GWL_STYLE);

        SetWindowLong(hwnd, GWL_STYLE, currentStyle & ~WS_MINIMIZEBOX);
    }
    
    public static void HideMaximize(this Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        var currentStyle = GetWindowLong(hwnd, GWL_STYLE);

        SetWindowLong(hwnd, GWL_STYLE, currentStyle & ~WS_MAXIMIZEBOX);
    }

    #endregion Methods
}