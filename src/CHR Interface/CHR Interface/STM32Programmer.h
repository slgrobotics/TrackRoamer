#pragma once

using namespace System;
using namespace System::IO;
using namespace System::IO::Ports;
using namespace System::Windows::Forms;

#define		STM32_BUFFER_SIZE		1000
#define		MAX_HEX_ENTRIES			10000
#define		MAX_RECORD_LENGTH		255

// Return codes
#define		STM32_SUCCESS							1

#define		STM32_ERROR_SERIAL_CONNECTION_FAILED		2
#define		STM32_ERROR_UNABLE_TO_OPEN_PORT				3

#define		STM32_ERROR_NO_ACK_RECEIVED					4
#define		STM32_ERROR_UNRECOGNIZED_DEVICE				5
#define		STM32_ERROR_COMMUNICATION_LOST				6
#define		STM32_ERROR_READBACK_MISMATCH				7
#define		STM32_ERROR_INSUFFICIENT_FLASH_SPACE		8
#define		STM32_ERROR_DEVICE_UNAVAILABLE				9
#define		STM32_ERROR_HEXFILE_OPEN_FAILED				10
#define		STM32_ERROR_NACK_RECEIVED					11
#define		STM32_ERROR_INVALID_HEXFILE					12


delegate void FirmwareProgressEventHandler( int progress );
delegate void FirmwareStatusEventHandler( String^ status_text );

ref class STM32Programmer :
public System::IO::Ports::SerialPort
{
public:
	STM32Programmer(void);

	int OpenPort( String^ port_name, int baud_rate );

	int LoadFirmware( String^ hexFilePath );
	int ProgramDevice( );

	event FirmwareProgressEventHandler^ OnProgressUpdate;
	event FirmwareStatusEventHandler^ OnStatusChange;

	property double FirmwareSizeKB
	{
		double get()
		{
			return (double)FirmwareSize/1024.0;
		}
	}

private:
	int TransmitBytes( cli::array<unsigned char,1>^ data_array, int count );
	int ReadBytes( cli::array<unsigned char,1>^ data_array, int count );

	int Connect( void );
	int EraseMemory( int page_count, cli::array<unsigned char,1>^ pages );
	int GlobalErase( void );
	int ReadMemory( UInt32 address, Byte bytes_to_read, cli::array<unsigned char,1>^ data );
	int WriteMemory( UInt32 address, Byte bytes_to_write, cli::array<Byte,1>^ data );
	int Get( cli::array<unsigned char,1>^ data );
	int GetID( cli::array<unsigned char,1>^ data );

	int STM32Programmer::Cleanup( int returnval );

	cli::array<unsigned char,1>^ RXBuffer;
	int RXBufPtr;		// Points to the index in the serial buffer where the next item should be placed

	// Array of hex entry object
	cli::array<HexEntry^,1>^ HexEntries;
	int HexEntryCount;

	int FirmwareSize;

	int SerialState;
	StreamReader^ hexFile;
};
