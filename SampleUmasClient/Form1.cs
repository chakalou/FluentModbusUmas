using FluentModbus;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace SampleUmasClient
{
    public partial class Form1 : Form
    {

        bool _isConnect = false;
        ModbusUMASTcpClient _client;
        BindingList<String> _list = new BindingList<String>();

        public Form1()
        {
            InitializeComponent();
            lbLog.DataSource = _list;
            _client = new ModbusUMASTcpClient();
            _client.UmasDataSent += Client_OnDisconnected;
            _client.UmasDataReceived += Client_OnReceive;

        }

        private void Client_OnDisconnected(object? sender, ModbusUmasDataSentEventArgs e)
        {
            if (e.Data != null)
                _list.Add("TX:" + BitConverter.ToString(e.Data));

        }

        private void Client_OnReceive(object? sender, ModbusUmasDataReceivedEventArgs e)
        {
            if (e.Data != null)
                _list.Add("RX:" + BitConverter.ToString(e.Data));

        }

        private void bConnect_Click(object sender, EventArgs e)
        {
            if (_isConnect)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }
        }

        private void Disconnect()
        {
            _isConnect = false;
            bConnect.Text = "Connect";
            bConnect.BackColor = Color.Green;
            bConnect.ForeColor = Color.White;
            _list.Add("Disconnected");
            _client.Disconnect();
        }

        private void Connect()
        {
            _isConnect = true;
            bConnect.Text = "Disconnect";
            bConnect.BackColor = Color.Red;
            bConnect.ForeColor = Color.White;
            _list.Add("Connected");
            _client.Connect(IPAddress.Parse(tbIP.Text));
        }

        private void bSendInfo_Click(object sender, EventArgs e)
        {
            if (_client.IsConnected)
            {
                try
                {
                    //_client.InitUmasRequest(0);
                    _client.UmasReadOutputsFromMemoryBlocks(TypeAPI.PLCSIM,0, 0, 96);
                }
                catch (Exception ex)
                {

                    _list.Add("Erreur : " + ex.Message);
                }
            }

        }
    }
}