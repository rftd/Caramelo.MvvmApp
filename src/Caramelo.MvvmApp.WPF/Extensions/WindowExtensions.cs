﻿using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Caramelo.MvvmApp.WPF.Extensions;

internal static class WindowExtensions
{
    // from winuser.h
    private const int GWL_STYLE      = -16,
        WS_MAXIMIZEBOX = 0x10000,
        WS_MINIMIZEBOX = 0x20000;

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hwnd, int index, int value);

    public static void HideMinimize(this Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        var currentStyle = GetWindowLong(hwnd, GWL_STYLE);

        SetWindowLong(hwnd, GWL_STYLE, currentStyle & ~WS_MINIMIZEBOX);
    }
    
    public static void HideMaximize(this Window window)
    {
        var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
        var currentStyle = GetWindowLong(hwnd, GWL_STYLE);

        SetWindowLong(hwnd, GWL_STYLE, currentStyle & ~WS_MAXIMIZEBOX);
    }
}