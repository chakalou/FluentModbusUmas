using FluentModbusUmas;
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
        List<APIDictionnaryVariable> _listeVariables = new List<APIDictionnaryVariable>();

        public Form1()
        {
            InitializeComponent();
            lbLog.DataSource = _list;
            _client = new ModbusUMASTcpClient();
            _client.UmasDataSent += Client_OnSend;
            _client.UmasDataReceived += Client_OnReceive;

            ModbusUmasFunctionCode[] umasvalues = (ModbusUmasFunctionCode[])Enum.GetValues(typeof(ModbusUmasFunctionCode));

            foreach (ModbusUmasFunctionCode item in umasvalues)
            {
                cbUMASFonction.Items.Add(item);
            }

        }

        private void Client_OnSend(object? sender, ModbusUmasDataSentEventArgs e)
        {
            if (e.Data != null)
            {
                byte test = (byte)e.FunctionCode;
                    _list.Add("TX:" +test.ToString("X2")+" "+ BitConverter.ToString(e.Data));
      
            }


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
            bConnect.BackColor = Color.Red;
            bConnect.ForeColor = Color.White;
            _list.Add("Disconnected");
            _client.Disconnect();
        }

        private void Connect()
        {
            _isConnect = true;
            bConnect.Text = "Disconnect";
            bConnect.BackColor = Color.Green;
            bConnect.ForeColor = Color.Black;
            _list.Add("Connected");
            _client.Connect(IPAddress.Parse(tbIP.Text));
        }



        private void bSendInfo_Click_1(object sender, EventArgs e)
        {

            try
            {
                if (_client.IsConnected)
                {

                    switch (cbUMASFonction.SelectedItem)
                    {
                        case ModbusUmasFunctionCode.UMAS_READ_VARIABLES:
                            _listeVariables = _client.SendUmas_GetDictionnaryVariables(0, TypeAPI.M580);
                            if(_listeVariables!=null && _listeVariables.Count > 0)
                                _client.SetVariablesValueFromREAD_SYSTEMBTISWORD_REQUEST(0, _listeVariables);
                            break;
                       
                        case ModbusUmasFunctionCode.UMAS_READ_ID:
                            _client.SendUmasREAD_PLC_ID(0, 0);
                            _list.Add(_client.PLCName + " FW:" + _client.PLCFWVersion +" State:"+ _client.PLCState);
                            break;
                        case ModbusUmasFunctionCode.UMAS_ENABLEDISABLE_DATADICTIONNARY:
                            _listeVariables = _client.SendUmas_GetDictionnaryVariables(0, TypeAPI.M580);
                            foreach (APIDictionnaryVariable var in _listeVariables)
                            {
                                _list.Add(var.Name + " BL" + var.BlockMemory.ToString("X") + " BO=" + var.Baseoffset.ToString("X") + " RO="+var.RelativeOffset.ToString("X"));

                            }
                            break;
                        case ModbusUmasFunctionCode.UMAS_READ_MEMORY_BLOCK:
                            _client.UmasReadOutputsWithMemoryBlocks(TypeAPI.M580, 0, 0, 64);
                            break;
                        default:
                            MessageBox.Show("Envoie fonction non implémenté", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;


                    }
                    //_client.InitUmasRequest(0);
                }
                else
                    _list.Add("UMAS Client is not connected");
            }
            catch (Exception ex)
            {

                _list.Add("Erreur : " + ex.StackTrace);
            }
        }
    }
}