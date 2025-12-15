using System;
using System.Windows.Forms;
using LogEventsLib;

namespace WinFormsApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ConsommateurConsole consoleConsommateur = new ConsommateurConsole();
            ConsommateurFichier fichierConsommateur = new ConsommateurFichier("log_winforms.txt");
            
            LogEvents.Instance.AjouterConsommateur(consoleConsommateur.TraiterEvent);
            LogEvents.Instance.AjouterConsommateur(fichierConsommateur.TraiterEvent);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormPrincipal());

            LogEvents.Instance.Stop();
        }
    }
}
