using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;
using FiguresLib;
using LogEventsLib;

namespace ORMLib
{
    public class Album
    {
        public int AlbumId { get; set; }
        public string Nom { get; set; }
        public DateTime DateCreation { get; set; }
        public List<clsDessin> Dessins { get; set; }

        public Album()
        {
            Dessins = new List<clsDessin>();
        }
    }

    public class GestionnaireDAO
    {
        private string _connectionString;

        public GestionnaireDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqlConnection OuvrirConnexion()
        {
            try
            {
                SqlConnection conn = new SqlConnection(_connectionString);
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur connexion BD: {ex.Message}"));
                throw;
            }
        }

        public int CreerAlbum(string nom)
        {
            SqlConnection conn = null;
            try
            {
                conn = OuvrirConnexion();
                string sql = "INSERT INTO Albums (Nom, DateCreation) OUTPUT INSERTED.AlbumId VALUES (@Nom, @DateCreation)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nom", nom);
                    cmd.Parameters.AddWithValue("@DateCreation", DateTime.Now);
                    int albumId = (int)cmd.ExecuteScalar();
                    LogEvents.Instance.Push(new Event(NiveauEvent.Information, $"Album créé: {nom} (ID={albumId})"));
                    return albumId;
                }
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur création album: {ex.Message}"));
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public List<Album> LireAlbums()
        {
            SqlConnection conn = null;
            try
            {
                conn = OuvrirConnexion();
                List<Album> albums = new List<Album>();
                string sql = "SELECT AlbumId, Nom, DateCreation FROM Albums";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Album album = new Album
                            {
                                AlbumId = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                DateCreation = reader.GetDateTime(2)
                            };
                            albums.Add(album);
                        }
                    }
                }
                return albums;
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur lecture albums: {ex.Message}"));
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public int CreerDessin(int albumId, clsDessin dessin)
        {
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = OuvrirConnexion();
                transaction = conn.BeginTransaction();

                string sqlDessin = "INSERT INTO Dessins (AlbumId, Nom, DateCreation, Version) OUTPUT INSERTED.DessinId VALUES (@AlbumId, @Nom, @DateCreation, @Version)";
                int dessinId;
                using (SqlCommand cmd = new SqlCommand(sqlDessin, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@AlbumId", albumId);
                    cmd.Parameters.AddWithValue("@Nom", dessin.Nom);
                    cmd.Parameters.AddWithValue("@DateCreation", dessin.DateCreation);
                    cmd.Parameters.AddWithValue("@Version", dessin.Version);
                    dessinId = (int)cmd.ExecuteScalar();
                }

                dessin.Parcourir(figure =>
                {
                    string sqlFigure = "INSERT INTO Figures (DessinId, TypeFigure, Nom, X, Y, CouleurR, CouleurG, CouleurB, X2, Y2, Rayon) VALUES (@DessinId, @TypeFigure, @Nom, @X, @Y, @CouleurR, @CouleurG, @CouleurB, @X2, @Y2, @Rayon)";
                    using (SqlCommand cmd = new SqlCommand(sqlFigure, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@DessinId", dessinId);
                        cmd.Parameters.AddWithValue("@TypeFigure", figure.GetType().Name);
                        cmd.Parameters.AddWithValue("@Nom", figure.Nom);
                        cmd.Parameters.AddWithValue("@X", figure.X);
                        cmd.Parameters.AddWithValue("@Y", figure.Y);
                        cmd.Parameters.AddWithValue("@CouleurR", figure.Couleur.R);
                        cmd.Parameters.AddWithValue("@CouleurG", figure.Couleur.G);
                        cmd.Parameters.AddWithValue("@CouleurB", figure.Couleur.B);

                        if (figure is clsLigne ligne)
                        {
                            cmd.Parameters.AddWithValue("@X2", ligne.X2);
                            cmd.Parameters.AddWithValue("@Y2", ligne.Y2);
                            cmd.Parameters.AddWithValue("@Rayon", DBNull.Value);
                        }
                        else if (figure is clsCercle cercle)
                        {
                            cmd.Parameters.AddWithValue("@X2", DBNull.Value);
                            cmd.Parameters.AddWithValue("@Y2", DBNull.Value);
                            cmd.Parameters.AddWithValue("@Rayon", cercle.Rayon);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@X2", DBNull.Value);
                            cmd.Parameters.AddWithValue("@Y2", DBNull.Value);
                            cmd.Parameters.AddWithValue("@Rayon", DBNull.Value);
                        }

                        cmd.ExecuteNonQuery();
                    }
                });

                transaction.Commit();
                LogEvents.Instance.Push(new Event(NiveauEvent.Information, $"Dessin créé: {dessin.Nom} (ID={dessinId})"));
                return dessinId;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur création dessin: {ex.Message}"));
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public List<clsDessin> LireDessins(int albumId)
        {
            SqlConnection conn = null;
            try
            {
                conn = OuvrirConnexion();
                List<clsDessin> dessins = new List<clsDessin>();
                
                string sql = "SELECT DessinId, Nom, DateCreation, Version FROM Dessins WHERE AlbumId = @AlbumId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@AlbumId", albumId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int dessinId = reader.GetInt32(0);
                            string nom = reader.GetString(1);
                            float version = (float)reader.GetDouble(3);
                            
                            clsDessin dessin = new clsDessin(nom, version);
                            dessins.Add(dessin);
                        }
                    }
                }

                foreach (var dessin in dessins)
                {
                    string sqlFigures = @"SELECT FigureId, TypeFigure, Nom, X, Y, CouleurR, CouleurG, CouleurB, X2, Y2, Rayon 
                                         FROM Figures f 
                                         INNER JOIN Dessins d ON f.DessinId = d.DessinId 
                                         WHERE d.Nom = @NomDessin AND d.AlbumId = @AlbumId";
                    using (SqlCommand cmd = new SqlCommand(sqlFigures, conn))
                    {
                        cmd.Parameters.AddWithValue("@NomDessin", dessin.Nom);
                        cmd.Parameters.AddWithValue("@AlbumId", albumId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string typeFigure = reader.GetString(1);
                                string nomFigure = reader.GetString(2);
                                int x = reader.GetInt32(3);
                                int y = reader.GetInt32(4);
                                byte r = reader.GetByte(5);
                                byte g = reader.GetByte(6);
                                byte b = reader.GetByte(7);
                                Color couleur = Color.FromArgb(r, g, b);

                                if (typeFigure == "clsLigne")
                                {
                                    int x2 = reader.GetInt32(8);
                                    int y2 = reader.GetInt32(9);
                                    dessin.AjouterFigure(new clsLigne(x, y, x2, y2, couleur, nomFigure));
                                }
                                else if (typeFigure == "clsCercle")
                                {
                                    int rayon = reader.GetInt32(10);
                                    dessin.AjouterFigure(new clsCercle(x, y, rayon, couleur, nomFigure));
                                }
                            }
                        }
                    }
                }

                return dessins;
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur lecture dessins: {ex.Message}"));
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public void SupprimerAlbum(int albumId)
        {
            SqlConnection conn = null;
            try
            {
                conn = OuvrirConnexion();
                string sql = "DELETE FROM Albums WHERE AlbumId = @AlbumId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@AlbumId", albumId);
                    cmd.ExecuteNonQuery();
                    LogEvents.Instance.Push(new Event(NiveauEvent.Information, $"Album supprimé: ID={albumId}"));
                }
            }
            catch (Exception ex)
            {
                LogEvents.Instance.Push(new Event(NiveauEvent.Alarme, $"Erreur suppression album: {ex.Message}"));
                throw;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
    }
}
