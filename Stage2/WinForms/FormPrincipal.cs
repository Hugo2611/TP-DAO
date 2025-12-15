using System;
using System.Drawing;
using System.Windows.Forms;
using FiguresLib;
using LogEventsLib;

namespace WinFormsApp
{
    public partial class FormPrincipal : Form
    {
        private clsDessin _dessin;

        public FormPrincipal()
        {
            InitializeComponent();
            InitialiserDessin();
        }

        private void InitialiserDessin()
        {
            try
            {
                _dessin = new clsDessin("MonDessin", 1.0f);
                _dessin.AjouterFigure(new clsLigne(50, 50, 200, 150, Color.Red, "Ligne1"));
                _dessin.AjouterFigure(new clsCercle(300, 200, 80, Color.Blue, "Cercle1"));
                _dessin.AjouterFigure(new clsLigne(400, 100, 500, 300, Color.Green));
                _dessin.AjouterFigure(new clsCercle(600, 150, 50, Color.Orange));

                LogEvents.Instance.Push(new Event(NiveauEvent.Information, "Dessin initialisé avec succès"));
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur initialisation dessin: {ex.Message}"));
            }
        }

        private void panelDessin_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                using (SupportWinForms support = new SupportWinForms(e.Graphics))
                {
                    clsFigure.Support = support;
                    _dessin.DessinerTout();
                }
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur lors du dessin: {ex.Message}"));
            }
        }

        private void btnDessiner_Click(object sender, EventArgs e)
        {
            try
            {
                panelDessin.Invalidate();
                LogEvents.Instance.Push(new Event(NiveauEvent.Information, "Dessin rafraîchi"));
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur rafraîchissement: {ex.Message}"));
            }
        }
    }
}
