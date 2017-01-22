using System;
using System.IO;
using System.Net;

namespace Laggson.Common
{
    public static class UpdateHelper
    {
        public static string MainDir => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Laggson Softworks";
        public static string UpdateDir => MainDir + @"\Updater";


        public static void InstallUpdater()
        {
            if (!Directory.Exists(MainDir))
                Directory.CreateDirectory(MainDir);

            if (!Directory.Exists(UpdateDir))
                //return;
                Directory.CreateDirectory(UpdateDir);

            RetrieveUpdater();
        }

        // TODO: Teste diesen shit, yo!
        private static void RetrieveUpdater()
        {
            string updFilePath = "ftp://h2608125.stratoserver.net:22/software/Updater.exe";

            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential("softw", "Volks3-wagen");
                    client.DownloadFile(new Uri(updFilePath), UpdateDir + @"\Updater.exe");
                }
            }
            catch (WebException e) when (e.Message.Contains("404"))
            { }
        }
    }
}
