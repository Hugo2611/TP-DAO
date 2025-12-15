using System;
using System.Collections.ObjectModel;
using System.Windows;
using LogEventsLib;

namespace WPFApp
{
    public partial class FormLogEvents : Window
    {
        private ObservableCollection<string> _events;

        public FormLogEvents()
        {
            InitializeComponent();
            _events = new ObservableCollection<string>();
            listBoxEvents.ItemsSource = _events;
            
            LogEvents.Instance.AjouterConsommateur(TraiterEvent);
            
            this.Closed += FormLogEvents_Closed;
        }

        private void TraiterEvent(Event evt)
        {
            Dispatcher.Invoke(() =>
            {
                _events.Add(evt.ToString());
                
                txtNbInfos.Text = LogEvents.Instance.NbInformations.ToString();
                txtNbAlertes.Text = LogEvents.Instance.NbAlertes.ToString();
                txtNbAlarmes.Text = LogEvents.Instance.NbAlarmes.ToString();
                txtNbPerdus.Text = LogEvents.Instance.NbPerdus.ToString();
                
                if (_events.Count > 100)
                {
                    _events.RemoveAt(0);
                }
            });
        }

        private void FormLogEvents_Closed(object sender, EventArgs e)
        {
            LogEvents.Instance.RetirerConsommateur(TraiterEvent);
        }
    }
}
