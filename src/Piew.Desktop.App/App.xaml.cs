using System.Threading;
using System.Windows;

namespace Piew.Desktop.App;

public partial class App : Application
{
    private static Mutex? _singleInstanceMutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            MessageBox.Show(
                $"Terjadi kesalahan saat memulai aplikasi:\n{args.ExceptionObject}",
                "PIEW Desktop - Peringatan",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        };

        DispatcherUnhandledException += (s, args) =>
        {
            MessageBox.Show(
                $"Terjadi kesalahan aplikasi:\n{args.Exception.Message}",
                "PIEW Desktop - Peringatan",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            args.Handled = true;
        };

        try
        {
            const string mutexName = "PiewDesktopSingleInstanceMutex";
            _singleInstanceMutex = new Mutex(true, mutexName, out var isNewInstance);

            if (!isNewInstance)
            {
                MessageBox.Show(
                    "Aplikasi PIEW Desktop sudah berjalan di latar belakang (periksa area dekat jam di pojok kanan bawah).",
                    "PIEW Desktop",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                Shutdown();
                return;
            }
        }
        catch
        {
            // Abaikan jika mutex gagal agar aplikasi tetap bisa terbuka
        }

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _singleInstanceMutex?.ReleaseMutex();
        _singleInstanceMutex?.Dispose();
        base.OnExit(e);
    }
}
