using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentModbusUmas.Umas
{
    public enum APIState
    {
        Stop = 0x01,
        Run = 0x02,
        Unknown = 0x03

    }


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

    /// <summary>
    /// Type of variable sent from the PLC to the client after a 0x26 request (Datadictionnary)
    /// </summary>
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



    public enum TypeInfoAPI
    {
        Coils = 2,
        HoldingRegisters = 3
    }

}
