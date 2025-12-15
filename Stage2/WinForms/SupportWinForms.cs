using System;
using System.Drawing;

namespace FiguresLib
{
    public class SupportWinForms : ISupportDessin
    {
        private Graphics _graphics;
        private Pen _pen;

        public SupportWinForms(Graphics graphics)
        {
            _graphics = graphics;
            _pen = new Pen(Color.Black);
        }

        public int Couleur_Selectionne(byte ARouge, byte AVert, byte ABleu)
        {
            if (_pen != null)
            {
                _pen.Dispose();
            }
            _pen = new Pen(Color.FromArgb(ARouge, AVert, ABleu));
            return 0;
        }

        public int Ligne_Trace(int AX_Debut, int AY_Debut, int AX_Fin, int AY_Fin)
        {
            try
            {
                _graphics.DrawLine(_pen, AX_Debut, AY_Debut, AX_Fin, AY_Fin);
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
                _graphics.DrawEllipse(_pen, AX_Centre - ARayon, AY_Centre - ARayon, ARayon * 2, ARayon * 2);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public void Dispose()
        {
            if (_pen != null)
            {
                _pen.Dispose();
            }
        }
    }
}
