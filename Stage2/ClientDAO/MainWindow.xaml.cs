using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Windows;
using FiguresLib;

namespace ClientDAO
{
    public partial class MainWindow : Window
    {
        private ClientTCP _client;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AfficherStatus(string message)
        {
            txtStatus.Text += $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            txtStatus.ScrollToEnd();
        }

        private void btnConnexion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _client = new ClientTCP(txtHost.Text, int.Parse(txtPort.Text));
                AfficherStatus($"Client configuré pour {txtHost.Text}:{txtPort.Text}");
            }
            catch (Exception ex)
            {
                AfficherStatus($"Erreur: {ex.Message}");
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCreerAlbum_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_client == null)
                {
                    MessageBox.Show("Veuillez d'abord configurer la connexion", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string nomAlbum = Microsoft.VisualBasic.Interaction.InputBox("Nom de l'album:", "Créer Album", "MonAlbum");
                if (string.IsNullOrEmpty(nomAlbum)) return;

                CommandeServeur commande = new CommandeServeur
                {
                    Type = "CreerAlbum",
                    Parametres = new Dictionary<string, object> { { "nom", nomAlbum } }
                };

                ReponseServeur reponse = _client.EnvoyerCommande(commande);
                AfficherStatus($"Création album: {reponse.Message}");
                
                if (reponse.Succes)
                {
                    MessageBox.Show($"Album créé avec ID: {reponse.Donnees}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(reponse.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                AfficherStatus($"Erreur: {ex.Message}");
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLireAlbums_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_client == null)
                {
                    MessageBox.Show("Veuillez d'abord configurer la connexion", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                CommandeServeur commande = new CommandeServeur
                {
                    Type = "LireAlbums",
                    Parametres = new Dictionary<string, object>()
                };

                ReponseServeur reponse = _client.EnvoyerCommande(commande);
                AfficherStatus($"Lecture albums: {reponse.Message}");

                if (reponse.Succes)
                {
                    listBoxResultats.Items.Clear();
                    string json = reponse.Donnees.ToString();
                    var albums = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);
                    foreach (var album in albums)
                    {
                        listBoxResultats.Items.Add($"Album ID: {album["AlbumId"]}, Nom: {album["Nom"]}");
                    }
                }
                else
                {
                    MessageBox.Show(reponse.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                AfficherStatus($"Erreur: {ex.Message}");
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCreerDessin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_client == null)
                {
                    MessageBox.Show("Veuillez d'abord configurer la connexion", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string albumIdStr = Microsoft.VisualBasic.Interaction.InputBox("ID de l'album:", "Créer Dessin", "1");
                if (string.IsNullOrEmpty(albumIdStr)) return;

                clsDessin dessin = new clsDessin("DessinTest", 1.0f);
                dessin.AjouterFigure(new clsLigne(10, 10, 100, 100, Color.Red, "L1"));
                dessin.AjouterFigure(new clsCercle(150, 150, 50, Color.Blue, "C1"));

                string dessinJson = JsonSerializer.Serialize(dessin);

                CommandeServeur commande = new CommandeServeur
                {
                    Type = "CreerDessin",
                    Parametres = new Dictionary<string, object> 
                    { 
                        { "albumId", int.Parse(albumIdStr) },
                        { "dessin", dessinJson }
                    }
                };

                ReponseServeur reponse = _client.EnvoyerCommande(commande);
                AfficherStatus($"Création dessin: {reponse.Message}");

                if (reponse.Succes)
                {
                    MessageBox.Show($"Dessin créé avec ID: {reponse.Donnees}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(reponse.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                AfficherStatus($"Erreur: {ex.Message}");
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLireDessins_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_client == null)
                {
                    MessageBox.Show("Veuillez d'abord configurer la connexion", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string albumIdStr = Microsoft.VisualBasic.Interaction.InputBox("ID de l'album:", "Lire Dessins", "1");
                if (string.IsNullOrEmpty(albumIdStr)) return;

                CommandeServeur commande = new CommandeServeur
                {
                    Type = "LireDessins",
                    Parametres = new Dictionary<string, object> { { "albumId", int.Parse(albumIdStr) } }
                };

                ReponseServeur reponse = _client.EnvoyerCommande(commande);
                AfficherStatus($"Lecture dessins: {reponse.Message}");

                if (reponse.Succes)
                {
                    listBoxResultats.Items.Clear();
                    string json = reponse.Donnees.ToString();
                    listBoxResultats.Items.Add($"Dessins récupérés: {json}");
                }
                else
                {
                    MessageBox.Show(reponse.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                AfficherStatus($"Erreur: {ex.Message}");
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
