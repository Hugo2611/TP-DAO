using System;
using System.Drawing;
using System.Text.Json.Serialization;

namespace FiguresLib
{
    public abstract class clsFigure
    {
        private static int _compteur = 0;
        private string _nom;
        protected Point _position;
        protected Color _couleur;
        
        [JsonIgnore]
        protected static ISupportDessin _support;

        public clsFigure(int x, int y, Color couleur, string nom = "")
        {
            _position = new Point(x, y);
            _couleur = couleur;
            
            if (string.IsNullOrWhiteSpace(nom))
            {
                _compteur++;
                _nom = $"Figure{_compteur}";
            }
            else
            {
                _nom = nom;
            }
        }

        [JsonConstructor]
        public clsFigure()
        {
            _compteur++;
            _nom = $"Figure{_compteur}";
        }

        public string Nom
        {
            get { return _nom; }
            private set { _nom = value; }
        }

        public int X
        {
            get { return _position.X; }
            set { _position.X = value; }
        }

        public int Y
        {
            get { return _position.Y; }
            set { _position.Y = value; }
        }

        public Color Couleur
        {
            get { return _couleur; }
            set { _couleur = value; }
        }

        public static ISupportDessin Support
        {
            set { _support = value; }
        }

        public abstract void Dessine();
        public abstract void Zoom(float facteur);
    }

    public class clsLigne : clsFigure
    {
        private Point _pointFin;

        public clsLigne(int x1, int y1, int x2, int y2, Color couleur, string nom = "") 
            : base(x1, y1, couleur, nom)
        {
            _pointFin = new Point(x2, y2);
        }

        [JsonConstructor]
        public clsLigne() : base()
        {
            _pointFin = new Point(0, 0);
        }

        public int X2
        {
            get { return _pointFin.X; }
            set { _pointFin.X = value; }
        }

        public int Y2
        {
            get { return _pointFin.Y; }
            set { _pointFin.Y = value; }
        }

        public override void Dessine()
        {
            if (_support != null)
            {
                _support.Couleur_Selectionne(_couleur.R, _couleur.G, _couleur.B);
                _support.Ligne_Trace(_position.X, _position.Y, _pointFin.X, _pointFin.Y);
            }
        }

        public override void Zoom(float facteur)
        {
            _pointFin.X = (int)(_pointFin.X * facteur);
            _pointFin.Y = (int)(_pointFin.Y * facteur);
        }
    }

    public class clsCercle : clsFigure
    {
        private int _rayon;

        public clsCercle(int x, int y, int rayon, Color couleur, string nom = "") 
            : base(x, y, couleur, nom)
        {
            _rayon = rayon;
        }

        [JsonConstructor]
        public clsCercle() : base()
        {
            _rayon = 0;
        }

        public int Rayon
        {
            get { return _rayon; }
            set { _rayon = value; }
        }

        public override void Dessine()
        {
            if (_support != null)
            {
                _support.Couleur_Selectionne(_couleur.R, _couleur.G, _couleur.B);
                _support.Cercle_Trace(_position.X, _position.Y, _rayon);
            }
        }

        public override void Zoom(float facteur)
        {
            _rayon = (int)(_rayon * facteur);
        }
    }

    public class clsDessin
    {
        private string _nom;
        private List<clsFigure> _figures;
        private DateTime _dateCreation;
        private float _version;

        public clsDessin(string nom, float version = 1.0f)
        {
            _nom = nom;
            _version = version;
            _dateCreation = DateTime.Now;
            _figures = new List<clsFigure>();
        }

        [JsonConstructor]
        public clsDessin()
        {
            _nom = "Dessin";
            _version = 1.0f;
            _dateCreation = DateTime.Now;
            _figures = new List<clsFigure>();
        }

        public string Nom
        {
            get { return _nom; }
            set { _nom = value; }
        }

        public DateTime DateCreation
        {
            get { return _dateCreation; }
        }

        public float Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public List<clsFigure> Figures
        {
            get { return _figures; }
            set { _figures = value; }
        }

        public void AjouterFigure(clsFigure figure)
        {
            _figures.Add(figure);
        }

        public void DessinerTout()
        {
            foreach (var figure in _figures)
            {
                figure.Dessine();
            }
        }

        public void SupprimerTout()
        {
            _figures.Clear();
        }

        public clsFigure RechercherFigure(string nom)
        {
            return _figures.Find(f => f.Nom == nom);
        }

        public void Parcourir(Action<clsFigure> action)
        {
            foreach (var figure in _figures)
            {
                action(figure);
            }
        }
    }
}
