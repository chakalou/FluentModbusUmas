using System;
using System.Collections.Generic;
using System.Text;

namespace FluentModbusUmas
{
    /// <summary>
    /// Specifies the UMAS action that client can send to control PLC
    /// </summary>
    public enum ModbusUmasFunctionCode : byte
    {
        // Existing entries...

        /// <summary>
        /// Code to Initialize a UMAS communication
        /// </summary>
        UMAS_INIT_COMM = 0x01,

        /// <summary>
        /// Code to Request a PLC ID
        /// </summary>
        UMAS_READ_ID = 0x02,

        /// <summary>
        /// Code to Read Project Information
        /// </summary>
        UMAS_READ_PROJECT_INFO = 0x03,

        /// <summary>
        /// Code to Get internal PLC Info
        /// </summary>
        UMAS_READ_PLC_INFO = 0x04,

        /// <summary>
        /// Code to Get internal PLC SD-Card Info
        /// </summary>
        UMAS_READ_CARD_INFO = 0x06,

        /// <summary>
        /// Code to Sends back data sent to PLC (used for synchronization)
        /// </summary>
        UMAS_REPEAT = 0x0A,

        /// <summary>
        /// Code to Assign an owner to the PLC
        /// </summary>
        UMAS_TAKE_PLC_RESERVATION = 0x10,

        /// <summary>
        /// Code to Release the reservation of a PLC
        /// </summary>
        UMAS_RELEASE_PLC_RESERVATION = 0x11,

        /// <summary>
        /// Code to Keep alive message
        /// </summary>
        UMAS_KEEP_ALIVE = 0x12,

        /// <summary>
        /// Code to Read a memory block of the PLC
        /// </summary>
        UMAS_READ_MEMORY_BLOCK = 0x20,
        /// <summary>
        /// Code to Write a memory block of the PLC
        /// </summary>
        UMAS_WRITE_MEMORY_BLOCK = 0x20,
        /// <summary>
        /// Code to Read system bits, system words and strategy variables
        /// </summary>
        UMAS_READ_VARIABLES = 0x22,

        /// <summary>
        /// Code to Write system bits, system words and strategy variables
        /// </summary>
        UMAS_WRITE_VARIABLES = 0x23,

        /// <summary>
        /// Code to Read coils and holding registers from PLC
        /// </summary>
        UMAS_READ_COILS_REGISTERS = 0x24,

        /// <summary>
        /// Code to Write coils and holding registers into PLC
        /// </summary>
        UMAS_WRITE_COILS_REGISTERS = 0x25,
        /// <summary>
        /// Code to Enable / Disable DataDictionnary
        /// </summary>
        UMAS_ENABLEDISABLE_DATADICTIONNARY = 0x26,
        /// <summary>
        /// Code to Initialize strategy upload (copy from PC to PLC)
        /// </summary>
        UMAS_INITIALIZE_UPLOAD = 0x30,

        /// <summary>
        /// Code to Upload a strategy block to the PLC
        /// </summary>
        UMAS_UPLOAD_BLOCK = 0x31,

        /// <summary>
        /// Code to Finish strategy upload
        /// </summary>
        UMAS_END_STRATEGY_UPLOAD = 0x32,

        /// <summary>
        /// Code to Initialize strategy download (copy from PLC to PC)
        /// </summary>
        UMAS_INITIALIZE_DOWNLOAD = 0x33,

        /// <summary>
        /// Code to Download a strategy block from the PLC
        /// </summary>
        UMAS_DOWNLOAD_BLOCK = 0x34,

        /// <summary>
        /// Code to Finish strategy download
        /// </summary>
        UMAS_END_STRATEGY_DOWNLOAD = 0x35,

        /// <summary>
        /// Code to Read Ethernet master data
        /// </summary>
        UMAS_READ_ETH_MASTER_DATA = 0x39,

        /// <summary>
        /// Code to Starts the PLC
        /// </summary>
        UMAS_START_PLC = 0x40,

        /// <summary>
        /// Code to Stops the PLC
        /// </summary>
        UMAS_STOP_PLC = 0x41,

        /// <summary>
        /// Code to Monitors variables, systems bits and words
        /// </summary>
        UMAS_MONITOR_PLC = 0x50,

        /// <summary>
        /// Code to Check PLC connection status
        /// </summary>
        UMAS_CHECK_PLC = 0x58,

        /// <summary>
        /// Code to Set a breakpoint on a specified rung
        /// </summary>
        UMAS_SET_BREAKPOINT = 0x60,

        /// <summary>
        /// Code to Read IO Object
        /// </summary>
        UMAS_READ_IO_OBJECT = 0x70,

        /// <summary>
        /// Code to Write IO Object
        /// </summary>
        UMAS_WRITE_IO_OBJECT = 0x71,

        /// <summary>
        /// Code to Get status module
        /// </summary>
        UMAS_GET_STATUS_MODULE = 0x73,

    /// <summary>
    /// Code from PLC for OK a request
    /// </summary>
    UMAS_RET_OK_FROM_API = 0xFE,
    ///<summary>
    ///Code from PLC for bad a request
    /// </summary>
    UMAS_RET_BAD_FROM_API = 0xFD
    }
}
