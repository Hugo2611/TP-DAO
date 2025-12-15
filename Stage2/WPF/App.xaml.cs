using System;
using System.Windows;
using LogEventsLib;

namespace WPFApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            ConsommateurConsole consoleConsommateur = new ConsommateurConsole();
            ConsommateurFichier fichierConsommateur = new ConsommateurFichier("log_wpf.txt");
            
            LogEvents.Instance.AjouterConsommateur(consoleConsommateur.TraiterEvent);
            LogEvents.Instance.AjouterConsommateur(fichierConsommateur.TraiterEvent);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            LogEvents.Instance.Stop();
            base.OnExit(e);
        }
    }
}
