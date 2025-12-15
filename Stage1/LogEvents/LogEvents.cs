using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LogEventsLib
{
    public enum NiveauEvent
    {
        Information,
        Alerte,
        Alarme
    }

    public class Event
    {
        public NiveauEvent Niveau { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string FichierSource { get; set; }
        public int NumeroLigne { get; set; }

        public Event(NiveauEvent niveau, string description, 
            [CallerFilePath] string fichier = "", 
            [CallerLineNumber] int ligne = 0)
        {
            Niveau = niveau;
            Date = DateTime.Now;
            Description = description;
            FichierSource = Path.GetFileName(fichier);
            NumeroLigne = ligne;
        }

        public override string ToString()
        {
            return $"[{Date:yyyy-MM-dd HH:mm:ss}] [{Niveau}] {Description} ({FichierSource}:{NumeroLigne})";
        }
    }

    public sealed class LogEvents
    {
        private static readonly Lazy<LogEvents> _instance = new Lazy<LogEvents>(() => new LogEvents());
        private readonly ConcurrentQueue<Event> _events;
        private readonly int _maxEvents = 1000;
        private readonly object _lockDelegate = new object();
        private Action<Event> _consommateurs;
        private Thread _flushThread;
        private bool _running;

        private int _nbInformations = 0;
        private int _nbAlertes = 0;
        private int _nbAlarmes = 0;
        private int _nbPerdus = 0;

        private LogEvents()
        {
            _events = new ConcurrentQueue<Event>();
            _running = true;
            _flushThread = new Thread(FlushLoop);
            _flushThread.IsBackground = true;
            _flushThread.Start();
        }

        public static LogEvents Instance
        {
            get { return _instance.Value; }
        }

        public int NbInformations => _nbInformations;
        public int NbAlertes => _nbAlertes;
        public int NbAlarmes => _nbAlarmes;
        public int NbPerdus => _nbPerdus;

        public Action<Event> Consommateurs
        {
            set
            {
                lock (_lockDelegate)
                {
                    _consommateurs = value;
                }
            }
        }

        public void AjouterConsommateur(Action<Event> consommateur)
        {
            lock (_lockDelegate)
            {
                _consommateurs += consommateur;
            }
        }

        public void RetirerConsommateur(Action<Event> consommateur)
        {
            lock (_lockDelegate)
            {
                _consommateurs -= consommateur;
            }
        }

        public void Push(Event evt)
        {
            if (_events.Count >= _maxEvents)
            {
                Interlocked.Increment(ref _nbPerdus);
                return;
            }

            _events.Enqueue(evt);

            switch (evt.Niveau)
            {
                case NiveauEvent.Information:
                    Interlocked.Increment(ref _nbInformations);
                    break;
                case NiveauEvent.Alerte:
                    Interlocked.Increment(ref _nbAlertes);
                    break;
                case NiveauEvent.Alarme:
                    Interlocked.Increment(ref _nbAlarmes);
                    break;
            }
        }

        private void FlushLoop()
        {
            while (_running)
            {
                Thread.Sleep(3000);
                Flush();
            }
        }

        private void Flush()
        {
            Action<Event> handlers;
            lock (_lockDelegate)
            {
                handlers = _consommateurs;
            }

            if (handlers == null)
                return;

            while (_events.TryDequeue(out Event evt))
            {
                try
                {
                    handlers(evt);
                }
                catch
                {
                }
            }
        }

        public void Stop()
        {
            _running = false;
            if (_flushThread != null && _flushThread.IsAlive)
            {
                _flushThread.Join();
            }
        }
    }

    public class ConsommateurFichier
    {
        private readonly string _cheminFichier;
        private readonly object _lockFile = new object();

        public ConsommateurFichier(string cheminFichier)
        {
            _cheminFichier = cheminFichier;
        }

        public void TraiterEvent(Event evt)
        {
            lock (_lockFile)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(_cheminFichier, true))
                    {
                        sw.WriteLine(evt.ToString());
                    }
                }
                catch
                {
                }
            }
        }
    }

    public class ConsommateurConsole
    {
        public void TraiterEvent(Event evt)
        {
            Console.WriteLine(evt.ToString());
        }
    }
}
