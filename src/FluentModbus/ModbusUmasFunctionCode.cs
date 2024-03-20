using System;
using System.Collections.Generic;
using System.Text;

namespace FluentModbus
{
    /// <summary>
    /// Specifies the UMAS action that client can send to control PLC
    /// </summary>
    public enum ModbusUmasFunctionCode : byte
    {
        //Application download to the PLC
        /// <summary>
        /// Code to Initiate the download of an application to the PLC
        /// </summary>
        UMAS_BEGIN_DOWNLOAD = 0x30,
        /// <summary>
        /// Code to copy a strategy block of application from engineering station to the PLC
        /// </summary>
        UMAS_DOWNLOAD = 0x31,
        /// <summary>
        /// Code to End the download of an application to the PLC
        /// </summary>
        UMAS_END_DOWNLOAD = 0x32,
        //Application Management
        /// <summary>
        /// Code to Read Ethernet Master Data
        /// </summary>
        UMAS_TDA = 0x39,
        /// <summary>
        /// Code to Monitor PLC (variables, system bits and words)
        /// </summary>
        UMAS_CSA = 0x50,
        //Application upload from the PLC
        /// <summary>
        /// Code to Initiate the upload of an application from the PLC to engeenering station
        /// </summary>
        UMAS_BEGIN_UPLOAD = 0x33,
        /// <summary>
        /// Code to copy a strategy block of application from the PLC to engineering station
        /// </summary>
        UMAS_UPLOAD = 0x34,
        /// <summary>
        /// Code to End the download of an application from the PLC to engeenering station
        /// </summary>
        UMAS_END_UPLOAD = 0x35,
        /// <summary>
        /// Code to Backup or Restore the PLC
        /// </summary>
        UMAS_END_BACKUP_RESTORE = 0x36,
        //Configuration Information requests
        /// <summary>
        /// Code to Request a PLC ID
        /// </summary>
        UMAS_GET_PLC_INFO = 0x02,
        /// <summary>
        /// Code to Read IO Object
        /// </summary>
        UMAS_READ_IO_OBJECT = 0x70,
        /// <summary>
        /// Code to Read Rack
        /// </summary>
        UMAS_READ_RACK = 0x72,
        /// <summary>
        /// Code to Get Status Module
        /// </summary>
        UMAS_READ_MODULE = 0x73,
        //Connection Information requests
        /// <summary>
        /// Code to Initiate an UMAS connection with the PLC
        /// </summary>
        UMAS_INIT_COMM = 0x01,
        //Debugging
        /// <summary>
        /// Code to Set a Breakpoint
        /// </summary>
        UMAS_SET_BREAKPOINT = 0x60,
        /// <summary>
        /// Code to Delete a Breakpoint
        /// </summary>
        UMAS_DELETE_BREAKPOINT = 0x61,
        /// <summary>
        /// Code to Set Over a function
        /// </summary>
        UMAS_SETOVER = 0x62,
        /// <summary>
        /// Code to Set In a function
        /// </summary>
        UMAS_SETIN = 0x63,
        /// <summary>
        /// Code to Set Out a function
        /// </summary>
        UMAS_SETOUT = 0x64,
        /// <summary>
        /// Code to Go to a line
        /// </summary>
        UMAS_GOTO = 0x66,
        /// <summary>
        /// ????
        /// </summary>
        UMAS_PUTRC = 0x6C,
        /// <summary>
        /// ????
        /// </summary>
        UMAS_PRIVATE = 0x6D,
        //Plc status commands
        /// <summary>
        /// Code to run the PLC
        /// </summary>
        UMAS_RUN = 0x40, //Start the PLC
        /// <summary>
        /// Code to stop the PLC
        /// </summary>
        UMAS_STOP = 0x41, //Stop the PLC
        /// <summary>
        /// Code to initialize the PLC
        /// </summary>
        UMAS_INIT = 0x42,
        //plc status requests
        /// <summary>
        /// Code to Read the PLC project information
        /// </summary>
        UMAS_GET_APPLI_INFO = 0x03, //Read Project Information
        /// <summary>
        /// Code to get PLC internal information
        /// </summary>
        UMAS_GET_PLC_STATUS = 0x04, //Get internal PLC Information
        /// <summary>
        /// Code to get PLC loader information
        /// </summary>
        UMAS_GET_LOADER_INFO = 0x05,
        /// <summary>
        /// Code to get PLC SD-Card information
        /// </summary>
        UMAS_GET_MEMORYCARD_INFO = 0x06, //Get internal PLC SD-Card Information
        /// <summary>
        /// Code to get PLC block information
        /// </summary>
        UMAS_GET_BLOCK_INFO = 0x07,
        //Read commands
        /// <summary>
        /// Code to Read a memory block
        /// </summary>
        UMAS_READ_MEMORYBLOCK = 0x20,
        /// <summary>
        /// Code to read a BOL
        /// </summary>
        UMAS_READ_BOL = 0x22,
        /// <summary>
        /// Code to read a variable list
        /// </summary>
        UMAS_READ_VAR_LIST = 0x24,
        /// <summary>
        /// Code to Activate or disactivate Data Dictionnary
        /// </summary>
        UMAS_DICTIONNARY = 0x26,
        /// <summary>
        /// Another code to Activate or disactivate Data Dictionnary
        /// </summary>
        UMAS_DICTIONNARY2 = 0x27,
        /// <summary>
        /// Code to read a physical address
        /// </summary>
        UMAS_READ_PHYSICAL_ADDRESS = 0x28,
        //Reservation Requests
        /// <summary>
        /// Code to take a reservation on the PLC
        /// </summary>
        UMAS_TAKE_PLC_RESERVATION = 0x10,
        /// <summary>
        /// Code to release a reservation on the PLC
        /// </summary>
        UMAS_RELEASE_PLC_RESERVATION = 0x11,
        /// <summary>
        /// Code to keep a reservation on the PLC
        /// </summary>
        UMAS_KEEP_PLC_RESERVATION = 0x12,
        //Write commands
        /// <summary>
        /// Code to write a memory block
        /// </summary>
        UMAS_WRITE_MEMORYBLOCK = 0x21,
        /// <summary>
        /// Code to write a BOL
        /// </summary>
        UMAS_WRITE_BOL = 0x23,
        /// <summary>
        /// Code to write a variable list
        /// </summary>
        UMAS_WRITE_VAR_LIST = 0x25,
        /// <summary>
        /// Code to write a physical address
        /// </summary>
        UMAS_WRITE_PHYSICAL_ADDRESS = 0x29,
        /// <summary>
        /// Code to preload blocks
        /// </summary>
        UMAS_PRELOAD_BLOCKS = 0x37,
        /// <summary>
        /// Code to write a IO Object
        /// </summary>
        UMAS_WRITE_IO_OBJECT = 0x71, //WriteIO Object
        /// <summary>
        /// Code from PLC for OK a request
        /// </summary>
        UMAS_RET_OK_FROM_API = 0xFE
    }
}
