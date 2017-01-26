using Laggson.Common;
using Laggson.Common.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using Updater.Wpf;

namespace UpdaterWpf
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string API_URL = "http://h2608125.stratoserver.net:5000/api/";
        private const string FTP_URL = "ftp://h2608125.stratoserver.net";
        
        private IEnumerable<string> _args;
        private double _progress;

        private Dictionary<Args, string> Arguments { get; set; }
        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;

                Dispatcher.Invoke(new Action(() =>
                {
                    LblProg.Content = value + " %";
                    ProgressBar.Value = value;
                }));
            }
        }


        public MainWindow(IEnumerable<string> args)
        {
            InitializeComponent();
            _args = args;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(() => TrySetArgsAndExit(_args));
            thread.Start();
        }


        /// <summary>
        /// Überprüft die WebApi nach einer neuen Version. Wurde diese gefunden, wird der 
        /// Ordner des Systems gelöscht und die neuen Datein eingebunden.
        /// </summary>
        private void UpdateStuff()
        {
            string workPath = Arguments[Args.Directory];

            if (UpdateHelper.IsNewVersionAvailable(Arguments[Args.Name], Arguments[Args.Version]))
            {
                string tempPath = @"C:\Temp\LaggsonDatedZeugUp\";

                DownloadNewFiles(GetFileNames(), tempPath);
                RemoveAllInFolder(workPath);
                MoveFiles(tempPath, workPath);

                Directory.Delete(tempPath);

                Progress = 100;
            }

            StartNewAndExit(GetExePath(workPath));
        }

        private string GetExePath(string path)
        {
            return Directory.GetFiles(path).Single(s => s.EndsWith(".exe") && !s.EndsWith(".vshost.exe"));
        }

        private void StartNewAndExit(string path)
        {
            if (!string.IsNullOrEmpty(path))
                Process.Start(path);

            Environment.Exit(0);
        }

        /// <summary>
        /// Fragt die Dateinamen der neuesten Version von der WebApi ab und gibt sie als <see cref="IEnumerable{string}" zurück/>
        /// </summary>
        /// <returns>Eine Aufzählung der Dateinamen, die für die neueste Version benötigt werden.</returns>
        private IEnumerable<string> GetFileNames()
        {
            return new WebClient().DownloadString(API_URL + "files/" + Arguments[Args.Name]).FromJsonArray<string>();
        }

        /// <summary>
        /// Lädt die übergebenen Dateien herunter und speichert diese im angegebenen Zielordner
        /// </summary>
        /// <param name="fileNames">Die Aufzählung der Dateinamen, nach denen gesucht werden soll.</param>
        /// <param name="destinationFolder">Der Zielordner für die Transaktion</param>
        private void DownloadNewFiles(IEnumerable<string> fileNames, string destinationFolder)
        {
            var client = new WebClient();
            client.Credentials = new NetworkCredential("uguest".Normalize(), "Volks3-wagen".Normalize());

            Directory.CreateDirectory(destinationFolder);
            foreach (var file in fileNames)
            {
                var path = file.Replace('\\', '/');
                var array = path.Split('/');
                string name = array[array.Length - 1];

                try
                {
                    client.DownloadFile(FTP_URL + path, destinationFolder + name);
                }
                catch (WebException e)
                {
                    var desc = ((FtpWebResponse)e.Response).StatusDescription;

                    if (!desc.StartsWith("550"))
                        throw;
                }

                Progress += 45 / fileNames.Count();
            }
            Progress = 55;
        }

        /// <summary>
        /// Verschiebt die Dateien im Ordner <paramref name="oldPath"/> in den Ordner <paramref name="newPath"/>
        /// </summary>
        /// <param name="oldPath">Der Ursprungs-Ordner</param>
        /// <param name="newPath">Der Ziel-Ordner</param>
        private void MoveFiles(string oldPath, string newPath)
        {
            var files = Directory.GetFiles(oldPath);

            foreach (var file in files)
            {
                var array = file.Split('\\');
                string name = array[array.Length - 1];

                File.Move(file, newPath + "\\" + name);

                Progress += 20 / files.Count();
            }

            Progress = 95;
        }

        /// <summary>
        /// Löscht alle Dateien im unter <paramref name="path"/> angegebenen Ordner
        /// </summary>
        /// <param name="path">Der Pfad des zu löschenden Ordners</param>
        private void RemoveAllInFolder(string path)
        {
            var oldFiles = Directory.GetFiles(path);

            foreach (var file in oldFiles)
            {
                File.Delete(file);

                Progress += 20 / oldFiles.Count();
            }

            Progress = 75;
        }

        /// <summary>
        /// Versucht, die eingegebenen Argumente als Settings zu setzen und beendet die Anwendung mit Warnung, 
        /// falls benötigte Argumente fehlen. Überflüssige werden ignoriert
        /// </summary>
        /// <param name="args">Die Aufzählung der Argumente</param>
        /// <param name="separator">Der Separator für die Zuweisungen. Default ist '='</param>
        public void TrySetArgsAndExit(IEnumerable<string> args, char separator = '=')
        {
            try
            {
                SetArguments(args, separator);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Es wurden ungültige Argumente eingegeben. Die Anwendung wird jetzt beendet.");

                Environment.Exit(-1);
            }

            UpdateStuff();
        }

        /// <summary>
        /// Mich solltest du von außen gar nicht aufrufen. Nutze <seealso cref="TrySetArgsAndExit(IEnumerable{string}, char)"/> um das aufzurufen und zu catchen.
        /// </summary>
        protected void SetArguments(IEnumerable<string> args, char separator)
        {
            var data = new Dictionary<Args, string>();
            string current;

            if (args.Count() == 1)
                args = args.First().Split(';');

            Progress = 1;

            current = args.Single(a => a.Contains("Name="));
            data.Add(Args.Name, current.Split(separator)[1]);

            Progress = 4;

            current = args.Single(a => a.Contains("Version="));
            data.Add(Args.Version, current.Split(separator)[1]);

            Progress = 7;

            current = args.Single(a => a.Contains("Directory=") || a.Contains("Path"));
            data.Add(Args.Directory, current.Split(separator)[1]);

            Progress = 10;

            Arguments = data;
        }
    }
}
