using FluentModbusUmas;
using FluentModbusUmas.Assets;
using FluentModbusUmas.Umas;
using System.Collections.Generic;
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
        private BindingSource _logBindingSource;

        public Form1()
        {
            InitializeComponent();
            _logBindingSource = new BindingSource(components);
            lbLog.DataSource = _logBindingSource;
            _logBindingSource.DataSource = typeof(String);
            ((System.ComponentModel.ISupportInitialize)_logBindingSource).EndInit();
            _client = new ModbusUMASTcpClient(0);
            _client.UmasDataSent += Client_OnSend;
            _client.UmasDataReceived += Client_OnReceive;
            _client.APIInfoChanged += Client_APIInfoChanged;
            _client.APIListVariableUpdated += Client_APIListVariableUpdated;
            _client.APILog += Client_APILog;

            ModbusUmasFunctionCode[] umasvalues = (ModbusUmasFunctionCode[])Enum.GetValues(typeof(ModbusUmasFunctionCode));

            foreach (ModbusUmasFunctionCode item in umasvalues)
            {
                cbUMASFonction.Items.Add(item);
            }
            lbCPU.Text = "";
            lbCRC.Text = "";
            lbFW.Text = "";
            lbCRCSHIFTED.Text = "";
            aPIDictionnaryVariableBindingSource.DataSource = _listeVariables;

        }

        private void printLog(String message)
        {
            if (lbLog.InvokeRequired)
            {
               
                lbLog.Invoke(new MethodInvoker(delegate
                {
                    MajLbLog(message);
                }));
               
            }
            else
            {
                MajLbLog(message);
            }
        }

        private void MajLbLog(String message)
        {
            if (_list.Count > 30)
            {
                _list.RemoveAt(0);
            }
            _list.Add(message);
            _logBindingSource.DataSource = _list;
            _logBindingSource.ResetBindings(false);
            if (lbLog.Items.Count > 0)
                lbLog.TopIndex = lbLog.Items.Count - 1; // Scroll to last item
        }
        private void Client_APILog(object? sender, ModbusUmasAPILogEventArgs e)
        {
            printLog(e.Log);
        }
        private void Client_APIListVariableUpdated(object? sender, ModbusAPIListVariableUpdatedEventArgs e)
        {
            if (_listeVariables != null && _listeVariables.Count == 0 && e.ListVariable != null)
            {
                _listeVariables = e.ListVariable;
            }
            else
            {
                if (e.ListVariableAddedOrChanged != null && _listeVariables != null)
                {
                    foreach (APIDictionnaryVariable item in e.ListVariableAddedOrChanged)
                    {
                        if (_listeVariables.Find(x => x.Name == item.Name) == null)
                            _listeVariables.Add(item);
                    }
                }

                if (e.ListVariableRemoved!=null && _listeVariables!=null)
                { 
                    foreach (APIDictionnaryVariable item in e.ListVariableRemoved)
                    {
                        APIDictionnaryVariable? var = _listeVariables.FirstOrDefault(x => x.Name == item.Name);
                        if (var != null)
                            _listeVariables.Remove(var);
                    }
                }
                
            }

            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke(new MethodInvoker(delegate
                {
                    aPIDictionnaryVariableBindingSource.DataSource = _listeVariables;
                    aPIDictionnaryVariableBindingSource.ResetBindings(false);
                }));
            }
            else
            {
                aPIDictionnaryVariableBindingSource.DataSource = _listeVariables;
                aPIDictionnaryVariableBindingSource.ResetBindings(false);
            }
        }
        private void Client_APIInfoChanged(object? sender, EventArgs e)
        {

            if (lbCPU.InvokeRequired)
            {
                lbCPU.Invoke(new MethodInvoker(delegate
                {
                    lbCPU.Text = _client.PLCName;
                    lbFW.Text = _client.PLCFWVersion;

                    if (_client.CRCFromPLC != null)
                        lbCRC.Text = BitConverter.ToString(_client.CRCFromPLC);
                    if (_client.CRCShiftedFromPLC != null)
                        lbCRCSHIFTED.Text = BitConverter.ToString(_client.CRCShiftedFromPLC);
                }));
            }
            else
            {
                lbCPU.Text = _client.PLCName;
                lbFW.Text = _client.PLCFWVersion;

                if (_client.CRCFromPLC != null)
                    lbCRC.Text = BitConverter.ToString(_client.CRCFromPLC);
                if (_client.CRCShiftedFromPLC != null)
                    lbCRCSHIFTED.Text = BitConverter.ToString(_client.CRCShiftedFromPLC);
            }
           

        }

        private void Client_OnSend(object? sender, ModbusUmasDataSentEventArgs e)
        {
            if (e.Data != null)
            {
                byte test = (byte)e.FunctionCode;
                printLog("TX:" + test.ToString("X2") + " " + BitConverter.ToString(e.Data));

            }


        }
     
        private void Client_OnReceive(object? sender, ModbusUmasDataReceivedEventArgs e)
        {
            if (e.Data != null)
                printLog("RX:" + BitConverter.ToString(e.Data));

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
            printLog("Disconnected");
            _client.Disconnect();
        }

        private void Connect()
        {
            try
            {
                _client.Connect(IPAddress.Parse(tbIP.Text));
                if (_client.IsConnected)
                {
                    bConnect.BackColor = Color.Green;
                    bConnect.ForeColor = Color.Black;
                    printLog("Connected");
                    bConnect.Text = "Disconnect";
                    _isConnect = true;

                }
            }
            catch (Exception ex)
            {

                printLog("Error connecting to " + tbIP.Text);
                printLog("Error : " + ex.Message);
            }
            
        }




        private void bSendInfo_Click_1(object sender, EventArgs e)
        {

           /* try
            {
                if (_client.IsConnected)
                {

                    switch (cbUMASFonction.SelectedItem)
                    {
                        case ModbusUmasFunctionCode.UMAS_READ_VARIABLES:
                           (bool ret, _listeVariables) = _client.MajListVariablesFromDataDictionnary(true);
                            if (_listeVariables != null && _listeVariables.Count > 0)
                                _client.SetVariablesValueFromREAD_SYSTEMBTISWORD_REQUEST(0, _listeVariables);
                            break;

                        case ModbusUmasFunctionCode.UMAS_READ_ID:
                            _client.get(0, 0);
                            _list.Add(_client.PLCName + " FW:" + _client.PLCFWVersion + " State:" + _client.PLCState);
                            break;
                        case ModbusUmasFunctionCode.UMAS_ENABLEDISABLE_DATADICTIONNARY:

                            break;
                        case ModbusUmasFunctionCode.UMAS_READ_MEMORY_BLOCK:


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
            finally
            {
                aPIDictionnaryVariableBindingSource.DataSource = _listeVariables;
                aPIDictionnaryVariableBindingSource.ResetBindings(false);
            }*/
        }
    }
}