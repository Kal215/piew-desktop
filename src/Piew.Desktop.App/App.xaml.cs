using System;
using System.Windows;

namespace Piew.Desktop.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            MessageBox.Show(
                $"Terjadi kesalahan fatal:\n{args.ExceptionObject}",
                "PIEW Desktop - Galat",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        };

        DispatcherUnhandledException += (s, args) =>
        {
            MessageBox.Show(
                $"Peringatan aplikasi:\n{args.Exception.Message}",
                "PIEW Desktop - Peringatan",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            args.Handled = true;
        };

        base.OnStartup(e);
    }
}
