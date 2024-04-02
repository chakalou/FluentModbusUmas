using FluentModbusUmas;
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
        ModbusUMASTcpClient? _client;
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
            lbCPU.Text = "";
            lbCRC.Text = "";
            lbFW.Text = "";
            lbCRCSHIFTED.Text = "";
            aPIDictionnaryVariableBindingSource.DataSource = _listeVariables;

        }

        private void Client_OnSend(object? sender, ModbusUmasDataSentEventArgs e)
        {
            if (e.Data != null)
            {
                byte test = (byte)e.FunctionCode;
                _list.Add("TX:" + test.ToString("X2") + " " + BitConverter.ToString(e.Data));

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
            _client.Connect(IPAddress.Parse(tbIP.Text));
            if (_client.IsConnected)
            {
                bConnect.BackColor = Color.Green;
                bConnect.ForeColor = Color.Black;
                _list.Add("Connected");
                bConnect.Text = "Disconnect";
                _isConnect = true;

                if (_client.SendUmasREAD_PLC_ID(0, 0))
                {
                    lbCPU.Text = _client.PLCName;
                    lbFW.Text = _client.PLCFWVersion;
                }
                if (_client.SendUmas_READ_PLC_INFO(0))
                {
                    if (_client.CRCFromPLC != null)
                        lbCRC.Text = BitConverter.ToString(_client.CRCFromPLC);
                    if (_client.CRCShiftedFromPLC != null)
                        lbCRCSHIFTED.Text = BitConverter.ToString(_client.CRCShiftedFromPLC);
                }
            }
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
                            if (_listeVariables != null && _listeVariables.Count > 0)
                                _client.SetVariablesValueFromREAD_SYSTEMBTISWORD_REQUEST(0, _listeVariables);
                            break;

                        case ModbusUmasFunctionCode.UMAS_READ_ID:
                            _client.SendUmasREAD_PLC_ID(0, 0);
                            _list.Add(_client.PLCName + " FW:" + _client.PLCFWVersion + " State:" + _client.PLCState);
                            break;
                        case ModbusUmasFunctionCode.UMAS_ENABLEDISABLE_DATADICTIONNARY:
                            _listeVariables = _client.SendUmas_GetDictionnaryVariables(0, TypeAPI.M580);
                            foreach (APIDictionnaryVariable var in _listeVariables)
                            {
                                _list.Add(var.Name + " BL" + var.BlockMemory.ToString("X") + " BO=" + var.Baseoffset.ToString("X") + " RO=" + var.RelativeOffset.ToString("X"));

                            }
                            break;
                        case ModbusUmasFunctionCode.UMAS_READ_MEMORY_BLOCK:

                            if (_listeVariables != null)
                            {
                                List<APIDictionnaryVariable> _sortedlist = _listeVariables;
                                if (_sortedlist != null)
                                {
                                    _sortedlist.OrderBy(x => x.BlockMemory).ThenBy(x => x.RelativeOffset).ToList();

                                    int memoryblock = -1;
                                    int minoffset = -1;
                                    int maxoffset = -1;
                                    int length = -1;
                                    foreach (var aPIDictionnaryVariable in _sortedlist)
                                    {
                                        //Si memoryblock = -1 alors c'est le premier
                                        if (memoryblock == -1)
                                        {
                                            memoryblock = aPIDictionnaryVariable.BlockMemory;
                                            minoffset = aPIDictionnaryVariable.RelativeOffset;
                                            maxoffset = aPIDictionnaryVariable.RelativeOffset;
                                            continue;
                                        }
                                        //Si le memoryblock est le même que le précédent on met à jour les offset min et max
                                        if (memoryblock == aPIDictionnaryVariable.BlockMemory)
                                        {
                                            if (aPIDictionnaryVariable.RelativeOffset < minoffset)
                                                minoffset = aPIDictionnaryVariable.RelativeOffset;
                                            if (aPIDictionnaryVariable.RelativeOffset > maxoffset)
                                                maxoffset = aPIDictionnaryVariable.RelativeOffset;
                                            length=aPIDictionnaryVariable.VariableLength;
                                            continue;
                                        }
                                        else
                                        {
                                            //sinon : on change de memoryblock => on envoie la requête pour récupérer les valeurs
                                          Span<byte>ret= _client.UmasReadVariablesFromMemoryBlocks(0, (byte)memoryblock, minoffset, maxoffset - minoffset + length);

                                            if (ret != null)
                                            {

                                                List<APIDictionnaryVariable> _sortedblock = _sortedlist.Where(_listeVariables => _listeVariables.BlockMemory == memoryblock).ToList();
                                                foreach (var aPIDictionnaryVariablebis in _sortedblock)
                                                {
                                                    byte[] tab = new byte[aPIDictionnaryVariablebis.VariableLength];
                                                     Array.Copy(ret.ToArray(), aPIDictionnaryVariablebis.RelativeOffset - minoffset,tab,0, aPIDictionnaryVariablebis.VariableLength);
                                                    switch (tab.Length)
                                                    {
                                                        case 1:
                                                            aPIDictionnaryVariablebis.Valeur = tab[0];
                                                            break;
                                                        case 2:
                                                            aPIDictionnaryVariablebis.Valeur = BitConverter.ToInt16(tab);
                                                            break;
                                                        case 4:
                                                            aPIDictionnaryVariablebis.Valeur = BitConverter.ToInt32(tab);
                                                            break;
                                                        default:
                                                            aPIDictionnaryVariablebis.Valeur = BitConverter.ToInt32(tab);
                                                            break;
                                                    }
                                                }
                                            }
                                            memoryblock = aPIDictionnaryVariable.BlockMemory;
                                            minoffset = aPIDictionnaryVariable.RelativeOffset;
                                            maxoffset = aPIDictionnaryVariable.RelativeOffset;
                                            length = aPIDictionnaryVariable.VariableLength;
                                        }

                                    }
                                    if(memoryblock!=-1)
                                    {
                                        Span<byte> ret = _client.UmasReadVariablesFromMemoryBlocks(0, (byte)memoryblock, minoffset, maxoffset - minoffset +length);
                                        if (ret != null)
                                        {

                                            List<APIDictionnaryVariable> _sortedblock = _sortedlist.Where(_listeVariables => _listeVariables.BlockMemory == memoryblock).ToList();
                                            foreach (var aPIDictionnaryVariablebis in _sortedblock)
                                            {
                                                byte[] tab = new byte[aPIDictionnaryVariablebis.VariableLength];
                                                Array.Copy(ret.ToArray(), aPIDictionnaryVariablebis.RelativeOffset - minoffset, tab, 0, aPIDictionnaryVariablebis.VariableLength);
                                               switch(tab.Length)
                                                {
                                                    case 1:
                                                        aPIDictionnaryVariablebis.Valeur = tab[0];
                                                        break;
                                                    case 2:
                                                        aPIDictionnaryVariablebis.Valeur = BitConverter.ToInt16(tab);
                                                        break;
                                                    case 4:
                                                        aPIDictionnaryVariablebis.Valeur = BitConverter.ToInt32(tab);
                                                        break;
                                                    default:
                                                        aPIDictionnaryVariablebis.Valeur = BitConverter.ToInt32(tab);
                                                        break;
                                                }
                                                
                                            }
                                        }
                                    }
                                }

                                //APIDictionnaryVariable? item = _listeVariables.FirstOrDefault(x => x.Variabletype == DictionnaryVariableClassType.EBOOL);
                                //if (item != null)
                                //    _client.UmasReadVariablesFromMemoryBlocks(0, (byte)item.BlockMemory, item.RelativeOffset, 1);

                            }
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
                aPIDictionnaryVariableBindingSource.DataSource=_listeVariables;
                aPIDictionnaryVariableBindingSource.ResetBindings(false);
            }
        }
    }
}