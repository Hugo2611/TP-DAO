using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FiguresLib;

namespace ClientDAO
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
        public JsonElement Donnees { get; set; }
    }

    public class ClientTCP
    {
        private string _host;
        private int _port;

        public ClientTCP(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public async Task<ReponseServeur> EnvoyerCommandeAsync(CommandeServeur commande)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(_host, _port);
                    NetworkStream stream = client.GetStream();

                    string json = JsonSerializer.Serialize(commande);
                    byte[] data = Encoding.UTF8.GetBytes(json);
                    await stream.WriteAsync(data, 0, data.Length);

                    byte[] buffer = new byte[8192];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string reponseJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    ReponseServeur reponse = JsonSerializer.Deserialize<ReponseServeur>(reponseJson);
                    return reponse;
                }
            }
            catch (Exception ex)
            {
                return new ReponseServeur { Succes = false, Message = $"Erreur communication: {ex.Message}" };
            }
        }

        public ReponseServeur EnvoyerCommande(CommandeServeur commande)
        {
            return EnvoyerCommandeAsync(commande).GetAwaiter().GetResult();
        }
    }
}
