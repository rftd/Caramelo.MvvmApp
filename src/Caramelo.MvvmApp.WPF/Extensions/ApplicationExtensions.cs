using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Caramelo.MvvmApp.WPF.Extensions;

public static class ApplicationExtensions
{
    public static void Restart(this Application app, params string[] args)
    {
        var startInfo = new ProcessStartInfo
        {
            UseShellExecute = true,
            WorkingDirectory = Environment.CurrentDirectory,
            FileName = Environment.GetCommandLineArgs()[0],
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