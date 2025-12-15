using System;

namespace FiguresLib
{
    public interface ISupportDessin
    {
        int Couleur_Selectionne(byte ARouge, byte AVert, byte ABleu);
        int Ligne_Trace(int AX_Debut, int AY_Debut, int AX_Fin, int AY_Fin);
        int Cercle_Trace(int AX_Centre, int AY_Centre, int ARayon);
    }

    public class SupportConsole_Texte : ISupportDessin
    {
        public int Couleur_Selectionne(byte ARouge, byte AVert, byte ABleu)
        {
            Console.WriteLine($"Console - Couleur sélectionnée: R={ARouge}, G={AVert}, B={ABleu}");
            return 0;
        }

        public int Ligne_Trace(int AX_Debut, int AY_Debut, int AX_Fin, int AY_Fin)
        {
            Console.WriteLine($"Console - Ligne tracée de ({AX_Debut},{AY_Debut}) à ({AX_Fin},{AY_Fin})");
            return 0;
        }

        public int Cercle_Trace(int AX_Centre, int AY_Centre, int ARayon)
        {
            Console.WriteLine($"Console - Cercle tracé centre ({AX_Centre},{AY_Centre}) rayon {ARayon}");
            return 0;
        }
    }

    public class SupportImprimante_Canon : ISupportDessin
    {
        public int Couleur_Selectionne(byte ARouge, byte AVert, byte ABleu)
        {
            Console.WriteLine($"Imprimante Canon - Couleur sélectionnée: R={ARouge}, G={AVert}, B={ABleu}");
            return 0;
        }

        public int Ligne_Trace(int AX_Debut, int AY_Debut, int AX_Fin, int AY_Fin)
        {
            Console.WriteLine($"Imprimante Canon - Ligne tracée de ({AX_Debut},{AY_Debut}) à ({AX_Fin},{AY_Fin})");
            return 0;
        }

        public int Cercle_Trace(int AX_Centre, int AY_Centre, int ARayon)
        {
            Console.WriteLine($"Imprimante Canon - Cercle tracé centre ({AX_Centre},{AY_Centre}) rayon {ARayon}");
            return 0;
        }
    }
}
