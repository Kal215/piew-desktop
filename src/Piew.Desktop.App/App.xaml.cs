using System.Threading;
using System.Windows;

namespace Piew.Desktop.App;

public partial class App : Application
{
    private static Mutex? _singleInstanceMutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        const string mutexName = "Global\\PiewDesktopSingleInstanceMutex";
        _singleInstanceMutex = new Mutex(true, mutexName, out var isNewInstance);

        if (!isNewInstance)
        {
            // Sudah ada instance berjalan
            Shutdown();
            return;
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
