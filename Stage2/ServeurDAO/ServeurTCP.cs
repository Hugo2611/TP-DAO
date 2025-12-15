using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using FiguresLib;
using LogEventsLib;
using ORMLib;

namespace ServeurDAO
{
    public class CommandeServeur
    {
        public string Type { get; set; }
        public Dictionary<string, object> Parametres { get; set; }
    }

    public class ReponseServeur
    {
        public bool Succes { get; set; }
        public string Message { get; set; }
        public object Donnees { get; set; }
    }

    public class ServeurTCP
    {
        private TcpListener _listener;
        private bool _running;
        private Thread _threadServeur;
        private List<Thread> _threadsClients;
        private GestionnaireDAO _dao;

        public ServeurTCP(int port, string connectionString)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _threadsClients = new List<Thread>();
            _dao = new GestionnaireDAO(connectionString);
        }

        public void Demarrer()
        {
            try
            {
                _running = true;
                _listener.Start();
                LogEvents.Instance.Push(new Event(NiveauEvent.Information, "Serveur TCP démarré"));

                _threadServeur = new Thread(ThreadServeur);
                _threadServeur.IsBackground = false;
                _threadServeur.Start();
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur démarrage serveur: {ex.Message}"));
            }
        }

        private void ThreadServeur()
        {
            while (_running)
            {
                try
                {
                    if (_listener.Pending())
                    {
                        TcpClient client = _listener.AcceptTcpClient();
                        LogEvents.Instance.Push(new Event(NiveauEvent.Information, "Client connecté"));

                        Thread threadClient = new Thread(() => ThreadClient(client));
                        threadClient.IsBackground = false;
                        _threadsClients.Add(threadClient);
                        threadClient.Start();
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    if (_running)
                    {
                        LogEvents.Instance.Push(new Event(NiveauEvent.Alerte, $"Erreur acceptation client: {ex.Message}"));
                    }
                }
            }
        }

        private void ThreadClient(TcpClient client)
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] buffer = new byte[8192];

                while (_running && client.Connected)
                {
                    if (stream.DataAvailable)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            CommandeServeur commande = JsonSerializer.Deserialize<CommandeServeur>(json);
                            
                            ReponseServeur reponse = TraiterCommande(commande);
                            
                            string reponseJson = JsonSerializer.Serialize(reponse);
                            byte[] reponseBytes = Encoding.UTF8.GetBytes(reponseJson);
                            stream.Write(reponseBytes, 0, reponseBytes.Length);
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alerte, $"Erreur thread client: {ex.Message}"));
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
                LogEvents.Instance.Push(new Event(NiveauEvent.Information, "Client déconnecté"));
            }
        }

        private ReponseServeur TraiterCommande(CommandeServeur commande)
        {
            try
            {
                switch (commande.Type)
                {
                    case "CreerAlbum":
                        string nomAlbum = commande.Parametres["nom"].ToString();
                        int albumId = _dao.CreerAlbum(nomAlbum);
                        return new ReponseServeur { Succes = true, Message = "Album créé", Donnees = albumId };

                    case "LireAlbums":
                        var albums = _dao.LireAlbums();
                        return new ReponseServeur { Succes = true, Message = "Albums récupérés", Donnees = albums };

                    case "CreerDessin":
                        int albumIdDessin = Convert.ToInt32(commande.Parametres["albumId"]);
                        string dessinJson = commande.Parametres["dessin"].ToString();
                        clsDessin dessin = JsonSerializer.Deserialize<clsDessin>(dessinJson);
                        int dessinId = _dao.CreerDessin(albumIdDessin, dessin);
                        return new ReponseServeur { Succes = true, Message = "Dessin créé", Donnees = dessinId };

                    case "LireDessins":
                        int albumIdLecture = Convert.ToInt32(commande.Parametres["albumId"]);
                        var dessins = _dao.LireDessins(albumIdLecture);
                        return new ReponseServeur { Succes = true, Message = "Dessins récupérés", Donnees = dessins };

                    case "SupprimerAlbum":
                        int albumIdSuppr = Convert.ToInt32(commande.Parametres["albumId"]);
                        _dao.SupprimerAlbum(albumIdSuppr);
                        return new ReponseServeur { Succes = true, Message = "Album supprimé" };

                    default:
                        return new ReponseServeur { Succes = false, Message = "Commande inconnue" };
                }
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur traitement commande: {ex.Message}"));
                return new ReponseServeur { Succes = false, Message = ex.Message };
            }
        }

        public void Arreter()
        {
            try
            {
                _running = false;
                _listener.Stop();

                if (_threadServeur != null && _threadServeur.IsAlive)
                {
                    _threadServeur.Join(2000);
                }

                foreach (var thread in _threadsClients)
                {
                    if (thread != null && thread.IsAlive)
                    {
                        thread.Join(1000);
                    }
                }

                LogEvents.Instance.Push(new Event(NiveauEvent.Information, "Serveur TCP arrêté"));
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur arrêt serveur: {ex.Message}"));
            }
        }
    }
}
