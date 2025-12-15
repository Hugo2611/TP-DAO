using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FiguresLib;
using LogEventsLib;
using Microsoft.Win32;

namespace WPFApp
{
    public partial class MainWindow : Window
    {
        private clsDessin _dessin;

        public MainWindow()
        {
            InitializeComponent();
            InitialiserDessin();
        }

        private void InitialiserDessin()
        {
            try
            {
                _dessin = new clsDessin("MonDessin", 1.0f);
                _dessin.AjouterFigure(new clsLigne(50, 50, 200, 150, System.Drawing.Color.Red, "Ligne1"));
                _dessin.AjouterFigure(new clsCercle(300, 200, 80, System.Drawing.Color.Blue, "Cercle1"));
                _dessin.AjouterFigure(new clsLigne(400, 100, 500, 300, System.Drawing.Color.Green));
                _dessin.AjouterFigure(new clsCercle(600, 150, 50, System.Drawing.Color.Orange));

                LogEvents.Instance.Push(new Event(NiveauEvent.Information, "Dessin initialisé avec succès"));
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur initialisation dessin: {ex.Message}"));
            }
        }

        private void btnDessiner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                canvasDessin.Children.Clear();

                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext dc = drawingVisual.RenderOpen())
                {
                    SupportWPF support = new SupportWPF(dc);
                    clsFigure.Support = support;
                    _dessin.DessinerTout();
                }

                RenderTargetBitmap bmp = new RenderTargetBitmap(
                    (int)canvasDessin.ActualWidth,
                    (int)canvasDessin.ActualHeight,
                    96, 96, PixelFormats.Pbgra32);
                bmp.Render(drawingVisual);

                System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                image.Source = bmp;
                canvasDessin.Children.Add(image);

                LogEvents.Instance.Push(new Event(NiveauEvent.Information, "Dessin rafraîchi"));
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur lors du dessin: {ex.Message}"));
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnChargerJSON_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Fichiers JSON (*.json)|*.json";
                if (openFileDialog.ShowDialog() == true)
                {
                    string json = File.ReadAllText(openFileDialog.FileName);
                    _dessin = JsonSerializer.Deserialize<clsDessin>(json);
                    btnDessiner_Click(sender, e);
                    LogEvents.Instance.Push(new Event(NiveauEvent.Information, $"Dessin chargé: {openFileDialog.FileName}"));
                }
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur chargement JSON: {ex.Message}"));
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSauverJSON_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Fichiers JSON (*.json)|*.json";
                if (saveFileDialog.ShowDialog() == true)
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string json = JsonSerializer.Serialize(_dessin, options);
                    File.WriteAllText(saveFileDialog.FileName, json);
                    LogEvents.Instance.Push(new Event(NiveauEvent.Information, $"Dessin sauvegardé: {saveFileDialog.FileName}"));
                    MessageBox.Show("Dessin sauvegardé avec succès", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur sauvegarde JSON: {ex.Message}"));
                MessageBox.Show($"Erreur: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLogEvents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FormLogEvents formLog = new FormLogEvents();
                formLog.Show();
                LogEvents.Instance.Push(new Event(NiveauEvent.Information, "Fenêtre LogEvents ouverte"));
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur ouverture LogEvents: {ex.Message}"));
            }
        }
    }
}
