﻿using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls.ApplicationLifetimes;

namespace Caramelo.MvvmApp.Avalonia.Extensions;

public static class ClassicDesktopApplicationExtensions
{
    public static void Restart(this IClassicDesktopStyleApplicationLifetime app, params string[] args)
    {
        var appPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        if (string.IsNullOrEmpty(appPath)) throw new Exception();
        
        var exeName = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
        exeName += OperatingSystem.IsWindows() ? ".exe" : "";
        var exePath = Path.Combine(appPath, exeName);
        
        var startInfo = new ProcessStartInfo
        {
            UseShellExecute = true,
            WorkingDirectory = Environment.CurrentDirectory,
            FileName = exePath,
            Arguments = string.Join(" ", args)
        };

        try
        {
            Process.Start(startInfo);
        }
        catch (Exception)
        {
            //ignore
        }
        
        app.Shutdown();
    }
}