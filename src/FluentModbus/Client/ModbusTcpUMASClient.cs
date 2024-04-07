using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using TrapilModbusUmas.Assets;
using TrapilModbusUmas.Umas;



namespace TrapilModbusUmas
{    /// <summary>
     /// Arguments for the event raised when the list of API variables is updated (variables added,changed or removed)
     /// </summary>
    public class ModbusUmasAPILogEventArgs : EventArgs
    {
        /// <summary>
        /// Log a envoyer
        /// </summary>
       public  String Log { get; set; }
        
    }
    /// <summary>
    /// Arguments for the event raised when the list of API variables is updated (variables added,changed or removed)
    /// </summary>
    public class ModbusAPIListVariableUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Complete List of variables read from PLC
        /// </summary>
        public List<APIDictionnaryVariable>? ListVariable { get; set; }
        /// <summary>
        /// List of variables added or changed
        /// </summary>
        public List<APIDictionnaryVariable>? ListVariableAddedOrChanged { get; set; }
        /// <summary>
        /// List of variables removed
        /// </summary>
        public List<APIDictionnaryVariable>? ListVariableRemoved { get; set; }
    }
    /// <summary>
    /// Modbus UMAS Event arguments triggered when data is sent to the PLC
    /// </summary>
    public class ModbusUmasDataSentEventArgs : EventArgs
    {
        /// <summary>
        /// Modbus Function Code Sent to the PLC
        /// </summary>
        public ModbusUmasFunctionCode FunctionCode { get; set; }
        /// <summary>
        /// Data sent to the PLC in byte array
        /// </summary>
        public byte[]? Data { get; set; }
    }
    /// <summary>
    /// Modbus UMAS Event arguments triggered when data is received from the PLC
    /// </summary>
    public class ModbusUmasDataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Data byte array received from PLC        /// </summary>
        public byte[]? Data { get; set; }
    }

    /// <summary>
    /// A Modbus UMAS TCP client to communicate with a Schneider PLC.
    /// </summary>
    public class ModbusUMASTcpClient : ModbusTcpClient
    {
        bool _updateListVariableAuto = true;
        int _updateListVariableAutoInterval = 5;
        int _updateListVariableAutoCounter = 0;
        String _fwversion = "";
        String _plcName = "";
        TypeAPI _typeAPI;
        APIState _aPIState = APIState.Unknown;
        List<APIDictionnaryVariable> _listeVariables;
        int _unitIdentifier;
        /// <summary>
        /// Firmware version of the PLC
        /// </summary>
        public String PLCFWVersion { get { return _fwversion; } }
        /// <summary>
        /// Name of the PLC
        /// </summary>
        public String PLCName { get { return _plcName; } }
        /// <summary>
        /// PLC State
        /// </summary>
        public String PLCState { get { return _aPIState.ToString(); } }
        private byte[]? _cRCFromPLC;
        byte[]? _cRCShifted;
        /// <summary>
        /// Event triggered when data is sent to the PLC
        /// </summary>
        public event EventHandler<ModbusUmasDataSentEventArgs>? UmasDataSent;
        /// <summary>
        /// Event triggered when data is received from the PLC
        /// </summary>
        public event EventHandler<ModbusUmasDataReceivedEventArgs>? UmasDataReceived;
        /// <summary>
        /// Event triggered when CRC, CRC shifted, API Firmware version or API name is changed
        /// </summary>
        public event EventHandler<EventArgs>? APIInfoChanged;
        /// <summary>
        /// Event triggered when the list of API variables is updated (variables added,changed or removed)
        /// </summary>
        public event EventHandler<ModbusAPIListVariableUpdatedEventArgs>? APIListVariableUpdated;
        /// <summary>
        /// Event triggered when the list of API variables is updated (variables added,changed or removed)
        /// </summary>
        public event EventHandler<ModbusUmasAPILogEventArgs>? APILog;
        Timer _timerUmasSendCdG;
        bool _umasRequestStarted = false;
        byte _pairing_key = 0x00;

        /// <summary>
        /// Retourne le CRC de l'application API
        /// </summary>
        public byte[]? CRCFromPLC { get { if (_cRCFromPLC == null) return null; else return _cRCFromPLC; } }
        /// <summary>
        /// Retourne le CRC de l'application API décalé de 1 octet vers la gauche
        /// </summary>
        public byte[]? CRCShiftedFromPLC { get { if (_cRCShifted == null) return null; else return _cRCShifted; } }
        /// <summary>
        /// Creates a new Modbus TCP UMAS client for communication with a schneider PLC.
        /// </summary>
        public ModbusUMASTcpClient(int uintidentifier) : base()
        {
            _listeVariables = new List<APIDictionnaryVariable>();
            _timerUmasSendCdG = new Timer(_UmasTimerSendWdGRequest);
        }
        /// <summary>
        /// Get PLC infos from the PLC with UMAS 0x02 et 0x04 requests
        /// </summary>
        /// <returns></returns>
        private bool GetPLCInfos()
        {
            (bool ret, String plcname, String fwversion) = SendUmasREAD_PLC_ID();
            if (ret)
            {
                (bool test, byte[]? crc, byte[]? crcshidted) = SendUmas_READ_PLC_INFO();
                if (test && crc != null && crcshidted != null)
                {
                    if (_fwversion != fwversion || _plcName != plcname || _cRCFromPLC != crc || _cRCShifted != crcshidted)
                        APIInfoChanged?.Invoke(this, new EventArgs());

                    _fwversion = fwversion;
                    _plcName = plcname;
                    _cRCFromPLC = crc;
                    _cRCShifted = crcshidted;
                    return true;
                }

            }

            return false;
        }

        /// <summary>
        /// Get variables and values from the data dictionnary of the PLC
        /// </summary>
        /// <param name="pMajListeVariable"></param>
        /// <returns></returns>
        public (bool, List<APIDictionnaryVariable>) MajListVariablesFromDataDictionnary(bool pMajListeVariable)
        {
            List<APIDictionnaryVariable> listeVariables = new List<APIDictionnaryVariable>();
            bool ret = false;
            if (pMajListeVariable)
            {
                (listeVariables, ret) = SendUmas_GetDictionnaryVariables();

                if (ret)
                {
                    ret = FillListVariableValues(listeVariables);
                }
            }
            if (ret)
            {
                List<APIDictionnaryVariable> listeVariablesDeleted = new List<APIDictionnaryVariable>();
                List<APIDictionnaryVariable> listeVariablesAddedOrModified = new List<APIDictionnaryVariable>();
                foreach (var item in _listeVariables)
                {
                    //Si la variable est dans la liste des variables existantes, on la met à jour
                    APIDictionnaryVariable? itemfromapi = listeVariables.FirstOrDefault(x => x.Name == item.Name);
                    if (itemfromapi != null)
                    {
                        if (itemfromapi.Valeur != item.Valeur)
                        {
                            item.Valeur = itemfromapi.Valeur;
                            listeVariablesAddedOrModified.Add(item);
                        }
                        //Si on a modifié le type de variable, le baseoffset, le blockmemory ou le relativeoffset, c'est comme si on avait supprimé la variable
                        else if (itemfromapi.Variabletype != item.Variabletype || itemfromapi.Baseoffset != item.Baseoffset ||
                            itemfromapi.BlockMemory != item.BlockMemory || itemfromapi.RelativeOffset != item.RelativeOffset)
                        {
                            listeVariablesDeleted.Add(item);
                        }
                    }
                    else
                    {
                        listeVariablesDeleted.Add(item);
                    }
                }

                //On ajoute les variables qui n'existent pas dans la liste des variables existantes
                foreach (var item in listeVariables)
                {
                    if (!_listeVariables.Exists(x => x.Name == item.Name))
                    {
                        _listeVariables.Add(item);
                        listeVariablesAddedOrModified.Add(item);
                    }
                }
                //On supprime les variables qui n'existent plus
                foreach (var item in listeVariablesDeleted)
                {
                    _listeVariables.Remove(item);
                }
                //On déclenche l'évènement de mise à jour de la liste des variables
                APIListVariableUpdated?.Invoke(this, new ModbusAPIListVariableUpdatedEventArgs() {ListVariable=listeVariables, ListVariableAddedOrChanged = listeVariablesAddedOrModified, ListVariableRemoved = listeVariablesDeleted });
            }
            return (ret, listeVariables);
        }

        private bool FillListVariableValues(List<APIDictionnaryVariable> plisteVariables)
        {
            try
            {
                if (plisteVariables != null)
                {
                    List<APIDictionnaryVariable> _sortedlist = plisteVariables;
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
                                length = aPIDictionnaryVariable.VariableLength;
                                continue;
                            }
                            else
                            {
                                //sinon : on change de memoryblock => on envoie la requête pour récupérer les valeurs
                                (byte[] ret, bool ret2) = UmasReadVariablesFromMemoryBlocks((byte)memoryblock, minoffset, maxoffset - minoffset + length);

                                if (ret2 && ret != null && ret.Length > 0)
                                {

                                    List<APIDictionnaryVariable> _sortedblock = _sortedlist.Where(plisteVariables => plisteVariables.BlockMemory == memoryblock).ToList();
                                    foreach (var aPIDictionnaryVariablebis in _sortedblock)
                                    {
                                        byte[] tab = new byte[aPIDictionnaryVariablebis.VariableLength];
                                        Array.Copy(ret, aPIDictionnaryVariablebis.RelativeOffset - minoffset, tab, 0, aPIDictionnaryVariablebis.VariableLength);
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
                        if (memoryblock != -1)
                        {
                            (byte[] ret, bool ret2) = UmasReadVariablesFromMemoryBlocks((byte)memoryblock, minoffset, maxoffset - minoffset + length);
                            if (ret2 && ret != null && ret.Length > 0)
                            {

                                List<APIDictionnaryVariable> _sortedblock = _sortedlist.Where(plisteVariables => plisteVariables.BlockMemory == memoryblock).ToList();
                                foreach (var aPIDictionnaryVariablebis in _sortedblock)
                                {
                                    byte[] tab = new byte[aPIDictionnaryVariablebis.VariableLength];
                                    Array.Copy(ret.ToArray(), aPIDictionnaryVariablebis.RelativeOffset - minoffset, tab, 0, aPIDictionnaryVariablebis.VariableLength);
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
                        }
                    }

                }
            }
            catch (Exception)
            {

                return false;
            }

            return true;




        }



        /// <summary>
        /// Function to send an UmasRequestTo PLC and get the reponse back
        /// </summary>
        /// <param name="p_pairing_key"></param>
        /// <param name="pCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Span<byte> SendUmasRequest(byte p_pairing_key, ModbusUmasFunctionCode pCode, byte[]? data)
        {

            var unitIdentifier_converted = ConvertUnitIdentifier(_unitIdentifier);

            var buffer = TransceiveFrame(unitIdentifier_converted, ModbusFunctionCode.UmasCode, writer =>
            {
                writer.Write((byte)ModbusFunctionCode.UmasCode);                         // 0x5a (90) UMAS Function Code
                writer.Write(p_pairing_key);                                             // 0xXX Pairing key if reservation of PLC is necessary
                writer.Write((byte)pCode);                                               // 0xXX UMAS Function Code
                if (data != null)
                    writer.Write(data, 0, data.Length);                                     //  Datas
                UmasDataSent?.Invoke(this, new ModbusUmasDataSentEventArgs() { FunctionCode = pCode, Data = data });
            });

            UmasDataReceived?.Invoke(this, new ModbusUmasDataReceivedEventArgs() { Data = buffer.ToArray() });
            return buffer;


        }
        private bool SetVariablesValueFromREAD_SYSTEMBTISWORD_REQUEST(List<APIDictionnaryVariable> listevar)
        {

            //format de la requete:
            //0x0X => nombre de type de variables à lire suivi de la liste des variables à lire
            //<0xTYPEVALEUR> <0xBB1 0xBB2> <0x01> <0xOF1> <0xof2 0xOF3> <0xNBVALEURALIRE1 0xNBVALEURALIRE2> 
            if (listevar == null || listevar.Count == 0)
                return true;

            listevar.OrderBy(x => x.Variabletype).ThenBy(x => x.BlockMemory).ThenBy(x => x.RelativeOffset);

            List<DictionnaryVariableClassType> typevar = listevar.Select(x => x.Variabletype).Distinct().ToList();

            List<byte[]> datavar = new List<byte[]>();
            foreach (DictionnaryVariableClassType type in typevar)
            {
                List<APIDictionnaryVariable> listvar = listevar.FindAll(x => x.Variabletype == type);
                if (listvar.Count > 0)
                {
                    listevar.OrderBy(x => x.BlockMemory).ThenBy(x => x.Baseoffset).ThenBy(x => x.RelativeOffset);
                    //on prepare les tableaux de data a envoyer
                    byte[] data = new byte[7];
                    byte vartype = 0xFF;
                    switch (type)
                    {
                        /*case DictionnaryVariableClassType.BOOL:
                            test = 0x01;
                            break;
                        case DictionnaryVariableClassType.INT:
                           test = 0x04;
                            break;
                        case DictionnaryVariableClassType.UINT:
                           test = 0x05;
                            break;
                        case DictionnaryVariableClassType.DINT:
                           test = 0x06;
                            break;
                        case DictionnaryVariableClassType.UDINT:
                            test = 0x07;
                            break;
                        case DictionnaryVariableClassType.REAL:
                            test= 0x08;
                            break;
                        case DictionnaryVariableClassType.STRING:
                            test= 0x09;
                            break;
                        case DictionnaryVariableClassType.TIME:
                            test= 0x0a;
                            break;
                        case DictionnaryVariableClassType.DATE:
                            test= 0x0e;
                            break;
                        case DictionnaryVariableClassType.TOD:
                            test = 0x0f;
                            break;
                        case DictionnaryVariableClassType.DT:
                           test = 0x10;
                            break;
                        case DictionnaryVariableClassType.BYTE:
                            test = 0x15;
                            break;*/
                        case DictionnaryVariableClassType.WORD:
                            vartype = 0x02;
                            break;
                        /*case DictionnaryVariableClassType.DWORD:
                           test = 0x17;
                            break;*/
                        case DictionnaryVariableClassType.EBOOL:
                            vartype = 0x00;
                            break;
                        /*
                     case DictionnaryVariableClassType.CTU:
                        test = 0x1a;
                         break;*/
                        default:
                            return false;
                    }
                    data[0] = vartype;
                    Int16 blockMemory = listvar[0].BlockMemory;
                    Array.Copy(BitConverter.GetBytes(blockMemory), 0, data, 1, 2);
                    data[3] = 0x01;
                    Array.Copy(BitConverter.GetBytes((Int16)listvar[0].Baseoffset), 0, data, 4, 1);
                    data[6] = listevar[0].RelativeOffset;

                    datavar.Add(data);
                }
            }
            List<DictionnaryVariableClassType> listetype = new List<DictionnaryVariableClassType>();
            foreach (var item in listevar)
            {
                if (!listetype.Contains(item.Variabletype))
                    listetype.Add(item.Variabletype);
            }
            int numberoftype = listetype.Count;
            //int numberoftype = listevar.Select(x => x.Variabletype).Distinct().Count();
            if (_cRCShifted == null)
            {
                (bool ret, byte[]? crc, byte[]? crc_shifted) = SendUmas_READ_PLC_INFO();
                if (!ret || crc == null || crc_shifted == null)
                    return false;
                else
                {
                    _cRCFromPLC = crc;
                    _cRCShifted = crc_shifted;
                    APIInfoChanged?.Invoke(this, new EventArgs());
                }
            }


            int tailletotal = _cRCShifted.Length + 1 + datavar.Count * datavar[0].Length;

            byte[] datafinal = new byte[tailletotal];
            datafinal[4] = (byte)numberoftype;
            Array.Copy(_cRCShifted, 0, datafinal, 0, _cRCShifted.Length);
            int position = 1 + _cRCShifted.Length;
            foreach (byte[] data in datavar)
            {
                Array.Copy(data, 0, datafinal, position, data.Length);
                position += data.Length;
            }

            SendUmasRequest(_pairing_key, ModbusUmasFunctionCode.UMAS_READ_VARIABLES, datafinal);


            return true;
        }
        /// <summary>
        /// Send a READ_PLC_ID request to the PLC in order to get the PLC name and firmware version
        /// </summary>
        /// <returns>bool ok or not, String PLC Name, String FW Version</returns>
        private (bool, String, String) SendUmasREAD_PLC_ID()
        {
            String plcName = "";
            String fwVersion = "";
            Span<byte> retrequest = SendUmasRequest(_pairing_key, ModbusUmasFunctionCode.UMAS_READ_ID, null);
            if (retrequest.Length >= 3)
            {
                byte[] bytes = retrequest.ToArray();
                if (bytes[0] == (byte)ModbusFunctionCode.UmasCode && bytes[2] == (byte)ModbusUmasFunctionCode.UMAS_RET_OK_FROM_API)
                {
                    //on récupère le nom de la CPU
                    byte idlength = bytes[25];
                    // Extraire les bytes requis
                    byte[] subBytes = new byte[idlength];
                    Array.Copy(bytes, 26, subBytes, 0, idlength);

                    // Convertir les bytes en string en spécifiant l'encodage
                    _plcName = Encoding.UTF8.GetString(subBytes); // Vous pouvez changer l'encodage si nécessaire

                    //on récupère les infos sur le firmware
                    _fwversion = bytes[12].ToString("X2") + "." + bytes[11].ToString("X2");

                    return (true, plcName, fwVersion);
                }
            }
            return (false, plcName, fwVersion);
        }

        /// <summary>
        /// Function to write the inputs of a PLC to the memory blocks (code UMAS 0x21)
        /// </summary>
        /// <param name="memoryblock"></param>
        /// <param name="startoffset"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool UmasWriteInputWithMemoryBlocks(int memoryblock, int startoffset, byte[] data)
        {

            Span<byte> retrequest = Umas_Write_Memoryblock(memoryblock, startoffset, data.Length, data);

            if (retrequest.Length >= 3)
            {
                byte[] bytes = retrequest.ToArray();
                if (bytes[0] == (byte)ModbusFunctionCode.UmasCode && bytes[2] == (byte)ModbusUmasFunctionCode.UMAS_RET_OK_FROM_API)
                {
                    return true;
                }
            }
            return false;
        }

        private Span<byte> Umas_Write_Memoryblock(int pnumeroblock, int startoffset, int nbBytestoWrite, byte[] pdata)
        {
            byte[] data = new byte[9 + pdata.Length];
            data[0] = 0x01;
            BitConverter.GetBytes((Int16)pnumeroblock).CopyTo(data, 1); // Numero de BLOCK
            BitConverter.GetBytes((short)startoffset).CopyTo(data, 3); // Start OFFSET
            BitConverter.GetBytes((short)nbBytestoWrite).CopyTo(data, 7); // Nombre de bytes à écrire
            Array.Copy(pdata, 0, data, 9, pdata.Length); // data a ecrire
            Span<byte> buffer = SendUmasRequest(0, ModbusUmasFunctionCode.UMAS_WRITE_MEMORY_BLOCK, data);

            return buffer;
        }
        /// <summary>
        /// Function that send an Enable/Disable Dictionnary UMAS Request in order to get API Dictionnary Variables (0x26)
        /// And get liste API variables from dictionnary
        /// </summary>
        /// <returns>retourne une liste de variables API du dictionnaire</returns>
        private (List<APIDictionnaryVariable>, bool) SendUmas_GetDictionnaryVariables()
        {
            List<APIDictionnaryVariable> listeret = new List<APIDictionnaryVariable>();
            bool retbool = false;
            //Si on a pas le code CRC de l'API, on va le chercher

            if (_cRCShifted == null)
            {
                (bool ret, byte[]? crc, byte[]? crc_shifted) = SendUmas_READ_PLC_INFO();
                if (!ret || crc == null || crc_shifted == null)
                    return (listeret, false);
                else
                {
                    _cRCFromPLC = crc;
                    _cRCShifted = crc_shifted;

                    APIInfoChanged?.Invoke(this, new EventArgs());
                }
            }



            byte[] data = new byte[13];
            data[0] = 0x02;
            data[1] = 0xfb;
            data[2] = 0x03;
            if (_cRCFromPLC != null)
                Array.Copy(_cRCFromPLC, 0, data, 3, _cRCFromPLC.Length);
            data[7] = 0xFF;
            data[8] = 0xFF;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;
            data[12] = 0x00;
            Span<byte> retrequest = SendUmasRequest(0, ModbusUmasFunctionCode.UMAS_ENABLEDISABLE_DATADICTIONNARY, data);

            if (retrequest.Length >= 9)
            {

                if (retrequest[0] == (byte)ModbusFunctionCode.UmasCode && retrequest[2] == (byte)ModbusUmasFunctionCode.UMAS_RET_OK_FROM_API)
                {
                    byte[] rebytes = retrequest.ToArray();
                    Int16 nbelement = BitConverter.ToInt16(rebytes, 8);

                    //On supprime les 10 elements du tableau pour arriver sur 
                    if (rebytes.Length > 10)
                    {
                        Array.Copy(rebytes, 10, rebytes, 0, rebytes.Length - 10);
                        Array.Resize(ref rebytes, rebytes.Length - 10);
                    }
                    else
                        return (listeret, retbool);

                    int position = 0;
                    for (int i = 0; i < nbelement; i++)
                    {
                        if (position + 10 < rebytes.Length)
                        {
                            bool test = BitConverter.IsLittleEndian;
                            DictionnaryVariableClassType variabletype = (DictionnaryVariableClassType)rebytes[position];
                            position = position + 2;
                            Int16 blockMemory = BitConverter.ToInt16(rebytes, position);
                            position = position + 2;
                            byte relativeoffset = rebytes[position];
                            position = position + 1;
                            Int16 baseoffset = BitConverter.ToInt16(rebytes, position);
                            position = position + 5;

                            string nom = "";
                            while (position < rebytes.Length && rebytes[position] != 0x00)
                            {
                                nom += (char)rebytes[position];
                                position++;
                            }
                            //On est arrivé au dernier caractère => on augmente de 1 la position
                            position++;
                            if (nom != "")
                                listeret.Add(new APIDictionnaryVariable(nom, baseoffset, relativeoffset, blockMemory, variabletype));

                        }

                    }
                }
                else
                    return (listeret, retbool);
            }
            else
                return (listeret, retbool);


            retbool = true;
            return (listeret, retbool);
        }
        /// <summary>
        /// Function to read the a variable (ebool, adressed words of a PLC from the memory blocks value (code UMAS 0x20) 
        /// => can get block number and offset from Datadictionnary request if enabled (0x26), if not: maybe from 0x01 or 0x03 but cannot get it firgured out at this time...
        /// </summary>
        /// <param name="memoryblock"></param>
        /// <param name="startoffset"></param>
        /// <param name="nbBytestoRead"></param>
        /// <returns>bytes read in plc or empty</returns>
        private (byte[], bool) UmasReadVariablesFromMemoryBlocks(byte memoryblock, int startoffset, int nbBytestoRead)
        {
            Span<byte> ret = new Span<byte>();
            bool ret2 = false;
            try
            {

                Span<byte> retrequest = Umas_Read_Memoryblock(memoryblock, startoffset, nbBytestoRead);

                if (retrequest.Length >= 6)
                {
                    byte[] bytes = retrequest.ToArray();
                    if (bytes[0] == (byte)ModbusFunctionCode.UmasCode && bytes[2] == (byte)ModbusUmasFunctionCode.UMAS_RET_OK_FROM_API)
                    {
                        int size = BitConverter.ToInt16(bytes, 4);
                        if (bytes.Length >= 6 + size)
                            ret = new Span<byte>(bytes, 6, size);
                    }
                    else
                        return (ret.ToArray(), ret2 = false);
                }
                else
                    return (ret.ToArray(), ret2 = false);
            }
            catch (Exception)
            {

                return (ret.ToArray(), ret2 = false);
            }
            return (ret.ToArray(), ret2 = true);
        }
        /// <summary>
        /// Generate a UMAS request to read a memory block from the PLC
        /// </summary>
        /// <param name="pnumeroblock"></param>
        /// <param name="startoffset"></param>
        /// <param name="nbBytestoRead"></param>
        /// <returns></returns>
        private Span<byte> Umas_Read_Memoryblock(int pnumeroblock, int startoffset, int nbBytestoRead)
        {
            byte[] data = new byte[9];
            data[0] = 0x01;
            BitConverter.GetBytes((Int16)pnumeroblock).CopyTo(data, 1); // Numero de BLOCK
            BitConverter.GetBytes((short)startoffset).CopyTo(data, 3); // Start OFFSET
            BitConverter.GetBytes((short)nbBytestoRead).CopyTo(data, 7); // Nombre de bytes à lire

            Span<byte> buffer = SendUmasRequest(0, ModbusUmasFunctionCode.UMAS_READ_MEMORY_BLOCK, data);

            return buffer;
        }


        private Span<byte> UmasReadSystemCoilsAndRegisters(TypeInfoAPI pdatatype, int startoffset, int nbBytestoRead)
        {
            byte[] data = new byte[11];
            data[0] = 0x01;
            data[1] = 0x00;
            data[2] = (byte)pdatatype;//03 Holding Registers, 02 Coils
            BitConverter.GetBytes((Int32)startoffset).CopyTo(data, 4); // Start OFFSET
            BitConverter.GetBytes((short)nbBytestoRead).CopyTo(data, 8); // Nombre de bytes à lire

            Span<byte> buffer = SendUmasRequest(0, ModbusUmasFunctionCode.UMAS_READ_COILS_REGISTERS, data);

            return buffer;
        }
        /// <summary>
        /// //Initiate an UMAS connection with the PLC (0x01) => to use before PLC reservation
        /// </summary>
        /// <returns></returns>
        private bool SendUmas_InitCOMMRequest()
        {
            if (_umasRequestStarted)
                return false;

            SendUmasRequest(_pairing_key, ModbusUmasFunctionCode.UMAS_INIT_COMM, new byte[] { 0x00 });
            _umasRequestStarted = true;
            return true;
        }
        /// <summary>
        /// Send a 0X04 (READ_PLC_INFO) in order to get info from PLC and CRC byte[]
        /// </summary>
        /// <returns></returns>
        private (bool, byte[]?, byte[]?) SendUmas_READ_PLC_INFO()
        {
            Span<byte> retrequest = SendUmasRequest(_pairing_key, ModbusUmasFunctionCode.UMAS_READ_PLC_INFO, null);


            if (retrequest.Length >= 64)
            {
                byte[] bytes = retrequest.ToArray();
                if (bytes[0] == (byte)ModbusFunctionCode.UmasCode && bytes[2] == (byte)ModbusUmasFunctionCode.UMAS_RET_OK_FROM_API)
                {
                    try
                    {
                        _aPIState = (APIState)bytes[63];
                    }
                    catch (Exception)
                    {

                        _aPIState = APIState.Unknown;
                    }
                    byte[] cRCFromPLC = new byte[4];
                    byte[] cRCShifted = new byte[4];
                    //On récuprère le CRC qui est normalement à la 11ème position
                    Array.Copy(bytes, 11, cRCFromPLC, 0, 4);
                    //On le convertie en Int32 (LittleEndian)
                    Int32 crcbase = BitConverter.ToInt32(cRCFromPLC, 0);
                    //On shift le résultat obtenu de 1 bit vers la gauche
                    Int32 crcshifted = crcbase << 1;
                    //On reconvertie en tableau de byte LittleEndian
                    cRCShifted = BitConverter.GetBytes(crcshifted);
                    return (true, cRCFromPLC, cRCShifted);
                }
            }


            return (false, null, null);

        }

        private bool ProcessInitUmasResponse(Span<byte> data)
        {
            if (data.Length != 2)
                return false;

            if (data[0] != 0x00)
                return false;

            _pairing_key = data[1];
            return true;
        }

        private void StopUmasRequests()
        {
            _aPIState = APIState.Unknown;
            _cRCFromPLC= null;
            _cRCShifted = null;
            _fwversion = "";
            _plcName = "";

            _umasRequestStarted = false;
            _timerUmasSendCdG.Change(Timeout.Infinite, Timeout.Infinite);

        }

        private void _UmasTimerSendWdGRequest(object? state)
        {
            if (!IsConnected)
            {
                StopUmasRequests();
                return;
            }
            _timerUmasSendCdG.Change(Timeout.Infinite, Timeout.Infinite);
            bool update = false;
            if (_updateListVariableAuto)
            {
                _updateListVariableAutoCounter++;
                if (_updateListVariableAutoCounter >= _updateListVariableAutoInterval)
                {
                    _updateListVariableAutoCounter = 0;
                    update = true;
                }
            }
            try
            {

                if (GetPLCInfos())
                    MajListVariablesFromDataDictionnary(update);
            }
            catch (Exception ex)
            {

                APILog?.Invoke(this, new ModbusUmasAPILogEventArgs() { Log = ex.Message });
            }
            finally
            {
                _timerUmasSendCdG.Change(1000, 1000);
            }
        }


        /// <summary>
        /// Connect to localhost at port 502 with <see cref="ModbusEndianness.LittleEndian"/> as default byte layout.
        /// </summary>
        public new void Connect()
        {
            base.Connect();
        }

        /// <summary>
        /// Connect to localhost at port 502. 
        /// </summary>
        /// <param name="endianness">Specifies the endianness of the data exchanged with the Modbus server.</param>
        public new void Connect(ModbusEndianness endianness)
        {
            base.Connect(endianness);
            _timerUmasSendCdG.Change(0, 1000);
        }

        /// <summary>
        /// Connect to the specified <paramref name="remoteEndpoint"/>.
        /// </summary>
        /// <param name="remoteEndpoint">The IP address and optional port of the end unit with <see cref="ModbusEndianness.LittleEndian"/> as default byte layout. Examples: "192.168.0.1", "192.168.0.1:502", "::1", "[::1]:502". The default port is 502.</param>
        public new void Connect(string remoteEndpoint)
        {
            base.Connect(remoteEndpoint);
            _timerUmasSendCdG.Change(0, 1000);
        }

        /// <summary>
        /// Connect to the specified <paramref name="remoteEndpoint"/>.
        /// </summary>
        /// <param name="remoteEndpoint">The IP address and optional port of the end unit. Examples: "192.168.0.1", "192.168.0.1:502", "::1", "[::1]:502". The default port is 502.</param>
        /// <param name="endianness">Specifies the endianness of the data exchanged with the Modbus server.</param>
        public new void Connect(string remoteEndpoint, ModbusEndianness endianness)
        {
            base.Connect(remoteEndpoint, endianness);
            _timerUmasSendCdG.Change(0, 1000);
        }

        /// <summary>
        /// Connect to the specified <paramref name="remoteIpAddress"/> at port 502.
        /// </summary>
        /// <param name="remoteIpAddress">The IP address of the end unit with <see cref="ModbusEndianness.LittleEndian"/> as default byte layout. Example: IPAddress.Parse("192.168.0.1").</param>
        public new void Connect(IPAddress remoteIpAddress)
        {
            try
            {

                base.Connect(remoteIpAddress);
                _timerUmasSendCdG.Change(0, 1000);
            }
            catch (Exception ex)
            {

                APILog?.Invoke(this, new ModbusUmasAPILogEventArgs() { Log = ex.Message });
            }
        }

        /// <summary>
        /// Connect to the specified <paramref name="remoteIpAddress"/> at port 502.
        /// </summary>
        /// <param name="remoteIpAddress">The IP address of the end unit. Example: IPAddress.Parse("192.168.0.1").</param>
        /// <param name="endianness">Specifies the endianness of the data exchanged with the Modbus server.</param>
        public new void Connect(IPAddress remoteIpAddress, ModbusEndianness endianness)
        {
            base.Connect(remoteIpAddress, endianness);
            _timerUmasSendCdG.Change(0, 1000);
        }

        /// <summary>
        /// Connect to the specified <paramref name="remoteEndpoint"/> with <see cref="ModbusEndianness.LittleEndian"/> as default byte layout.
        /// </summary>
        /// <param name="remoteEndpoint">The IP address and port of the end unit.</param>
        public new void Connect(IPEndPoint remoteEndpoint)
        {
            base.Connect(remoteEndpoint);
            _timerUmasSendCdG.Change(0, 1000);
        }

        /// <summary>
        /// Connect to the specified <paramref name="remoteEndpoint"/>.
        /// </summary>
        /// <param name="remoteEndpoint">The IP address and port of the end unit.</param>
        /// <param name="endianness">Specifies the endianness of the data exchanged with the Modbus server.</param>
        public new void Connect(IPEndPoint remoteEndpoint, ModbusEndianness endianness)
        {
            base.Connect(remoteEndpoint, endianness);
            _timerUmasSendCdG.Change(0, 1000);
        }
        /// <summary>
        ///   Disconnect from the end unit.
        /// </summary>
        public new void Disconnect()
        {

            StopUmasRequests();
            base.Disconnect();


        }



        /// <summary>
        /// Close the connection to the end unit and release all resources.
        /// </summary>
        public new void Dispose()
        {
            if (IsConnected)
                Disconnect();
            StopUmasRequests();
            _timerUmasSendCdG.Dispose();
            base.Dispose();
        }

        private ushort ConvertSize<T>(ushort count)
        {
            var size = typeof(T) == typeof(bool) ? 1 : Marshal.SizeOf<T>();
            size = count * size;

            if (size % 2 != 0)
                throw new ArgumentOutOfRangeException("La quantité de donnée demandée doit être paire");

            var quantity = (ushort)(size / 2);

            return quantity;
        }

        private byte ConvertUnitIdentifier(int unitIdentifier)
        {
            if (!(0 <= unitIdentifier && unitIdentifier <= byte.MaxValue))
                throw new Exception("L'identifiant modbus n'est pas valide");

            return (byte)unitIdentifier;
        }

        private ushort ConvertUshort(int value)
        {
            if (!(0 <= value && value <= ushort.MaxValue))
                throw new Exception("valeur non compatible ushort");

            return (ushort)value;
        }





    }
}
