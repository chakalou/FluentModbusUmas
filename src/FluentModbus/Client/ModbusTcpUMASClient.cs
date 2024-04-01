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
/// <summary>
/// Type of API to determine offset and communicate with the PLC
/// </summary>
public enum TypeAPI
{
    /// <summary>
    /// M340 PLC
    /// </summary>
    M340 = 0,
    /// <summary>
    /// M340 PLC
    /// </summary>
    M580 = 1,
    /// <summary>
    /// Simulated PLC
    /// </summary>
    PLCSIM = 2,
    /// <summary>
    /// Unknown PLC
    /// </summary>
    UNKNOWN = 3

}
public enum APIState
{
    Stop = 0x01,
    Run = 0x02,
    Unknown = 0x03

}

/*[enum uint 8 UmasDataType(uint 8 dataTypeSize, uint 8 requestSize)
    ['1' BOOL ['1','1']]
    ['2' UNKNOWN2 ['1','1']]
    ['3' UNKNOWN3 ['1','1']]
    ['4' INT ['2', '2']]
    ['5' UINT ['2','2']]
    ['6' DINT ['4','3']]
    ['7' UDINT ['4','3']]
    ['8' REAL ['4','3']]
    ['9' STRING ['1','17']]
    ['10' TIME ['4','3']]
    ['11' UNKNOWN11 ['1','1']]
    ['12' UNKNOWN12 ['1','1']]
    ['13' UNKNOWN13 ['1','1']]
    ['14' DATE ['4','3']]
    ['15' TOD ['4','3']]
    ['16' DT ['4','3']]
    ['17' UNKNOWN17 ['1','1']]
    ['18' UNKNOWN18 ['1','1']]
    ['19' UNKNOWN19 ['1','1']]
    ['20' UNKNOWN20 ['1','1']]
    ['21' BYTE ['1','1']]
    ['22' WORD ['2','2']]
    ['23' DWORD ['4','3']]
    ['24' UNKNOWN24 ['1','1']]
    ['25' EBOOL ['1','1']]
]*/
public enum DictionnaryVariableClassType
{
    BOOL = 0x01,
    INT = 0x04,
    UINT = 0x05,
    DINT = 0x06,
    UDINT = 0x07,
    REAL = 0x08,
    STRING = 0x09,
    TIME = 0x0a,
    DATE = 0x0e,
    TOD = 0x0f,
    DT = 0x10,
    BYTE = 0x15,
    WORD = 0x16,
    DWORD = 0x17,
    EBOOL = 0x19,
    CTU = 0x1a
}

/// <summary>
/// Description variable récupérée du dictionnaire de variable
/// </summary>
public class APIDictionnaryVariable
{
    String _name;
    DictionnaryVariableClassType _variabletype;
    Int16 _blockMemory;
    byte _relativeOffset;
    Int16 _baseoffset;
    /// <summary>
    /// Constructeur de APIDictionnaryVariable
    /// </summary>
    /// <param name="name">Nom de la variable</param>
    /// <param name="baseoffset">base offset associé à la variable</param>
    /// <param name="blockMemory">Block memory associé à la variable</param>
    /// <param name="relativeOffset">Offset relatif à la variable</param>
    /// <param name="variabletype">Type de variable</param>
    public APIDictionnaryVariable(string name, Int16 baseoffset, byte relativeOffset, Int16 blockMemory, DictionnaryVariableClassType variabletype)
    {
        _name = name;
        _variabletype = variabletype;
        _blockMemory = blockMemory;
        _baseoffset = baseoffset;
        _relativeOffset = relativeOffset;
    }
    /// <summary>
    /// Nom de la variable
    /// </summary>
    public string Name { get => _name; }

    /// <summary>
    /// Offset relatif à la variable
    /// </summary>
    public byte RelativeOffset { get => _relativeOffset; }
    /// <summary>
    /// Base offset assocé mémoire de la variable
    /// </summary>
    public Int16 Baseoffset { get => _baseoffset; }
    /// <summary>
    /// Bloc mémoire où lire la variable
    /// </summary>
    public Int16 BlockMemory { get => _blockMemory; }
    /// <summary>
    /// Type de variable API
    /// </summary>
    public DictionnaryVariableClassType Variabletype { get => _variabletype; }
}
public enum TypeInfoAPI
{
    Coils = 2,
    HoldingRegisters = 3
}



namespace FluentModbusUmas
{

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
    /// Modbus UMAS Event arguments triggered when data is received to the PLC
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
        String _fwversion;
        String _plcName = "";
        TypeAPI _typeAPI;
        APIState _aPIState = APIState.Unknown;
        public String PLCFWVersion { get { return _fwversion; } }
        public String PLCName { get { return _plcName; } }
        public String PLCState { get { return _aPIState.ToString(); } }
        byte[]? _cRCFromPLC;
        byte[]? _cRCShifted;
        /// <summary>
        /// Event triggered when data is sent to the PLC
        /// </summary>
        public event EventHandler<ModbusUmasDataSentEventArgs>? UmasDataSent;
        /// <summary>
        /// Event triggered when data is received from the PLC
        /// </summary>
        public event EventHandler<ModbusUmasDataReceivedEventArgs>? UmasDataReceived;
        Timer _timerUmasSendCdG;
        bool _umasRequestStarted = false;
        byte _pairing_key = 0x00;

        /// <summary>
        /// Retourne le
        /// </summary>
        public byte[]? CRCFromPLC { get { if (_cRCFromPLC == null) return null; else return _cRCFromPLC; } }
        public byte[]? CRCShiftedFromPLC { get { if (_cRCShifted == null) return null; else return _cRCShifted; } }
        /// <summary>
        /// Creates a new Modbus TCP UMAS client for communication with a schneider PLC.
        /// </summary>
        public ModbusUMASTcpClient() : base()
        {
            _timerUmasSendCdG = new Timer(_UmasTimerSendWdGRequest);
        }

        /// <summary>
        /// Function to send an UmasRequestTo PLC and get the reponse back
        /// </summary>
        /// <param name="unitIdentifier"></param>
        /// <param name="p_pairing_key"></param>
        /// <param name="pCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Span<byte> SendUmasRequest(int unitIdentifier, byte p_pairing_key, ModbusUmasFunctionCode pCode, byte[]? data)
        {

            var unitIdentifier_converted = ConvertUnitIdentifier(unitIdentifier);

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
        public bool SetVariablesValueFromREAD_SYSTEMBTISWORD_REQUEST(int unitIdentifier, List<APIDictionnaryVariable> listevar)
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
                    switch(type)
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
                if(!listetype.Contains(item.Variabletype))
                    listetype.Add(item.Variabletype);
            }
            int numberoftype=listetype.Count;
            //int numberoftype = listevar.Select(x => x.Variabletype).Distinct().Count();
            if(_cRCShifted==null && !SendUmas_READ_PLC_INFO(unitIdentifier))
                return false;

            int tailletotal= _cRCShifted.Length + 1 + datavar.Count * datavar[0].Length;

            byte[] datafinal = new byte[tailletotal];
            datafinal[4] = (byte)numberoftype;
            Array.Copy(_cRCShifted, 0, datafinal, 0, _cRCShifted.Length);
            int position = 1 + _cRCShifted.Length;
            foreach (byte[] data in datavar)
            {
                Array.Copy(data, 0, datafinal, position, data.Length);
                position += data.Length;
            }

            SendUmasRequest(unitIdentifier, _pairing_key, ModbusUmasFunctionCode.UMAS_READ_VARIABLES, datafinal);

      
            return true;
        }
        /// <summary>
        /// Send a READ_PLC_ID request to the PLC in order to get the PLC name and firmware version
        /// </summary>
        /// <param name="unitIdentifier"></param>
        /// <param name="pairingkey"></param>
        /// <returns></returns>
        public bool SendUmasREAD_PLC_ID(int unitIdentifier, byte pairingkey)
        {

            Span<byte> retrequest = SendUmasRequest(unitIdentifier, pairingkey, ModbusUmasFunctionCode.UMAS_READ_ID, null);
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

                    return SendUmas_READ_PLC_INFO(unitIdentifier);
                }
            }
            return false;
        }

        /// <summary>
        /// Function to write the inputs of a PLC to the memory blocks (code UMAS 0x21)
        /// </summary>
        /// <param name="memoryblock"></param>
        /// <param name="unitIdentifier"></param>
        /// <param name="startoffset"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool UmasWriteInputWithMemoryBlocks( int unitIdentifier, int memoryblock, int startoffset, byte[] data)
        {
          
            Span<byte> retrequest = Umas_Write_Memoryblock(unitIdentifier, memoryblock, startoffset, data.Length, data);

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

        private Span<byte> Umas_Write_Memoryblock(int unitIdentifier, int pnumeroblock, int startoffset, int nbBytestoWrite, byte[] pdata)
        {
            byte[] data = new byte[9 + pdata.Length];
            data[0] = 0x01;
            BitConverter.GetBytes((Int16)pnumeroblock).CopyTo(data, 1); // Numero de BLOCK
            BitConverter.GetBytes((short)startoffset).CopyTo(data, 3); // Start OFFSET
            BitConverter.GetBytes((short)nbBytestoWrite).CopyTo(data, 7); // Nombre de bytes à écrire
            Array.Copy(pdata, 0, data, 9, pdata.Length); // data a ecrire
            Span<byte> buffer = SendUmasRequest(unitIdentifier, 0, ModbusUmasFunctionCode.UMAS_WRITE_MEMORY_BLOCK, data);

            return buffer;
        }
        /// <summary>
        /// Function that send an Enable/Disable Dictionnary UMAS Request in order to get API Dictionnary Variables (0x26)
        /// And get liste API variables from dictionnary
        /// </summary>
        /// <param name="unitIdentifier"></param>
        /// <param name="pApi"></param>
        /// <returns>retourne une liste de variables API du dictionnaire</returns>
        public List<APIDictionnaryVariable> SendUmas_GetDictionnaryVariables(int unitIdentifier, TypeAPI pApi)
        {
            List<APIDictionnaryVariable> listeret = new List<APIDictionnaryVariable>();
            //Si on a pas le code CRC de l'API, on va le chercher
            if (_cRCFromPLC == null)
                SendUmas_READ_PLC_INFO(unitIdentifier);



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
            Span<byte> retrequest = SendUmasRequest(unitIdentifier, 0, ModbusUmasFunctionCode.UMAS_ENABLEDISABLE_DATADICTIONNARY, data);

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

                    int position = 0;
                    for (int i = 0; i < nbelement; i++)
                    {
                        if (position + 8 < rebytes.Length)
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
            }



            return listeret;
        }
        /// <summary>
        /// Function to read the a variable (ebool, adressed words of a PLC from the memory blocks value (code UMAS 0x20) 
        /// => can get block number and offset from Datadictionnary request if enabled (0x26), if not: maybe from 0x01 or 0x03 but cannot get it firgured out at this time...
        /// </summary>
        /// <param name="unitIdentifier"></param>
        /// <param name="memoryblock"></param>
        /// <param name="startoffset"></param>
        /// <param name="nbBytestoRead"></param>
        /// <returns>bytes read in plc or empty</returns>
        public Span<byte> UmasReadVariablesFromMemoryBlocks(/*TypeAPI api,*/ int unitIdentifier, byte memoryblock, int startoffset, int nbBytestoRead)
        {
            Span<byte> ret = new Span<byte>();
           
            Span<byte> retrequest = Umas_Read_Memoryblock(unitIdentifier, memoryblock, startoffset, nbBytestoRead);

            if (retrequest.Length >= 6)
            {
                byte[] bytes = retrequest.ToArray();
                if (bytes[0] == (byte)ModbusFunctionCode.UmasCode && bytes[2] == (byte)ModbusUmasFunctionCode.UMAS_RET_OK_FROM_API)
                {
                    int size = BitConverter.ToInt16(bytes, 4);
                    if (bytes.Length >= 6 + size)
                        ret = new Span<byte>(bytes, 6, size);
                }
            }
            return ret;
        }
        /// <summary>
        /// Generate a UMAS request to read a memory block from the PLC
        /// </summary>
        /// <param name="unitIdentifier"></param>
        /// <param name="pnumeroblock"></param>
        /// <param name="startoffset"></param>
        /// <param name="nbBytestoRead"></param>
        /// <returns></returns>
        private Span<byte> Umas_Read_Memoryblock(int unitIdentifier, int pnumeroblock, int startoffset, int nbBytestoRead)
        {
            byte[] data = new byte[9];
            data[0] = 0x01;
            BitConverter.GetBytes((Int16)pnumeroblock).CopyTo(data, 1); // Numero de BLOCK
            BitConverter.GetBytes((short)startoffset).CopyTo(data, 3); // Start OFFSET
            BitConverter.GetBytes((short)nbBytestoRead).CopyTo(data, 7); // Nombre de bytes à lire

            Span<byte> buffer = SendUmasRequest(unitIdentifier, 0, ModbusUmasFunctionCode.UMAS_READ_MEMORY_BLOCK, data);

            return buffer;
        }

 
        private Span<byte> UmasReadSystemCoilsAndRegisters(int unitIdentifier, TypeInfoAPI pdatatype, int startoffset, int nbBytestoRead)
        {
            byte[] data = new byte[11];
            data[0] = 0x01;
            data[1] = 0x00;
            data[2] = (byte)pdatatype;//03 Holding Registers, 02 Coils
            BitConverter.GetBytes((Int32)startoffset).CopyTo(data, 4); // Start OFFSET
            BitConverter.GetBytes((short)nbBytestoRead).CopyTo(data, 8); // Nombre de bytes à lire

            Span<byte> buffer = SendUmasRequest(unitIdentifier, 0, ModbusUmasFunctionCode.UMAS_READ_COILS_REGISTERS, data);

            return buffer;
        }
        /// <summary>
        /// //Initiate an UMAS connection with the PLC (0x01) => to use before PLC reservation
        /// </summary>
        /// <param name="unitIdentifier">Adresse modbus du plc</param>
        /// <returns></returns>
        private bool InitUmasRequest(int unitIdentifier)
        {
            if (_umasRequestStarted)
                return false;

            SendUmasRequest(0, _pairing_key, ModbusUmasFunctionCode.UMAS_INIT_COMM, new byte[] { 0x00 });
            _umasRequestStarted = true;
            return true;
        }
        /// <summary>
        /// Send a 0X04 (READ_PLC_INFO) in order to get info from PLC and CRC byte[]
        /// </summary>
        /// <returns></returns>
        public bool SendUmas_READ_PLC_INFO(int unitIdentifier)
        {
            Span<byte> retrequest = SendUmasRequest(unitIdentifier, _pairing_key, ModbusUmasFunctionCode.UMAS_READ_PLC_INFO, null);


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
                    _cRCFromPLC = new byte[4];
                    _cRCShifted = new byte[4];
                    //On récuprère le CRC qui est normalement à la 11ème position
                    Array.Copy(bytes, 11, _cRCFromPLC, 0, 4);
                    //On le convertie en Int32 (LittleEndian)
                    Int32 crcbase = BitConverter.ToInt32(_cRCFromPLC, 0);
                    //On shift le résultat obtenu de 1 bit vers la gauche
                    Int32 crcshifted = crcbase << 1;
                    //On reconvertie en tableau de byte LittleEndian
                    _cRCShifted = BitConverter.GetBytes(crcshifted);
                    return true;
                }
            }

            _cRCFromPLC = null;
            _cRCShifted = null;

            return false;

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
            _umasRequestStarted = false;

        }

        private void _UmasTimerSendWdGRequest(object? state)
        {
            if (!IsConnected)
                return;
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
        }

        /// <summary>
        /// Connect to the specified <paramref name="remoteEndpoint"/>.
        /// </summary>
        /// <param name="remoteEndpoint">The IP address and optional port of the end unit with <see cref="ModbusEndianness.LittleEndian"/> as default byte layout. Examples: "192.168.0.1", "192.168.0.1:502", "::1", "[::1]:502". The default port is 502.</param>
        public new void Connect(string remoteEndpoint)
        {
            base.Connect(remoteEndpoint);
        }

        /// <summary>
        /// Connect to the specified <paramref name="remoteEndpoint"/>.
        /// </summary>
        /// <param name="remoteEndpoint">The IP address and optional port of the end unit. Examples: "192.168.0.1", "192.168.0.1:502", "::1", "[::1]:502". The default port is 502.</param>
        /// <param name="endianness">Specifies the endianness of the data exchanged with the Modbus server.</param>
        public new void Connect(string remoteEndpoint, ModbusEndianness endianness)
        {
            base.Connect(remoteEndpoint, endianness);
        }

        /// <summary>
        /// Connect to the specified <paramref name="remoteIpAddress"/> at port 502.
        /// </summary>
        /// <param name="remoteIpAddress">The IP address of the end unit with <see cref="ModbusEndianness.LittleEndian"/> as default byte layout. Example: IPAddress.Parse("192.168.0.1").</param>
        public new void Connect(IPAddress remoteIpAddress)
        {
            base.Connect(remoteIpAddress);
        }

        /// <summary>
        /// Connect to the specified <paramref name="remoteIpAddress"/> at port 502.
        /// </summary>
        /// <param name="remoteIpAddress">The IP address of the end unit. Example: IPAddress.Parse("192.168.0.1").</param>
        /// <param name="endianness">Specifies the endianness of the data exchanged with the Modbus server.</param>
        public new void Connect(IPAddress remoteIpAddress, ModbusEndianness endianness)
        {
            base.Connect(remoteIpAddress, endianness);
        }

        /// <summary>
        /// Connect to the specified <paramref name="remoteEndpoint"/> with <see cref="ModbusEndianness.LittleEndian"/> as default byte layout.
        /// </summary>
        /// <param name="remoteEndpoint">The IP address and port of the end unit.</param>
        public new void Connect(IPEndPoint remoteEndpoint)
        {
            base.Connect(remoteEndpoint);
        }

        /// <summary>
        /// Connect to the specified <paramref name="remoteEndpoint"/>.
        /// </summary>
        /// <param name="remoteEndpoint">The IP address and port of the end unit.</param>
        /// <param name="endianness">Specifies the endianness of the data exchanged with the Modbus server.</param>
        public new void Connect(IPEndPoint remoteEndpoint, ModbusEndianness endianness)
        {
            base.Connect(remoteEndpoint, endianness);
        }
        /// <summary>
        ///   Disconnect from the end unit.
        /// </summary>
        public new void Disconnect()
        {
            _timerUmasSendCdG.Change(Timeout.Infinite, Timeout.Infinite);
            StopUmasRequests();
            base.Disconnect();


        }



        /// <summary>
        /// Close the connection to the end unit and release all resources.
        /// </summary>
        public new void Dispose()
        {
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
