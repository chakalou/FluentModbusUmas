using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace FluentModbus
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
        /// Creates a new Modbus TCP UMAS client for communication with a schneider PLC.
        /// </summary>
        public ModbusUMASTcpClient() : base()
        {
            _timerUmasSendCdG = new Timer(_UmasTimerSendWdGRequest);
        }

        private Span<byte> SendUmasSimpleRequest(int unitIdentifier, byte p_pairing_key, ModbusUmasFunctionCode pCode, byte[] data)
        {

            var unitIdentifier_converted = ConvertUnitIdentifier(unitIdentifier);

            var buffer = TransceiveFrame(unitIdentifier_converted, ModbusFunctionCode.UmasCode, writer =>
            {
                writer.Write((byte)ModbusFunctionCode.UmasCode);                         // 0x5a (90) UMAS Function Code
                writer.Write(p_pairing_key);                                             // 0xXX Pairing key if reservation of PLC is necessary
                writer.Write((byte)pCode);                                               // 0xXX UMAS Function Code
                writer.Write(data, 0, data.Length);                                     //  Datas
                UmasDataSent?.Invoke(this, new ModbusUmasDataSentEventArgs() { FunctionCode = pCode, Data = data });
            });

            UmasDataReceived?.Invoke(this, new ModbusUmasDataReceivedEventArgs() { Data = buffer.ToArray() });
            return buffer;


        }

        /// <summary>
        /// //Initiate an UMAS connection with the PLC
        /// </summary>
        /// <param name="unitIdentifier">Adresse modbus du plc</param>
        /// <returns></returns>
        public bool InitUmasRequest(int unitIdentifier)
        {
            if (_umasRequestStarted)
                return false;

            SendUmasSimpleRequest(0, _pairing_key, ModbusUmasFunctionCode.UMAS_INIT_COMM, new byte[] { 0x00 });
            _umasRequestStarted = true;
            return true;
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
