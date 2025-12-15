using System;
using System.Threading;
using LogEventsLib;

namespace ServeurDAO
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsommateurConsole consoleConsommateur = new ConsommateurConsole();
            ConsommateurFichier fichierConsommateur = new ConsommateurFichier("log_serveur.txt");
            
            LogEvents.Instance.AjouterConsommateur(consoleConsommateur.TraiterEvent);
            LogEvents.Instance.AjouterConsommateur(fichierConsommateur.TraiterEvent);

            string connectionString = @"Server=(localdb)\mssqllocaldb;Database=DessinDB;Integrated Security=true;";
            ServeurTCP serveur = new ServeurTCP(8888, connectionString);
            
            serveur.Demarrer();
            
            Console.WriteLine("Serveur DAO démarré. Appuyez sur une touche pour arrêter...");
            Console.ReadKey();
            
            serveur.Arreter();
            LogEvents.Instance.Stop();
        }
    }
}
