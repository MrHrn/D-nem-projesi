using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiyetProgrami
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string anaKlasorYolu = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
            // DataDirectory'yi (yani |DataDirectory| makrosunu) bu ana klasöre eşitliyoruz
            AppDomain.CurrentDomain.SetData("DataDirectory", anaKlasorYolu);
            // ----------------------------
            Application.Run(new login());
        }
    }
}
