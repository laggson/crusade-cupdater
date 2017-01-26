using System;
using System.Windows;
using UpdaterWpf;

namespace Updater.Wpf
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            Application app = new Application();
            app.DispatcherUnhandledException += App_DispatcherUnhandledException;

            app.Run(new MainWindow(args));
        }

        private static void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Es ist eine Ausnahme vom Typ " + e.Exception.ToString() + " aufgetreten. Die Anwendung muss beendet werden."
                + Environment.NewLine + "Zusätzliche Information: " + e.Exception.Message);
        }
    }
}
