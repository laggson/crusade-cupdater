using Laggson.Common.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace Updater.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            WriteProgramJson();
            //UpdateHelper.InstallUpdater();
            //UpdateHelper.LaunchUpdater();
        }

        private void Form1_Shown(object sender, System.EventArgs e)
        {
            //Application.Exit();
        }

        // Idee: Doch ne WebApi, die aus den hochgeladenen Datein automatisch die Version rauszieht. Dann kann ich das ändern nicht vergessen :D
        private void WriteProgramJson()
        {
            var data = new List<UpdateEntry>();

            data.Add(new UpdateEntry { Name = "TimeCalc", Version = "1.1.16" });
            data.Add(new UpdateEntry { Name = "FWA2", Version = "1.0.0" });

            var result = data.ToJson();
        }
    }
}
