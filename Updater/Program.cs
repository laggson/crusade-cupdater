using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            app.Run(new MainWindow(args));
        }
    }
    
    static class Extensions
    {
        public static bool IsLargerThan(this Version maybeLargerVer, Version otherVer)
        {
            int compared = maybeLargerVer.CompareTo(otherVer);

            if (compared == -1)
                throw new ArgumentException("Du kannst doch keine neuere Version haben als der Server vallah.");

            return compared == 1;
        }
    }
}
