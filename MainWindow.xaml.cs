using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace ServerWPF
{
    public partial class MainWindow : Window
    {
        private TcpListener tcpListener;
        private Thread tcpListenerThread;
        private bool isListening = false;
        private List<ClientHandler> connectedClients = new List<ClientHandler>();
        private readonly object clientsLock = new object();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartServer();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopServer();
        }

        private void StartServer()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListenerThread = new Thread(new ThreadStart(ListenForClients));
                tcpListenerThread.Start();
                isListening = true;

                UpdateUI(() =>
                {
                    StartButton.IsEnabled = false;
                    StopButton.IsEnabled = true;
                    StatusLabel.Content = "Estado: Ejecutándose";
                });

                LogMessage("Servidor iniciado en puerto 8888");
            }
            catch (Exception ex)
            {
                LogMessage($"Error al iniciar servidor: {ex.Message}");
            }
        }

        private void StopServer()
        {
            try
            {
                isListening = false;
                tcpListener?.Stop();
                tcpListenerThread?.Abort();

                lock (clientsLock)
                {
                    foreach (var client in connectedClients)
                    {
                        client.Close();
                    }
                    connectedClients.Clear();
                }

                UpdateUI(() =>
                {
                    StartButton.IsEnabled = true;
                    StopButton.IsEnabled = false;
                    StatusLabel.Content = "Estado: Detenido";
                    UpdateConnectionCount();
                });

                LogMessage("Servidor detenido");
            }
            catch (Exception ex)
            {
                LogMessage($"Error al detener servidor: {ex.Message}");
            }
        }

        private void ListenForClients()
        {
            tcpListener.Start();
            LogMessage("Servidor escuchando conexiones...");

            while (isListening)
            {
                try
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    ClientHandler clientHandler = new ClientHandler(client, this);

                    lock (clientsLock)
                    {
                        connectedClients.Add(clientHandler);
                    }

                    Thread clientThread = new Thread(new ThreadStart(clientHandler.HandleClient));
                    clientThread.Start();

                    LogMessage($"Cliente conectado desde: {client.Client.RemoteEndPoint}");
                    UpdateConnectionCount();
                }
                catch (Exception ex)
                {
                    if (isListening)
                    {
                        LogMessage($"Error aceptando cliente: {ex.Message}");
                    }
                }
            }
        }

        public void RemoveClient(ClientHandler client)
        {
            lock (clientsLock)
            {
                connectedClients.Remove(client);
            }
            UpdateConnectionCount();
            LogMessage("Cliente desconectado");
        }

        private void UpdateConnectionCount()
        {
            lock (clientsLock)
            {
                UpdateUI(() =>
                {
                    ConnectionInfo.Content = $"Puerto: 8888 | Clientes conectados: {connectedClients.Count}";
                });
            }
        }

        public void LogMessage(string message)
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}";

            UpdateUI(() =>
            {
                LogTextBox.AppendText(logEntry + Environment.NewLine);
                LogTextBox.ScrollToEnd();
            });
        }

        private void UpdateUI(Action action)
        {
            if (Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Dispatcher.Invoke(action);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            StopServer();
            base.OnClosed(e);
        }
    }
}