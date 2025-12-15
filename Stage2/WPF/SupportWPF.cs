using System;
using System.Windows;
using System.Windows.Media;

namespace FiguresLib
{
    public class SupportWPF : ISupportDessin
    {
        private DrawingContext _drawingContext;
        private Pen _pen;

        public SupportWPF(DrawingContext drawingContext)
        {
            _drawingContext = drawingContext;
            _pen = new Pen(Brushes.Black, 2);
        }

        public int Couleur_Selectionne(byte ARouge, byte AVert, byte ABleu)
        {
            Color couleur = Color.FromRgb(ARouge, AVert, ABleu);
            _pen = new Pen(new SolidColorBrush(couleur), 2);
            return 0;
        }

        public int Ligne_Trace(int AX_Debut, int AY_Debut, int AX_Fin, int AY_Fin)
        {
            try
            {
                _drawingContext.DrawLine(_pen, new Point(AX_Debut, AY_Debut), new Point(AX_Fin, AY_Fin));
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int Cercle_Trace(int AX_Centre, int AY_Centre, int ARayon)
        {
            try
            {
                _drawingContext.DrawEllipse(null, _pen, new Point(AX_Centre, AY_Centre), ARayon, ARayon);
                return 0;
            }
            catch
            {
                return -1;
            }
        }
    }
}
